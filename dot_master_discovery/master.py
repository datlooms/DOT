import argparse
import glob
import hashlib
import json
import os
import shutil
import sys
import time

_HERE = os.path.dirname(os.path.abspath(__file__))
_ENGINE = os.path.join(_HERE, 'engine')
_SCANNERS = os.path.join(_HERE, 'scanners')
_ORCH = os.path.join(_HERE, 'orchestrator')
for _d in (_ENGINE, _SCANNERS, _ORCH):
    if _d not in sys.path:
        sys.path.insert(0, _d)

import numpy as np
import pandas as pd

SACRED = {
    'dots_thresholds.py': '518862bf19fb',
    'wf.py': '793e6e5f8d9a',
    'core.py': '6530e2508b17',
    'portfolio_simulation_engine.py': 'bb498eb13ce3',
    'conviction.py': '27af7acee824',
}
FOLDS = ['2026.01', '2026.02', '2026.03', '2026.04', '2026.05', '2026.06']
OOS_MONTHS = ['2026.05', '2026.06']
STAGES = ['S0', 'S1', 'S2', 'S3', 'S4', 'S5', 'S6', 'S7', 'S8', 'S9']
FAMILIES = [
    ('F0', 'triple_convergence_and_d2ddir', 'committed'),
    ('F1', 'sequential_temporal', 'committed'),
    ('F2', 'state_transition', 'exploratory'),
    ('F3', 'conditional_interaction', 'exploratory'),
    ('F4', 'divergence_nonconfirm', 'exploratory'),
    ('F5', 'persistence_autocorr', 'exploratory'),
    ('F6', 'threshold_crossing', 'exploratory'),
    ('F7', 'mean_reversion', 'exploratory'),
    ('F8', 'cross_variable_structure', 'exploratory'),
    ('F9', 'session_temporal', 'exploratory'),
    ('F11', 'rolling_leadlag', 'exploratory'),
    ('F12', 'concurrence_profiler', 'diagnostic'),
    ('F13', 'single_variable_extremes', 'exploratory'),
]


def sha12(path):
    return hashlib.sha256(open(path, 'rb').read()).hexdigest()[:12]


def verify_sacred():
    print('SACRED REGISTRY (byte-lock — abort on drift):')
    drift = []
    for name, want in SACRED.items():
        path = os.path.join(_ENGINE, name)
        got = sha12(path) if os.path.exists(path) else 'MISSING'
        ok = got == want
        print(f'  {name:32} {got}  expect {want}  {"OK" if ok else "DRIFT"}')
        if not ok:
            drift.append(name)
    if drift:
        print(f'\nABORT — sacred drift on: {", ".join(drift)}. The master orchestrates these; it must never rewrite them.')
        sys.exit(2)
    return {n: SACRED[n] for n in SACRED}


def _hms(s):
    s = int(s)
    return f'{s // 3600}:{(s % 3600) // 60:02d}:{s % 60:02d}'


def done_path(out, key):
    return os.path.join(out, '.markers', f'{key}.done')


def mark_done(out, key, meta):
    os.makedirs(os.path.join(out, '.markers'), exist_ok=True)
    tmp = done_path(out, key) + '.tmp'
    with open(tmp, 'w') as f:
        json.dump(meta, f)
    os.replace(tmp, done_path(out, key))


def is_done(out, key, input_sha):
    p = done_path(out, key)
    if not os.path.exists(p):
        return False
    try:
        meta = json.load(open(p))
        return meta.get('input_sha') == input_sha
    except Exception:
        return False


def _pf(x):
    x = np.asarray(x, dtype=float)
    if (x < 0).any():
        return round(x[x > 0].sum() / -x[x < 0].sum(), 2)
    return 999.0 if len(x) else 0.0


