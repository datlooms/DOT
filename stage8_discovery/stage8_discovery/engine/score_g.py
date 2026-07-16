import os
import re
import sys

_HERE = os.path.dirname(os.path.abspath(__file__))
if _HERE not in sys.path:
    sys.path.insert(0, _HERE)

import numpy as np
import pandas as pd
import dots_thresholds as dt
import portfolio_simulation_engine as engine
import sequential_temporal as seq
import wf
import conviction as C

_F1 = re.compile(r'^(.*?)\s*->(\d+)->\s*(.*)$')


def build_book(df, pool, anchor, book):
    rows = []
    fk = 0
    for _, b in book.iterrows():
        if b['trigger'] == 'F0':
            ft = [p.strip().rsplit(':', 1) for p in b['signal_def'].split('+')]
            rows.append({'feat_1': ft[0][0], 'thresh_1': ft[0][1], 'feat_2': ft[1][0],
                         'thresh_2': ft[1][1], 'feat_3': ft[2][0], 'thresh_3': ft[2][1],
                         'direction': b['direction']})
        else:
            m = _F1.match(b['signal_def'])
            a, k, bb = m.group(1).strip(), int(m.group(2)), m.group(3).strip()
            col = f'__F1_{fk}'
            fk += 1
            df[col] = seq.pair_mask(pool[a], pool[bb], k, anchor).astype(int)
            rows.append({'feat_1': col, 'thresh_1': '==1', 'feat_2': col, 'thresh_2': '==1',
                         'feat_3': col, 'thresh_3': '==1', 'direction': b['direction']})
    return pd.DataFrame(rows)


def population(td):
    lots = td['lots'].values
    names = td['signal_name'].values
    gap = (names == 'GAP_HURST') | (names == 'GAP_FB')
    return {'x1': int(((lots == 1.0) & ~gap).sum()), 'x2': int((lots == 2.0).sum()),
            'x1.25': int((lots == 1.25).sum()), 'gapH': int((names == 'GAP_HURST').sum()),
            'gapF': int((names == 'GAP_FB').sum())}


def score(df, sigs, ad, st, w, conv, tag):
    td = engine.run_portfolio(df, sigs, adaptive=ad, structural=st, warmup=w,
                              verbose=False, conviction=conv)
    p = td['pnl'].values
    wd = wf.daily_pnl_points(td)['pnl'].min()
    pf = round(p[p > 0].sum() / -p[p < 0].sum(), 2) if (p < 0).any() else 999.0
    wr = round((p > 0).sum() / len(td) * 100, 1)
    pop = population(td)
    print(f"{tag:20} trades={len(td):5} net=${p.sum():8.0f} PF={pf:5} WR={wr:5} worst-day={wd:8.1f} | "
          f"x1={pop['x1']} x2={pop['x2']} x1.25={pop['x1.25']} gapH={pop['gapH']} gapF={pop['gapF']}")
    return td


def main():
    df = engine.load_sealed_baseline(verbose=False)
    w = engine.warmup_floor(df, verbose=False)
    ad = dt.compute_adaptive_thresholds(df)
    st = dt.compute_structural_gates(df)
    anchor = seq.anchor_array(df, 'ST_Flip')
    pool = seq.build_condition_pool(df, ad, st, w)
    book = pd.read_csv(os.path.join(_HERE, 'book50_signals.csv'))
    sigs = build_book(df, pool, anchor, book)
    print('=== S.20 OPTION MAP (BOOK-50 + jar + runner + momentum-SL + conviction) ===')
    print('  targets (honest pack-engine): A $58,277 | B $66,434 | G\' $84,554/-127.5 | G $89,487/-153.7')
    print('  G population target: x1=1339 x2=210 x1.25=809 gapH=48 gapF=285')
    score(df, sigs, ad, st, w, None, 'A flat (all off)')
    score(df, sigs, ad, st, w, C.build_conviction(df, True, False, False), 'B Hurst-only')
    score(df, sigs, ad, st, w, C.build_conviction(df, True, False, True), "G' recentFB-off")
    score(df, sigs, ad, st, w, C.build_conviction(df, True, True, True), 'G all-on')


if __name__ == '__main__':
    main()
