# CAKE DICTIONARY

**Read this before touching any file in this project.**

Every AI instance that joins this work has to eat the same cake. This document defines what
each of the 89 files is, what period it covers, what it is for, and — just as important —
what it must not be read as. It also fixes the perspective: what "good" means here, what the
incumbent is, and why persistence outranks profit.

---

# 0. FIRST: GROUND YOURSELF IN THE INCUMBENT

Do this before forming any opinion about the new data.

    git clone https://github.com/datlooms/DOT.git
    cd DOT/dot_master_discovery
    mkdir data
    # place the ten DOT_jan19_jul21_*.csv parts from project files into data\
    python master.py --data data --workers 14 --out discovery\full --book <committed book>

You do not need to run the full discovery — that is what produced these artifacts. What you
need is to see the committed system score, so you know what any new selection has to beat.

## THE INCUMBENT: BOOK-50, and the six contenders around it

From `contenders.csv`, on the same frame these artifacts come from:

    id                                        trades    net    WR    PF   worst-day  OOS-PF  OOS-net
    C0  flat book, 1-lot, no conviction/gaps    2,733  60,130  90.8  4.31    -617.0    4.28   17,709
    C1  + S.20 conviction (Hurst/recentFB)      2,733  75,222  90.8  4.73    -639.1    4.92   23,379
    C2  + S.20 gap-singles (Hurst-gap, FB-gap)  3,088  94,759  90.6  4.75    -565.3    4.82   28,387
    C3  + S.21 D2D-conviction (2x both dir)     3,088  96,092  90.6  4.79    -565.3    4.87   28,812
    C4  + S.21 D2D-gap = FULL                   3,101  97,675  90.6  4.81    -565.3    4.91   29,116
    C5  sizing variant (conviction off)         3,101  81,263  90.6  4.44    -473.2    4.35   23,020

**C4 FULL is the committed system: 3,101 trades, PF 4.81, $97,675, worst day −$565.**

That is the number to beat, and it is not the only one. **The book must also beat it on
worst-day, on out-of-sample persistence, and on terrain coverage — not just on net.**

## WHY THIS REBUILD HAPPENED — AND THE FRAMING HAS BEEN CORRECTED

BOOK-50 scored **PF 6.40 in-sample and PF 2.19 on first contact with genuinely unseen data.**

**The commonly repeated version of why — "the selection process was never validated" — is NOT
accurate, and the accurate version is more useful.**

From the record: **BOOK-48 WAS selected on Jan–April and measured on held-out May–June. It came
back at OOS PF 6.65 — the highest of any book in the project.** The discipline was applied.

**What killed it is that May–June sat inside the sealed baseline.** July was the first
genuinely unseen data, and that is where 6.40 → 2.19 happened.

> **The book had a real hold-out. The hold-out was too close.**

That is a different and harder problem than "nobody checked". It means a hold-out drawn from
within the same construction window can return an excellent number and still tell you nothing.
**It is why the walk-forward in this rebuild tests the INCLUSION RULE across anchored splits
rather than testing a book against a slice — and that genuinely is new.**

Everything in this project exists to stop that recurring. **Validate the method, not the
output — and be suspicious of any hold-out that shares a window with the build.**

## WHAT BOOK-50 ACTUALLY REACHES

    reachable terrain      1,143 UP  /  1,155 DOWN   = 2,298 episodes
    BOOK-50 touches           54     /     28        = 82 episodes
    as a share of reachable  4.72%   /   2.42%
    unclaimed                              2,092 episodes

**BOOK-50 participates in under 5% of the terrain available to it.** The "30%" figure that
appears in older notes is the *reachable* share of all terrain (30.0% UP / 31.4% DOWN of
7,490 episodes) — that is the size of the plate, not the size of the slice taken.

---

# 1. THE PERSPECTIVE — READ `DOT_signal_discovery_mantra.md`

It is in the repo. Read it. The parts that govern every decision made with these files:

**"WHOLE CAKE?" is a standing check.** After any stage, verify the counts reconcile across
all four links — scanned / exported / in-pool / REACH. A number that appears without its
denominator is not a measurement.

**Survival before profitability, always.** The FTMO daily-loss ceiling is the *first* filter,
not a caveat at the end. A $100K swing account: **$2,500 daily hard stop, $5,000 FTMO daily
limit, $10,000 total drawdown.** A book that makes more but breaches once is worthless.

**Persistence outranks raw P&L.** In-sample PF rank is a *flawed* selection criterion —
high-PF signals are systematically low-frequency, and greedy sets selected purely on training
data collapse out-of-sample while conservative sets hold. `folds_plus`, `min_fold_pf` and the
out-of-sample columns matter more than `agg_pf`.

**We are not losing money. We are building a book for the future.** There is no live capital
at risk in this phase. The only failure mode that costs anything is *shipping a book that
looks good and isn't* — which is exactly what happened last time. Optimise for a book that
will still work in six months, not one that maximises a backtest.

**Include and let the evidence sort it.** No upfront pruning, no pre-set directional targets,
no quotas, no "sensible defaults" that reduce what gets emitted. The catalogue holds
everything valid; the operator composes.

**Nothing in the pipeline chooses.** Item 15: the catalogue is emitted from VALID, never from
an argmax. No `selected_book.csv` is written anywhere. **That constraint applies to AI
instances too** — you measure, the operator selects.

---

# 2. THE DATA

## 2.1 The frame — `DOT_jan19_jul21_1.csv` … `_10.csv` (10 files, ~223 MB)

**The only raw input.** Everything else in this project is derived from these.

    177,251 rows x 172 columns
    2026.01.19 15:49  ->  2026.07.21 17:09      US30.cash, 1-minute
    input_sha 46586cbb1671

Column 1 is `Time`; the other 171 are the EA's exported variables — OHLCV, D2D state, OBV,
KAMA, HarmVol, ADX, ATR, PoC, VWAP, opening range, session levels, adaptive-trend slopes, and
28 `Micro_*` microstructure variables.

**Load them in numeric part order** (1,2,3…10, not lexical). Part 1 carries the header; parts
2–10 do not.

**CLOCK:** these are the **corrected TRUE-EST** files. An earlier export fed server time into
a GMT parameter and ran `EST_Hour`/`EST_Minute`/`EST_DayOfWeek` 2–3 hours fast. All figures in
this project use the corrected frame. If you ever see a nine-part `DOT_stitched172_*` set,
that is the **pre-fix** data and must not be used.

**PERIOD CAVEAT THAT AFFECTS EVERY FOLD COLUMN:** `wf.FOLDS` is a **calendar literal,
Jan–Jun**. The frame runs to 21 July. **July contributes nothing to `folds_plus` or
`min_fold_pf` on any row.** `discovery_master_POOL_NOTE.txt` states this at the point of use.
It cannot be fixed without editing `wf.py`, which is sacred and byte-locked. So a signal that
only works in July looks fold-invisible, and `folds_plus >= 4` was decided on six months, not
seven.

## 2.2 Provenance

Every artifact below comes from **one completed cold run**: 10h43m from an empty tree, every
stage executing, no resumes, no manual intervention, 99.7% concurrent. It reproduced a
previous independent cold run byte-for-byte on ten of ten comparable artifacts, including
`wf_pass_criterion`. A per-family seed defect that made the pricing column vary between runs
was found and fixed **before** this run — the pricing is now reproducible.

