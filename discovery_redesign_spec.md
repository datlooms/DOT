# discovery_redesign_spec.md

**Phase:** DOT_execution_sequence.md step 17n — gate-first discovery redesign
**Author seat:** Quant Analyst
**Status:** specification for Developer build and Auditor verification
**Date:** 2026-07-23

---

## 0. PROVENANCE

| item | value |
|---|---|
| repo | `github.com/datlooms/DOT` @ `852c7e0`, 140 files |
| `engine/dots_thresholds.py` | `518862bf19fb` (expected — MATCH) |
| `master.py` | `8ccff8d0df91` (expected — MATCH) |
| dataset | `DOT_stitched172_jan19_jul21_part01..09.csv` → 177,251 × 172, 2026.01.19 15:49 → 2026.07.21 17:09, strictly increasing, 0 dup, 0 NaN |
| S8B output | `cluster_participation_profile.csv`, 2,988 × 69 |
| harness sanity | 3,057 trades / net $98,205 on the full stitched series |
| condition pool | 249 = 180 FEAT hi/lo (90 × 2) + 69 equality |

Every figure in this document was computed in-session through the ratified path. Figures quoted from prior documents are labelled as such and were re-derived before use.

**Governing principle (Step 2, binding):** the redesign is **ADDITIVE**. The measured problem is reach, not excess. No variable, condition, mechanism or family is removed on a single measurement. Where a component looks weak, this spec states what will be measured to decide. Gates are state columns; no bar is ever deleted.

---

## 1. CORRECTIONS TO THE RECORD

Three corrections arise from measurements taken while writing this spec. Two are material to decisions already recorded. All are reproducible from the stitched series through the ratified path.

### 1.1 — MATERIAL: the reveal doc §5 "Aligned / Misaligned" table is a REGIME-STATE filter, not a directional-alignment gate

`DOT_new_data_reveal_2026-07-21.md` §5 and `DOT_progress_and_rd_plan.md` decision 2 describe "gating BOOK-50 trades on **directional alignment** with `AT_Regime_ST`". The table's figures do not correspond to a directional-alignment computation. They correspond exactly to a **regime-state filter** — keep trades whose entry bar carries `AT_Regime_ST == 1`, irrespective of trade direction.

All 375 NEW-segment trades, three candidate readings:

| reading | n | WR% | PF | net $ | worst day $ |
|---|---|---|---|---|---|
| A — directional, correct encoding (0=bull), aligned | 171 | 74.9 | **0.97** | **−111** | −682.7 |
| A — directional, correct encoding, misaligned | 204 | 86.3 | 3.98 | 8,518 | −476.0 |
| B — directional, inverted encoding (1=bull), aligned | 204 | 86.3 | 3.98 | 8,518 | −476.0 |
| **C — regime-state only, `AT_Regime_ST == 1`** | **267** | **86.5** | **3.83** | **9,228** | **−116.3** |
| C — regime-state only, `AT_Regime_ST == 0` | 108 | 67.6 | 0.78 | −821 | −534.9 |
| *record as written* | *267 / 108* | *87 / 68* | *3.83 / 0.78* | *9,228 / −821* | *−116 / −535* |

Reading C reproduces the record to every published digit. The arithmetic in the record is correct; **the label is wrong.** What was measured is not what was decided.

**Consequence:** recorded decision 2 — "`AT_Regime_ST` **directional agreement** joins the gate stack" — cites evidence for a different operation. The directional gate as decided (reading A) is **PF 0.97 and net −$111 on the NEW segment**: it removes the book's profit. This is consistent with my earlier independent measurements on F0 concurrent (ungated PF 9.76 → AT_Regime_ST-aligned 6.85 → sign(AT_Slope_ST)-aligned 6.97).

**This spec does not adopt a directional AT gate.** §E specifies what would be measured to decide the regime-state variant on its merits.

### 1.2 — MATERIAL: the Section D tension is not a contradiction; both figures are correct on different scales

The reveal doc's "77% of profit from the largest-move quartile" and S8B's "9.8–17.9% of deep-cluster bars are thrust bars" measure different quantities.

Reproduced, NEW-segment trades bucketed by |60-bar forward move| (n=370):

| quartile | n | net $ | share of profit | PF | median fwd60 |
|---|---|---|---|---|---|
| Q1 | 93 | 1,154 | 13.3% | 1.78 | 18.9 pt |
| Q2 | 92 | 735 | 8.5% | 1.66 | 53.8 pt |
| Q3 | 92 | 281 | 3.3% | 1.13 | 119.6 pt |
| **Q4** | **93** | **6,481** | **74.9%** | **4.27** | **204.0 pt** |

The record's PF 4.27 and 204 pt median reproduce exactly; profit share is 74.9% against the published 77%.

The reconciliation:

| scale | traded Q4 sits at | interpretation |
|---|---|---|
| **absolute points** | Q4 lower edge 160.8 pt = **percentile 88.9** of all eligible bars | book *does* capture big moves |
| **ATR-normalised** (what thrust uses) | traded Q4 median 5.90 disp/ATR = **percentile 72.6** of all eligible | book's moves are *ordinary multiples* |

All-eligible median disp/ATR is 3.40; the book's traded NEW bars median **2.56** — below the market median once normalised. Only **8.6%** of all traded NEW bars clear the all-eligible p85 disp/ATR threshold, and only 26.9% of traded Q4 bars do.

**Both statements are true.** The book trades during high-ATR conditions where a large absolute move is an unremarkable normalised move. "Big-move capture engine" holds **in absolute points**, which is what pays. The low thrust overlap holds **in ATR multiples**, which is what S8B measures. The record should carry the qualifier; neither figure is wrong.

### 1.3 — the "missed set is too small to trade" hypothesis is REFUTED

The brief instructed measuring absolute point displacement of missed vs traded thrust episodes before specifying anything that chases the missed set. Measured, thrust episodes chained at N=5:

| W, K | episodes | traded | missed | traded median | missed median | missed ≥50 pt |
|---|---|---|---|---|---|---|
| 15, p85 | 3,241 | 151 (4.7%) | 3,090 | 168.0 pt | **58.5 pt** | 61.0% |
| 30, p85 | 2,359 | 128 (5.4%) | 2,231 | 224.0 pt | **77.5 pt** | 80.4% |
| 15, p90 | 2,539 | 112 (4.4%) | 2,427 | 182.8 pt | **64.1 pt** | 67.8% |

The missed set is **not** dominated by untradeable moves: median 58–78 points, with 61–80% of missed episodes exceeding 50 points. On US30 at $1/point/lot these are tradeable.

But the second half of the result matters equally: **traded episodes are 2.5–3× larger than missed ones** (168–224 pt vs 58–78 pt). The book is not blind to thrusts — it is selecting the largest ones. Reaching the missed set means trading a systematically smaller-move population.

**Consequence:** reach is a real opportunity in count (only 4.4–5.4% of thrust episodes are touched) but any signal recruited to reach it must be scored on its own merits, not credited with the traded population's economics. §D specifies this.

### 1.4 — housekeeping: F10 stale flag

`discovery_map.md` §3 line 90 carries `F10 — GAP — flag`, contradicted by the same file's Resolution status line 243: "**F10 — FOLDED INTO F0** (concurrence lens null; F12 profiler is the diagnostic remnant). Not a gap; the family set is complete." Line 90 to be corrected to `FOLDED INTO F0`. Documentation-only; no code impact.

---

## 2. SECTION A — ALL FOURTEEN FAMILIES: OPEN EVALUATION

The committed book draws from F0 (48) and F1 (2). F2–F9, F11, F12, F13 are classified exploratory. **This spec treats that classification as unexamined, not as a diagnosis.** Nobody has evaluated the unused families under a depth/coverage framing; they may contain nothing or a great deal.

### A.1 — The per-family evidence review (mandatory first deliverable of the build)

For each of F0–F13 the build produces one row of an evidence table, grounded in the actual output files under `discovery_results/` and `dots_results/`, not in the map's prose descriptions.

Required columns per family:

| column | definition |
|---|---|
| `family`, `scanner`, `sha` | identity, self-computed |
| `rows_emitted` | actual row count in its results CSV on the sealed run |
| `candidates_passing_S5` | rows meeting `trades>=30 & folds_plus>=4 & agg_pf>=2.0` |
| `distinct_conditions_used` | how much of the 249-vocabulary the family actually touches |
| `depth_participation` | share of its candidates' fires falling inside size≥5 clusters (all three S8B bases) |
| `co_fire_with_F0` | share of its fires sharing an entry bar with an F0 book signal |
| `coverage_of_missed` | share of its fires landing in thrust episodes the current book does NOT touch |
| `regime_conditional_net` | net per §H.3 bucket, not aggregate |
| `verdict` | `SELECTABLE` / `DIAGNOSTIC` / `INSUFFICIENT-EVIDENCE` |

**No family may be assigned `DIAGNOSTIC` on the basis of its historical classification.** The verdict must be justified by the measured columns. `INSUFFICIENT-EVIDENCE` is an acceptable and expected outcome and must be used rather than a guess.

*Falsification:* if a family's candidates show `depth_participation` at or below the base rate and `coverage_of_missed` at or below the base rate across all three bases and all §H.3 buckets, it contributes nothing under this framing. That is a measurement, not an assumption.

*Fit risk:* the sealed-run outputs were produced on Jan–Jun. Family output must be regenerated on the full stitched series before the verdict is taken as evidence, or the review inherits the old window.

### A.2 — F12 (`concurrence_profiler.py`) vs S8B

F12 is classified "MEASURES, never selects" and reads `dots_results/deduped_survivors.csv`. S8B profiles the full 249-condition vocabulary against three cluster bases.

They are **not** the same lens. F12 measures raw variable-depth concurrence among F0 survivors; S8B measures per-condition participation against book-derived (bases 1, 2) and price-derived (basis 3) episodes. F12's population is the F0 survivor set; S8B's is the vocabulary.

**Specified measurement to decide the relationship:** run both on the stitched series and cross-tabulate F12's depth-outcome relation against S8B's `lift_5` and `cov_missed_share` per condition. Three possible outcomes, and the build reports which obtains:
- **duplicate lens** — F12's depth ranking correlates > 0.8 with S8B `lift_5` on shared conditions → F12 stays diagnostic, superseded.
- **independent lens** — correlation < 0.5 → F12 measures something S8B does not; its diagnostic-only classification should be revisited and the build states what it would take to make it selectable.
- **superseded** — F12's output is reconstructible from S8B columns → retire the lens, keep the file.

**Do not assume the answer.** The classification change, if any, follows the measurement.

### A.3 — F13 (`single_variable_extremes`)

F13 is a documented negative (0 stars, 0 candidates), independently corroborated by S8B basis 3: no single condition separates, maximum ATR-controlled lift 1.35.

**Verdict: the negative stands as settled for F13's stated claim** — that a single variable at an extreme is a standalone tradeable edge. Two independent methods on different framings agree.

**But the negative does not transfer.** F13 tested single conditions as *entry signals*. It did not test single conditions as *cluster participants* or *coverage contributors*, which is what S8B measures and what this redesign selects on. A condition that is worthless alone may still be the third leg of a triple that deepens clusters. The build must not use F13's negative to exclude any condition from the vocabulary or from triple formation.

*Falsification of the settled verdict:* if any single condition shows regime-conditional positive net in all §H.3 buckets with ATR-controlled lift > 2.0 and survives §H.1 correction, F13 warrants a re-look. The build reports whether any does.

### A.4 — CROSS-FAMILY CO-FIRING (new measurement, never performed)

Depth currently arises only from F0 signals firing together. Whether a signal from F2 (state transition) can co-fire with an F0 triple into shared depth has never been examined.

