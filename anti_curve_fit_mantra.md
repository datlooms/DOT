# ANTI-CURVE-FIT MANTRA
## The complete scope of what a selection process must account for

Merged from `anti_curve_fit_doctrine.txt` and `ai_on_discovery_patterns.txt` (independent reviews by
Gemini and GPT, 2026-08-20). Plain language throughout. This document is the checklist the DOT selection
process is measured against — not a summary of what has been done.

---

## PART 0 — THE ONE IDEA UNDERNEATH ALL OF IT

**A test is worth running only if it would probably have caught a flaw had one existed.**

That is called **severe testing** (Deborah Mayo's formulation of Popper). The point is never to confirm the
system. The point is to build tests it could plausibly fail, and then see whether it does.

In quant finance the umbrella term is **backtest overfitting detection**.

**And the practical version, in one line: if you cannot describe how the result could have come out wrong,
you have not tested anything.**

---

## PART 1 — WHAT WE ALREADY DO, WITH ITS PROPER NAME

Every one of these has been run on the DOT system. The proper name matters because it tells you what the
test does and does not prove.

```
random-book nulls, 24 draws, incumbent at 4th percentile   randomisation / permutation test
leave-one-out on 297 members                               jackknife
random-subset ablation, 50 draws                           ablation study
split-half persistence                                     split-half reliability
rarity-matched gate nulls                                  matched null / negative control
trial-count arithmetic                                     multiple-comparisons correction,
                                                             family-wise error rate
gates.preregistered                                        pre-registration (from clinical trials)
July untouched by any fold                                 holdout / out-of-sample
60 compositions walked forward                             walk-forward analysis
Book B — the anti-system                                   negative control / placebo arm
density control vs random bars                             matched-baseline control
```

### The published methods this maps to

**PBO — Probability of Backtest Overfitting**, via CSCV (combinatorially symmetric cross-validation).
Bailey, Borwein, Lopez de Prado. Already referenced in `selection_constraint_evaluation.csv`.

**Deflated Sharpe Ratio** — same authors. Adjusts a performance figure downward for how many trials
produced it.

**Harvey, Liu & Zhu, "...and the Cross-Section of Expected Returns"** — the paper that made
multiple-testing correction standard in finance. Its argument is exactly the trial-count problem: a t-stat
of 2 means nothing when 300 factors were tested.

### The unusual one

**We built a competing system from everything the primary system excludes, ran it on bars the primary never
touches, and it came back at PF 3.51 against 14.53.** That is a negative control. Most people never build
one.

---

## PART 2 — THE ARCHITECTURE-SPECIFIC ATTACKS

These come from the outside reviews and target THIS system rather than backtesting in general. They are the
ones most likely to land, because they attack the central claim: that simultaneous agreement between
independent detectors carries information.

### A. Is depth just volatility wearing a mask?

Many of the 171 variables load on realised volatility. So "twenty things agree" may only mean "the market
is busy right now."

**Test:** stratify entry bars by volatility, then measure the depth-outcome relationship WITHIN each
stratum.

**Kill condition:** depth 3 and depth 20 score alike once volatility is held constant.

**STATUS: RUN. Partly landed.** The pooled correlation of +0.137 is Simpson's paradox — it is **negative**
in the bottom two volatility quintiles (-0.105, -0.135) and **positive** in the top two (+0.152, +0.236).
The kill condition was not met: the ladder survives and strengthens where volatility is high. But depth is
not a property of depth alone. **It interacts with volatility and changes sign.**

### B. Is depth 20 actually twenty detectors?

With 20,000 triples built from 171 variables, many are algebraic cousins. If a depth-20 bar is twenty
rewordings of four variables, depth measures feature intensity, not multi-source agreement.

**Test part 1 — vocabulary:** count distinct base variables the book uses.
**STATUS: RUN. PASSED CLEANLY.** 297 signals x 3 conditions = 891 slots drawing on **105 distinct base
variables** of 117 available. Effective count (e^H) = 83.2 against a 105 ceiling. Most frequent variable is
4.3% of slots. The kill condition was "reduces to four or five features." It does not.

**Test part 2 — firing matrix:** two triples can share zero variables and still fire on identical bars.
Compute the effective dimensionality of the signal-by-bar incidence matrix, specifically on deep bars.
**STATUS: NOT RUN.** This half can still land.

### C. Is the bar boundary the signal?

If the edge depends on where the minute boundaries fall, it is an artifact of the clock.

**STATUS: IMPRACTICAL, NOT UNTESTED.** The EA computes every variable from bar-1 data through the stateful
shift chain, and MT4 has no offset bars. Testing it means re-exporting all 172 columns at a different clock.
Recorded as a known gap.

### D. Is the concurrency cap hiding a liquidity assumption?

The cap binds on 13.79% of occupied bars. Every trade the jar refuses is a trade the backtest never had to
fill. **STATUS: PARTLY ADDRESSED** — FLOORED admission fixed the version where the backtest deleted trades a
live engine had already opened. The remaining question is whether real fills match simulated fills at the
cap. **Only demo answers this.**

### E. Meta-overfitting — the one that is hardest to see

If you try twelve selection objectives and keep the four that worked, you have overfitted at a level no
single-objective test can detect.

**STATUS: OPEN AND ACKNOWLEDGED.** Four objectives built the adopted book; two further attempts were
discarded by judgement. **The only defence is to write the rule down BEFORE the next month of data arrives
and then not change it.** That is now possible and it is the standing commitment.

---

## PART 3 — THE MATHEMATICAL FRAMING WE HAD WRONG

Both outside reviews independently landed on the same word, and it explains several failures at once.

```
submodular      each extra member you add is worth LESS      (diminishing returns)
supermodular    each extra member you add is worth MORE      (increasing returns)
```

**Greedy selection algorithms are built for submodular problems. Depth-floor convergence is supermodular —
a signal is worth nothing until enough others join it to cross the floor.**

Consequences, all measured:

- **Greedy loss-day decorrelation was never going to be stable.** Two disjoint halves of the same market
  produce books sharing 12% of members, with quality varying 2x. That is the predicted behaviour of a
  path-dependent greedy on a supermodular problem, not a bug.
- **Leave-one-out cannot see it.** Removing one member at a time cannot detect a pair that only works
  together. Split-half rho = -0.060 is what "wrong instrument" looks like.
- **Ranking members individually cannot see it either.** The adopted 297 sit mid-field on every solo
  statistic — median rank ~2,000 of 8,016. They were never selected for solo quality.

**The correct framing: choose a subset whose value is entirely in simultaneous co-occurrence.** Related
formulations worth knowing exist — hypergraph dense-subgraph, submodular set cover with saturation,
partial information decomposition, majority-logic decoding. None has been tested. They are directions, not
answers.

---

## PART 4 — THE SELECTION PROCESS CHECKLIST

**Everything a selection procedure must account for. This is the scope.**

### 4.1 The statistic it selects on must survive a split-half

Two statistics failed this and one passed:

```
F1 reach ratio                rho = -0.064    anti-predictive
d_net leave-one-out           rho = -0.060    no persistence
co-fire affinity              rho = +0.4892   PERSISTS, p = 9.6e-240
```

**If a statistic does not persist across halves, no procedure built on it can work. Test this first, before
building anything.**

### 4.2 It must not read the calendar

An objective defined against specific losing days is frame-dependent by construction. Chance-pricing is a
within-family rank against matched nulls and never reads the window — **the same 60 signals are selected on
any frame.** Loss-day decorrelation churns 88% of membership between two halves of the same market.

**A procedure that must be re-run monthly cannot have 88% turnover and still be compared against itself.**

### 4.3 It must not maximise a similarity measure

**Maximising co-firing affinity selects near-duplicates.** One arm reached depth 114 on 137 bars with 73
firings per signal — one signal counted many times. **Constraining the same measure selects anti-coupled
sets that lose to a random draw at every ceiling tested.** Both failures have the same root: Jaccard
conflates *agreement* with *similar firing frequency*.

### 4.4 Size is a parameter and it is not free

At 4,575 signals, 82.6% of entry bars reach depth 5+ and depth-exactly-3 falls to 3.3%. The tier stack
routes almost everything into one cell and **the gates that cleared nulls go inert.** Performance collapses
from PF 14.53 to 1.81 with loss events 42 to 1,113. **"Same architecture" and "take them all" are
incompatible.**

### 4.5 Floor, gates and cap are one object, not three

Proven by controlled experiment — same 297 signals, only the floor moved:

```
L3/S3   PF 14.53   42 events   119 days   cap cliff at 22
L7/S4   PF 25.91   16 events   110 days   NO cliff, still rising uncapped
```

**`Micro_Hurst > p90` — the only CONFIRMED gate in the system — fires zero times at L7/S4 because tier 3 is
unreachable.** Carrying a gate stack, a floor or a cap across from one configuration to another is a
specification error. **Derive all three together, per book.**

### 4.6 A gate must be derived from the population it will filter

Run on an already-gated table, LONG d3 gives p = 0.475. On the ungated table it gives p = 3.4e-05. **The
gate had already removed what it was derived to remove.** Never derive a gate from a population the gate has
already filtered.

### 4.7 The screen must be train-only, all the way down

A train-only screen layered on a full-sample pre-filter is not train-only. Measured cost: **1,692 signals
wrongly excluded, the eligible population understated by 37%.** And for a monthly stage it compounds — each
month the pre-filter consumes the newest data.

### 4.8 Every threshold is run on this frame before it is treated as built

Four defects were caught by executing a rule rather than re-reading its prose:

- Stage C's comparison was **monotone** — 45 of 45 candidates passed, and the test could not fail.
- A resolution rule computed on the wrong population suppressed both cells it was meant to rank.
- A null measured on the wrong quantity gave p = 0.12 where the correct quantity gives 0.0303.
- The engine canary hardcoded stale figures and went silently inert for weeks.

**A rule with a threshold in it that has not been run is not built.**

### 4.9 Every p-value carries three things or it is not a measurement

**Its quantity, its population, and whether that population was sampled or enumerated.**

A subsample of 30 gave p = 0.000; exhaustive enumeration of all 66 gives p = 0.0303. Where the population is
finite and small, **enumerate it** — it is exact and it is cheaper.

### 4.10 The bar is the risk unit, not the trade

224 trade-losses are 42 loss events. A jar admitting nine lots of the same losing trade produces nine
"losses" from one bad bar. **Every risk statistic is stated per event.**

### 4.11 A ratio on fewer than 20 events is a count

PF 197.17 on one loss event is arithmetic. PF 74.62 on one is arithmetic. **State the count and decline the
rate.**

### 4.12 Auditability is a selection criterion

A 16-event book cannot be distinguished from noise inside a year of live trading. A 42-event book can. **A
lower profit factor on more events may be the better deliverable**, and this must be weighed explicitly
rather than left implicit in a PF ranking.

### 4.13 Judge every arm against a size-matched random baseline

Random draws of the same size from the same pool, several seeds, same architecture. **An arm inside its own
band has reproduced random selection, not found anything.** The baseline is size-relative — a 150-signal
book and a 500-signal book have completely different random expectations.

### 4.14 Report the frontier, not the argmax

Days traded against margin is a monotone trade across every configuration tested. **The tool maps the
options; the operator composes the system.** An argmax hides the trade being made.

---

## PART 5 — THE THINGS THAT ARE CLOSED, SO THEY ARE NOT REBUILT

Each with the measurement that killed it. A future analyst will propose all of these again.

```
per-signal ranking              rebuilt selector lost to 12 of 12 random draws
d_net leave-one-out             split-half rho = -0.060; it measures floor-criticality, not quality
direction balance as a rule     buys nothing; worst day and losing weeks both WORSE balanced
coverage as an objective        98% of terrain reachable by firing everywhere; not a scarce quantity
coverage as a constraint        correlates with a DEEPER worst day (+0.764, p = 0.027)
participation as a constraint   ~1 day of headroom at n=297
affinity maximisation           selects near-duplicates
affinity under a ceiling        selects anti-coupled sets, loses to random at every ceiling 0.02-0.20
large book at a high floor      depth does not scale with size; 94-of-4,000 never occurs in 177,251 bars
floor sweep on a random pool    buys days, never buys quality
non-F0 families (Book B)        best pruned arm PF 3.51 against 14.53; earns $300 per loss event
                                  against the book's $6,785
gap-filler singles              42 -> 83 loss events when added; every OOS window degrades
```

---

## PART 6 — WHAT IS STILL OPEN

```
Attack B part 2         firing-matrix effective dimensionality on deep bars
volatility-conditional  the axis is real (Attack A) but 42 events across 12 cells starves the power floor;
  gating                  a parametric surface on ONE variable also failed, but it deleted two working
                          gates to do it. Adding a volatility term to the INTACT stack is untested.
convergence lifecycle   104,643 events carry onset/peak/build_rate/decay_rate. 33.8% deepen after onset.
                          The system enters at onset and has never tested any other point.
regime as an input      never tested as a selection input. Recorded as not tested, which is not the same
                          as negative.
fill realism            only demo answers it
meta-overfitting        write the rule down before the next month arrives
```

---

## PART 7 — THE ONE-LINE VERSION

**Build the test that could prove you wrong. Run it. Report what it says, including when it says the thing
you did not want to hear. Everything else is decoration.**
