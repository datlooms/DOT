# SELECT_BUILD_SPEC.md — REVISION 7: §5.4 AND §C.2
# Supersedes §5.4 (REV5, as amended by REV6 §C.5) and §C.2 in full.
# Stage A, Stage B, Stage C metric and Stage D unchanged.

## §7.0 THE CONTRADICTION, CONFIRMED FROM SOURCE

`dots_thresholds.py` L48–49 hardcodes `_D_SPEC[(c,'hi')] = (c, 0.80)` and
`_D_SPEC[(c,'lo')] = (c, 0.20)`. **Verified at runtime: the percentiles present in
`_D_SPEC` are exactly `[0.2, 0.8]`.**

| adopted gate | expressible in the S2 pool? |
|---|---|
| `Micro_Hurst > p90` | **NO** |
| `AT_Slope_ST > p90` | **NO** |
| `Micro_FailedBreak > p20` | YES — the complement of `Micro_FailedBreak:lo` |

**§5.4 asked the scan to rediscover a condition its candidate space cannot represent.**
The `Micro_Hurst:hi` the scan ranked is `> p80` — a different, looser condition.

**AND THIS IS AN ARCHITECTURAL FACT WORTH RECORDING, NOT AN APOLOGY: the 297 signals are
built from a p80/p20 vocabulary, and the adopted gate stack uses percentiles that
vocabulary cannot express.** The gates entered the system through hand-derivation with
`swept_thresholds`, not through the scan grammar. Any scan over the S2 pool is therefore
searching a strictly smaller space than the one the gates live in.

---

## §7.1 THE TWO UNNAMED RULES — NAMED, WITH THEIR MEASURED EFFECT

Both were implicit in my §C.2 figures and neither was in any spec. **This is the third
time in this exchange a figure carried no basis. Both are now config keys.**

### RULE 1 — MINIMUM SUPPORT

```
gates.min_cell_support   int   required   (this frame: 20)
```

A candidate is testable at a cell only if it admits at least `min_cell_support` of that
cell's entry bars. Below that its rate is not estimable.

**MEASURED, §7.4 basis: excludes 41 of 249 at LONG d3 (208 supported) and 39 of 249 at
SHORT d3 (210 supported).** This is the whole difference between my `39 of 208 / 66 of 210`
and the Developer's `112 of 249 / 125 of 249` — **the Developer's denominator is correct
for an unrestricted pool; mine silently applied this rule.**

### RULE 2 — THE HALF SPLIT

```
gates.half_split = "cell_bar_median"     the only supported value
```

**Split at the median of THAT CELL'S OWN entry bars** — chronological and equal-support.
Not the frame midpoint, which is what I used and which produces unequal halves.

**MEASURED:**

| cell | split bar | date | half A | half B |
|---|---|---|---|---|
| LONG d3 | 71,983 | 2026.04.02 | 269 bars / 40 loss (0.1487) | 269 bars / 21 loss (0.0781) |
| SHORT d3 | 76,918 | 2026.04.08 | **192 bars / 4 loss (0.0208)** | 192 bars / 16 loss (0.0833) |

My frame-midpoint split gave 324/214 and 223/161. **The Developer's near-equal 251/252 and
170/183 came from splitting on a different basis again; `cell_bar_median` gives exactly
269/269 and 192/192 and is now the named rule.**

---

## §7.2 RUNNING THE THRESHOLD CHANGED THE ANSWER — THE ACCEPTANCE TEST FAILS AT SHORT d3

Applying my own standing rule — **any rule with a threshold is run on this frame and its
verdict printed before it ships** — Stage C was re-evaluated under Rules 1 and 2 with
`Micro_Hurst > p90` supplied directly as a mask.

| cell | prereg rate (A / B) | per-half base (A / B) | both halves | pool candidates better | rank |
|---|---|---|---|---|---|
| LONG d3 | 0.0702 (0.0645 / 0.0769) | 0.1487 / 0.0781 | **PASS** | 1 | **2 of 5** |
| SHORT d3 | 0.0175 (**0.0286** / 0.0000) | **0.0208** / 0.0833 | **FAIL** | 6 | 7 of 16 |

