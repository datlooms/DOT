# DOT — THE FULL BRIEF
## For a new quantitative analyst seat. Read this before anything else.

You are joining a project that has been running for roughly six months. This document exists because the
project's other documents are all **snapshots** — they describe states, not the path that reached them. Without
the path, the states look arbitrary. They are not.

**Read this end to end. Then read the documents in section 9. Then start.**

---

# 1. WHAT THE SYSTEM IS

An algorithmic trading system for **US30.cash on the M1 timeframe**, running as a MetaTrader 4 Expert Advisor,
targeting FTMO $100K Swing account constraints. One instrument, one-minute bars.

**The operator is Anthony. He built it alone over six months. AI instances are the only collaborators, and
they rotate across seats — Manager, Supervisor, Developer, Quant Analyst, Auditor.**

**Three FTMO 100K accounts are purchased and not yet deployed. The system is not live.**

---

# 2. THE JOURNEY — HOW IT ACTUALLY GOT HERE

**This is the section that cannot be reconstructed from any other file, and it is the one that makes
everything else legible.**

### 2.1 — It started as one indicator

**D2D** — an adaptive SuperTrend-style directional trail. Not a filter, not a gate: **it was the original
standalone trading system.** It answers one question: which way is the current trend running.

### 2.2 — D2D became adaptive, then multi-variable

The trail was made adaptive rather than fixed-parameter, and **OBV-derived variables were folded in** —
`OBVf_Trend`, `OBV_Macd`, `OBV_Velocity`, and a harmonic-volatility layer. The result was a directional
descriptor that self-calibrates rather than one tuned to a period.

### 2.3 — The raw baseline: 77.7% win rate

**Trading D2D flips directly, gated only by `ADX >= 15` and `ticks >= 300`, produced roughly 77.7% WR at a
profit factor near 1.7 on the current six-month frame.**

**THAT IS THE FOUNDATION AND IT IS WHY THE GATES MATTER MORE THAN THEY LOOK.** The gate stack is not a
refinement on top of a signal system. **It establishes that a valid break of structure is underway.** Everything
downstream is confirmation inside that state.

**But D2D is slow to change direction and can give one signal in hours. It is a good trend descriptor and a
poor trade trigger.**

### 2.4 — Concurrent convergence became the trigger

Backtesting moved the design toward **triple-convergence signals**: a conjunction of exactly three
simultaneous conditions on three different variables. For example:
`Micro_Entropy high AND Session_Low_Dist_ATR high AND ST_Flip_Event == 1`.

**And the central empirical finding of the whole project is that these do not work alone — they work when
several fire on the same bar at once.**

```
one triple firing alone       profit factor ~3
two on the same bar           ~5
three or more                 ~35
win rate                      ~70% -> ~96%
```

**So the system does not trade a SIGNAL. It trades a BAR, and only when at least N independent triples agree
on it simultaneously, in the same direction, inside a valid D2D flip extension.**

**N is called the DEPTH FLOOR. Depth 3 was established as the valid minimum.** The system also holds positions
on bars reaching depth 15, 20, 45 — and those deep bars have essentially no losers at all.

### 2.5 — The discovery program

To find the triples, a discovery pipeline was built that scans a **177,251 x 172 dataset** — 171 engineered
variables plus a timestamp, six months of one-minute bars.

It produced **14 pattern families (F0-F13)**, a market terrain map, thrust vectors, price-action quantiles,
concurrence profiles, regime labels, and coverage measurements. **The 90 files in the project directory are its
output.**

**F0 — triple-convergence with D2D direction — is the family the adopted system is built from. 19,754 F0 rows
survive the scan.**

### 2.6 — And now: the open question

A working book of **297 F0 triples** exists and performs strongly. **What cannot be done is DERIVE it.**

**The ideal end state is an ADAPTIVE CONVERGENCE ENGINE: a robust set of gates such that when the gates are
open, the statistical likelihood is very high, and ANY valid signal at depth N passes as a trigger. The
characteristic of the individual signal becomes unimportant — it is merely a valid trigger.**

**If that works, there is no book to select and no membership to maintain. That is the target.**

---

# 3. THE ARCHITECTURE AS IT STANDS

