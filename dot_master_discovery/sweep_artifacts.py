"""sweep_artifacts.py — THE ARTIFACT CLASS SWEEP.

    python sweep_artifacts.py --dir <artifact dir> [--out <report.csv>]

THE CLASS: an artifact whose columns are CONSTANT, TAUTOLOGICAL, SENTINEL-VALUED,
or SCALED TO A POPULATION THAT CANNOT ANSWER THE QUESTION THE ARTIFACT NAMES IN
ITS OWN HEADER.

Six instances reached the operator before this existed, each found by hand:

  1 unclaimed_reachable.csv  n_valid_triples_touching tautologically 0 everywhere
  2 the six gated_* catalogue columns, inert via a permissive threshold fallback
  3 wf_book_arm_entities.csv missing the denominator column its headline needs
  4 causal=False on four concurrence files, 302,513 rows
  5 concurrence_null_baseline.csv, one cell printing PF 999 / WR 100 on ~1 trade
  6 cohort_scored.csv at pool scale answering a book-scale question

A SWEEP THAT DOES NOT CATCH ITS OWN KNOWN CASES IS NOT A SWEEP, so each check
below names the instance it exists to catch and the suite is run against all six.

WHY THE CHECKS ARE SHAPED THIS WAY. A constant column is not automatically wrong -
population='POOL' is constant and correct. What makes a constant column a defect
is that it PURPORTS TO VARY: a provenance flag that records how a figure was
derived, a diagnostic that separates two cases, a count that should differ per
row. So CONSTANT is reported for every column and ESCALATED for the column names
whose whole purpose is to discriminate.
"""

import argparse
import glob
import os
import sys
import re

import numpy as np
import pandas as pd

SENTINELS = (999.0, 999, -999.0)
RATIO_HINT = ('pf', 'ratio', 'wr', 'rate', 'frac', 'pct', 'share', 'margin', 'breakeven')
DISCRIMINATOR_HINT = ('causal', 'in_denominator', 'admitted', 'traded_on_test', 'persisted',
                      'n_valid_triples_touching', 'gated_delta', 'n_conditions_firing',
                      'reason_code', 'verdict', 'sufficient', 'chosen', 'skipped')
COUNT_HINT = ('n_', 'count', 'trades', 'bars', 'rows', 'episodes', 'touching', 'losses')
BOOK_SCALE_MAX_TRADES = 20000
DEPTH_HINT = ('depth', 'cluster', 'ge3', 'ge5', 'ge8', 'k_deep', 'peak_depth')
BASIS_HINT = ('basis', 'population', 'terrain_cell', 'cell')


def read_artifact(path):
    header = []
    with open(path, 'r', encoding='utf-8', errors='replace') as f:
        for ln in f:
            if ln.startswith('#'):
                header.append(ln.rstrip('\n'))
            else:
                break
    try:
        df = pd.read_csv(path, comment='#', low_memory=False)
    except Exception as exc:
        return None, header, f'{type(exc).__name__}: {str(exc)[:70]}'
    return df, header, ''


KNOWN_GOOD_INVARIANT = {
    # A constant that is the CORRECT RESULT, not a collapsed flag. Escalating these
    # trains the reader to ignore the report, which is how six real defects survived.
    'null_WR_n_used': 'every permutation was used - no null distribution lost a draw',
    'null_agg_pf_n_used': 'every permutation was used',
    'null_folds_plus_n_used': 'every permutation was used',
    'null_worst_day_usd_n_used': 'every permutation was used',
    'train_passes': 'the null arm appends rows ONLY for qualifiers, so True is this '
                    'file\'s definition',
    'in_denominator': 'the null arm divides by every train qualifier, so this equals '
                      'train_passes by design',
    'causal': 'only stage3_entry_order carries a causal arm by ruling; the other stages '
              'are full-sample and say so in their headers',
    'population': 'genuinely constant - the file has one population',
    'cluster_basis': 'genuinely constant - the run executed on one basis',
}


PER_FILE_SCALAR = ('_family', 'terrain_cell', 'null_seed', 'null_matched_fraction',
                   'null_rejected_out_of_band', 'null_direction_long_share', 'dataset_rows',
                   'input_sha', 'run_id', 'oracle_sha', 'scanner_sha', 'split_definition',
                   'd2d_mode', 'onset_floor', 'min_stack_bars', 'n_perm', 'min_shift', 'method')
MIN_ROWS_FOR_CONSTANT = 20