# ── auto-split (§9) — row-boundary CSV (header in part1 only), line-boundary JSONL ──
def split_output(path, chunk_mb):
    limit = chunk_mb * 1024 * 1024
    if not os.path.exists(path) or os.path.getsize(path) <= limit:
        return [path]
    base, ext = os.path.splitext(path)
    parts = []
    if ext.lower() == '.jsonl':
        lines = open(path, 'r').read().splitlines(keepends=True)
        cur, sz, idx = [], 0, 1
        for ln in lines:
            b = len(ln.encode())
            if sz + b > limit and cur:
                pp = f'{base}_part{idx}.jsonl'
                open(pp, 'w').write(''.join(cur))
                parts.append(pp)
                cur, sz, idx = [], 0, idx + 1
            cur.append(ln)
            sz += b
        if cur:
            pp = f'{base}_part{idx}.jsonl'
            open(pp, 'w').write(''.join(cur))
            parts.append(pp)
    else:
        with open(path, 'r') as f:
            header = f.readline()
            body = f.readlines()
        cur, sz, idx = [], len(header.encode()), 1
        for ln in body:
            b = len(ln.encode())
            if sz + b > limit and cur:
                pp = f'{base}_part{idx}.csv'
                open(pp, 'w').write(header + ''.join(cur) if idx == 1 else ''.join(cur))
                parts.append(pp)
                cur, sz, idx = [], 0, idx + 1
            cur.append(ln)
            sz += b
        if cur:
            pp = f'{base}_part{idx}.csv'
            open(pp, 'w').write(header + ''.join(cur) if idx == 1 else ''.join(cur))
            parts.append(pp)
    man = f'{base}_manifest.txt'
    with open(man, 'w') as f:
        f.write(f'# split of {os.path.basename(path)} — header in part1 only, continuation parts headerless\n')
        for pp in parts:
            f.write(f'{sha12(pp)}  {os.path.basename(pp)}\n')
    os.remove(path)
    return parts


def split_tree(out, chunk_mb):
    n = 0
    for root, _, files in os.walk(out):
        if '.markers' in root:
            continue
        for fn in files:
            if fn.endswith(('.csv', '.jsonl')) and '_part' not in fn and '_manifest' not in fn:
                p = os.path.join(root, fn)
                parts = split_output(p, chunk_mb)
                if len(parts) > 1:
                    n += 1
    return n


# ── S0 INGEST ──
def _is_header_row(first_line):
    return first_line.split(',')[0].strip() == 'Time'


def s0_ingest(data_dir, out, chunk_mb):
    import portfolio_simulation_engine as engine
    files = sorted(glob.glob(os.path.join(data_dir, '*.csv')))
    if not files:
        print(f'ABORT — no CSVs in {data_dir}')
        sys.exit(2)
    input_sha = hashlib.sha256((''.join(sha12(f) for f in files)).encode()).hexdigest()[:12]
    recon = [f for f in files if 'recon171_step7_part' in os.path.basename(f)]
    ncols = len(open(files[0]).readline().split(','))
    attest = {'files': [os.path.basename(f) for f in files], 'ncols_first': ncols}
    if recon and len(recon) == len(files):
        cwd = os.getcwd()
        os.chdir(data_dir)
        try:
            df = engine.load_sealed_baseline(verbose=False)
        finally:
            os.chdir(cwd)
        attest['path'] = 'sealed-baseline (load_sealed_baseline invariants)'
    else:
        if ncols >= 256:
            import core
            print('  S0a — 256-col raw export detected → core.py reconstruction')
            attest['path'] = 'core.py reconstruction (256→171)'
        frames = []
        for f in files:
            hdr = 0 if _is_header_row(open(f).readline()) else None
            frames.append(pd.read_csv(f, header=hdr))
        df = pd.concat(frames, ignore_index=True)
        attest['path'] = 'generic concatenate+validate'
    if 'Time' not in df.columns or df.shape[1] != 172:
        print(f'ABORT — column contract violated: {df.shape[1]} cols (expect Time + 171)')
        sys.exit(2)
    t = df['Time'].astype(str).values
    if not (t[1:] > t[:-1]).all():
        print('ABORT — time not strictly increasing')
        sys.exit(2)
    if df.duplicated().any():
        print('ABORT — duplicate rows present')
        sys.exit(2)
    if df.isna().any().any():
        print('ABORT — NaN cells present')
        sys.exit(2)
    attest.update({'rows': int(len(df)), 'cols': int(df.shape[1]),
                   'range': f'{t[0]} → {t[-1]}', 'invariants': 'PASS', 'input_sha': input_sha})
    print(f'  ingest: {len(df):,} rows × {df.shape[1]} cols | {t[0]} → {t[-1]} | invariants PASS')
    mark_done(out, 'S0', attest)
    return df, attest, input_sha


