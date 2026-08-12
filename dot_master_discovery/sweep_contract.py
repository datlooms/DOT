"""sweep_contract.py — THE BLANKABLE-COLUMN CONTRACT GATE.

    python sweep_contract.py [--dir <package root>]
    exit 0 = clean, exit 1 = at least one unguarded numeric read

WHY THIS EXISTS. Three defects in a week, three lost starts, ONE PATTERN: a
function's RETURN CONTRACT changed and the sites that CONSUME it were not swept.

  * a file's sha changed          -> the registries that check shas were not updated
  * calls to _blank_pf were added -> the definition was never written
  * _blank_pf's return type changed to allow '' -> the readers still called
    .astype(float), and stage 8 died on the aggregation line after twenty minutes

None of the existing gates can see any of these. The undefined-symbol gate does
not fire because the symbol exists. The never-called sweep does not fire because
it is called. The symbol-table diff does not fire because nothing was LOST. They
are all TYPE-AND-CONTRACT changes, not symbol changes.

WHAT THIS CHECKS. A column produced through blank_sentinel_ratio / _blank_pf can
hold ''. Any site that reads such a column numerically - astype(float), float(),
np.sort, mean/std/min/max/percentile, or a bare comparison - must first go through
pf_aggregate, pf_is_undefined, or an explicit blank branch. This reports the ones
that do not.

SCOPE, STATED HONESTLY: it is a per-line textual check, not dataflow analysis. It
cannot prove a column is blankable at a given site; it flags the shape and leaves
the judgement. A false positive costs a glance; the false negative it is replacing
cost twenty minutes of stage 8.
"""

import argparse
import os
import re
import sys

BLANKABLE = ('agg_pf', 'min_fold_pf', 'gated_solo_PF', 'win_loss_ratio', 'breakeven_wr',
             'margin_pp', 'avg_win', 'avg_loss')
NUMERIC = re.compile(
    r"(astype\(\s*(?:float|np\.float|'float')|"
    r"\bfloat\(|np\.sort\(|np\.mean\(|np\.median\(|np\.std\(|np\.percentile\(|"
    r"\bmin\(|\bmax\(|\.mean\(\)|\.std\(\)|\.median\(\)|\.sum\(\)|"
    r":[<>^]?\d*\.\d+f\})")
GUARDS = ('pf_is_undefined', 'pf_aggregate', 'pf_passes_floor', 'pf_sort_key',
          'blank_sentinel_ratio', '_blank_pf', 'to_numeric', 'errors=', 'if not ',
          '_pf_ok_floor', 'pf_is_undef')
# Producers that never emit '' - a literal dict of zeros, or the definition itself.
BENIGN = ("pf_passes_floor", "float('nan')", "int(_pf_ok.sum()", "'agg_pf': 0.0", "'min_fold_pf': 0.0", 'def blank_sentinel_ratio',
          'def pf_is_undefined', 'def pf_aggregate', 'def pf_passes_floor',
          'def pf_sort_key', 'BLANKABLE', 'S5_GATE', 'GATE (write only')


INDIRECT = re.compile(r"(astype\(\s*(?:float|np\.float)|np\.sort\(|\.mean\(\)|\.std\(\))")


