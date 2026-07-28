# DOT MASTER DISCOVERY — CORRECTION CHECKLIST
**Version 2, 2026-07-28. Supersedes the uncommitted v1.**
**THIS IS THE VERSIONED DOCUMENT EVERY SEAT VERIFIES AGAINST.** Supervisor, Quant, Auditor and Developer all work from this file. Evidence lives in `dot_master_discovery/POST_SCAN_DEFECTS.md` (441 lines). Doctrine lives in `DOT_signal_discovery_mantra.md`.

**REPO STATE AT WRITING:** master.py `3d45bd3b7f74`. Supervisor verdict on v1 of the objectives: **REJECT** — six objectives closed, five HALF-fixed in a state the Supervisor called "more hazardous than the pre-brief state", and a fourteenth defect found.

**OPERATOR STATE:** `DOT_deploy` deleted. No cached S3, no markers, no artifacts. **Every run from here is from scratch.**

---

# PART 0 — THE DESIGN CHANGE THAT SUPERSEDES SEVERAL OBJECTIVES

**OPERATOR DIRECTIVE, and it is the most important item in this document.**

**THE SELECTION LAYER STOPS CHOOSING. IT BECOMES A CATALOGUE.**

Not one book chosen by an objective. **Fourteen books, one per family, containing EVERY valid signal in that family** — no top-N, no cap, no argmax. The operator reads all fourteen and composes the final system himself.

    per SIGNAL:  direction, trades, WR, PF, worst-day, folds_plus, min_fold_pf, OOS,
                 TERRAIN COVERAGE %, and same-bar co-occurrence counts (how many
                 times it fires alongside 1 / 2 / 3 / 4+ other signals)
    per BOOK:    family, signal count, LONG/SHORT split, union terrain coverage %

**WHY THIS IS RIGHT, AND WHY IT KILLS THREE DEFECTS OUTRIGHT.**
There is no argmax left to corrupt. Objective 8's `/n_signals` cannot shrink a catalogue to two entries. Objective 7's entry-bar miscount cannot distort a ranking that does not exist. The fourteenth defect's stubbed constraint gate cannot silently pass a book, because constraints become COLUMNS THE OPERATOR READS rather than a gate that quietly is not applied.

**It is doctrine rule 2 applied properly: include, and let the evidence sort.** A chooser was built where a map was wanted.

**HOW THE OPERATOR GOT HERE, in his words:** the committed book was never selected by an objective — it was assembled by hand, and the finding that mattered (same-bar 3+ at WR 98.0% / PF 53.70) was noticed AFTER the fact by looking at the data. **Every automated selection attempt so far has produced something worse than his hand-assembly.** So the tool's job is to surface everything, measured, and let him do the part he has repeatedly done better.

**ONE HONEST LIMIT.** FailConc and TailDep are PAIRWISE — they measure how signals fail together and only exist for a SET. They cannot be per-signal columns. The catalogue carries per-signal statistics plus same-bar co-occurrence; the pairwise constraints are computed against whatever book the operator assembles afterwards. **Say so in the artifact header; do not fabricate a per-signal value for a set property.**

**OBJECTIVE 9 IS NO LONGER A DECISION.** Per-candidate terrain coverage stops being "objective versus reported column" — it is simply one of the catalogue's columns and MUST be computed, because it is a primary thing the operator wants to read.

---

# PART 1 — CRITICAL PATH. THE RUN IS INVALID WITHOUT THESE.

## C1 — PATH RESOLUTION. THE ONE CERTAIN CORRUPTOR.
**Status: NOT FIXED. Live poison. Reproduces on the next run.**

`orch.RESULTS_DIR` is reassigned to the run tree at entry points, but:
- `master.py L858-860` still passes `os.path.join(_ROOT, 'discovery_results')` AND `dots_results` as fallback search paths to `build_family_evidence`.
- `family_evidence.py L73-77` `_find_outputs` **UNIONS across all search dirs** rather than taking first-match.
- `family_evidence.py L53` the F1 pattern is `('F1_part*.csv', 'results_F1_*.csv')` — it will read the part files objective C2 exists to exclude.
- `orchestrator L48` `RESULTS_DIR = "discovery_results"` is still the module default.

**This is what produced D8:** `family_evidence.csv` reported F0 at 19,777 and F13 at 5,142 while every other family read 0, because those two were the only ones with files in the stale legacy folder — **from a different dataset.** The report then stated ten times that "S3 discovery has not been run for this family on this dataset", in a run where those families produced 445,000 rows.

