# QUANT — SIX-PHASE ANALYSIS OF THE COMPLETED RUN

This produces the evidence the operator composes the final engine from. It is the last
analytical work before a live system, and it is the reason the last three weeks happened.

All artifacts are in project files. **Phase 0 is done** — the Supervisor verified every
headline figure reproduces: 39,308 catalogue rows across seven families, F0 at 1,840,
terrain 7,490 with 1,143 UP / 1,155 DOWN reachable, unclaimed 1,037/1,055, pass criterion
mean 2.1653 / min 1.5083 / lb95 1.4026 / FAIL. You do not need to re-verify those, but
re-derive anything you build on.

---

## MEASUREMENT PROTOCOL — BINDING, APPLIES TO EVERY PHASE

1. **EVERY FIGURE CITES ITS SOURCE**: file, column, and the filter applied. "Median PF is
   3.2" is worthless; "median `agg_pf` over `catalogue_F0.csv` rows with `verdict==VALID`
   and `direction==LONG` is 3.2 (n=1,104)" is a measurement.

2. **NEVER CARRY A NUMBER FORWARD FROM THIS BRIEF OR FROM MEMORY.** Every figure you use is
   re-derived from the file in front of you, this turn. Figures quoted here are context,
   not inputs.

3. **MEASURE, DO NOT RECOMMEND.** The operator composes the book. Your job is to make the
   choice informed, not to make it. Where a decision is genuinely his, say so and give him
   the numbers both ways.

4. **A COLUMN NAME IS NOT A DEFINITION.** If what a column contains is ambiguous, state the
   ambiguity and what you assumed, or open the producing code. Do not infer meaning from the
   name — that is the failure mode that has cost this project most.

5. **RULE 5**: a negative conclusion carries the same burden of proof as a positive one. "No
   relationship" needs the same evidence as "strong relationship". And a manufactured concern
   is as much a breach as a missed one.

6. **NO DEFERRALS.** If a phase cannot be answered from the files, say WHICH FILE OR
   MEASUREMENT would answer it. Do not say "further analysis needed" and move on.

7. **PHASES GATE EACH OTHER.** Phase 1's answer sets Phase 3's cut-off. Phase 2's answer
   determines whether VALID itself needs changing. Do not run them independently and staple
   the results together.

8. **SAMPLE SIZE WITH EVERY CLAIM.** n, and a confidence interval where the claim rests on a
   proportion. The 100%-WR-at-n=16 error has been made in this project before.

---

## PHASE 1 — WHERE DOES ADDING SIGNALS STOP HELPING?

**Files:** `dilution_curve_agg_pf.csv`, `dilution_curve_EXPECTED_ROWS_AT_OR_ABOVE_THIS_PF.csv`

39,260 admission steps each, two ranking keys. Columns: `admitted`, `signal_id`,
`ranking_key`, `same_bar_ge3_bars`, `tolerance_N`, `depth`, `population`, `basis`.

**THE QUESTION:** as signals are admitted best-first, at what point does `same_bar_ge3_bars`
stop growing usefully — and does the answer differ between the two keys?

- Describe `same_bar_ge3_bars` against `admitted` for both curves.
- Identify the knee: where marginal gain per admitted signal collapses.
- **THE GAP BETWEEN THE TWO CURVES IS THE OVERFIT ESTIMATE.** Quantify it. If ranking by raw
  PF and ranking by chance-adjusted price produce materially different curves, the difference
  is what PF-ranking buys you that is not real.
- Note: 39,260 not 39,308 — the 48 `UNEVALUABLE` rows cannot enter an admission curve.
  Confirm that is the whole discrepancy.
- `selection_depthyield_grid.csv` (35 rows, tolerance × S) is available for interpreting the
  curve at different N and S.

**OUTPUT:** a number. "Beyond N signals, depth 3+ stops being selective." With the uncertainty
on N, and separately per key.

---

## PHASE 2 — WHY DID THE WALK-FORWARD FAIL?