**Specified measurement:** extend the S8B cluster construction to admit entries from any family, tagged by origin. For every cluster, record the family composition vector. Then:

1. **Do mixed-family clusters form at all?** Report the count and size distribution of clusters containing ≥2 distinct families, at N=5 and N=10, all three bases.
2. **Do they behave like same-family clusters on the depth ladder?** Report WR / PF / net / worst-day by cluster-size band, split single-family vs mixed-family. The reference is the measured same-family ladder: size 5-7 → WR 95.5%, PF 11.39; size 13+ → WR 96.0%, PF 11.78, worst day −$12.3 (N=10).
3. **Does mixing add unique-variable count at depth?** Per §F.4 a depth-2 event carrying only 3 unique variables is not genuine corroboration. Cross-family entries are more likely to carry disjoint variables; measure whether they do.

**What a mixed-family cluster would mean:** if mixed clusters match the same-family ladder, depth is a property of *simultaneous independent agreement* rather than of F0's triple architecture, and the book's family monoculture is a reach constraint rather than a quality guarantee. If mixed clusters underperform, F0's architecture is doing work beyond mere co-occurrence. Both outcomes are informative; neither is assumed.

*Fit risk:* admitting 13 families multiplies the co-firing surface. Any mixed-cluster result must clear §H.1 correction at the expanded trial count before it informs selection.

### A.5 — Multiple-testing implications of the family expansion

Scoring all fourteen families rather than one changes the trial count materially. This is handled in §H.1, which must be parameterised on the **actual** post-expansion trial count, not the historical F0-only count. The build reports the count per family and in total.

---

## 3. SECTION B — EVERY MASTER.PY OUTPUT IS A RESEARCH INPUT

Per-stage audit. `feeds selection today` is a factual statement about the current pipeline; `measurement to decide` is what this redesign requires where the answer is no.

| stage | emits | feeds selection today | measurement to decide |
|---|---|---|---|
| **S0 ingest** | `df`, attestation, `input_sha` | no (provenance) | **DIAGNOSTIC — justified.** Provenance and integrity only. Requirement: the attestation must record dataset range and row count in the run report so no downstream figure is read without provenance. |
| **S1 thresholds** | `ad` (mechanism-D), `st` (structural gates) | yes, indirectly | Already the sole threshold source. No change. Sacred. |
| **S2 pool** | 249 conditions, `anchor`, warm-up floor `w` | yes | Feeds everything. §G specifies selection-time handling of duplicates and dead conditions **without modifying the pool** (S.10 sacred). |
| **S3 discovery** | `results_F*.csv` for all families | **F0 and F1 only** | §A.1. All families' output enters the evidence review. This is the largest unused surface in the pipeline. |
| **S4 schema** | `results/discovery_master.csv` (all families collated) | no — collated then filtered | **This file already contains every family's output in one schema.** It is the natural input to §A.1 and is currently consumed only by S5's three-condition filter. Requirement: §A.1's evidence table is built from `discovery_master.csv`, not from per-family files, so nothing is silently dropped. |
| **S5 filter** | `candidates.csv` — `trades>=30 & folds_plus>=4 & agg_pf>=2.0` | yes | **Three hard thresholds with no trial-count adjustment and no regime conditioning.** They are the funnel's only quality bar. §H replaces them: the thresholds stay as a *participation floor* but acceptance moves to the empirical-null bar (§H.1) plus stability (§H.2) plus regime-conditional positivity (§H.3). Measurement: report how many candidates each criterion removes independently, so the filter's actual selectivity is visible rather than assumed. |
| **S6 regen** | `signal_full_records.csv`, `signal_per_day_pnl.jsonl` | no | Regenerates artifacts documented stale (`746102aae415` / `0910f360a628`). **Measurement to decide:** these carry per-signal per-day P&L, which is exactly the input §C.2 needs for the failure-correlation matrix. Requirement: S6's output becomes the source for §C.2 rather than being regenerated and unused. |
| **S7 contenders** | C0–C5 conviction-variant scores | no — reporting only | Six sizing variants scored on the same book. **DIAGNOSTIC today, but §F.3 requires the depth-size curve to be validated independently of selection.** S7 is the correct home for that: add the depth-conditioned variant as a contender row and report it beside C0–C5. |
| **S8 committed** | committed-book replay + canary | no — verification | **DIAGNOSTIC — justified.** The $92,347 / 2,698-trade canary is an engine-integrity check, not a selection input. Keep as is. |
| **S8B cluster profile** | `cluster_participation_profile.csv`, 2,988 × 69 | **no — built, never consumed** | §D.3 makes its coverage columns a selection input. This is the specification's single largest change to what feeds selection. |
| **S9 report + split** | `master_report.md`, split artifacts | no | **DIAGNOSTIC — justified.** Packaging and reporting. Requirement: the report must surface the §A.1 evidence table and the §H acceptance decisions, not just the committed-book headline. |

**Standing requirement:** no stage output may be described as diagnostic-only in the final build without an explicit justification in the run report. Three are justified above (S0, S8, S9). Every other stage either feeds selection or has a named measurement that decides whether it should.

---

## 4. SECTION C — THE DECORRELATION QUESTION

The old objective selected for signals that do not overlap. Overlap **is** depth, and depth is the strongest persistence axis measured. But decorrelation produced a real result — the 448-signal persistent union collapsed to PF 1.82 while the curated 50 held PF 6.40 (record figures). Something in it works.

Two properties were conflated:

- **(i) ENTRY CO-FIRING** — signals firing together, creating depth. **WANTED.**
- **(ii) FAILURE CORRELATION** — signals losing together, concentrating risk. **NOT WANTED.**

These are not in conflict. A book can co-fire heavily while failures stay independent. The old method collapsed them into one axis and optimised the wrong one.

### C.1 — Measuring entry co-firing

For a candidate book *B* and every ordered pair (i, j) in *B*:

```
cofire(i,j) = |bars where i and j both qualify| / |bars where i qualifies|
```

