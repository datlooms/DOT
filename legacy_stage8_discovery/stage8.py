import os
import sys
import shutil
import runpy

# ═══════════════════════════════════════════════════════════════
#  stage8.py — self-contained launcher for the Stage 8 package.
#  Resolves the assembled layout so the BYTE-IDENTICAL engine/scanner files
#  run unmodified:
#    - puts engine/ scanners/ orchestrator/ on sys.path (import by name)
#    - the ratified loader reads the baseline by CWD-relative filename, so we
#      run with CWD = package root and make the 8 baseline CSVs resolvable
#      here via symlink (fallback: copy) from data/.
#  discovery_results/ is written at the package root.
#  Usage:  python stage8.py <script.py> [args...]
#      e.g. python stage8.py discovery_orchestrator.py full
#           python stage8.py sequential_temporal.py
#           python stage8.py f0_to_schema.py dots_results/deduped_survivors.csv
# ═══════════════════════════════════════════════════════════════

ROOT = os.path.dirname(os.path.abspath(__file__))
CODE_DIRS = ['engine', 'scanners', 'orchestrator', '.']
for d in CODE_DIRS:
    sys.path.insert(0, os.path.join(ROOT, d))
os.chdir(ROOT)

for name in os.listdir(os.path.join(ROOT, 'data')):
    if not name.endswith('.csv'):
        continue
    src = os.path.join(ROOT, 'data', name)
    dst = os.path.join(ROOT, name)
    if os.path.exists(dst):
        continue
    try:
        os.symlink(src, dst)
    except (OSError, NotImplementedError):
        shutil.copy2(src, dst)

if len(sys.argv) < 2:
    sys.exit("usage: python stage8.py <script.py> [args...]")

target = sys.argv[1]
sys.argv = sys.argv[1:]
for d in ['scanners', 'orchestrator', '.', 'engine']:
    cand = os.path.join(ROOT, d, target)
    if os.path.exists(cand):
        runpy.run_path(cand, run_name='__main__')
        break
else:
    sys.exit(f"script not found: {target}")
