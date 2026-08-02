# QUANT — SIX-PHASE ANALYSIS OF THE COMPLETED RUN

This produces the evidence the operator composes the final engine from. It is the last
analytical work before a live system, and it is the reason the last three weeks happened.

**THE SOURCE RUN.** Every artifact in project files comes from one completed cold run: 10h43m
from an empty tree, every stage executing, no resumes, no manual intervention, 99.7% concurrent.
It reproduced a previous independent cold run byte-for-byte on ten of ten comparable artifacts,
including `wf_pass_criterion`. The pricing column is reproducible — a per-family seed defect that
made it vary between runs was found and fixed before this run.

Figures already verified by the Supervisor, given as context and not as inputs — re-derive
anything you build on: 39,308 catalogue rows across seven families, F0 at 1,840 (1,818 VALID),
terrain 7,490 with 1,143 UP / 1,155 DOWN reachable, unclaimed 1,037/1,055, pass criterion mean
2.1653 / min 1.5083 / lb95 1.4026 / **FAIL**.

**READ `cake_dictionary.md` SECTION 4B BEFORE STARTING.** It records what prior research
already established with numbers — the conviction constants and why Hurst is longs-only
(OOS PF 2.22 short vs 4.99 long), the jar mechanism and its simulated result, the Heart of the
Ocean tested to a documented negative, how BOOK-50 was actually selected, and the independent
auditor's 26L/19S from the identical pool. **Several questions you might otherwise open are
already closed. Do not re-discover them.**

---

## MEASUREMENT PROTOCOL — BINDING, EVERY PHASE

1. **EVERY FIGURE CITES ITS SOURCE**: file, column, filter. "Median PF is 3.2" is worthless;
   "median `agg_pf` over `catalogue_F0.csv` where `verdict=='VALID'` and
   `direction=='LONG'` is 3.2 (n=1,445)" is a measurement.
2. **NEVER CARRY A NUMBER FORWARD FROM THIS BRIEF OR FROM MEMORY.** Everything re-derived this
   turn.
3. **MEASURE, DO NOT RECOMMEND.** The operator composes. Where a decision is his, say so and
   give the numbers both ways.
4. **A COLUMN NAME IS NOT A DEFINITION.** If ambiguous, state what you assumed or open the
   producing code. Inferring meaning from a name is the failure mode that has cost most here.
5. **RULE 5**: a negative conclusion carries the same burden of proof as a positive one. A
   manufactured concern is as much a breach as a missed one.
6. **NO DEFERRALS.** If a phase cannot be answered, name the file or measurement that would
   answer it. Never "further analysis needed".
7. **PHASES GATE EACH OTHER.** Phase 1 sets Phase 3's cut-off. Phase 2 determines whether VALID
   itself needs changing.
8. **SAMPLE SIZE WITH EVERY CLAIM.** n, and a confidence interval where a claim rests on a
   proportion.
9. **PER DIRECTION, ALWAYS.** See finding B. Longs and shorts are structurally different
   populations; a blended statistic conceals the difference rather than summarising it.

---

# WHAT THE SUPERVISOR ALREADY MEASURED — FIVE FINDINGS THAT SHAPE THE PHASES

One illustrative book was built from `catalogue_F0.csv` and simulated on the real frame.
**It is not a selection** — item 15 stands, the operator composes — but it changed what several
phases should ask.

**All of it is in-sample, unscored (`score_book.py` has not run), and unpriced. Each gate below
was found by a search that was NEVER priced against a null.** Hypotheses to test, not results
to inherit.

**THE BOOK:** 100 F0 signals, ranked per direction separately by
`EXPECTED_ROWS_AT_OR_ABOVE_THIS_PF` ascending then `folds_plus` then `agg_pf`. 50L/50S. Full
conviction stack. Old S.15 jar — the engine jar fix is still open, step 17f. Gate: solo →
Hurst p90 + ticks≥300, dual → Hurst p90, 3+ → free.

    BOOK-50      3,101 trades  PF 4.81  $97,675  worst-day -565   37L/13S
    this book    3,105 trades  PF 5.62  $98,380  worst-day -626   50L/50S

