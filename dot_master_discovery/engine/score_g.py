import os
import re
import sys

_HERE = os.path.dirname(os.path.abspath(__file__))
_ROOT = os.path.abspath(os.path.join(_HERE, '..'))
for _d in (_HERE, os.path.join(_ROOT, 'scanners')):
    if _d not in sys.path:
        sys.path.insert(0, _d)

import numpy as np
import pandas as pd
import dots_thresholds as dt
import portfolio_simulation_engine as engine
import sequential_temporal as seq
import wf
import conviction as C

_F1 = re.compile(r'^(.*?)\s*->(\d+)->\s*(.*)$')


_F1 = re.compile(r'^(.*?)\s*->(\d+)->\s*(.*)$')
_F2 = re.compile(r'^(.+?):(-?\d+)->(-?\d+)$')
_F3 = re.compile(r'^(.+?)\s+GATED-BY\s+(.+?)==(-?\d+)$')
_F7 = re.compile(r'^FADE\s+(.+)$')
_F9 = re.compile(r'^(.+?)\s+IN-SESSION\s+(\d{1,2}):(\d{2})$')

UNSCOREABLE_FAMILIES = {
    'F4': 'divergence_nonconfirm — "A NOT-CONFIRMED-BY B" needs the scanner divergence window and '
          'its non-confirmation state machine, which run_search holds internally and does not expose',
    'F6': 'threshold_crossing — "X up-cross(level=hi) ROC=none" needs the scanner crossing detector '
          'and its ROC variant selection',
    'F8': 'cross_variable_structure — "A > B" needs the scanner relative-structure normalisation',
    'F11': 'rolling_leadlag — "A<->B N=30 leadlag_pos" needs the scanner rolling correlation window',
}


def _pool_mask(pool, label, fam, sig):
    if label not in pool:
        raise SystemExit(
            f'ABORT [{fam}] signal_def "{sig}" references condition "{label}", which is not in the '
            f'{len(pool)}-condition pool. The book cannot be scored; the candidate is NOT silently '
            f'dropped and NOT reparsed as another family.')
    return np.asarray(pool[label], dtype=bool)


def family_mask(df, pool, fam, sig):
    if fam == 'F5':
        return _pool_mask(pool, sig.strip(), fam, sig)
    if fam == 'F7':
        m = _F7.match(sig)
        if m:
            return _pool_mask(pool, m.group(1).strip(), fam, sig)
    if fam == 'F3':
        m = _F3.match(sig)
        if m:
            base = _pool_mask(pool, m.group(1).strip(), fam, sig)
            col, val = m.group(2).strip(), int(m.group(3))
            if col not in df.columns:
                raise SystemExit(f'ABORT [{fam}] gate column "{col}" absent from the frame for "{sig}".')
            return base & (df[col].values == val)
    if fam == 'F2':
        m = _F2.match(sig)
        if m:
            col, a_v, b_v = m.group(1).strip(), int(m.group(2)), int(m.group(3))
            if col not in df.columns:
                raise SystemExit(f'ABORT [{fam}] state column "{col}" absent from the frame for "{sig}".')
            v = df[col].values
            out = np.zeros(len(v), dtype=bool)
            out[1:] = (v[:-1] == a_v) & (v[1:] == b_v)
            return out
    if fam == 'F9':
        m = _F9.match(sig)
        if m:
            base = _pool_mask(pool, m.group(1).strip(), fam, sig)
            hh, mm = int(m.group(2)), int(m.group(3))
            return base & (df['EST_Hour'].values == hh) & (df['EST_Minute'].values == mm)
    if fam in UNSCOREABLE_FAMILIES:
        raise SystemExit(
            f'ABORT [{fam}] cannot be scored by build_book: {UNSCOREABLE_FAMILIES[fam]}. '
            f'signal_def "{sig}". S5 should have filtered this family out before S8; if it reached '
            f'here the filter and the scorer disagree.')
    raise SystemExit(
        f'ABORT [{fam}] unrecognised signal_def grammar: "{sig}". build_book will not guess and will '
        f'not fall through to another family parser — that silent fall-through is what crashed S8.')


def build_book(df, pool, anchor, book):
    rows = []
    fk = 0
    for _, b in book.iterrows():
        fam = str(b['family']).strip() if 'family' in book.columns else str(b['trigger']).strip()
        sig = str(b['signal_def'])
        if fam == 'F0':
            ft = [p.strip().rsplit(':', 1) for p in sig.split('+')]
            rows.append({'feat_1': ft[0][0], 'thresh_1': ft[0][1], 'feat_2': ft[1][0],
                         'thresh_2': ft[1][1], 'feat_3': ft[2][0], 'thresh_3': ft[2][1],
                         'direction': b['direction']})
            continue
        col = f'__BOOK_{fk}'
        fk += 1
        if fam == 'F1':
            m = _F1.match(sig)
            if m is None:
                raise SystemExit(
                    f'ABORT [F1] signal_def "{sig}" does not match the sequential-pair grammar '
                    f'A ->k-> B. Refusing to guess.')
            a, k, bb = m.group(1).strip(), int(m.group(2)), m.group(3).strip()
            for lbl in (a, bb):
                if lbl not in pool:
                    raise SystemExit(f'ABORT [F1] "{lbl}" is not in the condition pool for "{sig}".')
            df[col] = seq.pair_mask(pool[a], pool[bb], k, anchor).astype(int)
        else:
            df[col] = family_mask(df, pool, fam, sig).astype(int)
        rows.append({'feat_1': col, 'thresh_1': '==1', 'feat_2': col, 'thresh_2': '==1',
                     'feat_3': col, 'thresh_3': '==1', 'direction': b['direction']})
    return pd.DataFrame(rows)