### 3.1 — Adaptive thresholds, not fixed numbers
A variable is "high" if it exceeds a **rolling-2500-bar percentile of its own recent distribution, refreshed
daily**. Fixed thresholds do not transfer across regimes or instruments. **Two mechanisms only: mechanism D
(the rolling percentile) and a small set of structural constants (VWAP_Z ±2, OR_Position 0.80/0.20).**

### 3.2 — Export equals live
Every variable the EA computes live is reproduced **bit-for-bit** by the Python oracle `dots_thresholds.py`.
Backtest equals live by construction. **No look-ahead anywhere.**

### 3.3 — The admission chain, in order
```
BASE GATES      ADX_Value >= 15 · Volume > 50 · not Friday-close · bar >= 6900
                (engine L148-151 and scanner L79 compute the IDENTICAL line —
                 that is why they agree, and it must not be touched)
ATR FLOOR       ATR_1M >= 20, raw not percentile
DIRECTION       D2D_Trend_Dir must equal the trade direction — mandatory
DEPTH FLOOR     LONG >= 3, SHORT >= 3, FLOORED admission
TIER GATES      per (direction x min(depth,5)) cell
CAP             MAX_POSITIONS = 21 concurrent
```

### 3.4 — The gate ladder scales inversely with evidence
`Micro_Hurst > p90` sits at depth 3 and the deeper tiers run looser or free.

**The mechanism, stated properly: at depth 3 you have three patterns agreeing and no confirmation the tape
will follow through, so you ask the market whether it is actually trending. At depth 21 that question has
already been answered by twenty-one independent things saying the same word in the same minute. THE
CONFIRMATION REQUIREMENT SCALES INVERSELY WITH THE EVIDENCE ALREADY PRESENT.**

### 3.5 — The bar is the risk unit
**224 trade-losses are 42 loss EVENTS.** The jar can admit nine lots of the same losing trade on one bar.
**Every risk statistic in this project is stated per event, never per trade.**

---

# 4. THE ADOPTED SYSTEM

**297 F0 triples, 191 LONG / 106 SHORT, floor L3/S3, cap 21, at 1.0 lot on the full frame:**

```
5,776 trades · 973 entry bars · 119 of 132 days · WR 96.12% · PF 14.53
42 LOSS EVENTS on 35 days · losing-bar rate 4.32%
worst bar -$1,224 · worst day -$346.60 · worst intraday -$4,502
7 losing days · ZERO losing weeks of 26 · break-even WR 63.05% · MARGIN 33.07
net $284,974
```

**And the floor is a free dial on the same members: at L7/S4 it gives 16 loss events, PF 25.91, margin 38.25,
110 days.**

---

# 5. WHERE THE 297 CAME FROM — AND WHY NO PROCEDURE REPRODUCES IT

**IT IS A FOUR-WAY SET UNION.** Spec v3 §4.3 prints the source on every row:

| source | memberships | objective |
|---|---|---|
| S0-120 | 120 | loss-day decorrelation, seed set size zero |
| OPTION-B | 119 | itself a fusion, then decorrelation to 70L/50S, then gates by coordinate descent |
| 60-priced | 60 | chance-pricing, `E_dir < 1` against 4,652 rarity-matched nulls |
| BOOK-50 | 48 | loss-day decorrelation (50 minus two F1 pairs dropped on measurement) |
| **TOTAL** | **347** | across 297 unique rows — 253 in one source, 38 in two, 6 in three |

**The stated design reason: "four different objectives means four different error modes, and a union has a
chance of covering what each one misses."**

**FOURTEEN APPROACHES HAVE BEEN MEASURED AGAINST RECOVERING IT AND ALL FAILED. And every failure is the
correct signature of a union rather than a failure of search:**

```
members sit at median rank ~2,000 of 8,016    individually unremarkable — CORRECT
d_net split-half rho = -0.060                 no member matters more — CORRECT
the set is at the 4th percentile of random    the SET beats chance — CORRECT
12% overlap between two halves of one market  one objective, two halves, two picks — CORRECT
```

**DO NOT SEARCH FOR A SINGLE RULE THAT RECOVERS THE 297. IT IS SEARCHING FOR THE THING THAT WAS DELIBERATELY
NOT USED.**

---