## A. THE EROSION IS DIRECTIONAL, NOT DILUTION

                    solo    dual    3-4      5+
    LONG   PF       3.59    2.43    8.68   39.40    WR 97.4% at 5+
    SHORT  PF       3.52    2.49    3.88    3.38    WR 84.6% at 5+

**The long side is intact** — PF 39.40 at depth 5+ on 387 trades, BOOK-50's bulletproof tier at
scale. **The short side is flat** — convergence adds nothing; five shorts agreeing is worth no
more than one.

Blended figures hide this. "3+ PF 6.61" is 8.68 long / 3.88 short. "5+ PF 7.45" is **39.40 long
/ 3.38 short**.

## B. LONG AND SHORT ENTRIES ARE STRUCTURALLY DIFFERENT

`AT_Slope_ST` is the short-term log-regression slope. **NOT inverted** — verified in `DOT.cs`:
L3051 `GetBestFit_OnBar` → `st_Slope`, L3118 `hist_detectedSlope_ST[i]=st_Slope` raw, L843
exported. Positive = bullish. The inversion is in `AT_Regime_ST` (L3102, `st_Slope>0 ? 0 : 1`)
which **must never be used for direction**.

Frame-wide, `sign(AT_Slope_ST)` vs `D2D_Trend_Dir`: **81.1% agreement, corr +0.62**, neutral
excluded. Neutral is 0.83% of bars and 19 trades — negligible here, worth re-checking on a
longer export. **AT_ST leads D2D by a median 6 bars** (4,480 flips; p25 2, p75 20).

But at entry the agreement collapses, and only on longs:

    frame-wide                       81.1%
    LONG entries                     40.0%
    LONG entries at depth 3+         16.3%
    SHORT entries                    67.6%
    SHORT entries at depth 3+        64.9%

**Deep long convergence fires when AT has already turned against D2D — the pullback. Deep short
convergence fires when both agree — continuation.** Hurst does not change this: agreement is
81.1 / 81.4 / 81.5 / 81.7% at all bars, >p50, >p70, >p90. Flat.

An `AT == D2D` agreement gate keeps 16% of deep longs and drops PF 13.97 → 6.14. Inverted it
gives 16.42 on 83% of trades. **Longs catch the turn; shorts join a move already running — and
AT led D2D by six bars, so the shorts may simply be late.**

## C. SHORTS RESPOND TO A DIFFERENT GATE

Eleven variables tested on SHORT depth-3+ (n=1,485, ungated PF 3.60):

    Micro_FailedBreak > p50    n=626   WR 93.1%   PF 7.61    <- 2.1x, keeps 42%
    VWAP_Z > p50               n=374   WR 91.7%   PF 6.14
    Micro_Hurst > p70          n=448   WR 92.2%   PF 5.90
    Bar_Range > p70            n=899   WR 92.7%   PF 4.42

Stacking AT on top adds nothing (7.67 AT-bearish, 7.44 AT-bullish) — **FailedBreak does all the
work.** Structurally coherent: a failed break *is* a short setup; Hurst measures trend
persistence, which is what longs need.

**BUT A PRIOR SWEEP ALREADY FOUND A BETTER-EVIDENCED SHORT GATE, AND IT REPRODUCES.** 117
adaptive variables were swept in earlier research; **`Micro_Rejection:lo` scored at the 98.6th
random-subset percentile, OOS-positive and mechanism-backed** — a *priced* gate result.
Re-tested on this catalogue's short depth-3+ population:

    Micro_Rejection    lo p50   n=973  PF 5.00  net 25,709   keeps 89% of net
    Micro_Rejection    lo p30   n=630  PF 5.53  net 19,176
    Micro_FailedBreak  hi p50   n=626  PF 7.61  net 13,298   keeps 42% of net

**A gate found on the old pool still works on a pool built by a rebuilt pipeline from a
different catalogue.** That is out-of-sample confirmation and it is the strongest gate evidence
this project holds. FailedBreak has no such standing — 66 tests, one frame, never priced.