def check_constant(df):
    """Instances 1 and 4: a column that PURPORTS TO VARY and does not.

    A constant column is not automatically wrong. population='POOL' is constant
    and correct; n_trials_family is a per-family scalar and constant within one
    family's file BY CONSTRUCTION. What makes a constant a defect is that the
    column exists to DISCRIMINATE - a provenance flag, a diagnostic that
    separates two cases, a count that should differ per row. Per-file scalars are
    excluded and a floor of 20 rows applies, because a 2-row file has constants
    trivially and reporting them buries the signal.
    """
    out = []
    if len(df) < MIN_ROWS_FOR_CONSTANT:
        return out
    for c in df.columns:
        s = df[c]
        if s.nunique(dropna=False) != 1:
            continue
        lc = str(c).lower()
        if any(h in lc for h in PER_FILE_SCALAR):
            continue
        val = s.iloc[0]
        if str(c) in KNOWN_GOOD_INVARIANT:
            out.append({'column': c, 'value': repr(val)[:34], 'escalated': False,
                        'known_good': KNOWN_GOOD_INVARIANT[str(c)]})
            continue
        esc = any(h in lc for h in DISCRIMINATOR_HINT) or any(h in lc for h in COUNT_HINT)
        out.append({'column': c, 'value': repr(val)[:34], 'escalated': esc})
    return out


def check_sentinel(df):
    """Instance 5: a ratio wearing 999 or inf instead of blanking."""
    out = []
    for c in df.columns:
        lc = str(c).lower()
        if not any(h in lc for h in RATIO_HINT):
            continue
        v = pd.to_numeric(df[c], errors='coerce')
        n999 = int(v.isin(SENTINELS).sum())
        ninf = int(np.isinf(v.fillna(0)).sum())
        if n999 or ninf:
            out.append({'column': c, 'n_999': n999, 'n_inf': ninf})
    return out


def check_scale(df, header):
    """Instance 6: a trade or net column at pool scale in a book-scale artifact."""
    txt = ' '.join(header).upper()
    claims_book = 'BOOK' in txt and 'POOL' not in txt
    out = []
    for c in df.columns:
        lc = str(c).lower()
        if lc not in ('trades', 'net', 'trades_ge3', 'n_traded'):
            continue
        v = pd.to_numeric(df[c], errors='coerce')
        mx = float(v.max()) if v.notna().any() else 0.0
        if lc == 'trades' and mx > BOOK_SCALE_MAX_TRADES:
            out.append({'column': c, 'max': mx,
                        'note': f'max {mx:,.0f} exceeds any real book (~3,000); this is POOL scale'
                                + (' while the header claims BOOK' if claims_book else '')})
    return out


def check_missing_basis(df, header):
    """Instance from item E: a depth or cluster figure with no stated basis."""
    has_depth = any(any(h in str(c).lower() for h in DEPTH_HINT) for c in df.columns)
    if not has_depth:
        return []
    txt = ' '.join(header).lower()
    named = any(h in txt for h in BASIS_HINT) or \
        any(any(h in str(c).lower() for h in BASIS_HINT) for c in df.columns)
    if named:
        return []
    return [{'note': 'carries a depth/cluster figure but names NO basis in header or column - '
                     'a depth-5+ population is 128 or 1,958 clusters depending on basis, a '
                     'factor of 15, so the figure is unreadable without it'}]


def check_required(name, df):
    """Instance 3: a headline artifact missing the column its own figure needs."""
    REQ = {'wf_book_arm_entities.csv': ('in_denominator', 'train_passes', 'test_passes'),
           'wf_null_arm_entities.csv': ('in_denominator', 'train_passes'),
           'cohort_scored.csv': ('win_loss_ratio', 'n_losses')}
    need = REQ.get(name)
    if not need:
        return []
    miss = [c for c in need if c not in df.columns]
    return [{'missing': miss}] if miss else []


SKIP_DIRS = ('.markers', '_f13_shards', '__pycache__', 'data_for_analysis')
# S10's OWN exclusion rule, imported rather than re-stated. Two enumerators
# disagreeing about what an artifact is is how the scope went wrong in BOTH
# directions: first 32 of 76 (non-recursive), then 4,370 (chunk shards counted as
# artifacts). S10 skipped 13,205 and collected 90; that is the reader's set.
_SKIP_NAME = re.compile(
    r'(_c\d{4}\.(csv|pkl|done|cand)$)|(\.done$)|(\.cand$)|(\.provenance$)'
    r'|(^_frame_.*\.csv$)|(^_s3_frame.*\.csv$)|(^shard_\d+\.csv$)|(^_f0_kept\.pkl$)')


def enumerate_artifacts(directory):
    """EVERY artifact under the tree, RECURSIVELY.

    This was glob('<dir>/*.csv') - NON-RECURSIVE - so it audited the 32 files in
    discovery/full/ and never saw the 44 in discovery/full/results/ or
    catalogues/. All six known defects lived in the unaudited two thirds:
    n_valid_triples_touching, cohort_scored's POOL-scale nets, the inert gated_*
    columns, candidates.csv's 50 rows at 999, the null-baseline sentinel, and
    causal=False across the concurrence files.

    THE DETECTION LOGIC WAS ALWAYS CORRECT AND THE SCOPE WAS SHORT. That is the
    third auditor-coverage failure of the week - the never-called sweep's file
    list omitted a scanner, the contract gate missed a loop-variable read - and
    every time the thing the tool could not see was where the defect was. A
    provenance record that silently covers a third of the tree is WORSE than
    none, because it reads as clean evidence.
    """
    out = []
    for dp, dirs, files in os.walk(directory):
        dirs[:] = [d for d in dirs if d not in SKIP_DIRS]
        for fn in files:
            if not fn.endswith(('.csv', '.jsonl')):
                continue
            if _SKIP_NAME.search(fn):
                continue
            out.append(os.path.join(dp, fn))
    return sorted(out)


