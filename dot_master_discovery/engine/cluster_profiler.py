"""S8B — per-candidate cluster-participation profiler.

Measures, for every single condition in the discovery vocabulary, how that
condition participates in same-direction convergence clusters. It measures; it
does not select, rank into a book, or tune anything.

HARD BOUNDARY — BASIS 3 IS FORWARD-LOOKING BY CONSTRUCTION.
The basis-3 thrust label uses Close[t+W], i.e. information not available at bar
t. That is legitimate for a selection-side diagnostic, because the question
"does this condition fire at the start of moves that turn out big" is inherently
a forward question. It means BASIS 3 CAN NEVER BECOME A LIVE GATE OR AN ENTRY
CONDITION. Anyone trading "thrust" would be trading future information. The
oracle ring for the thrust magnitude/efficiency columns therefore also contains
forward-looking values; this is inherent to the basis and is a further reason it
is selection-side only. The ATR_1M normaliser is taken at bar t and is causal.

COUPLING MITIGATION IMPLEMENTED — mitigation 2 of quant_response_6 §3.
Running depth is both a selection metric here (metric e) and the intended
runtime sizing input; selecting on it and then sizing by it would fit the book
to its own sizing mechanism. Metric (e) is therefore computed and emitted for
BASIS 3 as well as for bases 1 and 2. Price-anchored episodes are defined
without reference to the book or the jar, so "fires early in a thrust" is
measured against market structure rather than against the sizing mechanism's own
object.

SCOPE LIMIT.
The vocabulary profiled here is SINGLE CONDITIONS; the book's signals are
TRIPLES. A single condition's profile cannot be read as a signal's value, and no
book should be selected directly from this output. It is an input to selection,
not a selection rule.

ELIGIBLE UNIVERSE.
(ADX_Value >= 15) & (Volume > 50) & post-warmup. The Volume == 0 and
Friday-close exclusions carried by run_portfolio's entry_ok are OUT of the
measurement universe: Volume == 0 is already subsumed by Volume > 50, and this
matches build_condition_pool's own scannable definition, which is what defines
the vocabulary being profiled. The identical universe is applied to condition
fires and to every base-rate denominator. Cluster objects keep their native
definitions (book entries require the engine's entry_ok; thrust episodes are
price-anchored), which is a property of the object, not of the measurement.

GATES ARE STATE, NEVER ROW FILTERS. No bar is deleted anywhere in this stage.
Eligibility, validity and warm-up are recorded as boolean state and applied as
masks at measurement time.

All thresholds, including the basis-3 K and E, come from dots_thresholds via its
own compute_adaptive_thresholds (mechanism D, rolling-2500, day-refreshed,
floor-index). No percentile is computed locally. The oracle is left
byte-identical: _D_SPEC is extended at runtime and restored.
"""

import numpy as np
import pandas as pd
import dots_thresholds as dt
import portfolio_simulation_engine as engine

GAP_NAMES = ('GAP_HURST', 'GAP_FB', 'GAP_D2D')
N_VALUES = (5, 10)
SIZE_BANDS = (3, 5, 8)
MIN_FIRE_FLOOR = 200
THRUST_W = (15, 30)
THRUST_K_PCTS = (0.85, 0.90)
THRUST_E_PCTS = (0.75,)
TIMING_MIN_SIZE = 3
VOL_PROXY_COLLAPSE = 0.70
ATR_BUCKETS = 5
ELIGIBILITY_PREDICATE = '(ADX_Value >= 15) & (Volume > 50) & post-warmup; Volume==0 and Friday-close exclusions OUT'


def eligible_universe(df, warmup):
    n = len(df)
    return (df['ADX_Value'].values >= 15.0) & (df['Volume'].values > 50.0) & (np.arange(n) >= warmup)


def _chain(bars, n_tol):
    out = []
    if len(bars) == 0:
        return out
    start = 0
    for i in range(1, len(bars) + 1):
        if i == len(bars) or (bars[i] - bars[i - 1]) > n_tol:
            out.append((start, i))
            start = i
    return out


