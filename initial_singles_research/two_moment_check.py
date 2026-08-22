"""two_moment_check.py — SELECT coverage check (Part 1). A COUNT and a COVERAGE RATIO. NEVER an objective.

Definitions v1 (frozen 2026-08-22; bar-truth via dots_thresholds.py 518862bf19fb, imported through master.s1_thresholds/s2_pool):
  PARTICIPATION  entry_ok (ADX>=15, Volume>50, post-warmup, not Friday-close) AND ATR_1M>=20 AND D2D_Trend_Dir==d
  MOMENT 1, TURN (A)   >= 2 of T_d true       T_d = RULE_PREREG.md full-frame turn set (top-6 lift on the 297's bars)
  MOMENT 2, LEAD       turn count <= 1 AND >= 2 of L_d true
                       L_d = top-6 by hour-matched lift on 297\\A vs eligible-untraded (>=20 bars), excluding T_d
                           (initial_singles_research/hold/m1_three_population_lift.csv, WILL_THE_297_HOLD_REPORT §1)
  The DEFINITIONS were derived from the incumbent's bars (book-dependent at derivation, frozen here by version);
  the CHECK is computed from the frame alone. An arm's coverage = entry bars in each moment / the incumbent's.
CONSTRAINT: the description behind both moments churns across months (m3: TURN Jaccard 0.00-0.50 LONG, 0.00 in five of
six folds SHORT). Optimising this count would be a third objective on an unstable description. It is reported, never scored on.
"""
import os, sys, json, numpy as np, pandas as pd
HERE = os.path.dirname(os.path.abspath(__file__))
for d in (HERE, os.path.join(HERE, 'engine'), os.path.join(HERE, 'scanners'), os.path.join(HERE, 'orchestrator')):
    if d not in sys.path: sys.path.insert(0, d)
import master as MS, adm_engine as AE, swept_thresholds as SW, cluster_profiler as CP, conviction as C, score_g
VERSION = 'two_moment_v1'
T = {'LONG': ['AT_Score_ST:lo', 'Slope_EMA_ST:lo', 'D2D_Signal:==1', 'D2D_Up_Count:hi', 'EMA_Oscillator:lo', 'AT_Slope_ST:lo'],
     'SHORT': ['AT_Score_LT:hi', 'D2D_Signal:==-1', 'VAL_Side:==0', 'D2D_Dn_Count:hi', 'EMA_Oscillator:hi', 'D2D_Dynamic_Sensitivity:lo']}
L = {'LONG': ['KAMA_Dist:lo', 'KAMA_Slope:lo', 'ST_Flip_Event:==-1', 'OBV_Macd:lo', 'AT_Score_LT:lo', 'PrevDay_Low_Side:==-1'],
     'SHORT': ['OBV_Velocity:hi', 'Sqz_Val:hi', 'KAMA_Dist:hi', 'Momentum_Value:hi', 'KAMA_Slope:hi', 'AT_Slope_ST:hi']}

def moments(df, pool, w, atr_min):
    n = len(df); warm = np.arange(n) < w
    eligible = (df['ADX_Value'].values >= AE.ELIG_ADX) & (df['Volume'].values > AE.ELIG_VOL)
    fri = (df['EST_DayOfWeek'].values == 5) & ((df['EST_Hour'].values > 16) | ((df['EST_Hour'].values == 16) & (df['EST_Minute'].values >= 45)))
    part = eligible & (df['Volume'].values != 0) & ~fri & ~warm & (df['ATR_1M'].values >= atr_min)
    d2d = df['D2D_Trend_Dir'].values; M = {}
    for dn, dv in (('LONG', 1), ('SHORT', -1)):
        for c in T[dn] + L[dn]:
            if c not in pool: raise SystemExit(f'ABORT: {c} not in the 249 pool')
        tc = np.vstack([pool[c] for c in T[dn]]).sum(0); lc = np.vstack([pool[c] for c in L[dn]]).sum(0); p = part & (d2d == dv)
        M[dn] = {'M1': p & (tc >= 2), 'M2': p & (tc <= 1) & (lc >= 2)}
    return M