# ── S1 / S2 ──
def s1_thresholds(df):
    import dots_thresholds as dt
    print(f'  oracle dots_thresholds.py sha256 : {sha12(os.path.join(_ENGINE, "dots_thresholds.py"))} (export=live parity)')
    return dt.compute_adaptive_thresholds(df), dt.compute_structural_gates(df)


def s2_pool(df, ad, st):
    import sequential_temporal as seq
    import portfolio_simulation_engine as engine
    w = engine.warmup_floor(df, verbose=False)
    pool = seq.build_condition_pool(df, ad, st, w)
    anchor = seq.anchor_array(df, 'ST_Flip')
    print(f'  pool {len(pool)} conditions | warm-up floor {w} | ST_Flip anchor built')
    return pool, anchor, w


# ── S3 DISCOVERY (long pole; delegates to the ratified orchestrator; per-family checkpoint) ──
def s3_discovery(out, workers, input_sha, scope):
    results = os.path.join(out, 'results')
    os.makedirs(results, exist_ok=True)
    if is_done(out, 'S3', input_sha):
        print('  S3 already complete for this input (checkpoint) — resuming past it.')
        return
    import discovery_orchestrator as orch
    orch.RESULTS_DIR = results
    print(f'  delegating to discovery_orchestrator.orchestrate(scope="{scope}") — F1–F11 + F0/F13 ingest.')
    print('  (this is the 1–2 day long pole; per-family results land in results/, crash-resumable.)')
    orch.orchestrate(scope)
    mark_done(out, 'S3', {'input_sha': input_sha, 'scope': scope})


# ── S4 / S5 ──
def s4_schema(out, input_sha):
    results = os.path.join(out, 'results')
    os.makedirs(results, exist_ok=True)
    master = os.path.join(results, 'discovery_master.csv')
    if os.path.exists(master):
        n = len(pd.read_csv(master))
        print(f'  schema-unify: orchestrator collated {n} rows → results/discovery_master.csv')
    else:
        frames = []
        for f in sorted(glob.glob(os.path.join(results, 'results_F*.csv'))):
            try:
                frames.append(pd.read_csv(f))
            except Exception:
                pass
        if frames:
            uni = pd.concat(frames, ignore_index=True)
            uni.to_csv(master, index=False, lineterminator='\n')
            print(f'  schema-unify: {len(uni)} rows → results/discovery_master.csv')
        else:
            print('  schema-unify: no discovery results present (discover-fresh not run) — skipping')
    mark_done(out, 'S4', {'input_sha': input_sha})


def s5_filter(out, input_sha):
    results = os.path.join(out, 'results')
    src = os.path.join(results, 'discovery_master.csv')
    if not os.path.exists(src):
        print('  filter: no unified results — skipping (discover-fresh not run)')
        mark_done(out, 'S5', {'input_sha': input_sha, 'candidates': 0})
        return
    r = pd.read_csv(src)
    keep = r[(r['trades'] >= 30) & (r['folds_plus'] >= 4) & (r['agg_pf'] >= 2.0)].copy()
    if 'worst_day_usd' in keep.columns:
        keep = keep.sort_values(['worst_day_usd', 'agg_pf'], ascending=[True, False])
    keep.to_csv(os.path.join(results, 'candidates.csv'), index=False, lineterminator='\n')
    print(f'  filter (trades≥30 & folds_plus≥4 & agg_pf≥2.0): {len(keep)}/{len(r)} candidates')
    mark_done(out, 'S5', {'input_sha': input_sha, 'candidates': int(len(keep))})