def population(td):
    lots = td['lots'].values
    names = td['signal_name'].values
    dirs = td['direction'].values
    gap = (names == 'GAP_HURST') | (names == 'GAP_FB') | (names == 'GAP_D2D')
    book2 = (lots == 2.0) & ~gap
    return {'x1': int(((lots == 1.0) & ~gap).sum()), 'x2': int(book2.sum()),
            'x2_short': int((book2 & (dirs == 'SHORT')).sum()), 'x1.25': int((lots == 1.25).sum()),
            'gapH': int((names == 'GAP_HURST').sum()), 'gapF': int((names == 'GAP_FB').sum()),
            'gapD2D': int((names == 'GAP_D2D').sum())}


def score(df, sigs, ad, st, w, conv, tag):
    td = engine.run_portfolio(df, sigs, adaptive=ad, structural=st, warmup=w,
                              verbose=False, conviction=conv)
    p = td['pnl'].values
    wd = wf.daily_pnl_points(td)['pnl'].min()
    pf = round(p[p > 0].sum() / -p[p < 0].sum(), 2) if (p < 0).any() else 999.0
    wr = round((p > 0).sum() / len(td) * 100, 1)
    pop = population(td)
    mdd = _daily_mdd(td)
    print(f"{tag:22} tr={len(td):5} net=${p.sum():8.0f} PF={pf:5} WR={wr:5} wd={wd:7.1f} mDD={mdd:7.1f} | "
          f"x2={pop['x2']}(sh{pop['x2_short']}) x1.25={pop['x1.25']} gapH={pop['gapH']} gapF={pop['gapF']} gapD2D={pop['gapD2D']}")
    return td, p.sum()


def _daily_mdd(td):
    d = wf.daily_pnl_points(td).sort_values('exit_date')
    eq = d['pnl'].cumsum().values
    peak = np.maximum.accumulate(eq)
    return float((eq - peak).min()) if len(eq) else 0.0


def _baseline_dir():
    probe = 'equiDOT_recon171_step7_part1.csv'
    for cand in (_ROOT, os.path.join(_ROOT, 'data'), _HERE):
        if os.path.exists(os.path.join(cand, probe)):
            return cand
    return _ROOT


def main():
    os.chdir(_baseline_dir())
    df = engine.load_sealed_baseline(verbose=False)
    w = engine.warmup_floor(df, verbose=False)
    ad = dt.compute_adaptive_thresholds(df)
    st = dt.compute_structural_gates(df)
    anchor = seq.anchor_array(df, 'ST_Flip')
    pool = seq.build_condition_pool(df, ad, st, w)
    book = pd.read_csv(os.path.join(_HERE, 'book50_signals.csv'))
    sigs = build_book(df, pool, anchor, book)
    print('=== D2D CROWN-JEWEL OPTION MAP (BOOK-50 + jar + runner + momentum-SL + S.20 + D2D roles) ===')
    print('  built-system canonical: WR 92.3 / PF 6.40 / net $92,347 / daily wd -104.4 / daily mDD -145.9 / OOS PF 6.96')
    print('  toggles: DOT-alone $89,432/-153.7 | +Role2 +$1,011 | +Role1 14 gaps ~+$1,900 wd -104.4')
    _, base = score(df, sigs, ad, st, w,
                    C.build_conviction(df, True, True, True, d2d_conviction=False, d2d_gap=False), 'DOT-alone (S.20+warmup)')
    _, r2 = score(df, sigs, ad, st, w,
                  C.build_conviction(df, True, True, True, d2d_conviction=True, d2d_gap=False), '+Role2 D2D-conviction')
    _, r1 = score(df, sigs, ad, st, w,
                  C.build_conviction(df, True, True, True, d2d_conviction=False, d2d_gap=True), '+Role1 D2D-gap')
    _, crown = score(df, sigs, ad, st, w,
                     C.build_conviction(df, True, True, True, d2d_conviction=True, d2d_gap=True), 'CROWN JEWEL (all)')
    print(f"\n  Role2 conviction delta: +${r2-base:.0f} (target +$1,011)")
    print(f"  Role1 gap delta:        +${r1-base:.0f} (built-system canonical ~+$1,900, 14 gaps)")
    print(f"  Crown jewel net:        ${crown:.0f} (built-system canonical $92,347)")


if __name__ == '__main__':
    main()
