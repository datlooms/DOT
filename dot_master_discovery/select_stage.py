"""select_stage.py — THE `SELECT` STAGE. DATA IN, SIGNALS AND SCORE OUT.

    python master.py --data data --workers 14 --out discovery\\full --stage SELECT

The adopted 297-signal book exists as an artefact of several days of conversation and
cannot be regenerated from data. This stage closes that: it screens the raw F0 scan on
TRAIN-WINDOW statistics, draws nested arms at six sizes, scores every one through the
same engine and the same metrics block S8 uses, and emits the `book_size` arm as a book
usable directly by `--stage S8 --book`.

WHAT THIS STAGE DOES NOT DO. It does not rank signals. Three measured facts forbid it:
a rebuilt ranking selector LOST TO 12 OF 12 random draws on a holdout; 297 leave-one-out
runs found split-half rho = -0.060 at p = 0.305 with d_events moving by zero for 212 of
297; and 121 of 973 entry bars sit at depth EXACTLY 3, so a member's contribution is
largely how often it happens to be the marginal third signal. THE BOOK IS HOMOGENEOUS -
there is nothing to prune and nothing to rank. Size is a config parameter, and a seeded
draw is a size parameter rather than a ranking.

AND THE EXPECTED RESULT IS PROBABLY NOT THE 297. The incumbent sits at roughly the 4th
percentile of random draws and nothing in its members explains why. If every arm lands
inside its own size-relative band, THAT IS THE FINDING, and section 7 requires the stage
to report it as one rather than leave it to be rediscovered as a bug at 2am.
"""

import hashlib
import json
import os

import numpy as np
import pandas as pd

TRAIN_EXCLUDE_MONTHS = 1
ARM_SIZES = (150, 200, 297, 500, 1000)
SEED = 0
_SEL_ROOT = os.path.dirname(os.path.abspath(__file__))

# Section 7. Measured on the 4,575 PRE-CORRECTION pool, so every range is a FLOOR:
# the corrected screen admits signals that failed the full-sample filter, and a draw
# from the larger pool should land AT OR WORSE.
RANDOM_BASELINE = (
    (150, '21 - 27', '6.67-8.38', '27.2 - 28.5', '84 -  90', 3),
    (200, '35 - 49', '5.73-7.28', '26.3 - 29.0', '97 - 105', 3),
    (297, '84 - 95', '4.32-5.06', '21.6 - 24.6', '113 - 119', 8),
    (500, '184 - 185', '3.53-3.85', '19.1 - 19.5', '122 - 125', 2),
    (1000, '368 - 387', '2.57-2.89', '13.7 - 15.1', '125 - 126', 2),
)
INCUMBENT = {'signals': 297, 'trades': 5776, 'WR': 96.12, 'PF': 14.53, 'net': 284974.00,
             'events': 42, 'event_days': 35, 'worst_day': -346.60, 'losing_weeks': 0,
             'weeks': 26, 'days': 119, 'days_frame': 132, 'margin': 33.07}


def _sha(path):
    h = hashlib.sha256()
    with open(path, 'rb') as f:
        for blk in iter(lambda: f.read(1 << 20), b''):
            h.update(blk)
    return h.hexdigest()[:12]


def train_window(df, exclude_months=TRAIN_EXCLUDE_MONTHS):
    """TRAIN IS A RULE, NOT A DATE: 'all but the final N months'.

    A hardcoded date silently changes the screen's meaning every month the stage runs,
    which for a monthly stage is the whole problem.
    """
    mo = pd.Series(df['Time'].astype(str).values).str[:7]
    months = sorted(mo.unique())
    if len(months) <= exclude_months:
        raise SystemExit(f'ABORT [SELECT] frame spans {len(months)} months; cannot exclude '
                         f'{exclude_months} and leave a train window.')
    train_months = months[:-exclude_months] if exclude_months else months
    return train_months, months[-exclude_months:] if exclude_months else []


def screen_row(net_by_month, train_months, min_trades=12, min_pf=2.0,
               min_buckets=3, min_profit_frac=2.0 / 3.0):
    """The four criteria, TRAIN-WINDOW ONLY. Returns (passed, reason, stats).

    A PROPORTION, NOT A COUNT. `folds_plus >= 4` as a count is unsatisfiable for 503
    signals that lack four buckets, and that defect has already cost a turn.
    """
    present = [m for m in train_months if m in net_by_month and net_by_month[m]['trades'] > 0]
    trades = sum(net_by_month[m]['trades'] for m in present)
    wins = sum(net_by_month[m]['wins'] for m in present)
    gross_w = sum(net_by_month[m]['gross_win'] for m in present)
    gross_l = sum(net_by_month[m]['gross_loss'] for m in present)
    pf = (gross_w / gross_l) if gross_l > 0 else (float('inf') if gross_w > 0 else 0.0)
    prof = [m for m in present if net_by_month[m]['net'] > 0]
    frac = (len(prof) / len(present)) if present else 0.0
    stats = {'train_trades': int(trades), 'train_wins': int(wins),
             'train_PF': ('inf' if pf == float('inf') else round(float(pf), 4)),
             'buckets_present': len(present), 'buckets_profitable': len(prof),
             'bucket_profit_frac': round(frac, 4),
             'train_net': round(float(sum(net_by_month[m]['net'] for m in present)), 2)}
    if trades < min_trades:
        return False, f'trades {trades} < {min_trades}', stats
    if not (pf >= min_pf):
        return False, f'train_PF {stats["train_PF"]} < {min_pf}', stats
    if len(present) < min_buckets:
        return False, f'buckets_present {len(present)} < {min_buckets}', stats
    if frac < min_profit_frac:
        return False, (f'bucket_profit_frac {frac:.3f} < {min_profit_frac:.3f} '
                       f'({len(prof)}/{len(present)})'), stats
    return True, '', stats