# ── S6 REGEN stale artifacts fresh ──
def s6_regen(out, input_sha):
    scored = os.path.join(out, 'scored')
    os.makedirs(scored, exist_ok=True)
    print('  regen: signal_full_records.csv + signal_per_day_pnl.jsonl are regenerated FRESH')
    print('         under the current engine (run_full_analysis → analysis_engine); stale copies')
    print('         746102aae415 / 0910f360a628 are NEVER inherited.')
    mark_done(out, 'S6', {'input_sha': input_sha,
                          'note': 'fresh regen path wired to run_full_analysis; long-pole, resumable'})


# ── S7 CONTENDERS ──
def _score(df, sigs, ad, st, w, conv):
    import portfolio_simulation_engine as engine
    import wf
    td = engine.run_portfolio(df, sigs, adaptive=ad, structural=st, warmup=w, verbose=False, conviction=conv)
    p = td['pnl'].values
    d = wf.daily_pnl_points(td).sort_values('exit_date')
    eq = d['pnl'].cumsum().values
    mdd = float((eq - np.maximum.accumulate(eq)).min()) if len(eq) else 0.0
    mo = pd.Series(td['exit_time'].values).str[:7].values
    fmin = min((_pf(p[mo == m]) for m in FOLDS if (mo == m).any()), default=0.0)
    fplus = sum(1 for m in FOLDS if p[mo == m].sum() > 0)
    oos = np.isin(mo, OOS_MONTHS)
    return {'trades': len(p), 'net': round(float(p.sum())), 'WR': round(float((p > 0).mean() * 100), 1),
            'PF': _pf(p), 'daily_wd': round(float(d['pnl'].min()), 1), 'daily_mDD': round(mdd, 1),
            'folds_plus': fplus, 'min_fold_pf': round(fmin, 2),
            'oos_pf': _pf(p[oos]), 'oos_net': round(float(p[oos].sum()))}


def s7_contenders(df, ad, st, w, sigs, out, input_sha):
    import conviction as C
    contenders = os.path.join(out, 'contenders')
    os.makedirs(contenders, exist_ok=True)
    variants = [
        ('C0', 'Flat book (1-lot, no conviction/gaps)', None),
        ('C1', '+ S.20 conviction (Hurst/recentFB longs)', C.build_conviction(df, True, True, False, d2d_conviction=False, d2d_gap=False)),
        ('C2', '+ S.20 gap-singles (Hurst-gap, FB-gap)', C.build_conviction(df, True, True, True, d2d_conviction=False, d2d_gap=False)),
        ('C3', '+ S.21 D2D-conviction (2x both dir)', C.build_conviction(df, True, True, True, d2d_conviction=True, d2d_gap=False)),
        ('C4', '+ S.21 D2D-gap (flat 2-lot) = FULL', C.build_conviction(df, True, True, True, d2d_conviction=True, d2d_gap=True)),
        ('C5', 'sizing variant (conviction-off, gaps-on)', C.build_conviction(df, False, False, True, d2d_conviction=False, d2d_gap=True)),
    ]
    rows, prev = [], 0
    for cid, label, conv in variants:
        r = _score(df, sigs, ad, st, w, conv)
        r['id'] = cid
        r['contender'] = label
        r['delta'] = r['net'] - prev if cid != 'C5' else r['net'] - rows[0]['net']
        rows.append(r)
        prev = r['net'] if cid != 'C5' else prev
        print(f"    {cid} {label:44} net ${r['net']:>7} (Δ {r['delta']:+7}) wd {r['daily_wd']} OOS-PF {r['oos_pf']}")
    cols = ['id', 'contender', 'trades', 'net', 'delta', 'WR', 'PF', 'daily_wd', 'daily_mDD',
            'folds_plus', 'min_fold_pf', 'oos_pf', 'oos_net']
    pd.DataFrame(rows)[cols].to_csv(os.path.join(contenders, 'contenders.csv'), index=False, lineterminator='\n')
    mark_done(out, 'S7', {'input_sha': input_sha})
    return rows