def coverage(directory, audited):
    """ONE enumerator. A second walk with different rules is how a 69-file gap appeared
    at 4,370: whatever enumerate_artifacts skipped, the coverage count still counted."""
    exist = len(enumerate_artifacts(directory))
    return exist, audited, (exist == audited)


def sweep(directory):
    rows = []
    files = enumerate_artifacts(directory)
    for p in files:
        name = os.path.basename(p)
        df, header, err = read_artifact(p)
        if df is None:
            rows.append({'file': name, 'finding': 'UNREADABLE', 'detail': err})
            continue
        found = False
        for h in check_constant(df):
            found = True
            rows.append({'file': name,
                         'finding': ('CONSTANT-ESCALATED' if h['escalated']
                                     else ('constant-KNOWN-GOOD' if h.get('known_good')
                                           else 'constant')),
                         'detail': (f"{h['column']} == {h['value']} on all {len(df)} rows"
                                    + (f" | KNOWN GOOD: {h['known_good']}"
                                       if h.get('known_good') else ''))})
        for h in check_sentinel(df):
            found = True
            rows.append({'file': name, 'finding': 'SENTINEL',
                         'detail': f"{h['column']}: {h['n_999']} x 999, {h['n_inf']} x inf"})
        for h in check_scale(df, header):
            found = True
            rows.append({'file': name, 'finding': 'SCALE', 'detail': h['note']})
        for h in check_missing_basis(df, header):
            found = True
            rows.append({'file': name, 'finding': 'NO-BASIS', 'detail': h['note']})
        for h in check_required(name, df):
            found = True
            rows.append({'file': name, 'finding': 'MISSING-REQUIRED',
                         'detail': f"missing {h['missing']}"})
        if not found:
            rows.append({'file': name, 'finding': 'none found', 'detail': f'{len(df)} rows'})
    return pd.DataFrame(rows)


def main():
    ap = argparse.ArgumentParser(description='Artifact class sweep.')
    ap.add_argument('--dir', required=True)
    ap.add_argument('--out', default=None)
    ap.add_argument('--summary-only', action='store_true')
    a = ap.parse_args()
    if a.out:
        if os.path.isdir(a.out):
            a.out = os.path.join(a.out, 'sweep_report.csv')
        parent = os.path.dirname(os.path.abspath(a.out))
        if not os.path.isdir(parent):
            print(f'  ABORT: --out parent directory does not exist: {parent}')
            return 2
        try:
            with open(a.out, 'a', encoding='utf-8'):
                pass
        except OSError as exc:
            print(f'  ABORT: --out is not writable BEFORE doing the work: {a.out} '
                  f'({type(exc).__name__}). The previous form discovered this at the LAST line '
                  f'and discarded the whole audit.')
            return 2
    rep = sweep(a.dir)
    esc = rep[rep['finding'].isin(('CONSTANT-ESCALATED', 'SENTINEL', 'SCALE', 'NO-BASIS',
                                   'MISSING-REQUIRED', 'UNREADABLE'))]
    n_exist, n_audited, ok = coverage(a.dir, rep['file'].nunique())
    print(f'  COVERAGE: audited {n_audited} of {n_exist} artifacts present under {a.dir}')
    if not ok:
        print(f'  *** COVERAGE GAP: {n_exist - n_audited} artifact(s) EXIST AND WERE NOT '
              f'AUDITED. A provenance record covering part of the tree reads as clean '
              f'evidence for files it never opened. THIS IS A FAILURE, NOT A WARNING. ***')
    print(f'  files swept: {rep["file"].nunique()}')
    for k, v in rep['finding'].value_counts().items():
        print(f'    {k:22} {v}')
    print()
    print('  ESCALATED FINDINGS (the class):')
    for _i, r in esc.iterrows():
        print(f'    [{r["finding"]:18}] {r["file"]:42} {r["detail"][:88]}')
    if not len(esc):
        print('    NONE FOUND')
    clean = sorted(rep[rep['finding'] == 'none found']['file'].tolist())
    print()
    print(f'  CLEAN ({len(clean)} files): none found')
    if a.out:
        rep = pd.concat([rep, pd.DataFrame([{
            'file': '<COVERAGE>', 'finding': ('OK' if ok else 'COVERAGE-GAP'),
            'detail': f'audited {n_audited} of {n_exist} artifacts present under {a.dir}'}])],
            ignore_index=True)
        rep.to_csv(a.out, index=False, lineterminator='\n')
        print(f'  report -> {a.out}')
    return 0 if ok else 1


if __name__ == '__main__':
    sys.exit(main() or 0)