**`Micro_Hurst > p90` fails Stage C at SHORT d3: half A retains 1 loss bar of 35 admitted
= 0.0286 against a half-A base of 0.0208 on FOUR loss bars.**

**AND THE VERDICT FLIPS ON THE SPLIT DEFINITION ALONE.** Under my frame-midpoint split it
passed (half A base 0.0314, prereg 0.0238). Under the named `cell_bar_median` split it
fails. **A gate that cleared its rarity-matched null at p = 0.000 on the full cell fails a
per-half filter whose weaker half carries four loss bars.** Its LONG d3 rank also moved
from 1 to 2.

**THE PER-HALF FILTER DESTROYS POWER AT SHORT d3. THAT IS A PROPERTY OF A 4-LOSS-BAR HALF,
NOT A PROPERTY OF THE GATE.**

---

## §7.3 THE DECISION — OPTION B. AND THE ACCEPTANCE TEST VALIDATES THE PIPELINE, NOT THE SCAN.

**OPTION A IS REJECTED, ON TWO GROUNDS.**

*Arithmetic:* `{p80, p90, p95, p97, p99}` and lo mirrors over 90 FEAT_ variables is
`90 x 5 x 2 = 900`, plus 69 equality = **969 conditions x 4 admitted cells = 3,876
trials.** Bonferroni gives `0.05 / 3876 = 1.29e-05`, and a rarity-matched null resolves at
`1/draws`, so **~77,500 draws per candidate.** Stage D's correction was already unreachable
at 264 trials; at 3,876 it is not close.

*Method, and this is the binding reason:* **selecting which of five percentiles a variable
uses, on the basis of which scored best, is curve-fitting.** It is the practice this
project has refused all week, and it would be applied 900 times.

**OPTION B IS ADOPTED.**

**THE ACCEPTANCE TEST VALIDATES THE PIPELINE'S ABILITY TO CONFIRM A PRE-REGISTERED
MECHANISM — NOT THE SCAN'S ABILITY TO REDISCOVER ONE.** The scan's job is breadth over
what its vocabulary can express, reported honestly as `CANDIDATE` with trial counts. The
prereg path is the only route to a defensible `CONFIRMED`, and §7.2 shows why: the full-cell
null has 20 loss events at SHORT d3, the per-half filter has 4.

**§5.4 IS REPLACED BY:**

> **ACCEPTANCE TEST.** The PREREG path, evaluated by the Stage D null on the full cell,
> must return `p <= 0.05` for `Micro_Hurst > p90` at **both** LONG d3 and SHORT d3, at
> trial count = `len(gates.preregistered)`.
> **Reference values from hand-derivation: LONG d3 p = 0.022 (1 of 45 rarity-matched
> better); SHORT d3 p = 0.000 (0 of 30 better).**
> **THE PREREG PATH DOES NOT PASS THROUGH STAGE C.** Stage C is a scan filter and its
> per-half comparison is underpowered where a cell's weaker half carries few loss bars —
> measured at four, SHORT d3 half A. A pre-registered candidate is tested against the null
> on the whole cell, which is how it was derived.
> **The scan's own output is never promoted to `CONFIRMED`, and the acceptance test says
> nothing about the scan.**

**DO NOT STRENGTHEN THIS TO REQUIRE THE SCAN TO FIND IT — the candidate space cannot
express it. DO NOT WEAKEN IT BY LOWERING `confirm_alpha`.**

---

## §7.4 THE BRIDGE — THE PREREG EVALUATOR, EXECUTABLE

**Currently the PREREG block is declared and printed but never evaluated. This is the
missing step.**

```python
import swept_thresholds as sw

# gates.preregistered : list of {cell: [dir, tier], variable: str, side: "hi"|"lo", pct: float}
def prereg_mask(df, variable, side, pct):
    t = sw.swept(df, {(variable, side): (variable, pct)})[(variable, side)]
    v = df[variable].values
    return (v > t) if side == "hi" else (v < t)      # STRICT both ways
```

`sw.swept` substitutes `dt._D_SPEC`, calls the sacred `dt.compute_adaptive_thresholds`,
and restores `_D_SPEC` in a `finally`. **Ring 2500, day-refreshed on the day-of-month
field only, floor-index percentile, no warm-up special case — identical to production by
construction.**

