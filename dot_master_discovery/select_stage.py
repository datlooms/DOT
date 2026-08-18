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


def screen_all(items, frame_path, workers, train_months):
    """Solo-run every candidate and screen on TRAIN-WINDOW statistics.

    Falls back to serial on a pool failure: on a small-RAM box, 14 workers each
    holding a 177,251 x 172 frame is spawn thrash rather than parallelism, and a
    stage that silently stalls is worse than one that runs slowly and says so.
    """
    import multiprocessing as _mp
    from concurrent.futures import ProcessPoolExecutor
    from concurrent.futures.process import BrokenProcessPool
    n = len(items)
    res = []
    if workers and workers > 1 and n >= 64:
        size = max(1, -(-n // (int(workers) * 4)))
        chunks = [(k, items[i:i + size]) for k, i in enumerate(range(0, n, size))]
        try:
            with ProcessPoolExecutor(max_workers=min(int(workers), len(chunks)),
                                     mp_context=_mp.get_context('spawn'),
                                     initializer=_scr_init,
                                     initargs=(frame_path,)) as ex:
                done = 0
                for _i, out in ex.map(_scr_chunk, chunks):
                    res.extend(out)
                    done += 1
                    print(f'    screen chunk {done}/{len(chunks)} - {len(res):,}/{n:,}',
                          flush=True)
        except (BrokenProcessPool, OSError, MemoryError, Exception) as exc:
            print(f'    screen pool failed ({type(exc).__name__}: {str(exc)[:70]}) - SERIAL',
                  flush=True)
            res = []
    if not res:
        _scr_init(frame_path)
        step = max(1, n // 20)
        for i in range(0, n, step):
            _j, out = _scr_chunk((0, items[i:i + step]))
            res.extend(out)
            print(f'    screen serial {len(res):,}/{n:,}', flush=True)
    surv, rej = [], []
    for sd, dr, per in res:
        ok, why, stats = screen_row(per, train_months)
        row = dict(signal_def=sd, direction=dr, **stats)
        if ok:
            surv.append(row)
        else:
            rej.append(dict(row, reject_criterion=why))
    return stable_survivors(surv), rej


def emit_artifacts(out, res, arm_books, scores, cfg_path, cfg_sha):
    """FOUR OUTPUTS. The selection file says PROVENANCE, NOT A SCORE - a column that
    looks like a ranking will be read as one within a week, and facts F1-F3 say there
    is nothing here to rank."""
    bs = res['book_size']
    seed = res['seed']
    d = os.path.join(out, 'select')
    os.makedirs(d, exist_ok=True)
    paths = {}
    bk = pd.DataFrame([{'trigger': 'F0', 'direction': r['direction'],
                        'signal_def': r['signal_def']} for r in arm_books[bs]])
    p1 = os.path.join(d, f'{bs}_signals.csv')
    bk.to_csv(p1, index=False, lineterminator='\n')
    paths['signals'] = p1
    chosen = {(r['signal_def'], r['direction']) for r in arm_books[bs]}
    sel = []
    for r in res['survivors']:
        if (r['signal_def'], r['direction']) not in chosen:
            continue
        sel.append(dict(r, SEED=seed, book_size=bs,
                        PASS_trades='PASS', PASS_train_PF='PASS',
                        PASS_buckets='PASS', PASS_bucket_frac='PASS'))
    p2 = os.path.join(d, f'{bs}_selection.csv')
    _write_hdr(p2, pd.DataFrame(sel), [
        'DOT SELECT - PROVENANCE OF THE ADMITTED SIGNALS. NOT A SCORE AND NOT A RANKING.',
        f'SEED={seed} book_size={bs} config={os.path.basename(cfg_path)} sha={cfg_sha}',
        'Every column here records WHY a signal was ADMITTED by the train-window screen. '
        'NOTHING IN THIS FILE ORDERS SIGNALS BY QUALITY: a rebuilt ranking selector lost to '
        '12 of 12 random draws on a holdout, 297 leave-one-out runs gave split-half rho '
        '-0.060 at p=0.305, and d_events moved by zero for 212 of 297. The book is '
        'homogeneous - there is nothing to prune and nothing to rank.',
        'Membership is a SEEDED DRAW at a config size, which is a SIZE PARAMETER, not a '
        'ranking. The arm is reproducible from SEED and book_size alone.'])
    paths['selection'] = p2
    p3 = os.path.join(d, f'{bs}_rejected.csv')
    _write_hdr(p3, pd.DataFrame(res['rejected']), [
        'DOT SELECT - EVERYTHING THE TRAIN-WINDOW SCREEN CUT, WITH THE CRITERION THAT CUT IT.',
        f'SEED={seed} book_size={bs}',
        'Screened on the RAW F0 scan. The 6,488/6,034 pre-filter is deliberately NOT applied: '
        'it uses full-sample agg_pf, folds_plus and trades computed over the months the screen '
        'is meant to validate against, so layering a train-only screen on it means the '
        'population was already selected with the answer.'])
    paths['rejected'] = p3
    return paths


def _write_hdr(path, frame, header):
    with open(path, 'w', encoding='utf-8', newline='') as f:
        for h in header:
            f.write(f'# {h}\n')
        frame.to_csv(f, index=False, lineterminator='\n')


def baseline_table():
    """SECTION 7. Size-relative, and every range is a FLOOR."""
    out = ['', '  RANDOM BASELINE - COMPARE EACH ARM TO ITS OWN ROW, NOT TO THE INCUMBENT',
           f'    {"signals":>9}{"events":>12}{"PF":>12}{"margin":>14}{"days":>12}{"seeds":>7}']
    for n, ev, pf, mg, dy, sd in RANDOM_BASELINE:
        out.append(f'    {n:>9}{ev:>12}{pf:>12}{mg:>14}{dy:>12}{sd:>7}')
    out.append(f'    {"INCUMBENT":>9}{"42":>12}{"14.53":>12}{"33.07":>14}{"119":>12}{"-":>7}')
    out.append('    All ranges are FLOORS: measured on the 4,575 PRE-CORRECTION pool, and the')
    out.append('    corrected screen admits signals that failed the full-sample filter.')
    out.append('    AN ARM INSIDE ITS OWN RANGE HAS REPRODUCED THE RANDOM BASELINE, NOT THE BOOK.')
    out.append('    THE ONLY RESULT THAT MEANS SOMETHING IS AN ARM BELOW ITS RANGE.')
    out.append('    AND IF EVERY ARM LANDS INSIDE ITS BAND, THAT IS THE FINDING, NOT A DEFECT:')
    out.append('    the incumbent sits at the 4th percentile of this distribution and NOTHING IN')
    out.append('    ITS MEMBERS EXPLAINS WHY - 297 leave-one-out runs, a 50-draw ablation,')
    out.append('    split-half rho -0.060 and four indistinguishable source objectives all say')
    out.append('    so. Its edge lives in the specific combination, and no reproducible')
    out.append('    procedure can currently regenerate it.')
    return out