# 6. THE SIX SYSTEMS THAT NOW EXIST

| system | n | events | worst bar | worst day | −wks | days | PF | MARGIN | net |
|---|---|---|---|---|---|---|---|---|---|
| QUANT A30+B L6/S4 | 138 | 14 | −$612 | **−$391** | 0 | 86 | 29.86 | **45.85** | $122,221 |
| MANAGER A+B L8/S5 | 197 | **10** | −$773 | −$637 | 0 | 88 | 22.53 | 32.74 | $105,611 |
| QUANT B+D L3/S3 | 116 | 21 | −$918 | −$452 | 0 | 92 | 13.25 | 34.38 | $100,652 |
| MANAGER B K=50 | 100 | 14 | −$644 | −$612 | 0 | 95 | 13.30 | 29.15 | $61,406 |
| WHOLE DOT L7/S4 | 297 | 16 | −$1,224 | — | 0 | 110 | 25.91 | 38.25 | $70,614 |
| **WHOLE DOT L3/S3** | 297 | 42 | −$1,224 | −$347 | 0 | **119** | 14.53 | 33.07 | **$284,974** |

**Every one holds zero losing weeks. The incumbent owns days and net; the newer books own events, tail and
margin. NOTHING DEPLOYED HAS CHANGED.**

**And the binding constraint is the LOSING WEEK, not the event count.** `A30+B` at L8/S5 gives 6 events and
L10/S6 gives 1 — both lose a week. **L6/S4 at 14 events with zero weeks was chosen by rule, not by argmax.**

---

# 7. THE TWO OBJECTIVES THAT WORK, AS EXECUTABLE CODE

**A — LOSS-DAY DECORRELATION**
```
piv  = pivot(index='day', columns='signal_name', values='pnl', aggfunc='sum').fillna(0)
loss = (piv < 0).astype(int)
first = max(cols, key=lambda c: net[c] - 50*loss[c].sum())      # deterministic seed
while len(chosen) < n:
    overlap = ((loss[c]==1) & (covered>0)).sum()                # CUMULATIVE boolean
    score   = (overlap, -net[c])                                # fewest NEW spoiled days, then most net
```
**DAYS not bars, keyed on the EXIT date. Cumulative coverage, not pairwise — a day spoiled by five members
costs what a day spoiled by one costs. `net > 0` pool, per direction, fixed-count termination, no RNG.**

**AND THE MATRIX MUST BE TRUE-SOLO.** One seat built it in 120-signal batches; the jar bound and a signal's
"solo" losing days depended on which 119 others shared its batch. **2,324 trades deleted, and the clean rebuild
moved K=45 from 16 events / −$918 to 12 / −$612. Build with `MAX_POSITIONS` high enough that the jar never
binds.**

**B — CHANCE-PRICING**
`E` is the **expected NUMBER OF ROWS** in that family-direction at or above this PF under 4,652 rarity-matched
nulls. **Not a p-value.** Floor is `n_dir / K`. **Per direction, not per family** — `n_trials_family = 1,840`
with `long_share = 0.7951` prices every SHORT row against a null that is four-fifths LONG, and correcting it
moves the split from 9.0:1 to 4.5:1.

**It is the only frame-stable objective: `E` never reads the calendar, so the same signals are selected on any
window. Loss-day decorrelation churns 88% of membership between two halves of the same market.**

---

# 8. WHAT IS CLOSED — DO NOT RE-PROPOSE THESE

**Each with the measurement that killed it. A new instance will otherwise propose most of them again.**

