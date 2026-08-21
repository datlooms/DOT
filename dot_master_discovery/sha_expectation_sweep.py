"""sha_expectation_sweep.py — FIND AND FIX STALE SHA EXPECTATIONS IN DOCUMENTATION.

    cd <repo root>
    python dot_master_discovery/sha_expectation_sweep.py            audit, changes nothing
    python dot_master_discovery/sha_expectation_sweep.py --apply    rewrite

    exit 0 = clean, exit 1 = stale references found (audit mode) or a document unreadable

RUN IT FROM THE REPO ROOT. It walks upward for documents and computes actual shas from
dot_master_discovery/**/*.py. Run from inside the package and it reaches ~18 of 46
documents, missing non_negotiable_prompts/, foundational_documents/ and every root-level
.md - a silent coverage gap of exactly the kind this tool exists to close. The coverage
line it prints tells you immediately: 46 documents from the root, far fewer elsewhere.

WHY IT MATTERS, AND WHY THE OBVIOUS IMPLEMENTATION FAILS.

A SHA AUTHORISED IS NOT A SHA RECORDED. When a file changes, every document quoting its
sha becomes a false statement, and a future instance reading one is given a fact that was
true a fortnight ago. That has cost this project real time more than once.

THE OBVIOUS APPROACH - grep the repo for the OLD sha and replace it - MISSES MOST OF THE
DAMAGE. Measured on this repo: sweeping for the nine shas a file had recently held found
ONE stale reference. The filename-paired sweep below found TWENTY-FOUR across seven
documents, because the other documents carried shas from EARLIER still, which were not in
the list anyone thought to search for. YOU CANNOT SEARCH FOR A SHA YOU HAVE FORGOTTEN
THE FILE EVER HAD.

So this pairs by POSITION instead: each `<name>.py` claims the NEXT 12-hex token on the
line, provided no other filename intervenes. That needs no knowledge of history - it
compares what the document asserts against what the file IS.

AND THE PAIRING HAS TO BE POSITIONAL, NOT PER-LINE. Two earlier attempts reported ~425
and ~28 phantom drifts by cross-multiplying every filename on a line against every sha on
it; a registry line listing five files and five shas became twenty-five combinations.
Only the nearest-following-token rule gives the true count.

SCOPE, STATED HONESTLY: it can only check a sha a document states NEXT TO a filename. A
sha quoted in prose a paragraph away from the name it belongs to is invisible here, and no
amount of regex fixes that - it is a documentation convention problem. Write
`master.py 47e8c14d6c97` and this tool keeps it true forever.
"""

import argparse
import glob
import hashlib
import os
import re
import sys

TOKEN = re.compile(r"([A-Za-z_0-9]+\.py)|\b([0-9a-f]{12})\b")
PACKAGE = 'dot_master_discovery'


def _sha12(path):
    h = hashlib.sha256()
    with open(path, 'rb') as f:
        for blk in iter(lambda: f.read(1 << 20), b''):
            h.update(blk)
    return h.hexdigest()[:12]


def actual_shas(root):
    """Every .py in the package, by basename.

    A duplicate basename in two directories is reported rather than silently resolved:
    picking one arbitrarily would make the tool assert something it cannot know.
    """
    out, dupes = {}, {}
    for p in sorted(glob.glob(os.path.join(root, PACKAGE, '**', '*.py'), recursive=True)):
        b = os.path.basename(p)
        if b in out:
            dupes.setdefault(b, [out[b][1]]).append(p)
            continue
        out[b] = (_sha12(p), p)
    return out, dupes


def documents(root):
    docs = set()
    for pat in ('**/*.md', '**/*.txt'):
        docs.update(glob.glob(os.path.join(root, pat), recursive=True))
    return sorted(docs)


def scan_line(line, actual):
    """Yield (position, filename, declared, actual) for each filename->sha pairing.

    POSITIONAL: a filename claims the NEXT sha token with no other filename between.
    """
    toks = [(m.start(), m.group(1), m.group(2)) for m in TOKEN.finditer(line)]
    for i, (_pos, fn, _s) in enumerate(toks):
        if not fn or fn not in actual:
            continue
        for pos2, fn2, sha2 in toks[i + 1:]:
            if fn2:
                break
            if sha2:
                yield pos2, fn, sha2, actual[fn][0]
                break