---

# 3. THE ARTIFACTS

## 3.1 THE CATALOGUES — the deliverable

**`catalogue_F0.csv`** (1,840 rows, 1,818 VALID) — **the core file.** Same-bar
triple-convergence signals gated by D2D. This is where the operator's book comes from.

**`catalogue_F1.csv`** (37,276) — sequential temporal: condition A, then condition B k bars
later, anchored on ST_Flip.
**`catalogue_F3.csv`** (53) — conditional interaction, A gated by a state.
**`catalogue_F9.csv`** (133) — session/weekday-scoped.
**`catalogue_F2.csv`** (1), **`catalogue_F4.csv`** (3), **`catalogue_F11.csv`** (2) — state
transition, divergence/non-confirmation, rolling lead-lag.

**61 columns. What they mean:**

*Identity:* `signal_id` (`family|signal_def|direction`), `family`, `signal_def`, `direction`.

*Verdict:* `verdict` ∈ {VALID, UNEVALUABLE}, `reason_code`. **UNEVALUABLE rows are retained
deliberately** — they are not failures, they are rows that could not be measured
(insufficient trades, undefined PF, too few active days, too few regime buckets). Do not drop
them silently; count them.

*Performance, ungated:* `trades`, `WR`, `agg_pf`, `net`, `worst_day_usd`, `active_days`.

*Performance, gated:* `gated_trades`, `gated_WR`, `gated_PF`, `gated_worst_day_usd`,
`gated_net`, `gated_delta_net`. **Both arms are present so gating is decided per signal, not
book-wide.**

*Persistence:* `folds_plus`, `min_fold_pf`, `fold_buckets`, `regime_total_buckets`,
`regime_positive_buckets`.

*Terrain:* `touched_episode_ids` (semicolon-separated), `episodes_touched`,
`coverage_pct_raw_terrain`, `coverage_pct_reachable`, `terrain_cell`.

*Pricing — the most important block:* `n_trials_family`, `null_valid_rate_family`,
`expected_valid_by_chance_family`, `pf_null_p50/p90/p99_family`, `pf_null_exceedance_pct`,
**`EXPECTED_ROWS_AT_OR_ABOVE_THIS_PF`**, `q_value_BY_family`, `n_null_family`,
`null_matched_fraction`, `null_rejected_out_of_band`, `null_direction_long_share`,
`null_seed`.

> **`EXPECTED_ROWS_AT_OR_ABOVE_THIS_PF` is the single most important column in this project.**
> It answers: *how many rows this good would chance alone produce, given how many candidates
> were searched?* A row with a high PF sitting beside a large expected-count is noise. A row
> priced **below 1** is one chance essentially does not produce. **Never rank on `agg_pf`
> without consulting this.**
>
> Read `null_matched_fraction` per family before trusting the price. F0 drew K=500; the other
> families K=200. Poor matching means weaker pricing for every row in that family.

*Balance axes (17 columns):* `market_structure`, `market_structure_secondary`;
`session_<name>_pct` × 8 plus `session_modal`; `regime_causal_0_pct`, `regime_causal_1_pct`,
`regime_burnin_pct`, `regime_modal`.

## 3.2 THE MAP — terrain

**`terrain_summary.csv`** (16) — episode counts per grid cell. **The pinned cell is
`W=15 K=p85 E=p75`: 7,490 episodes, 3,816 UP / 3,674 DOWN.** Every coverage figure in this
project denominates on it.

**`terrain_hour_profile.csv`** (104) — episodes by hour and session. NY OPEN holds 225 of the
largest-decile episodes; overnight holds 0. **This is the cost of session balance, in one
file.**

**`terrain_episodes.csv`** (24,247, 8 MB) — every episode across all four grid cells, with
start/end bar, duration, displacement, hour, session.

> **Terrain is a property of PRICE ONLY.** No signals, no book. It is the denominator, and it
> cannot be moved by anything you select.

**`unclaimed_reachable.csv`** (2,092) — reachable episodes no VALID signal touches.
1,037 UP / 1,055 DOWN.

> **Two columns, one useful and one not.** `n_prefilter_candidates_touching` is the
> diagnostic: **min 76, median 1,810, max 73,357, and zero episodes have zero.** Every
> unclaimed episode was reached by the search — the vocabulary is not the limit. What
> excluded them was S5's filter.
>
> **`n_valid_triples_touching` is TAUTOLOGICAL and carries no information.** An episode
> appears in this file precisely because no VALID signal touches it, so the count can only
> ever be 0. **Do not read it as a finding.**

**`reach_D0_missed_decomposition.csv`** (12) — why an episode is out of reach at all: not
eligible, D2D disagreed, or reachable-but-untouched. The first two are excluded by design.
**`reach_D01_directional_baseline.csv`** (26), **`reach_D02_D2_coverage.csv`** (67),
**`reach_D02_book_depth_structure.csv`** (8) — directional baselines and stratified coverage.

## 3.3 THE CONVERGENCE EVIDENCE

**`same_bar_cohort.csv`** (27) — bars, distinct signal slots and family composition per
(direction, depth). **Counts only, by design** — P&L requires a book, and the design forbids
one.

**`cohort_scored.csv`** (101) — the scored version: `direction, depth, family_composition,
purity, bars, trades, WR, PF, net, avg_trade, worst_day_usd, sufficient`. ALL-ONE-FAMILY and
MIXED separated. Cohorts with `sufficient=False` are **retained with their counts** — report
them, do not drop them.

**`cross_family_cofiring.csv`** (14) — which families land together.

**`dilution_curve_agg_pf.csv`** and
**`dilution_curve_EXPECTED_ROWS_AT_OR_ABOVE_THIS_PF.csv`** (39,260 steps each) — signals
admitted best-first under two ranking keys, with `same_bar_ge3_bars` recomputed at every step.

> **This is the file that answers "how many signals before depth 3+ stops being special".**
> 39,260 not 39,308 because the 48 UNEVALUABLE rows cannot enter an admission curve.
> **The gap between the two curves is the overfit estimate** — the difference between ranking
> on raw PF and ranking on chance-adjusted price is what PF-ranking buys that is not real.

**`concurrence_depth_bars.csv`** (170,351) — per bar: `depth_long`, `depth_short`,
`depth_d2d_aligned`, `d2d_dir`, `regime`, `regime_causal`, plus context variables. The raw
convergence distribution.

**`concurrence_events.csv`** (104,643) — concurrence events with onset/peak depth, duration,
build and decay rates.
**`concurrence_outcome_map.csv`** / **`_secondary.csv`** (3,216 each) — outcome by
(peak_depth_k × duration × direction × d2d_mode).
**`concurrence_entry_order.csv`** (4,618) — which conditions join a cluster early vs late.
**`concurrence_d2d_flips.csv`** (22,901) — depth behaviour around D2D flips.
**`concurrence_category_depth.csv`** (3,972), **`concurrence_composition_part_1..3.csv`**
(813,824 rows total, split for upload) — depth by condition category and cluster composition.
**`concurrence_null_baseline.csv`** (20) — circular-shift permutation null for the
concurrence measurements.
**`concurrence_regimes.csv`** (572) — regime cluster summaries. **Summaries only — it does not
say which bar is in which regime.**

