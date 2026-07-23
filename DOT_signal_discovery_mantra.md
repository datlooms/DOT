# DOT — THE SIGNAL DISCOVERY MANTRA
## The blindfolded cake — how to look at the data, and what not to conclude from it

**Status: DOCTRINE.** This governs how signal discovery is approached, by every seat, on every run.
It is not a finding and it is not superseded by one. Findings are measured and change; this is the
posture that decides whether a measurement means anything at all.

Read this before any discovery run, any selection decision, and any interpretation of a result.

---

## 1. THE ANALOGY

You are blindfolded. Someone hands you a fork and tells you there is a cake.

You take a bite. It tastes good. What just happened?

You do not know. You cannot know. Because with the blindfold on, **all four of these feel identical
in the mouth:**

| What you actually ate | What it feels like |
|---|---|
| The first bite of a whole, untouched cake | A good bite |
| A bite from the middle of a cake being eaten | A good bite |
| A bite of a cake that is nearly gone | A good bite |
| A crumb left on the plate after the cake is finished | A good bite |

Every one of them is sweet. Every one registers as success. And **not one of them tells you how big
the cake is, where you are in it, or whether there is anything left.**

That is the position signal discovery has been operating from.

---

## 2. WHY THIS IS NOT A METAPHOR — IT IS THE MEASURED SITUATION

The dataset is **177,251 bars x 172 variables**. Measured from price alone, with no signals involved,
it contains thousands of clean directional thrust episodes — sustained moves exceeding a causal
ATR-normalised threshold with a directional-efficiency filter.

The committed book contains **50 signals**.

**LABELLING NOTE — this document holds itself to rule 1.** Episode counts and participation rates are
PARAMETER-SENSITIVE: they depend on the forward window W, the magnitude threshold K, the efficiency
threshold E, and the clustering tolerance N. Measured on the same data, episode counts range from
~2,400 (W=60) to ~4,700 (W=15), and the eligible-universe mask moves counts by a further ~2x. Any
figure quoted below carries its parameters. A participation figure without its parameters is not a
measurement.

Measured participation, W=30 / K=p85 / E=p75 / post-warmup mask:

| | Episodes | Book traded | Book missed |
|---|---|---|---|
| UP-thrusts | 2,684 | 11.6% | **88.4%** |
| DOWN-thrusts | 2,705 | 10.6% | **89.4%** |

At the spec's §D.0.2 operating point the same relationship reads 3,067 episodes at 4.1% / 3.0%
traded. **The parameters move the level by several-fold. They do not move the conclusion at all:**
across every cell tested, the book participates in a small single-digit-to-low-double-digit
percentage of the market's clean directional moves, and the up/down split of the opportunity stays
within +/-0.6pp of 50/50.

And of the episodes it misses, **89.8% are places where not one signal in the book fires at all.**
Not blocked. Not busy. Absent.

**The book is not a system with a coverage gap. It is a handful of crumbs, and the terrain it samples
has been mistaken for the terrain itself.**

---

## 3. THE ERROR THIS DOCTRINE EXISTS TO PREVENT

The recurring failure is not a bad measurement. It is **measuring the market through the book, and
reporting the reading as a property of the market.**

Documented instances, all real, all from this project:

- **"US30 drifts upward, so a long-dominant book is natural."** Measured from price alone, thrust
  episodes are **50.0% up / 50.0% down**, and the down-moves are *larger* (83.5 vs 76.5 pts median).
  Every month sits near 50/50 — even the strongest up-month was 44.3% down-thrusts. The book takes
  **13.8%** of its net from shorts. The asymmetry is in the instrument, not the market.

- **"March was the best month and it was a crash, so shorts must be working."** In March, longs made
  **$22,394** and shorts **$3,067**. The crash was monetised by dip-buying longs. The reading said
  nothing about what the crash offered — only about what a 37-long/13-short book was built to take.

- **"The overlap divergences are cold-start effects."** Never checked where they occurred. The
  warm-up region had max diff **0.000000**; the divergences sat on one day, entering via a single
  High-bar discrepancy and self-healing the same session.

- **"The AT_Regime_ST directional gate is the headline fix."** The figures came from a filter that
  ignored trade direction entirely. Measured properly the gate is **PF 0.97, net -$111** — it removes
  the book's profit.

- **"D2D adds nothing."** Measured on the wrong column. The cohort called harmful produced **$29,700
  at PF 5.04** — the best bucket in the file.