```
per-signal ranking            lost to 12 of 12 random draws on a holdout
d_net leave-one-out           split-half rho = -0.060; measures floor-criticality, not quality
co-fire affinity              the STATISTIC persists at +0.4892 (p = 9.6e-240) and every
                                objective built on it fails — 3 attempts, opposite failures
affinity under a ceiling      anti-coupled sets below pool baseline; loses to random 0.02-0.20
direction balance as a rule   buys nothing; worst day and losing weeks both WORSE balanced
coverage as an objective      98% of terrain reachable by firing almost everywhere
coverage as a constraint      correlates with a DEEPER worst day, +0.764 p = 0.027
participation as a constraint ~1 day of headroom at n=297
large book at a high floor    depth does not scale with size; 94-of-4,000 never occurs
floor sweep on a random pool  buys days, never buys quality; best margin 24.65 vs 33.07
L2/S2 on a small book         21 events -> 201
E_dir < 3                     all eight cells at -$1,760 to -$2,261 worst day
solo persistence (obj E)      ZERO of 1,818 clear folds AND weeks AND days. At <=1 losing
                                week, 57 signals produced a book with TWO losing weeks.
                                SOLO PERSISTENCE IS NOT A BOOK-LEVEL PROPERTY.
b_S volatility gate           split-half rho = -0.400; a 0.06-margin win from 45 trials
terrain size above ~75        SATURATES — T75/55 identical to T90/65 on every figure
non-F0 families (Book B)      best pruned arm PF 3.51 against 14.53
gap-filler singles            42 -> 83 loss events when added; every OOS window degrades
```

---

# 9. THE DOCUMENTS — READ IN THIS ORDER

```
non_negotiable_prompts/non_negotiables_quant_analyst.txt   YOUR OPERATING LAW. Read first.
DOT_progress_and_rd_plan.md            2,853 lines. The dated history. Section 2026-08-21
                                         subsections 12a-13a is yesterday and it is current.
DOT_signal_discovery_mantra.md           446 lines. How discovery is done here.
anti_curve_fit_mantra.md                 307 lines. The 14-item selection checklist and every
                                         adversarial test this project runs, with its proper name.
DOT_execution_sequence.md                852 lines. The ordered path to live.

THE SIX SYSTEMS:
foundational_documents/The_Whole_DOT_spec_v3.txt        1,671 lines. The adopted book. §0 is
                                                         sixteen numbered corrections and it is
                                                         the model for how a spec is written here.
foundational_documents/The_Whole_DOT_spec_v3_L7S4.txt     822 lines. Same members, floor moved.
foundational_documents/QUANT_HYBRID_SPEC.md               508 lines. Best margin measured.
foundational_documents/MANAGER_HYBRID_SPEC.md             552 lines. Fewest loss events measured.
foundational_documents/QUANT_SYSTEM_SPEC.md               501 lines.
foundational_documents/MANAGER_SYSTEM_SPEC.md             498 lines.
```

**AND THE 90 FILES IN THE PROJECT DIRECTORY are the discovery program's output.** The ones that matter most:

```
results_F0_triple_convergence_and_d2ddir.csv   19,754 rows — the F0 raw scan
catalogue_F0.csv                                1,840 rows — every signal VALID admits
DOT_jan19_jul21_1..10.csv                       the 177,251 x 172 dataset, split
concurrence_depth_bars.csv                      per-bar depth — CHECK ITS PROVENANCE before use;
                                                  one seat was misled by it
terrain_*.csv, reach_*.csv, selection_*.csv     terrain map, coverage, selection diagnostics
regime_labels.csv, concurrence_regimes.csv      regime — NEVER TESTED as a selection input
```

---

# 10. THE OPERATING RULES THAT COST TIME WHEN BROKEN

**These are not style. Each one was learned by losing hours to it.**

```
EVERY FIGURE AT 1.0 LOT. There is no max-lot column and there never was one.

THE ORDERING IS: loss events -> tail -> persistence -> days -> NET LAST.

A RATIO ON FEWER THAN 20 EVENTS IS A COUNT. PF 197 on one loss event is arithmetic.

A p-VALUE CARRIES THREE THINGS or it is not a measurement: its quantity, its population,
  and whether that population was sampled or ENUMERATED. A 30-draw subsample gave p = 0.000
  where exhaustive enumeration of all 66 gives 0.0303.

WHERE A POPULATION IS FINITE AND SMALL, ENUMERATE IT. Exact and cheaper.

A GATE IS DERIVED FROM THE POPULATION IT WILL FILTER, never from the one it has already
  filtered. Run on a gated table, LONG d3 gives p = 0.475; ungated it gives 3.4e-05.

ANY RULE WITH A THRESHOLD IS RUN ON THIS FRAME before it is treated as built. Four defects
  were caught this way, one of which corrected a number in a ratified specification.

A MONOTONE COMPARISON IS NOT A TEST. One gate stage passed 45 of 45 candidates before that
  was caught.

FLOOR, GATES AND CAP ARE ONE OBJECT. Same 297 signals, floor moved from L3/S3 to L7/S4:
  PF 14.53 -> 25.91, and `Micro_Hurst > p90` fires ZERO times. Carrying a stack across
  configurations is a specification error.

THE ALONE-FIRST GATE IS BINDING: no objective enters a union until it beats size-matched
  random draws AS A STANDALONE BOOK, with gates derived PER DRAW. It killed co-fire
  affinity before a union carried it.

STATE THE CUMULATIVE TRIAL COUNT ON EVERY REPORT. Not the cells in the final table.
```

