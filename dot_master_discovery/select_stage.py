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


def run_select(df, ad, st, w, pool, anchor, cfg, cfg_path, out, input_sha, workers,
               scan_path, score_fn, metrics_fn, grammar_fn, breakdown_fn, loss_events_fn,
               arm_sizes=ARM_SIZES, seed=SEED, book_size=None, frame_path=None):
    """SCREEN -> NESTED ARMS -> SCORE ALL SIX -> EMIT. Data in, signals and score out.

    The adopted 297 exists as an artefact of several days of conversation and cannot be
    regenerated from data. This closes that gap - and section 7 says the honest expected
    outcome is that every arm lands inside its own random band, because the incumbent
    sits at roughly the 4th percentile and NOTHING IN ITS MEMBERS EXPLAINS WHY.
    """
    import time as _t
    scan = pd.read_csv(scan_path)
    train_months, held = train_window(df)
    print(f'  TRAIN RULE: all but the final {TRAIN_EXCLUDE_MONTHS} month(s) - a RULE, not a '
          f'date, so the screen keeps its meaning every month the stage runs', flush=True)
    print(f'  TRAIN WINDOW: {train_months[0]} -> {train_months[-1]} ({len(train_months)} '
          f'months) | HELD OUT: {held}', flush=True)
    print(f'  SCREEN: {len(scan):,} RAW F0 rows. THE 6,488/6,034 PRE-FILTER IS NOT APPLIED - '
          f'it uses full-sample agg_pf, folds_plus and trades computed over the months the '
          f'screen is meant to validate against.', flush=True)
    items = [(scan['signal_def'].iloc[i], scan['direction'].iloc[i]) for i in range(len(scan))]
    fp = frame_path
    if fp is None:
        fp = os.path.join(out, '_frame_select.csv')
        if not os.path.exists(fp):
            df.to_csv(fp, index=False, lineterminator='\n', encoding='utf-8')
    t0 = _t.time()
    surv, rej = screen_all(items, fp, workers, train_months)
    el = _t.time() - t0
    from collections import Counter
    why_c = Counter(r['reject_criterion'].split()[0] for r in rej)
    print(f'  SURVIVORS: {len(surv):,} of {len(scan):,} '
          f'({100.0 * len(surv) / max(len(scan), 1):.1f}%) in {el / 60:.1f} min', flush=True)
    print(f'    spec band 5,800-6,800 | 200-row sample projected 6,914 +/-670', flush=True)
    if len(scan) > 5000 and not (5800 <= len(surv) <= 6800):
        print(f'    *** {len(surv):,} IS OUTSIDE THE SPEC BAND. Near 4,575 means the '
              f'pre-filter is back. THE TWO BANDS COME FROM DIFFERENT SAMPLES AND ARE NOT '
              f'RECONCILED HERE - if it also sits outside 6,244-7,584, both are wrong.',
              flush=True)
    print(f'  REJECTIONS ({len(rej):,}): {dict(why_c)}', flush=True)
    print(f'    200-row sample gave trades 100 / train_PF 21 / bucket_profit_frac 9 of 130 - '
          f'A MATERIAL SHIFT IN THIS COMPOSITION MATTERS MORE THAN THE COUNT.', flush=True)
    fpr = survivor_fingerprint(surv)
    print(f'  SURVIVOR FINGERPRINT {fpr} - sorted on (signal_def, direction), NEVER a float, '
          f'so the seed is not cosmetic', flush=True)
    sizes = tuple(n for n in arm_sizes if n <= len(surv))
    arms, order = nested_arms(len(surv), sizes, seed=seed)
    bs = book_size if book_size is not None else (297 if 297 in arms else max(arms))
    if bs not in arms:
        raise SystemExit(
            f'ABORT [SELECT] book_size {bs} is not one of the emitted arms {sorted(arms)}. '
            f'A seventh arm is not drawn silently and the size is not rounded to the nearest: '
            f'the emitted book must be one of the arms the six-arm table reports, or the '
            f'scorecard describes a book nobody scored.')
    arm_books = {n: [surv[i] for i in idxs] for n, idxs in arms.items()}
    print(f'  ARMS: {sorted(arms)} drawn as PREFIXES OF ONE SEEDED PERMUTATION (seed={seed}) - '
          f'independent draws would confound size with draw luck, and draw-to-draw variation '
          f'is larger than the size effect at small N', flush=True)
    for a_, b_ in zip(sorted(arms)[:-1], sorted(arms)[1:]):
        sa = {(r['signal_def'], r['direction']) for r in arm_books[a_]}
        sb = {(r['signal_def'], r['direction']) for r in arm_books[b_]}
        if not sa <= sb:
            raise SystemExit(f'ABORT [SELECT] arm {a_} is not a subset of arm {b_} - the arms '
                             f'are not nested and the size sweep is unreadable.')
    print(f'  NESTING VERIFIED: every arm is a strict prefix of the next.', flush=True)
    scores = {}
    for n in sorted(arms):
        bk = pd.DataFrame([{'trigger': 'F0', 'direction': r['direction'],
                            'signal_def': r['signal_def']} for r in arm_books[n]])
        grammar_fn(bk)
        t1 = _t.time()
        import score_g as _sg
        sigs = _sg.build_book(df, pool, anchor, bk, adaptive=ad, structural=st)
        r, td = score_fn(df, sigs, ad, st, w, _conv_for(df, cfg), cfg)
        tl, ev, dy = loss_events_fn(td)
        r.update({'signals': n, 'events': ev, 'event_days': dy, 'trade_losses': tl,
                  'secs': round(_t.time() - t1, 1)})
        scores[n] = (r, td)
        print(f'    arm {n:>6}: {r["trades"]:>6} trades  WR {r["WR"]:>6}  PF {str(r["PF"]):>7}  '
              f'net {r["net"]:>12,.2f}  events {ev:>4}  days {dy:>3}  ({r["secs"]}s)', flush=True)
    return {'survivors': surv, 'rejected': rej, 'arms': arms, 'arm_books': arm_books,
            'scores': scores, 'seed': seed, 'book_size': bs, 'fingerprint': fpr,
            'train_months': train_months, 'held': held, 'screen_secs': el,
            'scan_rows': len(scan)}


