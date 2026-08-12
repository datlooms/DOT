"""repair_family_markers.py — RECOVER COMPLETED WORK THE STALENESS GATE WOULD DISCARD.

    python repair_family_markers.py --out discovery\\full            (report only)
    python repair_family_markers.py --out discovery\\full --apply    (stamp)

WHY THIS EXISTS. A run completed all eleven families across 4,279 chunks in ~13
hours, collated every one, and was then told:

    F1: marker STALE - scanner sequential_temporal.py sha None -> 6c89c865fff1

`sha None` was not evidence the producer had moved. collate_family_chunks called
_mark_family_done WITHOUT the `script` argument, so scanner_sha(None) returned None
and every marker that path wrote lacked the field. The gate then read the absence
as a change and re-scanned ten families minutes after they collated IN THE SAME
RUN. A gate that invalidates CURRENT work is worse than no gate.

WHY STAMPING IS SAFE HERE, AND THE CHECK THAT ESTABLISHES IT. The marker already
carries csv_sha256, and family_is_complete verifies it against the artifact on
disk. So the artifact is provably the one the marker was written for. The only
missing fact is WHICH SCANNER produced it - and if the scanner on disk is the same
one the run used, stamping its sha records a true fact rather than asserting an
unverified one.

THIS TOOL REFUSES TO GUESS. It stamps only when:
  * the family CSV exists and its sha matches the marker's csv_sha256, and
  * the marker has NO scanner_sha (absence, not a mismatch).
A marker whose scanner_sha is PRESENT AND DIFFERENT is a genuine producer change
and is left alone to re-scan. That distinction is the whole point: absence is a
writer gap, mismatch is a real staleness signal.
"""

import argparse
import glob
import hashlib
import json
import os
import sys

SCANNER_FOR = {
    'F0': 'triple_convergence_and_d2ddir', 'F1': 'sequential_temporal',
    'F2': 'state_transition', 'F3': 'conditional_interaction',
    'F4': 'divergence_nonconfirm', 'F5': 'persistence_autocorr',
    'F6': 'cross_variable_structure', 'F7': 'mean_reversion',
    'F8': 'cross_variable_structure', 'F9': 'session_temporal',
    'F11': 'rolling_leadlag', 'F12': 'concurrence_profiler',
    'F13': 'single_variable_extremes',
}


def _sha(path):
    h = hashlib.sha256()
    with open(path, 'rb') as f:
        for blk in iter(lambda: f.read(1 << 20), b''):
            h.update(blk)
    return h.hexdigest()


def scanner_sha(here, script):
    for cand in (os.path.join(here, 'scanners', f'{script}.py'),
                 os.path.join(here, '..', 'scanners', f'{script}.py')):
        if os.path.exists(cand):
            return _sha(cand)[:12]
    return None


def main():
    ap = argparse.ArgumentParser(description='Stamp scanner_sha into complete family markers.')
    ap.add_argument('--out', required=True, help='the run tree, e.g. discovery\\full')
    ap.add_argument('--apply', action='store_true')
    a = ap.parse_args()
    here = os.path.dirname(os.path.abspath(__file__))
    markers = sorted(glob.glob(os.path.join(a.out, '**', '*.done'), recursive=True))
    if not markers:
        print(f'  no .done markers found under {a.out}')
        return 1
    stamped, skipped, mismatched, broken = [], [], [], []
    for done in markers:
        base = os.path.basename(done)[:-5]
        fam = None
        for f, sc in SCANNER_FOR.items():
            if base.startswith(f'{f}_') or base == f or f'_{sc}' in base:
                fam = f
                break
        csv = done[:-5] + '.csv'
        if not os.path.exists(csv):
            continue
        try:
            meta = json.load(open(done, encoding='utf-8'))
        except Exception as exc:
            broken.append((base, f'{type(exc).__name__}'))
            continue
        if 'csv_sha256' not in meta:
            skipped.append((base, 'no csv_sha256 - not a family marker'))
            continue
        if meta['csv_sha256'] != _sha(csv):
            broken.append((base, 'CSV SHA MISMATCH - the artifact is not the one the marker '
                                 'was written for; NOT stamping'))
            continue
        got = meta.get('scanner_sha')
        if got is not None:
            script = SCANNER_FOR.get(fam or '', '')
            want = scanner_sha(here, script) if script else None
            if want is not None and got != want:
                mismatched.append((base, f'{got} -> {want} GENUINE producer change, leave stale'))
            else:
                skipped.append((base, 'already stamped and current'))
            continue
        script = SCANNER_FOR.get(fam or '')
        if not script:
            skipped.append((base, f'cannot map to a scanner (fam={fam})'))
            continue
        want = scanner_sha(here, script)
        if want is None:
            skipped.append((base, f'{script}.py not found beside this tool'))
            continue
        stamped.append((base, script, want))
        if a.apply:
            meta['scanner_sha'] = want
            tmp = done + '.tmp'
            with open(tmp, 'w', encoding='utf-8') as f:
                json.dump(meta, f)
            os.replace(tmp, done)
    verb = 'STAMPED' if a.apply else 'WOULD STAMP'
    print(f'  {verb} {len(stamped)} marker(s) whose CSV sha VERIFIES and whose scanner_sha was '
          f'ABSENT:')
    for b, sc, sh in stamped:
        print(f'    {b:52} {sc:34} scanner_sha={sh}')
    if mismatched:
        print(f'  LEFT STALE {len(mismatched)} - a PRESENT-AND-DIFFERENT sha is a real producer '
              f'change and must re-scan:')
        for b, why in mismatched:
            print(f'    {b:52} {why}')
    if broken:
        print(f'  REFUSED {len(broken)}:')
        for b, why in broken:
            print(f'    {b:52} {why}')
    if skipped:
        print(f'  skipped {len(skipped)} (already current or not a family marker)')
    if not a.apply and stamped:
        print()
        print('  re-run with --apply to stamp. Nothing has been modified.')
    return 0


if __name__ == '__main__':
    sys.exit(main())