computed on the **pre-jar qualifying masks** (S8B basis 2), not the executed trade list, so the jar's 6-lot cap does not truncate the measurement. Executed max depth is 6; pre-jar qualifying depth reaches **14**. Book-level statistic:

```
CoFire(B) = mean over pairs of cofire(i,j)
DepthYield(B) = count of same-direction clusters of size >= 5 the book produces per trading day
```

`DepthYield` is the objective-relevant quantity — co-firing matters only insofar as it produces depth.

### C.2 — Measuring failure correlation, separately

Build the per-signal daily-loss series from S6's regenerated `signal_per_day_pnl.jsonl` (§B). For each pair, on days where **both** signals traded:

```
failcorr(i,j) = Pearson correlation of the two daily-loss series, restricted to days where both traded and at least one lost
```

Book-level:

```
FailCorr(B) = mean pairwise failcorr
FailConc(B) = the book's worst single-day loss expressed as a multiple of its mean daily loss
```

`FailConc` is the survival-first quantity and is the one the FTMO ceiling cares about. It is reported alongside `FailCorr` because a low mean correlation can still admit one catastrophic co-failure day.

### C.3 — The objective

**Maximise `DepthYield(B)` subject to `FailConc(B)` and survival constraints, with `FailCorr(B)` as a reported penalty rather than a hard bound.**

Formally, the build implements a lexicographic objective consistent with survival-first doctrine:

1. **Hard constraint:** worst modelled day within the FTMO ceiling with stated margin. Any book violating this is rejected regardless of other properties.
2. **Hard constraint:** `FailConc(B) <= F_max`, with `F_max` set from the committed book's measured value as the reference point, not tuned.
3. **Maximise:** `DepthYield(B)`.
4. **Tie-break:** lower `FailCorr(B)`.

**This inverts the old objective on axis (i) while preserving it on axis (ii).** Co-firing is now rewarded; correlated failure is still penalised.

*Motivating numbers:* the depth ladder is monotonic on cluster size — bands 1-2 WR ~87%, PF 2.33–2.83; band 5-7 WR 95.5%, PF 11.39; band 13+ WR 96.0%, PF 11.78, worst day **−$12.3** (N=10, trade-level). Pre-jar qualifying depth 5-6 reaches PF 87.58 at WR 98.6%. Solo (depth-1) is PF 3.16 with a −$574 worst day.

*What would falsify the inversion:* if a book selected for high `DepthYield` shows `FailConc` materially worse than the committed book at equal net, co-firing and correlated failure are not separable in practice and the two-axis model is wrong. The build must report both axes for every candidate book so this is visible rather than assumed.

*Fit risk:* `F_max` anchored to the committed book's measured value imports that book's six-month history as the reference. Stated openly; the alternative (a free parameter) is worse.

---

## 5. SECTION D — THE REACH PROBLEM

**Resolved first, per §1.2 and §1.3:** the book captures large *absolute* moves (traded Q4 at percentile 88.9 of absolute forward move) while sitting at ordinary *ATR-normalised* multiples (percentile 72.6). The missed thrust set is tradeable (median 58–78 pt) but systematically smaller than what the book already takes (168–224 pt). Only 4.4–5.4% of thrust episodes are touched.

Reach is therefore a **real and large opportunity in count**, addressing a **smaller-move population** than the book currently trades. Both halves must govern the design.

### D.1 — Reach must not relax quality

The acceptance bar for a signal recruited for coverage is **identical** to the bar for any other signal: §H.1 empirical-null, §H.2 stability, §H.3 regime-conditional positivity, and the §C.3 survival constraints. Coverage is a **tie-breaker among qualifying signals**, never a substitute for qualification.

Concretely, coverage enters as step 4 of the §C.3 lexicographic objective:

3. maximise `DepthYield(B)`
3b. **among books within a stated tolerance of the best `DepthYield`, prefer the book with higher `Coverage(B)`**
4. tie-break on `FailCorr(B)`

### D.2 — Defining Coverage

```
Coverage(B) = fraction of thrust episodes (basis 3) that book B touches with >= 1 entry
```

reported per (W, K, E) grid cell, never at a single parameter setting. Reference value: the committed book's measured 4.4–5.4%.

Because missed episodes are smaller, `Coverage` is additionally reported **stratified by episode absolute size** (<50 pt, 50–100 pt, 100–200 pt, >200 pt) so that coverage gains in the small-move stratum are never presented as equivalent to gains in the large.

### D.3 — S8B coverage columns as a selection input

`cov_book_traded`, `cov_book_missed`, `cov_missed_share` become inputs to candidate ranking, subject to three constraints established in the S8B review:

1. **Lift, not raw rate.** Raw participation is confounded by fire frequency; cluster-span coverage of the eligible universe is 1.18–2.28% at size≥5, while conditions fire at a median 20.0% of eligible bars. Rank on `lift`, with the absolute participating-fire count reported alongside.
2. **ATR-controlled.** The volatility-proxy check must be applied. Its necessity is measured: `ATR_1M:hi` lift collapses 4.118 → 1.302 under ATR control, `Bar_Range:hi` 3.937 → 1.244, `Volume:hi` 4.524 → 2.243. Conditions that rise under control (`Slope_Accel_LT:hi` 3.907 → 5.814, `OR_Low_Side:==-1` 4.434 → 4.979) are the informative ones.
3. **Single conditions are not signals.** The vocabulary is single conditions; the book's signals are triples. No book may be selected from the S8B CSV. It ranks *ingredients*, and the triple built from them is scored on its own merits through the full funnel.

*What would falsify the reach programme:* if signals recruited for coverage qualify under §H but their clusters show materially worse depth-ladder behaviour than the incumbent population at equal size band, the missed set is structurally lower-quality and reach should be abandoned in favour of deepening existing coverage. **The measurement that decides this is the depth ladder computed separately for reach-recruited and incumbent clusters.** The build must produce it.

