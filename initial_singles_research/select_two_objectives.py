"""select_two_objectives.py — SELECT §9.3: the two proven objectives, wired.

  solo    build the TRUE-SOLO loss-day matrix ONE SIGNAL AT A TIME (each VALID signal run alone through
          run_portfolio at floor 1 / cap 21 / ATR>=20 / no tier gates). Resumable; deterministic; no RNG.
  select  loss-day decorrelation (per direction, fixed counts, cumulative coverage, net>0 pool, seed set size 0)
          + chance-pricing (E < 1 from the catalogue; direction-corrected ONLY if a per-direction null vector is
          supplied, otherwise the family E is used and a WARN line is printed) -> union arm CSV.
Usage: python select_two_objectives.py solo [--start i --end j]     python select_two_objectives.py select
"""
import os, sys, json, argparse, numpy as np, pandas as pd
HERE = os.path.dirname(os.path.abspath(__file__))
for d in (HERE, os.path.join(HERE, 'engine'), os.path.join(HERE, 'scanners'), os.path.join(HERE, 'orchestrator')):
    if d not in sys.path: sys.path.insert(0, d)
import master as MS, adm_engine as AE, cluster_profiler as CP, conviction as C, score_g

CATALOGUE = os.environ.get('DOT_CATALOGUE', '/mnt/project/catalogue_F0.csv')
OUT = os.path.join(HERE, 'discovery', 'select2'); os.makedirs(OUT, exist_ok=True)
SOLO_PATH = os.environ.get('DOT_SOLO_PATH', os.path.join(OUT, 'solo_daily.jsonl'))
K_DECORR = {'LONG': 70, 'SHORT': 50}      # recorded OPTION-B per-direction counts; fixed before scoring (SELECT2_PREREG.md)
LOSS_PENALTY = 50                          # seed rule from §2.1: max(net - 50 * solo_loss_days)
E_CEILING = 1.0                            # chance-pricing: E < 1

def load_frame():
    out = os.path.join(HERE, 'discovery', 'select2', 'frame'); os.makedirs(out, exist_ok=True)
    df, attest, input_sha = MS.s0_ingest('data', out)
    ad, st = MS.s1_thresholds(df); pool, anchor, w = MS.s2_pool(df, ad, st)
    cfg, cfg_path = MS.book_config_for('engine/whole_dot_signals.csv'); cv = cfg['conviction']
    conv = C.build_conviction(df, bool(cv['hurst']), bool(cv['recentfb']), bool(cv['d2d']), d2d_conviction=bool(cv['d2d_conviction']), d2d_gap=bool(cv['d2d_gap']))
    return df, ad, st, pool, anchor, w, cfg, conv, input_sha

def configure_solo(df, cfg):
    AE.ADMISSION_RULE = 'FLOORED'; AE.MAX_POSITIONS = int(cfg['max_positions']); AE.ADM_FLOOR = {1: 1, -1: 1}
    AE.ADM_GATES = {'ATR': df['ATR_1M'].values.astype(float), 'atr_min': float(cfg['global_gate']['value'])}
    AE.ADM_TIERGATES = {}

def valid_rows():
    c = pd.read_csv(CATALOGUE, comment='#'); c = c[c.verdict == 'VALID'].reset_index(drop=True)
    c['key'] = c.signal_def + '|' + c.direction; return c

def cmd_solo(args):
    df, ad, st, pool, anchor, w, cfg, conv, input_sha = load_frame()
    c = valid_rows(); done = set()
    if os.path.exists(SOLO_PATH):
        for line in open(SOLO_PATH): done.add(json.loads(line)['key'])
    lo, hi = args.start, (args.end if args.end is not None else len(c))
    print(f'[solo] VALID rows {len(c)} · already built {len(done)} · range {lo}:{hi} · frame {input_sha}', flush=True)
    configure_solo(df, cfg); AE._assert_admission_configured() if hasattr(AE, '_assert_admission_configured') else None
    for i in range(lo, hi):
        r = c.iloc[i]
        if r.key in done: continue
        bk = pd.DataFrame([{'trigger': 'F0', 'direction': r.direction, 'signal_def': r.signal_def}])
        sigs = score_g.build_book(df, pool, anchor, bk, adaptive=ad, structural=st)
        td = AE.run_portfolio(df, sigs, adaptive=ad, structural=st, warmup=w, verbose=False, conviction=conv)
        td = td[~td['signal_name'].isin(CP.GAP_NAMES)]
        daily = td.groupby(td['exit_time'].astype(str).str[:10])['pnl'].sum() if len(td) else pd.Series(dtype=float)
        rec = {'key': r.key, 'signal_def': r.signal_def, 'direction': r.direction, 'trades': int(len(td)), 'net': round(float(td.pnl.sum()), 2) if len(td) else 0.0,
               'loss_days': int((daily < 0).sum()), 'daily': {k: round(float(v), 2) for k, v in daily.items()}}
        with open(SOLO_PATH, 'a') as f: f.write(json.dumps(rec) + '\n')
        if (i - lo) % 50 == 0: print(f'[solo] {i}/{hi} {r.key} trades {rec["trades"]} net {rec["net"]} loss_days {rec["loss_days"]}', flush=True)
    print('[solo] done', flush=True)