def build_cluster_set(n_bars, events_by_dir, n_tol):
    cid = {1: np.full(n_bars, -1, np.int32), -1: np.full(n_bars, -1, np.int32)}
    depth = {1: np.zeros(n_bars, np.int32), -1: np.zeros(n_bars, np.int32)}
    fsize = {1: np.zeros(n_bars, np.int32), -1: np.zeros(n_bars, np.int32)}
    pos = {1: np.full(n_bars, np.nan), -1: np.full(n_bars, np.nan)}
    rows = []
    k = 0
    for d in (1, -1):
        ev = np.sort(np.asarray(events_by_dir.get(d, []), dtype=np.int64))
        for (i0, i1) in _chain(ev, n_tol):
            b0 = int(ev[i0])
            b1 = int(ev[i1 - 1])
            size = int(i1 - i0)
            seg = np.arange(b0, b1 + 1)
            cid[d][seg] = k
            depth[d][seg] = np.searchsorted(ev[i0:i1], seg, side='right')
            fsize[d][seg] = size
            if b1 > b0:
                pos[d][seg] = (seg - b0) / float(b1 - b0)
            rows.append({'cluster_id': k, 'dir': d, 'size': size, 'b0': b0, 'b1': b1, 'span': b1 - b0})
            k += 1
    clusters = pd.DataFrame(rows) if rows else pd.DataFrame(columns=['cluster_id', 'dir', 'size', 'b0', 'b1', 'span'])
    return {'cid': cid, 'depth': depth, 'fsize': fsize, 'pos': pos, 'clusters': clusters}


def book_events(td):
    bk = td[~td['signal_name'].isin(GAP_NAMES)]
    out = {}
    for d, lab in ((1, 'LONG'), (-1, 'SHORT')):
        out[d] = np.sort(bk[bk['direction'] == lab]['entry_bar'].values.astype(np.int64))
    return out, bk


def qualifying_events(df, sigs, ad, st, warmup):
    n = len(df)
    warm = np.arange(n) < warmup
    eligible = (df['ADX_Value'].values >= 15) & (df['Volume'].values > 50)
    vol_zero = df['Volume'].values == 0
    fri_block = (df['EST_DayOfWeek'].values == 5) & ((df['EST_Hour'].values > 16) | ((df['EST_Hour'].values == 16) & (df['EST_Minute'].values >= 45)))
    entry_ok = eligible & ~vol_zero & ~fri_block & ~warm
    masks, dirs, _names = engine.build_signal_masks(df, sigs, ad, st, entry_ok, verbose=False)
    out = {}
    depth_per_bar = {}
    for d in (1, -1):
        cnt = np.zeros(n, np.int32)
        for m, sd in zip(masks, dirs):
            if sd == d:
                cnt += m.astype(np.int32)
        depth_per_bar[d] = cnt
        out[d] = np.repeat(np.flatnonzero(cnt), cnt[cnt > 0])
    return out, depth_per_bar


def _swept(df, specs):
    saved = dict(dt._D_SPEC)
    try:
        dt._D_SPEC.clear()
        dt._D_SPEC.update(specs)
        out = dt.compute_adaptive_thresholds(df)
    finally:
        dt._D_SPEC.clear()
        dt._D_SPEC.update(saved)
    return out


def thrust_state(df, W):
    c = df['Close'].values.astype(float)
    atr = df['ATR_1M'].values.astype(float)
    n = len(df)
    absd = np.abs(np.diff(c, prepend=c[0]))
    cs = np.cumsum(absd)
    fwd = np.zeros(n)
    path = np.zeros(n)
    valid = np.zeros(n, bool)
    fwd[:n - W] = c[W:] - c[:n - W]
    path[:n - W] = cs[W:] - cs[:n - W]
    valid[:n - W] = True
    mag = np.zeros(n)
    nz = atr > 0.0
    mag[nz] = np.abs(fwd[nz]) / atr[nz]
    eff = np.zeros(n)
    pz = path > 0.0
    eff[pz] = np.abs(fwd[pz]) / path[pz]
    return fwd, mag, eff, valid