def _conv_for(df, cfg):
    import conviction as _C
    cv = cfg['conviction']
    return _C.build_conviction(df, bool(cv['hurst']), bool(cv['recentfb']), bool(cv['d2d']),
                               d2d_conviction=bool(cv['d2d_conviction']),
                               d2d_gap=bool(cv['d2d_gap']))


def side_by_side(res, incumbent_book):
    """THE ARM AGAINST THE INCUMBENT 297, AND THE OVERLAP.

    The overlap is the part that decides whether a near-miss is a near-miss: an arm
    scoring 80% of the incumbent while sharing 3% of its members has not nearly found
    the book, it has found a different book of similar size.
    """
    bs = res['book_size']
    r, td = res['scores'][bs]
    out = ['', '  SIDE BY SIDE - selected arm against the INCUMBENT 297',
           f'    {"metric":22}{"SELECTED":>16}{"INCUMBENT":>16}{"delta":>14}']

    def row(lbl, a, b, fmt='{:,.2f}'):
        try:
            d = float(a) - float(b)
            ds = fmt.format(d)
        except (TypeError, ValueError):
            ds = '-'
        av = fmt.format(a) if isinstance(a, (int, float)) else str(a)
        bv = fmt.format(b) if isinstance(b, (int, float)) else str(b)
        out.append(f'    {lbl:22}{av:>16}{bv:>16}{ds:>14}')

    row('signals', bs, INCUMBENT['signals'], '{:,.0f}')
    row('trades', r['trades'], INCUMBENT['trades'], '{:,.0f}')
    row('WR %', r['WR'], INCUMBENT['WR'])
    row('PF', r['PF'], INCUMBENT['PF'])
    row('net $', r['net'], INCUMBENT['net'])
    row('LOSS EVENTS', r['events'], INCUMBENT['events'], '{:,.0f}')
    row('event-days', r['event_days'], INCUMBENT['event_days'], '{:,.0f}')
    row('worst bar $', r.get('worst_bar', ''), -1224.00)
    row('worst day $', r.get('daily_wd', ''), INCUMBENT['worst_day'])
    row('losing weeks', r.get('losing_weeks', ''), INCUMBENT['losing_weeks'], '{:,.0f}')
    row('days traded', r.get('days_traded', ''), INCUMBENT['days'], '{:,.0f}')
    row('folds positive', r.get('folds_plus', ''), 6, '{:,.0f}')
    row('OOS PF', r.get('oos_prop_pf', r.get('oos_pf', '')), 9.78)
    inc = {(str(a), str(b)) for a, b in zip(incumbent_book['signal_def'],
                                            incumbent_book['direction'])}
    sel = {(str(x['signal_def']), str(x['direction'])) for x in res['arm_books'][bs]}
    surv = {(str(x['signal_def']), str(x['direction'])) for x in res['survivors']}
    rejm = {(str(x['signal_def']), str(x['direction'])): x['reject_criterion']
            for x in res['rejected']}
    shared = sel & inc
    inc_rej = [k for k in inc if k in rejm]
    from collections import Counter
    rc = Counter(rejm[k].split()[0] for k in inc_rej)
    out += ['', '  OVERLAP WITH THE INCUMBENT 297',
            f'    selected signals in the 297      : {len(shared)} of {len(sel)} '
            f'({100.0 * len(shared) / max(len(sel), 1):.1f}%)',
            f'    selected signals NOT in the 297  : {len(sel - inc)}',
            f'    297 members that SURVIVED screen : {len(inc & surv)} of {len(inc)}',
            f'    297 members the screen REJECTED  : {len(inc_rej)}  {dict(rc)}',
            '    An arm scoring near the incumbent while sharing few of its members has not',
            '    nearly found the book - it has found a different book of the same size.']
    return out