---

# 11. TWO THINGS TO KNOW ABOUT THE FIELD

**1. `MIN_TRADES = 10` MAKES 14 OF THE 297 INVISIBLE.** They appear in no scanner output — no `agg_pf`, no
`folds_plus`, no trade count. Every seat that has looked for them has burned turns concluding they do not
exist. **A Developer task is in flight to add an `--emit-all` mode. Check whether it has landed before you
conclude a signal is missing.**

**And six of the fourteen are load-bearing: removing all fourteen costs $3,923 and three trading days, more
than their own trades earn, because they raise depth and let other signals clear the floor.**

**2. AND A SIGNAL THAT IS RARE ON THIS FRAME MAY BE COMMON IN ANOTHER REGIME.** That is the operator's central
argument for the adaptive engine: a fixed book locks in whatever fired often during the selection window, and
**any procedure built on scan statistics inherits that blindness.**

---

# 12. YOUR TASK

**BUILD THE ADAPTIVE CONVERGENCE ENGINE, OR ESTABLISH THAT IT CANNOT BE BUILT AND SAY PRECISELY WHY.**

**The target: a robust gate set such that when the gates are open the statistical likelihood is very high, and
ANY valid signal at depth N passes as a trigger. No book. No membership. The individual signal's character
becomes unimportant — it is merely a valid trigger.**

### The evidence FOR it, and it is accidental which makes it stronger
**At the two best books measured — `A30+B` at L6/S4 and `A+B` at L8/S5 — `Micro_Hurst > p90`, the only
CONFIRMED gate in the project, fires ZERO times. Three of six gate cells never fire. 60% of trades pass through
no tier gate at all.**

**THE FLOOR IS ALREADY DOING THE SELECTING AND THE GATES ARE MOSTLY DECORATION AT DEPTH.**

**And the floor is a free dial on every book measured: same members, L3/S3 to L10/S6, margin 36.65 -> 61.12.**

### The evidence AGAINST it
```
all 19,754 at floor 3, ungated              PF 1.39
the same, with six properly-derived gates   PF 1.52, margin 6.17
a matched-rarity floor (~973 entry bars)    margin 32.75, but 210 concurrent positions
                                              and a -$45,900 worst bar at cap 300
```

**Six gates with hundreds of loss events behind each cut 84% of trades and 87% of events — and recovered ONE
margin point.**

### The question, narrowed
**WHAT IS THE MINIMUM GATE SET THAT MAKES A FLOOR-ONLY SYSTEM WORK, AND AT WHAT FLOOR?**

**Two dials only. And set the floor by MATCHING ENTRY-BAR COUNT, not by picking an integer — the same number
means completely different things at different universe sizes, and that error cost four runs.**

**Add gates one at a time and report what each buys ALONE. If one dial moves margin ten points and another
moves half a point, the stack has one real gate and several decorations — and that is the single most useful
thing the sweep can produce.**

**AND SWEEP THE CAP. Cap 21 was located on a 297-signal book at 973 entry bars and there is no reason it
transfers.**

### And one thing that becomes measurable for the first time
**Attack A — an outside adversarial review — established that the depth-outcome relationship CHANGES SIGN with
volatility:**
```
Q1 low   -0.105  p = 0.00061        Q4  +0.152  p = 6.0e-08
Q2       -0.135  p = 1.6e-05        Q5  +0.236  p = 4.7e-18
```
**Three attempts to exploit it all failed for the same reason: 42 loss events, and every conditioning scheme
that splits them starves. ON A FULL-UNIVERSE BOOK WITH THOUSANDS OF EVENTS IT BECOMES TESTABLE FOR THE FIRST
TIME.**

