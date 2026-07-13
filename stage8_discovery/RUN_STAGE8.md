# RUN_STAGE8.md — equiDOT Stage 8 discovery run package

Self-contained. Every `.py` here is BYTE-IDENTICAL to its ratified project
original (engine/ vs project; scanners/ + orchestrator/ vs the Step-13/14
ratified deliverables). Hash-match the manifest at the bottom before running.

Two files are NOT copies and are packaged helpers:
- `stage8.py` — launcher (resolves the layout; no signal/TM/threshold logic).
- `scanners/f0_to_schema.py` — **FLAGGED FOR FULL AUDIT**: re-scores F0
  survivors through the ratified wf 6-fold to place F0 on the same
  survival-first basis as F1–F11. Not a pure formatter (adds re-scoring
  logic; no new TM, no new threshold).
- `scanners/run_f0_full.py` — thin wrapper that sets the F0 module global
  `MIN_PF = 2.0` (operator param, trim only) before `main()`, so the ratified
  `triple_convergence_and_d2ddir.py` stays byte-identical (no source edit).
- `scanners/run_f1_parallel.py` — parallelises the ratified F1 ordered-pair
  search across cores. ZERO new signal/TM/threshold logic; reuses
  `sequential_temporal.pair_mask` + `score_candidate` (byte-identical scoring)
  via a thin A-subset-vs-full-pool helper. `sequential_temporal.py` is NOT
  modified. Run it DIRECTLY (see Step 2), not via `stage8.py`.

---

## A. SETUP

Ratified environment:
- Python 3.12.3
- numpy 2.4.4
- pandas 3.0.2

```
pip install numpy==2.4.4 pandas==3.0.2
```

Path / baseline resolution — do NOT set PYTHONPATH by hand. Run everything
through the launcher:

```
python stage8.py <script.py> [args]
```

The launcher puts `engine/ scanners/ orchestrator/` on `sys.path`, runs with
CWD = package root, and makes the 8 baseline CSVs in `data/` resolvable at the
root (symlink; falls back to copy on systems without symlink permission — e.g.
Windows). The ratified loader reads the baseline by CWD-relative filename, so
this is what lets the byte-identical engine run unmodified. `discovery_results/`
is written at the package root.

Baseline sanity (printed on every load): `152983 rows x 171 cols`,
`Range: 2026.01.19 15:49 -> 2026.06.25 18:18`, `0 duplicate rows | 0 NaN`,
warmup `chosen=6900`, first scannable `2026.01.26 20:54`.

---

## B. STEP 1 — F0 (run separately; it is the long pole)

F0 is `C(117,3)=260,130` triples over the 249-condition pool, both directions,
D2D=confirm, deduped at overlap 0.80. Run it on its own with the MIN_PF=2.0
trim:

```
python stage8.py run_f0_full.py
```

Writes `dots_results/raw_survivors.csv` and `dots_results/deduped_survivors.csv`.
Runtime: hours on a single core (the heaviest single job — run it first / in
the background). Then convert survivors to the common schema:

```
python stage8.py f0_to_schema.py dots_results/deduped_survivors.csv
```

Writes `discovery_results/results_F0_triple_convergence_and_d2ddir.csv` (14-col
schema). This re-scores each survivor through the wf 6-fold — **flagged for
full audit** (see top).

---

## C. STEP 2 — orchestrator (F1–F9, F11 at full scope; ingests F0)