def arm_table(res):
    out = ['', '  SIX-ARM TABLE',
           f'    {"signals":>9}{"trades":>9}{"WR%":>8}{"PF":>9}{"net $":>14}'
           f'{"EVENTS":>8}{"days":>7}{"worstDay":>11}{"secs":>7}']
    for n in sorted(res['scores']):
        r, _ = res['scores'][n]
        out.append(f'    {n:>9}{r["trades"]:>9,}{r["WR"]:>8}{str(r["PF"]):>9}'
                   f'{r["net"]:>14,.2f}{r["events"]:>8}{r["event_days"]:>7}'
                   f'{r.get("daily_wd", 0):>11,.2f}{r["secs"]:>7}')
    out.append(f'    emitted book_size = {res["book_size"]}  (SEED={res["seed"]}, '
               f'fingerprint {res["fingerprint"]})')
    return out


REQUIRED_KEYS = ('screen.min_trades', 'screen.min_train_pf', 'screen.min_buckets_present',
                 'screen.min_bucket_profit_frac', 'screen.holdout_months_N',
                 'draw.seed', 'draw.book_size', 'draw.arm_sizes',
                 'arch.floor_long', 'arch.floor_short', 'arch.max_positions',
                 'arch.atr_min', 'arch.recentfb_sizing', 'gates.min_cell_events')


def cfg_get(cfg, key, default='__ABORT__'):
    """CONFIG OR ABORT (SPEC §1). A missing key aborts WITH THE KEY NAME rather than
    falling back to a value fitted to this frame."""
    cur = cfg
    for part in key.split('.'):
        if not isinstance(cur, dict) or part not in cur:
            if default != '__ABORT__':
                return default
            raise SystemExit(f'ABORT [SELECT config] required key "{key}" is absent. SPEC §1: '
                             f'every threshold, window rule, band and size comes from config.')
        cur = cur[part]
    return cur


def assert_config(cfg, cfg_path):
    missing = [k for k in REQUIRED_KEYS
               if cfg_get(cfg, k, default='__MISS__') == '__MISS__']
    if missing:
        raise SystemExit(f'ABORT [SELECT config] {os.path.basename(cfg_path)} is missing '
                         f'{len(missing)} required key(s): {missing}')
    return True