def check(path):
    """Only files that ACTUALLY blank can produce '' - flagging the rest is noise.

    And the read that crashed stage 8 indexed by a LOOP VARIABLE:

        for stat in ['agg_pf', 'WR', 'folds_plus', 'worst_day_usd']:
            v = g[stat].values.astype(float)

    'agg_pf' never appears on the offending line, so a per-line literal match
    cannot see it. The blankable name is in the loop header. So this also scans
    each function for a literal list containing a blankable column and flags any
    indirect numeric read inside that function.
    """
    src = open(path, encoding='utf-8', errors='replace').read()
    if os.path.basename(path) == 'sweep_contract.py':
        return []
    if not any(g in src for g in ('_blank_pf', 'blank_sentinel_ratio')):
        return []
    out = []
    lines = src.split('\n')
    # indirect: a loop over a list literal containing a blankable name
    for i, ln in enumerate(lines, 1):
        m = re.search(r"for\s+(\w+)\s+in\s+\[([^\]]*)\]", ln)
        if not m:
            continue
        var, items = m.group(1), m.group(2)
        if not any(c in items for c in BLANKABLE):
            continue
        ind = len(ln) - len(ln.lstrip())
        for j in range(i, min(i + 40, len(lines))):
            body = lines[j]
            if body.strip() and (len(body) - len(body.lstrip())) <= ind:
                break
            prev_b = lines[j - 1] if j >= 1 else ''
            if re.search(rf"\[\s*{var}\s*\]", body) and INDIRECT.search(body) and \
                    not any(g in body or g in prev_b for g in GUARDS):
                out.append((j + 1, f'{items.split(",")[0].strip()} (via loop var {var})',
                            body.strip()[:66]))
    for i, ln in enumerate(lines, 1):
        st = ln.strip()
        if st.startswith('#') or not st:
            continue
        cols = [c for c in BLANKABLE if re.search(rf"['\"]?\b{re.escape(c)}\b", ln)]
        if not cols:
            continue
        if not NUMERIC.search(ln):
            continue
        # A guard may sit on the PREVIOUS physical line of a wrapped conditional -
        # L1125 reads `else round(float(...))` with `if not pf_is_undefined(...)` above
        # it. Checking only the offending line reports a site that IS guarded.
        prev = lines[i - 2] if i >= 2 else ''
        if any(g in ln or g in prev for g in GUARDS) or any(b in ln for b in BENIGN):
            continue
        out.append((i, cols[0], st[:66]))
    return out


def main():
    ap = argparse.ArgumentParser(description='Blankable-column contract gate.')
    ap.add_argument('--dir', default=os.path.dirname(os.path.abspath(__file__)))
    a = ap.parse_args()
    mods = []
    for sub in ('.', 'engine', 'scanners', 'orchestrator'):
        d = os.path.join(a.dir, sub)
        if not os.path.isdir(d):
            continue
        for fn in sorted(os.listdir(d)):
            if fn.endswith('.py'):
                mods.append((fn if sub == '.' else f'{sub}/{fn}', os.path.join(d, fn)))
    bad = 0
    examined, scoped_out = [], []
    for rel, path in mods:
        try:
            _src = open(path, encoding='utf-8', errors='replace').read()
        except OSError:
            scoped_out.append(f'{rel} (unreadable)')
            continue
        if os.path.basename(path) == 'sweep_contract.py':
            scoped_out.append(f'{rel} (this tool)')
        elif not any(g in _src for g in ('_blank_pf', 'blank_sentinel_ratio')):
            scoped_out.append(rel)
        else:
            examined.append(rel)
        for ln, col, txt in check(path):
            bad += 1
            print(f'  UNGUARDED  {rel}:{ln}  reads {col} numerically  -> {txt}')
    print(f'  COVERAGE: {len(examined)} of {len(mods)} modules EXAMINED; '
          f'{len(scoped_out)} SCOPED OUT because they never call a blanking helper, so they '
          f'cannot emit \'\'. That skip is deliberate - and it is NAMED, because an '
          f'unexamined file is not a passing file.')
    if len(examined) <= 3:
        print(f'      examined: {examined}')
    _unreadable = [x for x in scoped_out if '(unreadable)' in x]
    if _unreadable:
        print(f'  *** {len(_unreadable)} MODULE(S) UNREADABLE: {_unreadable}. Deliberately '
              f'scoped-out files are fine; a file that could not be OPENED is a coverage gap '
              f'and must not pass. ***')
        return 1
    if bad:
        print(f'  *** {bad} UNGUARDED NUMERIC READ(S) of a blankable column. A zero-loss cell '
              f'holds \'\' and these will raise at run time, possibly deep inside a stage. ***')
        return 1
    print('  UNGUARDED NUMERIC READS: none found')
    return 0


if __name__ == '__main__':
    sys.exit(main())