**F1 first — run it in parallel (it is the orchestrator's long pole).**
F1 = 238² × 15 × 2 = 1,699,320 ordered-pair candidates; ~1.7 cand/s single-core
≈ 11–12 days. Run it across cores (Ryzen 9 270: 8 physical / 16 threads) →
~1.5 days. Run it DIRECTLY (not through `stage8.py` — Python multiprocessing on
Windows uses spawn, which re-imports the main module, so the parallel runner
must be its own `__main__`; it self-bootstraps engine/ imports and the
baseline):

```
python scanners/run_f1_parallel.py          (default 8 workers)
python scanners/run_f1_parallel.py 16        (16 workers)
```

Prints a start banner (candidate count, workers, pool, start time), a live
progress line every 30 s (% / done / total / elapsed / ETA / rate), a
`chunk k/8 done — …` line per chunk, and a final summary. Writes
`discovery_results/results_F1_sequential_temporal.csv` (14-col schema).
Parity: `python scanners/run_f1_parallel.py proof` re-runs a small A-subset both
parallel and serial and asserts row-for-row identity.

Then the orchestrator (F1 and F0 are both drop-in ingests — if their CSVs are
present it schema-validates and skips in-process; else runs in-process):

```
python stage8.py discovery_orchestrator.py full
```

- Loads baseline + oracle ONCE, passes into every scanner (nothing recomputes).
- Runs F2–F9, F11 at full scope; ingests `results_F1_...csv` and
  `results_F0_...csv` if present; writes `discovery_results/results_F<n>_<script>.csv`.
- Collates `discovery_results/discovery_master.csv` (family tag; persistence-first
  sort; no rows dropped).

---

## D. STEP 3 — F0 density over the survivors

Sweep the fused F10 density dimension over the discovered/selected set:

```
python stage8.py triple_convergence_and_d2ddir.py density dots_results/deduped_survivors.csv
```

Prints, per direction, `count>=k` bands (bars / trades / aggPF / worst-day /
hard-stop / spread) so the density-vs-performance relation is legible over the
chosen set (discriminating — unlike density over the raw 249 pool).

---

## E. OUTPUT

`discovery_results/discovery_master.csv` — every candidate from every family
(survivors AND rejects; only floor is each scanner's MIN_TRADES sample-size
floor). Columns:

```
family, script, signal_def, direction, d2d_mode, trades, WR, agg_pf,
worst_day_usd, hard_stop_days, folds_plus, min_fold_pf, spread_pf, survival
```

Already sorted persistence-first: `folds_plus` desc, `min_fold_pf` desc,
`worst_day_usd` asc, `agg_pf` desc, `WR` desc. `worst_day_usd` is raw at lot
1.0 (a ranking axis to minimize toward 0 — NOT hard-gated at −2500; multiply by
lot size after the fact). `survival` is the reference −2500 flag, not a cut.
Selection is a later step; nothing is dropped here.

---

## F. TROUBLESHOOTING

- pandas fragmentation `PerformanceWarning` (repeated single-column insert):
  cosmetic, ignore.
- `[F0] ... not found — Skipping F0 at collation`: the F0 schema CSV is not yet
  in `discovery_results/`. Run Step 1 (`run_f0_full.py` → `f0_to_schema.py`)
  first, then re-run the orchestrator.
- Windows without symlink permission: the launcher copies the 8 CSVs to root
  instead of symlinking (one-time ~200 MB); harmless.

---

## MANIFEST — sha256 (Auditor hash-match)

```
9b27119ab5649e1d82c3781bcd1e37ab6528cd56c956e09a2f85614cfd0cd0d9  data/equiDOT_recon171_step7_part1.csv
32e30da5b1fbf70d36f31cb84e69cbd7963320a0e014954fc19880d76d789d3e  data/equiDOT_recon171_step7_part2.csv
602df7438e312448ef5640745d6d289a96f3a854ab52427d15fd63bf8f05e414  data/equiDOT_recon171_step7_part3.csv
db52d43640f28d51dff730eb71e30c7091edf7f3bc0194ccd618ac78b72b0795  data/equiDOT_recon171_step7_part4.csv
341b4ed1caed7d5df5219d6b26f20994fcf524797e60fa2de3c5f80de6ac42a6  data/equiDOT_recon171_step7_part5.csv
c0443346abc1ef4cdd0f41bf0520dad05dc1de2f96fd5f0927138956221541f8  data/equiDOT_recon171_step7_part6.csv
12fa174b93a50bafd230f3cfd9196e45df1de7d54907e59a319f365421478836  data/equiDOT_recon171_step7_part7.csv
3605299a3fa1df1f7ff958aafd88d7f76f016d8d60ed4936424fce7a2c102d09  data/equiDOT_recon171_step7_part8.csv
6530e2508b17f4c4523a97f9a5a8e065180334811c16cc0849573afdce75a767  engine/core.py
518862bf19fb532814d3b4bb327cf2091a479d51cb29400d27e86c4ae9c294c1  engine/dots_thresholds.py
f88d2f472c278643e0c118b7b02800b2684b4c91961a29a2eb1422fb6b66d84c  engine/portfolio_simulation_engine.py
c4337a151b5fae3446c7bd720d084965030ddceea30f0e63972724ccfde623b3  engine/wf.py
31165e9a17df3bd9fc08bb64c1a957701e9e3b5fbd7b3ff8b6d2769be0a77596  orchestrator/discovery_orchestrator.py
423e6e60c38efa8c8d41a0b14624d429bfd12e5e5a227b0f606d8f484ae204ad  reference/equiDOT_discovery_blueprint.md
1a7a9d423381ed7f3daa7d1f6ec849ce4dcd84bb3c6c10a3c82bc5c12f6dfad7  reference/equiDOT_discovery_pattern_map.md
7908ed0c5fbcfabef98375b076340f7118f2d6866eec5c9a0ef9b4e494bdefcc  scanners/conditional_interaction.py
5594fa73a7d3ab5765bc57e47c72e73c2600783efec344276328cbc1081ee72d  scanners/cross_variable_structure.py
a95c521cd55c62d7a4a3c191b60bc85aebbb50a4ea8980b0f44c5c8cd7277ef3  scanners/divergence_nonconfirm.py
f878d3b46c8ba4ba9748da52bd22e486ef808057872f834987651aa291d5ceae  scanners/f0_to_schema.py
868bc7edf5fed2eb4a301e02cf7c2a95de729c58684f68d83720d788a6d442e5  scanners/mean_reversion.py
cd3afbfe69947669bbcea8ab28049e854b82eac0e9d9852fac3064c67ea79323  scanners/persistence_autocorr.py
08848774ca1c4376a16ca2c790af74fbb65d5246696fa26bb282879700e640fd  scanners/rolling_leadlag.py
8a8a276cfbef8f464f09cd8e01d0ad18ccafde4fa2e082c0c09da54f8cfc6a35  scanners/run_f0_full.py
47bf4d0ce4b9cd21d6beb01f6bb808cc809fe95c32331c00f4fe3433220d11f8  scanners/run_f1_parallel.py
cda5b74590774f747b56b379b20ad68371c5cdcc2513639c9f9ce699e751c7e1  scanners/sequential_temporal.py
2e5f1703aaa25c88310117f0070ba33cfadae28d2090894f7d734b36e6c95c1f  scanners/session_temporal.py
8cb42c9d9891e529077002737b0885def340c1cbe70f273f76d19558e94987fa  scanners/state_transition.py
147deb44d1b54739602b7c47ea34aac34ae3df4825794456ee5b2f09e77d9e6a  scanners/threshold_crossing.py
5ed2221e5339fb467040b5a4f4b887ed0100f193ef2b0c75f87bdbad0c940e3a  scanners/triple_convergence_and_d2ddir.py
8e8f59d80e23e3155788064ec29d181c34d4702b8c9f8b851d13fdcd926b8371  stage8.py
```