def resolved_window(df, N):
    """SPEC §2. N, the window and the bucket count print on EVERY run."""
    mo = pd.Series(df['Time'].astype(str).values).str[:7]
    months = sorted(mo.unique())
    if len(months) <= N:
        raise SystemExit(f'ABORT [SELECT] frame spans {len(months)} months; holdout N={N} '
                         f'leaves no train window.')
    train = months[:-N] if N else months
    tmask = mo.isin(train).values
    ts = df['Time'].astype(str).values
    return {'N': N, 'train_months': train, 'held': months[-N:] if N else [],
            'buckets': len(train), 'first_bar': str(ts[tmask][0]),
            'last_bar': str(ts[tmask][-1])}


def band_for(cfg, N, size):
    b = cfg.get('bands', {}).get(str(N), {})
    row = b.get(str(size))
    if not row:
        return None, None
    return row, {'N': b.get('_N', N), 'window': b.get('_window', '?'),
                 'pool': b.get('_pool', '?'), 'seeds': row.get('seeds', '?')}


def band_verdict(value, rng):
    if rng is None or value in (None, ''):
        return '-'
    lo, hi = min(rng), max(rng)
    return 'BELOW' if float(value) < lo else ('ABOVE' if float(value) > hi else 'INSIDE')


def gate_candidates(pool):
    """THE S2 CONDITION POOL IS THE CANDIDATE SPACE (249). DO NOT DETECT VARIABLES.

    A previous pass inferred it from column dtypes and found 151. The true space is
    249 x 6 cells = 1,494 single-gate trials, which corrects §5.2's estimate from 117
    variables - same order, same design problem.
    """
    return list(pool.keys()) if isinstance(pool, dict) else list(pool)


def ungated_trades(df, sigs, ad, st, w, conv, cfg, adm_mod, gap_names):
    """THE §5.1 BASIS: ADM_TIERGATES EMPTY, floors and the global ATR gate applied.

    A GATE IS DERIVED FROM THE POPULATION IT WILL FILTER, NEVER FROM THE POPULATION IT
    HAS ALREADY FILTERED. On the gated incumbent LONG d3 reads 3/59 and SHORT d3 2/62
    and neither can separate - Micro_Hurst > p90 has already removed what it was derived
    to remove.
    """
    keep = dict(tg=adm_mod.ADM_TIERGATES, mx=adm_mod.MAX_POSITIONS, fl=adm_mod.ADM_FLOOR,
                gt=adm_mod.ADM_GATES, rule=adm_mod.ADMISSION_RULE)
    adm_mod.ADM_TIERGATES = None
    adm_mod.ADMISSION_RULE = cfg_get(cfg, 'admission')
    adm_mod.MAX_POSITIONS = int(cfg_get(cfg, 'arch.max_positions'))
    adm_mod.ADM_FLOOR = {1: int(cfg_get(cfg, 'arch.floor_long')),
                         -1: int(cfg_get(cfg, 'arch.floor_short'))}
    adm_mod.ADM_GATES = {'ATR': df['ATR_1M'].values.astype(float),
                         'atr_min': float(cfg_get(cfg, 'arch.atr_min'))}
    try:
        td = adm_mod.run_portfolio(df, sigs, adaptive=ad, structural=st, warmup=w,
                                   verbose=False, conviction=conv)
    finally:
        for k, v in (('ADM_TIERGATES', keep['tg']), ('MAX_POSITIONS', keep['mx']),
                     ('ADM_FLOOR', keep['fl']), ('ADM_GATES', keep['gt']),
                     ('ADMISSION_RULE', keep['rule'])):
            setattr(adm_mod, k, v)
    return td[~td['signal_name'].isin(gap_names)]


