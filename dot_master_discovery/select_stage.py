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
SMOKE_NULL_CAP = None
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
    # AN ARM OF SIZE ZERO IS NOT AN ARM. With 0 survivors this line created arms={0: []},
    # the loop below built an EMPTY book, run_portfolio returned a frame with no columns,
    # and _score_configured raised KeyError: 'signal_name'. A zero-survivor screen is a
    # legitimate outcome of a capped smoke scan and must degrade, not crash.
    if n_survivors > 0:
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
               arm_sizes=ARM_SIZES, seed=SEED, book_size=None, frame_path=None,
               gate_ctx=None):
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
        if not len(bk):
            print(f'    arm {n:>6}: EMPTY BOOK - no signals to score. This is a screen '
                  f'outcome, not an error.', flush=True)
            scores[n] = ({'trades': 0, 'WR': 0.0, 'PF': '', 'net': 0.0, 'signals': n,
                          'events': 0, 'event_days': 0, 'trade_losses': 0,
                          'secs': 0.0}, None)
            continue
        sigs = _sg.build_book(df, pool, anchor, bk, adaptive=ad, structural=st)
        try:
            r, td = score_fn(df, sigs, ad, st, w, _conv_for(df, cfg), cfg)
        except (ZeroDivisionError, ValueError, IndexError, KeyError, AttributeError) as exc:
            # A SMALL ARM CAN PRODUCE ZERO BOOK-ONLY TRADES: every trade on the bar is a
            # conviction gap filler, so the BOOK-only population is empty and the metrics
            # block divides by zero. Report the arm as EMPTY rather than aborting the
            # sweep - at stand-in sizes this is expected, and at real sizes it is a
            # finding about the arm.
            print(f'    arm {n:>6}: EMPTY - no BOOK-only trades '
                  f'({type(exc).__name__}); scored as zero', flush=True)
            scores[n] = ({'trades': 0, 'WR': 0.0, 'PF': '', 'net': 0.0, 'signals': n,
                          'events': 0, 'event_days': 0, 'trade_losses': 0,
                          'secs': round(_t.time() - t1, 1)}, None)
            continue
        tl, ev, dy = loss_events_fn(td)
        r.update({'signals': n, 'events': ev, 'event_days': dy, 'trade_losses': tl,
                  'secs': round(_t.time() - t1, 1)})
        scores[n] = (r, td)
        print(f'    arm {n:>6}: {r["trades"]:>6} trades  WR {r["WR"]:>6}  PF {str(r["PF"]):>7}  '
              f'net {r["net"]:>12,.2f}  events {ev:>4}  days {dy:>3}  ({r["secs"]}s)', flush=True)
    # THE GATE LAYER, REACHABLE FROM --stage SELECT. Presence proved the merge and said
    # nothing about wiring: none of Stage A/B/C/D, resolve_ranking, prereg_mask or the
    # exhaustive prereg null was reachable from the command the operator types.
    gate_lines, gate_verdicts = [], {}
    if gate_ctx is not None:
        import cluster_profiler as _cp
        td_ung = ungated_trades(gate_ctx['df'], gate_ctx['sigs'], ad, st, w,
                                gate_ctx['conv'], cfg, gate_ctx['adm'], _cp.GAP_NAMES)
        gate_lines, gate_verdicts = run_gate_layer(
            gate_ctx['df'], gate_ctx['sigs'], ad, st, w, gate_ctx['conv'], cfg,
            gate_ctx['pool'], gate_ctx['adm'], gate_ctx['sw'], _cp.GAP_NAMES, td_ung)
        for ln in gate_lines:
            print(ln, flush=True)
    # §4.5 QUALITY ARMS - never a default derived from the incumbent's medians.
    qa = quality_arms(surv, cfg_get(cfg, 'screen.quality_pctile_arms', default=[None]))
    print('  §4.5 QUALITY ARMS  percentile of the SURVIVING pool, PER DIRECTION, applied '
          'AFTER the four absolute criteria. Measured on FULL-SAMPLE statistics - one step '
          'from the look-ahead the pre-filter had - so it is emitted as ARMS and the '
          'OPERATOR RULES.', flush=True)
    print(f'    {"arm":8}{"book size":>11}', flush=True)
    for k in sorted(qa, key=lambda x: (x != 'null', x)):
        print(f'    {k:8}{len(qa[k]):>11}', flush=True)
    return {'survivors': surv, 'rejected': rej, 'arms': arms, 'arm_books': arm_books,
            'scores': scores, 'seed': seed, 'book_size': bs, 'fingerprint': fpr,
            'train_months': train_months, 'held': held, 'screen_secs': el,
            'scan_rows': len(scan), 'gate_lines': gate_lines,
            'gate_verdicts': gate_verdicts, 'quality_arms': {k: len(v) for k, v in qa.items()}}


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