**AND IT EXPLAINS F13's 1-BYTE OUTPUT.** F13 writes shards to `RESULTS_DIR`. The operator has deleted `discovery_results/` from the repo, the module default still points there, so F13 writes into a path created empty and produces nothing.

**REQUIRED:** every read and write resolves inside `--out`. Delete the legacy fallbacks at L858-860. Point F13's `RESULTS_DIR` at the run tree. Change `_find_outputs` from union-glob to run-tree only. **No fallback to any legacy directory, on any code path, ever.**

## C2 — S4's GLOB MUST EXCLUDE `*_part*.csv`
**Status: NOT FIXED.** One line. Belongs with C1.

Even with the auto-split removed (C7), S4's `glob('results_F*.csv')` must never admit part files. This cost a full day: S9 split `discovery_master.csv` into 17 parts and `results_F1_sequential_temporal.csv` into 6, **deleting the originals**; S4's glob matched the headerless parts; `pd.read_csv` turned each part's first data row into column names. Result: `candidates.csv` with a data row fused onto its header, 46,245 rows against a reported 41,148, every family count wrong, 6,938 duplicates. **Hours were spent diagnosing a selection defect that was a file-format defect.**

## C3 — THE CATALOGUE. REPLACES THE OBJECTIVE CLUSTER.
**Status: the underlying defects are NOT FIXED; the design change supersedes how they are fixed.**

Per PART 0, the selection layer emits **fourteen per-family catalogues**, not one chosen book. The following must still be corrected because the catalogue's columns depend on them:

**C3a — `clusters_from_entries` MUST COUNT DISTINCT SIGNALS ON A BAR, NOT ENTRY ROWS.**
`selection.py L272-282` counts `i - start` — entry bars within N of each other. **One signal firing five times inside five bars scores identically to five signals converging on one bar.** The same-bar co-occurrence columns in the catalogue are meaningless until this is right.
Three populations are conflated. Measured on the committed book and independently reproduced by the Supervisor:

    A) SAME-BAR, DISTINCT SIGNALS   <- THE TARGET, and what the catalogue reports
       1 signal   1199 tr  WR 88.7%  PF   3.18  wd -$574.0  136 losses
       2 signals   974 tr  WR 91.0%  PF   5.27  wd -$365.8   88 losses
       3+ signals  505 tr  WR 98.0%  PF  53.70  wd -$138.9   10 losses
       5+ signals  160 tr  WR100.0%  PF 999     wd  +$59.5    0 losses
    B) CONCURRENT OPEN POSITIONS (overlapping, different entry bars)
       1 open     1381 tr  WR 89.6%  PF   4.18   |   3+ open 853 tr PF 6.01
       ONLY 27% OVERLAP WITH A — largely different trades
    C) ENTRY BARS WITHIN N   <- what the code counts. Weakest of the three.

**C3b — `/n_signals_d` OUT OF ANY OBJECTIVE.**
`selection.py L290` `return float(ge)/traded_days/n_signals_d`. The n-th signal must lift cluster count by `1/(n-1)` just to not reduce the score — #3 by 50%, #11 by 10%. **The argmax is minimal by construction; 2 signals is the true optimum of this metric.** With the catalogue there is no argmax, but the normalised value may remain as a REPORTED column for comparability. It must never gate inclusion.

**C3c — THE FOURTEENTH DEFECT: THE CONSTRAINT GATE IS STUBBED OPEN.**
`master.py L1174` `_sel_con = lambda d, ss: (True, '')`. **FailConc, TailDep, mCVaR and absolute survival are computed by S5B, written to CSV, verified across nine audit passes — and the search bypasses every one of them.**
The Supervisor's words: *"last time the selection layer was built, audited, and never called; this time the constraint layer is built, audited, written to disk, and the search that would use it is passed a stub."*
Under the catalogue design the stub becomes moot for inclusion, **but the constraint machinery must not be left dangling.** Either wire it to compute against an operator-assembled book on demand, or delete the stub and state plainly in the artifact header that pairwise constraints are computed post-hoc. **A verified region unreachable from production is the exact pattern this pipeline exists to close.**

## C4 — PER-CANDIDATE TERRAIN COVERAGE
**Status: NOT STARTED.** `coverage_by_direction` and `terrain.py` exist but have only ever run on the incumbent book.

