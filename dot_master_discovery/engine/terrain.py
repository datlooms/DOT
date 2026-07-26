"""engine/terrain.py — S2B, the market terrain map.

WHAT THIS IS. A price-only enumeration of every significant directional episode
in the loaded data. NO SIGNALS ARE INVOLVED AT ANY POINT. It is a property of
the MARKET, not of any book, and every table it emits is labelled MARKET.

WHY IT EXISTS. The committed book participates in roughly 11% of the market's
clean directional moves, and 89.8% of what it misses is places where not one of
its fifty signals fires at all. Selection maximises DEPTH, and depth is measured
against THE BOOK'S OWN CLUSTERS, so a rediscovered book can score brilliantly
while sitting in the same corner of the market. A fixed, book-independent
denominator is the only way to ask whether a NEW book reaches more of the market
than the old one.

THE EPISODE LABEL IS FORWARD-LOOKING BY CONSTRUCTION AND CAN NEVER BECOME A LIVE
GATE OR AN ENTRY CONDITION. It reads Close[t+W]. That is legitimate for a
selection-side diagnostic, because "did this signal fire at the start of moves
that turned out big" is inherently a backward-looking question asked after the
fact. Anyone trading "thrust detected" would be trading future information. This
boundary is repeated in the header of every artifact this module writes.

CONSTRUCTION IS THE RATIFIED BASIS-3 ONE, REUSED, NOT REINVENTED. K and E come
from dots_thresholds via cluster_profiler's thrust_thresholds, which sweeps them
through the oracle's own compute_adaptive_thresholds (mechanism D, rolling-2500,
day-refreshed, floor-index). No local percentile and no hardcoded constant
appears here; mechanisms A, B and C are retired and must not reappear.

THE GRID IS PART OF THE FINDING. Episode counts move by a factor of 2-4x with
the mask and the thresholds while the up/down ratio stays inside a fraction of a
point of 50/50. Every row and every summary line carries its own (W, K, E) cell
and the eligibility mask, because a count without its parameters is not a
measurement.
"""

import numpy as np
import pandas as pd
import cluster_profiler as cp

GRID_W = (15, 30)
GRID_K = (0.85, 0.90)
GRID_E = (0.75,)
CONTIGUOUS_TOLERANCE = 1
TOP_EPISODE_FRACTION = 0.10
FORWARD_LOOKING_BOUNDARY = (
    'FORWARD-LOOKING BY CONSTRUCTION: the episode label reads Close[t+W]. Legitimate as a '
    'SELECTION-SIDE diagnostic; IT CAN NEVER BECOME A LIVE GATE OR AN ENTRY CONDITION, because '
    'anyone trading "thrust detected" would be trading future information.')
MARKET_LABEL = (
    'PROPERTY OF THE MARKET — price only, no signals, no book. The denominator is fixed across '
    'every run and every candidate book.')
SESSION_BANDS = ((0, 8, 'overnight'), (8, 10, 'pre-open'), (10, 12, 'morning'),
                 (12, 14, 'midday'), (14, 16, 'afternoon'), (16, 24, 'after-hours'))


def session_of(hour):
    for lo, hi, name in SESSION_BANDS:
        if lo <= hour < hi:
            return name
    return 'unknown'


def eligibility_label():
    return 'ADX_Value >= 15 & Volume > 50 & post-warmup (cluster_profiler.eligible_universe)'


def build_terrain(df, warmup, grid_w=GRID_W, grid_k=GRID_K, grid_e=GRID_E):
    n = len(df)
    close = df['Close'].values.astype(float)
    atr = df['ATR_1M'].values.astype(float)
    hours = df['EST_Hour'].values
    times = df['Time'].astype(str).values
    universe = cp.eligible_universe(df, warmup)
    rows = []
    cells = {}
    eid = 0
    for W in grid_w:
        fwd, mag, eff, valid, thr, mcol, ecol = cp.thrust_thresholds(df, W, grid_k, grid_e)
        for kp in grid_k:
            for ep in grid_e:
                karr = thr[(mcol, f'k{int(round(kp * 100))}')]
                earr = thr[(ecol, f'e{int(round(ep * 100))}')]
                events = cp.thrust_events(fwd, mag, eff, valid, karr, earr, warmup)
                cs = cp.build_cluster_set(n, events, CONTIGUOUS_TOLERANCE)
                cl = cs['clusters']
                cell = (W, kp, ep)
                cells[cell] = cs
                if len(cl) == 0:
                    continue
                for _i, r in cl.iterrows():
                    d = int(r['dir'])
                    b0 = int(r['b0'])
                    b1 = int(r['b1'])
                    end_ref = min(b1 + W, n - 1)
                    disp = float(close[end_ref] - close[b0]) * d
                    a0 = float(atr[b0])
                    path = float(np.abs(np.diff(close[b0:end_ref + 1])).sum()) if end_ref > b0 else 0.0
                    hour = int(hours[b0])
                    rows.append({
                        'episode_id': eid, 'W': W, 'K_pct': kp, 'E_pct': ep,
                        'direction': 'UP' if d == 1 else 'DOWN',
                        'start_bar': b0, 'end_bar': b1,
                        'start_time': times[b0], 'end_time': times[b1],
                        'duration_bars': int(b1 - b0 + 1),
                        'abs_displacement_pts': round(abs(disp), 1),
                        'displacement_atr': round(abs(disp) / a0, 3) if a0 > 0 else 0.0,
                        'efficiency': round(abs(disp) / path, 4) if path > 0 else 0.0,
                        'est_hour_start': hour, 'session': session_of(hour),
                        'eligibility_mask': eligibility_label(),
                        'population': 'MARKET'})
                    eid += 1
    return pd.DataFrame(rows), cells, int(universe.sum())


