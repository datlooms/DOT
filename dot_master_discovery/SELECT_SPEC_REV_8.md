# SELECT_BUILD_SPEC.md — REVISION 8: §7.4 PREREG NULL
# Supersedes §7.4 EVALUATION in full. Everything else in REV7 stands.

## §8.0 THE DIAGNOSIS IS RATIFIED — IT WAS THE QUANTITY, NOT THE POPULATION

The Developer asked whether the prereg null should draw from the rarity shortlist or from
the hand-derivation's reference set. **That is not the difference. The Supervisor's
diagnosis is correct and is ratified in full.**

| | quantity | source | population |
|---|---|---|---|
| hand-derivation | **BOOK loss events** | full engine run, replace-not-stack | rarity-matched conditions |
| §7.4 as built | **cell `retained_loss_rate`** | masks only | rarity-matched conditions |

**`retained_loss_rate` was invented for Stage C to be cheap and non-monotone. It is not
what the gate was derived on, and it is not what the acceptance test claims.** The
population was right in kind at both ends; the metric changed underneath it.

**The Supervisor's evidence was the decisive part:** LONG d3 missing only on the
correction (`0.045 x 39 = 1.8`, consistent with `1 of 45`) while SHORT d3 returned
`24 of 200` against `0 of 30` — a correction effect cannot produce that, a different
quantity can.

---

## §8.1 AND THE DRAW COUNT WAS ALSO WRONG — THE NULL POPULATION IS FINITE

**At 39 and 66 distinct shortlist candidates, a 200-draw null cannot resolve below
`1/39 = 0.0256` and `1/66 = 0.0152`. Drawing 200 times from 39 candidates is resampling,
not resolution.** The Supervisor named this and it is correct.

**THE NULL POPULATION IS SMALL AND FINITE, SO ENUMERATE IT.** `p` becomes exact, the run
is cheaper, and `gates.null_draws` becomes irrelevant for the prereg path.

```
gates.null_mode = "exhaustive"      the only supported value for the PREREG path
```

**Cost, measured this frame at 4.2 s per engine run with `PRE_MASKS` caching:
LONG d3 39 runs = 2.8 min, SHORT d3 66 runs = 4.6 min.** Not the 31 minutes a 200-draw
null would have cost, and exact rather than sampled.

*(My hand-derivation was itself inconsistent here: LONG d3 was exhaustive at 45 of 45,
SHORT d3 was 30 of 66 — a subsample. Exhaustive at both is the correction.)*

---

## §8.2 §7.4 EVALUATION — REPLACED

```
INPUT   gates.preregistered  : list of {cell:[dir,tier], variable, side, pct}
        the cell's rarity shortlist from Stage B (min_cell_support applied)

MASK    prereg_mask(df, variable, side, pct)      via swept_thresholds.swept
        CHECKSUM against §7.4 table, ABORT on mismatch      <- unchanged, and it works:
                                                               it caught a 90 vs 0.90
                                                               scale error before any
                                                               mask was consumed
METRIC  BOOK loss events = |{(entry_bar, direction) : pnl < 0}| over the BOOK-only
        population, from a FULL ENGINE RUN with ADM_TIERGATES = {cell: [mask]}
        NOT retained_loss_rate. NOT masks only. NOT cell-scoped.

NULL    replace-not-stack, EXHAUSTIVE over every shortlist candidate.
        BOTH ARMS GATED (REV6 SC.6). One engine run per candidate.
        p = (candidates with strictly FEWER book loss events) / (shortlist size)
        Report ties separately; ties are NOT counted as better.

FAILURE if the shortlist is empty, print "PREREG: NO NULL POPULATION" and return
        no verdict for that cell. Do not fall back to a sampled null.
```

---

## §8.3 THE VERDICT THIS FRAME — RUN BEFORE SHIPPING, PER MY OWN STANDING RULE

**BASIS: 297 book · FLOORED · floor 3/3 · cap 21 · `ATR_1M >= 20` · `recentfb_sizing =
False` · full frame 177,251 bars · 1.0 lot · S2 pool 249 · `min_cell_support = 20` ·
exhaustive null · `PRE_MASKS` caching.**

| cell | adopted book events | null n | null min / median / max | strictly better | **p** | ties |
|---|---|---|---|---|---|---|
| **LONG d3** | **96** | 39 | 97 / 101.0 / 105 | **0** | **0.0000** | 0 |
| **SHORT d3** | **128** | 66 | 127 / 130.0 / 136 | **2** | **0.0303** | 5 |

**LONG d3: `Micro_Hurst > p90` is better than every one of the 39 rarity-matched
alternatives. p = 0.0000.** Against my hand-derived `1 of 45 = 0.022` — the exhaustive
book-event null is *stronger* than what hand-derivation reported.