**Files:** `wf_pass_criterion.csv`, `wf_null_arm_summary.csv`, `wf_per_segment_rederivation.csv`,
`wf_splits.csv`, `wf_null_arm_entities.csv`

    split 0: 15,328 admitted →  7,962/11,816 → 0.6738  vs null 0.2360 = 2.86x
    split 1: 23,832 admitted →  9,712/19,176 → 0.5065  vs null 0.2375 = 2.13x
    split 2: 34,496 admitted → 11,303/28,901 → 0.3911  vs null 0.2593 = 1.51x

The ratio decays as VALID admits more. **TWO COMPETING EXPLANATIONS AND THEY HAVE DIFFERENT
REMEDIES:**

- **(a) DILUTION** — later-admitted signals are weaker, so the population degrades.
  *Remedy: tighten VALID.*
- **(b) REGIME** — the third test window is simply harder, and the same signals would have
  failed there whenever admitted. *Remedy: none; it is a property of the period.*

**DECOMPOSE IT.** Within split 2, separate signals that were ALSO admitted in split 0/1 from
those FIRST admitted in split 2, and compare persistence rates. Dilution predicts the
newly-admitted persist **WORSE**. Regime predicts they persist the **SAME or BETTER**.

This exact decomposition was run once before on a small pool and found the marginal signals
persisted BETTER — the opposite of dilution. It needs re-asking at real scale.

Also: the training segments are nested (anchored), so split 2's train contains split 0's.
State whether that nesting confounds the comparison and how you handled it.

**OUTPUT:** which explanation the data supports, with the decomposition table, or a statement
that three splits cannot distinguish them — with what would.

---

## PHASE 3 — WHICH SIGNALS ARE DIAMONDS?

**Files:** `catalogue_F0.csv` (1,840), then F1 (37,276), F3 (53), F9 (133), F2/F4/F11

44 columns. The pricing block is: `n_trials_family`, `null_valid_rate_family`,
`expected_valid_by_chance_family`, `pf_null_p50/p90/p99_family`, `pf_null_exceedance_pct`,
`EXPECTED_ROWS_AT_OR_ABOVE_THIS_PF`, `q_value_BY_family`, `n_null_family`,
`null_matched_fraction`, `null_rejected_out_of_band`, `null_direction_long_share`, `null_seed`.

**THE QUESTION:** how many signals survive being priced against chance?

- Rank by `EXPECTED_ROWS_AT_OR_ABOVE_THIS_PF` ascending. How many rows have an expected count
  **below 1** — i.e. rows chance essentially does not produce? Per family, per direction.
- Cross-check against `q_value_BY_family`. Do the two agree on which rows are exceptional?
  Where they disagree, say why.
- Then check persistence: `folds_plus`, `min_fold_pf`. A row that is both cheap-by-chance AND
  holds across folds is the diamond. Report the intersection size.
- **REPORT `null_matched_fraction` PER FAMILY.** If a family's null matched poorly, its pricing
  is weaker and every row in it should be read with that caveat. F0 ran K=500; others 200.
- Both arms are present: `trades/WR/agg_pf/worst_day_usd/net` ungated, and
  `gated_trades/gated_WR/gated_PF/gated_worst_day_usd/gated_net/gated_delta_net`. Report the
  gate's effect per family — the operator sets gating per signal, not book-wide.
- **IMPORTANT:** `catalogue_F0` is 1,840 rows against BOOK-50's 48. State plainly how many
  clear the bar, because the operator's scaling target is 72–100 F0 signals.

**OUTPUT:** the shortlist size per direction per family, and the intersection of
cheap-by-chance AND fold-persistent.

---

## PHASE 4 — DOES MIXED-FAMILY CONVERGENCE HOLD?

**Files:** `same_bar_cohort.csv`, `cross_family_cofiring.csv`, `cluster_basis_summary.csv`

On BOOK-50 the mixed F0+F1 same-bar 3+ cohort held at PF 29.35 against all-F0's 35.72 (n=38,
corrected frame). That was a floor on 2 F1 signals, not an answer.

**THE QUESTION:** at pool scale, which families land together on a bar, and does a mixed
cohort keep the edge?

This decides whether F1's 37,276 rows are **FUEL** for the convergence engine or **NOISE**
beside it. It is the single biggest open question about the catalogue's composition.

State the cohort table's population and basis explicitly — it is counts, not P&L, by design
(depth-3 has no discriminating power at pool scale and P&L needs a book).
`cluster_basis_summary.csv` (12 rows) defines the three bases; read the cohort table against
the right one.