def prereg_mask(df, variable, side, pct, sw_mod):
    """THE BRIDGE (REV7 §7.4). STRICT BOTH WAYS.

    sw.swept substitutes dt._D_SPEC, calls the sacred compute_adaptive_thresholds and
    restores in a finally - ring 2500, day-refreshed on the day-of-month field, floor-
    index percentile, no warm-up special case, identical to production by construction.

    THIS EXISTS BECAUSE dt._D_SPEC IS EXACTLY [0.2, 0.8]: the S2 pool cannot express
    > p90, so a Micro_Hurst:hi drawn from the pool is > p80 - a LOOSER condition. Ranking
    it is ranking the wrong candidate, and that already shipped once.
    """
    # _D_SPEC IS EXPRESSED AS FRACTIONS (0.8 / 0.2), NOT PERCENTAGES. Passing 90 where
    # 0.90 belongs made Micro_Hurst > p90 pass 0.2104% of bars instead of 9.7478% - the
    # checksum caught it on its first run, which is exactly what it is for.
    q = float(pct) / 100.0 if float(pct) > 1.0 else float(pct)
    t = sw_mod.swept(df, {(variable, side): (variable, q)})[(variable, side)]
    v = df[variable].values
    return (v > t) if side == 'hi' else (v < t)


def assert_prereg_checksums(df, cfg, sw_mod):
    """HARD ABORT, EXACT TO 4 DP. A mask near 20% where Micro_Hurst > p90 belongs means
    the p80 series is live - the defect swept_thresholds exists to prevent."""
    want = cfg_get(cfg, 'gates.prereg_checksums')
    lines, bad = [], []
    for key, exp in sorted(want.items()):
        var, side, pct = key.split('|')
        got = round(100.0 * float(prereg_mask(df, var, side, float(pct), sw_mod).mean()), 4)
        ok = abs(got - float(exp)) < 1e-4
        lines.append(f'    {var} {side} p{pct:<4} expected {float(exp):8.4f}%  got '
                     f'{got:8.4f}%  {"OK" if ok else "MISMATCH"}')
        if not ok:
            bad.append((key, exp, got))
    if bad:
        raise SystemExit('ABORT [prereg checksum] ' + '; '.join(
            f'{k}: expected {e}% got {g}%' for k, e, g in bad) +
            '. A mask near 20% where a p90 condition belongs means ad[(var,"hi")] - the p80 '
            'series - is being used. swept_thresholds exists to prevent exactly this and the '
            'defect has already shipped once.')
    return lines


def support_filter(masks, cell_bars, min_support):
    """gates.min_cell_support - AN UNSTATED RULE, NOW NAMED WITH ITS EFFECT.

    A condition admitting fewer than min_support of the cell's entry bars cannot be
    tested there. Measured: excludes 41 of 249 at LONG d3 (208 supported) and 39 at
    SHORT d3 (210) - THE ENTIRE 39-of-208 vs 112-of-249 divergence.
    """
    ids = np.asarray(sorted(set(int(b) for b in cell_bars)), dtype=np.int64)
    kept, dropped = [], 0
    for k, m in masks.items():
        if int(m[ids].sum()) >= int(min_support):
            kept.append(k)
        else:
            dropped += 1
    return sorted(kept, key=str), dropped