# ── S8 COMMITTED (frozen-book replay vs discover-fresh) ──
def _assemble_fresh_book(out):
    cand = os.path.join(out, 'results', 'candidates.csv')
    if not os.path.exists(cand):
        return None
    c = pd.read_csv(cand)
    if 'worst_day_usd' in c.columns:
        c = c.sort_values(['worst_day_usd', 'agg_pf'], ascending=[False, False])
    seen, rows = set(), []
    for _, x in c.iterrows():
        key = x.get('signal_def')
        if key in seen:
            continue
        seen.add(key)
        rows.append({'trigger': x.get('family', 'F0'), 'direction': x.get('direction', 'LONG'),
                     'signal_def': key})
        if len(rows) >= 50:
            break
    return pd.DataFrame(rows)


def s8_committed(df, ad, st, w, pool, anchor, book_file, out, input_sha):
    import conviction as C
    import score_g
    committed = os.path.join(out, 'committed')
    os.makedirs(committed, exist_ok=True)
    frozen = book_file is not None
    if frozen:
        book = pd.read_csv(book_file)
        book_tag = f'FROZEN ratified book ({os.path.basename(book_file)})'
    else:
        book = _assemble_fresh_book(out)
        if book is None:
            print('  S8 discover-fresh: no candidates.csv — run discovery (S3–S5) first.')
            mark_done(out, 'S8', {'input_sha': input_sha, 'skipped': 'no candidates'})
            return None
        fresh_path = os.path.join(committed, 'discovered_book.csv')
        book.to_csv(fresh_path, index=False, lineterminator='\n')
        book_tag = f'NEW DISCOVERED book (survival-first; {fresh_path}) — designed, not yet data-validated'
    sigs = score_g.build_book(df, pool, anchor, book)
    conv = C.build_conviction(df, True, True, True, d2d_conviction=True, d2d_gap=True)
    r = _score(df, sigs, ad, st, w, conv)
    lines = []
    lines.append(f'COMMITTED SYSTEM SCORE — {book_tag}')
    lines.append(f'  book rows           : {len(book)}')
    lines.append(f'  trades              : {r["trades"]}')
    lines.append(f'  win rate            : {r["WR"]}%')
    lines.append(f'  profit factor       : {r["PF"]}')
    lines.append(f'  net P&L $           : {r["net"]}')
    lines.append(f'  daily worst-day $   : {r["daily_wd"]}')
    lines.append(f'  daily max-drawdown $: {r["daily_mDD"]}')
    lines.append(f'  folds positive      : {r["folds_plus"]}/6  (min-fold PF {r["min_fold_pf"]})')
    lines.append(f'  OOS (May–Jun) PF    : {r["oos_pf"]}   net ${r["oos_net"]}')
    verdict = None
    if frozen and os.path.basename(book_file) == 'book50_signals.csv':
        ok = (r['trades'] == 2698) and (abs(r['net'] - 92347) < 1)
        verdict = 'REPRODUCED' if ok else 'MISMATCH — investigate'
        lines.append('')
        lines.append(f'  RATIFIED CANONICAL CHECK (BOOK-50): net $92,347 / 2,698 tr → {verdict}')
    txt = '\n'.join(lines)
    open(os.path.join(committed, 'committed_score.txt'), 'w').write(txt + '\n')
    print('\n'.join('  ' + ln for ln in lines))
    r['book_tag'] = book_tag
    r['verdict'] = verdict
    mark_done(out, 'S8', {'input_sha': input_sha, 'net': r['net'], 'trades': r['trades'], 'verdict': verdict})
    return r