The two are a genuine trade: FailedBreak is the sharper filter, Rejection the wider one
retaining far more net. **Under survival-first that distinction matters more than the PF
headline. Measure both; recommend neither.**

Book with FailedBreak on shorts instead of Hurst:

    A  Hurst both sides        3,105 trades  PF 5.62  $98,380  wd -626
    B  FailedBreak on shorts   2,830 trades  PF 6.29  $89,981  wd -597

    B tiers:  solo   406  PF  3.07  wd -153
              dual   648  PF  2.48  wd  -96
              3-4  1,216  PF  7.44  wd -324
              5+     560  PF 38.44  wd  +37    WR 97.3%, NO LOSING DAY

    B monthly: 28 of 28 tier-months POSITIVE. Monthly PF 5.1 to 13.0.
               Jun $15,140, Jul $17,486 — the months that broke BOOK-50.

**The gate is coarse** — `>p50` applied at every depth. Short solo+dual went 227 → 811 trades
with PF barely moving (2.44 → 2.48), so the whole gain is in the 3+ tier. A tiered version
(Hurst p90 on short solos/duals, FailedBreak on short triples) is untested.

## D. COVERAGE AND RARITY PULL AGAINST EACH OTHER, HARD

                          LONG            SHORT
    BOOK-50              4.72%           2.42%
    rarity-ranked 50/50  3.32%   DOWN    3.81%   up
    coverage-greedy 50   6.82%           7.71%
    entire catalogue     9.01%           7.71%

**Ranking on rarity LOST long coverage against BOOK-50.** Rare signals fire rarely — that is
what makes them rare.

- **50 shorts chosen for coverage reach 7.71% — the entire catalogue's ceiling.** All 373 shorts
  together reach no further than the best 50. **The short side saturates immediately.**
- **50 longs chosen for coverage reach 6.82% of the 9.01% available** — three-quarters of what
  1,445 signals can reach, from 50 of them.

## E. THE HEADROOM IS A QUALITY GAP, NOT A GRAMMAR GAP

`unclaimed_reachable.csv` carries `n_prefilter_candidates_touching`: **min 76, median
1,810, max 73,357. Zero episodes have zero.**

**Every unclaimed reachable episode was reached by the search.** The 249-condition vocabulary
touches the entire reachable terrain. What excluded those 2,092 episodes was S5's filter —
`trades>=30 & folds_plus>=4 & agg_pf>=2.0`, which cut 463,996 to 39,308.

The companion column `n_valid_triples_touching` is **tautological and carries no information**:
an episode appears in that file precisely because no VALID signal touches it, so the count can
only ever be zero. Do not read it as a finding.

---

# PHASE 1 — WHERE DOES ADDING SIGNALS STOP HELPING?

**Files:** `dilution_curve_agg_pf.csv`, `dilution_curve_EXPECTED_ROWS_....csv`,
`selection_depthyield_grid.csv`

39,260 admission steps each, two ranking keys. Columns: `admitted`, `signal_id`, `ranking_key`,
`same_bar_ge3_bars`, `tolerance_N`, `depth`, `population`, `basis`. (39,260 not 39,308 — the 48
UNEVALUABLE rows cannot enter an admission curve. Confirm that is the whole discrepancy.)

**THE QUESTION, now two-dimensional because of finding D:** not only "where does depth 3+ stop
being selective" but **"where between rarest and widest does the book sit best, per
direction"**.

- Describe `same_bar_ge3_bars` against `admitted` for both curves. Identify the knee.
- **THE GAP BETWEEN THE TWO CURVES IS THE OVERFIT ESTIMATE.** If ranking by raw PF and by
  chance-adjusted price produce materially different curves, the difference is what PF-ranking
  buys that is not real.
- **Report the frontier, not a point.** For a given book size, the achievable
  (depth-selectivity, terrain-coverage) pairs, per direction.

**OUTPUT:** the frontier per direction, with uncertainty on where the knee sits.

---

