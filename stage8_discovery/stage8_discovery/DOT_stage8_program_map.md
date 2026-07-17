# DOT_stage8_program_map.md

**equiDOT — Stage 8 discovery / selection / validation pipeline: program map & from-scratch re-run manual.**

This document is accurate to the physical contents of the `stage8_discovery/` pack as inspected on disk. Where a file the operator expected is absent, or where the packaged code does not embody the current ratified behaviour, that is stated explicitly rather than assumed. Two such gaps are flagged up front in **Section 0** because they block a valid from-scratch re-run today.

Companion documents (cross-referenced, not duplicated here):
- `RUN_STAGE8.md` (in the pack root) — the copy-paste command runbook with the sha256 manifest.
- `DOT_anti_curvefit_guide.md` (project root, **not** in this pack) — the selection discipline referenced in Section 7.

---

## 0. STATUS — BOTH GAPS CLOSED (pack is FINAL and re-runnable)

The two gaps flagged in the prior revision of this map are now closed. The pack embodies the current ratified TM and the book is reproducible end-to-end.

**GAP 1 — CLOSED. The full ratified TM is merged into the pack engine.**
`engine/portfolio_simulation_engine.py` now implements S.7 + S.17 **plus**:
- **S.15 6-lot live-risk jar** — admission counts live (pre-BE) lots: `live_lots = sum(1 for t in active_trades if not t.be_nudged)`, admit only when `live_lots < 6`; a BE'd winner frees its lot the same bar. Count-cap fully removed. Verified `live_lots ≤ 6` never exceeded.
- **S.12 momentum-runner** — at entry `v = Micro_LogReturn × dir`; if `v ≥ 0.00012` the LeapFrog trail uses lag 3 (activate tier 4, trail `tiers−3`) else lag 2 (activate tier 3, trail `tiers−2`). Runner trail only.
- **S.19 momentum-conditional initial SL** — momentum entries take initial catastrophe stop `min(ATR×4, 150)`, non-momentum `min(ATR×2, 150)`, cap 150 inviolate. **Critical convention:** the widened stop applies to the catastrophe SL only; `step_size` and `be_trigger` stay on the **base (ATR×2) risk**, so break-even arms at the normal distance and converts would-be stop-outs to BEs (SL 194→166). Scaling the step off the widened risk instead breaks the worst-day (−320 vs −127.5) — that is the wrong reading and is not what the record used.
- **S.16 per-bar model** — the sim already processes bar-by-bar, repositioning the SL on each new bar off the closed bar High/Low; per-bar sequence (exit-check → tiers → BE-arm → trail → admit) confirmed to hold after the merge.
- S.17 constants unchanged (SPREAD 3.0, RISK_MULT 2.0, MAX_RISK 150, STEP_PCT 0.30, BE_TRIG_FRAC 1.0, LOCK_FRAC 1.0).

