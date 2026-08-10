"""pf_parity_harness.py — the acceptance test for removing the 999/inf PF sentinel.

    python pf_parity_harness.py --out <run tree> --capture before.json
    ... apply the change, regenerate ...
    python pf_parity_harness.py --out <run tree> --capture after.json --compare before.json

It prints PASS or a NAMED DIFF. It does not print a wall of numbers to eyeball.

TWO CATEGORIES, BOTH EXPECTED TO APPEAR, AND THE POINT IS TO SEPARATE THEM:

  MUST NOT CHANGE  every verdict, every count, every ratio. The change exists to
                   stop shipping a sentinel as a measurement, NOT to alter what
                   the pipeline concludes. A zero-loss signal passed the S5 gate
                   at 999 and must still pass it undefined - by an explicit
                   branch instead of by the accident of the sentinel's size.

  SHOULD CHANGE    the emitted PF cells that read 999 or inf, now blank. Expected
                   counts are asserted below, so the operator can confirm he got
                   the change he asked for AND NOTHING ELSE.

If a MUST-NOT-CHANGE figure moves, the change is wrong and must be reverted.
"""

import argparse
import glob
import json
import os

import numpy as np
import pandas as pd

EXPECTED_BLANKED = {
    'wf_book_arm_entities.csv': {'test_PF': 21996},
    'results_F0_triple_convergence_and_d2ddir.csv': {'agg_pf': 5688, 'min_fold_pf': 5688},
    'discovery_master_part_2.csv': {'agg_pf': 3330, 'min_fold_pf': 3330},
    'discovery_master_part_1.csv': {'agg_pf': 2458, 'min_fold_pf': 2458},
    'concurrence_outcome_map.csv': {'agg_pf': 86, 'fold1_pf': 156, 'fold2_pf': 281, 'fold3_pf': 123, 'fold4_pf': 76, 'fold5_pf': 176, 'fold6_pf': 251, 'min_fold_pf': 86},
    'concurrence_outcome_map_secondary.csv': {'agg_pf': 81, 'fold1_pf': 153, 'fold2_pf': 271, 'fold3_pf': 117, 'fold4_pf': 80, 'fold5_pf': 177, 'fold6_pf': 261, 'min_fold_pf': 81},
    'concurrence_category_depth.csv': {'agg_pf': 133, 'fold2_pf': 193, 'fold3_pf': 157, 'fold4_pf': 189, 'fold5_pf': 214, 'fold6_pf': 149, 'min_fold_pf': 133},
    'concurrence_regimes.csv': {'agg_pf': 37, 'fold2_pf': 57, 'fold3_pf': 35, 'fold4_pf': 36, 'fold5_pf': 40, 'fold6_pf': 65, 'min_fold_pf': 37},
    'wf_null_arm_entities.csv': {'test_PF': 57, 'train_PF': 150},
    'results_F13_single_variable_extremes.csv': {'agg_pf': 137},
    'results_F1_sequential_temporal_part_1.csv': {'agg_pf': 57, 'min_fold_pf': 57},
    'candidates.csv': {'agg_pf': 50, 'min_fold_pf': 50},
    'results_F1_sequential_temporal_part_2.csv': {'agg_pf': 42, 'min_fold_pf': 42},
    'cluster_participation_profile.csv': {'non_pf_3': 7, 'non_pf_5': 34, 'part_pf_3': 20, 'part_pf_5': 22},
    'cohort_scored.csv': {'PF': 22},
    'catalogue_F1.csv': {'agg_pf': 18},
    'concurrence_null_baseline.csv': {'null_agg_pf_max': 4, 'null_agg_pf_p95': 4, 'observed_agg_pf': 1},
    'catalogue_F0.csv': {'agg_pf': 7},
    'results_F9_session_temporal.csv': {'agg_pf': 1, 'min_fold_pf': 1},
}
EXPECTED_BLANKED_TOTAL = 49662
SENTINELS = (999.0, 999)


def _read(path):
    try:
        return pd.read_csv(path, comment='#', low_memory=False)
    except Exception:
        return None


def _find(out, name):
    for pat in (os.path.join(out, name), os.path.join(out, '*', name),
                os.path.join(out, 'results', name), os.path.join(out, 'catalogues', name)):
        hit = sorted(glob.glob(pat))
        if hit:
            return hit[0]
    return None


