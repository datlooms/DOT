"""score_book.py — item 16. Scores an operator-assembled book on APPENDIX B.

    python score_book.py --book <csv> --data <frame> --out <dir>

Input is a CSV of signal_id (family|signal_definition|direction, the same key
the catalogue emits) plus an optional direction column for assertion. NOTHING
ELSE. No option changes the verdict — a flag that could soften a constraint is
a constraint that is not enforced.

WHY THIS IS A SEPARATE TOOL. The quantities here are SET PROPERTIES of an
assembled book. They have no per-signal value, and fabricating one would be
worse than omitting it, so they cannot live in the catalogue. The exposure this
closes is union collapse: a 448-signal persistent union scored PF 1.82 against
the curated 50-signal book's PF 6.40, and nothing in a per-signal table would
have shown it.

EVERY ESTIMATOR IS IMPORTED FROM selection.py, NEVER REIMPLEMENTED. A second
implementation is how two arms drift, and in this project that is not
hypothetical. If a number here disagrees with S5B, it is because the book
differs, not because the code does.

THREE ENFORCEMENT MECHANISMS, because a convention is what failed fourteen
times: a non-zero exit naming the breach on stdout so a wrapper cannot ignore it
by accident; an append-only attestation record; and the catalogue header that
already declares any assembled book UNSCORED until this tool has run on it.

book_scored.jsonl IS AN ATTESTATION RECORD, NOT AN ARTIFACT. It is exempt from
the determinism rule and is REQUIRED to carry wall-clock, per Appendix B's
carve-out. Every other output of this pipeline is byte-identical across runs and
carries no wall-clock.
"""

import argparse
import contextlib
import hashlib
import io
import re
import json
import os
import sys
import time

import numpy as np
import pandas as pd

_HERE = os.path.dirname(os.path.abspath(__file__))
for _d in ('engine', 'scanners', 'orchestrator'):
    _p = os.path.join(_HERE, _d)
    if _p not in sys.path:
        sys.path.insert(0, _p)

import dots_thresholds as dt
import portfolio_simulation_engine as engine
import sequential_temporal as seq
import conviction as C
import cluster_profiler as cp
import selection as sel
import terrain as tr
import catalogue as cat
import score_g
import triple_convergence_and_d2ddir as f0

PINNED_W, PINNED_K, PINNED_E = cat.PINNED_CELL
TAU = sel.TAU
MIN_SHARED = sel.MIN_SHARED
MCVAR_WORST_FRAC = 0.05
FTMO_DAILY_CEILING = cat.FTMO_DAILY_CEILING


def _sha_file(path):
    return hashlib.sha256(open(path, 'rb').read()).hexdigest()


def _code_sha():
    h = hashlib.sha256()
    for rel in sorted(('score_book.py', 'engine/selection.py', 'engine/catalogue.py')):
        p = os.path.join(_HERE, rel)
        if os.path.exists(p):
            h.update(open(p, 'rb').read())
    return h.hexdigest()


def load_book(path):
    b = pd.read_csv(path, comment='#')
    if 'signal_id' not in b.columns:
        raise SystemExit('ABORT — the book CSV must carry a signal_id column '
                         '(family|signal_definition|direction). It is the sole required input.')
    rows = []
    for _i, r in b.iterrows():
        sid = str(r['signal_id'])
        parts = sid.split('|', 2)
        if len(parts) != 3:
            raise SystemExit(f'ABORT — signal_id "{sid}" is not family|signal_definition|direction.')
        fam, sig, direction = parts
        if 'direction' in b.columns and not pd.isna(r['direction']):
            stated = str(r['direction']).strip().upper()
            if stated != direction.strip().upper():
                raise SystemExit(
                    f'ABORT — signal_id "{sid}" encodes direction {direction} but the direction '
                    f'column says {stated}. The optional column exists to assert agreement, so a '
                    f'disagreement is a hard stop, not a preference.')
        rows.append({'signal_id': sid, 'trigger': fam, 'family': fam,
                     'signal_def': sig, 'direction': direction.strip().upper()})
    return pd.DataFrame(rows)