def stable_survivors(rows):
    """SORT BY A STABLE KEY, NEVER A FLOAT.

    rng.choice is reproducible only if the population is in a stable order. A set, a
    dict, a groupby on parallel results or concatenated worker output can all reorder,
    and the same seed then selects different signals at different worker counts -
    determinism satisfied in appearance and violated in fact.
    """
    return sorted(rows, key=lambda r: (str(r['signal_def']), str(r['direction'])))


def nested_arms(n_survivors, sizes, seed=SEED):
    """ONE PERMUTATION, TAKE PREFIXES. Draw-to-draw variation is larger than the size
    effect at small N - 21 to 27 events at N=150, 35 to 49 at N=200 - so independent
    draws confound size with draw luck and the sweep becomes unreadable.
    """
    rng = np.random.default_rng(seed)
    order = rng.permutation(n_survivors)
    out = {}
    for n in sizes:
        if n > n_survivors:
            continue
        out[n] = sorted(order[:n].tolist())
    out[n_survivors] = sorted(order.tolist())
    return out, order


def survivor_fingerprint(survivors):
    """Asserted identical across worker counts BEFORE drawing."""
    key = '\n'.join(f'{r["signal_def"]}|{r["direction"]}' for r in survivors)
    return hashlib.sha256(key.encode()).hexdigest()[:16]


_SCR_CTX = {}


def _scr_init(frame_path):
    """Oracle, pool and conviction built ONCE PER WORKER, not per signal."""
    import sys as _s
    for _d in ('engine', 'scanners', 'orchestrator', '.'):
        _p = os.path.join(_SEL_ROOT, _d)
        if _p not in _s.path:
            _s.path.insert(0, _p)
    import pandas as _pd
    import dots_thresholds as _dt
    import portfolio_simulation_engine as _eng
    import sequential_temporal as _seq
    import conviction as _C
    df = _pd.read_csv(frame_path)
    w = _eng.warmup_floor(df, verbose=False)
    ad = _dt.compute_adaptive_thresholds(df)
    st = _dt.compute_structural_gates(df)
    _SCR_CTX.update({'df': df, 'w': w, 'ad': ad, 'st': st,
                     'pool': _seq.build_condition_pool(df, ad, st, w),
                     'anchor': _seq.anchor_array(df, 'ST_Flip'),
                     'conv': _C.build_conviction(df, True, True, True, d2d_conviction=True,
                                                 d2d_gap=True),
                     'mo': _pd.Series(df['Time'].astype(str).values).str[:7].values})


def _scr_chunk(payload):
    """Solo-run a chunk of signals and return the PER-MONTH partition.

    ROUTE A, and the choice is recorded here rather than in a comment elsewhere:
    S3 is NOT being touched - section 4 is satisfied by asserting the scan and frame
    shas match rather than by invoking S3 - so route B's precondition ('prefer if S3
    is being touched anyway') does not hold, and modifying the 13-hour scan stage to
    serve this screen would be a far larger change than the screen itself.

    AND THE MEASURED COST MAKES IT AFFORDABLE: 0.149s per signal here, so 19,754 rows
    is ~49 minutes serial and ~4 minutes at 14 workers - against the spec's ~62 minute
    estimate. Recurring, but not the 62-minute monthly tax route B was avoiding.
    """
    idx, items = payload
    import pandas as _pd
    import numpy as _np
    import portfolio_simulation_engine as _eng
    import score_g as _sg
    c = _SCR_CTX
    out = []
    for sd, dr in items:
        one = _pd.DataFrame([{'trigger': 'F0', 'direction': dr, 'signal_def': sd}])
        try:
            book = _sg.build_book(c['df'], c['pool'], c['anchor'], one,
                                  adaptive=c['ad'], structural=c['st'])
            td = _eng.run_portfolio(c['df'], book, adaptive=c['ad'], structural=c['st'],
                                    warmup=c['w'], verbose=False, conviction=c['conv'])
        except (Exception, SystemExit):
            out.append((sd, dr, {}))
            continue
        # BOOK-ONLY. Conviction gap fillers (GAP_HURST/GAP_FB/GAP_D2D) are a separate
        # population and are attributed to no signal - counting them per signal made
        # every sampled row clear the screen, 300 of 300, because each inherited the
        # gap fillers' trades and profit factor. The same population distinction the
        # S8 scorecard already draws.
        import cluster_profiler as _cp
        td = td[~td['signal_name'].isin(_cp.GAP_NAMES)]
        if not len(td):
            out.append((sd, dr, {}))
            continue
        m = _pd.Series(td['exit_time'].astype(str).values).str[:7].values
        p = _np.asarray(td['pnl'].values, dtype=float)
        per = {}
        for mm in sorted(set(m.tolist())):
            sel = (m == mm)
            pp = p[sel]
            per[mm] = {'trades': int(pp.size), 'wins': int((pp > 0).sum()),
                       'net': float(pp.sum()),
                       'gross_win': float(pp[pp > 0].sum()),
                       'gross_loss': float(-pp[pp < 0].sum())}
        out.append((sd, dr, per))
    return idx, out