def sweep(root, apply_fix=False):
    actual, dupes = actual_shas(root)
    docs = documents(root)
    stale, examined, unreadable, touched = [], 0, [], {}
    for d in docs:
        try:
            text = open(d, encoding='utf-8', errors='replace').read()
        except OSError as exc:
            unreadable.append(f'{d} ({type(exc).__name__})')
            continue
        if re.search(r'[0-9a-f]{12}', text) and re.search(r'[A-Za-z_0-9]+\.py', text):
            examined += 1
        lines = text.split('\n')
        changed = False
        for i, _ln in enumerate(lines):
            # rescan the line after each rewrite: positions shift only if lengths differ
            # (they do not, 12 hex for 12 hex) but a line can hold several pairings.
            seen = set()
            while True:
                hit = None
                for pos, fn, declared, act in scan_line(lines[i], actual):
                    if declared != act and (pos, fn) not in seen:
                        hit = (pos, fn, declared, act)
                        break
                if hit is None:
                    break
                pos, fn, declared, act = hit
                stale.append((os.path.relpath(d, root), i + 1, fn, declared, act))
                seen.add((pos, fn))
                if apply_fix:
                    lines[i] = lines[i][:pos] + act + lines[i][pos + 12:]
                    changed = True
                    touched[d] = touched.get(d, 0) + 1
                else:
                    break
        if apply_fix and changed:
            with open(d, 'w', encoding='utf-8') as f:
                f.write('\n'.join(lines))
    return {'stale': stale, 'docs': len(docs), 'examined': examined,
            'unreadable': unreadable, 'touched': touched, 'dupes': dupes,
            'files': len(actual)}


def main():
    ap = argparse.ArgumentParser(description='Find and fix stale sha expectations.')
    ap.add_argument('--root', default='.', help='repo root (run from there)')
    ap.add_argument('--apply', action='store_true', help='rewrite; default is audit only')
    a = ap.parse_args()
    root = os.path.abspath(a.root)
    if not os.path.isdir(os.path.join(root, PACKAGE)):
        print(f'  ABORT: {PACKAGE}/ not found under {root}. RUN THIS FROM THE REPO ROOT - '
              f'from inside the package it reaches a fraction of the documents and reports '
              f'clean on the ones it never opened.')
        return 2
    # exit 2 is a REFUSAL TO RUN, distinct from exit 1 (ran, found stale). A wrong
    # working directory must never read as a pass.
    r = sweep(root, apply_fix=a.apply)
    print(f'  COVERAGE: {r["docs"]} documents found, {r["examined"]} carry a sha/filename '
          f'pair and were EXAMINED, {r["docs"] - r["examined"]} carry none. '
          f'{r["files"]} python files supplied the actual shas.')
    if r['dupes']:
        print(f'  *** DUPLICATE BASENAMES - not checked, because which file a document meant '
              f'cannot be known: {  {k: len(v) for k, v in r["dupes"].items()} } ***')
    if r['unreadable']:
        print(f'  *** {len(r["unreadable"])} DOCUMENT(S) UNREADABLE: {r["unreadable"]}. An '
              f'unexamined document is not a passing one. ***')
    if r['stale']:
        verb = 'REWROTE' if a.apply else 'STALE'
        print(f'  {verb} {len(r["stale"])} pairing(s):')
        print(f'    {"document":52}{"ln":>5}  {"file":34}{"declared":14}actual')
        for d, ln, fn, dec, act in r['stale']:
            print(f'    {d:52}{ln:>5}  {fn:34}{dec:14}{act}')
        if a.apply:
            print(f'  files touched: { {os.path.basename(k): v for k, v in r["touched"].items()} }')
        else:
            print('  re-run with --apply to rewrite. Nothing has been modified.')
    else:
        print('  STALE PAIRINGS: none found')
    return 1 if (r['stale'] and not a.apply) or r['unreadable'] else 0


if __name__ == '__main__':
    sys.exit(main())