def score(book_path, data_path, out_dir):
    os.makedirs(out_dir, exist_ok=True)
    book = load_book(book_path)
    df = pd.read_csv(data_path)
    input_sha = hashlib.sha256(open(data_path, 'rb').read()).hexdigest()[:12]
    w = engine.warmup_floor(df, verbose=False)
    ad = dt.compute_adaptive_thresholds(df)
    st = dt.compute_structural_gates(df)
    pool = seq.build_condition_pool(df, ad, st, w)
    anchor = seq.anchor_array(df, 'ST_Flip')
    print(f'BOOK {os.path.basename(book_path)} — {len(book)} signals | input_sha {input_sha}')
    print(f'  frame {len(df):,} rows | {df["Time"].iloc[0]} -> {df["Time"].iloc[-1]}')
    sigs = score_g.build_book(df, pool, anchor, book, adaptive=ad, structural=st)
    conv = C.build_conviction(df, True, True, True, d2d_conviction=True, d2d_gap=True)
    full = engine.run_portfolio(df, sigs, adaptive=ad, structural=st, warmup=w,
                                verbose=False, conviction=conv)
    bk = full[~full['signal_name'].isin(cp.GAP_NAMES)]
    verdicts = {}
    daily = sel.per_signal_daily(bk)
    smap = sel.daily_series_map(daily)
    names = sorted(smap.keys())
    pairs = sel.pair_tail_dependence(smap, names, tau=TAU, min_shared=MIN_SHARED)
    td = sel.tail_dep_book(pairs)
    print(f'  TailDep {td["TailDep"]:.4f} | tau={TAU} MIN_SHARED={MIN_SHARED} | RAW daily P&L, '
          f'never min(pnl,0) | retention {td["retention_pct"]}%')
    print(f'    exclusion_bias_degeneracy_guarded={td["exclusion_bias_degeneracy_guarded"]} | '
          f'degenerate_excluded_pairs_k_lt3={td["degenerate_excluded_pairs_k_lt3"]}')
    bd = bk.copy()
    bd['day'] = pd.Series(bd['exit_time'].astype(str).values).str[:10].values
    f_conc = sel.fail_conc(bd.groupby('day')['pnl'].sum().values)
    print(f'  FailConc {f_conc:.4f} | worst single-day loss as a multiple of mean daily loss')
    mc = sel.mcvar_per_signal(bk, daily, worst_frac=MCVAR_WORST_FRAC)
    worst_mcvar = float(np.nanmin(mc['mCVaR'])) if len(mc) else float('nan')
    print(f'  mCVaR worst {worst_mcvar:.1f} over {len(mc)} signals | worst '
          f'{int(MCVAR_WORST_FRAC*100)}% of book days')
    fd = full.copy()
    fd['day'] = pd.Series(fd['exit_time'].astype(str).values).str[:10].values
    surv = sel.absolute_survival(fd.groupby('day')['pnl'].sum().values)
    print(f'  ABSOLUTE SURVIVAL worst modelled day {surv["worst_modelled_day"]:.1f} vs ceiling '
          f'{surv["ceiling"]} | FULL population (gap fillers INCLUDED) | '
          f'{"PASS" if surv["passes"] else "BREACH"}')
    verdicts['absolute_survival'] = bool(surv['passes'])
    fwd, mag, eff, valid, thr, mcol, ecol = cp.thrust_thresholds(df, PINNED_W, (PINNED_K,),
                                                                (PINNED_E,))
    cat.assert_episode_thresholds_mechanism_d(_HERE, thr, mcol, ecol,
                                              f'k{int(PINNED_K*100)}', f'e{int(PINNED_E*100)}')
    ev = cp.thrust_events(fwd, mag, eff, valid, thr[(mcol, f'k{int(PINNED_K*100)}')],
                          thr[(ecol, f'e{int(PINNED_E*100)}')], w)
    cs = cp.build_cluster_set(len(df), ev, tr.CONTIGUOUS_TOLERANCE)
    U = cp.eligible_universe(df, w)
    reach = cat.reachable_episodes(cs, df, w, U)
    ev_book, _bk2 = cp.book_events(full)
    cov = sel.coverage_by_direction(ev_book, cs, label='ASSEMBLED BOOK')
    print(f'  UNION TERRAIN COVERAGE — cell W{PINNED_W}/K{int(PINNED_K*100)}/E{int(PINNED_E*100)}, '
          f'per direction, terrain=MARKET entries=BOOK')
    for _i, r in cov.iterrows():
        if str(r['direction']).startswith('BOTH'):
            continue
        d = 1 if r['direction'] == 'UP' else -1
        print(f'    {r["direction"]:<5} raw {r["coverage_pct"]:6.3f}% of '
              f'{int(r["terrain_episodes"])} | reachable denominator {len(reach[d])}')
    ids = {1: [], -1: []}
    ents = {1: [], -1: []}
    for d, lab in ((1, 'LONG'), (-1, 'SHORT')):
        sub = bk[bk['direction'] == lab]
        ents[d] = sub['entry_bar'].values.tolist()
        ids[d] = sub['signal_name'].values.tolist()
    print('  SAME-BAR DEPTH LADDER — DISTINCT-SIGNAL basis, per direction, T=0 then the curve')
    ladder = []
    for n_tol in (1, 5, 10, 15, 20, 25, 30):
        for d, lab in ((1, 'LONG'), (-1, 'SHORT')):
            sizes = sel.clusters_from_entries(ents[d], n_tol, ids[d])
            if not sizes:
                continue
            ladder.append({'tolerance_N': n_tol, 'direction': lab, 'runs': len(sizes),
                           'mean_distinct_depth': round(float(np.mean(sizes)), 3),
                           'max_distinct_depth': int(max(sizes)),
                           'runs_ge3': int(sum(1 for x in sizes if x >= 3)),
                           'population': 'BOOK'})
        if n_tol in (1, 5, 30):
            row = [x for x in ladder if x['tolerance_N'] == n_tol]
            desc = ' | '.join(f'{x["direction"]} mean {x["mean_distinct_depth"]} max '
                              f'{x["max_distinct_depth"]} ge3 {x["runs_ge3"]}' for x in row)
            print(f'    N={n_tol:<3} {desc}')
    nl = int((bk['direction'] == 'LONG').sum())
    ns = int((bk['direction'] == 'SHORT').sum())
    print(f'  DIRECTIONAL COMPOSITION trades LONG {nl} / SHORT {ns} — REPORTED, NEVER TARGETED')
    p = bk['pnl'].values
    print(f'  BOOK trades {len(bk)} | net ${p.sum():.0f} | WR {(p>0).mean()*100:.1f}%')
    cons = pd.DataFrame([
        {'quantity': 'TailDep', 'value': round(td['TailDep'], 6),
         'parameters': f'tau={TAU}, MIN_SHARED={MIN_SHARED}, RAW daily P&L'},
        {'quantity': 'exclusion_bias_degeneracy_guarded',
         'value': td['exclusion_bias_degeneracy_guarded'], 'parameters': 'k>=3 only'},
        {'quantity': 'degenerate_excluded_pairs_k_lt3',
         'value': td['degenerate_excluded_pairs_k_lt3'], 'parameters': 'lambda is 1/tau at k=1'},
        {'quantity': 'FailConc', 'value': round(f_conc, 6),
         'parameters': 'worst single-day loss / mean daily loss'},
        {'quantity': 'worst_mCVaR', 'value': round(worst_mcvar, 4),
         'parameters': f'worst {int(MCVAR_WORST_FRAC*100)}% of book days'},
        {'quantity': 'absolute_survival_worst_day', 'value': round(surv['worst_modelled_day'], 2),
         'parameters': f'FULL population incl. gap fillers, ceiling {-FTMO_DAILY_CEILING}'},
        {'quantity': 'absolute_survival_passes', 'value': bool(surv['passes']),
         'parameters': 'HARD CONSTRAINT - non-zero exit on breach'},
        {'quantity': 'directional_composition', 'value': f'LONG {nl} / SHORT {ns}',
         'parameters': 'REPORTED, NEVER TARGETED'},
    ])
    _write(os.path.join(out_dir, 'book_constraints.csv'), cons, [
        'DOT item 16 - APPENDIX B constraint verdicts for an ASSEMBLED BOOK',
        f'book={os.path.basename(book_path)} signals={len(book)} input_sha={input_sha}',
        'These are SET properties: they have no per-signal value and fabricating one would be worse '
        'than omitting it. Estimators are IMPORTED from selection.py, never reimplemented.',
        'PROPERTY OF THE BOOK, except the terrain denominators which are MARKET.'])
    _write(os.path.join(out_dir, 'book_coverage.csv'), cov, [
        'DOT item 16 - union terrain coverage, PER DIRECTION',
        f'pinned cell W{PINNED_W}/K{int(PINNED_K*100)}/E{int(PINNED_E*100)} | '
        f'mask {tr.eligibility_label()}',
        f'reachable denominators UP {len(reach[1])} DOWN {len(reach[-1])} (MARKET)',
        'terrain = MARKET, entries = BOOK. Never pooled across directions.'])
    _write(os.path.join(out_dir, 'book_depth_ladder.csv'), pd.DataFrame(ladder), [
        'DOT item 16 - same-bar depth ladder, DISTINCT-SIGNAL basis (item 4)',
        'T=0 is tolerance N=1. The full tolerance curve follows so N is chosen from evidence.',
        'PROPERTY OF THE BOOK. Counts only.'])
    _write(os.path.join(out_dir, 'book_mcvar.csv'), mc, [
        'DOT item 16 - per-signal marginal tail contribution',
        f'worst {int(MCVAR_WORST_FRAC*100)}% of book days | PROPERTY OF THE BOOK'])
    print('  F10 CONVERGENCE-DENSITY LADDER')
    density_ladder(df, sigs, ad, st, w, out_dir, book_path)
    breaches = [k for k, v in verdicts.items() if v is False]
    rec = {'utc_timestamp': time.strftime('%Y-%m-%dT%H:%M:%SZ', time.gmtime()),
           'book_sha256': _sha_file(book_path), 'input_sha': input_sha, 'code_sha': _code_sha(),
           'book_path': os.path.abspath(book_path), 'signals': int(len(book)),
           'trades': int(len(bk)), 'net': round(float(p.sum()), 2),
           'TailDep': round(td['TailDep'], 6), 'FailConc': round(f_conc, 6),
           'worst_mCVaR': round(worst_mcvar, 4),
           'absolute_survival_worst_day': round(surv['worst_modelled_day'], 2),
           'absolute_survival_passes': bool(surv['passes']),
           'directional_composition': f'LONG {nl} / SHORT {ns}',
           'breaches': breaches, 'verdict': 'BREACH' if breaches else 'PASS'}
    with open(os.path.join(out_dir, 'book_scored.jsonl'), 'a', encoding='utf-8') as f:
        f.write(json.dumps(rec, sort_keys=True) + '\n')
    print(f'  attestation appended -> book_scored.jsonl (ATTESTATION RECORD: exempt from the '
          f'determinism rule and REQUIRED to carry wall-clock)')
    if breaches:
        print(f'BREACH: {", ".join(breaches)} — non-zero exit so a wrapper cannot ignore it.')
        return 1
    print('VERDICT PASS — every hard constraint holds on this book.')
    return 0