def cells_of(td, tiers):
    """(direction, tier) -> bars/trades/events. tier = min(depth, max_tier), INHERITED.
    Tiers below the depth floor are unreachable and are not built."""
    bar = td['entry_bar'].values.astype(np.int64)
    dirn = np.array([1 if (x == 1 or str(x).upper() == 'LONG') else -1
                     for x in td['direction'].values])
    cnt = {}
    for b_, d_ in zip(bar, dirn):
        cnt[(int(d_), int(b_))] = cnt.get((int(d_), int(b_)), 0) + 1
    depth = np.array([cnt[(int(d_), int(b_))] for b_, d_ in zip(bar, dirn)])
    tier = np.minimum(depth, max(tiers))
    out = {}
    for dv, dl in ((1, 'LONG'), (-1, 'SHORT')):
        for t in tiers:
            sel = (dirn == dv) & (tier == t)
            ids = sorted(set(int(b) for b in bar[sel]))
            sub = td[sel]
            ev = int((sub.groupby('entry_bar')['pnl'].sum() < 0).sum()) if len(sub) else 0
            out[(dl, t)] = {'bars': ids, 'trades': int(sel.sum()), 'events': ev,
                            'losses': int((sub['pnl'].values < 0).sum()) if len(sub) else 0}
    return out


def stage_a_power(cells, min_events):
    """STAGE A - POWER FLOOR. "CAN THIS CELL SUPPORT A TEST?", not "is this cell bad?".

    The original asked whether the cell was WORSE than the book. SHORT d3 is BETTER
    (5.21% vs 7.29%), so it was closed before any candidate could be tested - while
    Micro_Hurst > p90 demonstrably works there at p = 0.000. The acceptance test could
    never have passed.

    This closes SHORT d4 (4 events) and SHORT d5+ (2) ON A STATED BASIS, which is the
    same verdict 20 hand-relocations reached independently.
    """
    out = {}
    for k, m in cells.items():
        out[k] = {'events': m['events'], 'trades': m['trades'], 'bars': len(m['bars']),
                  'verdict': 'TESTABLE' if m['events'] >= min_events else 'BELOW POWER FLOOR'}
    return out


def pool_masks(df, pool, keys):
    """Masks come from the S2 pool itself - built through the sacred dots_thresholds
    path - so nothing here re-derives a threshold and the p80/p20-for-p90 defect
    cannot recur."""
    out = {}
    for k in keys:
        a = np.asarray(pool[k])
        if a.dtype != bool:
            a = a.astype(bool)
        if a.shape[0] == len(df):
            out[k] = a
    return out


def stage_b_rarity(masks, bar_ids, lo, hi, ref=None):
    """STAGE B - RARITY SHORTLIST, BEFORE ANY OUTCOME IS READ.

    Pass rate on THAT CELL'S ENTRY BARS ONLY, retained within [lo x ref, hi x ref].
    A RARITY FILTER, NOT A PERFORMANCE FILTER - it is what makes the Stage D null
    rarity-matched. Expect ~25% of the pool; Micro_Hurst > p90 passes 14.84% of
    SHORT d3's bars and a +/-50% band retains 66 of 249.
    """
    ids = np.asarray(sorted(set(int(b) for b in bar_ids)), dtype=np.int64)
    if not ids.size:
        return [], {}, None
    rates = {k: float(m[ids].mean()) for k, m in masks.items()}
    pos = [r for r in rates.values() if r > 0]
    if not pos:
        return [], rates, None
    r0 = float(ref) if ref else float(np.median(pos))
    band = (r0 * lo, r0 * hi)
    return sorted([k for k, r in rates.items() if r > 0 and band[0] <= r <= band[1]],
                  key=str), rates, (r0, band)


def _events_of(td, sel):
    b = td[sel].groupby('entry_bar')['pnl'].sum()
    return int((b < 0).sum())