Every one of these was a confident conclusion drawn from a blindfolded bite.

---

## 4. WHY THE CONCURRENT CONVERGENCES ARE THE TASTIER BITES

This is the part the blindfold hides, and it is the finding the project fought against for months
before the data settled it.

**A bite is not a bite. Depth is what distinguishes a mouthful of cake from a crumb.**

Measured three independent ways — single-bar depth, temporal cluster size, and pre-jar qualifying
depth — the gradient is monotonic and it holds in both segments:

| Cluster size | WR | PF |
|---|---|---|
| 1 (solo) | 86% | 2.33 |
| 2 | 86% | 2.83 |
| 3-4 | 88% | 3.14 |
| **5-7** | **95.5%** | **11.39** |
| 8-12 | 92.5% | 6.30 |
| **13+** | **96.0%** | **11.78** (worst day -$12) |

And measured by qualifying depth before the jar admits anything: **5-6 qualifying = PF 87.6;
7+ qualifying = 100% WR.**

**Mechanism, measured not assumed:** solo entries carry an average loss 2-3x their average win. They
are solvent only on an extreme win rate and collapse the moment it slips. Concurrent entries have
balanced payoff and degrade gracefully. That is why depth-1 is PF 3.16 with a -$574 worst day while
depth 3+ took **zero losses in both segments**.

**The crumb tastes the same as the cake. It is simply much smaller, and it runs out.**

### 4.1 The bites have a shape

Within a large cluster (N=10, size >= 8, BOOK population, gaps excluded, normalised position
j/(size-1) so the first entry sits at 0 and the last at 1):

| Position in cluster | PF | Avg trade |
|---|---|---|
| First 25% | 7.74 | **$46.58** |
| **Second 25%** | **19.43** | $41.91 |
| Third 25% | 8.32 | $32.14 |
| Last 25% | 6.79 | $27.09 |

**Two different shapes, and the distinction matters.** Profit factor ARCS — it peaks in the second
quarter at nearly 2.5x the first. Average trade DECLINES MONOTONICALLY from the first quarter
onward. So the opening quarter carries the biggest wins *and* the biggest losses; the second quarter
is the best risk-adjusted eating; everything after that falls away. The last quarter is worth roughly
a third of the second on PF and 58% on average trade.

*(An earlier version of this section used the convention (j+1)/size, which shifts every entry right
and made average trade appear to peak in the second quarter alongside PF. Both conventions reproduce
exactly under their own definition; j/(size-1) is the correct normalisation and is what the spec
adopts. Note also that the monotonic-decline half is tolerance-specific: at N=5 average trade rises
Q1 to Q2 before falling. Q4 < Q1 at both tolerances.)*

**The cake has a shape, and it is not flat. Later bites are smaller and riskier than earlier ones.**

**But — and this is the part that cannot be traded on — you cannot know where in the arc you are
while you are eating.** Normalised position requires the cluster's final size, and the final size is
unknowable until the cluster has ended. At fire time the system knows how many entries have already
occurred and never how many will follow. This is the first-entry problem mirrored at the other end,
and the spec correctly refuses to specify a taper on an uncomputable quantity. The arc is a measured
property of the terrain; whether any causally-available proxy recovers it is an open measurement, and
a documented negative is a permitted answer.

### 4.2 Inside a big enough bite, the fork does not matter

Spread of win rate **across different signals**, by depth:

| Depth | Signals | WR spread (sd) | Worst signal | Mean |
|---|---|---|---|---|
| Shallow (1-2) | 20 | 9.01 | **70%** | 86.5% |
| Mid (3-7) | 36 | 4.67 | 80% | 91.3% |
| Deep (8+) | 33 | 5.52 | **81%** | **94.5%** |

At shallow depth, which signal fired matters enormously. **At depth 8+, the worst signal in the book
still wins 81% of the time.** The regime dominates the signal.

**Implication for selection, and it is the central one: the job is not to find excellent signals. It
is to find signals that are PRESENT when a real bite is available.** A mediocre signal that reliably
participates in depth is worth more than a brilliant one that fires alone on an empty plate.

---

## 5. WHY THE SHORT SIDE HAS BEEN EATING CRUMBS

Reach is symmetric. Depth is not.