**`regime_labels.csv`** (170,351) — **per-bar regime labels.** `bar_index` (6900–177250),
`time`, `lab_causal`, `lab_desc`.

> **Use `lab_causal` ONLY.** It is burn-in fitted and forward-only. `lab_desc` is full-sample
> and must **never** characterise a tradeable signal. `lab_causal == -1` marks 5,636 burn-in
> bars — unlabelled, not a third regime. Join on `bar_index`.

**`cluster_basis_summary.csv`** (12) and **`cluster_participation_profile.csv`** (3,013) —
the three cluster bases and per-condition participation. Read the cohort tables against the
right basis.

## 3.4 THE WALK-FORWARD — the method's own test

**This is what the whole rebuild exists to produce.**

**`wf_splits.csv`** (3) — three anchored splits with a 1,440-bar embargo. The split count is
**derived from an executability floor** (≥60 days, ≥3 monthly buckets), not fixed.

**`wf_pass_criterion.csv`** — the verdict.

    mean_ratio 2.1653   target >= 2.40
    min_ratio  1.5083   target >= 1.85
    mean_ratio_lb95 1.4026  target > 1.00     <- PASSES
    verdict: FAIL

    split 0: 15,328 admitted ->  7,962/11,816 -> 0.6738  vs null 0.2360 = 2.86x
    split 1: 23,832 admitted ->  9,712/19,176 -> 0.5065  vs null 0.2375 = 2.13x
    split 2: 34,496 admitted -> 11,303/28,901 -> 0.3911  vs null 0.2593 = 1.51x

> **READ THIS CORRECTLY.** The inclusion rule **beats chance on every split** — 2.86×, 2.13×,
> 1.51×, and the 95% lower bound clears 1.0. What fails is the *strength* bar set in advance.
> **No threshold was lowered to obtain a pass.** A FAIL you can trust is the entire point;
> last time the project got a PF 6.40 it could not.
>
> The ratio decays as VALID admits more. **Two explanations with opposite remedies:**
> dilution (later signals weaker → tighten VALID) or regime (the third window is harder →
> nothing to fix). `wf_book_arm_entities.csv` is what distinguishes them.

**`wf_book_arm_entities.csv`** (73,656) — one row per (split, signal): `admitted`,
`traded_on_test`, `persisted`, plus full train/test stats. **This file exists solely to answer
the dilution-vs-regime question.**

**`wf_null_arm_summary.csv`** (3 + header) and **`wf_null_arm_entities.csv`** (250) — the
random-triple null arm. **A MEASUREMENT, NOT A PASS CRITERION.**

**`wf_per_segment_rederivation.csv`** (3) — proof every threshold was recomputed inside its own
training segment. **`wf_oracle_causality.csv`** (10) — the anti-leak assertion.
**`wf_rejection_checks.csv`** (13) — the rejection list as executable checks.
**`wf_split_derivation_attempts.csv`** (3) — the floor being applied, not just the answer.
**`_wf_attest.jsonl`** — append-only attestation: input sha, code sha, split sha, verdict, per
run. One of only two artifacts permitted to carry wall-clock.

## 3.5 THE POOL LINEAGE — how 463,996 became 39,308

**`discovery_master_part_1..3.csv`** (463,996 rows, split for upload) — **every candidate the
scan produced**, before any quality filter. 14 columns: family, script, signal_def, direction,
d2d_mode, trades, WR, agg_pf, worst_day_usd, hard_stop_days, folds_plus, min_fold_pf,
spread_pf, survival.

> **Reassemble in part order.** Part 1 carries the header. This is the file that proves the
> unclaimed terrain is a quality gap: the candidates that would have reached it are in here,
> and S5 removed them.

**`candidates.csv`** (39,308) — the survivors of S5's filter:
`trades>=30 & folds_plus>=4 & agg_pf>=2.0`. **This one filter cut 463,996 to 39,308 and is
the single most consequential threshold in the pipeline.** Note it contains one duplicate
`signal_id` (an F4 row appearing twice with 113 and 119 trades) — 39,308 rows, 39,307 distinct
keys.

**`results_F0_*`, `results_F1_part_1..2`, `results_F2_*` … `results_F13_*`** — the raw
per-family scan output, before collation. Provided for tracing a catalogue row back to its
scan.

> **`results_F13_single_variable_extremes.csv`** (5,160) is different in kind — every one of
> the 117 variables scanned **alone**, no convergence. **5,160 surfaced, 0 reached candidate
> tier, 0 STARS.** The tier bar is ≥95% WR standalone with full-span persistence. Read it
> narrowly: *no single variable clears a 95%-WR standalone bar.* It is **not** evidence that
> single variables cannot contribute — two of them (`Micro_Hurst`, `Micro_FailedBreak`) are
> live gap-fillers in the committed system, doing a conditional job this scan does not test.

**`discovery_master_POOL_NOTE.txt`** — the `wf.FOLDS` calendar-literal caveat. Read it.

**`family_evidence.csv`** (14 families) — per family: scanner name and sha, rows emitted,
candidates passing S5, D2D gate measurements, and a SELECTABLE / INSUFFICIENT-EVIDENCE verdict.
**`grammar_coverage.csv`** (14) — every distinct `signal_def` grammar shape in the filtered
pool, and whether `build_book` can parse it.

## 3.6 THE SELECTION-LAYER MEASUREMENTS

> **IMPORTANT CONTEXT:** these describe the **incumbent BOOK-50** and constraint references —
> not the new catalogue. The greedy search in this run **selected zero signals**, which is
> correct and expected: the objective counts runs of ≥5 distinct signals, so it is identically
> zero for any set smaller than 5, and greedy adds at most 2 at a time. **Nothing downstream
> depends on it.** `score_book.py` recomputes all of these for any composed book.

**`selection_depthyield_grid.csv`** (40) — DepthYield across the tolerance × S grid. **Useful
for the new work** — it tells you how depth-yield behaves at different N and S.
**`selection_g2_domain_bridging.csv`** (48) — BOOK-50's triples classified by domain. **A
working reference for the market-structure classifier.**
**`selection_constraints.csv`** (16), **`selection_constraint_evaluation.csv`** (5) — TailDep,
FailConc, mCVaR, absolute survival, and PBO via CSCV.
**`selection_mcvar.csv`** (55), **`selection_cofire.csv`** (8),
**`selection_coverage.csv`** (13), **`selection_h3_persistence.csv`** (6),
**`selection_vocabulary_hygiene.csv`** (5), **`selection_g2_near_duplication.csv`** (5),
**`selection_fixture_exhaustive_vs_greedy.csv`** (14).

## 3.7 THE RUN ITSELF

**`contenders.csv`** (6) — the six configurations above. **The bar to beat.**

**`run_log.txt`** (734 KB) — the complete run, including the stage timing table and
`MASTER COMPLETE`. **An attestation record — one of only two artifacts permitted to carry
wall-clock.** Every CSV is wall-clock-free and byte-identical across runs.

**`master_report.md`** — the S9 narrative summary.

---

# 4. TRAPS — read before interpreting anything

**1. Blended statistics conceal direction.** Long and short are structurally different
populations. Long depth-5+ runs PF ~39; short depth-5+ runs ~3.4. Any figure that averages
them is not a summary, it is a disguise. **Report per direction, always.**