def cell_halves(cell_bars, mode):
    """gates.half_split - THE OTHER UNSTATED RULE. 'cell_bar_median' splits on the median
    of THAT CELL'S OWN entry bars, giving exactly 269/269 and 192/192. A frame-midpoint
    split gives 251/252 and 170/183 and FLIPS THE SHORT d3 VERDICT on its own."""
    ids = sorted(set(int(b) for b in cell_bars))
    if mode != 'cell_bar_median':
        raise SystemExit(f'ABORT [gates.half_split] "{mode}" is not implemented. Only '
                         f'"cell_bar_median" is specified; a frame-midpoint split changes '
                         f'the SHORT d3 verdict on its own and must not be chosen silently.')
    mid = len(ids) // 2
    return {'A': ids[:mid], 'B': ids[mid:]}, (ids[mid] if ids else None)


def retained_loss_rate(mask, cell_bars, loss_bars):
    """STAGE C METRIC ONLY - A RATE, NOT A COUNT, NON-MONOTONE BY CONSTRUCTION.

    INVENTED FOR STAGE C TO BE CHEAP. IT IS NOT WHAT THE GATE WAS DERIVED ON: the
    hand-derivation measured BOOK LOSS EVENTS FROM A FULL ENGINE RUN. Using this
    quantity for the prereg null produced p=0.045 and p=0.12; on the derived quantity
    they are 0.0000 and 0.0303. Do not reuse it for the prereg verdict.
    """
    adm = mask[cell_bars]
    n = int(adm.sum())
    if n == 0:
        return None, 0, 0
    return int((adm & loss_bars).sum()) / float(n), int((adm & loss_bars).sum()), n


def stage_c_rate(masks, cell_bars, loss_bar_ids, halves, shortlist):
    """Candidate must beat the cell's base retained-loss rate in BOTH halves.

    "Reduce book loss events in both halves" was MONOTONE and could not fail - 45 of 45
    passed. A ratio moves either way because admitting fewer bars removes winners too.
    """
    cb = np.asarray(sorted(set(int(b) for b in cell_bars)), dtype=np.int64)
    lb = np.isin(cb, np.asarray(sorted(set(int(b) for b in loss_bar_ids)), dtype=np.int64))
    out, base = [], {}
    for hname, hbars in halves.items():
        sel = np.isin(cb, np.asarray(sorted(set(int(b) for b in hbars)), dtype=np.int64))
        nb = int(sel.sum())
        base[hname] = ((int((lb & sel).sum()) / float(nb)) if nb else None,
                       int((lb & sel).sum()), nb)
    for k in shortlist:
        m = masks[k][cb]
        ok = True
        for hname, hbars in halves.items():
            sel = np.isin(cb, np.asarray(sorted(set(int(b) for b in hbars)), dtype=np.int64))
            a = m & sel
            n = int(a.sum())
            if n == 0 or base[hname][0] is None or \
                    not (int((a & lb).sum()) / float(n) < base[hname][0]):
                ok = False
                break
        if ok:
            nf = int(m.sum())
            out.append({'condition': str(k),
                        'rate': round(int((m & lb).sum()) / float(nf), 6) if nf else None,
                        'loss_bars': int((m & lb).sum()), 'bars': nf})
    return out, base


def resolve_ranking(survivors, max_ties):
    """ties = survivors at the minimum rate. Above the ceiling the ranking is SUPPRESSED
    and NO ROW CARRIES A RANK - candidates tied at 0.0000 in a four-loss-bar half is the
    arithmetic of a small denominator, not evidence."""
    if not survivors:
        return [], 0, None, 'NO SURVIVORS'
    rates = [s['rate'] for s in survivors if s['rate'] is not None]
    if not rates:
        return survivors, 0, None, 'FILTER ONLY - no resolvable rate'
    mn = min(rates)
    ties = sum(1 for r in rates if r == mn)
    if ties > int(max_ties):
        return survivors, ties, mn, (f'FILTER ONLY - {ties} CANDIDATES TIED AT MINIMUM RATE '
                                     f'{mn:.4f}, RANKING SUPPRESSED')
    ranked = sorted(survivors, key=lambda s: (s['rate'], s['condition']))
    for i, s in enumerate(ranked, 1):
        s['rank'] = i
    return ranked, ties, mn, 'RANKED'