def thrust_thresholds(df, W, k_pcts, e_pcts):
    fwd, mag, eff, valid = thrust_state(df, W)
    mcol = f'__THRUST_MAG_W{W}'
    ecol = f'__THRUST_EFF_W{W}'
    df[mcol] = mag
    df[ecol] = eff
    spec = {}
    for kp in k_pcts:
        spec[(mcol, f'k{int(round(kp * 100))}')] = (mcol, kp)
    for ep in e_pcts:
        spec[(ecol, f'e{int(round(ep * 100))}')] = (ecol, ep)
    try:
        thr = _swept(df, spec)
    finally:
        df.drop(columns=[mcol, ecol], inplace=True)
    return fwd, mag, eff, valid, thr, mcol, ecol


def thrust_events(fwd, mag, eff, valid, karr, earr, warmup):
    n = len(fwd)
    postwarm = np.arange(n) >= warmup
    qual = valid & postwarm & (fwd != 0.0) & (mag >= karr) & (eff >= earr)
    sgn = np.sign(fwd)
    return {1: np.flatnonzero(qual & (sgn > 0)).astype(np.int64),
            -1: np.flatnonzero(qual & (sgn < 0)).astype(np.int64)}


def map_trades_to_clusters(cs, bk):
    bars = bk['entry_bar'].values.astype(np.int64)
    dirs = np.where(bk['direction'].values == 'LONG', 1, -1)
    out = np.full(len(bk), -1, np.int32)
    for d in (1, -1):
        sel = dirs == d
        if sel.any():
            out[sel] = cs['cid'][d][bars[sel]]
    return out


def _pf(x):
    x = np.asarray(x, dtype=float)
    if len(x) == 0:
        return 0.0
    loss = -x[x < 0].sum()
    if loss <= 0:
        return 999.0 if x.sum() > 0 else 0.0
    return round(float(x[x > 0].sum() / loss), 3)


def _outcome(pnl, dates):
    if len(pnl) == 0:
        return 0.0, 0.0, 0.0, 0.0
    net = round(float(pnl.sum()), 1)
    pf = _pf(pnl)
    wr = round(float((pnl > 0).mean() * 100), 1)
    s = pd.Series(pnl).groupby(pd.Series(dates)).sum()
    wd = round(float(s.min()), 1)
    return net, pf, wr, wd


