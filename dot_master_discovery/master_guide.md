# master.py — Operator Guide

## Document index — which doc to open

| Document | What it's for |
|---|---|
| **master_guide.md** (this file) | **How to run it** — the single entry point (`master.py`), every stage S0–S9, every output, `--book` vs no-book, resume, auto-split. Start here. |
| **discovery_map.md** | The script/file **inventory** — every file, sha256, canonical-vs-retired, the carry/drop manifest, project-vs-local reconciliation. |
| **master_stage_spec.md** | The **build contract** — the §1–§10 design spec `master.py` was built from (historical reference). |

*Retired (do not use as current): `RUN_STAGE8.md`, `DOT_stage8_program_map.md`, `stage8.py` — the pre-master multi-script pack workflow, superseded by `master.py` + this guide.*

One command runs the whole DOT pipeline: ingest a market export → compute thresholds → discover patterns → filter → rank the mechanism choices → score the committed system → write one report. `master.py` orchestrates the ratified scripts; it never rewrites them.

## The one command

```
python master.py --book book50_signals.csv      # REPLAY + VERIFY a ratified book
python master.py                                 # DISCOVER a fresh book from the data
```

- **With `--book`** the master replays the supplied frozen ratified book and scores it. This is the verification path. On the sealed US30 baseline it prints `net $92,347 / 2,698 tr → REPRODUCED`.
- **Without `--book`** the master runs full discovery and assembles a *fresh* book from the discovered candidates by the survival-first rule, then scores that. It is flagged as a new discovered book.

The only branch in the whole program is "was a book supplied?" — never "is this US30?". Stages S0–S7 are pure geometry (percentile-relative distributional patterns) and know nothing about the asset.

## Options

| flag | default | meaning |
|---|---|---|
| `--data DIR` | `/data` (falls back to the pack `data/`) | where the EA CSV export(s) live |
| `--out DIR` | `discovery/` | where all outputs land |
| `--book FILE` | *(none)* | supply a frozen ratified book to replay+verify; omit to discover fresh |
| `--workers N` | 12 (capped) | parallel worker bound |
| `--stage S0..S9` | *(all)* | run/resume a single stage |
| `--market-label STR` | `US30 (sealed baseline)` | report tag only — never changes logic |
| `--chunk-mb N` | 9 | auto-split target size |

## What each stage does and where output lands

Everything lands under `--out` (default `discovery/`):

```
discovery/
  raw/          per-family survivor scans (discover-fresh path)
  results/      discovery_master.csv (unified 14-col) + candidates.csv (gate survivors)
  scored/       fresh signal_full_records + signal_per_day_pnl (regenerated, never inherited)
  contenders/   contenders.csv — the mechanism head-to-head
  committed/    committed_score.txt (+ discovered_book.csv on the no-book path)
  master_report.md
  .markers/     one .done per completed stage/family (checkpoint/resume)
```

- **S0 Ingest & validate.** Reads every `*.csv` in `--data`, detects header vs headerless continuation parts, concatenates in filename order, and asserts the contract: `Time` + 171 features, time strictly increasing, zero duplicate rows, zero NaN. A raw 256-col export is routed through `core.py` first (S0a); a clean 171-col EA drop ingests directly. Any violation aborts loudly and says why. The attestation (rows × cols, range, invariants) goes into the report.
- **S1 Adaptive thresholds.** The oracle (`dots_thresholds.py`) computes per-bar hi/lo percentiles and structural gates. Its sha256 is printed every run — that is the export=live parity check.
- **S2 Pool & anchors.** Builds the scannable condition vocabulary, the `ST_Flip` anchor, and the warm-up floor.
- **S3 Family discovery** *(discover-fresh path only; the long pole)*. Delegates to the ratified `discovery_orchestrator` to run all 13 families (F0 committed, F1–F13). Results land in `results/`. Crash-resumable per family.
- **S4 Schema unify.** Collates the family results into `results/discovery_master.csv` (common 14-column schema).
- **S5 Candidate filter.** Keeps rows passing the auditor-validated gate `trades ≥ 30 AND folds_plus ≥ 4 AND agg_pf ≥ 2.0`, carrying worst-day for survival-first ranking → `results/candidates.csv`.
- **S6 Full-field scoring (regen).** Regenerates `signal_full_records.csv` + `signal_per_day_pnl.jsonl` *fresh* under the current engine. The stale pre-S.19/S.20/D2D copies are never inherited.
- **S7 Contender head-to-head.** Scores C0 (flat) → C4 (full) plus a sizing variant through the ratified engine, each with net / WR / PF / daily worst-day / daily mDD / 6-fold min-PF / OOS, and the delta each mechanism buys. This validates every committed choice on the data itself → `contenders/contenders.csv`.
- **S8 Committed-system score.** With `--book`: loads that book, scores it, prints the `REPRODUCED / MISMATCH` self-check (does not re-select). Without `--book`: assembles a fresh book from `candidates.csv` (survival-first rank → overlap-dedup → cap) and scores it. → `committed/committed_score.txt`.
- **S9 Report & split.** Writes `master_report.md` and auto-splits every oversized output.

## How to read the report

`master_report.md` has: (1) ingest attestation, (2) the sacred sha registry, (3) the C0→C4 contender build-up with each delta, (4) the committed headline (and the BOOK-50 REPRODUCED verdict on the baseline), (5) per-family coverage, (6) the stale-artifact regeneration note. The headline line is the bottom line; the contender table shows exactly how each mechanism earned its place.

## Dropping in a new market

1. Put the new market's EA export (one big CSV or N chunks, any filename) in `--data`. It must carry the same 171-feature schema.
2. Run `python master.py --market-label BTC` (no `--book`) to discover and score a fresh book for that market, or `--book <ratified_book.csv>` to replay a ratified book against it.
3. The oracle self-calibrates its percentiles to the new data — nothing in the code is US30-specific.

> **Known-and-accepted:** the no-book / new-market book-selection rule (S8 discover-fresh branch) is *designed but not yet data-validated* — its first live use is the first fresh-discovery run on a real non-baseline export. The `--book` frozen-book path is fully ratified and simply replays the supplied book.

## Resuming after a crash

Every stage writes a `.done` marker recording the input sha. Re-running the same command skips any stage whose `.done` exists and whose inputs are unchanged, and resumes the rest. A 1–2 day discovery run survives interruption: `python master.py` again (or `--stage S3`) picks up where it stopped. If the input data changes, markers invalidate automatically and the affected stages re-run.

## Sacred files (never edited by the master)

`dots_thresholds.py`, `wf.py`, `core.py`, `portfolio_simulation_engine.py`, `conviction.py` are byte-locked. The master prints all five shas at startup and aborts on any drift. No stage computes a threshold except through the oracle; no stage opens a trade except through the ratified engine.