**SHORT d3: 2 of 66 better, p = 0.0303.** Against my hand-derived `0 of 30 = 0.000` — the
exhaustive null over the full 66 is *weaker*, because my 30-candidate subsample happened to
exclude both of the two better conditions. **The exhaustive figure supersedes it; the
hand-derived `p = 0.000` at SHORT d3 was a subsampling artefact and must be corrected in
the record.**

**The 0.12 the Developer measured came from `retained_loss_rate`. On the derived quantity
it is 0.0303.**

---

## §8.4 THE TRIAL-COUNT CORRECTION — RULED ON, AND IT IS COMPUTED NOT DECLARED

**RULE:**

```
n_tests = number of (candidate, cell) PAIRS actually evaluated       COMPUTED BY THE CODE
threshold = gates.confirm_alpha / n_tests
```

**`n_tests` is counted by the implementation from what it evaluated. It is never read from
config and never declared by the operator.** Declaring "one hypothesis" to obtain a weaker
correction is the failure mode this closes, and it is the reason the count cannot be an
input.

**On this frame: 2 pairs → threshold `0.05 / 2 = 0.025`.**

| cell | p | threshold | verdict |
|---|---|---|---|
| LONG d3 | **0.0000** | 0.025 | **CONFIRMED** |
| SHORT d3 | 0.0303 | 0.025 | **CANDIDATE** |

**I decline the "one mechanism tested twice" argument.** It is available — both cells test
the same condition on the same mechanism — but it is not measurable, it would be decided by
whoever writes the config, and it buys exactly the gap between 0.0303 and 0.05 at one cell.
**A correction that can be argued down by re-describing the hypothesis is not a
correction.** Two pairs, two tests.

**AND REPORT THE CONJUNCTION AS SUPPORTING EVIDENCE, NEVER AS THE CRITERION:** for a
candidate evaluated at multiple cells, print `p_product` (here `0.0000 x 0.0303`) and label
it `SUPPORTING — CELLS ARE NOT INDEPENDENT (shared book, overlapping bars)`. It is
informative and it is not a test.

---

## §8.5 THE ACCEPTANCE TEST — VERDICT AND WHAT IT MEANS

**REV7 §7.3 requires `p <= 0.05` at both cells at trial count `len(preregistered)`.**

**Measured: LONG d3 p = 0.0000 PASS. SHORT d3 p = 0.0303 PASS at 0.05, CANDIDATE at the
corrected 0.025.**

**THE ACCEPTANCE TEST AS WRITTEN IN REV7 PASSES AT BOTH CELLS.** The `CONFIRMED` tier is
reached at LONG d3 and not at SHORT d3, and that is the honest result rather than a
failure: **the gate is unambiguous at the cell with 61 loss bars and marginal at the cell
with 20.**

**DO NOT WEAKEN `gates.confirm_alpha` TO PROMOTE SHORT d3. DO NOT DROP THE TRIAL
CORRECTION.** A `CONFIRMED` obtained either way would mean nothing, and REV6 §C.7 is the
standard: a scan that manufactures a confirmation would poison every future asset this
pipeline touches.

**PRINT PER CELL:** adopted book events, null size, null min/median/max, strictly-better
count, ties, exact `p`, `n_tests`, the corrected threshold, and `CONFIRMED` / `CANDIDATE`
with the reason on the row.

---

## §8.6 THE RECORD CORRECTION THAT FOLLOWS

`The_Whole_DOT_spec_v2.txt` §3.5 states `Micro_Hurst > p90` at SHORT d3 cleared at
**p = 0.000, 0 of 30 rarity-matched better**. **That figure came from a 30-of-66
subsample. Exhaustive over all 66 it is 2 of 66 = 0.0303.** The gate still clears an
uncorrected 0.05 and remains the best-supported gate in the system, and it is still the
only condition ever to clear at two independent cells — **but the SHORT d3 p-value in the
spec is wrong and should read 0.0303 with the exhaustive basis stated.**

**LONG d3 improves in the same correction: `1 of 45 = 0.022` becomes `0 of 39 = 0.0000` on
the exhaustive book-event null.** The two figures moved in opposite directions, which is
what a change of quantity looks like and what a change of correction does not.

---

## §8.7 THE STANDING RULE, FOURTH APPLICATION

This revision exists because §7.4 specified a null without running it. **The rule has now
caught four defects: the Stage C vacuity (Developer, by executing it), the SHORT d3
per-half failure, the `min_retained_loss_bars_per_half` boundary contradiction, and this
metric substitution.**

**Every one was a threshold or a comparison stated as an expectation rather than executed.
Nothing with a threshold in it ships from here without its measured verdict printed in the
spec.**
