# SELECT_BUILD_SPEC.md — REVISION 6: §5.3 STAGE C
# Supersedes §5.3 STAGE C in full. Stage A, B, D and §5.4 unchanged except where §6.3 states.

## §C.0 THE DEFECT

**"Candidate must reduce book loss events in both halves" IS MONOTONE AND CANNOT FAIL.**
A gate admits a subset of bars, so it can only remove loss events. Every candidate passes
by construction — the Developer measured 45 of 45, then 112 of 112 entering Stage D.

**Both obvious readings are monotone.** Cell events with the candidate vs without it is
the same defect at cell scale. **The only non-vacuous comparison is against a
rarity-matched alternative, because that holds the number of admitted bars roughly fixed
and asks whether the candidate's admitted bars are *cleaner*.**

---

## §C.1 THE METRIC — A RATE, NOT A COUNT

**`retained_loss_rate` = (cell loss bars the candidate ADMITS) / (cell bars the candidate
ADMITS).**

Non-monotone by construction: admitting fewer bars removes losses **and** winners, so the
ratio can move either way. **Computed from masks and the ungated trade table only — no
engine run per candidate.** The 249-condition scan at both d3 cells took under one minute.

**BASIS FOR EVERY FIGURE BELOW: 297 book · FLOORED · floor 3/3 · cap 21 · `ATR_1M >= 20`
· NO TIER GATES · full frame 177,251 bars · 1.0 lot.**

---

## §C.2 IT SEPARATES — MEASURED AT BOTH ACCEPTANCE CELLS

**LONG d3 — 538 bars, 61 loss bars, base rate 0.1134. Half A 41/324, half B 20/214.**
Rarity band `[0.053, 0.159]` → shortlist **39 of 208** testable conditions.

| filter | survivors |
|---|---|
| beat base rate overall | 13 of 40 = 32% |
| **beat it in BOTH halves** | **4 of 40 = 10%** |

**`Micro_Hurst > p90` ranks #1 of 4 — zero candidates strictly better.** Admits 57 bars,
`retained_loss_rate` 0.0702 (A 0.0625 on n=32, B 0.0800 on n=25) against a 0.1134 base.
Runners-up: `Micro_Rejection:hi` 0.0964, `Lower_Wick:lo` 0.1029, `AT_Score_ST:hi` 0.1053.

**SHORT d3 — 384 bars, 20 loss bars, base rate 0.0521. Half A 7/223, half B 13/161.**
Rarity band `[0.074, 0.223]` → shortlist **66 of 210**.

| filter | survivors |
|---|---|
| beat base rate overall | 32 of 67 = 48% |
| **beat it in BOTH halves** | **17 of 67 = 25%** |

**`Micro_Hurst > p90` ranks #7 of 17.** Admits 57 bars, rate 0.0175 (A 0.0238 on n=42,
**B 0.0000 on n=15**). Six candidates are strictly better, and **three of them sit at
exactly 0.0000** — `Micro_Hurst:lo`, `RangeOsc_State:==2`, `PoC_Side:==-1`.

**10% and 25% pass rates. The test is not vacuous and it can fail.**

---

## §C.3 BUT AT SHORT d3 IT CANNOT ORDER — AND THAT IS A RESOLUTION FLOOR, NOT A RESULT

**Half A at SHORT d3 retains between 0 and 5 loss bars across the entire shortlist.** A
candidate retaining zero is indistinguishable from one retaining one, and **three
candidates tie at 0.0000 overall on a 20-event cell.** Those ties are not evidence of
quality; they are the arithmetic of a 7-loss-bar half.

**LONG d3 does not have this problem** — half A retains 0 to 11 loss bars, enough to
order.

**CONFIG, AND THE FAILURE MODE:**

```
gates.min_retained_loss_bars_per_half   int   required   (this frame: 5)
```

For each admitted cell, compute the maximum retained loss bars in the weaker half across
the shortlist. **If it is below `min_retained_loss_bars_per_half`, Stage C runs as a
FILTER ONLY and the printout must state `STAGE C: FILTER ONLY — RESOLUTION FLOOR
<max> < <threshold>, RANKING SUPPRESSED`.** Candidates passing both halves proceed to
Stage D **unordered**, and no `CANDIDATE`-tier row for that cell may carry a rank.

**On this frame: LONG d3 ranks (floor 11). SHORT d3 is FILTER ONLY (floor 5, at the
boundary — and the three-way tie at 0.0000 is the proof it cannot order).**

**An honest filter that states its own vacuity beats a filter that passes everything and
looks like a test.**

---

## §C.4 STAGE C, EXECUTABLE