def profile_conditions(pool, cs, U, df, bk, trade_cid, basis, n_tol, grid, hours, atr_bucket):
    n = len(U)
    n_elig = int(U.sum())
    dirs = (1, -1)
    in_band = {}
    for k in SIZE_BANDS:
        per_dir = {d: (cs['cid'][d] >= 0) & (cs['fsize'][d] >= k) for d in dirs}
        per_dir['any'] = per_dir[1] | per_dir[-1]
        in_band[k] = per_dir
    base_rate = {k: float((in_band[k]['any'] & U).sum()) / n_elig if n_elig else 0.0 for k in SIZE_BANDS}
    timing_ok = {d: (cs['cid'][d] >= 0) & (cs['fsize'][d] >= TIMING_MIN_SIZE) & np.isfinite(cs['pos'][d]) for d in dirs}
    timing_zero = {d: (cs['cid'][d] >= 0) & (cs['fsize'][d] >= TIMING_MIN_SIZE) & ~np.isfinite(cs['pos'][d]) for d in dirs}
    shallow_m = {d: (cs['cid'][d] >= 0) & (cs['depth'][d] >= 1) & (cs['depth'][d] <= 2) & (cs['fsize'][d] >= 5) for d in dirs}
    pile_m = {d: (cs['cid'][d] >= 0) & (cs['depth'][d] >= 5) for d in dirs}
    incl = {d: cs['cid'][d] >= 0 for d in dirs}
    pnl = bk['pnl'].values.astype(float)
    dates = pd.Series(bk['exit_time'].values).str[:10].values
    band_cluster_ids = {}
    for k in SIZE_BANDS:
        cl = cs['clusters']
        band_cluster_ids[k] = set(cl[cl['size'] >= k]['cluster_id'].tolist()) if len(cl) else set()
    nb = ATR_BUCKETS
    rows = []
    for name, mask in pool.items():
        fm = mask & U
        fires = int(fm.sum())
        rec = {'condition': name, 'cluster_basis': basis, 'N': n_tol,
               'W': grid[0], 'K_pct': grid[1], 'E_pct': grid[2],
               'eligible_bars': n_elig, 'fires': fires,
               'fire_share_pct': round(100.0 * fires / n_elig, 4) if n_elig else 0.0,
               'min_fire_ok': bool(fires >= MIN_FIRE_FLOOR)}
        for k in SIZE_BANDS:
            pa = int((fm & in_band[k]['any']).sum())
            pl = int((fm & in_band[k][1]).sum())
            ps = int((fm & in_band[k][-1]).sum())
            rate = pa / fires if fires else 0.0
            rec[f'part_count_{k}'] = pa
            rec[f'part_rate_{k}'] = round(100.0 * rate, 4)
            rec[f'lift_{k}'] = round(rate / base_rate[k], 3) if base_rate[k] > 0 else 0.0
            rec[f'part_long_{k}'] = pl
            rec[f'part_short_{k}'] = ps
            rec[f'base_rate_{k}'] = round(100.0 * base_rate[k], 4)
        tvals = np.concatenate([cs['pos'][d][fm & timing_ok[d]] for d in dirs]) if fires else np.array([])
        rec['timing_n'] = int(len(tvals))
        rec['timing_excluded_zero_span'] = int(sum(int((fm & timing_zero[d]).sum()) for d in dirs))
        if len(tvals):
            rec['timing_median'] = round(float(np.median(tvals)), 4)
            rec['timing_q1'] = round(float(np.percentile(tvals, 25)), 4)
            rec['timing_q3'] = round(float(np.percentile(tvals, 75)), 4)
        else:
            rec['timing_median'] = ''
            rec['timing_q1'] = ''
            rec['timing_q3'] = ''
        shallow = int(sum(int((fm & shallow_m[d]).sum()) for d in dirs))
        pile = int(sum(int((fm & pile_m[d]).sum()) for d in dirs))
        rec['shallow_edge_count'] = shallow
        rec['pile_on_count'] = pile
        rec['shallow_pile_ratio'] = round(shallow / pile, 3) if pile else ''
        dvals = np.concatenate([cs['depth'][d][fm & incl[d]] for d in dirs]) if fires else np.array([])
        rec['depth_at_fire_median'] = round(float(np.median(dvals)), 2) if len(dvals) else ''
        rec['depth_at_fire_p90'] = round(float(np.percentile(dvals, 90)), 2) if len(dvals) else ''
        h5 = hours[fm & in_band[5]['any']]
        if len(h5):
            hc = np.bincount(h5.astype(int), minlength=24)
            rec['peak_hour_size5'] = int(np.argmax(hc))
            rec['share_1100_1300_size5'] = round(100.0 * hc[11:14].sum() / hc.sum(), 2)
            rec['hour_hist_size5'] = ';'.join(str(int(x)) for x in hc)
        else:
            rec['peak_hour_size5'] = ''
            rec['share_1100_1300_size5'] = ''
            rec['hour_hist_size5'] = ''
        for k in (3, 5):
            ids = np.concatenate([cs['cid'][d][fm & in_band[k][d]] for d in dirs]) if fires else np.array([], dtype=np.int32)
            part_ids = set(np.unique(ids).tolist())
            allowed = band_cluster_ids[k]
            in_part = np.isin(trade_cid, list(part_ids)) if part_ids else np.zeros(len(trade_cid), bool)
            in_band_tr = np.isin(trade_cid, list(allowed)) if allowed else np.zeros(len(trade_cid), bool)
            pnet, ppf, pwr, pwd = _outcome(pnl[in_part & in_band_tr], dates[in_part & in_band_tr])
            nnet, npf, nwr, nwd = _outcome(pnl[~in_part & in_band_tr], dates[~in_part & in_band_tr])
            rec[f'part_clusters_{k}'] = len(part_ids)
            rec[f'part_net_{k}'] = pnet
            rec[f'part_pf_{k}'] = ppf
            rec[f'part_wr_{k}'] = pwr
            rec[f'part_wd_{k}'] = pwd
            rec[f'non_net_{k}'] = nnet
            rec[f'non_pf_{k}'] = npf
            rec[f'non_wr_{k}'] = nwr
            rec[f'non_wd_{k}'] = nwd
        num = 0.0
        den = 0
        for b in range(nb):
            bm = atr_bucket == b
            ub = U & bm
            nub = int(ub.sum())
            if nub == 0:
                continue
            fb = int((fm & bm).sum())
            if fb == 0:
                continue
            br = float((in_band[5]['any'] & ub).sum()) / nub
            if br <= 0:
                continue
            num += fb * ((int((fm & bm & in_band[5]['any']).sum()) / fb) / br)
            den += fb
        lift_ctrl = round(num / den, 3) if den else 0.0
        rec['lift_5_atr_controlled'] = lift_ctrl
        rec['vol_proxy_flag'] = bool(rec['lift_5'] > 1.0 and lift_ctrl < VOL_PROXY_COLLAPSE * rec['lift_5'])
        rows.append(rec)
    return rows