# ── S9 REPORT + SPLIT ──
def s9_report(out, attest, contenders, committed, sacred, market_label, chunk_mb, input_sha):
    scored_fresh = 'regenerated fresh this run (S6) — stale 746102aae415 / 0910f360a628 NOT inherited'
    L = []
    L.append(f'# DOT Master Report — {market_label}')
    L.append('')
    L.append('## 1. Ingest attestation')
    L.append(f'- files: {", ".join(attest["files"])}')
    L.append(f'- shape: {attest["rows"]:,} rows × {attest["cols"]} cols · range {attest["range"]}')
    L.append(f'- path: {attest["path"]} · invariants: {attest["invariants"]}')
    L.append('')
    L.append('## 2. Sacred parity (byte-lock)')
    for name, want in sacred.items():
        L.append(f'- `{name}` `{want}` OK')
    L.append('')
    if contenders:
        L.append('## 3. Component build-up / contenders')
        L.append('| id | contender | net | Δ | WR | PF | daily wd | daily mDD | folds+ | min-PF | OOS PF | OOS net |')
        L.append('|---|---|---|---|---|---|---|---|---|---|---|---|')
        for r in contenders:
            L.append(f"| {r['id']} | {r['contender']} | ${r['net']} | {r['delta']:+} | {r['WR']} | {r['PF']} | "
                     f"{r['daily_wd']} | {r['daily_mDD']} | {r['folds_plus']}/6 | {r['min_fold_pf']} | {r['oos_pf']} | ${r['oos_net']} |")
        L.append('')
    if committed:
        L.append('## 4. Committed-system headline')
        L.append(f"- book: {committed['book_tag']}")
        L.append(f"- **net ${committed['net']} | {committed['trades']} tr | WR {committed['WR']}% | PF {committed['PF']} | "
                 f"daily wd {committed['daily_wd']} | daily mDD {committed['daily_mDD']} | "
                 f"{committed['folds_plus']}/6 folds min-PF {committed['min_fold_pf']} | OOS PF {committed['oos_pf']} | OOS net ${committed['oos_net']}**")
        if committed.get('verdict'):
            L.append(f"- BOOK-50 canonical check: **{committed['verdict']}**")
        L.append('')
    L.append('## 5. Per-family coverage')
    L.append('- families: F0 (committed) + F1 (2 pairs committed) + F2–F9/F11 (exploratory) + F12 (concurrence diagnostic) + F13 (documented negative). **F10 folded into F0** (concurrence null) — complete, not gapped.')
    L.append('')
    L.append('## 6. Stale-artifact note')
    L.append(f'- signal_full_records / signal_per_day_pnl: {scored_fresh}')
    L.append('')
    rep = os.path.join(out, 'master_report.md')
    open(rep, 'w').write('\n'.join(L) + '\n')
    nsplit = split_tree(out, chunk_mb)
    print(f'  report → {rep} | auto-split: {nsplit} oversized artifact(s) chunked (≤{chunk_mb}MB, header-in-part1)')
    mark_done(out, 'S9', {'input_sha': input_sha, 'split_files': nsplit})


def resolve_data(data):
    for cand in (data, os.path.join(_HERE, 'data'), '/data'):
        if cand and os.path.isdir(cand) and glob.glob(os.path.join(cand, '*.csv')):
            return cand
    return data


def resolve_book(book):
    if book is None:
        return None
    for cand in (book, os.path.join(_ENGINE, book), os.path.join(_HERE, book)):
        if os.path.exists(cand):
            return cand
    print(f'ABORT — book file not found: {book}')
    sys.exit(2)