**2. A single absolute threshold is not neutral across directions.** Short `min_fold_pf`
median is 0.52 against long 0.79; short `agg_pf` median 2.89 against long 3.55. One number
applied to both imposes the long side's standard on the short side. **Rank each direction on
its own distribution.**

**3. Rarity and coverage pull against each other.** Rare signals fire rarely — that is what
makes them rare. Ranking purely on price *reduces* terrain coverage. Both are legitimate
objectives and they are not compatible; the trade must be made deliberately.

**4. `AT_Regime_ST` is inverted; `AT_Slope_ST` is not.** In `DOT.cs`, `AnchorType` is encoded
`st_Slope > 0 ? 0 : 1` — **0 means bullish.** The exported `AT_Slope_ST` is the raw regression
slope, positive = bullish, no inversion. **Never use `AT_Regime_ST` for direction.** Its
neutral state (`AT_Slope_ST == 0`) is 0.83% of bars and is a state of the *variable*, not of
the system — the system is hard-coded alternating bias and always has a side.

**5. Measurement circularity.** Reading the market through the book produces artifacts that
look like market properties. BOOK-50's short-side imbalance was a *selection* artifact — the
full-scan pool is 51.3% long / 48.7% short, so the market itself is balanced. **Always check
whether a "market property" is actually a property of the instrument you measured it with.**