def atr_buckets(df, U, nb=ATR_BUCKETS):
    atr = df['ATR_1M'].values.astype(float)
    out = np.full(len(df), -1, np.int8)
    vals = atr[U]
    if len(vals) == 0:
        return out
    edges = [np.percentile(vals, 100.0 * i / nb) for i in range(1, nb)]
    b = np.zeros(len(df), np.int8)
    for e in edges:
        b = b + (atr > e).astype(np.int8)
    out[U] = b[U]
    return out


def _cover_mask(cs, n_bars, min_size=5):
    cover = np.zeros(n_bars, bool)
    for d in (1, -1):
        cover |= (cs['cid'][d] >= 0) & (cs['fsize'][d] >= min_size)
    return cover


def overlap_validation(thrust_cs, cs_book, cs_qual, n_bars, U):
    cov_b1 = _cover_mask(cs_book, n_bars)
    cov_b2 = _cover_mask(cs_qual, n_bars)
    cov_any = cov_b1 | cov_b2
    cl = thrust_cs['clusters']
    ep_tot = len(cl)
    ep_hit = 0
    for _i, r in cl.iterrows():
        if cov_any[int(r['b0']):int(r['b1']) + 1].any():
            ep_hit += 1
    thrust_bars = np.zeros(n_bars, bool)
    for d in (1, -1):
        thrust_bars |= thrust_cs['cid'][d] >= 0
    tb = int((thrust_bars & U).sum())
    tb_hit = int((thrust_bars & cov_any & U).sum())
    cb = int((cov_any & U).sum())
    cb_hit = int((cov_any & thrust_bars & U).sum())
    return {'episodes': ep_tot, 'episodes_hit': ep_hit,
            'episode_pct': round(100.0 * ep_hit / ep_tot, 1) if ep_tot else 0.0,
            'thrust_bars': tb, 'thrust_bars_in_cluster_pct': round(100.0 * tb_hit / tb, 1) if tb else 0.0,
            'cluster_bars': cb, 'cluster_bars_in_thrust_pct': round(100.0 * cb_hit / cb, 1) if cb else 0.0,
            'b1_only_pct': round(100.0 * int((thrust_bars & cov_b1 & U).sum()) / tb, 1) if tb else 0.0,
            'b2_only_pct': round(100.0 * int((thrust_bars & cov_b2 & U).sum()) / tb, 1) if tb else 0.0}