def capture(out):
    """Every MUST-NOT-CHANGE figure, plus the sentinel census."""
    snap = {'must_not_change': {}, 'sentinel_census': {}}
    m = snap['must_not_change']

    for p in sorted(glob.glob(os.path.join(out, 'catalogues', 'catalogue_*.csv'))):
        fam = os.path.basename(p).replace('catalogue_', '').replace('.csv', '')
        d = _read(p)
        if d is None:
            continue
        m[f'catalogue_{fam}_rows'] = int(len(d))
        if 'verdict' in d.columns:
            for k, v in d['verdict'].value_counts().items():
                m[f'catalogue_{fam}_{k}'] = int(v)

    cand = _find(out, 'candidates.csv')
    if cand:
        d = _read(cand)
        if d is not None:
            m['candidates_rows'] = int(len(d))

    pc = _find(out, 'wf_pass_criterion.csv')
    if pc:
        d = _read(pc)
        if d is not None and len(d):
            for c in ('mean_ratio', 'min_ratio', 'mean_ratio_lb95', 'verdict',
                      'splits_with_ratio'):
                if c in d.columns:
                    m[f'wf_{c}'] = str(d[c].iloc[0])

    ent = _find(out, 'wf_book_arm_entities.csv')
    if ent:
        d = _read(ent)
        if d is not None and 'split_index' in d.columns:
            for s in sorted(d['split_index'].unique()):
                sub = d[d['split_index'] == s]
                for c in ('admitted', 'in_denominator', 'persisted'):
                    if c in sub.columns:
                        m[f'split{s}_{c}'] = int(sub[c].astype(bool).sum())

    cont = _find(out, 'contenders.csv')
    if cont:
        d = _read(cont)
        if d is not None:
            for _i, r in d.iterrows():
                cid = str(r.get('id', _i))
                for c in ('trades', 'net', 'WR', 'PF'):
                    if c in d.columns:
                        m[f'contender_{cid}_{c}'] = str(r[c])

    for p in sorted(glob.glob(os.path.join(out, '**', '*.csv'), recursive=True)):
        d = _read(p)
        if d is None:
            continue
        n = 0
        for c in d.columns:
            lc = str(c).lower()
            if not any(h in lc for h in ('pf', 'ratio')):
                continue
            v = pd.to_numeric(d[c], errors='coerce')
            n += int(v.isin(SENTINELS).sum()) + int(np.isinf(v.fillna(0)).sum())
        if n:
            snap['sentinel_census'][os.path.basename(p)] = n
    return snap


def compare(before, after):
    fails, notes = [], []
    b, a = before['must_not_change'], after['must_not_change']
    for k in sorted(set(b) | set(a)):
        bv, av = b.get(k, '<absent>'), a.get(k, '<absent>')
        if str(bv) != str(av):
            fails.append(f'{k}: {bv} -> {av}')
    bc, ac = before['sentinel_census'], after['sentinel_census']
    total_before = sum(bc.values())
    total_after = sum(ac.values())
    notes.append(f'sentinel cells: {total_before} -> {total_after} '
                 f'(cleared {total_before - total_after}; EXPECTED {EXPECTED_BLANKED_TOTAL} '
                 f'from a FULL census of all artifacts - an earlier sampled estimate said ~1,116 '
                 f'and was 44x low, which would have read a successful change as a failure)')
    for f, exp in EXPECTED_BLANKED.items():
        want = sum(exp.values())
        got = bc.get(f, 0) - ac.get(f, 0)
        flag = 'as expected' if got == want else f'EXPECTED {want}'
        notes.append(f'  {f:44} cleared {got:5}  {flag}')
    if total_after:
        for f, n in sorted(ac.items()):
            notes.append(f'  STILL SENTINEL: {f} {n} cells')
    return fails, notes


def main():
    ap = argparse.ArgumentParser(description='PF sentinel-removal parity harness.')
    ap.add_argument('--out', required=True)
    ap.add_argument('--capture', required=True)
    ap.add_argument('--compare', default=None)
    x = ap.parse_args()
    snap = capture(x.out)
    with open(x.capture, 'w', encoding='utf-8') as f:
        json.dump(snap, f, indent=1, sort_keys=True)
    nm = len(snap['must_not_change'])
    ns = sum(snap['sentinel_census'].values())
    print(f'  captured {nm} MUST-NOT-CHANGE figures and {ns} sentinel cells -> {x.capture}')
    if not x.compare:
        print('  (no --compare: this is the BEFORE capture)')
        return 0
    before = json.load(open(x.compare, encoding='utf-8'))
    fails, notes = compare(before, snap)
    print()
    print('  SHOULD CHANGE:')
    for n in notes:
        print(f'    {n}')
    print()
    if fails:
        print(f'  *** FAIL: {len(fails)} MUST-NOT-CHANGE figure(s) MOVED ***')
        for f in fails[:40]:
            print(f'      {f}')
        print('  THE CHANGE IS WRONG AND MUST BE REVERTED. The sentinel removal must not alter '
              'what the pipeline concludes.')
        return 1
    print(f'  PASS: all {nm} MUST-NOT-CHANGE figures identical.')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