def stage_d_gated(masks, cell_bars, loss_bar_ids, survivors, shortlist, draws, seed):
    """Stage D scan tier - BOTH ARMS GATED. §C.6 caught a latent instance of the Stage C
    vacuity here: "comparison is loss EVENTS" against the UNGATED book reproduces it."""
    cb = np.asarray(sorted(set(int(b) for b in cell_bars)), dtype=np.int64)
    lb = np.isin(cb, np.asarray(sorted(set(int(b) for b in loss_bar_ids)), dtype=np.int64))
    rng = np.random.default_rng(seed)
    cache = {}

    def rate(k):
        if k not in cache:
            m = masks[k][cb]
            n = int(m.sum())
            cache[k] = (int((m & lb).sum()) / float(n)) if n else 9e9
        return cache[k]

    out = {}
    for s in survivors:
        key = next((kk for kk in shortlist if str(kk) == s['condition']), None)
        if key is None:
            continue
        obs = rate(key)
        idx = rng.integers(0, len(shortlist), size=int(draws))
        out[s['condition']] = {
            'p': round(sum(1 for j in idx if rate(shortlist[int(j)]) <= obs) / float(draws), 4),
            'obs_rate': round(obs, 6), 'draws': int(draws), 'basis': 'GATED BOTH ARMS'}
    return out


def book_loss_events(td, gap_names):
    """THE DERIVED QUANTITY: |{(entry_bar, direction) : pnl < 0}| over the BOOK-only
    population, from a FULL ENGINE RUN. Not retained_loss_rate, not masks only, not
    cell-scoped."""
    b = td[~td['signal_name'].isin(gap_names)] if gap_names else td
    if not len(b):
        return 0
    g = b.groupby(['entry_bar', 'direction'])['pnl'].sum()
    return int((g < 0).sum())