def half_bases(td, train_months, cell_bars):
    """CONSTRAINT 2, PRINTED: the per-half event base. SHORT d3's 20 events split 7/13,
    so a both-halves result rests on 7 events. Split-half REMOVES NOISE; IT ESTABLISHES
    NOTHING, and must not be reported as if it does."""
    half = max(1, len(train_months) // 2)
    h1, h2 = set(train_months[:half]), set(train_months[half:])
    mo = pd.Series(td['exit_time'].astype(str).values).str[:7].values
    bar = td['entry_bar'].values.astype(np.int64)
    cell = np.isin(bar, np.asarray(sorted(set(int(b) for b in cell_bars)), dtype=np.int64))
    return (h1, h2, _events_of(td, cell & np.isin(mo, list(h1))),
            _events_of(td, cell & np.isin(mo, list(h2))))


def stage_cd(masks, cell_bars, td, train_months, shortlist, draws, seed, ckpt_path=None,
             ckpt_every=5, progress=None):
    """STAGES C AND D IN ONE PASS, WITH A CHECKPOINT.

    The previous attempt reached 20 of 66 and died with no resume - the third run this
    week lost to a wall without one. State is flushed every `ckpt_every` candidates and
    reloaded on entry, so a killed run resumes rather than restarts.
    """
    half = max(1, len(train_months) // 2)
    h1, h2 = set(train_months[:half]), set(train_months[half:])
    mo = pd.Series(td['exit_time'].astype(str).values).str[:7].values
    bar = td['entry_bar'].values.astype(np.int64)
    cell = np.isin(bar, np.asarray(sorted(set(int(b) for b in cell_bars)), dtype=np.int64))
    m1, m2 = np.isin(mo, list(h1)), np.isin(mo, list(h2))
    base1, base2 = _events_of(td, m1), _events_of(td, m2)
    done = {}
    if ckpt_path and os.path.exists(ckpt_path):
        try:
            done = json.load(open(ckpt_path, encoding='utf-8'))
        except Exception:
            done = {}
    cache = {}

    def ev(k):
        if k not in cache:
            cache[k] = _events_of(td, ~cell | masks[k][bar])
        return cache[k]

    passed = []
    for i, k in enumerate(shortlist, 1):
        sk = str(k)
        if sk in done:
            if done[sk].get('both'):
                passed.append(k)
            continue
        keep = ~cell | masks[k][bar]
        b1 = _events_of(td, m1 & keep) < base1
        b2 = _events_of(td, m2 & keep) < base2
        done[sk] = {'both': bool(b1 and b2), 'h1': bool(b1), 'h2': bool(b2)}
        if b1 and b2:
            passed.append(k)
        if ckpt_path and (i % ckpt_every == 0 or i == len(shortlist)):
            with open(ckpt_path, 'w', encoding='utf-8') as f:
                json.dump(done, f)
            if progress:
                progress(i, len(shortlist), len(passed))
    rng = np.random.default_rng(seed)
    D = {}
    for k in passed:
        obs = ev(k)
        if not shortlist:
            continue
        idx = rng.integers(0, len(shortlist), size=int(draws))
        better = sum(1 for j in idx if ev(shortlist[int(j)]) <= obs)
        D[str(k)] = {'p': round(better / float(draws), 4), 'obs': obs, 'draws': int(draws),
                     'better': int(better)}
    return {'half_base': (base1, base2), 'both': [str(k) for k in passed], 'null': D,
            'checked': len(done)}


def tier_split(D, shortlist_n, cells_admitted, alpha):
    """TWO TIERS, NEVER CONFLATED.

    CONFIRMED needs p < alpha/(shortlist x cells) - 0.05/264 = 0.00019 on this frame,
    which 200 draws CANNOT RESOLVE (floor 0.005). AN EMPTY CONFIRMED TIER IS THE CORRECT
    RESULT, NOT A FAILURE, and no threshold is lowered to populate it.
    """
    trials = max(shortlist_n * max(cells_admitted, 1), 1)
    corrected = alpha / trials
    conf, cand = [], []
    for k, v in sorted(D.items(), key=lambda x: (x[1]['p'], x[0])):
        row = dict(v, condition=k, trials=trials, corrected_threshold=round(corrected, 8))
        (conf if v['p'] < corrected else (cand if v['p'] < alpha else []))
        if v['p'] < corrected:
            conf.append(row)
        elif v['p'] < alpha:
            cand.append(row)
    return conf, cand, corrected, trials