def coverage(td, M, incumbent=None):
    rows = []
    for dn in ('LONG', 'SHORT'):
        bars = np.array(sorted(td[td.direction == dn].entry_bar.unique()), dtype=int)
        m1 = int(M[dn]['M1'][bars].sum()) if len(bars) else 0; m2 = int(M[dn]['M2'][bars].sum()) if len(bars) else 0
        r = {'direction': dn, 'entry_bars': len(bars), 'M1_turn': m1, 'M2_lead': m2, 'neither': len(bars) - m1 - m2, 'M1_universe': int(M[dn]['M1'].sum()), 'M2_universe': int(M[dn]['M2'].sum())}
        if incumbent is not None:
            r['M1_ratio_vs_297'] = round(m1 / incumbent[dn]['M1_turn'], 3) if incumbent[dn]['M1_turn'] else None
            r['M2_ratio_vs_297'] = round(m2 / incumbent[dn]['M2_lead'], 3) if incumbent[dn]['M2_lead'] else None
        rows.append(r)
    return rows

def configure(df, cfg, G, floor, tiergates=True):
    AE.ADMISSION_RULE = 'FLOORED'; AE.MAX_POSITIONS = int(cfg['max_positions']); AE.ADM_FLOOR = dict(floor)
    AE.ADM_GATES = {'ATR': df['ATR_1M'].values.astype(float), 'atr_min': float(cfg['global_gate']['value'])}
    _NAME = {('Micro_Hurst', 90): 'HU90', ('Micro_FailedBreak', 20): 'FB20', ('AT_Slope_ST', 90): 'ATS90'}; tg = {}
    if tiergates:
        for dname, dv in (('LONG', 1), ('SHORT', -1)):
            for tier, gates in cfg['tier_gates'][dname].items():
                if not gates: continue
                t = 5 if tier == '5+' else int(tier); tg[(dv, t)] = [G[_NAME[(g['variable'], int(g['pct']))]] for g in gates]
    AE.ADM_TIERGATES = tg

def score(td, tag):
    p = td['pnl'].values.astype(float); tl, ev, dy = MS.loss_events(td)
    day = pd.Series(td['exit_time'].astype(str).values).str[:10].values; byday = pd.Series(p).groupby(day).sum()
    iso = pd.to_datetime(pd.Series(td['exit_time'].astype(str).values).str[:10], format='%Y.%m.%d')
    wk = iso.dt.isocalendar().set_index(iso.index)[['year', 'week']].astype(str).agg('-W'.join, axis=1).values; byweek = pd.Series(p).groupby(wk).sum()
    bybar = td.groupby('entry_bar').pnl.sum(); wins = p[p > 0]; losses = -p[p < 0]
    be = (losses.mean() / (wins.mean() + losses.mean()) * 100) if len(losses) and len(wins) else 0.0; wr = 100 * (p > 0).mean()
    return {'tag': tag, 'loss_events': ev, 'worst_bar': round(float(bybar.min()), 2), 'worst_day': round(float(byday.min()), 2), 'losing_weeks': int((byweek < 0).sum()), 'weeks': int(len(byweek)), 'days': int(len(byday)),
            'entry_bars': int(len(bybar)), 'trades': int(len(td)), 'WR': round(wr, 2), 'PF': round(wins.sum() / losses.sum(), 2) if len(losses) else None, 'MARGIN': round(wr - be, 2), 'net': round(p.sum(), 2), 'breach_2500': int((byday < -2500).sum())}