def prereg_null_exhaustive(df, sigs, ad, st, w, conv, cfg, adm_mod, gap_names,
                           cell, adopted_mask, shortlist_masks, run_label=None,
                           progress=None):
    """REV8 §7.4 - THE PREREG NULL. EXHAUSTIVE, NOT SAMPLED, ON BOOK LOSS EVENTS.

    THE POPULATION IS FINITE AND SMALL, SO ENUMERATE IT: p becomes exact and it is
    cheaper than sampling. At 39 and 66 distinct candidates a 200-draw null cannot
    resolve below 1/39 and 1/66, and 200 draws over 39 distinct values is 31 minutes of
    recomputing the same handful of numbers.

    replace-not-stack, BOTH ARMS GATED. One engine run per candidate.
    p = (candidates with STRICTLY FEWER book loss events) / (shortlist size).
    TIES ARE REPORTED SEPARATELY AND ARE NOT COUNTED AS BETTER.
    """
    dv = 1 if str(cell[0]).upper() == 'LONG' else -1
    keep = dict(tg=adm_mod.ADM_TIERGATES, mx=adm_mod.MAX_POSITIONS, fl=adm_mod.ADM_FLOOR,
                gt=adm_mod.ADM_GATES, rule=adm_mod.ADMISSION_RULE)
    adm_mod.ADMISSION_RULE = cfg_get(cfg, 'admission')
    adm_mod.MAX_POSITIONS = int(cfg_get(cfg, 'arch.max_positions'))
    adm_mod.ADM_FLOOR = {1: int(cfg_get(cfg, 'arch.floor_long')),
                         -1: int(cfg_get(cfg, 'arch.floor_short'))}
    adm_mod.ADM_GATES = {'ATR': df['ATR_1M'].values.astype(float),
                         'atr_min': float(cfg_get(cfg, 'arch.atr_min'))}

    def events_with(mask):
        adm_mod.ADM_TIERGATES = {(dv, int(cell[1])): [mask]}
        td = adm_mod.run_portfolio(df, sigs, adaptive=ad, structural=st, warmup=w,
                                   verbose=False, conviction=conv)
        return book_loss_events(td, gap_names)

    # SMOKE_NULL_CAP truncates the ENUMERATION so smoke does not become the 186s/320s
    # long pole it exists to precede. A CAPPED NULL IS NOT A VERDICT and the caller must
    # print it as a cap - a smoke figure that reads like a result is worse than no figure.
    _items = sorted(shortlist_masks.items(), key=lambda x: str(x[0]))
    _cap = SMOKE_NULL_CAP
    _capped = bool(_cap) and len(_items) > int(_cap)
    if _capped:
        _items = _items[:int(_cap)]
    try:
        adopted = events_with(adopted_mask)
        nulls = []
        for i, (k, m) in enumerate(_items, 1):
            nulls.append((str(k), events_with(m)))
            if progress and (i % 5 == 0 or i == len(_items)):
                progress(i, len(_items))
    finally:
        for a_, v_ in (('ADM_TIERGATES', keep['tg']), ('MAX_POSITIONS', keep['mx']),
                       ('ADM_FLOOR', keep['fl']), ('ADM_GATES', keep['gt']),
                       ('ADMISSION_RULE', keep['rule'])):
            setattr(adm_mod, a_, v_)
    if not nulls:
        return {'verdict': 'NO NULL POPULATION', 'adopted': adopted, 'n': 0}
    vals = [v for _k, v in nulls]
    better = sum(1 for v in vals if v < adopted)
    ties = sum(1 for v in vals if v == adopted)
    return {'capped': _capped, 'cap': (int(_cap) if _capped else None),
            'adopted': adopted, 'n': len(vals), 'min': min(vals),
            'median': float(np.median(vals)), 'max': max(vals),
            'better': better, 'ties': ties, 'p': round(better / float(len(vals)), 4),
            'nulls': nulls}


def prereg_threshold(n_tests, alpha):
    """n_tests IS THE NUMBER OF (candidate, cell) PAIRS ACTUALLY EVALUATED, COUNTED BY THE
    CODE. Never read from config and never declared by the operator: the "one mechanism
    tested twice" argument buys exactly the gap between 0.0303 and 0.05, and A CORRECTION
    THAT CAN BE ARGUED DOWN BY RE-DESCRIBING THE HYPOTHESIS IS NOT A CORRECTION."""
    return alpha / max(int(n_tests), 1)


def quality_arms(survivors, arms):
    """§4.5 AS ARMS - NEVER A DEFAULT DERIVED FROM THE INCUMBENT'S MEDIANS.

    Measured on FULL-SAMPLE scan statistics, not train-window, and one step from the
    look-ahead the pre-filter had. Percentile of the SURVIVING pool, PER DIRECTION,
    applied AFTER the four absolute criteria. null DISABLES. The operator rules.
    """
    out = {}
    for a in arms:
        if a is None:
            out['null'] = list(survivors)
            continue
        keep = []
        for d in sorted({str(s.get('direction', '')) for s in survivors}):
            sub = [s for s in survivors if str(s.get('direction', '')) == d]
            tv = [float(s.get('train_trades', 0)) for s in sub]
            pv = [float(s['train_PF']) for s in sub
                  if str(s.get('train_PF', '')) not in ('', 'inf')]
            if not tv or not pv:
                keep.extend(sub)
                continue
            tc, pc = float(np.quantile(tv, a)), float(np.quantile(pv, a))
            for s in sub:
                pf = s.get('train_PF', '')
                if float(s.get('train_trades', 0)) >= tc and \
                        (str(pf) == 'inf' or (pf != '' and float(pf) >= pc)):
                    keep.append(s)
        out[str(a)] = keep
    return out