| | Signals | Clusters | Mean depth | Max | Reach depth 5+ | Solo share |
|---|---|---|---|---|---|---|
| LONG | 37 | 587 | 3.38 | 39 | **19.3%** | 28.4% |
| SHORT | 13 | 404 | 1.72 | 11 | **3.0%** | **56.9%** |

The book shows up to down-moves at almost exactly the same rate it shows up to up-moves — 10.6% vs
11.6%. And then, on the short side, **it never builds anything.** Fifty-seven percent of short
clusters are a single brick.

This is combinatorial, not qualitative. Shorts are active on *more* days per signal (38 vs 31) with
comparable trade counts (49 vs 51). **Thirteen signals cannot co-fire into a pyramid. Thirty-seven
can.** The old funnel handed one side of the market a fork and the other side a toothpick.

That is a property of the selection process. It is not a property of US30.

---

## 6. THE MANTRA

> **Take the blindfold off before deciding what the cake tastes like.**

Operationally, that means five things:

**1. MEASURE THE CAKE, NOT THE BITE.**
Any claim about the market must be measured from price, or from the full variable set, or from the
full episode population — never through the book. The book is an instrument with known bias. Reading
the instrument and reporting the market is the error that has cost this project the most.
Every finding must be labelled: *property of the MARKET*, or *property of the BOOK*.

**2. INCLUDE, THEN LET THE EVIDENCE SORT.**
Nothing is removed on a single measurement. No variable, condition, mechanism or family. Gates are
recorded as STATE COLUMNS, never applied as row filters — every bar is retained, every candidate
scored both gated and ungated, so no decision is irreversible and every counterfactual stays visible.
A conclusion that deletes data cannot be revisited when it turns out to be wrong. And it has turned
out to be wrong, repeatedly.

**3. NO PRE-SET TARGETS.**
Do not tell the search what to find. No quotas, no floors, no directional targets, no expected
composition. A number chosen in advance guarantees the answer confirms it. If the correct structure
is 45 long and 5 short, that must be a *measured outcome*. If it is 25 and 25, likewise. Composition
is an OUTPUT to be reported, never an INPUT to be constrained.

**4. DEPTH IS THE UNIT OF QUALITY, NOT THE SIGNAL.**
Score candidates on their contribution to depth in the right direction at the right moment — not on
their standalone statistics. In-sample profit factor did not predict forward performance
(correlation ~0). Depth did, three ways over, monotonically, in both segments.

**5. NEGATIVE CONCLUSIONS REQUIRE THE SAME BURDEN OF PROOF AS POSITIVE ONES.**
"This does not work" is a claim about the data and needs the same evidence as "this works." A weak
result is one data point that redirects the search — never a verdict that closes a line of inquiry.
The specific failure mode to guard against: **a single negative conclusion hardening into a lens that
pre-interprets everything measured afterwards.** Concurrence was dismissed for months on exactly that
basis and is now the most-corroborated finding in the project.

---

## 7. WHAT THIS DOCUMENT FORBIDS

- Concluding anything about market structure from a book-derived measurement without saying so.
- Removing a variable, condition, mechanism or family on one measurement.
- Deleting rows anywhere in the pipeline, for any reason.
- Setting a target, floor, quota or expected composition before a search runs.
- Ranking candidates on standalone performance while ignoring depth participation.
- Treating an unexplained result as a failure rather than as an unmapped region of the cake.
- Carrying a prior instance's pessimism forward as if it were a finding. A conclusion binds only if
  it is recorded in the non-negotiables or the progress doc AND backed by verified clean data.

---

## 8. WHAT IT REQUIRES

- Every measurement labelled MARKET or BOOK.
- Every candidate scored gated AND ungated.
- Every design decision carrying its motivating measurement, its falsification condition, and its
  fit risk against the span it was measured on.
- Depth-participation scored as a first-class property of every candidate.
- Directional composition, family composition and coverage reported as headline outputs of any run.
- Where the data cannot answer a question: say so, and specify the measurement that would.

---

## 9. THE STANDING REMINDER

The system did not fail. **The sampling was too sparse to know whether it was succeeding.**

When results were strong, we called it edge. When they weakened, we called it regime change. Both
were readings off an instrument with fifty forks in a cake with 5,389 slices — and neither claim was
supportable at that density.

The redesign exists to widen the sampling, not to prune it further. Every subtractive instinct
encountered from here should be checked against this document first.

**Take the blindfold off. Find out how big the cake actually is. Then decide what is worth eating.**