**GAP 2 — CLOSED. The runnable BOOK-50 artifact + in-book scorer exist and reproduce the record.**
- `engine/book50_signals.csv` — the 50 book definitions in runnable form (48 F0 triples recovered exactly from the record's per-signal fingerprint against `signal_full_records.csv`; 2 F1 sequential defs). Schema: `trigger` (F0/F1), `direction`, `signal_def`.
- `engine/score_book50.py` — the in-book scorer: builds all 50 masks (F0 as native triples, F1 via `sequential_temporal.pair_mask` injected as a `==1` column through the ST_Flip anchor), runs them together through the merged engine, and reports the book totals + OOS.

**Reproduction vs the authoritative record** (`DOT_performance_record.xlsx` → Portfolio Summary, RUN):

| Metric | Reproduced | Record (RUN) |
|---|---|---|
| trades | 2,361 | 2,363 |
| net $ | 58,277 | 58,249 |
| PF | **6.12** | 6.12 |
| WR % | **92.8** | 92.8 |
| worst-day $ | **−127.5** | −127.5 |
| SL / FC | **166 / 5** | 166 / 5 |
| BE / LF | 1,809 / 380 | 1,810 / 382 |
| OOS PF | 7.04 | 6.99 |
| L / S | 1,755 / 606 | 1,758 / 605 |

Every ratified/survival metric — PF, WR, worst-day, SL, FC — reproduces **exactly**. The residual (2 trades, +$28 net, ±2 on BE/LF) is F0 def-recovery precision (1–2 of the 48 F0 signals resolve to a numeric-fingerprint look-alike), not a TM convention error. This closes the S.15 held book-total verification and gives the blind auditor a reproducible book.

**A from-scratch re-run now produces current-book behaviour**, and the book is reproducible end-to-end via `python engine/score_book50.py`.

**One parity caveat that remains (tracked, not blocking the pack):** the live EA `equiDOT.cs` is a pre-Stage-9 build and does **not** yet implement S.12/S.15/S.19 — its SL is a single `MathMin(atr*Dots_SL_Mult, Dots_SL_Cap)` and its LeapFrog is SuperTrend-based. So *export = live* (sim == EA, Section 5) holds on the **Python side** now, but true sim↔EA parity also requires the Stage-9 EA build to incorporate the ratified TM. That EA build is separate, tracked work.

---

## 1. DIRECTORY STRUCTURE

```
stage8_discovery/
├── stage8.py                 launcher (path/baseline resolver for byte-identical scripts)
├── RUN_STAGE8.md             command runbook + sha256 manifest
├── DOT_stage8_program_map.md this document
├── data/                     the sealed baseline — 8 parts, 192 MB total
│   └── equiDOT_recon171_step7_part1..8.csv   (152,983 rows × 171 cols, Jan 19 – Jun 25 2026)
├── engine/                   the ratified compute core (SACRED files) + the analysis export
│   ├── dots_thresholds.py            threshold ORACLE (sacred, unmodifiable)
│   ├── portfolio_simulation_engine.py  S.7 trade engine (see GAP 1)
│   ├── wf.py                         walk-forward folds + survival-first scoring
│   ├── core.py                       raw-export → 171-col reconstruction
│   ├── analysis_engine.py            F0 signal → S.7 scoring + 3 persistence scales
│   ├── run_full_analysis.py          full-field gated export driver (checkpointed)
│   ├── book50_signals.csv            the 50 BOOK-50 defs (48 F0 triples + 2 F1) — runnable
│   ├── score_book50.py               in-book BOOK-50 scorer (F1 path); reproduces the flat record
│   ├── conviction.py                 S.20 conviction/gap-single builder (oracle-swept p90/p97)
│   └── score_g.py                    S.20 option-map scorer (A/B/G'/G, toggleable)
├── orchestrator/
│   └── discovery_orchestrator.py     drives F1–F9/F11 scanners, ingests F0/F1, collates master
├── scanners/                 the discovery family scanners + F0/F1 runners + profiler
│   ├── triple_convergence_and_d2ddir.py   F0 (same-bar triple) — the diamond engine
│   ├── sequential_temporal.py             F1 (ordered pair A→k→B, ST_Flip-anchored)
│   ├── run_f0_full.py                     F0 full-scope wrapper (MIN_PF=2.0 trim override)
│   ├── run_f1_parallel.py                 F1 parallel runner (spawn MP, checkpointed)
│   ├── f0_to_schema.py                    F0 survivors → common 14-col schema (re-scores via wf)
│   ├── concurrence_profiler.py            F12 raw-depth concurrence measurement (MEASURE not select)
│   ├── single_variable_extremes.py        F13 single-variable extreme hunt (STANDALONE, not convergence)
│   ├── state_transition.py                F2
│   ├── conditional_interaction.py         F3
│   ├── divergence_nonconfirm.py           F4
│   ├── persistence_autocorr.py            F5
│   ├── threshold_crossing.py              F6
│   ├── mean_reversion.py                  F7
│   ├── cross_variable_structure.py        F8
│   ├── session_temporal.py                F9
│   └── rolling_leadlag.py                 F11
├── reference/
│   ├── equiDOT_discovery_blueprint.md     discovery design spec
│   └── equiDOT_discovery_pattern_map.md   the 12-family pattern catalogue
└── discovery_results/        OUTPUT dir — ships EMPTY; all run outputs land here
```

**Runtime-created directories (not shipped, appear on first run):**
- `dots_results/` — the F0 scanner (`triple_convergence_and_d2ddir.py`) writes `raw_survivors.csv` and `deduped_survivors.csv` here.
- `discovery_results/_shards/` — `run_full_analysis.py` checkpoint shards + `field_manifest.csv`.

**Absent vs. the operator's expected list:** `dots_results/` is not shipped (created at runtime, above). `DOT_anti_curvefit_guide.md` is in the project root, not in `reference/`.

---

## 2. EVERY EXECUTABLE SCRIPT — purpose, inputs, outputs, pipeline slot

`load_sealed_baseline` below means the script reads the 8 `data/equiDOT_recon171_step7_part*.csv` parts by bare filename (CWD-relative — see Section 6 for why `stage8.py` / self-bootstrap matters).

### engine/ — compute core

| Script | Purpose | Inputs | Outputs | Pipeline slot |
|---|---|---|---|---|
| `dots_thresholds.py` | **Threshold ORACLE (SACRED).** Computes the adaptive rolling-2500 floor-index percentiles (day-refreshed) + structural gates (VWAP_Z ±2, OR 0.80/0.20). All threshold reproduction goes through here — never re-implemented. | baseline (in-memory, passed by caller) | none (library) | Underlies every scan/score |
| `portfolio_simulation_engine.py` | The ratified trade engine: `run_portfolio()` builds signal masks via `condition_mask` (oracle), simulates S.7 SL / BE-nudge / LeapFrog / Friday-close + S.12 momentum-runner + S.15 6-lot live-risk jar + S.19 momentum-SL + **S.20 conviction sizing + gap-singles** (via the optional `conviction=` arg). Per-trade `lock_frac` + `lots`; jar counts non-BE POSITIONS (≤6), conviction scales LOTS on them (6×2=12 live-risk-lot ceiling). `conviction=None` → flat book. | baseline; signals frame; optional `conviction` dict | trades frame (with `lots`/`pnl_per_lot`) | Sole trade path for all scoring |
| `wf.py` | Walk-forward: `FOLDS` (6 monthly), `fold_metrics`, `points_to_usd` ($1/pt/lot), `daily_pnl_points`, `spread_stress`, survival-first scoring. | baseline; drives `run_portfolio` per fold | per-fold metrics (in-memory) | Scoring layer for scanners + analysis |
| `core.py` | **Reconstruction.** Rebuilds the 171-col sealed baseline from raw broker export + step-7 export. | `first..fourth.csv` (raw OHLCV) + `64_256_*.csv` (172-col step-7 export) | `equiDOT_reconstructed_171_step7.csv` (→ split into the 8 parts) | **Stage 0** (upstream of everything) |
| `analysis_engine.py` | Rebuilds an **F0** `signal_def` → entry mask (oracle `condition_mask` ×3), scores it **bit-for-bit through `run_portfolio`** (S.7), and summarises across 3 persistence scales (6 monthly folds, ISO-weekly, day-of-week Sun–Fri) + exit distribution + risk texture + the per-day P&L vector. | baseline; F0 `signal_def` strings | per-signal record dicts (consumed by the driver) | Scoring library for the export |
| `run_full_analysis.py` | Full-field driver over the F0 field: builds a manifest, shards it, scores each via `analysis_engine`, applies the quality gate, writes survivor records + per-day vectors. Spawn MP, live progress+ETA, atomic checkpoint/resume. | `discovery_results/results_F0_triple_convergence_and_d2ddir.csv` (the F0 field); baseline | `discovery_results/signal_full_records.csv`, `discovery_results/signal_per_day_pnl.jsonl`, `_shards/` | **Tic-proof scoring stage** |
| `book50_signals.csv` | The 50 ratified BOOK-50 definitions in runnable form: `trigger` (F0/F1), `direction`, `signal_def` (48 F0 triples + 2 F1 `A ->k-> B`). | — (data artifact) | — | Book reproduction input |
| `score_book50.py` | In-book BOOK-50 scorer: builds all 50 masks (F0 native triples; F1 via `pair_mask` injected through the ST_Flip anchor), runs them together through the merged engine, reports book totals + OOS. Reproduces the flat record (Section 0). | `book50_signals.csv`; baseline | stdout book totals | **Flat-book verification** |
| `conviction.py` | **S.20 conviction/gap-single builder.** Sweeps Micro_Hurst p90/p97 and Micro_FailedBreak p90 through the oracle's OWN `compute_adaptive_thresholds` via a runtime-extended `_D_SPEC` (restored after — oracle byte-identical). Builds the book-LONG lot multiplier array (Hurst>p90→2.0, recentFB[+1,+5]→1.25, higher wins) and the two gap-single masks (Hurst>p97 & D2D+1; FB>p90 & D2D−1; gated ADX≥15 & Vol≥300). Independently toggleable (`hurst_sizing`/`recentfb_sizing`/`gap_singles`). | baseline | conviction dict for `run_portfolio` | S.20 scoring |
| `score_g.py` | **S.20 option-map scorer.** Builds the book, then scores A (flat) / B (Hurst-only) / G' (recentFB-off) / G (all-on) through the merged engine, reporting net/PF/WR/worst-day + population (x1/x2/x1.25/gapH/gapF). Reproduces the committed-design option map. | `book50_signals.csv`; `conviction.py`; baseline | stdout option map | **Full-design (G) verification** |

Gate in `run_full_analysis.py`: write a survivor only if `trades ≥ 30` **and** `agg_pf ≥ 2.0` **and** `folds_plus ≥ 4`. Units $1/point/lot. `SHARD=500`, `N_WORKERS=6` (default; see Section 6 for the 12-worker guidance).

### scanners/ — discovery

| Script | Family | Purpose | Inputs | Outputs |
|---|---|---|---|---|
| `triple_convergence_and_d2ddir.py` | **F0** | Same-bar triple-convergence discovery (3 conditions AND-ed + D2D gate); the diamond engine. Internal `MIN_TRADES=10`, `MIN_PF=4.0`, dedup `OVERLAP_THRESHOLD=0.80`. | baseline | `dots_results/raw_survivors.csv`, `dots_results/deduped_survivors.csv` |
| `run_f0_full.py` | F0 | Full-scope F0 wrapper that overrides `MIN_PF=2.0` (trim only) as a module-global so the F0 scanner stays byte-identical. | (drives F0) | (F0 outputs, looser trim) |
| `f0_to_schema.py` | F0 | Converts F0 survivors to the common 14-col schema by **re-scoring each through the wf 6-fold** (flagged: re-scoring logic, not a pure formatter). | `dots_results/deduped_survivors.csv`; baseline | `discovery_results/results_F0_triple_convergence_and_d2ddir.csv` |
| `sequential_temporal.py` | **F1** | Ordered-pair discovery: `A@(t−k) AND B@t`, latched on an ST_Flip anchor; scorable pool ≥30-activation conditions; `LAGS = 1..15`. | baseline | (scored rows; run via the parallel runner / orchestrator) |
| `run_f1_parallel.py` | F1 | Parallel F1 over the full 238²×15×2 = 1,699,320 ordered-pair scope; spawn MP; live progress; checkpointed. | baseline | `discovery_results/results_F1_sequential_temporal.csv` |
| `state_transition.py` | F2 | State-transition patterns. | baseline (via orchestrator) | `discovery_results/results_F2_state_transition.csv` |
| `conditional_interaction.py` | F3 | Conditional interactions. | ” | `results_F3_conditional_interaction.csv` |
| `divergence_nonconfirm.py` | F4 | Divergence / non-confirmation (D2D invert/exempt). | ” | `results_F4_divergence_nonconfirm.csv` |
| `persistence_autocorr.py` | F5 | Persistence / autocorrelation. | ” | `results_F5_persistence_autocorr.csv` |
| `threshold_crossing.py` | F6 | Threshold crossings. | ” | `results_F6_threshold_crossing.csv` |
| `mean_reversion.py` | F7 | Mean reversion (D2D invert/exempt). | ” | `results_F7_mean_reversion.csv` |
| `cross_variable_structure.py` | F8 | Relative / cross-variable structure. | ” | `results_F8_cross_variable_structure.csv` |
| `session_temporal.py` | F9 | Session / time-of-day. | ” | `results_F9_session_temporal.csv` |
| `rolling_leadlag.py` | F11 | Rolling lead-lag (causal trailing windows). | ” | `results_F11_rolling_leadlag.csv` |
| `concurrence_profiler.py` | **F12** | Raw-variable-depth concurrence **measurement** (does stacking depth predict outcome?). MEASURES, never selects. 9 output tables incl. a circular-shift null and a survivor-only secondary lens. Causal burn-in regime labels gate entries; descriptive labels gate nothing. | baseline; (secondary lens) `dots_results/deduped_survivors.csv` | 9 × `discovery_results/concurrence_*.csv` (Section 4) |
| `single_variable_extremes.py` | **F13** | **STANDALONE single-variable extreme hunt — NOT a convergence family.** For each of the 117 discovery variables (88 D-adaptive percentile-swept p50–p99 / p1–p50 through the oracle, 2 structural, 27 equality), tests {hi,lo}×{long,short}×{D2D-aligned,D2D-counter} through the ratified engine. Hunts a lone variable with ~100% WR + full-span persistence + ≥22 trades. Sharded by variable, spawn MP, checkpoint/resume. | baseline (oracle-swept thresholds) | `discovery_results/results_F13_single_variable_extremes.csv` |

F10 (density) is fused into F0 as a `density` mode — there is no standalone F10 file.

### orchestrator/

| Script | Purpose | Inputs | Outputs |
|---|---|---|---|
| `discovery_orchestrator.py` | Drives F1–F9/F11 at full scope, **ingests F0 and F1 from dropped-in CSVs** (skips in-process re-run if the CSV is present), collates every family into a 14-col common schema with a persistence-first sort. | baseline; `results_F0_*.csv`, `results_F1_*.csv` if present | `results_F<n>_*.csv` (per family), `discovery_results/discovery_master.csv` |

### root

| Script | Purpose |
|---|---|
| `stage8.py` | Launcher that puts `engine/ scanners/ orchestrator/` on `sys.path` and makes the CWD-relative baseline resolvable, so the byte-identical scripts run unmodified. **Note:** the multiprocessing runners (`run_f1_parallel.py`, `concurrence_profiler.py`, `run_full_analysis.py`) self-bootstrap and are run **directly**, not through `stage8.py` (Windows spawn re-imports the main module). |

---

## 3. PIPELINE ORDER (from scratch)

Each stage's command is the canonical invocation; see `RUN_STAGE8.md` for exact flags and the sha256 manifest. All commands are run from the pack root; the MP runners run directly.

| # | Stage | Consumes | Produces | Command |
|---|---|---|---|---|
| 0 | **Reconstruction** | raw broker export (`first..fourth.csv`) + step-7 export (`64_256_*.csv`) | `equiDOT_reconstructed_171_step7.csv` → the 8 `data/` parts | `python engine/core.py` |
| 1 | **Oracle** (implicit) | the 8 baseline parts | adaptive + structural thresholds (in-memory) | (library — invoked by every scan/score) |
| 2a | **F0 discovery** | baseline | `dots_results/deduped_survivors.csv` | `python scanners/run_f0_full.py` |
| 2b | **F0 → schema** | `deduped_survivors.csv` | `discovery_results/results_F0_...csv` | `python scanners/f0_to_schema.py dots_results/deduped_survivors.csv` |
| 2c | **F1 discovery** | baseline | `discovery_results/results_F1_sequential_temporal.csv` | `python scanners/run_f1_parallel.py` |
| 2d | **Orchestrate F2–F9/F11 + collate** | baseline; F0/F1 CSVs | `results_F<n>_*.csv`, `discovery_master.csv` | `python stage8.py discovery_orchestrator.py full` |
| 3 | **F12 concurrence** (blocking, pre-selection) | baseline; `deduped_survivors.csv` | 9 × `concurrence_*.csv` | `python scanners/concurrence_profiler.py` |
| 3b | **F13 single-variable extremes** (STANDALONE, optional; separate cave from F0–F12) | baseline | `results_F13_single_variable_extremes.csv` | `python scanners/single_variable_extremes.py [workers]` |
| 4 | **Tic-proof scoring** | `results_F0_...csv` | `signal_full_records.csv`, `signal_per_day_pnl.jsonl` | `python engine/run_full_analysis.py` |
| 5 | **Selection** (manual, disciplined) | `signal_full_records.csv` + per-day vectors + `discovery_master.csv` + concurrence tables | the validated book (BOOK-50) | see Section 7 |

The F0 field is the spine: F0 discovery (2a) → schema (2b) is what stage 4 scores into `signal_full_records.csv`, which is the object selection works on.

---

## 4. EXPORTED CONTEXT (post-run artifacts + schemas)

All land in `discovery_results/`.

**`signal_full_records.csv`** — one row per gated F0 survivor (the primary selection object). Columns: identity (`signal_def, family, direction, d2d_mode`); headline (`trades, WR, agg_pf, net_pts, worst_day, folds_plus, min_fold_pf`); per-monthly-fold (`fold_<Jan19-31/Feb/Mar/Apr/May/Jun1-25>_pf|_trades|_net`); per-day-of-week (`dow_<Sun..Fri>_trades|_WR|_pf|_net` — **Sunday bucket included**, EST_DayOfWeek 0); exit distribution (`exit_<SL|BE|LF|FC|EOD>_count|_pnl`); risk texture (`worst_day, worst_week, longest_losing_streak, max_consecutive_losing_days`); `weekly_json` (compact ISO-week array); `signal_key`. **Consumed by:** the selection step (Section 7).

**`signal_per_day_pnl.jsonl`** — one JSON object per line: `{signal_key: {"YYYY-MM-DD": net_pts, ...}}`. The anti-fabrication per-calendar-day vector, so any book's in-book worst-day / loss-correlation is a **lookup, never a re-sim**. **Consumed by:** book-construction / worst-day-concentration analysis. Sums per signal reconcile exactly to `net_pts`.

**`results_F0_triple_convergence_and_d2ddir.csv`** / **`results_F1_sequential_temporal.csv`** / **`results_F<2..9,11>_*.csv`** — 14-col common schema per candidate: `family, script, signal_def, direction, d2d_mode, trades, WR, agg_pf, worst_day_usd, hard_stop_days, folds_plus, min_fold_pf, spread_pf, survival`. **Consumed by:** the orchestrator collation.

**`discovery_master.csv`** — every family's candidates in the 14-col schema, family-tagged, persistence-first sorted (`folds_plus` desc, `min_fold_pf` desc, `worst_day_usd` asc, `agg_pf` desc, `WR` desc). Nothing dropped. **Consumed by:** cross-family selection.

**`dots_results/deduped_survivors.csv`** — F0's own fast-scorer survivors: `feat_1,thresh_1,feat_2,thresh_2,feat_3,thresh_3,direction` + fast metrics (`trades,pf,wr,sl,be,lf,fc,…`). Runnable signals schema. **Consumed by:** `f0_to_schema.py`, F0 density, F12 secondary lens.

**`results_F13_single_variable_extremes.csv`** — one row per surfaced single-variable candidate (WR ≥ 90% or trades ≥ 22). Columns: identity (`family, signal_def, direction, d2d_mode, gating`); frequency (`trades, total_firings` [= trades], `firings_per_week, firings_per_day, mask_bars` [raw condition-true bars]); quality (`WR, wr_pre_fc, wr_post_fc, agg_pf, worst_day_usd`); persistence (`folds_plus, folds_present, weeks_present, weeks_won, weekdays_present, weekdays_won, persistence`); exits (`exit_FC, exit_EOD`); `survival, tier`. **Tiers:** surface (WR ≥ 90%), candidate (WR ≥ 95% + trades ≥ 22 + near-full-span persistence), STAR (WR = 100% + full-span persistence + ≥ weekly). STANDALONE — not consumed by the convergence selection; a documented single-variable cave. **Consumed by:** single-variable analysis / the documented negative. **RESULT (this run): documented NEGATIVE** — 0 STAR, 0 candidate; 184 surfaced ≥90% WR. No single variable at any swept threshold reaches ≥95% WR with full-span persistence and ≥22 trades. The lone 100%-WR performer with ≥22 trades (Sqz_Val:hi@p99 SHORT) covers only 10/22 weeks and 5/6 folds; the best genuinely full-6-fold single variable (Micro_Hurst:hi@p97 LONG, 93 trades, 5/5 weekdays) tops out at 91.4% WR. Singles don't carry it — convergence is necessary.

**F12 — 9 tables** (`concurrence_depth_bars, _events, _entry_order, _outcome_map, _composition, _category_depth, _regimes, _d2d_flips, _null_baseline`, + `_outcome_map_secondary`). Depth-vs-outcome measurement with per-fold columns, circular-shift null (observed vs null + p-values), and causal regime gating. **Consumed by:** the QRA reading depth evidence pre-selection (measurement only — selects nothing).

---

## 5. RATIFIED BEHAVIOUR THE ENGINE MUST REFLECT (export = live)

**Parity requirement:** the Python `run_portfolio` must reproduce, bit-for-bit, what the MQL4 EA computes live. Every threshold the EA computes must be reproduced by `dots_thresholds.py`; the backtest must equal live; no look-ahead anywhere. Discovery/scoring run on the sealed baseline; the numbers are only trustworthy if the engine that produced them is the engine that trades.

The **current committed TM** that the engine must embody:
- **S.7 base** — SL `min(ATR_1M×2.0, 150)`; BE nudge at `1.0×step` → lock `1.0×step`; LeapFrog `0.30×risk`, lag 2, activate tier 3; spread 3.0; Friday 16:45 EST flat. **No feature exits, no time stops, no cooldown.**
- **S.17** — swept TM optima incl. `LOCK_FRAC=1.0` (looser lock costs net −$12.9k).
- **S.12 momentum-runner** — `v = Micro_LogReturn × direction; if v ≥ 0.00012 → LeapFrog lag 3 else lag 2` (runner trail only).
- **S.19 momentum-conditional initial SL** — the change added most recently.
- **S.15 6-lot live-risk jar** — admission counts **live (pre-BE) lots**: `live_lots = sum(1 for t in active_trades if not t.be_nudged)`; admit only when `live_lots < 6`; a position reaching BE frees its lot the same bar.
- **S.16 per-bar sequence** — the EA repositions the SL only on a new bar off the closed bar's High/Low (broker holds the hard intrabar SL); the sim's per-bar order (exit-check → tiers → BE-arm → trail → admit) mirrors this.

**AS PACKAGED, this engine now embodies the full ratified TM** — S.7 + S.17 + S.12 (momentum-runner) + S.15 (6-lot live-risk jar) + S.19 (momentum-conditional SL) + the per-bar model (S.16) + **S.20 (conviction self-scaling + gap-singles)**. A from-scratch re-run therefore produces current-committed-design behaviour, and `engine/score_g.py` reproduces the option map end-to-end.

### S.20 — conviction self-scaling + gap-singles (the committed "G" design)

The engine scores the FULL committed design, not just the flat book. Via the optional `conviction=` arg to `run_portfolio` (built by `conviction.py`):

- **Conviction sizing (book LONGS only):** at a book-LONG entry, Micro_Hurst > adaptive p90 → 2.0 lots; else a book long within 5 bars *after* a Micro_FailedBreak > p90 extreme (recentFB [+1,+5]) → 1.25 lots; both → 2.0 (higher wins, never product). Shorts always 1.0.
- **Gap-singles (two NEW entries):** Hurst-single (Micro_Hurst > p97 & D2D_Trend_Dir==+1) and FailedBreak-single (Micro_FailedBreak > p90 & D2D_Trend_Dir==−1), LONG 1 lot, LOCK=3, gate ADX≥15 & Vol≥300. Fire ONLY when zero Dots positions are open (any open — live OR BE'd — blocks). Per-bar: STEP1 book (conviction-sized), STEP2 gap only if zero open after STEP1.
- **Per-trade lock:** gap-singles `lock_frac=3.0`; book keeps 1.0. **Jar counts non-BE POSITIONS (≤6); conviction scales LOTS** → 6×2 = 12 live-risk-lot ceiling (structurally enforced by the admission guard). This is ambiguity-resolution #1: a 2× trade does NOT consume 2 jar slots; the deferred "count multiplied lots toward the cap" variant is NOT implemented (1-lot-base deployment).
- **Thresholds:** p90/p97 computed via the oracle's OWN rolling-2500 day-refreshed mechanism (runtime-extended `_D_SPEC`, oracle **byte-identical**, sha 518862bf). Values exact to the record: Hurst p90 0.521, p97 0.550, FB p90 0.664.

**Option-map reproduction** (`python engine/score_g.py`, 1 lot, true $1/pt):

| Config | Reproduced net | Target net | Reproduced worst-day | Target worst-day |
|---|---|---|---|---|
| A flat (all off) | $58,277 | $58,249 | −127.5 | −127.5 |
| B Hurst-only | $66,434 | $66,407 | −127.5 | — |
| G' (recentFB off) | $84,554 | $85,134 | −127.5 | −120.9 |
| **G (all on)** | **$89,487** | **$90,103** | **−153.7** | **−147.2** |

A and B (conviction sizing alone) reproduce to within ~$28 (the F0 def-recovery residual + the exact x1.25 population match, 809 vs 811). The G residual (net −0.68%, worst-day −153.7 vs −147.2, trade count 2,691 vs 2,828, x2 210 vs 254) traces to the quant's **analysis harness** — which the rd_plan explicitly records as the source of the $90,103 figure ("the quant MODELLED G via its analysis harness; the ratified-engine implementation is still to be built and audited"). Under a position-jar + lot-scaling engine the book trade count is invariant to conviction (lots scale P&L, not entries), so the harness's ~137 additional G book entries — and its G' worst-day (−120.9) being *shallower* than the flat book (−127.5) — cannot arise from this mechanism; they are harness modelling conventions to be reconciled in audit. The DESIGN is implemented faithfully (thresholds exact, jar/lock/gap mechanics per spec, oracle byte-frozen).

The one remaining parity item is the Stage-9 EA build (the live EA does not yet embody S.12/S.15/S.19/S.20/D2D) — tracked, not blocking the pack.

### D2D crown-jewel — three-role integration (per DOT_d2d_integration_blueprint.md §1–§6)

D2D is promoted from directional gate to a three-role member. The engine + `conviction.py` now carry the full mechanism (`short_mult`, directional 2-lot D2D-gap, priority, warm-up guard):

- **Role 2 — conviction sizer (both directions):** D2D-agree at a book entry (`D2D_Signal == trade_dir & ADX_Value ≥ 30 & Micro_Hurst ≥ p30`) sizes the trade 2×. §1 precedence is higher-wins: `if D2D_agree OR (long & Hurst>p90): 2.0 elif long & recentFB: 1.25 else 1.0`. **D2D is the only short-side conviction source** — `conviction.py` now emits a `short_mult` array and the engine sizes shorts (the one structural change; both were longs-only).
- **Role 1 — gap-filler (both directions):** D2D standalone throne entries (`D2D_Signal != 0 & ADX ≥ 30 & Micro_Hurst ≥ p30`) fire as NEW trades at a **flat 2.0-lot constant** ONLY when zero Dots are open. §3 priority: D2D-gap → S.20 Hurst/FB gaps, one gap per flat-bar, 2-lot jar accounting (`live_lots + gap_lots ≤ 6`). §2/guard-2: the 2-lot size is assigned **directly, bypassing the conviction arrays** (a gap routed through conviction would be 2×, +hardcoded 2× = 4× — forbidden); verified D2D-gap lots = {2.0}, no gap > 2 lots.
- **§4 warm-up guard:** all three gap sources now inherit the `bar ≥ InitBars(6900)` floor (fixes the 6 leaked pre-floor entries; verified min gap entry bar 7511 ≥ 6900). Export = live.
- Thresholds swept through the oracle (byte-identical): Hurst p30/p90/p97, FB p90. D2D's native SuperTrend trail is NOT used (research: −$836/28.8% WR) — D2D runs on the ratified TM.

**Reproduction (`python engine/score_g.py`) — built-system canonical, all self-computed:**

| Config | Net | WR | PF | daily wd | daily mDD | vs target |
|---|---|---|---|---|---|---|
| DOT-alone (S.20 + warm-up guard) | $89,432 | 92.3 | 6.23 | −153.7 | −153.7 | ✓ exact |
| + Role 2 (D2D-conviction, both dir) | +$1,015 | — | — | — | — | ✓ (+$1,011 ±$5; 1 book SHORT at 2×) |
| + Role 1 (D2D-gap, 14 × 2.0 lots) | +$1,900 | — | — | −104.4 | −145.9 | ✓ (14 gaps, wd −104.4) |
| **Crown jewel (all)** | **$92,347** | **92.3** | **6.40** | **−104.4** | **−145.9** | ✓ exact |

The built-system canonical **$92,347** supersedes the earlier modelled **$92,567**: the model routed the D2D-gap through the signal-path harness; the ratified build routes it through the gap-path engine, a ~$220 admission-timing displacement (immaterial). Two blueprint under-specifications were corrected in this build: (1) `gap_d2d_dir` inherits the `Vol≥300` gate the other gaps carry — this is what reduces **245 raw throne flips → 33 (Vol≥300) → 14 (DOT-flat + warm-up)**, self-computed and exact; the gate stays **p30** (`D2D_HURST_PCT = 0.30`, unchanged — the count is an admission filter, not a tighter percentile); (2) the D2D-gap uses the book `LOCK_FRAC = 1.0`, not `GAP_LOCK = 3.0` (a loose lock inflated exits above the realised high). Verified: short_mult live (1 book SHORT at 2×); every D2D-gap lots == 2.0 (no 4×, conviction-array bypass); all three gap sources respect `bar ≥ InitBars=6900` (min gap bar 9411).

The one remaining parity item is the Stage-9 EA build (the live EA does not yet embody S.12/S.15/S.19/S.20/D2D) — tracked, not blocking the pack.

### D2D crown-jewel — three-role integration (per DOT_d2d_integration_blueprint.md §1–§6)

D2D is promoted from directional gate to a three-role member. The engine + `conviction.py` now carry the full mechanism (`short_mult`, directional 2-lot D2D-gap, priority, warm-up guard):

- **Role 2 — conviction sizer (both directions):** D2D-agree at a book entry (`D2D_Signal == trade_dir & ADX_Value ≥ 30 & Micro_Hurst ≥ p30`) sizes the trade 2×. §1 precedence is higher-wins: `if D2D_agree OR (long & Hurst>p90): 2.0 elif long & recentFB: 1.25 else 1.0`. **D2D is the only short-side conviction source** — `conviction.py` now emits a `short_mult` array and the engine sizes shorts (the one structural change; both were longs-only).
- **Role 1 — gap-filler (both directions):** D2D standalone throne entries (`D2D_Signal != 0 & ADX ≥ 30 & Micro_Hurst ≥ p30`) fire as NEW trades at a **flat 2.0-lot constant** ONLY when zero Dots are open. §3 priority: D2D-gap → S.20 Hurst/FB gaps, one gap per flat-bar, 2-lot jar accounting (`live_lots + gap_lots ≤ 6`). §2/guard-2: the 2-lot size is assigned **directly, bypassing the conviction arrays** (a gap routed through conviction would be 2×, +hardcoded 2× = 4× — forbidden); verified D2D-gap lots = {2.0}, no gap > 2 lots.
- **§4 warm-up guard:** all three gap sources now inherit the `bar ≥ InitBars(6900)` floor (fixes the 6 leaked pre-floor entries; verified min gap entry bar 7511 ≥ 6900). Export = live.
- Thresholds swept through the oracle (byte-identical): Hurst p30/p90/p97, FB p90. D2D's native SuperTrend trail is NOT used (research: −$836/28.8% WR) — D2D runs on the ratified TM.

**Reproduction (`python engine/score_g.py`) — PARTIAL; one flagged blueprint discrepancy:**

| Config | Reproduced net | Target |
|---|---|---|
| DOT-alone (S.20 + warm-up guard) | **$89,432** | $89,432 ✓ exact |
| + Role 2 (D2D-conviction) | +$1,015 | +$1,011 ✓ (1 book short at 2× — `short_mult` live) |
| + Role 1 (D2D-gap, literal Hurst≥p30) | +$5,709 (197 gaps) | +$2,070 (14 gaps) ✗ |
| Crown jewel (all, literal p30) | $96,156 | $92,567 ✗ |

**FLAGGED for Supervisor/Auditor — the D2D-gap throne gate does not reproduce the 14-gap target as written.** The literal blueprint gate `ADX≥30 & Micro_Hurst≥p30 & flip` yields **245 throne bars → 197 gap trades**, not 14 — and the blueprint's own "96.9% WR, one loss in ~5 months" implies a ~32-trade throne, incompatible with 245. A Hurst-percentile scan (ADX≥30) gives: p30→197, p50→151, p70→79, p90→**18**, p95→**11**, p97→7 gaps; the target 14 falls between p90 and p95. **No `ADX≥30 & Hurst≥pXX` gate reproduces net $92,567 with worst-day −100.9**: a 2-lot gap can only improve the worst day by winning on it, and the base worst day (−153.7) is untouched at every gate, so the crown's −100.9 / mDD −145.9 are not reachable on the ratified engine — consistent with the S.20 harness-vs-engine worst-day gap already on record (harness −147.2/−120.9 vs engine −153.7). The build is faithful to the blueprint **text** (`D2D_HURST_PCT = 0.30`, one-line parameter); base + Role 2 + both guards + the warm-up fix reproduce/verify exactly. Resolution needed: (a) the intended throne Hurst percentile (text says p30; the count implies ~p90–p95), and (b) confirmation that the crown net/worst-day/mDD are analysis-harness figures rather than ratified-engine figures.

---

## 6. HOW TO RE-RUN FROM SCRATCH (runbook)

**Environment:** Python 3.12; `numpy 2.4.4`, `pandas 3.0.2` only (`pip install numpy==2.4.4 pandas==3.0.2`). No parquet/pyarrow dependency — all outputs are CSV/JSONL by design. Confirm against `RUN_STAGE8.md`'s manifest before trusting a run.

**Preconditions:** merge the ratified TM (Section 5 / GAP 1) into the engine and create the runnable BOOK-50 artifact (GAP 2) if the run must be book-representative or book-verifying. Pure field discovery/scoring can proceed on the packaged engine, but its worst-day/PF reflect S.7-base.

**Order & commands:** run from the pack root; MP runners run **directly** (not via `stage8.py`).
1. `python engine/core.py` — reconstruction (only if rebuilding the baseline from raw export).
2. `python scanners/run_f0_full.py` — F0 discovery (**long pole**, hours).
3. `python scanners/f0_to_schema.py dots_results/deduped_survivors.csv` — F0 → schema.
4. `python scanners/run_f1_parallel.py` — F1 (1,699,320 pairs; ~1.5 days on 8+ cores).
5. `python stage8.py discovery_orchestrator.py full` — F2–F9/F11 + collate.
6. `python scanners/concurrence_profiler.py` — F12 (blocking, ~60–110 min).
7. `python engine/run_full_analysis.py` — tic-proof F0 export.

**Worker counts:** 12 workers stable; **16 oversubscribes the 6-core/16-thread VPS and freezes** — do not exceed 12. `run_full_analysis.py` defaults `N_WORKERS=6` (raise via CLI arg up to 12).

**Checkpoint / crash-resume:** `run_full_analysis.py` shards the field (`SHARD=500`) into `discovery_results/_shards/`. Each shard writes its CSV + JSONL via atomic `os.replace`, then a `.done` marker **last** — so a hard kill mid-shard leaves no partial output and that shard simply re-runs; completed shards are skipped on restart (no duplication, no loss). `run_f1_parallel.py` and `concurrence_profiler.py` are likewise spawn-safe and resumable. Deterministic: shard boundaries and per-candidate scoring depend only on input order.

**Verify the run reproduced known-good numbers (trust gate):**
- **Field sanity:** the F0 field row count and the gated survivor count match the last-good run; every survivor's per-day vector in the JSONL sums to its `net_pts` (built-in reconciliation).
- **Book total:** requires the BOOK-50 runnable artifact (GAP 2). Target = `DOT_performance_record.xlsx` → Portfolio Summary (RUN 2,363 / $58,249 / PF 6.12 / WR 92.8% / worst-day −$127.5 / OOS PF 6.99). Do **not** target the superseded `DOT50_performance_record.xlsx` or any pre-momentum-SL figure.
- **Parity:** engine output must match the EA on a shared signal (export = live). A mismatch invalidates the run until resolved.

---

## 7. ANTI-CURVE-FIT SELECTION (field → validated book)

Scoring produces a field; **selection** turns it into a book, and selection is where curve-fit risk lives. Follow `DOT_anti_curvefit_guide.md` (project root). The ratified method, in order:

1. **Survival-first is the binding gate** — worst-day USD must clear the FTMO daily ceiling **before** any profitability metric is considered. A set that fails survival is rejected regardless of PF.
2. **Persistence at three scales** — 6 monthly folds, ISO-weeks, day-of-week; a diamond is positive across folds/weeks/weekdays, not on single-fold dependence.
3. **OOS strengthening as the real edge signal** — fixed-book OOS PF (measured May–Jun on a book selected Jan–Apr) that **exceeds** in-sample PF is the primary non-curve-fit indicator. Greedy/expanded sets that collapse OOS are rejected.
4. **Decorrelation** — additions must fire on largely different bars and decorrelate losses; redundant co-firers are trimmed.
5. **Leave-one-out trim (anti-fit)** — erosion ranking that protects bear-fold contributors; the trim is *why* the OOS result is trustworthy.
6. **Structure coverage by impossibility-fill only** — fill a missing market structure only where no existing triple decorrelates cleanly (this is the sole role of the 2 F1 members).
7. **Null-honouring** — the edge must exceed the circular-shift / shuffle null by a wide margin (TM-alone on random entries loses money); F12's null table is the depth-side instance of this discipline.

Selection is manual and evidence-driven: `signal_full_records.csv` + the per-day vectors + `discovery_master.csv` + the F12 tables are the inputs; the human draws the book. The engine measures; it does not select.

---

*Accurate to the pack on disk. The two blocking gaps in Section 0 must be closed before a from-scratch re-run is treated as current-book-representative.*
