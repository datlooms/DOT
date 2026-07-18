# DOT — MASTER ORCHESTRATOR: CANONICAL STAGE-SEQUENCE SPEC

**Date:** 2026-07-18 · **Author:** QRA (analyst seat) · **Status:** design/logic only — the Developer builds `master.py` from this. Supervisor verifies against the ratified pipeline + `discovery_map.md` before build.

**Ground truth:** `discovery_map.md` (§1–§7). This spec applies the two Supervisor-confirmed corrections: **(1) F10 is folded into F0 (concurrence lens yielded ~zero, stayed folded; F12 profiler + the 9 `concurrence_*.csv` are the diagnostic remnant) — the family set is complete, not gapped. (2) `signal_full_records.csv` / `signal_per_day_pnl.jsonl` are STALE (pre-S.19/S.20/D2D) and are REGENERATED as a pipeline stage, never carried.**

---

## DESIGN PRINCIPLES (bind every stage)

- **One command, one script.** `python master.py` runs ingest → thresholds → discovery → filter → contenders → committed score → report. Options: `--data DIR` (default `/data/`), `--out DIR` (default `/discovery/`), `--workers N` (≤12), `--stage S0..S9` (run/resume one stage), `--market-label STR` (report tag).
- **Market-agnostic.** Nothing is US30-specific except the frozen committed book. The oracle computes percentiles from whatever data is ingested; a new market (BTC/S&P) runs the same pipeline provided its EA export carries the **same 171-feature schema**.
- **Sacred parity.** The oracle, `wf.py`, `core.py`, and the ratified engine are byte-locked. `master.py` **prints each sacred sha at startup and aborts if any drift** (see Sacred Registry). Discovery must equal live: no look-ahead, oracle is the only threshold source.
- **Checkpoint everything.** Every stage writes a `.done` marker; long stages checkpoint per-family and per-shard (reuse the working `_f13_shards/` + `.done` pattern). Restart skips any unit whose `.done` exists and whose inputs are unchanged (input sha recorded in the marker).
- **Upload-friendly by construction.** Every artifact landing in `/discovery/` is auto-split into chunks under the size limit (§9). The operator never hand-splits.
- **Progress + ETA** on every multi-unit stage; `--workers` bounds the pool.

Directory contract:
```
/data/        any EA .csv export(s) drop here — 1 or N files, any filename
/discovery/   all outputs land here, auto-split into chunks
  raw/          per-family survivor scans (pre-schema)
  results/      unified results_F*.csv (post-schema) + filtered candidates
  scored/       fresh signal_full_records + per_day_pnl (regenerated)
  contenders/   mechanism head-to-head table
  committed/    book + committed-system score
  master_report.md
master.py
master_guide.md
```

---

## 1. ORDERED STAGES (raw `/data/` CSV → committed report)

| # | Stage | INPUT | OUTPUT (→ location) | Existing script/logic | Checkpoint boundary |
|---|---|---|---|---|---|
| **S0** | INGEST & VALIDATE | `/data/*.csv` | validated in-memory baseline (152,983×171 for US30) | new thin loader wrapping `core.py` schema; `portfolio_simulation_engine.load_sealed_baseline` for the split-parts case | `S0.done` (records input file shas + row/col counts) |
| **S1** | ADAPTIVE THRESHOLDS | baseline | `adaptive` (per-bar hi/lo per FEAT_) + `structural` gates | `dots_thresholds.compute_adaptive_thresholds` / `compute_structural_gates` (SACRED, sha printed) | `S1.done` |
| **S2** | POOL & ANCHORS | baseline, adaptive, structural | condition pool (scannable vocabulary) + `ST_Flip` anchor + warm-up floor | `sequential_temporal.build_condition_pool` / `anchor_array`; `portfolio_simulation_engine.warmup_floor` | `S2.done` |
| **S3** | FAMILY DISCOVERY | baseline, adaptive, pool, anchor | per-family survivors → `/discovery/raw/` (auto-split) | the 13 scanners (F0 committed + F1–F13), driven by `discovery_orchestrator.py` | per-family `.done` + per-shard `.done` |
| **S4** | SCHEMA UNIFY | raw survivors | `results_F*.csv` (common 14-col) → `/discovery/results/` | `f0_to_schema.py` (F0, re-scores via `wf`); each family scanner already emits the 14-col schema for F1–F13 | per-family `.done` |
| **S5** | CANDIDATE FILTER | `results_F*.csv` | `candidates.csv` (survivors of the gate) → `/discovery/results/` | the gate `trades≥30 & folds_plus≥4 & agg_pf≥2` (§5) applied to the `survival` columns | `S5.done` |
| **S6** | FULL-FIELD SCORING (REGEN) | candidates, baseline | **fresh** `signal_full_records.csv` + `signal_per_day_pnl.jsonl` → `/discovery/scored/` (auto-split) | `run_full_analysis.py` → `analysis_engine.py` (current engine; **replaces the stale artifacts**) | per-shard `.done` (reuse `_shards/`) |
| **S7** | CONTENDER HEAD-TO-HEAD | committed book, baseline | `contenders.csv` (6 mechanism variants ranked) → `/discovery/contenders/` | `score_g.py` option-map + `conviction.build_conviction` toggles + `wf` folds (§6) | `S7.done` |
| **S8** | COMMITTED-SYSTEM SCORE | book + baseline | `committed_score.txt` + build-up → `/discovery/committed/` | `reproduce_dot.py` logic (folded in): `build_book` → full `conviction` → `run_portfolio` → daily wd/mDD + folds + OOS | `S8.done` |
| **S9** | REPORT & SPLIT | all stage outputs | `master_report.md` + all `/discovery/` chunks | new reporter + the auto-splitter (§9) | `S9.done` |