```
INPUT   cell bars and cell loss bars from the §C.1 ungated basis
        Stage B shortlist (rarity band on the cell's own bars)

FOR each candidate in shortlist:
    admitted        = cell_bars[mask[cell_bars]]
    retained_loss   = count(admitted in cell_loss_bars)
    rate            = retained_loss / len(admitted)
    rate_A, rate_B  = same, restricted to each half
    PASS iff rate_A < base_rate_A AND rate_B < base_rate_B

RESOLUTION  floor = max over shortlist of retained_loss in the weaker half
            if floor < gates.min_retained_loss_bars_per_half:
                emit FILTER ONLY, suppress ranking, pass survivors unordered
            else:
                rank survivors by `rate` ascending

FAILURE     if zero candidates pass in an admitted cell, print
            "STAGE C: NO SURVIVORS" and declare the cell FREE. Do not relax the test.
```

**PRINT PER CELL:** bars, loss bars, base rate, per-half bases (e.g. `SHORT d3: 7/223 and
13/161`), shortlist size, beat-overall count, **beat-both-halves count**, resolution floor,
`RANKED` or `FILTER ONLY`, and the survivor list with rates.

---

## §C.5 CONSEQUENCE FOR §5.4 — THE ACCEPTANCE TEST PASSES, AND ONLY IN ITS REVISED FORM

**`Micro_Hurst > p90` appears in the both-halves survivor set at BOTH d3 cells: #1 of 4
at LONG d3, and among the 17 at SHORT d3.** The REV5 acceptance test — *appear in the
`CANDIDATE` tier at both cells* — **PASSES.**

**Had REV5 required rank 1 at both cells, it would FAIL at SHORT d3**, where six
candidates including three zero-rate ties outrank it on a 20-event cell. **That is the
resolution floor, not a better gate**, and it is why the revised wording was the right
one.

**§5.6 CARRIES FORWARD UNCHANGED:** the pre-registered path remains the only route to a
defensible `CONFIRMED`. `Micro_Hurst > p90` earned p = 0.000 and p = 0.022 as **one**
pre-registered candidate on a stated mechanism — trial count 1. As best-of-66 it is a
shortlist entry and nothing more. **A lowered threshold is not an alternative.**

---

## §C.6 SWEEP FOR THE DEFECT CLASS — EXHAUSTIVE

**THE CLASS: comparing a gated configuration against an ungated one on a quantity that
gating can only move in one direction.** Every comparison in §5 checked:

| location | comparison | monotone? | verdict |
|---|---|---|---|
| §5.3 Stage A — power floor | `cell_events >= min_cell_events` | n/a — threshold on a measured count, no comparison | **SAFE.** Declared a power filter, not a test. |
| §5.3 Stage B — rarity band | pass rate within `[lo x ref, hi x ref]` | n/a — two-sided band on rarity, outcome never read | **SAFE.** Filter by construction and labelled so. |
| §5.3 Stage C — **as written in REV5** | book loss events, gated vs ungated | **YES** | **DEFECT. Fixed by §C.1–C.4.** |
| §5.3 Stage D — null | loss events, candidate gate vs **rarity-matched alternative gate** | NO — both arms gated | **SAFE, BUT ONLY BY CONSTRUCTION.** See below. |
| §5.4 acceptance | membership of `Micro_Hurst > p90` in the CANDIDATE tier | n/a — set membership | **SAFE.** |
| §5.1 cell basis | per-cell loss rate vs book loss rate | NO — both ungated | **SAFE.** Descriptive only; the original Stage A misused it as a test and that is already fixed. |

**ONE INSTANCE FOUND, AND ONE LATENT RISK THAT MUST BE CLOSED IN WORDING.**

**Stage D is safe only because both arms are gated.** A Developer implementing
*"comparison is loss EVENTS"* against the **ungated** book would reproduce the Stage C
defect exactly. **§5.3 Stage D must therefore read:**

> **The comparison is candidate-gate against rarity-matched-alternative-gate. BOTH ARMS
> ARE GATED. A comparison against the ungated book is monotone and vacuous — see §C.0.**

**No other instance of the class exists in §5.**

---

## §C.7 WHAT THIS SERVES

The objective is **the best non-curve-fitted persistence of the triple-convergence
signals** — gates that survive because the market has that structure, derived by a process
that would find whatever is true on a different instrument tomorrow.

**That is why an empty `CONFIRMED` tier is acceptable and a lowered threshold is not**, and
why §C.3 suppresses ranking rather than reporting an order it cannot support. A scan that
reports `CANDIDATE` tiers with their trial counts, alongside a pre-registered path for
mechanism arguments, is complete and defensible. **A scan that manufactures a confirmation
would poison every asset this pipeline touches.**