# PHASE 2 — WHY DID THE WALK-FORWARD FAIL?

**Files:** `wf_pass_criterion.csv`, `wf_null_arm_summary.csv`,
`wf_book_arm_entities.csv`, `wf_per_segment_rederivation.csv`, `wf_splits.csv`

    split 0: 15,328 admitted →  7,962/11,816 → 0.6738  vs null 0.2360 = 2.86x
    split 1: 23,832 admitted →  9,712/19,176 → 0.5065  vs null 0.2375 = 2.13x
    split 2: 34,496 admitted → 11,303/28,901 → 0.3911  vs null 0.2593 = 1.51x

The ratio decays as VALID admits more. **TWO EXPLANATIONS, OPPOSITE REMEDIES:**

- **(a) DILUTION** — later-admitted signals are weaker. *Remedy: tighten VALID.*
- **(b) REGIME** — the third window is harder and the same signals would have failed there
  whenever admitted. *Remedy: none; it is a property of the period.*

**DECOMPOSE IT.** `wf_book_arm_entities.csv` holds 73,656 rows, one per (split, signal), with
`admitted`, `traded_on_test`, `persisted` and full train/test stats. Within split 2, separate
signals **also** admitted in split 0/1 from those **first** admitted in split 2, and compare
persistence. Dilution predicts the newly-admitted persist **worse**; regime predicts **same or
better**.

The training segments are nested (anchored) — split 2's train contains split 0's. State whether
that confounds the comparison and how you handled it.

**AND DO IT PER DIRECTION** (finding A). If the decay is short-side, the remedy is a gate, not
a tighter VALID.

**OUTPUT:** which explanation the data supports with the decomposition table, or a statement
that three splits cannot distinguish them — with what would.

---

# PHASE 3 — WHICH SIGNALS ARE DIAMONDS?

**Files:** `catalogue_F0.csv` (1,840), then F1, F3, F9, F2, F4, F11

44 base columns plus 17 axis columns. Pricing block: `n_trials_family`,
`null_valid_rate_family`, `expected_valid_by_chance_family`, `pf_null_p50/p90/p99_family`,
`pf_null_exceedance_pct`, `EXPECTED_ROWS_AT_OR_ABOVE_THIS_PF`, `q_value_BY_family`,
`n_null_family`, `null_matched_fraction`, `null_rejected_out_of_band`,
`null_direction_long_share`, `null_seed`.

- Rank by `EXPECTED_ROWS_AT_OR_ABOVE_THIS_PF` ascending. How many rows priced **below 1**? Per
  family, **per direction**. Note the F0 distribution is bimodal — 286 rows sit below 1 and the
  same 286 sit below 0.1. Clean separation, not a gradient. Establish whether that holds per
  family.
- Cross-check against `q_value_BY_family`. Where they disagree, say why.
- **PERSISTENCE MUST BE RANKED PER DIRECTION ON ITS OWN DISTRIBUTION.** A single absolute
  `min_fold_pf` threshold cut shorts by 76% and longs by 71%: short `min_fold_pf` median is
  **0.52** against long **0.79**; short `agg_pf` median **2.89** against long **3.55**. One
  number applied to both imposes the long side's standard on the short side.
- **REPORT `null_matched_fraction` PER FAMILY.** Poor matching means weaker pricing for every
  row in that family. F0 ran K=500, others K=200.
- Both arms are present: `trades/WR/agg_pf/worst_day_usd/net` ungated and
  `gated_trades/gated_WR/gated_PF/gated_worst_day_usd/gated_net/gated_delta_net`. Report the
  gate's effect per family — the operator sets gating **per signal**, not book-wide.
- F0 is 1,840 rows against BOOK-50's 48. State plainly how many clear the bar per direction;
  the operator's scaling target is 72–100 F0 signals for 4× the 505 triple population.