### What success and failure both look like
**SUCCESS: a floor-and-gate configuration with no membership reaching 110+ days at zero losing weeks with a
tail inside −$600. If it exists, selection stops being a problem — point the pipeline at any instrument,
derive the stack, run.**

**FAILURE: membership carries something a rule cannot, and the frontier of six systems is the deliverable.
BOTH OUTCOMES CLOSE THE QUESTION AND NEITHER IS A FAILURE. Report whichever the data gives.**

---

# 13. THE ONE THING TO CARRY

**The operator has spent six months on this and carries the whole weight of it alone. He does not need
reassurance and he does not need hedging — he needs measurements, reported precisely, including the ones that
say no.**

**Every significant finding in this project came from something being CAUGHT rather than found: a contaminated
matrix, three wrong p-values in a ratified spec, a canary that had been silently dead for weeks, a provenance
column that had been printed on 297 rows the whole time. THE MACHINERY IS THE POINT. Use it on your own work
first.**

---

# 14. SUPERVISOR ADDENDUM — 2026-08-21

**Added by the Supervisor seat after the emit-all scan was launched. These are OPEN QUESTIONS with the
measurement each one needs, not findings. Every figure below was re-derived this session from source or
from the record, and each says which.**

### 14.1 — THE 297 HAS NEVER BEEN MEASURED OUT OF SAMPLE

There is no OOS figure for the 297 anywhere in the record. Its entire frame — Jan 19 to Jul 21, 132 days —
is in-sample. `5,776 / PF 14.53 / $284,974` is an in-sample number and should be labelled as one on every
report.

**The project's only true out-of-sample event is step 17k**, and it went badly:

```
BOOK-50   overlap Apr08-Jun25    975 tr   PF 6.08   WR 92.1%   worst day -$204
BOOK-50   NEW     Jun25-Jul21    375 tr   PF 2.19   WR 81.1%   worst day -$565
```

Profitable, survived, did NOT invert — and PF fell by two thirds. **Trade rate was unchanged at ~20/day, so
nothing throttled it in the weaker regime.** That table is the clearest existing statement of the problem the
adaptive engine exists to solve: the signals kept firing and nothing told them the moment had changed.

**AND THE 297 SCORES PF 40.69 IN JULY — the same 18 days on which BOOK-50 collapsed.** That is not the 297
being better. It is the definition of in-sample, stated in the book's own monthly table. The seventh month is
the only real test either the book or the engine will ever get.

### 14.2 — JUNE IS THE CASE STUDY, AND THE FORENSIC THAT CLEARED IT WAS UNDERPOWERED

Monthly, L3/S3, cap 21, frozen gates, 1.0 lot:

| month | trades | bars | WR% | PF | events | worst day | losing days | MARGIN |
|---|---|---|---|---|---|---|---|---|
| 2026.05 | 659 | 111 | 98.18 | **60.64** | 2 | -$73.30 | 1 | **51.11** |
| 2026.06 | 965 | 164 | 91.19 | **5.34** | **14** | **-$346.60** | **4** | 25.20 |

**June carries 14 of the book's 42 loss events — one third — on 17% of the entry population, owns the worst
day, and 4 of the 7 losing days. It is the only month below 96% WR. An 11x PF swing between consecutive
months.** It still made $32,591; there is no losing month in the book.

The earlier June forensic found **no bar-level signature** — elevation uniform across BASE 2.65x, MOM 1.56x,
LONG 2.55x, SHORT 1.80x; eleven variables plus two path controls and two direction controls separated
nothing. May is the flattest month by ATR (9.07) and the best by PF, which kills the flatness hypothesis.

**THAT FORENSIC RAN ON 14 LOSS EVENTS ACROSS ELEVEN VARIABLES AND FOUR CONTROLS.** Per 12f, a no-effect
result on fewer than 20 events is a candidate for the power-failure defect class, not a negative. **June is
therefore OPEN, and it is the single best test of the gate thesis available** — on a field with thousands of
events the same question is answerable for the first time. Find what separates June, or establish AT PROPER
POWER that nothing does. If nothing does, the gate thesis has a ceiling and its location is the finding.