For every candidate: which of the 7,490 terrain episodes its entries touch, per direction. **This is a primary catalogue column, not an optional diagnostic.** The terrain is the fixed denominator — MARKET, price-only, 3,816 UP / 3,674 DOWN, near 50/50 — and coverage against it is how the operator judges whether a set of signals sees the whole map.

## C5 — THE WALK-FORWARD MUST PRODUCE A NUMBER
**Status: HALF FIXED, and the half-fix is worse than none.**

`funnel_rerun` now derives correctly from pool existence and provenance (`master.py L1431-1449`) — good. But `book_rates = [nan]*len` at L1460 and the book arm is still a hardcoded `'UNEVALUABLE...'` string at L1462-1463. **When the pool exists and `_pool_ok` is True, the numerator still never computes.**

**So the run now reports `funnel_rerun=True` alongside a verdict of `nan` — confidently inconsistent in the one artifact the redesign exists to produce.**

The null arm already works: 89/80/81 qualifiers, rates 0.236 / 0.2375 / 0.2593, seeded, reproducible. **Only the numerator is missing.** The book arm must re-run the funnel inside each training segment per §I.2 and produce `book_persistence(s)`.
**VERIFY BY OPENING THE ARTIFACT, NOT BY READING A DIFF.** This defect was reported fixed once already while the artifact stayed byte-identical.

---

# PART 2 — SPEED. NOT VALIDITY, BUT IT IS THE DIFFERENCE BETWEEN A NIGHT AND A WEEK.

## C6 — PARALLELISE EVERY STAGE
**Status: NOT STARTED.** `workers` reaches **2 of 15 stages** — `s3_discovery` L202 and `run_diagnostic_families` L639. Every other stage takes `w`, which is warmup, not workers. **Thirteen stages single-threaded on a 16-thread machine.**

    stage                   cost                             independent work
    f0_rows_from_raw L421   5 HOURS MEASURED, list comp      19,757 independent re-scores
    S5B search              ran silent, unmeasured           41,148 evals per greedy step
    S5C                     null ~1,950 sims; book arm 3x    splits and triples independent
    S7 contenders           6 full portfolio scores          C0-C5 independent
    S3B D2D                 4 variants x ~13k trades         independent
    S6 regen                unmeasured

**Profile all fifteen on the real pool and report the table BEFORE parallelising.** Do not triage — the operator's night is the budget.

**HARD CONSTRAINT: DO NOT PARALLELISE THE DEDUP.** `deduplicate()` is a global greedy pass against a running `keep_sets` list; splitting it lets near-duplicates survive that a single pass drops. **Dedup stays single-pass in ascending chunk order; only the RE-SCORE parallelises, and a parity proof against the serial result is mandatory.**

**Greedy is sequential BETWEEN steps and parallel WITHIN them.** Step 2 depends on step 1's choice; the 41,148 marginal evaluations inside a step are independent. That is where the 14x lives. (Applies only if any ranked search survives the catalogue redesign.)

## C7 — REMOVE THE AUTO-SPLIT
**Status: verify.** `split_tree()` at `master.py L111-122`, called from S9 L1747, walks the entire output tree and replaces every `.csv`/`.jsonl` over `--chunk-mb` with numbered parts, **deleting the original.**

**DELETE IT. Remove `--chunk-mb` from the pipeline. Every artifact written as ONE file**, exactly as when BOOK-50 was scored. It existed solely so the operator could upload artifacts for review — a manual, occasional need for two or three files, never a pipeline concern. **If splitting is ever wanted it is a standalone utility run by hand on a named file; it never touches the run tree and never removes an original.**

## C8 — EVERY LONG STAGE PRINTS PROGRESS
**Status: NOT STARTED.** `f0_rows_from_raw` and the S5B search both ran for hours with no heartbeat, no ETA, no markers. **A stage that cannot be watched cannot be trusted and cannot be diagnosed.** The operator has twice had to decide whether to kill a healthy run because it looked stalled.

---

# PART 3 — OPERATIONAL HYGIENE

## C9 — THE PIPELINE WRITES ITS OWN LOG
**Status: NOT STARTED.** A complete log lands in the output tree at the end of every run, automatically. The first full scan lost its early stages past the terminal buffer; the diagnosis of three defects survived only because the operator happened to capture it by hand.