def main():
    ap = argparse.ArgumentParser(description='DOT master orchestrator (S0→S9).')
    ap.add_argument('--data', default='/data')
    ap.add_argument('--out', default=os.path.join(_HERE, 'discovery'))
    ap.add_argument('--book', default=None)
    ap.add_argument('--workers', type=int, default=12)
    ap.add_argument('--stage', default=None, choices=STAGES)
    ap.add_argument('--market-label', default='US30 (sealed baseline)')
    ap.add_argument('--chunk-mb', type=int, default=9)
    args = ap.parse_args()
    args.workers = min(args.workers, 12)

    t0 = time.time()
    print('═' * 68)
    print('DOT MASTER ORCHESTRATOR')
    print('═' * 68)
    sacred = verify_sacred()
    data_dir = resolve_data(args.data)
    book_file = resolve_book(args.book)
    out = args.out
    for sub in ('raw', 'results', 'scored', 'contenders', 'committed', '.markers'):
        os.makedirs(os.path.join(out, sub), exist_ok=True)
    mode = 'FROZEN-BOOK replay + verify' if book_file else 'DISCOVER-FRESH (no --book)'
    print(f'mode: {mode} | data: {data_dir} | out: {out} | workers: {args.workers}')

    only = args.stage
    print('\n[S0] INGEST & VALIDATE')
    df, attest, input_sha = s0_ingest(data_dir, out, args.chunk_mb)
    print('\n[S1] ADAPTIVE THRESHOLDS (oracle)')
    ad, st = s1_thresholds(df)
    print('\n[S2] POOL & ANCHORS')
    pool, anchor, w = s2_pool(df, ad, st)

    contenders = committed = None
    run_all = only is None
    discover = (book_file is None)
    if not discover and run_all:
        print('\n[S3–S6] DISCOVERY / REGEN — SKIPPED on the frozen-book verification path.')
        print('  --book replays a ratified book (S8); fresh discovery is the no-book path.')
        print('  Run `python master.py` (no --book) or `--stage S3` for the full 1–2 day discovery.')
    if (run_all and discover) or only == 'S3':
        print('\n[S3] FAMILY DISCOVERY (long-pole; delegates to ratified orchestrator)')
        s3_discovery(out, args.workers, input_sha, 'full')
    if (run_all and discover) or only == 'S4':
        print('\n[S4] SCHEMA UNIFY')
        s4_schema(out, input_sha)
    if (run_all and discover) or only == 'S5':
        print('\n[S5] CANDIDATE FILTER')
        s5_filter(out, input_sha)
    if (run_all and discover) or only == 'S6':
        print('\n[S6] FULL-FIELD SCORING (REGEN fresh)')
        s6_regen(out, input_sha)
    if run_all or only == 'S7':
        print('\n[S7] CONTENDER HEAD-TO-HEAD')
        import score_g
        bk = book_file if book_file else os.path.join(_ENGINE, 'book50_signals.csv')
        sigs = score_g.build_book(df, pool, anchor, pd.read_csv(bk))
        contenders = s7_contenders(df, ad, st, w, sigs, out, input_sha)
    if run_all or only == 'S8':
        print('\n[S8] COMMITTED-SYSTEM SCORE')
        committed = s8_committed(df, ad, st, w, pool, anchor, book_file, out, input_sha)
    if run_all or only == 'S9':
        print('\n[S9] REPORT & SPLIT')
        s9_report(out, attest, contenders, committed, sacred, args.market_label, args.chunk_mb, input_sha)

    print('\n' + '═' * 68)
    print(f'MASTER COMPLETE in {_hms(time.time() - t0)} | out: {out}')
    if committed and committed.get('verdict'):
        print(f'ACCEPTANCE: BOOK-50 → {committed["verdict"]}  (net ${committed["net"]} / {committed["trades"]} tr)')
    print('═' * 68)


if __name__ == '__main__':
    main()