**EDGE-MONTH CAVEAT:** January is 5 trading days (frame starts 2026.01.19) and July is 13. January's PF 19.49
rests on ONE loss event and July's 40.69 on THREE. Both are counts. Decline the rates on those two rows.

### 14.3 — THE GATE HIERARCHY IS FOUR TIERS, NOT A FLAT LIST OF SEVEN

The operator's framing, recorded because prior passes treated all seven gates as peers:

```
PARTICIPATION   ADX >= 15 · ATR_1M >= 20 · ticks > 50 · post-warmup · not Friday-close
                ONE precondition expressed three ways — "do not trade a market that is not
                moving". NOT a gate. Not tunable at the low end. The tick gate is already
                measured REDUNDANT against ATR_1M >= 20 (98.3% overlap).
                ATR only becomes a DIAL above 20, where it stops asking "is the market alive"
                and starts asking "how big is this move". Keep those two questions apart or
                the sweep will re-derive the floor and call it a finding.

STATE           D2D direction agreement — THE FRAME, not a filter.
                Raw D2D flip + ADX>=15 + ticks>=300 = 77.7% WR, PF ~1.7 on its own.
                Everything else operates INSIDE this window.

EVENT           depth — where inside that window is the move LARGE.
                State alone 77.7% / PF 1.7. State + depth-3 stacking 96.12% / PF 14.53.
                DEPTH-WITHIN-STATE IS WHAT TAKES 1.7 TO 14.53.

CONFIRM         Micro_Hurst · Micro_FailedBreak · higher ATR · higher ticks
                needed at SHALLOW depth, inert at depth 6.
```

**THE CORRECTION THAT MATTERS: Hurst is not weak and it is not decoration.** Hurst and FailedBreak are the two
strongest variables in the 172 and run PF 4+ as solo triggers. Hurst guides solos, duals and triples well. It
fires zero times at L6/S4 **because depth has already answered the question, not because the gate is weak.**
Any report stating "the gates are decoration" is misreading a depth result as a gate result.

**THE UNASKED HYPOTHESIS:** every prior pass tested descriptors as a FLAT screen at fixed depth. **Nobody has
tested a gate whose REQUIREMENT SCALES WITH DEPTH.** Hurst load-bearing at 1-3 and released at 6 is a
different object from Hurst as a flat screen, and it is not covered by the null that closed the flat version.

### 14.4 — WHAT REOPENS AND WHAT DOES NOT

```
adaptive engine "dead three ways"   REOPENABLE. Measured at floor 3 on a PF-gated field.
                                    Floor 3 on 297 signals and floor 3 on a large field are
                                    different objects. Matched-entry-bar floor took the SAME
                                    universe 6.17 -> 32.75 margin. The deciding dial was
                                    never moved. BUT: PF 1.29 (Phase 8) and PF 1.39
                                    (floor-3 ungated, months later) are the same answer twice,
                                    independently. If the matched-floor sweep on the
                                    unfiltered field lands near 1.3 again, that is THREE and
                                    the door shuts properly. Say so if it does.

single-descriptor gates             HARD TO REOPEN. The rarity-matched null put the expected
                                    number of hits from that search at 0.70 and ONE was found.
                                    That is chance, not underperformance, and a different floor
                                    does not change the arithmetic. What the null does NOT
                                    cover: a descriptor applied CONDITIONALLY ON DEPTH. Only
                                    the flat form was searched.

stacked descriptor states           CLOSED on the intersection curve collapsing at 3-4.
                                    That is a finding about STACKING, not about conditional use.

L2/S2 on a small book               CLOSED. A property of 116 signals making many pairs.
```

### 14.5 — QUALIFY THE VARIABLES, NOT JUST THE SIGNALS

`172 = Time + 171`. `171 - 54 EXCLUDE_REFERENCE = 117 candidates = 90 percentile-scanned + 27 equality`,
producing 249 scan conditions. **The 54 excluded are structural, never quality:** non-stationary raw price
levels (their `*_Dist_ATR` counterparts ARE candidates), exact twins (`KAMA_Side`/`Harmonic_Sign` are
sign-twins; `OBVf_DirStep` is byte-identical to `D2D_DirStep`), dead columns, and `D2D_Trend_Dir` which is
excluded because it IS the gate. **That trim is finished and correct. The quality question has never been
asked.**