**MANDATORY CHECKSUM BEFORE ANY PREREG CANDIDATE IS EVALUATED.** Over all 177,251 bars of
this frame, measured this turn:

| condition | pass rate | tolerance |
|---|---|---|
| `Micro_Hurst > p90` | **9.7478%** | exact to 4 dp |
| `AT_Slope_ST > p90` | **6.2217%** | exact to 4 dp |
| `Micro_FailedBreak > p20` | **80.1874%** | exact to 4 dp |

**FAILURE MODE: if a checksum does not match, ABORT the gate layer and print both values.**
A mask near 20% where `Micro_Hurst > p90` belongs means `ad[(var,'hi')]` — the p80 series —
is being used, which is the defect `swept_thresholds` exists to prevent and which has
already shipped once.

**EVALUATION.** Each prereg candidate goes to Stage D directly: replace-not-stack against
the rarity-matched shortlist for its cell, **both arms gated** (REV6 §C.6), comparison on
loss EVENTS over the whole cell, `gates.null_draws` draws. Adopt as `CONFIRMED` iff
`p <= gates.confirm_alpha / len(gates.preregistered)`.

**INTEGRITY.** `gates.preregistered` is read from config, printed in the banner before any
scan output exists, and the **config sha is written into every artifact row**. A candidate
added after seeing scan output changes the sha and is detectable.

---

## §7.5 §C.2 REPLACED — THE SCAN FIGURES, WITH THEIR FULL BASIS

**BASIS FOR EVERY FIGURE: 297 book · FLOORED · floor 3/3 · cap 21 · `ATR_1M >= 20` ·
NO TIER GATES · full frame 177,251 bars · 1.0 lot · S2 pool 249 conditions ·
`gates.min_cell_support = 20` · `gates.half_split = cell_bar_median`.**

| | LONG d3 | SHORT d3 |
|---|---|---|
| cell bars / loss bars / base rate | 538 / 61 / 0.1134 | 384 / 20 / 0.0521 |
| split bar (date) | 71,983 (2026.04.02) | 76,918 (2026.04.08) |
| half A bars / loss / base | 269 / 40 / 0.1487 | 192 / **4** / 0.0208 |
| half B bars / loss / base | 269 / 21 / 0.0781 | 192 / 16 / 0.0833 |
| excluded by min support | 41 of 249 | 39 of 249 |
| supported | 208 | 210 |
| rarity band | [0.0530, 0.1589] | [0.0742, 0.2227] |
| shortlist | 39 | 66 |
| **both-halves survivors** | **5 of 39 = 13%** | **16 of 66 = 24%** |
| ties at minimum rate | 0 | 0 |
| Stage C verdict per REV6 §C.3 | `RANKED` | `RANKED` |

**The rarity band reference is the prereg candidate's own pass rate on the cell — state
that in the printout, because it means the shortlist is centred on the thing being
tested.** If `gates.preregistered` is empty for a cell, `gates.rarity_ref` must be
supplied explicitly or the cell is skipped with `NO RARITY REFERENCE`.

**Stage C rejects: 13% and 24% pass. The metric works. It is a filter over the scan's own
vocabulary and nothing more.**

---

## §7.6 WHAT STANDS

**REV6 §C.7 unchanged.** The objective is the best non-curve-fitted persistence of the
triple-convergence signals. An empty `CONFIRMED` from the scan is the correct result. A
`CONFIRMED` from a pre-registered mechanism at trial count 1–2 is defensible. **Widening
the percentile space to 969 conditions to make the scan look like a discovery engine would
produce `CANDIDATE` rows nobody can act on, at a correction nobody can meet.**

**AND THE PROCESS RULE THAT PRODUCED THIS REVISION, NOW BINDING ON ME:** three consecutive
Stage C revisions and this one were all caused by a threshold specified without being
executed on this frame. **§7.1 Rule 1, §7.1 Rule 2, and the §7.2 acceptance verdict were
all run before shipping, and the verdict changed the decision.** Any future rule with a
threshold in it is run and its output printed in the spec, or it does not ship.