def summarise(terrain):
    out = []
    if len(terrain) == 0:
        return pd.DataFrame(out)
    for (W, kp, ep), g in terrain.groupby(['W', 'K_pct', 'E_pct']):
        up = g[g['direction'] == 'UP']
        dn = g[g['direction'] == 'DOWN']
        tot = len(g)
        disp = g['abs_displacement_pts'].values
        row = {'W': W, 'K_pct': kp, 'E_pct': ep, 'episodes': tot,
               'up': len(up), 'down': len(dn),
               'up_share_pct': round(100.0 * len(up) / tot, 1) if tot else 0.0,
               'down_share_pct': round(100.0 * len(dn) / tot, 1) if tot else 0.0,
               'median_disp_pts': round(float(np.median(disp)), 1),
               'q1_disp_pts': round(float(np.percentile(disp, 25)), 1),
               'q3_disp_pts': round(float(np.percentile(disp, 75)), 1),
               'median_disp_up': round(float(np.median(up['abs_displacement_pts'])), 1) if len(up) else 0.0,
               'median_disp_down': round(float(np.median(dn['abs_displacement_pts'])), 1) if len(dn) else 0.0,
               'median_duration_bars': int(np.median(g['duration_bars'].values)),
               'population': 'MARKET', 'eligibility_mask': eligibility_label()}
        out.append(row)
    return pd.DataFrame(out)


def hour_profile(terrain, top_fraction=TOP_EPISODE_FRACTION):
    rows = []
    if len(terrain) == 0:
        return pd.DataFrame(rows)
    for (W, kp, ep), g in terrain.groupby(['W', 'K_pct', 'E_pct']):
        cut = float(np.percentile(g['abs_displacement_pts'].values, 100 * (1 - top_fraction)))
        big = g[g['abs_displacement_pts'] >= cut]
        for hour in range(24):
            h = g[g['est_hour_start'] == hour]
            hb = big[big['est_hour_start'] == hour]
            if len(h) == 0 and len(hb) == 0:
                continue
            rows.append({'W': W, 'K_pct': kp, 'E_pct': ep, 'est_hour': hour,
                         'session': session_of(hour), 'episodes': len(h),
                         'up': int((h['direction'] == 'UP').sum()),
                         'down': int((h['direction'] == 'DOWN').sum()),
                         'median_disp_pts': round(float(np.median(h['abs_displacement_pts'])), 1) if len(h) else 0.0,
                         'biggest_decile_episodes': len(hb),
                         'biggest_decile_share_pct': round(100.0 * len(hb) / len(big), 1) if len(big) else 0.0,
                         'largest_disp_pts': round(float(h['abs_displacement_pts'].max()), 1) if len(h) else 0.0,
                         'population': 'MARKET'})
    return pd.DataFrame(rows)


def render_hour_profile(prof, cell):
    W, kp, ep = cell
    g = prof[(prof['W'] == W) & (prof['K_pct'] == kp) & (prof['E_pct'] == ep)]
    lines = []
    if len(g) == 0:
        return lines
    peak = int(g['biggest_decile_episodes'].max()) or 1
    lines.append(f'  WHERE THE BIGGEST RUNS LIVE — W={W} K=p{int(kp * 100)} E=p{int(ep * 100)} '
                 f'| MARKET | bar = share of the largest-decile episodes')
    for _i, r in g.sort_values('est_hour').iterrows():
        bar = '#' * int(round(20.0 * r['biggest_decile_episodes'] / peak))
        lines.append(f"    {int(r['est_hour']):02d}:00 {r['session']:<12} "
                     f"{int(r['episodes']):5} eps  up {int(r['up']):4} dn {int(r['down']):4}  "
                     f"med {r['median_disp_pts']:7.1f}pt  max {r['largest_disp_pts']:8.1f}pt  "
                     f"top10% {int(r['biggest_decile_episodes']):4} {bar}")
    return lines


def terrain_clusters(cells, cell):
    return cells[cell]