def run_gate_layer(df, sigs, ad, st, w, conv, cfg, pool, adm_mod, sw_mod, gap_names, td_ungated):
    """THE GATE LAYER, DRIVEN. §5.5 PRINT ORDER.

    Every one of Stage A/B/C/D, resolve_ranking, prereg_mask and the exhaustive prereg
    null is reachable from here, and run_select calls this - PRESENCE PROVED THE MERGE
    AND SAID NOTHING ABOUT WIRING, which is why none of it was reachable from the command
    the operator types.
    """
    out = []
    tiers = cfg_get(cfg, 'gates.tiers')
    alpha = cfg_get(cfg, 'gates.confirm_alpha')
    msup = cfg_get(cfg, 'gates.min_cell_support')
    hsp = cfg_get(cfg, 'gates.half_split')
    lo, hi = cfg_get(cfg, 'gates.rarity_lo'), cfg_get(cfg, 'gates.rarity_hi')
    draws = cfg_get(cfg, 'gates.null_draws')
    mt = cfg_get(cfg, 'gates.max_min_rate_ties')
    seed = cfg_get(cfg, 'draw.seed')
    pre = cfg_get(cfg, 'gates.preregistered')
    out.append('  ' + '=' * 96)
    out.append('  GATE LAYER')
    out.append(f'    CELL BASIS  UNGATED {len(td_ungated):,} BOOK-only trades | floor '
               f'L{cfg_get(cfg, "arch.floor_long")}/S{cfg_get(cfg, "arch.floor_short")} '
               f'cap {cfg_get(cfg, "arch.max_positions")} '
               f'ATR_1M >= {cfg_get(cfg, "arch.atr_min")} | ADM_TIERGATES EMPTY')
    out.append('    A GATE IS DERIVED FROM THE POPULATION IT WILL FILTER, NEVER FROM THE '
               'POPULATION IT HAS ALREADY FILTERED.')
    out.append('    PREREG CHECKSUM (hard abort, exact to 4dp):')
    for ln in assert_prereg_checksums(df, cfg, sw_mod):
        out.append('  ' + ln)
    cells = cells_of(td_ungated, tiers)
    A = stage_a_power(cells, cfg_get(cfg, 'gates.min_cell_events'))
    out.append(f'    STAGE A  POWER FLOOR min_cell_events='
               f'{cfg_get(cfg, "gates.min_cell_events")} - "can this cell support a test?", '
               f'not "is this cell worse than the book?"')
    for k in sorted(A, key=lambda x: (x[0], x[1])):
        v = A[k]
        out.append(f'      {k[0]:5} d{k[1]:<3} {v["verdict"]:18} events {v["events"]:>3}  '
                   f'trades {v["trades"]:>5}  bars {v["bars"]:>4}')
    admitted = [k for k, v in A.items() if v['verdict'] == 'TESTABLE']
    out.append(f'      ADMITTED {len(admitted)} of {len(cells)}')
    masks = pool_masks(df, pool, gate_candidates(pool))
    prereg_cells, n_tests = [], 0
    for pe in pre:
        key = (str(pe['cell'][0]), int(pe['cell'][1]))
        if key not in cells:
            out.append(f'    PREREG {key} - CELL ABSENT, skipped')
            continue
        cb = cells[key]['bars']
        sub = td_ungated[np.isin(td_ungated['entry_bar'].values.astype(np.int64),
                                 np.asarray(sorted(set(int(x) for x in cb)), dtype=np.int64))]
        bs = sub.groupby('entry_bar')['pnl'].sum()
        lossb = [int(x) for x in bs[bs < 0].index]
        sup, dropped = support_filter(masks, cb, msup)
        halves, splitbar = cell_halves(cb, hsp)
        pm = prereg_mask(df, pe['variable'], pe['side'], pe['pct'], sw_mod)
        ref = float(pm[np.asarray(sorted(set(int(x) for x in cb)), dtype=np.int64)].mean())
        short, _r, bi = stage_b_rarity({k: masks[k] for k in sup}, cb, lo, hi, ref=ref)
        out.append(f'    --- {key[0]} d{key[1]}: {len(cb)} bars, {len(lossb)} loss bars, '
                   f'base {len(lossb) / max(len(cb), 1):.4f}')
        out.append(f'      STAGE B  min_cell_support={msup} excludes {dropped} of '
                   f'{len(masks)} -> {len(sup)} supported')
        out.append(f'               half_split={hsp}: A {len(halves["A"])} / B '
                   f'{len(halves["B"])} bars | split bar {splitbar}')
        out.append(f'               RARITY REF = THE PREREG CANDIDATE\'S OWN pass rate on '
                   f'this cell {100 * ref:.2f}% - §7.5 CIRCULARITY: the shortlist is centred '
                   f'on the thing being tested. A filter, not a test.')
        out.append(f'               band [{bi[1][0]:.4f},{bi[1][1]:.4f}] -> shortlist '
                   f'{len(short)} of {len(sup)}')
        surv, base = stage_c_rate(masks, cb, lossb, halves, short)
        out.append(f'      STAGE C  RATE (non-monotone). base A {base["A"][1]}/'
                   f'{base["A"][2]}={base["A"][0]:.4f}  B {base["B"][1]}/{base["B"][2]}='
                   f'{base["B"][0]:.4f} -> both halves {len(surv)} of {len(short)}')
        ranked, ties, mn, vd = resolve_ranking(surv, mt)
        out.append(f'               RESOLUTION ties {ties} (ceiling {mt}) -> {vd}')
        D = stage_d_gated(masks, cb, lossb, surv, short, draws, seed)
        thr_scan = alpha / max(len(short) * max(len(admitted), 1), 1)
        conf = [k for k, v in D.items() if v['p'] < thr_scan]
        out.append(f'      STAGE D  draws {draws} | BASIS GATED BOTH ARMS | corrected '
                   f'p < {thr_scan:.6f}')
        out.append(f'               CONFIRMED {len(conf)} - AN EMPTY TIER IS THE CORRECT '
                   f'RESULT; {draws} draws cannot resolve {thr_scan:.6f} and no threshold '
                   f'is lowered to populate it')
        cand = sorted([dict(v, condition=k) for k, v in D.items() if v['p'] < alpha],
                      key=lambda x: x['p'])[:5]
        for r in cand:
            rk = next((s.get('rank') for s in ranked if s['condition'] == r['condition']), None)
            out.append(f'               CANDIDATE {r["condition"]:30} p={r["p"]:<7} '
                       f'trials={len(short) * max(len(admitted), 1)} NOT MET {thr_scan:.6f}'
                       + (f' rank {rk}' if rk and vd == 'RANKED' else ''))
        prereg_cells.append((key, pe, pm, {k: masks[k] for k in short}, len(lossb)))
        n_tests += 1
    thr = prereg_threshold(n_tests, alpha)
    out.append(f'    PREREG  n_tests {n_tests} (candidate,cell) PAIRS COUNTED BY THE CODE '
               f'-> threshold {thr}. Never read from config: a correction that can be argued '
               f'down by re-describing the hypothesis is not a correction.')
    verdicts = {}
    for key, pe, pm, sm, nloss in prereg_cells:
        print(f'    PREREG null {key[0]} d{key[1]}: {len(sm)} exhaustive engine runs...',
              flush=True)
        r = prereg_null_exhaustive(df, sigs, ad, st, w, conv, cfg, adm_mod, gap_names,
                                   key, pm, sm,
                                   progress=lambda i, n: print(f'      ...{i}/{n} runs',
                                                               flush=True))
        if r.get('verdict') == 'NO NULL POPULATION':
            out.append(f'      PREREG: NO NULL POPULATION at {key[0]} d{key[1]} - no verdict. '
                       f'No fallback to a sampled null.')
            continue
        v = 'CONFIRMED' if r['p'] <= thr else 'CANDIDATE'
        verdicts[key] = v
        if r.get('capped'):
            out.append(f'      *** {key[0]} d{key[1]} PREREG NULL ENUMERATION CAPPED AT '
                       f'{r["cap"]} OF THE FULL SHORTLIST - THIS p IS NOT A VERDICT. The real '
                       f'run enumerates every candidate. ***')
        out.append(f'      {key[0]:5} d{key[1]:<3} {pe["variable"]} > p{int(pe["pct"])}  '
                   f'BOOK LOSS EVENTS (full engine run, replace-not-stack) adopted '
                   f'{r["adopted"]} | null n {r["n"]} EXHAUSTIVE | min/med/max {r["min"]}/'
                   f'{r["median"]}/{r["max"]} | strictly better {r["better"]} | p {r["p"]} | '
                   f'ties {r["ties"]} (not counted as better) -> {v}')
        out.append(f'                    full cell {nloss} loss bars - THE PREREG ROUTE '
                   f'BYPASSES STAGE C, whose weaker half can carry four')
    out.append(f'    CONJUNCTION  SUPPORTING - CELLS ARE NOT INDEPENDENT. Never the criterion.')
    out.append(f'    ACCEPTANCE  ' + ' | '.join(f'{k[0]} d{k[1]} {v}'
                                                for k, v in sorted(verdicts.items())))
    out.append(f'    TOTAL TRIALS  scan {sum(1 for _ in prereg_cells)} cells x shortlist, '
               f'prereg {n_tests} pairs')
    out.append('  ' + '=' * 96)
    return out, verdicts