Stages are strictly ordered S0→S9; S3/S4/S6 parallelise internally across `--workers`. A crash mid-S3 resumes at the first family/shard without a `.done`.

---

## 2. INGEST (S0) — arbitrary CSV(s) in `/data/`

**Schema validation (the gate that makes it market-agnostic):**
1. Read every `*.csv` in `/data/`. Detect header: **part1-style** (has the `Time,Open,High,...` header) vs **headerless continuation** parts (the baseline's parts 2–8 are headerless — first line is data). Rule: a file whose first row parses as the 171-column header is a header file; files whose first row parses as numeric data are continuations.
2. **N-file handling — CONCATENATE in filename sort order**, then assert **time strictly increasing, 0 duplicate rows, 0 NaN** (the `load_sealed_baseline` invariants). This is how the US30 8-part baseline already loads to 152,983×171. A single-file export loads directly. A new market dropping one big CSV or N chunks both work — the concatenation + invariant check is identical.
3. **Column contract:** exactly `Time` + 171 features, names matching the export schema. If a file has the retired 126-col shape or a 256-col raw step-7 shape, route to `core.py` reconstruction first (S0a, below); otherwise ingest directly.
4. **S0a (conditional reconstruction):** only if the input is a raw 256-col step-7 export (`64_256_*`) — run `core.py` (`cols171 = cols256[:171]`, `NEW45 = cols171[126:]`, cold-start recompute + δ-shift). For an already-sealed 171-col export (the normal EA drop), S0a is skipped. This keeps the pipeline market-agnostic: any clean 171-col EA export is ingest-ready with no reconstruction.

**Abort conditions (reported, not silent):** wrong column count, non-increasing time, duplicate/NaN rows, or a feature-name mismatch. The report states plainly whether the ingested data is a valid oracle-processable export.

---

## 3. ADAPTIVE THRESHOLDS (S1) — the oracle

- `dots_thresholds.compute_adaptive_thresholds(df)` → per-bar hi/lo for every FEAT_ (mechanism-D rolling-2500 day-refreshed floor-index percentiles, swept p30/p90/p97 via runtime `_D_SPEC` extension — never edited on disk). `compute_structural_gates(df)` → VWAP_Z ±2, OR_Position 0.80/0.20.
- **`dots_thresholds.py` is SACRED, sha `518862bf19fb`.** `master.py` prints this sha at S1 start and **aborts on mismatch** — this is the export=live parity check, printed every run.
- Market-agnostic by construction: percentiles are data-relative, so the same oracle self-calibrates to BTC/S&P without change.

---

## 4. DISCOVERY (S3) — the full family run

All 13 scanners run in one pass (F0 committed; the rest exploratory/diagnostic). **F10 is folded into F0** (concurrence lens yielded ~zero and stayed folded into the convergence attempt; the `concurrence_profiler` (F12) + 9 `concurrence_*.csv` are the diagnostic remnant — the family set is complete, F10 is not a missing scanner).

| F | Script | Scans | Path |
|---|---|---|---|
| **F0** | `triple_convergence_and_d2ddir.py` | Triple-convergence (3 vars at extremes) + D2D direction; **F10 concurrence folded in**. Internal `MIN_TRADES=10`, `MIN_PF=4.0`, dedup `OVERLAP=0.80`; `run_f0_full.py` overrides `MIN_PF=2.0` as trim. | **COMMITTED — BOOK-50 is drawn from F0 survivors** |
| **F1** | `sequential_temporal.py` | Ordered pairs `A@(t−k) → B@t`, ST_Flip-anchored, `LAGS 1..15`. | **COMMITTED — 2 F1 pairs entered BOOK-50** |
| F2 | `state_transition.py` | Transition / regime-change | exploratory |
| F3 | `conditional_interaction.py` | Conditional / context-gating | exploratory |
| F4 | `divergence_nonconfirm.py` | Divergence / non-confirmation | exploratory |
| F5 | `persistence_autocorr.py` | Persistence / autocorrelation | exploratory |
| F6 | `threshold_crossing.py` | Threshold-crossing / momentum-ignition | exploratory |
| F7 | `mean_reversion.py` | Mean-reversion-from-extreme | exploratory |
| F8 | `cross_variable_structure.py` | Relative / cross-variable structure | exploratory |
| F9 | `session_temporal.py` | Session / calendar conditioner | exploratory |
| **F10** | *(folded into F0)* | Concurrence lens — null; not a standalone scanner | **folded, concurrence null** |
| F11 | `rolling_leadlag.py` | Cross-variable × windowed lead-lag / correlation | exploratory |
| F12 | `concurrence_profiler.py` | Variable-depth concurrence **measurement** (MEASURES, never selects) | diagnostic |
| F13 | `single_variable_extremes.py` | Single-variable extremes, both dirs/polarities | exploratory — **documented negative** (0 candidates; convergence necessary) |

**Committed path = F0 (→ BOOK-50) + the 2 adopted F1 pairs.** Everything else is exploratory/diagnostic and exists to (a) prove convergence is necessary (F13), (b) measure whether depth predicts outcome (F12), (c) surface additive structure a new market might reward (F2–F9, F11). On a **new market** all families run identically and the report shows which ones yield gate-passing candidates there.

Each scanner writes to `/discovery/raw/<family>/`, auto-split. Per-family and per-shard `.done` markers give crash-resume (the F13 30-shard `_f13_shards/` trail is the working precedent to reuse for every family).

---

## 5. FILTER (S5) — the candidate gate

The **auditor-validated gate**, applied to the unified `results_F*.csv` (`family, script, signal_def, direction, d2d_mode, trades, WR, agg_pf, worst_day_usd, hard_stop_days, folds_plus, min_fold_pf, spread_pf, survival`):

```
KEEP a candidate iff:  trades >= 30  AND  folds_plus >= 4  AND  agg_pf >= 2.0
```

- `trades≥30` — sample-size floor (below it, WR/PF are noise).
- `folds_plus≥4` — persistence across ≥4 of the 6 monthly folds (survives out-of-fold, not one-month luck).
- `agg_pf≥2.0` — minimum aggregate edge.

This is the gate the blind auditor independently validated as the line between signal and noise. It sits **after schema-unify (S4), before scoring (S6)** — it is a cheap pre-filter on the 14-col records so full-field scoring (S6) only runs on survivors. Output: `/discovery/results/candidates.csv`. Survival-first ordering is preserved: worst-day is carried through so the report can rank on it before PF.

---

## 6. CONTENDERS (S7) — mechanism head-to-head

The point: **let the data rank every "arbitrary" choice, on any dataset.** Six variants, each scored on the same committed book through the ratified engine (`score_g` option-map + `conviction.build_conviction` toggles + `wf` folds), reported side-by-side with net / WR / PF / daily worst-day / daily mDD / 6-fold min-PF / OOS:

| # | Contender | conviction toggles | US30 net (reference) |
|---|---|---|---|
| C0 | **Flat book** (1-lot, no conviction/gaps/D2D) | all off | $58,277 |
| C1 | + S.20 conviction (Hurst/recentFB longs) | hurst+recentFB | $71,377 (+$13,100) |
| C2 | + S.20 gap-singles (Hurst-gap, FB-gap) | + gap_singles | $89,432 (+$18,055) |
| C3 | + S.21 D2D-conviction (2× both dir) | + d2d_conviction | $90,447 (+$1,015) |
| C4 | + S.21 D2D-gap (flat 2-lot) = **FULL** | + d2d_gap | **$92,347 (+$1,900)** |
| C5 | sizing variant(s) — e.g. blanket-2× (known-rejected), conviction-off | per-variant | ranked, incl. the worst-day cost each lever buys |

Each contender also reports the **trade it makes** (e.g. conviction buys +$13,100 but deepens worst-day −127.5 → −153.7). This is the layer that validates or refutes each committed choice on the data itself — for US30 it reproduces the known build-up; for a new market it shows whether the same layering holds there. Output: `/discovery/contenders/contenders.csv` (+ the ranked build-up for the report).

---

## 7. COMMITTED SYSTEM (S8) — assemble & score

The `reproduce_dot.py` role, folded in:
1. **Book assembly.** US30: load the frozen ratified `book50_signals.csv` (48 F0 triples + 2 F1 pairs, `trigger/direction/signal_def`) — do **not** re-select (the committed selection is ratified and out-performs a blind reconstruction). New market: assemble a book from `candidates.csv` (S5) by the documented selection rule — survival-first rank (worst-day, then PF), overlap-dedup, cap — and flag it as a **new-market book** distinct from the frozen US30 book.
2. **Score.** `build_book` → full `conviction.build_conviction(..., d2d_conviction=True, d2d_gap=True)` → `run_portfolio` (the sole ratified trade path) → daily worst-day / daily mDD via `wf.daily_pnl_points`, 6-fold min-PF, OOS (May–Jun) PF+net.
3. **Headline.** US30 → **$92,347 / WR 92.3% / PF 6.40 / daily wd −104.4 / daily mDD −145.9 / 6-6 folds min-PF 5.39 / OOS PF 6.96 / OOS net $29,190.** A new market yields whatever its data produces — same code, same guards. Output: `/discovery/committed/committed_score.txt`.

**Self-check:** when the book is the frozen US30 book, S8 asserts `net==92,347 & trades==2,698` and prints `REPRODUCED` / `MISMATCH — investigate` (the `reproduce_dot.py` canonical check).

---

## 8. REPORT (S9) — the single summary

`master_report.md`, one document, containing:
1. **Ingest attestation** — files, rows×cols, invariants passed, market label.
2. **Oracle parity** — `dots_thresholds.py` sha `518862bf19fb` printed (+ wf/core/engine shas).
3. **Component build-up** — C0→C4 with each delta (the transparent path to the headline).
4. **Headline** — committed-system stats.
5. **Contender ranking** — the six variants, ranked, each with its net / survival cost / OOS (the "arbitrary choice, validated" table).
6. **Per-family coverage** — candidates per family, which families yielded gate-passers, F13 negative confirmation, F12 concurrence read.
7. **Stale-artifact note** — confirms `signal_full_records`/`per_day_pnl` were **regenerated** this run (fresh shas), not inherited.

---

## 9. AUTO-SPLIT (S9, applied to every `/discovery/` artifact)

- **Precedent:** the baseline ships as 8 parts (~19,122 rows each); F1 as `F1_part1/2.csv`. **No in-pack splitter exists** (the part-splits were external) — so a canonical splitter is a required new master utility.
- **Rule:** any output file exceeding `--chunk-mb` (default **9 MB**, safely under common single-file upload limits) is split into `NAME_part1.csv … partN.csv`. CSVs split on **row boundaries** with the header repeated in `part1` only and continuation parts headerless — **exactly the baseline's part1-has-header / parts-2..N-headerless convention**, so the master's own ingest (S0) reads its outputs straight back. JSONL (`per_day_pnl`) splits on line boundaries. A `NAME_manifest.txt` lists the parts + per-part sha for reassembly.
- Result: the operator uploads any single `_partK.csv` under the limit; no hand-splitting, no zips/folders.

---

## 10. CANONICAL SCRIPT MANIFEST — carry vs drop

**CARRY (canonical — the master consolidates these):**

| Group | Files (sha256[:12]) |
|---|---|
| Oracle / SACRED | `dots_thresholds.py` `518862bf19fb`, `wf.py` `793e6e5f8d9a`, `core.py` `6530e2508b17` |
| Ratified engine | `portfolio_simulation_engine.py` `bb498eb13ce3`, `conviction.py` `27af7acee824`, `score_g.py` `3129aecec634`, `score_book50.py` `f2db7eb592a6` |
| F0 scoring | `analysis_engine.py` `fb1a30341e88`, `run_full_analysis.py` `110767ea58dd` (current engine — regenerates S6 fresh) |
| Scanners (13) | `triple_convergence_and_d2ddir.py`(F0) `5ed2221e5339`, `sequential_temporal.py`(F1) `cda5b7459077`, `state_transition.py`(F2) `8cb42c9d9891`, `conditional_interaction.py`(F3) `7908ed0c5fbc`, `divergence_nonconfirm.py`(F4) `a95c521cd55c`, `persistence_autocorr.py`(F5) `cd3afbfe6994`, `threshold_crossing.py`(F6) `147deb44d1b5`, `mean_reversion.py`(F7) `868bc7edf5fe`, `cross_variable_structure.py`(F8) `5594fa73a7d3`, `session_temporal.py`(F9) `2e5f1703aaa2`, `rolling_leadlag.py`(F11) `08848774ca1c`, `concurrence_profiler.py`(F12) `188a5794bce5`, `single_variable_extremes.py`(F13) `0ca336cdf9df` |
| Runners / schema | `run_f0_full.py` `8a8a276cfbef`, `run_f1_parallel.py` `47bf4d0ce4b9`, `f0_to_schema.py` `f878d3b46c8b` |
| Orchestration | `discovery_orchestrator.py` `31165e9a17df` (its scan→filter→schema drive logic is the S3–S4 backbone `master.py` absorbs), `stage8.py` `8e8f59d80e23` (path/baseline resolver — its CWD-relative resolution logic informs S0) |
| Book + reproduction | `book50_signals.csv` `e86a52244501` (frozen US30 book), `reproduce_dot.py` (S8 logic, folded in) |

**DROP (superseded / stale — must NOT be carried as canonical):**

| Item | Why |
|---|---|
| `signal_full_records.csv` `746102aae415`, `signal_per_day_pnl.jsonl` `0910f360a628` | **STALE** — pre-S.19/S.20/D2D engine; version-skewed P&L field. **Regenerated by S6**, never inherited. |
| 126-col generation (`64_186_*`, `equiDOT_recon_part*`) | Retired data generation. |
| `64_236_*` / `64_246_*` intermediates | Superseded by the sealed 171-col baseline. |
| Modelled figures `$90,103` (S.20 harness) / `$92,567` (D2D modelled) | Superseded by built `$92,347`. Not files — do not re-cite. |

**Materialise (currently project-root, belong under `/discovery/`):** the F0 outputs (`results_F0…csv`, `raw_survivors.csv`, `deduped_survivors.csv`) → `dots_results/`; the F1 outputs (`F1_part1/2.csv`) → `discovery_results/`. These are canonical *outputs* regenerated by S3–S4 on each run; the frozen copies are reference only.

---

## SACRED REGISTRY & PARITY (binding on `master.py`)

Byte-locked, printed at startup, **abort on any drift**:

| File | sha256[:12] | Role |
|---|---|---|
| `dots_thresholds.py` | `518862bf19fb` | oracle — sole threshold source (export=live) |
| `wf.py` | `793e6e5f8d9a` | walk-forward folds + daily series |
| `core.py` | `6530e2508b17` | reconstruction (only the S0a raw-export path) |
| `portfolio_simulation_engine.py` | `bb498eb13ce3` | ratified TM — sole trade path |

The master prints the oracle sha at S1 and the full sacred-registry shas in the S9 report. No stage may compute a threshold except through the oracle; no stage may open a trade except through the ratified engine. Any behaviour-changing edit to a sacred file invalidates the run regardless of merit.

---

**End of spec.** The Developer builds `master.py` + `master_guide.md` from this. Open item for Supervisor sign-off: the **new-market book-selection rule** in S8 (survival-first rank → overlap-dedup → cap) is specified but has never been exercised on a non-US30 market — flagged as the one stage whose logic is designed, not yet data-validated.