if __name__ == '__main__':
    os.chdir(HERE); out = os.path.join(HERE, 'discovery', 'two_moment'); os.makedirs(out, exist_ok=True)
    df, attest, input_sha = MS.s0_ingest('data', out); ad, st = MS.s1_thresholds(df); pool, anchor, w = MS.s2_pool(df, ad, st)
    cfg, cfg_path = MS.book_config_for('engine/whole_dot_signals.csv'); cv = cfg['conviction']
    conv = C.build_conviction(df, bool(cv['hurst']), bool(cv['recentfb']), bool(cv['d2d']), d2d_conviction=bool(cv['d2d_conviction']), d2d_gap=bool(cv['d2d_gap'])); G = SW.build_whole_dot_gates(df)
    M = moments(df, pool, w, float(cfg['global_gate']['value']))
    print(f'{VERSION}: M1 universe LONG {int(M["LONG"]["M1"].sum())} SHORT {int(M["SHORT"]["M1"].sum())} · M2 universe LONG {int(M["LONG"]["M2"].sum())} SHORT {int(M["SHORT"]["M2"].sum())} · M1∩M2 = 0 by construction')
    book = pd.read_csv('engine/whole_dot_signals.csv'); b50 = pd.read_csv('engine/book50_signals.csv')
    field = pd.read_csv('/mnt/project/results_F0_triple_convergence_and_d2ddir.csv'); fk = set(zip(field.signal_def, field.direction))
    orphans = book[~book.apply(lambda r: (r.signal_def, r.direction) in fk, axis=1)]; print(f'the fourteen (297 rows absent from the 19,754 field): {len(orphans)} — ' + '; '.join(f'[{r.direction}] {r.signal_def}' for r in orphans.itertuples()))
    arms = [('WHOLE DOT L3/S3 (control, IN-SAMPLE)', book, {1: 3, -1: 3}, True), ('WHOLE DOT L7/S4 (IN-SAMPLE)', book, {1: 7, -1: 4}, True),
            ('BOOK-50 canary L3/S3', b50, {1: 3, -1: 3}, True), ('297 minus the fourteen L3/S3 (IN-SAMPLE)', book[book.apply(lambda r: (r.signal_def, r.direction) in fk, axis=1)], {1: 3, -1: 3}, True)]
    up = os.path.join(HERE, 'discovery', 'select2', 'union_arm_book.csv')
    if '--only-union' in sys.argv: arms = arms[:1]
    if os.path.exists(up): arms.append(('UNION ARM decorr70/50 + priced60, L3/S3 + 297 gates (IN-SAMPLE)', pd.read_csv(up), {1: 3, -1: 3}, True))
    results = []; inc = None; trials = 0
    for tag, bk, floor, tgs in arms:
        sigs = score_g.build_book(df, pool, anchor, bk, adaptive=ad, structural=st); configure(df, cfg, G, floor, tgs); trials += 1
        td = AE.run_portfolio(df, sigs, adaptive=ad, structural=st, warmup=w, verbose=False, conviction=conv); td = td[~td['signal_name'].isin(CP.GAP_NAMES)].copy()
        s = score(td, tag); cov = coverage(td, M, inc)
        if inc is None: inc = {r['direction']: r for r in cov}
        s['coverage'] = cov; s['n_signals'] = len(bk); results.append(s); td.to_csv(os.path.join(out, f'trades_{len(results)}.csv'), index=False)
        print('RESULT ' + json.dumps(s), flush=True)
    json.dump(results, open(os.path.join(out, 'two_moment_results.json'), 'w'), indent=1)
    if '--only-union' in sys.argv: print(f'ENGINE TRIALS THIS SCRIPT: {trials}'); sys.exit(0)
    # the fourteen: depth mechanism inside the control
    tdc = pd.read_csv(os.path.join(out, 'trades_1.csv')); ok = set(orphans.signal_def + '|' + orphans.direction)
    sigs_all = score_g.build_book(df, pool, anchor, book, adaptive=ad, structural=st)
    n = len(df); warm = np.arange(n) < w
    eligible = (df['ADX_Value'].values >= AE.ELIG_ADX) & (df['Volume'].values > AE.ELIG_VOL); fri = (df['EST_DayOfWeek'].values == 5) & ((df['EST_Hour'].values > 16) | ((df['EST_Hour'].values == 16) & (df['EST_Minute'].values >= 45)))
    entry_ok = eligible & (df['Volume'].values != 0) & ~fri & ~warm
    masks, dirs, names = AE.build_signal_masks(df, sigs_all, ad, st, entry_ok, verbose=False); Mk = np.vstack(masks); dirs = np.array(dirs)
    is_orphan = np.array([(book.signal_def.iloc[i] + '|' + book.direction.iloc[i]) in ok for i in range(len(book))])
    rows = []
    for dn, dv in (('LONG', 1), ('SHORT', -1)):
        bars = np.array(sorted(tdc[tdc.direction == dn].entry_bar.unique()), dtype=int)
        d_all = Mk[dirs == dv][:, bars].sum(0); d_wo = Mk[(dirs == dv) & ~is_orphan][:, bars].sum(0)
        o_tr = tdc[(tdc.direction == dn) & tdc.signal_name.isin([book.signal_def.iloc[i] for i in range(len(book)) if is_orphan[i]])] if 'signal_name' in tdc else None
        rows.append({'direction': dn, 'orphans': int((is_orphan & (dirs == dv)).sum()), 'control_entry_bars': len(bars), 'bars_with_an_orphan_firing': int((Mk[(dirs == dv) & is_orphan][:, bars].sum(0) > 0).sum()),
                     'bars_that_drop_below_floor3_without_them': int(((d_all >= 3) & (d_wo < 3)).sum()), 'bars_depth_exactly_3_with_orphan': int(((d_all == 3) & (d_wo < 3)).sum())})
    print('FOURTEEN depth mechanism: ' + json.dumps(rows))
    print(f'ENGINE TRIALS THIS SCRIPT: {trials}')
