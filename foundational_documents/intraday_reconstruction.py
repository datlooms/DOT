"""intraday_reconstruction.py — the marked-to-adverse intraday equity floor.

WHY THIS EXISTS. Every intraday figure in the rule master spec's risk section was
a reconstruction produced by a script that was never committed. The engine holds
no equity or floating-P&L state (grep-confirmed: the only unrealised quantity is
the local at portfolio_simulation_engine.py L246, used to fire the BE nudge and
never accumulated), so the figure that decides FTMO deployability was the one
number in the document that could not be re-derived. This module is that script.

THE CONVENTION, STATED SO IT CAN BE DISAGREED WITH.

  POPULATION      the assembled book only. Gap fillers are reported separately
                  because engine L302 hardcodes glots=1.0 and L306 reads the
                  module constant D2D_GAP_LOTS, so neither honours a base-lot
                  setting and they do not scale with the book.

  MARKING         each position open during a bar is marked at that bar's
                  ADVERSE EXTREME (Low for longs, High for shorts), FLOORED AT
                  ITS OWN STOP IN FORCE DURING THAT BAR. A position cannot be
                  marked below a level at which it would already have closed,
                  and the engine's own exit decision for that bar establishes
                  that price did not reach the stop. NO INTRABAR ORDERING
                  ASSUMPTION IS REQUIRED. Positions past the BE nudge are
                  floored at the lock, because that is what current_sl holds.

  AGGREGATION     SIMULTANEOUS, PER BAR. The day's floor is the minimum over
                  bars of (realised P&L closed so far that day + the summed mark
                  of everything still open at that bar). This is the tightest
                  defensible reading.

                  IT IS NOT the sum of each position's worst moment over the
                  day. That variant is also computed and reported as
                  WORST_MOMENT because it is the bound a spec claiming "all
                  positions assumed to reach their worst moment simultaneously"
                  actually describes, and on this book it is ~3x deeper. The two
                  bracket the truth; neither is a substitute for tick data.

  DAY KEY         calendar date of the BAR. Positions carried across a day
                  boundary are marked from their own entry price, which is
                  conservative: FTMO measures the daily loss from the day's
                  opening equity, so a carried position's prior unrealised loss
                  is charged twice. REBASED is reported alongside for that
                  reason.

  RESIDUAL        all open positions are assumed to reach their bar-adverse
                  extreme together. TRUE DRAWDOWNS ARE SHALLOWER BY AN
                  UNMEASURED AMOUNT.

REQUIRED INPUT. A trace from the instrumented engine: one record per (bar, open
position) carrying the stop in force at the top of that bar. The stock engine
does not emit it; instrument() below applies the five edits and asserts the
trades frame is byte-identical afterwards, so the instrumentation cannot change
a result silently.

USAGE
    from intraday_reconstruction import reconstruct
    tab = reconstruct(df, trace, book_trades, lot_scale=0.1)
"""

import numpy as np
import pandas as pd

SPREAD = 3.0


def day_key(df):
    return pd.Series(df['Time'].values).str[:10].values


def mark_open(df, trace, spread=SPREAD):
    """Mark every (bar, open position) at the bar's adverse extreme, floored at its stop."""
    high = df['High'].values
    low = df['Low'].values
    b = trace['bar'].values.astype(np.int64)
    d = trace['dir'].values
    ep = trace['ep'].values
    sl = trace['sl'].values
    lots = trace['lots'].values
    adverse = np.where(d == 1, low[b], high[b])
    mark_px = np.where(d == 1, np.maximum(adverse, sl), np.minimum(adverse, sl))
    raw = np.where(d == 1, mark_px - ep - spread, ep - mark_px - spread)
    return raw * lots


def reconstruct(df, trace, book_trades, lot_scale=1.0):
    """Return a per-day table: SIMULTANEOUS floor, WORST_MOMENT bound, and close."""
    dk = day_key(df)
    tr = trace.copy()
    tr['mark'] = mark_open(df, tr) * lot_scale
    tr['day'] = dk[tr['bar'].values.astype(np.int64)]
    still_open = tr[tr['exit_bar'].values > tr['bar'].values]
    per_bar = still_open.groupby(['day', 'bar'])['mark'].sum()
    worst_moment = (still_open.groupby(['day', 'tid'])['mark'].min()
                    .groupby('day').sum())
    cl = book_trades.copy()
    cl['day'] = dk[cl['exit_bar'].values.astype(np.int64)]
    cl['P'] = cl['pnl'].values * lot_scale
    realised = cl.groupby(['day', 'exit_bar'])['P'].sum()
    closed_by_day = cl.groupby('day')['P'].sum()
    rows = []
    for day, grp in tr.groupby('day'):
        bars = np.unique(grp['bar'].values.astype(np.int64))
        rr = realised.loc[day] if day in realised.index.get_level_values(0) else pd.Series(dtype=float)
        aa = per_bar.loc[day] if day in per_bar.index.get_level_values(0) else pd.Series(dtype=float)
        cum = 0.0
        floor = 0.0
        floor_bar = None
        for bb in bars:
            if bb in rr.index:
                cum += float(rr.loc[bb])
            v = cum + (float(aa.loc[bb]) if bb in aa.index else 0.0)
            if v < floor:
                floor, floor_bar = v, int(bb)
        rows.append({'day': day,
                     'simultaneous_floor': round(floor, 2),
                     'floor_bar': floor_bar,
                     'worst_moment_bound': round(min(0.0, float(worst_moment.get(day, 0.0))), 2),
                     'day_closed': round(float(closed_by_day.get(day, 0.0)), 2)})
    return pd.DataFrame(rows).sort_values('day').reset_index(drop=True)


def summarise(tab, daily_limit, total_limit, label=''):
    q = tab['simultaneous_floor']
    out = {'label': label, 'days': int(len(tab)),
           'worst': float(q.min()), 'worst_pct_daily': 100 * abs(float(q.min())) / daily_limit,
           'median': float(q.median()), 'p75': float(q.quantile(0.25)),
           'p90': float(q.quantile(0.10)), 'p95': float(q.quantile(0.05)),
           'p98': float(q.quantile(0.02)),
           'worst_moment_bound': float(tab['worst_moment_bound'].min()),
           'days_closing_red': int((tab['day_closed'] < 0).sum())}
    for thr in (-0.02, -0.05, -0.10, -0.15, -0.20, -0.50, -1.00):
        out[f'days_below_{abs(thr):.2f}xdaily'] = int((q < thr * daily_limit).sum())
    return out