**OUTPUT:** the co-firing structure, and whether cross-family depth is achievable or whether
the families fire in disjoint places.

---

## PHASE 5 — WHAT IS REACHABLE THAT HE DOES NOT HOLD?

**Files:** `unclaimed_reachable.csv`, `terrain_hour_profile.csv`, `terrain_episodes.csv`,
`reach_D0_missed_decomposition.csv`, `reach_D02_D2_coverage.csv`

2,092 unclaimed (1,037 UP / 1,055 DOWN) of 2,298 reachable. Columns include
`n_conditions_firing` — and item 6 also specified `n_valid_triples_touching`.

**THE QUESTION:** how many of the 2,092 are **GENUINELY OCCUPIABLE**?

- `n_conditions_firing` separates a **SEARCH gap** (many conditions fire, no valid triple
  landed) from a **GRAMMAR gap** (few fire, nothing can express it). Split the 2,092 on it.
- If `n_valid_triples_touching` is present and populated, that is the sharper split — episodes
  with at least one valid triple touching them are reachable TODAY. If it is blank or zero, say
  so; it is only populated when F0 rows exist.
- Profile the unclaimed by `est_hour_start` and `displacement_pts`. Are they concentrated in a
  session, or spread?
- `reach_D0_missed_decomposition.csv` is the gate decomposition (why an episode is out of
  reach at all); `reach_D02_D2_coverage.csv` has 60 rows of stratum × direction coverage.

**OUTPUT:** the real headroom number. It will be well below 2,092 and that figure matters more
than 2,092 does.

---

## PHASE 6 — BALANCE ACROSS FOUR AXES

**Files:** `catalogue_F0.csv` + `terrain_hour_profile.csv` + `family_evidence.csv` +
`cluster_participation_profile.csv` + `selection_g2_domain_bridging.csv`

The operator's target: **direction (2) × market structure (9) × session (8) × regime (2)**.

**THE OBSTACLE, AND IT IS REAL:** the catalogue has NO `market_structure` column, NO `session`
column and NO `regime` column. All three are derivable from data already produced; none exists
today.

- **MARKET STRUCTURE:** nine categories from `DOT_signal_dictionary.xlsx` —
  TREND_CONTINUATION, MOMENTUM_IGNITION, STRUCTURAL_ENTRY, BREAKOUT_EXPANSION,
  SQUEEZE_BREAKOUT, PRICE_ACTION, TREND_EXHAUSTION, VOLUME_CONFIRMED, and **D2D** (the ninth,
  named in the dictionary's Name Key as the founding break-of-structure engine). Classify a
  discovered triple by the VARIABLES in its `signal_def`. Specify the mapping; do not build it.
  `selection_g2_domain_bridging.csv` (48 rows) shows BOOK-50's triples already classified by
  domain — a working reference for how this was done by hand.
- A triple can span two structures. **STATE THE AMBIGUITY** — primary label or both is the
  operator's call, not yours.
- **SESSION:** derivable from entry bars via the eight terrain session labels.
- **REGIME:** F12's causal labels.

**THE TENSION TO QUANTIFY RATHER THAN RESOLVE:** NY OPEN holds 225 of the largest-decile
episodes and overnight holds 0. A session-balanced book deliberately takes worse episodes to
be even. **MEASURE THAT COST.** The operator decides whether to pay it.

**OUTPUT:** the classification spec, the achievable coverage per cell, and the measured cost of
balance against the cost of concentration.

---

## WHAT YOU DO NOT DO

- **You do not select the book.** Item 15: the catalogue is emitted from VALID, never from an
  argmax, and nothing in this build chooses. That includes you.
- **You do not propose a quota, floor, target or expected composition.** Doctrine rule 3.
  Composition is an OUTPUT to be reported, never an INPUT to be constrained.
- **You do not choose N** from the tolerance grid. You report the curve.
- **You do not build.** Specify; the Developer builds.

---

## DELIVER

Phase by phase, in order, each with its answer stated as a number or a plain finding. Where a
phase cannot be closed, name the file or measurement that would close it.

**Do not summarise all six into a recommendation at the end.** The operator draws the
conclusions — that is the whole architecture of this rebuild.