**6. The matched null matches rarity, not structure.** By design it does **not** match temporal
structure (F1's lagged pairs, F2's transitions, F6's crossings) or condition composition.
Matching composition would make the null a near-copy of the population and destroy the
independence that gives it meaning. **This is a property of the method, not a defect awaiting
a fix.**

**7. Gates HAVE been priced before — and one prior result independently reproduced.** An
earlier sweep of 117 adaptive variables hunting a short-side conditioner found
**`Micro_Rejection:lo` at the 98.6th random-subset percentile, OOS-positive and
mechanism-backed.** That is a priced gate result; the machinery existed and was used.

It was re-tested on the NEW catalogue's short depth-3+ population and it holds:

    SHORT 3+ ungated                n=1,485  PF 3.60
    Micro_Rejection    lo p50       n=  973  PF 5.00   net 25,709   keeps 89% of net
    Micro_Rejection    lo p30       n=  630  PF 5.53   net 19,176
    Micro_FailedBreak  hi p50       n=  626  PF 7.61   net 13,298   keeps 42% of net

**A gate found on the old pool, priced at the time, still works on a pool built by a rebuilt
pipeline from a different catalogue.** That is out-of-sample confirmation of a gate and it is
the strongest gate evidence this project holds.

**But `Micro_FailedBreak` is NOT in that category.** It was found by testing eleven variables
at three thresholds on two sides — 66 tests, one frame, never priced. Treat it as a hypothesis
until it has the equivalent of a random-subset percentile. The two also trade off differently:
FailedBreak is the sharper filter, Rejection the wider one that retains far more net. Under
survival-first that distinction matters more than the PF headline.

**8. F0's `MIN_PF = 2.0` internal pre-gate.** F0 rows are already a PF-filtered subset before
S5 ever sees them. Coverage below the reachable ceiling has this as one of its named causes.

---

# 4B. WHAT THE PRIOR RESEARCH ALREADY ESTABLISHED

Recovered from `foundational_documents/DOT_rule_master_spec.txt` and the Manager's record.
**These are not new questions. They were answered once, with numbers, and the answers are
still binding.** Do not re-open them without new evidence, and do not re-discover them.

## CONVICTION — why the constants are what they are

    MAX_RISK 150.0   HURST_SIZE_PCT 0.90   HURST_GAP_PCT 0.97   FB_PCT 0.90
    CONV_HURST_MULT 2.0   CONV_RECENTFB_MULT 1.25   RECENTFB_WINDOW 5   GAP_LOCK 3.0

**Hurst sizing is LONGS ONLY, and it was measured.** `Micro_Hurst` confers **no** short edge:
**OOS PF 2.22 on shorts against 4.99 on longs.** Shorts stay at 1.0 lots unless D2D fires.

**D2D is the ONLY short-side conviction source** — verbatim from the spec: *"it fills the gap
where longs had Hurst/recentFB and shorts had nothing."*

**A long that qualifies for both Hurst and recentFB takes the HIGHER multiplier (×2), never
the product.**

**SACRED BUILD-GUARD:** engine and `conviction.py` must size shorts (D2D-agree short → 2×).
Without it D2D's entire short-side conviction is silently dropped — verified live, book ×2
count 210 → 249 with the role on.

**DELIBERATE ASYMMETRY, not a bug:** D2D-gap fires at a flat 2 lots because it clears the
throne (96.9% WR, highest in the book). S.20 gaps stay 1 lot because they clear a lower ~p90
gate. D2D-gap assigns lots **directly**, bypassing the conviction arrays — routing through
them then hardcoding 2× would yield 4× and breach the gate.

**A LIVE DECISION THE OPERATOR SHOULD RE-EXAMINE.** Two configurations were measured:

    G'  Hurst x2 ONLY          net $85,134   worst-day -120.9   max-DD -120.9
    G   Hurst x2 + recentFB    net $90,103   worst-day -147.2   max-DD -147.2

**G' was the recommended tail-clean config. G is what is live.** $4,969 of net bought $26 of
worst-day. That was decided before survival-first was the explicit governing filter. It is
worth re-deciding.

## THE JAR — mechanism, and it is a free improvement sitting unbuilt

The jar is **6 LIVE LOTS, not 6 positions.** `g_dots_live_lots` is a single integer:

    open                     += 1
    admission                 only if g_dots_live_lots < 6
    BE transition (S.8.1)    -= 1     the winner's lot LEAVES the jar
    still-live exit pre-BE   -= 1     (no double-decrement after BE)

A break-even'd winner carries no risk, so it stops blocking. **Unlimited open positions
provided no more than 6 are pre-break-even.**

**IT WAS SIMULATED:**

    2,409 trades (+74)   net $58,685 (+$1,418)   PF 5.83   WR 91.9%
    worst-day -$127.5    max-DD -$165.6          IDENTICAL to the count cap
    6/6 folds   22/22 weeks   OOS 6.57

**Strict improvement at the identical 6-lot hard bound.** More positions, same tail, because
the extras are only admitted once existing risk has already been retired.

Not blocked by a defect — sequenced behind the 50-signal panel so it is verified against the
final book. It is step 17f. **Why 6 in the first place is NOT RECORDED.**

## THE HEART OF THE OCEAN — a target, tested to destruction

**It is not a signal and there is no named set to look up.** It was defined as: one variable,
one threshold, ~100% WR **and** ~100% persistence **and** firing at least weekly. F13 exists
solely to hunt it.

**F13 ran. Clean documented NEGATIVE.** 117 variables × swept thresholds × 2 directions × 2
D2D polarities: **0 stars, 0 candidates.**

    best 100%-WR with >=22 trades   Sqz_Val:hi@p99 SHORT    24 tr, 10/22 weeks   FAILS persistence
    best full-6-fold single         Micro_Hurst:hi@p97 LONG 93 tr, 91.4% WR

91.4% is **below** the convergence book's 92.8%. The finding: *high WR only appears at low
coverage; full coverage caps at ~91%; persistence and WR are mutually exclusive for singles.*

**A caught defect validates the negative.** The first pass reported 126 stars — median 2
trades, 55 present in a single week — because `total_firings` counted condition-true bars
instead of entries. All 126 collapsed once frequency meant trades and persistence meant
full-span presence.

**"Diamonds" is F13's tier vocabulary**, not a named set of signals.

## HOW BOOK-50 WAS ACTUALLY CHOSEN

A 2,420-signal gated F0 pool → **BOOK-48 by loss-decorrelation** → two F1 sequentials added to
fill missing market structures, reaching 8/8 coverage:

    SQUEEZE_BREAKOUT   Sqz_Val:hi ->13-> Micro_OrderFlowDelta:lo   33 tr, WR 93.9%, PF 9.69
    TREND_EXHAUSTION   ADX_Rising:==0 ->8-> D2D_DirStep:==-1       42 tr, WR 92.9%, PF 4.75

**The binding criterion was worst-day held constant** — both additions recorded as "worst-day
HELD at −127.5". Ordering was **persistence and decorrelation first, then structure coverage.
Not PF.**

**THE 37/13 SPLIT FELL OUT. IT WAS NOT CHOSEN, AND IT WAS NOT FORCED BY THE DATA.** An
independent auditor, blind re-deriving from the **identical** 2,420-signal pool, produced
**26 LONG / 19 SHORT** — OOS PF 3.23, 6/6 folds, 22/22 weeks. Same pool, far more balanced
answer. **The imbalance is a property of the selection method, now confirmed three ways:** the
auditor's re-derivation, the 51.3/48.7 pool split, and the fact that 13 signals cannot stack
to depth 3.

**One close call, rejected on survival:** an earlier exhaustion fill including shorts stacked
counter-trend losses and pushed worst-day to −$238.

**AND THE FOUNDING FACT, CONFIRMED BY THE RECORD:** OOS *was* measured — but on May–June of
the same window the book was built on. The auditor separately demonstrated proper discipline
by selecting on Jan–April and measuring on held-out May–June. **BOOK-50 itself never had
that.**

## SHORT-SIDE CONDITIONERS — the field was swept, not assumed

117 adaptive variables were swept looking for a legitimate short-side conditioner.
**`Micro_Rejection:lo` was the best found: 98.6th random-subset percentile, OOS-positive,
mechanism-backed.** The sequential exhaustion sequence — Hurst-diminishing → FailedBreak →
D2D flip — was tested and found *"real but fragile: better at flagging worst flips than best
ones."*

**`Micro_Rejection:lo` independently reproduces on the new catalogue** — see trap 7.

---

# 4C. THE FOUR PRIOR EXPANSIONS — EVERY ONE CURVED DOWN

**This is the strongest prior in the project and it bears directly on composition.** A
dilution curve has been built before. Twice. Neither landed on an N above its starting point.

    taskB expansion      n=78 -> 124              VERDICT: DO NOT EXPAND PAST 78
    BOOK-48 + F1         x5 / 10 / 15 / 20
        OOS PF          6.65 -> 6.07 -> 5.87 -> 4.55 -> 4.03
        worst-day       -127 -> -167 ->  -320 ->  -457 ->  -405
    the +18 structure fillers                     "quality-for-throughput trade"
    rolling walk-forward on 78 signals            expanded sets collapse to OOS PF ~3,
                                                  WR ~87%, failing the 4.0 floor, while the
                                                  conservative baseline holds OOS PF 7.3-10.9
                                                  in EVERY fold

**Every increment traded OOS PF and worst-day for net.** The new same-bar-depth dilution curve
is the first of its kind, but it is not the first of its family, and its ancestors all curve
down.

## THE TRAP IN THE NEW WORK — AND WHY IT IS HARDER TO SEE

The new headline number is a **coverage** number: 6.82% / 7.71% against BOOK-50's 4.72% /
2.42%. Measured on episodes touched — **not on PF, not on worst-day, not on OOS.**

> **COVERAGE CANNOT FALL AS SIGNALS ARE ADDED, SO IT CANNOT WARN YOU.**
>
> Every prior expansion had a falling OOS PF to signal the stop. Coverage has no such
> property. It is monotone by construction. A book can be expanded until it is worthless and
> the coverage number will improve the entire way down.

**Three specific forms the failure takes:**

1. **The inclusion rule is 2.17× chance against a 2.40 bar.** A book composed from a catalogue
   inherits its inclusion rule's strength. 2.17× is real and it is not much.
2. **F1 is 95% of the catalogue by row count and contributes 3 convergence slots.** Browsing by
   eye means browsing F1. **The volume is not where the edge is.**
3. **The 512-trade PF 35.11 population is one frame.** The honest reading of every ancestor
   curve is *stop sooner than you want to.*

**The countermeasure already exists.** Score every candidate composition through
`score_book.py` on worst-day and OOS before committing, and treat coverage as **the tie-break
it is in the lexicographic order — not the headline.** The machinery to avoid this is built.
The risk is reading the coverage number first.

---

# 4D. THE PRICED-GATE METHOD — RECOVERED IN FULL

This is the only priced-gate method the project has, and it does not need re-inventing.

**Pool:** raw D2D flips both directions at ADX≥15, **n=302, ~17% losers** — chosen deliberately
because the 32 throne trades had one loss and could not discriminate.
**Tests:** 360 — 90 FEAT_ variables × hi/lo × direction.
**Statistic:** for a candidate slice of size k, draw many random k-subsets from the same 302
pool, compute the statistic, read where the candidate lands in that distribution.
**Five gates, all required:** random-pct ≥97.5 · OOS-positive · fold-persistent · n≥8 · a
stated mechanism.

**THE MULTIPLE-COMPARISONS CAVEAT WAS RECORDED AT THE TIME AND MUST TRAVEL WITH THE RESULT:**

> *"360 tests. At the p97.5 gate, expected chance survivors ≈9.0. Bonferroni for FWER 5% =
> 99.986th pct; max candidate reached 99.9th → **NONE clear strict Bonferroni**."*

That honesty is precisely why the out-of-sample reproduction matters so much.

**The ranked survivors — it is a FAMILY, not a variable:**

    variable              dir     n    WR      avg      PF    rnd-pct   OOS avg   folds
    Micro_Rejection lo   LONG    17   100%   $135.8   999      99.9     +$56.7    5/5
    Lower_Wick lo        SHORT   17  94.1%   $ 52.4   6.82     98.9     +$19.9    4/5
    Micro_Rejection lo   SHORT   19  94.7%   $ 48.1   6.98     98.6     +$42.8    3/4
    AT_Lookback_LT hi    LONG    25  88.0%   $ 81.8   5.46     98.1     +$27.5    6/6

**Recorded mechanism:** *"low rejection at a D2D flip = a decisive, clean flip bar with no
counter-wick; for shorts, no buying-wick defending the low."*

## RE-TESTED ON THE NEW CATALOGUE — AND THE FAMILY IS DIRECTIONALLY MIRRORED

The prior sweep ran on **D2D flips**. The re-test ran on the new catalogue's **depth-3+
entries** — a different pool AND a different population, so it is out-of-sample twice over.

    SHORT depth 3+ ungated  n=1,485  PF  3.60
      Micro_FailedBreak hi p50   n=  626  WR 93.1%  PF  7.61  net 13,298
      Micro_Rejection   lo p10   n=  238  WR 89.1%  PF  6.86  net  5,286
      Upper_Wick        hi p70   n=  788  WR 90.2%  PF  5.77  net 22,863   keeps 79% of net
      Micro_Rejection   lo p30   n=  630  WR 89.7%  PF  5.53  net 19,176

    LONG  depth 3+ ungated  n=1,150  PF 13.97
      Micro_FailedBreak hi p70   n=  484  WR 98.1%  PF 34.26  net 33,764
      Micro_Rejection   lo p10   n=  183  WR 96.2%  PF 27.54  net  5,234
      Lower_Wick        hi p70   n=  480  WR 96.7%  PF 25.50  net 29,919
      Micro_FailedBreak hi p30   n=  795  WR 96.5%  PF 21.90  net 45,796

**Three findings that were not visible before:**

1. **The wick family is MIRRORED, and the mirror is mechanically correct.** LONG wants
   `Lower_Wick:hi` — a buying wick defending the low. SHORT wants `Upper_Wick:hi` — a selling
   wick rejecting the high. Same mechanism, opposite member. The prior record's
   `Lower_Wick:lo SHORT` was measured on flip bars, where the reading differs.
2. **`Micro_FailedBreak` is NOT a short-side gate.** It is the strongest gate found on BOTH
   sides, and it is stronger on longs — **PF 34.26 against an ungated 13.97**. The earlier
   characterisation of it as a short-side fix was wrong.
3. **`Upper_Wick:hi p70` keeps 79% of short net at 1.6× PF** — the best net-retention gate
   found. Under survival-first, retention matters as much as the PF headline.

**NONE of these has been priced by the 302-pool method.** They are re-tests of a priced family
plus new members found by an unpriced search. **Price them before adopting.**

---

# 4F. THE CONVERGENCE GRADIENT HAS BEEN TESTED AGAINST A NULL — AND IT HOLDS

**This is the load-bearing claim of the whole project and it had never been put against a
random baseline.** The depth gradient — PF rising with same-bar convergence — could in
principle be a property of CLUSTERING rather than of SIGNAL QUALITY. If it were clustering,
random triples from the same vocabulary would show the same rise.

**They do not.**

**Method:** 50 random triples drawn from the 243 firing conditions of the same 249-condition
pool, direction at the pool's own 51/49 split, built through the identical
`score_g.build_book` path and scored through the identical `run_portfolio` with the full
conviction stack. Three seeds. No filtering, no VALID predicate.

    depth ladder      solo    dual    3-4      5+       trades at 5+
    random seed 11    1.07    0.95    0.92    999.00          5
    random seed 22    1.01    0.86    0.80      n/a           0
    random seed 33    1.13    1.07    1.14    999.00          5

    REAL BOOK (LONG)  3.59    2.43    8.68     39.40        387

1. **Random triples show NO gradient — flat to DECLINING**, 1.07 → 0.95 → 0.92. The real book
   rises 3.59 → 2.43 → 8.68 → 39.40. **The gradient is signal quality, not clustering.**
2. **Random triples cannot reach depth.** 50 selected signals produce 387 long trades at depth
   5+; 50 random ones produce 5, 0 and 5. **Deep convergence is specific to signals selected
   for it.**

**Caveat:** the random triples were not fire-rate matched, so their depth distribution differs.
That cuts in the project's favour — they produced 3–4× MORE total trades and still could not
stack.

**STILL UNTESTED:** the triple has never been competed against a 4- or 5-variable grammar, nor
against a same-bar PAIR. Three was chosen as a search design where the arithmetic was
affordable. This shows three beats random; it does not show three beats two or four.

---

# 4G. THE DENSITY SWEEP — A BOOK-SCORING TOOL NOTHING POINTS AT

**Every depth figure in circulation was INFERRED by grouping trades on `entry_bar`. The engine
has its own proper measurement and it has never been run.**

    scanners/triple_convergence_and_d2ddir.py
      L461-469   DENSITY_K_BANDS = [1, 2, 3, 4, 5, 6, 8, 10]   "fused F10 dimension"
      L472       build_set_density()
      L523       density_sweep()
      L549       run_density()
      L589       fires ONLY on:  ... density <book.csv>

**It is a SEPARATE entry point.** `master.py` imports the module for its triple scan and never
calls `run_density`. `discovery_orchestrator.py` L36 records the decision in a comment — *"it
is run SEPARATELY and its CSV ingested"* — and nothing else mentions it. **That is why the
10h43m run produced no density output: nothing was ignored, nothing was called.**

**WHY IT CANNOT LIVE IN THE PIPELINE:** `density_sweep` scores a **finished signal set** at
k = 1…10. During discovery there is no book — the catalogue is the deliverable and item 15
means nothing chooses. It is correctly outside the pipeline.

**HOW TO RUN IT:**

    python scanners/triple_convergence_and_d2ddir.py density <your_book.csv>

**Its default argument is `recommended_set_76.csv`, a superseded pre-reconstruction file.
ALWAYS pass the book explicitly.**

**WHAT IT GIVES YOU:** the depth ladder measured by the ratified engine rather than inferred —
co-firing count ≥ k over the **candidate signal set under evaluation**, direction-aligned, run
as a **gate** rather than a post-hoc grouping. The scanner's own note records why it is not
measured over the raw 249-condition pool: *"that saturates"* — confirmed independently, since
at 1,000 signals 97.1% of bars reach depth ≥3.

> **USE IT AS A CHECKPOINT ON ANY CANDIDATE BOOK.** It answers *"does count≥5 outperform
> count≥2"* directly, on whatever set you point it at, and it is the correct instrument for
> that question. BOOK-50 is the current signal set; every future book should be screened the
> same way.

---

# 5. THE STANDING RULINGS

**The matched-null tolerance band is ±35% and MUST NEVER BE WIDENED.** If a family blanks its
pricing at K=200, the remedy is a **larger K (400–600)**, never a wider band. Widening it
produces a null matched to a different rarity than the population it prices — which is the
original defect re-entering through the one parameter that looks like a dial and isn't.

**No quotas, no floors, no pre-set directional targets.** Composition is an **output** to be
reported, never an **input** to be constrained. Balance is achieved by ranking each direction
separately, not by imposing a count.

**A book is UNSCORED until `score_book.py` has run on it.** No matter how good the per-signal
numbers look, set properties — TailDep, FailConc, mCVaR, union coverage, absolute survival —
have no per-signal value and must be computed on the assembled book.

**Failure is a map, not a verdict.** A weak result redirects a line of inquiry; it never ends
one. Do not let a prior negative harden into a lens that pre-interprets new evidence.

---

# 5B. THE TARGET SYSTEM — THIS IS WHAT IS BEING IMPROVED

**The operator is not composing a book from scratch. He is expanding a system that already
works.** Every measurement in this project should be read against this structure.

    solo    -> Hurst p90 AND ticks >= 300
    double  -> Hurst p90
    triple+ -> FREE, no gate

On BOOK-50, full span, 1 lot:

    tier                        trades    WR       PF      net      worst day  losses
    ungated (book, no gaps)      2,678   91.3%    5.14   $77,239    -$639.1     234
      solo + Hurst + ticks         146   91.1%    6.49    $6,877    -$187.6
      double + Hurst               104   96.2%   16.18    $2,575     -$50.8
      triple+ free                 505   98.0%   53.70   $26,616    -$138.9
    GATED TOTAL                    755   96.4%   19.71   $36,068    -$130.7      27
    GATED @ flat 2 lots            755   96.4%   16.87   $48,604    -$209.2      27

**Three properties that make this the target and not merely a result:**

1. **Gating IMPROVES the worst day below triples alone** — −$130.7 against −$138.9. The gated
   solos and doubles lose on DIFFERENT days from the triples. They are decorrelated ballast,
   not filler.
2. **It held out of sample, with the gates unfitted.** On the 18 unseen trading days
   Jun 25 → Jul 21: ungated PF 2.25 → gated PF 16.63. **96% of the money at 28% of the worst
   day and 8% of the losses.** The solo gate alone cut 143 break-even trades and nearly tripled
   solo net.
3. **The triples survived that segment and the solos did not** — solo PF 1.09, triple+ PF
   36.17. **That is what took BOOK-50 from 6.40 to 2.19: it carried 2,173 ungated shallow
   trades against 505 triples.** The engine never failed. It was dragging weight.

**THE OBJECTIVE IS TO EXPAND THIS — more longs, more shorts, more terrain, more opportunities —
WITHOUT DEGRADING IT.**

**METHOD CAVEAT, and it is not small:** the gated rows above were produced by FILTERING an
existing trade table, not by re-running the simulation with the gate active. The position/lot
cap binds regularly — 15.4% of entries occur with 5+ already open. Blocking ~1,900 shallow
trades frees capacity and would admit trades previously turned away. **The real gated book has
more than 755 trades and different ones.** The out-of-sample split is unaffected and stands;
the gated totals need re-running with the gate inside the simulation before they are final.

---

# 5C. THE ACCEPTANCE RULE — MARGIN OF SAFETY, NOT PROFIT FACTOR

**This is the stopping rule the four failed expansions never had.**

Every prior expansion raised net and lowered out-of-sample PF, and the only warning arrived
afterwards. Coverage cannot warn you — it is monotone. **Break-even win rate can, and it moves
live as signals are added.**

    break-even WR = avg_loss / (avg_win + avg_loss)
    margin        = actual WR - break-even WR

**BOOK-50, corrected frame:**

    tier       n     WR      PF    avg win  avg loss   win/loss   break-even   margin
    solo    1,225   88.2    3.00    38.13     95.42      0.40       71.4%     16.8pp   144 losses
    double    992   90.5    4.99    31.46     60.21      0.52       65.7%     24.8pp    94 losses
    triple+   512   97.3   35.11    54.55     55.27      0.99       50.3%     46.9pp    14 losses

**THE CROSSOVER IS THE FINDING.** At solo the average loss is 2.5x the average win — solos are
solvent only on an extreme win rate, which is exactly why they collapsed to PF 1.09 the moment
that rate slipped. **At triple+ the payoff reaches parity (0.99) and the margin of safety
nearly triples.** A 10-point win-rate degradation is fatal to solos and harmless at depth.

**AND HERE IS WHY PF IS THE WRONG AXIS.** A 100-signal illustrative book, same frame, same
engine, 5x the triple population:

    tier       n     WR      PF    avg win  avg loss   win/loss   break-even   margin
    solo    1,704   90.6    3.56    35.06     95.13      0.37       73.1%     17.5pp
    double  2,278   86.0    2.46    26.26     65.83      0.40       71.5%     14.5pp
    triple+ 2,635   91.2    6.61    42.86     67.48      0.64       61.2%     30.1pp
    five+     895   90.2    7.45    63.60     78.24      0.81       55.2%     35.0pp

**It never crosses over.** Best is 0.81 at five+. **BOOK-50's triples pay $1 for every $1
risked; these pay 64 cents.** Margin at triple+ is 30pp against 47pp.

PF 6.61 looks respectable and conceals all of it. **Break-even WR shows it in one number.**

> ## THE RULE
>
> **A signal may be added only if the triple+ tier's win/loss ratio does not fall.**
>
> Expand for terrain, for direction balance, for opportunity — and check the ratio after each
> admission. **The moment it starts falling, stop.** That is the live warning light the
> previous four expansions did not have.

**CAVEAT ON DEGENERATE TIERS:** BOOK-50's five+ shows break-even 0% and margin 100pp because it
has ZERO losses. Quad+ rests on 8. **Never quote a win/loss ratio or a break-even WR without
its loss count** — a tier resting on fewer than ~20 losses is noise wearing a number.

---


## THE NUMBER TO BEAT — OPTION B

A Supervisor pre-pass over seven attempts produced this. **All 1 lot, in-sample, full conviction stack, jar
active, and every book below PASSES every hard constraint through `score_book.py`.**

    system                       trades      net     WR      PF    FailConc    mCVaR    survival
    BOOK-50 raw                   2,729   $76,458  90.8%     -      3.458   -$4,922     -$565
    FUSED-50 raw                  2,697   $84,691  91.7%     -      1.649   -$2,091     -$340
    FUSED-120 raw                 6,433  $158,418  89.8%   3.38     4.071        -      -$658
    FUSED-120 + Option A          3,343  $120,373  93.6%   7.36     2.067        -      -$139
    FUSED-120 + Option B          1,988  $100,094  96.6%  14.14     1.352        -      -$273

**OPTION B: $100,094 | WR 96.58% | PF 14.14 | 68 losses | 3 losing days of 119 | FailConc 1.352 — the lowest
measured anywhere in this project.**

    avg win $56.10 | avg loss -$112.06 | win/loss 0.501
    break-even WR 66.64% vs actual 96.58% -> MARGIN 29.9pp
    gross win $107,714 | gross loss -$7,620

    month     n     W    L    WR%      PF      net       wd        concurrence     n     PF      net
    2026.01   98    90    8  91.8    7.82   $3,266    +$266        solo          186   3.14   $6,608
    2026.02  386   378    8  97.9   16.25  $19,240     +$50        dual          494   7.20  $14,835
    2026.03  440   428   12  97.3   13.17  $22,660     +$32        triple        507  14.99  $23,782
    2026.04  354   341   13  96.3   13.77  $13,393     +$69        quad          348    inf  $18,942
    2026.05  255   241   14  94.5   11.03  $12,719    -$236        5+            453  81.95  $35,927
    2026.06  274   265    9  96.7   14.87  $14,637    -$273
    2026.07  181   177    4  97.8   22.93  $14,180     +$69        Jan-May PF 13.04 -> Jun-Jul PF 17.94

**Quality RISES into the window that broke BOOK-50.** The three losing days across six months total $605.


### SIX MEASUREMENTS TAKEN AFTER THE FIRST DRAFT — ALL MATERIAL

**1. THE GAP-FILLER SYSTEM MUST NOT BE APPLIED TO OPTION B.**

    Option B, NO gaps   1,988 tr  $100,094  WR 96.58%  PF 14.14  wd -$272.9   68 losses  3 losing days
    Option B + gaps     2,546 tr  $116,028  WR 94.70%  PF  8.06  wd -$495.6  135 losses  7 losing days

+$16K for double the losses and an 82% worse tail. **The BOOK degrades before a single gap trade is
counted** — a gap holds a lot in the 6-lot jar and changes which book trades are admitted afterwards.
Option B's gates leave it flat 80.5% of the time, so gaps fire into space the book was about to use. On
BOOK-50 gaps IMPROVE the worst day; **that does not transfer to a heavily gated book.**
**ADOPTED: OPTION B WITHOUT GAP FILLERS.**

**2. NO OTHER FAMILY WORKS AS A GAP FILLER — clean negative on flat bars.**

    F3   53 signals  17,300 on-flat trades  WR 75.1%  PF 0.78  -$38,753
    F9  127 signals     651                 WR 78.3%  PF 0.92   -$1,143
    F11   2 signals     399                 WR 78.9%  PF 0.85     -$674
    F2    1 signal       12                 WR 75.0%  PF 0.38     -$366

Every one loses money where the book is flat. **The gap-filler role was the most promising unexplored
use for the non-F0 families and on this test it is dead.**

**3. TERRAIN COVERAGE IS OPTION B'S WEAK AXIS — LONG WENT BACKWARDS.**

    LONG   70 signals  47 episodes  4.11% of reachable   vs BOOK-50 54 / 4.72%  = 0.87x
    SHORT  50 signals  48 episodes  4.16%                vs BOOK-50 28 / 2.42%  = 1.71x
    ceiling 103 UP / 89 DOWN — Option B captures 45.6% long, 53.9% short

**Balance achieved; reach not.** The objective was loss decorrelation and coverage was never in it —
decorrelated signals cluster in time by definition. Greedy union coverage reaches 6.82% / 7.71% from 50
signals. **Option B is one corner of the frontier: maximum quality, minimum reach.**

**4. THE RANDOM-SUBSET CONTROL — THE GATES SIT AT THE 100th PERCENTILE.** 4,000 random 216-trade draws
from the 1,666 ungated Jun-Jul trades: median PF **3.32** (full set 3.31), p99 6.69, **max 9.68**.
**Gated PF 11.79 is above all 4,000.** Random selection returns the same PF as the full set, so "any
rule taking the top 13% wins by construction" is FALSE. **The gates carry real information out of
sample.**

**5. PER-TIER AVERAGE LOSS — the tail is in the GATED long shallow cells, not the free ones.**

    LONG  solo   14 losses  avg -$220.27  total -$3,084   40% of all loss   GATED
    LONG  dual   22 losses  avg -$100.90  total -$2,220   29%               GATED
    SHORT triple 12 losses  avg -$105.65  total -$1,268   17%               FREE
    SHORT quad / SHORT 5+ / SHORT solo / LONG quad:  ZERO losses

**69% of all losses sit in the two long shallow cells that are already gated.** The free short deep
tiers carry 17% at a below-average loss size. The objection that a free tier hides the risk is not
supported here.

**6. THREE CORRECTIONS.** (a) **The walk-forward tested a DIFFERENT gate set** — fitted Jan-May, 1,146
trades, against the headline's all-seven-months 1,988. It proves the METHOD generalises; **the headline
gate set has never been out of sample.** (b) **The real build item is JAR ADMISSION ORDER, not depth
counting** — the EA already evaluates every rule every bar so the tally is free; what is unspecified is
which five of eight qualifying entries get the six live lots on a deep bar. (c) **The one F1 signal came
from BOOK-50**, not from catalogue-shopping — it is the SQUEEZE_BREAKOUT structure-fill, unioned in and
kept on merit.


**FUSED-50 is the low-fit control.** Beats BOOK-50 on every axis at the same size — FailConc 1.65 vs 3.46,
mCVaR -$2,091 vs -$4,923, survival -$340 vs -$565, +$8,233 net on 32 fewer trades — **with no gate fitting at
all.** Its LONG triple+ tier pays above parity: win/loss 1.0349, break-even 49.14%, margin 48.8pp, 7 losses.
**It passes the acceptance rule outright.**

**THE GATES WERE WALK-FORWARDED AND THEY HOLD.** Re-fitted on Jan-May only and applied unchanged to Jun-Jul:
**PF 11.79 against an ungated control of 3.31, WR 95.83% vs 89.02%, worst day -$249 vs -$658** — on months the
optimiser never saw. What does not carry is the TIGHTNESS: the training fit reaches PF 2,378 on one loss, and
at that tightness it keeps only 24% of the net out of sample. **Fit looser than the optimiser wants.** The
direction is proven; the calibration is the open work.

### WALK-FORWARD ON THE GATES — RUN, AND THEY HOLD

The gates were re-fitted on **Jan-May ONLY** and applied **UNCHANGED** to June-July. The optimiser never saw
the test months.

    segment                     trades      WR       PF       net     worst day   losses
    TRAIN Jan-May (fitted)         930   99.89%  2378.38   $56,106      +$5.4         1
    TEST  Jun-Jul (UNSEEN)         216   95.83%    11.79   $10,400     -$249.0        9
    ungated control Jun-Jul      1,666   89.02%     3.31   $43,103     -$658.0      183

**On genuinely unseen data the gated book runs PF 11.79 against the ungated control's 3.31 — 3.6x — with
WR 95.83% against 89.02% and a worst day of -$249 against -$658.** The gates generalise.

**What does NOT generalise is the TIGHTNESS.** The training fit is PF 2,378 on a single loss — the search
overfitting, plainly visible. Tuned that hard it keeps only **24% of the net** out of sample ($10,400 of a
possible $43,103).

**THE OPERATIONAL CONCLUSION: fit gates LOOSER than the optimiser wants.** A gate tuned to eliminate losses
in-sample keeps almost nothing out-of-sample; one tuned to IMPROVE the ratio keeps most of the money and most
of the improvement. That is a tuning problem with a measured direction, not a failure.

**AND IT IS BUILDABLE.** 119 of Option B's 120 signals are F0 triples — exactly what the rule table expresses.
95 distinct variables, all present in the 172-column export. Every gate variable present in the export. One F1
signal needs the sequential latch already specified for BOOK-50's two. **No new variable, no new export, no
mechanism that does not exist.**



**Both books go to you: one proven by method, one by search. Beat them, or show why they do not hold.**

# 6. WHAT "BETTER THAN BOOK-50" ACTUALLY MEANS

Not one number. All of these, and survival first:

    survival     worst day  <  -$2,500       BOOK-50: -$565
    persistence  folds_plus, min_fold_pf, and OOS-PF   BOOK-50 OOS-PF 4.91
    profit       PF and net                  BOOK-50: PF 4.81, $97,675
    reach        share of reachable terrain  BOOK-50: 4.72% UP / 2.42% DOWN
    balance      direction x structure x session x regime   BOOK-50: 37L/13S
    convergence  the same-bar depth population  BOOK-50: 512 trades at 3+, 160 at 5+

A book that doubles the net and doubles the worst day has not improved. A book that matches
the net at higher PF, wider coverage and balanced direction **has**.

**And the one that matters most:** it has to still be true in six months. That is what the
walk-forward measures, and it is why `EXPECTED_ROWS_AT_OR_ABOVE_THIS_PF` outranks `agg_pf`
every time.