**PRECEDENT WORTH KNOWING:** an independent auditor blind re-deriving from BOOK-50's identical
2,420-signal pool produced **26 LONG / 19 SHORT** (OOS PF 3.23, 6/6 folds, 22/22 weeks) against
the committed 37/13. Same pool, far more balanced answer. **A balanced book was always
available from the same evidence** — the imbalance was a property of the selection method, not
of the market. Your shortlist should make that visible rather than reproduce it.

**OUTPUT:** shortlist size per direction per family, and the intersection of cheap-by-chance AND
fold-persistent.

---

# PHASE 4 — CONVERGENCE, AND HOW THE OTHER FAMILIES WORK ALONGSIDE F0

**Files:** `cohort_scored.csv` (95 rows, PF/WR/net/worst-day per family composition),
`same_bar_cohort.csv`, `cross_family_cofiring.csv`, `cluster_basis_summary.csv`

**PART 1 — DOES A MIXED COHORT KEEP THE EDGE, PER DIRECTION?**

`cohort_scored.csv` carries `direction, depth, family_composition, purity, bars, trades, WR, PF,
net, avg_trade, worst_day_usd, sufficient`. ALL-ONE-FAMILY and MIXED are separated.

**Read per direction.** Finding A: the long side carries a 10× depth gradient and the short side
carries none, so a blended cohort PF is not interpretable. Cohorts with `sufficient=False` are
retained with their counts — report them, do not drop them.

**PART 2 — THE THREE ROLES THE OTHER FAMILIES COULD PLAY**

F1 (37,276), F3 (53), F9 (133), F2, F4, F11 were absent from the illustrative book. Measure each
role separately:

1. **TRADE COMPANIONS** — do they add depth to F0 bars? `same_bar_cohort` at depth 3 LONG was
   `F0:467; F1:3; F11:8; F3:23; F4:1; F9:41`. **F1 contributes 3 slots from 37,276 catalogue
   rows.** Establish whether that is a property of the families or of the selection — it is the
   single biggest open question about the catalogue's composition, and it decides whether F1 is
   fuel or noise.
2. **GATES** — an F1 sequential pattern or an F9 session condition may qualify an F0 entry
   without trading itself. **Untested, and the most interesting unexplored use.** Specify how it
   would be measured.
3. **INDEPENDENT COVERAGE** — do they touch episodes F0 cannot reach? Compare
   `touched_episode_ids` across families. A family reaching terrain F0 misses earns a place on
   coverage grounds regardless of its own PF.

**OUTPUT:** per direction, the co-firing structure; and for each family, which of the three roles
the evidence supports.

---

# PHASE 5 — WHAT IS REACHABLE THAT HE DOES NOT HOLD?

**Files:** `unclaimed_reachable.csv`, `terrain_hour_profile.csv`, `terrain_episodes.csv`,
`reach_D0_missed_decomposition.csv`, `reach_D02_D2_coverage.csv`

2,092 unclaimed (1,037 UP / 1,055 DOWN) of 2,298 reachable.

**The headline is established (finding E): 100% quality gap, 0% grammar gap.** What remains:

- **Quantify the quality gap.** Of the 463,996 pre-filter candidates, what would a looser filter
  admit, and how much unclaimed terrain would it reach? The operator's decision is whether
  `agg_pf >= 2.0` moves, and this is the number that informs it.
- Profile the unclaimed by `est_hour_start` and `displacement_pts`. Concentrated in a session, or
  spread? Cross-reference `terrain_hour_profile` — NY OPEN holds 225 of the largest-decile
  episodes, overnight holds 0.
- `reach_D0_missed_decomposition` gives the gate decomposition — why an episode is out of reach
  at all. `reach_D02_D2_coverage` has 60 rows of stratum × direction coverage.
- **Note the short-side saturation from finding D:** 50 shorts reach the same 7.71% that all 373
  do. Establish why — is the short terrain genuinely smaller, or are the shorts clustered?

**OUTPUT:** the real headroom number per direction, and what filter change would reach it.

---

# PHASE 6 — BALANCE ACROSS FOUR AXES, AND DIRECTIONAL GATING

**Files:** `catalogue_F0.csv` (all four axes present), `regime_labels.csv`,
`terrain_hour_profile.csv`, `family_evidence.csv`, `selection_g2_domain_bridging.csv`,
`DOT_signal_dictionary.xlsx`