*Fit risk:* the thrust definition's W, K, E are chosen on six months. The grid requirement (never a single cell) is the mitigation. Basis 3 is forward-looking by construction and is **selection-side only** — it may never become a live gate or entry condition.

---

## 6. SECTION E — GATES

### E.1 — Unchanged

`ADX >= 15` and `Volume > 50` (**strictly greater** — `portfolio_simulation_engine.py` L146, `dots_thresholds.py` L87; the record's "ticks >= 50" is imprecise) stay as minimal-participation filters removing dead bars. The reveal doc §5 measured ADX≥15 as redundant in practice — every trade already clears it. They are a floor, not an edge, and are not to be presented as one. D2D directional agreement is existing and unchanged.

### E.2 — The AT gate: not adopted, on evidence

Per §1.1, the directional AT gate as recorded in decision 2 is **not supported**:

| population | ungated PF | AT_Regime_ST directional-aligned PF | sign(AT_Slope_ST)-aligned PF |
|---|---|---|---|
| F0 concurrent, full series | 9.76 | 6.85 | 6.97 |
| whole book, NEW segment | 2.19 | **0.97** (net −$111) | — |

The directional gate removes profitable trades. `sign(AT_Slope_ST)` is the panel-true variable (DOT.cs L10800; the panel's "Direction" row reads `sign(detectedSlope_ST)`, and `detectedSlope_ST = hist_detectedSlope_ST[i]` at L3154, which exports as `AT_Slope_ST`) — the right variable was tested and gave the same non-result.

**Encoding, for the record:** `AT_Regime_ST == 0` is BULLISH (DOT.cs L3102 `curr_AnchorType_ST = (st_Slope > 0.0) ? 0 : 1`, corroborated L3113). It is a **latched anchor**, updated only when a slope sign change coincides with a qualifying flip (L3110); 96.98% agreement with `sign(AT_Slope_ST)` on nonzero-slope bars, and 100% of the 5,302 disagreements fall within 3 bars of a flip. Additionally `UseTrendAnchor=false` (L1584), so the exported `AT_Slope_ST` is the **raw per-bar** regression slope, not a latched value — it changes on 88.5% of bars.

### E.3 — The regime-state variant: measured, and it does not persist

The operation the record actually measured (`AT_Regime_ST == 1`, irrespective of direction) is genuinely interesting on the NEW segment. It does not survive regime-conditional examination:

| segment | ungated | reg==1 | reg==0 |
|---|---|---|---|
| OLD | PF 6.25, $89,797 | PF 6.33, $52,750 | PF 6.14, $37,047 |
| NEW | PF 2.19, $8,407 | **PF 3.83, $9,228** | PF 0.78, −$821 |

Monthly (whole book, net $):

| month | reg==1 | reg==0 |
|---|---|---|
| 2026.01 | PF 13.51, 3,095 | PF 5.33, 1,920 |
| 2026.02 | PF 5.78, 11,721 | **PF 6.48, 7,698** |
| 2026.03 | PF 6.69, 15,276 | PF 5.24, 10,185 |
| 2026.04 | PF 6.70, 5,984 | PF 5.79, 7,278 |
| 2026.05 | PF 10.81, 9,803 | **PF 11.03, 4,040** |
| 2026.06 | PF 3.88, 9,744 | **PF 4.47, 6,015** |
| 2026.07 | PF 3.72, 6,355 | PF 0.71, **−910** |

`reg==0` outperforms on PF in **three of seven months**. On the OLD segment the filter is essentially neutral (6.33 vs 6.25). **The entire effect is one month — July.** This is precisely the pattern §H.3 exists to catch, and it fails that test.

**Specified decision: the regime-state filter is NOT adopted as a gate.** It is retained as a **measured conditioner** entering §H.3's regime bucketing as a candidate bucket definition, where its behaviour can be observed without it filtering anything.

*What would establish it:* positive differential in ≥6 of 7 monthly buckets and in both volatility terciles, surviving §H.1 at the expanded trial count. The build reports this; the answer today is no.

### E.4 — The long-term adaptive family: an open question, explicitly not decided

S8B causal-strata ranking places the **long-term** adaptive family at the top of basis-1 ATR-controlled lift:

| condition | controlled lift | PF in clusters touched | PF in clusters not touched |
|---|---|---|---|
| `Slope_Accel_LT:hi` | 6.40 | 17.26 | 9.08 |
| `AT_Score_LT:lo` | 5.06 | — | — |
| `Slope_EMA_LT:lo` | 3.59 | — | — |
| `AT_Slope_LT:lo` | 3.46 | — | — |

**Every AT gate tested to date has been short-term.** The LT family has never been tested as a gate.

**Specified measurement, not a decision:**
1. Score the committed book gated on each LT directional state, both encodings tested explicitly (the ST encoding is inverted; the LT encoding must be verified against DOT.cs, not assumed to match).
2. Report per §H.3 buckets, never aggregate — this is what would have caught §E.3.
3. Report on F0 concurrent separately from whole book, since §1.1 shows the two populations respond oppositely.
4. Report against all three S8B bases, since basis 1 is book-derived and high lift there partly means "resembles the incumbents."

**Two caveats stated in the spec so they cannot be lost:** these are single conditions, not triples; and basis-1 lift is confounded with incumbency. High basis-1 lift is necessary but not sufficient evidence.

*Fit risk:* the LT family surfaced from a ranking computed on these six months. Testing it as a gate on the same six months is circular. It must be evaluated inside the §I walk-forward, selected on A and tested on B.

---

## 7. SECTION F — DEPTH AND SOLO ENTRIES

### F.1 — The measured facts

- Solo entries are **profitable**: 1,135 trades, WR 88.5%, PF 3.16, **+$26,710** full series. Early entries in clusters that stayed shallow: WR 86.4%, PF 2.62, +$12,561.
- They are **structurally fragile**: avg loss 2–3× avg win, breakeven-WR 64–75% vs 39–52% concurrent, and they carry the tail — the −$574 worst day and the July bleed (2026.07 solo: WR 70.1%, PF 0.64, **−$1,208**).
- The first entry of a large cluster is **indistinguishable from a solo at fire time.** I measured this and found no predictor. Early entries (position 1–2) of clusters that became size≥8: WR 92.2%, PF 5.31, avg **$43.7**; of clusters that stayed 1–2: WR 86.4%, PF 2.62, avg **$17.3** — better, but not separable *before the second entry arrives*.
- Waiting for depth destroys the edge: take-all book net **$77,239**; commit only at running-depth≥3 → **$45,430**; at ≥5 → **$28,004**.

### F.2 — Depth enters SIZING, not selection-time filtering

**Specified: depth is a sizing input applied live as the cluster develops, not a pre-trade filter.** A depth filter costs 41–64% of book net (F.1) to buy a WR improvement, and discards the front of every large move.

The base position opens on the existing trigger with no depth pre-condition; size scales as running depth crosses stated thresholds. Motivating ladder (pre-jar qualifying depth, executed trades bucketed by their entry bar's qualifying depth):

| qualifying depth | trades | WR% | PF | worst day $ |
|---|---|---|---|---|
| 1-2 | 2,008 | 89.3 | 3.52 | −639.1 |
| 3-4 | 417 | 95.7 | 16.79 | −138.9 |
| 5-6 | 141 | 98.6 | 87.58 | **+8.4** |
| 7-9 | 67 | 100.0 | — | +28.8 |
| 10+ | 45 | 100.0 | — | +83.4 |

**Recorded decision 1 ("solo convergence is excluded from the forward book") is a selection-side statement and is preserved as such:** the forward book need not *contain* signals whose only mode is solo firing. But an *executed* first entry of a developing cluster is not a solo and must not be blocked at fire time — it cannot be distinguished from one, and blocking it costs the front of every large move. The distinction is between **which signals enter the book** (selection) and **which entries are taken and at what size** (runtime). This spec keeps them separate per §F.3.

### F.3 — The coupling constraint (binding)

Selecting a signal *because* it fires at shallow depth in episodes that deepen, and then *sizing* by depth, fits the book to its own sizing mechanism. The measured pull is not small ($43.7 vs $17.3 on the same trigger).

**Mandatory mitigations, all three:**
1. The depth→size curve is **fixed a priori** and never tuned against the selected book. It is scored as an S7 contender row (§B) against C0–C5, independently of selection.
2. Selection-side depth metrics use **basis 3** (price-anchored) wherever available, since basis 3 is defined without reference to the book or the jar.
3. Selection and sizing are validated on **different splits** of the §I walk-forward.

*Falsification:* if the depth-sized book's advantage over flat sizing disappears when the curve is fixed a priori rather than fitted, the ladder's economic value was an artifact of the fitting.

### F.4 — Minimum unique-variable count at depth

Measured depth-2 events exist carrying only **3 unique variables** — the same read counted twice, which is not corroboration.

**Specified:** define `unique_vars(cluster)` = count of distinct underlying variables across all conditions of all entries in the cluster, computed after §G.1 duplicate-collapsing. A cluster counts toward `DepthYield` (§C.3) only if `unique_vars >= U`.

`U` is **not chosen in this spec.** The build reports the depth ladder recomputed at `U ∈ {3, 4, 5, 6}` and the operator sets it on the evidence. Setting `U` here would be fitting a threshold to six months without seeing the curve.

---

## 8. SECTION G — VOCABULARY HYGIENE

**Verified exact duplicate pairs** (5): `OBVf_Signal:==1` ≡ `OBVf_Trend_Dir:==1`; `OBVf_Signal:==-1` ≡ `OBVf_Trend_Dir:==-1`; `Trend_Concordance:==0` ≡ `Trend_Conflict:==1`; `Trend_Concordance:==1` ≡ `Trend_Conflict:==0`; `PrevDay_Close_Side:==0` ≡ `DailyOpen_Side:==0`.

**Verified zero-firing conditions** (7): `D2D_Dn_Count:lo`, `D2D_Dynamic_Sensitivity:hi`, `D2D_Up_Count:lo`, `AT_Lookback_LT:lo`, `AT_Lookback_ST:lo`, `OR_High_Dist_ATR:lo`, `OR_Low_Dist_ATR:lo`.

Effective live vocabulary ≈ **237**, not 249. **0 of the 50 committed triples are affected.**

### G.1 — Handling, at SELECTION time only

`build_condition_pool` is S.10 sacred and is **not modified**. All handling occurs downstream, in the selection layer, against the pool the oracle produces.

1. **Duplicate collapsing.** Maintain a canonical-equivalence map, built by measuring bitwise mask identity on the stitched series (not hardcoded — re-derived each run, since equality-arm membership is data-dependent). For ranking and for triple formation, the members of an equivalence class count as **one** condition. A triple containing both members of a pair is a two-condition signal and must be **rejected at formation**, not discovered and then explained.
2. **Dead conditions.** The 7 zero-firing conditions are **excluded from RANKING and from triple formation**, and are **NOT deleted from the vocabulary.** They are reported in the run report with their fire count of 0. If a future dataset makes one live, it re-enters automatically — the exclusion is a computed property of the run, never a hardcoded list.
3. **Trial-count impact.** The effective vocabulary of 237 (not 249) is the number entering §H.1's trial count. Duplicates manufacture false corroboration in ranking; collapsing them is also what makes the multiple-testing correction honest.

*Fit risk:* none material — this removes an artifact rather than adding a choice. The one judgment is treating exact mask identity as equivalence; near-duplicates (correlation 0.99) are **not** collapsed, and the build reports the pairwise mask-correlation distribution so the operator can see how much near-duplication remains uncollapsed.

---

## 9. SECTION H — QUANT-DESK RIGOUR

The historical funnel ran 1.7M → 89K → 51K → 1,788 → 50 with **no trial-count adjustment**, and that was one family. §A expands to fourteen.

### H.1 — Multiple-testing correction

**Primary method: empirical null via matched random signals.** Parametric corrections (Bonferroni, and to a lesser degree BH) assume a tractable dependence structure; the candidate triples here are massively correlated (shared conditions, shared bars), so a parametric bar is either wildly conservative or wrong. The project already has the right machinery: the 400-random-triple baseline that established the 27% random-persistence rate.

Specified:
1. Generate `M >= 10,000` random signals from the **same** post-§G vocabulary, **matched on fire-rate distribution** to the real candidate pool (unmatched random signals are a weaker null — a rare random signal is not comparable to a rare real one).
2. Score every one through the **identical funnel**, including the §H.2 and §H.3 stages.
3. The acceptance bar is the **q-quantile of the null distribution** of the selection statistic, with `q` set so the expected false-acceptance count across the whole expanded funnel is `<= 1`. The build reports `q`, `M`, the realised null distribution, and the implied bar.

**Secondary cross-check: Benjamini–Hochberg FDR at q = 0.10**, applied with the family as strata (14 strata), reported alongside. Where the two disagree, the empirical null governs and the discrepancy is reported.

**Effective number of independent tests.** The naive trial count is not the effective one. The build computes and reports:
- naive trial count per family and in total;
- effective count after §G.1 duplicate collapsing (237 not 249 conditions);
- an eigenvalue-based effective-dimension estimate on the condition-mask correlation matrix, so the correlation-induced reduction is measured rather than guessed.

*Fit risk:* the null is generated on the same six months. It shares the period's structure, which is the point — it is a null for *this* market, not a universal one. Stated openly.

### H.2 — Stability selection

Specified per Meinshausen–Bühlmann, adapted to time-series structure.

- **Resampling scheme: day-level block bootstrap.** Draw whole **trading days** with replacement. Naive iid bar resampling is forbidden — it destroys intraday autocorrelation, session structure, and cluster formation (clusters span up to 80 bars; sessions span ~1,400). Day-level blocks preserve all three.
- **Subsample size:** 80% of the 123 available trading days per draw.
- **Count:** `B = 200` draws. Reported with a convergence check (selection frequencies stable between B=100 and B=200); if not converged, increase B rather than accept.
- **What is re-run:** the **entire** selection funnel on each draw — not a re-score of a fixed book. This is the point of the exercise.
- **Retention threshold:** a signal is retained only if selected in `>= 70%` of draws. Sensitivity at 60% and 80% reported so the threshold's influence is visible.

*Falsification:* if fewer than a workable number of signals clear 70%, the selection process is unstable and the book is not ready — that is a valid and reportable outcome, not a reason to lower the threshold.

*Fit risk:* 123 trading days is a small pool for 200 draws; draws overlap heavily. The convergence check and the sensitivity band are the mitigations; the limitation is real and stated.

### H.3 — Regime-conditional persistence

Aggregate positivity hides regime failure. This is what would have caught the short-side collapse, and it is what disqualifies the §E.3 regime-state filter.

**Primary bucketing: calendar month** — 7 buckets (2026.01 partial, 02, 03, 04, 05, 06, 07 partial). Partial buckets are flagged and reported but **not excluded**.

**Secondary bucketing: causal volatility tercile**, derived from the oracle's mechanism-D `ATR_1M` thresholds (never a local full-sample quantile — that is a mechanism-A surface, and the S8B remediation established this precedent).

**Tertiary bucketing (reported, not gating): `AT_Regime_ST` state**, per §E.3.

**Requirement:** a signal must be **positive in each bucket**, not in aggregate. Specified bar: net > 0 in `>= 6 of 7` monthly buckets **and** net > 0 in **all** volatility terciles. The 6-of-7 allowance (rather than 7-of-7) acknowledges the two partial months; the volatility requirement is strict because terciles are balanced by construction.

*Motivating number:* the §E.3 filter is positive in aggregate on every segment and still fails, because `reg==0` beats `reg==1` in 3 of 7 months and the whole effect is July. Aggregate would have passed it.

---

## 10. SECTION I — WALK-FORWARD ON THE SELECTION PROCESS (MANDATORY)

All prior validation — blind audit, OOS May–Jun, 6/6 folds, decorrelation, null/shuffle — sat **inside** Jan–Jun, downstream of a funnel already run across that whole window. **The book was validated; the method never was.**

### I.1 — Construction

- **Splits: 4**, contiguous and chronological across 2026.01.19 → 2026.07.21.
- **Scheme: anchored walk-forward.** Split *k* selects on all data up to boundary *k* and tests on the untouched segment between boundary *k* and *k+1*. Anchored rather than rolling because the warm-up floor (6,900 bars) consumes a fixed prefix and a rolling window would repeatedly pay it.
- **Embargo: 1 full trading day (~1,440 bars) between train and test.** Required, not optional: the §D thrust labels use forward windows up to W=60 bars, trades hold for a bounded but nonzero duration, and cluster spans reach 80 bars. Without an embargo the training window's forward labels peek into the test segment.
- **Test segments are touched exactly once**, at the end, to produce the reported number.

### I.2 — What is re-run on each split

**The ENTIRE selection funnel across ALL families** — S3 discovery through §H.1 correction, §H.2 stability, §H.3 regime conditioning, and final book assembly. Every parameter that the process would set is set *inside* the training segment: thresholds, `U` (§F.4), `F_max` (§C.3), the null bar (§H.1), the retention threshold sensitivity, and the thrust grid (§D).

**Nothing may be carried across the boundary except the code itself.**

### I.3 — Pass criterion

Signal-level persistence into the held-out segment, measured as the record measures it (net > 0, PF >= 2, WR >= 75 in both train and test):

- **Current baseline: 50%** (26/52 entities).
- **Random-triple null: 27%** (4/15) — and this null must be **re-run on each split**, not carried from the record, because the null is period-dependent.
- **Target: materially above 50%**, specified as **>= 65% on the mean of the 4 splits, with no single split below 50%.**

The single-split floor matters as much as the mean: a process that scores 80/80/80/20 is not a working process, and the mean alone would hide it.

**Additionally:** the held-out books must satisfy the §C.3 survival constraint on their test segments. A book that persists but breaches the daily ceiling fails.

### I.4 — What constitutes a FAKE step (rejection list)

Any of the following invalidates the walk-forward and must be rejected by the Auditor:

1. **Re-scoring a fixed book on held-out data.** That is book validation. It is what has already been done and is not this test.
2. **Any parameter chosen with sight of a test segment** — including thresholds, `U`, `F_max`, the thrust grid, bucket boundaries, or the retention threshold.
3. **Running discovery across the full series and splitting afterwards.** The funnel must run inside each training segment.
4. **Omitting the embargo**, or setting it shorter than the longest forward-looking label in use.
5. **Reporting the best split, or the mean without the per-split values.** All four splits are reported.
6. **Re-running a failed split after adjustment.** If a split fails, the iteration happens and the *whole* walk-forward re-runs from clean code — the failed configuration is reported, not buried.
7. **Carrying the 27% null from the record** instead of regenerating it per split.
8. **Deleting rows anywhere** to construct a segment. Segments are index ranges over the intact series; the oracle's rolling-2500 ring must see the unbroken data.

*Fit risk, stated plainly:* four splits over six months gives training segments of roughly 1.5–5 months and test segments of ~1.5 months. These are short. A pass is meaningful evidence that the method generalises; it is not proof that it generalises across market regimes not present in 2026 H1. The honest resolution is a second out-of-sample export, which remains the project's highest-value un-run validation.

---

## 11. SCRIPTS THAT CHANGE

Sacred five stay **byte-locked**: `dots_thresholds.py` `518862bf19fb`, `wf.py` `793e6e5f8d9a`, `core.py` `6530e2508b17`, `portfolio_simulation_engine.py` `bb498eb13ce3`, `conviction.py` `27af7acee824`. `verify_sacred()` must still abort on drift. `build_condition_pool` (S.10) is not modified (§G.1).

| script | change |
|---|---|
| `master.py` | new stages: **S3B** family evidence review (§A.1–A.5), **S5B** selection layer (§C, §D, §G, §H). S7 gains the depth-sized contender row (§F.3). S9 report surfaces the §A.1 table and §H decisions. New sha; fresh Auditor pass required. |
| `engine/cluster_profiler.py` | extend to admit cross-family entries with origin tags (§A.4); emit family-composition vectors per cluster; stratify coverage by episode absolute size (§D.2). |
| `scanners/concurrence_profiler.py` (F12) | unchanged pending the §A.2 measurement; classification may change on the result. |
| **new** `engine/selection.py` | implements §C.3 objective, §D.2 coverage, §G.1 collapsing, §H.1–H.3. The single place selection logic lives, so the Auditor has one file to verify. |
| **new** `engine/wf_selection.py` | implements §I. Must not import from `selection.py` in a way that lets a test segment influence a fitted parameter; the split boundary is enforced structurally, not by convention. |
| `discovery_map.md` | §3 line 90 F10 flag correction (§1.4). |
| `DOT_new_data_reveal_2026-07-21.md`, `DOT_progress_and_rd_plan.md` | §1.1, §1.2 corrections. Decision 2 to be restated. |

Everything runs under `master.py`. No standalone scripts.

---

## 12. GROUNDED vs PROPOSED

**Grounded in measurement taken this session, reproducible through the ratified path:**

- §1.1 the record's §5 table is a regime-state filter — reading C reproduces every published digit; the directional gate gives PF 0.97 / −$111 on NEW.
- §1.2 the 77% / thrust-overlap reconciliation — Q4 at percentile 88.9 absolute vs 72.6 ATR-normalised; traded median 2.56 disp/ATR vs all-eligible 3.40.
- §1.3 missed episodes are tradeable (median 58–78 pt) but 2.5–3× smaller than traded (168–224 pt); 4.4–5.4% of episodes touched.
- §E.3 the regime-state filter's effect is one month; `reg==0` wins on PF in 3 of 7 months; OLD-segment differential is 6.33 vs 6.25.
- §F.1 solo economics, the non-separability of first entries, and the cost of waiting for depth ($77,239 → $45,430 → $28,004).
- §F.2 the pre-jar depth ladder.
- §G the 5 duplicate pairs and 7 dead conditions.
- The depth ladders and cluster statistics quoted throughout, on all three bases.

**Reasoned proposals awaiting a first run — no result is claimed for any of these:**

- §A.1 the family evidence table and every verdict in it. Nobody has looked; the outcome is unknown.
- §A.2 whether F12 is duplicate, independent or superseded.
- §A.4 whether mixed-family clusters form at all, and whether they behave like same-family clusters. This is a genuinely open question with no prior measurement.
- §C.3 that `DepthYield` and `FailConc` are separable in practice. The two-axis model is motivated but unproven.
- §D.3 that reach-recruited signals qualify under §H and that their clusters behave. §1.3 gives grounds for caution — the missed population is smaller.
- §E.4 whether the LT adaptive family carries gate value. The S8B lift is real; its meaning as a gate is untested.
- §F.4 the value of `U`. Deliberately not chosen.
- §H.1–H.3 that a book survives the corrected funnel at all. It is entirely possible that few or no signals clear the empirical-null bar plus 70% stability plus 6-of-7 regime positivity. **That outcome must be reported as a result, not treated as a failure requiring the bar to be lowered.**
- §I that the method passes at ≥65%. Unknown. The current 50% against a 27% null is the honest starting point.

**One standing limitation applies to everything above:** six months, one instrument, two partial months, one dominant crash month. The direction of the depth findings survives three independent measurement methods and both cluster tolerances; the magnitudes do not have out-of-sample support. A second export remains the highest-value un-run validation in the project.