DENSITY_LINE = re.compile(
    r'count>=(\d+)\s+\[(\w+)\s*\]\s+bars\s+([\d,]+)\s+\|\s+trades\s+(\d+)\s+\|\s+'
    r'aggPF\s+([\d.]+)\s+\|\s+WR\s+([\d.]+)%\s+\|\s+worst-day\s+\$\s*(-?[\d,]+)\s+\|\s+'
    r'hard-stop\s+(\d+)\s+\|\s+spread\s+([\d.]+)->([\d.]+)')
DENSITY_DIR = re.compile(r'^\s+(LONG|SHORT) subset')


def density_ladder(df, book, adaptive, structural, warmup, out_dir, book_path):
    """Item F10: the convergence-density ladder, on the SAME book score_book is given.

    THE LADDER IS NOT REIMPLEMENTED. f0.density_sweep is the ratified measurement
    and is called directly with f0.DENSITY_K_BANDS; a second implementation would
    drift from it, which is the failure this project has met most often.

    density_sweep PRINTS and returns nothing, and
    scanners/triple_convergence_and_d2ddir.py is a SCANNER that may not be
    edited, so its stdout is captured, echoed so it still reaches the console and
    the run log, and parsed into the CSV. The parse is of a fixed format string
    in that file - if the format ever changes the CSV comes out empty and says
    so, rather than silently wrong.

    run_density() is NOT used: it loads the sealed baseline itself and defaults
    to recommended_set_76.csv, a superseded pre-reconstruction file. The book
    here comes from --book and the frame from --data, so that default is not
    reachable on this path.
    """
    buf = io.StringIO()
    try:
        with contextlib.redirect_stdout(buf):
            f0.density_sweep(df, book, f0.DENSITY_K_BANDS, adaptive, structural, warmup)
    except Exception as exc:
        print(f'  DENSITY LADDER FAILED: {type(exc).__name__}: {str(exc)[:120]}', flush=True)
        return None
    text = buf.getvalue()
    print(text, flush=True)
    rows = []
    direction = ''
    for ln in text.split('\n'):
        m0 = DENSITY_DIR.match(ln)
        if m0:
            direction = m0.group(1)
            continue
        m = DENSITY_LINE.search(ln)
        if m:
            rows.append({'k': int(m.group(1)), 'direction': direction,
                         'survival': m.group(2), 'bars': int(m.group(3).replace(',', '')),
                         'trades': int(m.group(4)), 'PF': float(m.group(5)),
                         'WR': float(m.group(6)),
                         'worst_day_usd': float(m.group(7).replace(',', '')),
                         'hard_stop_days': int(m.group(8)),
                         'pf_base': float(m.group(9)), 'pf_stress': float(m.group(10))})
    base = os.path.splitext(os.path.basename(book_path))[0]
    path = os.path.join(out_dir, f'density_{base}.csv')
    _write(path, pd.DataFrame(rows), [
        'DOT F10 convergence-density ladder for an ASSEMBLED BOOK',
        f'book={os.path.basename(book_path)} signals={len(book)} k_bands={f0.DENSITY_K_BANDS}',
        'Entry is restricted to bars where >= k of the book\'s own set-conditions co-fire. This is '
        'how a book is screened for whether its convergence tiers actually PAY: every depth figure '
        'in circulation was INFERRED by grouping entry bars, not measured by the engine.',
        'Produced by scanners/triple_convergence_and_d2ddir.density_sweep - the ratified '
        'measurement, called not reimplemented. Rows absent for a k band mean that band fell below '
        'MIN_TRADES and was not scored.',
        'PROPERTY OF THE BOOK.'])
    if not rows:
        print(f'  DENSITY LADDER: 0 rows parsed - either every k band fell below MIN_TRADES or the '
              f'printed format in the scanner changed. {path} written empty rather than wrong.',
              flush=True)
    else:
        print(f'  density_{base}.csv: {len(rows)} ladder rows '
              f'(k {min(r["k"] for r in rows)}..{max(r["k"] for r in rows)}, '
              f'{len(set(r["direction"] for r in rows))} directions)', flush=True)
    return rows


def _write(path, frame, header):
    tmp = path + '.tmp'
    with open(tmp, 'w', encoding='utf-8', newline='') as f:
        for ln in header:
            f.write(f'# {ln}\n')
        frame.to_csv(f, index=False, lineterminator='\n')
    os.replace(tmp, path)


def main():
    ap = argparse.ArgumentParser(description='Score an operator-assembled book (item 16).')
    ap.add_argument('--book', required=True)
    ap.add_argument('--data', required=True)
    ap.add_argument('--out', required=True)
    a = ap.parse_args()
    sys.exit(score(a.book, a.data, a.out))


if __name__ == '__main__':
    main()