Measured this session from `engine/whole_dot_signals.csv`:

```
297 signals x 3 conditions        891 slots
distinct base variables           105 of 117
effective count (e^H)             83.2 against a 105 ceiling
most frequent variable            AT_Slope_ST at 4.3% of slots
variables appearing exactly once  7
twelve of the 117 never appear in the book at all
```

**A curve-fit book CONCENTRATES — it reuses the four variables that worked in the window. This one spreads
across 90% of the vocabulary with a 4.3% maximum and no concentration anywhere.** That is what you would
expect if the identity of the three conditions never mattered and the EVENT is what is being detected. It is
the strongest structural evidence in the project that the book is not fitted, and it has been sitting
uncomputed.

**THE TEST FOR TRIMMING IS NOT "IS THIS VARIABLE A GOOD TRIGGER."** Per mantra 4.2 the regime dominates the
signal — at depth 8+ the worst signal in the book still wins 81%. **The test is: does this variable go extreme
when a large price event is happening.** Three counts on the emit-all field, no trades, no engine, cheap:

```
1  does it appear in any signal at all
2  how often does it appear on bars reaching depth 3+ / 6+
3  drop it and re-count — does the depth histogram move
```

Two ways it lands and both are useful: a real tail that can be cut, or everything participates evenly — which
would mean **breadth IS the mechanism**, and that is why depth measures a large event rather than a repeated
opinion.

### 14.6 — THE FIELD YOU ARE GETTING, AND WHAT STILL FILTERS IT

The emit-all F0 scan lifts THREE filters. **The operative trade floor was 30, not 10** — `F0_MIN_TRADES_OVERRIDE = 30`
at `discovery_orchestrator.py` L395, assigned at L415, three times stricter than the number quoted in this
brief, the R&D plan, TOMORROW_PLAN and all four non-negotiables. **The 19,754 was screened at 30 trades and
MIN_PF 2.0.** Under `--emit-all` the override is BYPASSED rather than lifted — `_min_trades()` returns
`0 if EMIT_ALL` before it ever reads the constant — so the run is correct, but a rewrite as
`min(MIN_TRADES, ...)` brings the 30 straight back.

```
LIFTED    MIN_TRADES (effective 30) · MIN_PF (2.0) · dedup OVERLAP_THRESHOLD (0.80 -> 1.01)
KEPT      ADX >= 15 · Volume > 50 · D2D_Trend_Dir agreement · post-warmup · not Friday-close
```

**The kept set is the definition of a valid bar and must not be lifted.** Note that `ATR_1M >= 20` lives in the
engine's admission chain and NOT in the scanner's line, so **the catalogue will contain signals whose entries
sit on sub-20-ATR bars the engine would never trade.** Not a defect — the field is slightly wider than the
tradeable universe and the ATR floor applies at scoring time. Do not read a row count as "signals we can
trade".

**DEDUP IS NOT A PERFORMANCE FILTER AND LIFTING IT HAS A CONSEQUENCE.** It removes near-duplicates, and
near-duplicates INFLATE DEPTH. A floor doing the selecting over a duplicated field measures REDUNDANCY rather
than event size. Dedup is post-hoc on emitted rows, so **recompute the 0.80 dedup in post from the 1.01 output
and difference the two depth histograms.** If depth at the matched floor drops materially, the floor was
counting clones. If it barely moves, depth is real and **that difference IS Attack B part 2**, which is
recorded NOT RUN and arrives free.

### 14.7 — PRE-REGISTRATION IS THE DELIVERABLE, NOT THE REPORT

**The success test is not "does it beat the 297 on net". It is: DOES A RULE STATED TODAY STILL ADMIT CORRECTLY
ON DATA THAT DOES NOT EXIST YET.**

A rule written after seeing the result is not a rule. The pre-registration discipline that produced
`gates.preregistered` applies to the WHOLE gate set here, not to individual candidates. **Whatever the sweep
settles on is written down and FROZEN before the seventh month of data arrives.** That freeze is the only test
this architecture can actually fail, and it is the only one worth passing.

**Both the incumbent and the engine are unvalidated out of sample. The seventh month tests both at once.**