DEPLOY_FILES = (
    'master.py', 'select_stage.py', 'engine/whole_dot_config.json',
    'engine/canary_reference.json', 'engine/adm_engine.py', 'engine/swept_thresholds.py',
    'engine/whole_dot_signals.csv', 'engine/book50_signals.csv',
    'engine/dots_thresholds.py', 'engine/wf.py', 'engine/core.py',
    'engine/portfolio_simulation_engine.py', 'engine/conviction.py', 'engine/score_g.py',
    'engine/cluster_profiler.py', 'orchestrator/discovery_orchestrator.py',
    'scanners/sequential_temporal.py', 'dot_frame_binding.py', 'sitecustomize.py',
)


def deploy_manifest(root, reference=None):
    """EVERY FILE --stage SELECT REQUIRES, WITH ITS SHA. FAILS BY NAME.

    The operator has a second tree (DOT_deploy) and had to be told by hand which files
    to copy across a build that changed many of them. One command should tell him exactly
    what to copy rather than making him diff two trees.

    Returns (rows, missing, stale). A file absent or differing from the reference is
    named individually - a manifest that says "something is wrong" is not a manifest.
    """
    rows, missing, stale = [], [], []
    for rel in DEPLOY_FILES:
        path = os.path.join(root, rel)
        if not os.path.exists(path):
            rows.append({'file': rel, 'sha': '', 'status': 'MISSING'})
            missing.append(rel)
            continue
        sha = _sha(path)
        st = 'OK'
        if reference and rel in reference and reference[rel] != sha:
            st = f'STALE (have {sha}, want {reference[rel]})'
            stale.append(rel)
        rows.append({'file': rel, 'sha': sha, 'status': st})
    return rows, missing, stale


def print_deploy_manifest(root, reference=None):
    rows, missing, stale = deploy_manifest(root, reference)
    out = ['', '  DEPLOY MANIFEST - every file --stage SELECT requires',
           f'    {"file":48}{"sha":14}status']
    for r in rows:
        out.append(f'    {r["file"]:48}{r["sha"] or "-":14}{r["status"]}')
    if missing:
        out.append(f'    *** {len(missing)} FILE(S) MISSING: {missing} - COPY THESE ACROSS. '
                   f'The run dies without them. ***')
    if stale:
        out.append(f'    *** {len(stale)} FILE(S) STALE: {stale} - COPY THESE ACROSS. ***')
    if not missing and not stale:
        out.append('    all present and current')
    out.append('    NOTE: a COPIED F0 scan does NOT carry its provenance stamp. Re-run '
               '--stage S3 in the target tree, or SELECT aborts on the section-4 guard.')
    return out, missing, stale