def load_solo():
    recs = [json.loads(l) for l in open(SOLO_PATH)]
    days = sorted(set(d for r in recs for d in r['daily']))
    keys = [r['key'] for r in recs]; di = {d: i for i, d in enumerate(days)}
    P = np.zeros((len(recs), len(days)))
    for i, r in enumerate(recs):
        for d, v in r['daily'].items(): P[i, di[d]] = v
    return recs, keys, days, P

def decorrelate(recs, P, direction, k):
    """§2.1 exactly: net>0 pool per direction; seed = argmax(net - 50*loss_days) (deterministic);
    then greedily add the signal with the fewest loss days already covered (CUMULATIVE boolean), ties by higher net."""
    idx = [i for i, r in enumerate(recs) if r['direction'] == direction and r['net'] > 0]
    loss = (P < 0).astype(int); net = {i: recs[i]['net'] for i in idx}
    first = max(idx, key=lambda i: (net[i] - LOSS_PENALTY * loss[i].sum(), -i))
    chosen = [first]; covered = loss[first].copy()
    while len(chosen) < k:
        rem = [i for i in idx if i not in chosen]
        if not rem: break
        best = min(rem, key=lambda i: (int(((loss[i] == 1) & (covered > 0)).sum()), -net[i], i))
        chosen.append(best); covered = covered + loss[best]
    return chosen

def cmd_select(args):
    recs, keys, days, P = load_solo(); c = valid_rows().set_index('key')
    print(f'[select] solo matrix {len(recs)} signals x {len(days)} exit-days · TRUE-SOLO (one signal per run) · zero RNG')
    missing = [k for k in c.index if k not in keys]
    if missing: print(f'[select] WARN {len(missing)} VALID rows have no solo record — solo build incomplete; decorrelation runs on what exists')
    arm = []
    for dn in ('LONG', 'SHORT'):
        ch = decorrelate(recs, P, dn, K_DECORR[dn]); cov = (P[ch] < 0).any(0).sum()
        print(f'[decorr] {dn}: pool net>0 {sum(1 for r in recs if r["direction"]==dn and r["net"]>0)} · chosen {len(ch)} (K={K_DECORR[dn]}) · loss-days covered by >=1 member {cov} of {len(days)}')
        arm += [{'trigger': 'F0', 'direction': dn, 'signal_def': recs[i]['signal_def'], 'source': 'DECORR'} for i in ch]
    # chance-pricing
    E = c['EXPECTED_ROWS_AT_OR_ABOVE_THIS_PF'].astype(float)
    if args.null_by_direction and os.path.exists(args.null_by_direction):
        nb = json.load(open(args.null_by_direction))  # {"LONG": [pf...], "SHORT": [pf...], "n_trials": {"LONG": n, "SHORT": n}}
        from catalogue import pf_is_undefined
        Ed = {}
        for k_, r in c.iterrows():
            arr = np.asarray([float(x) for x in nb[r.direction] if not pf_is_undefined(x)]); ex = (arr >= float(r.agg_pf)).mean() if len(arr) else np.nan
            Ed[k_] = nb['n_trials'][r.direction] * ex
        E = pd.Series(Ed); print('[price] direction-corrected E from the supplied per-direction null')
    else:
        print('[price] WARN direction-blind E (family null, long_share 0.7951) — the per-direction null PF vector is not persisted by catalogue.py; correction NOT applied')
    priced = c[E < E_CEILING]
    print(f'[price] E<{E_CEILING}: {len(priced)} rows (LONG {int((priced.direction=="LONG").sum())} / SHORT {int((priced.direction=="SHORT").sum())}); resolution floor {c.pricing_resolution_floor.iloc[0]}')
    arm += [{'trigger': 'F0', 'direction': r.direction, 'signal_def': r.signal_def, 'source': 'PRICED'} for _, r in priced.iterrows()]
    A = pd.DataFrame(arm); A['key'] = A.signal_def + '|' + A.direction
    both = A.groupby('key').source.agg(lambda s: '+'.join(sorted(set(s))))
    U = A.drop_duplicates('key').copy(); U['source'] = U.key.map(both)
    U[['trigger', 'direction', 'signal_def', 'source']].to_csv(os.path.join(OUT, 'union_arm_signals.csv'), index=False)
    U[['trigger', 'direction', 'signal_def']].to_csv(os.path.join(OUT, 'union_arm_book.csv'), index=False)
    print(f'[union] {len(U)} unique signals (LONG {int((U.direction=="LONG").sum())} / SHORT {int((U.direction=="SHORT").sum())}) · in both objectives {int((U.source=="DECORR+PRICED").sum())} · written {OUT}/union_arm_book.csv')

if __name__ == '__main__':
    ap = argparse.ArgumentParser(); ap.add_argument('cmd', choices=['solo', 'select']); ap.add_argument('--start', type=int, default=0); ap.add_argument('--end', type=int, default=None); ap.add_argument('--null-by-direction', default=None)
    a = ap.parse_args(); os.chdir(HERE); (cmd_solo if a.cmd == 'solo' else cmd_select)(a)