**ALL FOUR AXES ARE NOW MEASURABLE.** The catalogue carries 17 axis columns:

- **STRUCTURE** — `market_structure`, `market_structure_secondary`. Nine categories from the
  dictionary's Name Key: TC Trend Continuation, MI Momentum Ignition, SE Structural Entry, BX
  Breakout Expansion, SB Squeeze Breakout, PA Price Action, TE Trend Exhaustion, VC Volume
  Confirmed, and **D2D Break-of-Structure — the ninth**, named in the same sheet as the founding
  break-of-structure engine. F0 distribution: TREND_CONTINUATION 763, MOMENTUM_IGNITION 616,
  BREAKOUT_EXPANSION 178, STRUCTURAL_ENTRY 160, PRICE_ACTION 60, VOLUME_CONFIRMED 19,
  TREND_EXHAUSTION 2, **UNMAPPED 20**. Verify the classifier against the dictionary and report
  whether the 20 unmapped are a class or stragglers.
- **SESSION** — `session_*_pct` across the eight terrain sessions plus `session_modal`. F0 modal:
  overnight 987, morning 810, preclose 21.
- **REGIME** — `regime_causal_0_pct`, `regime_causal_1_pct`, `regime_burnin_pct`, `regime_modal`,
  joined from `regime_labels.csv` (170,351 rows, `bar_index` 6900–177250, `lab_causal` in
  {-1,0,1}, 5,636 burn-in bars). **Use `lab_causal` only** — `lab_desc` is full-sample and must
  never characterise a tradeable signal. F0 modal: causal_0 1,459, causal_1 359, NaN 22 (the
  UNEVALUABLE rows, correctly blank).
- **DIRECTION** — in every row.

**THE TENSION TO QUANTIFY RATHER THAN RESOLVE:** NY OPEN holds 225 of the largest-decile episodes
and overnight holds 0. A session-balanced book deliberately takes worse episodes to be even.
**Measure that cost.** The operator decides whether to pay it.

**AND THE GATING WORK, WHICH IS PART OF THIS PHASE:**

Findings B and C establish that gating is **directional**. Longs enter on pullbacks and respond
to Hurst; shorts enter on continuation and respond to FailedBreak. Neither gate has been priced.

**SPECIFY WHAT A PRICED GATE TEST LOOKS LIKE — AND THE PRECEDENT ALREADY EXISTS.** The prior
research priced `Micro_Rejection:lo` at the **98.6th random-subset percentile**. That is the
form the measurement should take, and it means the machinery has been done once before.
Recover the method from the record rather than inventing one.

`Micro_FailedBreak > p50` has no such standing: 66 tests (eleven variables x three thresholds x
two sides), one frame, never priced. **How large a lift would the best of 66 tested gates show
by chance alone?** Specify the null; do not build it. Then state which of the two gates the
evidence actually supports.

Also specify how to test the **tiered** gate that is currently untested: Hurst p90 on short
solos/duals, FailedBreak on short triples.

**OUTPUT:** the achievable coverage per (direction × structure × session × regime) cell, the
measured cost of balance against concentration, and the specification for a priced gate test.

---

## WHAT YOU DO NOT DO

- **You do not select the book.** Item 15: the catalogue is emitted from VALID, never from an
  argmax, and nothing in this build chooses. That includes you.
- **You do not propose a quota, floor, target or expected composition.** Doctrine rule 3.
  Composition is an OUTPUT to be reported, never an INPUT to be constrained.
- **You do not choose N** from the tolerance grid. You report the curve.
- **You do not adopt a gate.** You specify how one would be priced.
- **You do not build.** Specify; the Developer builds.

## DELIVER

Phase by phase, in order, each with its answer stated as a number or a plain finding. Where a
phase cannot be closed, name the file or measurement that would close it.

**Do not summarise all six into a recommendation at the end.** The operator draws the
conclusions — that is the whole architecture of this rebuild.