## C10 — CLEAN OUTPUT IN ALL THREE MODES
**Status: write-side handled; console and stderr NOT.**
- Console, piped, and redirected must all render correctly. The operator saw `ΓòÉΓòÉΓòÉ` — UTF-8 read as cp437. **This has been fixed twice and broken twice.** Either emit pure ASCII or set the console code page from inside the program.
- **No routine output on stderr.** PowerShell renders every stderr line as a red `NativeCommandError`; pandas `PerformanceWarning` floods a healthy run with red, indistinguishable from failure. Suppress the known-benign warnings or route them to the log. Errors on stderr; warnings not.

## C11 — DELIVERY
**Status: N/A until build.** **One .zip of the complete `dot_master_discovery/` directory** — not a patch, not loose files. The operator has no working tree to patch against. **Windows: .zip, NOT .tar.gz.** Include a manifest with every file's sha256[:12].

---

# PART 4 — VERIFY, DO NOT BUILD

**V1 — D1, F0 double-count.** Reported FIXED (`orchestrator L1222` skips `ingest_f0()` when the chunked path collated). Confirm on a clean run that the per-family count equals the collated row count.

**V2 — D8.** Cannot be closed while C1's mechanism is live. Confirm after C1 that `family_evidence.csv` reads the run's own tree — F1 at its true row count, not 0.

**V3 — F13's 1-byte output.** Diagnosed as the same root as C1. Confirm it writes real shards into the run tree.

**V4 — D4, D6, and the tolerance grid.** Reported FIXED — `orchestrator L1151` re-queues only missing chunks and L1144 prints `MISSING [...]`; `master L726` S2B always recomputes; `selection L307` defaults to `(1, 5, 10, 15, 20, 25, 30)`. Spot-check each survives the build.

---

# PART 5 — MEASURE, DO NOT FIX

**M1 — POOL COMPOSITION.** The filtered pool was **90.6% F1 / 8.9% F0** because F0 is pre-filtered twice (its own fast scorer at `MIN_PF >= 2.0`, `MIN_TRADES >= 30`, then S5) and F1 has no pre-screen at all. Two families ranked in one pool having cleared different funnel depths.
**Under the catalogue design this largely dissolves** — per-family books mean F1's numerical dominance no longer crowds F0 out of a shared ranking. **Measure and report it; do not remedy. A per-family quota is not available — doctrine rule 3.**

**M2 — DEPTH VERSUS THRUST.** ~3% of terrain episodes touch a size>=5 book cluster, and only 12-15% of deep-cluster bars are thrust bars. Stable across all eight grid cells. **Depth and thrust are largely disjoint populations.** Re-measure once per-candidate coverage exists (C4). If they stay disjoint, that is a substantive finding about the vocabulary, not a defect.

---

# PART 6 — SEQUENCING

    C1 + C2   ->   C3 + C4   ->   C5
    C6, C7, C8, C9, C10 are independent and may ship anytime

**MUST LAND TOGETHER:** C3a, C3b, C3c and C4. They share one call site (`master.py L1160-1186`, `selection.py L272-304`). **Shipping any one alone reproduces the failure it was meant to fix and buries it under an artifact that looks like a real search ran.**

**C1 BEFORE ANYTHING ELSE.** It is the only certain corruptor. Without it the overnight run reads stale or foreign data through the legacy fallback paths.

**C5 DEPENDS ON C3** producing a real catalogue for the book arm to re-run per split.

---

# THE PATTERN THAT KEEPS RECURRING — FOURTEEN INSTANCES

A code path verified by a route other than the one production uses:
`verify_family_coverage` by direct call · `orchestrate()` by reading · F0 chunking by `--parity` alone · the frame binding in the parent only · the S5B consumption path by assumption · `build_book` never run on a non-F0/F1 candidate · `coverage_by_direction` twice · an entire unreachable function region · a duplicate definition · a hardcoded `False` · a wrong directory · a UTF-8 fix that worked on console but not pipe · an auto-split never tested against a run that had to READ its own output · **and now a constraint layer computed, written to disk, audited nine times, and passed a stub by the search that was supposed to use it.**

**EVERY CHANGE MUST BE EXERCISED ON THE REAL PATH, AT REAL SCALE, BEFORE IT IS CALLED DONE. Verifying that a function COMPUTES correctly is not verifying that anything CONSUMES it.**
