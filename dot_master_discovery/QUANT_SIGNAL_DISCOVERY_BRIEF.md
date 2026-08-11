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

## C2. THE CONVERGENCE GRADIENT SURVIVED A NULL TEST

The depth gradient could have been a property of clustering rather than signal quality. 50
random triples from the same 243 firing conditions, three seeds, identical build and scoring
path, no filtering:

    depth ladder      solo    dual    3-4      5+      trades at 5+
    random seed 11    1.07    0.95    0.92    999            5
    random seed 22    1.01    0.86    0.80     n/a           0
    random seed 33    1.13    1.07    1.14    999            5
    REAL BOOK (LONG)  3.59    2.43    8.68   39.40         387

**Random shows NO gradient — flat to declining — and cannot reach depth at all** despite making
3-4x more total trades. **The gradient is signal quality.** Full detail and caveats in
`cake_dictionary.md` section 4F.

**Still untested:** three has never been competed against a same-bar PAIR or a 4/5-variable
grammar. This shows three beats random, not that three beats two.

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

# THE OBJECTIVE — 4x THE GATED SYSTEM, SAME STATS OR BETTER

**This is not "compose a book from the catalogue." It is: take a system that already works and
make it four times larger without degrading it.** Everything you measure should be read against
this.

## THE SYSTEM BEING EXPANDED

    solo    -> Hurst p90 AND ticks >= 300
    double  -> Hurst p90
    triple+ -> FREE

    BOOK-50, full span, 1 lot:
      solo + Hurst + ticks     146   WR 91.1%   PF  6.49   net  $6,877   wd -$187.6
      double + Hurst           104   WR 96.2%   PF 16.18   net  $2,575   wd  -$50.8
      triple+ free             505   WR 98.0%   PF 53.70   net $26,616   wd -$138.9
      GATED TOTAL              755   WR 96.4%   PF 19.71   net $36,068   wd -$130.7   27 losses

    Unseen 18 days, gates unfitted:  ungated PF 2.25 -> gated PF 16.63
      96% of the money at 28% of the worst day and 8% of the losses.

## THE TARGET

    tier            now              ->  target
    solo            146   $6,877     ->    584   $27,508
    double          104   $2,575     ->    416   $10,300
    triple+         505  $26,616     ->  2,020  $106,464
    TOTAL           755  $36,068     ->  3,020  $144,272

    WR       >= 96.4%       PF >= 19.71
    WORST DAY MUST STAY AT ROUGHLY -$130.7 — IT MUST NOT SCALE TO -$523.

**THAT LAST LINE IS THE WHOLE DIFFICULTY.** Four times the trades at the same tail means the
added trades must lose on DIFFERENT DAYS from the ones already there. That is decorrelation.

BOOK-48 was built that way — per-signal daily P&L vectors, greedy admission by minimum co-fire
of LOSS days, plus a leave-one-out trim removing high-standalone-PF signals whose losses were
correlated. **Treat that as a starting point, not a prescription.** It was blind to three things
you now have: the long/short asymmetry, the terrain map, and the possibility of gating each
direction differently. A selection method that produced 37L/13S without anyone choosing that
split is not the method to inherit unexamined.

## HOW MANY SIGNALS THAT IMPLIES

Same-bar triples scale roughly as C(n,3):

    48 signals  ->  C(n,3) 17,296   1.00x   ~505 triples   (BOOK-50 today)
    60 signals  ->         34,220   1.98x   ~999
    72 signals  ->         59,640   3.45x   ~1,741
    76 signals  ->         70,300   4.06x   ~2,052        <- the target lands here
    84 signals  ->         95,284   5.51x   ~2,782
   100 signals  ->        161,700   9.35x   ~4,721

**~76 signals is the arithmetic MINIMUM for 4x triples. THE OPERATOR'S TARGET IS 100.**

100 implies ~4,721 triples — well past the 2,020 needed — which is headroom, not a problem. The
acceptance rule decides how many are kept, not the arithmetic.

**AND DO NOT READ THE 100-SIGNAL ILLUSTRATION AS EVIDENCE AGAINST 100.** It produced 2,635
triples at win/loss 0.64 because it was ranked PURELY ON RARITY, with one gate applied to both
directions and no decorrelation step at all. That is evidence that THAT selection was bad. It
is not evidence that 100 signals cannot hold the ratio. **Nobody has yet tried 100 selected per
direction, decorrelated on loss days, and gated by side.**

## THE GUARD — WHY MORE TRIPLES IS NOT AUTOMATICALLY PROGRESS

    break-even WR = avg_loss / (avg_win + avg_loss)
    margin        = actual WR - break-even WR

    BOOK-50 corrected          win/loss   break-even   margin    losses
      solo                       0.40       71.4%     16.8pp      144
      double                     0.52       65.7%     24.8pp       94
      triple+                    0.99       50.3%     46.9pp       14

    100-signal illustration    win/loss   break-even   margin
      triple+                    0.64       61.2%     30.1pp
      five+                      0.81       55.2%     35.0pp

**BOOK-50's triples pay $1 for every $1 risked. The expanded book's pay 64 cents.** It reached
2,635 triples — past the target — and the payoff shape collapsed. **PF 6.61 looks respectable
and conceals all of it.**

> ### THE ACCEPTANCE RULE
>
> **A signal may be added only if the triple+ tier's win/loss ratio does not fall.**
>
> Coverage is monotone and cannot warn anyone. Every one of the four prior expansions raised
> net, lowered out-of-sample PF, and gave no signal until afterwards. **Break-even WR moves
> live as signals are admitted.** Carry win/loss ratio and margin alongside PF everywhere, and
> never quote either without the loss count — a tier resting on fewer than ~20 losses is noise
> wearing a number.

## WHAT YOU HAVE THAT BOOK-50'S SELECTION DID NOT

    1,818 VALID F0 signals        against a 2,420 pre-gate pool, now PRICED against a null
    a terrain map                 2,298 reachable episodes; BOOK-50 touches 82
    100% quality gap              every unclaimed episode WAS reached by the search
    four balance axes             direction x structure x session x regime, all populated
    a priced-gate method          recovered, with one gate reproduced out of sample
    the full family set           F1/F3/F9/F11/F2/F4 — measured, not assumed

## ORDER OF WORK

**F0 FIRST. Hit the 4x target with F0 signals and gates alone.** Only once that is achieved do
the other families get considered as additions — as companions, gates, or independent coverage
(Phase 4). **Do not mix the two questions.** Expanding F0 and evaluating F1/F3/F9 are separate
problems and conflating them is how the last expansion went wrong.

**AND ONE METHOD CAVEAT TO CARRY:** the gated figures above were produced by FILTERING an
existing trade table, not by re-simulating with the gate active. The position cap binds on 15.4%
of entries, so blocking shallow trades frees capacity and admits others. **The out-of-sample
split stands; the gated totals need re-running inside the simulation.**


## THE NUMBER TO BEAT — OPTION B

A Supervisor pre-pass over seven attempts produced this. **All 1 lot, in-sample, full conviction stack, jar
active, and every book below PASSES every hard constraint through `score_book.py`.**

    system                       trades      net     WR      PF    FailConc    mCVaR    survival
    BOOK-50 raw                   2,729   $76,458  90.8%     -      3.458   -$4,922     -$565
    FUSED-50 raw                  2,697   $84,691  91.7%     -      1.649   -$2,091     -$340
    FUSED-120 raw                 6,433  $158,418  89.8%   3.38     4.071        -      -$658
    FUSED-120 + Option A          3,343  $120,373  93.6%   7.36     2.067        -      -$139
    FUSED-120 + Option B          1,988  $100,094  96.6%  14.14     1.352        -      -$273

**OPTION B, ADOPTED CONFIGURATION (with `ATR_1M >= 20`): $97,410 | WR 97.57% | PF 15.72 | 44 losses |
3 losing days | worst day -$272.9.** Without the ATR gate: $100,094 | WR 96.58% | PF 14.14 | 68 losses |
FailConc 1.352 — the lowest measured anywhere in this project.

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



### THE ATR GATE — ADOPTED, AND A CORRECTION TO THE SHORT GATES

**`ATR_1M >= 20` at the entry bar, raw value not a percentile, applied to every cell.**

    gate            trades    WR%      PF       net     worst day  losses  ldays
    none (base)      1,988   96.58   14.14   $100,094    -$272.9      68      3
    ATR >= 15        1,902   97.06   14.74    $99,802    -$272.9      56      3
    ATR >= 20        1,808   97.57   15.72    $97,410    -$272.9      44      3   ADOPTED
    ATR >= 25        1,628   97.91   17.42    $93,511    -$287.7      34      3

**35% of all losses removed for 2.7% of the net, and the worst day does not move.** The
three losing days survive every setting.

**THE MECHANISM.** At low ATR the break-even lock sits ~12 points out and the 3.0 spread
eats **24% of it** — a BE exit banks $10.92 against $34.11 in normal conditions. The low
band still nets +$7,097, so it does not bleed; it earns $18.15 a trade against $58.23 and
carries a disproportionate share of the stop-outs.

**WHAT FAILED.** A minimum BE trigger makes it worse (floor 25 → losses 68 → 108) because
widening the trigger means fewer trades reach break-even and more ride to the stop. A
minimum STOP does nothing — minimum ATR in the frame is 8.5, so floors under 15 never
bind, and 20+ hurts because flooring ATR widens the stop and the step together.
**Do not move the trigger. Do not take the trade.**

**AND THE WHOLE TM CONSTANT SET WAS SWEPT** — RISK_MULT, MOMENTUM_SL_MULT, STEP_PCT,
LAG_BASE, LAG_MOMENTUM, MOMENTUM_THRESHOLD, MAX_RISK, BE_TRIG_FRAC, LOCK_FRAC. **Nothing
beat the ratified values.** One trap found: **LOCK_FRAC above 1.0 books exits at prices
the market never traded** — 52.7% of exits at LOCK_FRAC 2.0, producing a fictional
$258,236. **1.0 is a ceiling by construction, not a tuning choice.**

**CORRECTION TO THE GATE SPEC — THE SHORT CELLS ARE STACKED.**

    SHORT solo   Bar_Range < p95         AND  Micro_FailedBreak < p10
    SHORT dual   Efficiency_Ratio < p80  AND  Micro_VPIN > p70

    short gates                       trades    PF      net      worst day  losses
    Bar_Range/Efficiency alone         3,343   7.36   $120,373    -$139.1     213
    FailedBreak/VPIN alone             2,323   8.53   $103,464    -$495.6     115
    BOTH IN SERIES = OPTION B          1,988  14.14   $100,094    -$272.9      68

**Either alone gives roughly half the profit factor. The stack is the system**, and an
earlier draft listed only four short conditions where there are eight.

**All trade-management work ran on a research copy. The sacred
`portfolio_simulation_engine.py` is byte-identical at `bb498eb13ce3`.**



### THE CONCURRENCE LADDER — AND THE JAR IS TURNING AWAY $27,851

**Adopted configuration (jar cap 6, ATR>=20):**

    depth         trades       WR       PF      net $   avg $   wins / losses
    solo      177 ( 9.8%)   92.7%     3.16     6,552    37.0      164 / 13
    2         412 (22.8%)   96.1%     8.54    14,216    34.5      396 / 16
    3         480 (26.5%)   96.9%    14.31    22,628    47.1      465 / 15
    4         324 (17.9%)  100.0%      inf    18,458    57.0      324 / 0
    5         145 ( 8.0%)  100.0%      inf     9,446    65.1      145 / 0
    6         270 (14.9%)  100.0%      inf    26,109    96.7      270 / 0
    TOTAL           1,808   97.6%    15.72    97,410    53.9    1,764 / 44

**ALL 44 LOSSES SIT AT DEPTH 1, 2 AND 3. Depth 4+ is 739 trades with ZERO losses.** The
average trade climbs $37 → $34 → $47 → $57 → $65 → **$97**. Depth 6 alone earns $26,109,
more than any other tier, from 15% of the trades.

**Uncapped — true demand:**

    capped     1,808 trades   $97,410   PF 15.72
    uncapped   2,237 trades  $125,261   PF 17.51

    depth 8   40 trades  avg $273.8      depth 14  28 trades  avg $354.2
    depth 15  30 trades  avg $374.3      depth 19  19 trades  avg  $52.5

**The 6-lot jar turns away 429 trades worth $27,851, and they are BETTER than average** —
PF rises to 17.51 uncapped. **The deepest convergences are the ones it cannot hold.**

**THIS REFRAMES THE BUILD ITEM.** Not only "which six entries get the slots on a deep bar"
but **whether six is the right number at all.** The cap was never derived — NOT RECORDED.
Raising it is a survival question to be answered on the tail, not on the net.

**ONE ANOMALY:** uncapped depth 6 shows 18 losses and PF 2.76 between two perfect tiers —
the only non-monotone cell in the ladder. It may be a single bad day. Establish which.

### OPTION B MONTH BY MONTH

    month          n     W     L      WR%       PF        NET   worst day   losing days
    2026.01       87    87     0   100.00   999.00      3,724       +446         0
    2026.02      357   349     8    97.76    15.58     18,396        +34         0
    2026.03      422   410    12    97.16    12.92     22,179        +32         0
    2026.04      322   316     6    98.14    17.95     13,420        +69         0
    2026.05      219   212     7    96.80    12.83     12,560       -236         2
    2026.06      248   241     7    97.18    15.05     13,977       -273         1
    2026.07      153   149     4    97.39    21.34     13,154        +65         0
    TOTAL      1,808 1,764    44    97.57    15.72     97,410       -273         3

**January has zero losing trades. Four of seven months have no losing day at all.** Every
month clears PF 12.8; July is strongest at 21.34.

**The three losing days total $605.40.** Peak-to-trough equity drawdown is **$272.9** — the
maximum drawdown IS one day. The curve never has two bad days in a row.

    avg win $58.97 | avg loss -$150.41 | w/l 0.392
    break-even WR 71.83% vs actual 97.57% -> MARGIN 25.7pp

### THE NON-F0 EXPERIMENT — A SECOND BOOK, NOT A GAP FILLER

A book built from **everything except F0** (F1, F3, F9, F11, F2, F4), 2,184-signal field,
decorrelated per direction:

    50L/50S ungated   6,867 tr  PF 2.73  $113,933  wd -$638.4   F1 83 | F9 16 | F3 1
    best gated        2,764 tr  PF 7.33   $90,025  wd -$157.8

**F1 carries the entire non-F0 book — 83 of 100 signals.** The family that contributes
three slots to F0's same-bar depth is the one that stands up alone.

**Weaker than F0** (PF 4-7 vs 15.72, 446 losses vs 44) **but with a better worst day at the
tighter settings.**

**AND THERE IS ROOM.** Option B holds nothing on **95.7% of bars**; the jar is full on
**0.44%**. Non-F0 quad+ fires into an empty book 71.8% of the time and into a full jar only
7.3%.

**Joint run, one shared jar:**

    F0 Option B alone (in joint)   1,733   PF 14.37   $92,894   wd -$435.9
    F0 + NF0 5+                    1,919   PF 14.69   $99,164   wd -$272.9
    F0 + NF0 quad+                 2,275   PF 12.91  $109,235   wd -$348.4
    F0 + NF0 triple+               3,115   PF  9.06  $125,835   wd -$681.4

**`F0 + NF0 5+` beats F0 alone in the same run on every axis — including a worst day $163
better.** Adding 186 trades improved the tail. They win on F0's bad days.

**But note the cost:** F0 alone *in the joint run* is worse than F0 standalone, because
non-F0 takes jar slots and changes F0's admission. Against the true standalone baseline,
`+NF0 5+` is roughly neutral and `+NF0 quad+` is +$11,825 for −2.81 PF and a 28% worse tail.


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

---

---

### FOUR CORRECTIONS FROM THE QUANT'S PHASES 1-3 — READ THESE BEFORE ANY OF THE FIGURES ABOVE

**1. THE GATED ARM IS INERT. 39,260 OF 39,260 ROWS.** Every `gated_*` column equals its ungated
counterpart on every row of every family; `gated_delta_net` is exactly 0 everywhere. **No gate was
ever applied** — the gated arm is a duplicate under different column names, and no gate spec
appears in any catalogue header.

    family   VALID    gated_trades==trades   gated_delta_net==0
    F0       1,818          1,818                  1,818
    F1      37,258         37,258                 37,258
    F9/F3/F11/F2/F4    185            185                    185
    TOTAL   39,260         39,260                 39,260

**Any instruction to "report the gate's effect per family from the gated arm columns" is void.**
There is no gate effect in those columns. This matters more than it looks: the gates carry MOST of
the performance (applying Option B's spec to a 50-signal book multiplies PF by 2.77x, while adding
70 more signals buys far less), so the most important lever in the system has no per-signal
measurement at all.

**And there is a spec problem before there is a build problem:** the gate is INDEXED BY DEPTH TIER
and a signal scored ALONE has no depth. A per-signal gated column is only well defined for the
SOLO tier. Nine of the ten cells are undefined per-signal.

**2. `EXPECTED_ROWS_AT_OR_ABOVE_THIS_PF` CANNOT RANK — IT IS A FLOOR AT ZERO.**

    F0 rows with E < 1                      286
    of those, E == 0.0 EXACTLY              286
    pf_null_exceedance_pct == 0 exactly     286
    smallest NON-ZERO E in the family       8.9320    <- nothing between 0 and 8.93
    agg_pf span within the E==0 set         5.53 to 48.24

Every signal that beat the null's MAXIMUM PF prices identically at zero. At n_null median 470 the
resolution limit is 1/470 = 0.0021, so a PF-48 signal and a PF-5.5 signal are indistinguishable.
**"Rank by EXPECTED_ROWS ascending" IDENTIFIES the 286 and then goes flat. It selects; it cannot
order.** Ranking within the shortlist needs a larger null (K≈5,000) or a second key.

**3. THE SHORT-SIDE ASYMMETRY IS F0-SPECIFIC, NOT PROJECT-WIDE.**

    family  dir    n        agg_pf med   min_fold_pf med
    F0      LONG   1,445      3.552          0.785
    F0      SHORT    373      2.888          0.517      <- the asymmetry
    F1      LONG  18,505      2.349          0.486
    F1      SHORT 18,753      2.292          0.489      <- SYMMETRIC, and not sample noise
    F9      SHORT     74      2.187          0.127      <- INVERTED

**So thresholds must be per-direction PER FAMILY, not per-direction globally.** The blanket claim
that one absolute threshold imposes the long side's standard on the short side is true for F0 and
false for F1.

**4. COVERAGE FIGURES QUOTED FROM `touched_episode_ids` ARE UNGATED SIGNAL-SET REACH, NOT REALISED
BOOK REACH.** That column is a per-signal property counted from ALL of a signal's fires. Gating
removes ~71% of Option B's trades, so the coverage of trades actually taken is far lower:

    N     covUP ungated -> gated     covDOWN ungated -> gated
    50       2.89% -> 0.79%             4.85% -> 0.69%
    100      5.34% -> 1.14%             6.32% -> 1.30%
    200      8.49% -> 1.84%             8.57% -> 1.99%

**A gated 200-signal book reaches less terrain than an UNGATED 20-signal one.** Option B's
4.11% / 4.16% is its signal set's reach; its realised reach is nearer 1.1% / 1.3%. **Gating buys
quality by destroying coverage, and that trade was invisible until it was measured on executed
trades.**

### AND ONE PHASE 1 CONCLUSION IS REVERSED

An earlier Supervisor frontier showed OOS PF FLAT at 3.34-4.00 across a ten-fold range in book
size and read that as robustness. **It was a floor.** Gating lifts OOS PF by 1.5x to 4x, and the
GATED curve DOES decline — PF 12.23 -> 5.33 across the same range, gated OOS PF peaking at N=100
and falling to 5.77 by N=200. **The ancestral prior that four prior expansions all curved down is
NOT refuted. It was masked by measuring ungated books.**

Related: the payoff-parity break is NOT at N=50. With N=55/60/65 filled in it is a flat plateau at
w/l 0.70-0.75 from 50 to 70; parity is lost BEFORE 50 and the real cliff is 70 -> 85 -> 100. A
two-point grid manufactured a cliff that is not there.

**METHOD CAVEAT ON EVERY FRONTIER FIGURE:** the 1,818-signal field was scored in BATCHES OF 120,
and the 6-lot jar admits by contention — so each signal's recovered daily P&L depends on which
signals shared its scoring call. **The field is not a property of the signals alone.** Two
independent builds differ 4-10% on net. Treat the SHAPES as sound and the LEVELS as indicative,
and do not quote frontier nets to the dollar.

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

**THE STRONGEST PRIOR IN THE PROJECT, AND IT BEARS DIRECTLY ON THIS PHASE.** A dilution curve
has been built twice before. **Neither landed on an N above its starting point.**

    taskB expansion   n=78 -> 124        VERDICT: DO NOT EXPAND PAST 78
    BOOK-48 + F1      x5/10/15/20        OOS PF  6.65 -> 6.07 -> 5.87 -> 4.55 -> 4.03
                                         worst-day -127 -> -167 -> -320 -> -457 -> -405
    rolling walk-forward on 78           expanded sets collapse to OOS PF ~3 failing the 4.0
                                         floor; the conservative baseline holds 7.3-10.9 in
                                         EVERY fold

**Every increment traded OOS PF and worst-day for net.** The new same-bar-depth curve is the
first of its kind but not the first of its family. **Report whether it curves down like its
ancestors, and if it does not, say why this one differs.**

**AND THE STRUCTURAL WARNING:** the new headline is a COVERAGE number, and **coverage cannot
fall as signals are added, so it cannot warn you.** Every prior expansion had a falling OOS PF
to signal the stop. Coverage is monotone by construction. A book can be expanded until it is
worthless while the coverage number improves the entire way down. **Any frontier you report
must carry OOS PF and worst-day on the same axis as coverage, or it is not a frontier.**

**A TOOL YOU CAN RUN YOURSELF, AND SHOULD.** Every depth figure in circulation — mine included
— was INFERRED by grouping trades on `entry_bar`. The engine has its own measurement and it has
never been run:

    python scanners/triple_convergence_and_d2ddir.py density <book.csv>

`DENSITY_K_BANDS = [1,2,3,4,5,6,8,10]`, co-firing count >= k over the candidate signal set,
direction-aligned, applied as a GATE rather than a post-hoc grouping. See `cake_dictionary.md`
section 4G. **Always pass the book explicitly — its default argument is a superseded file.**

**Use it as a checkpoint on any candidate composition.** It answers "does count>=5 outperform
count>=2" directly and it is the correct instrument for that question, where my groupby figures
are an approximation.

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

# THE ARCHITECTURAL RULING THAT GOVERNS PHASES 5, 7 AND 8

**F0 IS THE TRADE SIGNAL. EVERY OTHER FAMILY IS TERRAIN CONTEXT UNTIL PROVEN OTHERWISE.**

## D2D IS THE PROOF OF CONCEPT, AND IT IS ALREADY IN PRODUCTION

**D2D was built as a TRADE MECHANISM — a break-of-structure entry — and it fails as one.** Backtested
on the corrected frame it is roughly 77% WR at PF ~1.7. On the evidence bar this project applies, that
is not a signal.

**AS A DESCRIPTOR IT IS THE MOST LOAD-BEARING ELEMENT IN THE SYSTEM.** `D2D_Trend_Dir` is the mandatory
fourth directional term on every entry in every family. It answers one question — *which trend
direction is the market currently within* — and it answers it well enough that nothing trades without
it.

**THE SAME OBJECT FAILED ONE TEST AND PASSED ANOTHER, AND THE DIFFERENCE WAS THE QUESTION ASKED.**
That is the precedent, it is in production, and it has been for the life of this project. **Every
non-F0 family has been evaluated only against the test D2D FAILS.**

## AND SIX FAMILIES WERE DELETED FOR FAILING THAT SAME TEST

S5's filter is `agg_pf >= 2.0`. It cut every row of six families:

    family                                rows   med PF   med WR   med trades
    F5  persistence / autocorrelation       16     1.08    78.0%       494
    F6  threshold crossing                  10     1.11    79.3%       283
    F7  mean reversion                      28     1.26    79.2%       579
    F8  cross-variable structure            10     1.08    78.1%       810
    F4  divergence / NOT-CONFIRMED-BY       66     1.21    79.2%       166
    F2  state transition                    47     1.13    78.6%       139

**177 signals. Read what they describe rather than what they score:**

    F5   Micro_AutoCorr:hi                                   -> trending, or chopping
    F7   FADE VWAP_Z:hi                                      -> stretched from value
    F8   Slope_EMA_ST > Slope_EMA_LT                         -> short-term outrunning long-term
    F4   VWAP_Z:hi NOT-CONFIRMED-BY Micro_OrderFlowDelta:lo  -> PRICE EXTENDED, FLOW ABSENT

**READ THAT LAST ONE ALOUD. "Price is stretched high and order flow is not confirming it" IS BULLISH
EXHAUSTION**, and it was deleted because its median profit factor is 1.21.

**PF ~1.1 AT 78-79% WR ON 500-800 TRADES IS EXACTLY WHAT A NEUTRAL DESCRIPTOR LOOKS LIKE.** It fires
often, it is directionally agnostic, and it survives folds. Those are the properties you want in a
state label and precisely the properties that fail an entry filter. **A structure descriptor is not
supposed to predict direction. It is supposed to describe a state.** Asking "is the market trending"
to be profitable on its own is asking a thermometer to make money.

## WHY F0 IS THE EXCEPTION AND STAYS THE TRIGGER

F0 is not a better descriptor. **It is a different kind of object: SAME-BAR CONCURRENCE DEPTH MEASURES
THE MAGNITUDE OF A PRICE-ACTION EVENT.** Six independent patterns agreeing on one bar is not a
description of a state — it is an event large enough to move several unrelated measurements at once. A
massive directional NY open registers as depth; a quiet drift does not.

**The measured ladder is that claim's evidence:** all 44 of Option B's losses sit at depth 1-3, depth 4
and above is 739 trades with ZERO losses, and the average trade climbs $37 -> $34 -> $47 -> $57 -> $65
-> $97. **Depth is not a confidence score. It is a size measurement.** And a random-triple null shows no
gradient at all, so the size is real rather than an artifact of clustering.

## THE ARCHITECTURE THIS IMPLIES, AND IT IS WHAT PHASES 5, 7 AND 8 SHOULD TEST

    STATE      every family except F0, composed into named market structures
               "bullish exhaustion" = F4 divergence + F5 low autocorrelation + F8 slope rollover
               nameable, checkable, and diagnosable when it stops working

    EVENT      F0 concurrence depth, which measures how large the price-action event is

    TRIGGER    an F0 cluster, admitted because the STATE says clusters of this shape are valid here

**THE PAYOFF IS NOT PROFIT, IT IS DIAGNOSIS.** BOOK-50 went 6.40 -> 2.19 and nobody could say why. With
named states the question becomes answerable and checkable independently of P&L: *is the market still
producing exhaustion the way it did in January?* **A book you cannot diagnose cannot be held through a
bad month, because you have no way to tell a bad month from a broken assumption.**

**SO EVALUATE EVERY FAMILY TWICE.** Once as it has always been evaluated, and once as a descriptor —
and note that the second evaluation has NO PF BAR, because a descriptor with an edge would be a signal
and a descriptor without one is doing its job. **The scan has already run. The 177 cut rows exist in
the results files. Nobody has ever looked at them this way.**



This is the operator's ruling and it reverses how every prior pass approached the non-F0 families.
They were evaluated as CANDIDATE ENTRY TRIGGERS — scored on their own trades, ranked on their own PF,
and asked to earn a place in the book. On that basis F1 looks like 37,258 signals nobody can
navigate, F3 looks like a family that fires on 92% of bars, and F9 looks like 127 rows too few to
matter.

**ON THE TERRAIN BASIS THEY ARE SOMETHING ELSE ENTIRELY: A DESCRIPTION OF WHERE PRICE IS AND WHAT IT
HAS JUST DONE, INSIDE WHICH AN F0 TRIPLE EITHER FIRES OR DOES NOT.**

## WHY THE RULING EXISTS — THREE MEASURED FACTS

1. **The other families FAIL as entries and FAIL as gap fillers.** Non-F0 standalone reaches PF 4-7
   against F0's 15.72. On flat bars F3 loses $38,753, F9 -$1,143, F11 -$674, F2 -$366. Both roles
   are closed by measurement, not by opinion.
2. **THE GATES CARRY MOST OF THE PERFORMANCE.** Applying Option B's gate spec to a 50-signal F0 book
   multiplies PF by 2.77x; adding 70 more signals buys far less. Whatever improves the GATE LAYER is
   worth more than whatever adds to the signal layer.
3. **Every gate tested so far is A SINGLE VARIABLE AT A THRESHOLD** — Hurst > p90, FailedBreak < p10,
   VPIN > p70, ATR >= 20. A family is a PATTERN: a sequence, a divergence, a session structure, a
   state transition. **Nobody has ever gated an F0 triple on a pattern.**

## THE WORKED EXAMPLE — F1 AS A TERRAIN LAYER, ALREADY MEASURED

F1's grammar is `A ->k-> B`: condition A fires, condition B follows within k bars. Read as an entry
that is a signal. **READ AS TERRAIN IT IS A WINDOW WITH A DIRECTION AND AN EXPIRY**, which is
something no existing element of this system provides — D2D_Trend_Dir is a continuous state that
says only "which way is the trail pointing right now" and never expires.

**Coverage measured on the corrected frame, fold-persistent pairs only:**

    4 pairs at WR>=98%      43.7% of bars
    238 pairs at WR>=96%   100.0% of bars      <- SATURATES
    400 pairs              100.0% of bars      <- SATURATES

**A UNION OF PAIRS IS ON ALL THE TIME AND GATES NOTHING.** This is the same trap that killed F10's
density family. **A terrain layer must be a HANDFUL of pairs, not a family.**

**Single pairs are usable.** Across 300 fold-persistent pairs at WR>=93%: coverage p5 4.4%, p25
10.5%, median 18.1%, and 21 pairs cover under 5% of bars. The tightest:

    A ->k-> B                                            dir     WR     PF   k    cov
    ST_Flip_Event:==-1  ->7->  Micro_AutoCorr:hi         SHORT  97.9  18.22   7   4.07%
    Session_High_Dist_ATR:lo ->10-> Slope_EMA_ST:lo      SHORT  97.8  24.73  10   3.46%
    Slope_EMA_LT:hi ->8-> OR_High_Side:==-1              SHORT  97.4   8.04   8   3.36%
    Micro_BarOverlap:hi ->4-> D2D_ATR:hi                 LONG   97.6  13.09   4   6.94%
    EMA_Oscillator:hi ->2-> Harmonic_D2D_Concordance:==0 LONG   97.4  17.94   2   5.32%

**READ WHAT THOSE ARE.** `Session_High_Dist_ATR:lo -> Slope_EMA_ST:lo` is *price at the session high,
then short-term slope turns down* — a rejection at an extreme, over ten bars, marking 3.46% of the
frame. `ST_Flip_Event:==-1 -> Micro_AutoCorr:hi` is *a flip happened, then autocorrelation rose* —
the flip established direction and the market TRENDED rather than chopped.

**Neither is expressible in a single-variable threshold and neither is expressible by D2D.** And both
match the leader-then-confirmer shape Phase 4 found independently: structural location or an event
leads (mean_join_offset ~0.00-0.19), trend-strength and slope conditions confirm (offset 3-22).

**CAVEAT THAT MUST TRAVEL:** those WR and PF figures are F1 used as an ENTRY with D2D already applied
as the mandatory fourth term, so they are not independent of D2D. **The COVERAGE figures are a
property of the window itself and are what decide whether it can gate at all.** Using a window as a
gate on F0 triples is a different measurement and it has not been made.

## WHAT THIS ASKS OF YOU, PER FAMILY

For F1, F2, F3, F4, F9, F11 — and F5-F8 if their rows justify it — answer FOUR questions each,
PER DIRECTION:

  **1. WHAT DOES THIS FAMILY DESCRIBE?** In one sentence, in market terms, not grammar terms. F1 is a
     sequence with an expiry. F3 is a conditional interaction. F4 is a divergence — a
     NOT-CONFIRMED-BY. F9 is session-temporal structure. F2 is a state transition. Name what each
     one KNOWS that a single variable does not.

  **2. IS IT RARE ENOUGH TO GATE?** Report the share of bars the family's state is ACTIVE, per member
     and for the union. **RUN THE SATURATION TEST FIRST AND STOP THERE IF IT FAILS** — a state that
     is true on most bars cannot gate, however good its standalone numbers look. F3 fires on 92.1% of
     bars and is dead on arrival as a union; F9 fires on 4.1% and is the strongest candidate.

  **3. DOES IT SEPARATE F0's OUTCOMES?** The measurement that matters: take Option B's F0 trades, mark
     which fired while the family's state was active, and compare. WR, PF, worst day, payoff ratio,
     WITH LOSS COUNTS, per direction, per depth tier. **A terrain layer earns its place by separating
     trades that already exist, not by adding new ones.**

  **4. AND WHERE DOES IT SIT IN THE STACK?** The gate stack under test is:

         solo convergence + ATR_1M>=20 + ADX + ticks + [FAMILY STATE] + D2D dir

     State whether the family term is ADDITIVE to D2D (both required), ALTERNATIVE to it (either
     satisfies), or a CONVICTION term (sizes rather than blocks). These are different propositions
     with different failure modes and the record shows the same variable can succeed in one role and
     fail in another — the exhaustion sequence cleared decisively as a NEGATIVE screen at the 1.4th
     percentile and failed as a positive conditioner at the 71.8th.

## FOUR RULES FOR THIS WORK

- **A UNION OF MEMBERS SATURATES. A HANDFUL DOES NOT.** Choose members for RARITY, then check quality.
  Never pool a family and call it a state.
- **THE STANDALONE NUMBERS ARE NOT THE TERRAIN NUMBERS.** A family's catalogue WR is its performance
  as an entry with D2D applied. Its value as terrain is a different measurement on F0's trades.
- **CAUSALITY IS NOT OPTIONAL HERE.** A state derived from full-sample labels and used to gate an
  entry is look-ahead. concurrence_entry_order.csv now carries a causal arm; terrain_episodes.csv is
  FULL-SAMPLE and carries an eligibility_mask and population column — state which you used.
- **REPORT WHAT IT DESCRIBES, NOT ONLY WHAT IT SCORES.** The operator's question is "why was this
  trade opened?" Today the answer is "three conditions agreed and D2D pointed up." A terrain layer
  should make it "three conditions agreed, D2D pointed up, AND we were inside a window where price
  had just rejected the session high." **That sentence is the deliverable.**

**AND THE DOOR STAYS OPEN BOTH WAYS.** If a family measures better as an entry than as terrain, say
so — the ruling is a reframing to be tested, not a conclusion to be confirmed. F1 carried 83 of 100
signals in the best non-F0 book, which is not the profile of a pure context layer.

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

**A PRECEDENT THAT WOULD NOW BE A DOCTRINE BREACH.** BOOK-50's 8/8 market-structure coverage
was a **REQUIREMENT, not an observation** — two F1 sequentials were added specifically to fill
SQUEEZE_BREAKOUT and TREND_EXHAUSTION, and the task is recorded as "Filling both missing
structures." **Under doctrine rule 3 that is a quota and would not be permitted.** Report
structure coverage as an OUTPUT. Do not propose filling a gap.

Note also the original assignment used a rule-based `classify(s)` against a PRIORITY list, and
**the rule's contents are NOT RECORDED** — so the new classifier's 20 UNMAPPED rows cannot be
checked against the original. State that limitation rather than assuming agreement.

**THE TENSION TO QUANTIFY RATHER THAN RESOLVE:** NY OPEN holds 225 of the largest-decile episodes
and overnight holds 0. A session-balanced book deliberately takes worse episodes to be even.
**Measure that cost.** The operator decides whether to pay it.

**AND THE GATING WORK, WHICH IS PART OF THIS PHASE:**

Findings B and C establish that gating is **directional**. Longs enter on pullbacks and respond
to Hurst; shorts enter on continuation and respond to FailedBreak. Neither gate has been priced.

**A PRICED-GATE METHOD ALREADY EXISTS — USE IT AS THE BASELINE, AND IMPROVE ON IT IF YOU CAN.** See
`cake_dictionary.md` section 4D. Pool: raw D2D flips both directions at ADX>=15, n=302, ~17%
losers. 360 tests (90 FEAT_ x hi/lo x direction). For a candidate slice of size k, draw many
random k-subsets from the same pool and read where the candidate lands. Five gates, all
required: random-pct >=97.5, OOS-positive, fold-persistent, n>=8, stated mechanism.

**If you can specify a better pricing method, do so and say why.** The requirement is that a
gate found by searching variables and thresholds MUST be priced against the search that found
it, by some defensible method. Recovering this one saves inventing from nothing; it does not
bind you to it.

**AND THE CAVEAT MUST TRAVEL WITH ANY RESULT YOU PRODUCE:** *"360 tests. At the p97.5 gate,
expected chance survivors ~9.0. Bonferroni for FWER 5% = 99.986th pct; max candidate reached
99.9th -> NONE clear strict Bonferroni."* The prior research stated this about its own
findings. Hold yourself to the same standard.

`Micro_FailedBreak > p50` has no such standing: 66 tests (eleven variables x three thresholds x
two sides), one frame, never priced. **How large a lift would the best of 66 tested gates show
by chance alone?** Specify the null; do not build it. Then state which of the two gates the
evidence actually supports.

Also specify how to test the **tiered** gate that is currently untested: Hurst p90 on short
solos/duals, FailedBreak on short triples.

**OUTPUT:** the achievable coverage per (direction × structure × session × regime) cell, the
measured cost of balance against concentration, and the specification for a priced gate test.

---

# PHASE 8 — THE ADAPTIVE CONVERGENCE ENGINE

**THIS IS AN ARCHITECTURAL ALTERNATIVE, NOT A REFINEMENT. It is the operator's own design and it
may replace the frozen-triple architecture entirely. Treat it as the most consequential open
question in the project.**

## 8.0 — TWO LEVELS OF LICENSING, AND THE SECOND IS THE OPERATOR'S

**PHASE 8 AS ORIGINALLY WRITTEN LICENSES THE CONDITION. THE OPERATOR'S FORMULATION LICENSES THE BAR.**
Read 8.1-8.6 with that distinction in mind, because it changes what the saturation test is testing.

    CONDITION-LEVEL (8.1 below)   license each of the 249 conditions per direction against terrain
                                  -> count LICENSED conditions per bar -> k+ agreement is the signal
                                  the filter acts on the VOCABULARY

    BAR-LEVEL (the operator's)    use the non-F0 families as TERRAIN DESCRIPTORS to establish which
                                  BARS are in a favourable state -> then ANY convergence cluster
                                  forming on such a bar is admissible
                                  the filter acts on the BARS

**WHY THE SECOND MAY SUCCEED WHERE THE FIRST FAILED.** F10's density family was fused into F0 and
never run because raw density saturates — ~24 of 120 thresholds sit at extremes on any bar by
construction, and at 1,000 signals 97.1% of bars reach depth 3. Directional licensing of the
VOCABULARY was proposed as the filter. **The F1 coverage measurement now suggests that filter may not
bite hard enough: a union of 238 fold-persistent pairs at WR>=96% covers 100% of bars. Pooling
anything in this system saturates.**

**BAR-LEVEL LICENSING FILTERS A DIFFERENT THING.** It does not ask "which conditions are allowed to
speak" — it asks "is this bar in a state where agreement means something". A handful of rare terrain
descriptors, each active on 3-8% of bars, intersected, produces a small set of bars. **On those bars
the vocabulary needs no filtering at all, because the scarcity has already been supplied by the
terrain rather than by the licence.**

**THE OPERATOR'S FRAMING, IN HIS WORDS, AND IT IS THE DESIGN SPEC:** every bite of the cake is smelled
in advance of consumption, so all valid convergence clusters are valid — all well-cooked cake is
edible. The terrain layers are the nose: a whole-state judgement assembled from many weak signals at
once, no one of them decisive. **A single-variable threshold is a thermometer. It reads one number
precisely and says nothing about whether the cake is cooked.**

## WHAT THIS ASKS YOU TO MEASURE, IN THIS ORDER

  **1. BUILD THE TERRAIN STATE FROM PHASE 5's OUTPUT.** Phase 5 asks, per non-F0 family, what it
     describes and whether it is rare enough to gate. Take the members that pass — rare, causal,
     fold-persistent — and form a per-bar state array per direction. NOT a union of a family. A
     handful of members across SEVERAL families, chosen for rarity.

  **2. INTERSECT AND REPORT THE SCARCITY CURVE.** For 1, 2, 3, 4 descriptors intersected: what share
     of bars survive, per direction? **THIS IS THE SATURATION TEST IN ITS BAR-LEVEL FORM AND IT IS
     STILL THE FIRST THING TO RUN.** If four rare descriptors intersected still leave 60% of bars,
     the terrain is not scarce and the architecture fails for the old reason at a new level. Say so
     and stop.

  **3. THEN ASK WHETHER THE VOCABULARY NEEDS FILTERING AT ALL.** On the surviving bars only, report
     the raw depth distribution — how many of the 249 conditions agree, per direction, unfiltered.
     **If depth on well-cooked bars is naturally rare, the licence is unnecessary and the
     architecture simplifies to: terrain admits the bar, any k+ cluster is the signal.** If depth is
     still saturated there, condition-level licensing (8.1-8.6) is the fallback and both filters are
     needed.

  **4. SCORE IT AGAINST OPTION B's LADDER, LIKE FOR LIKE.** Same frame, same engine, same conviction,
     same jar, 1 lot. Option B: all 44 losses at depth 1-3, 739 trades at depth 4+ with ZERO losses,
     average trade climbing $37 -> $97. **IF TERRAIN-ADMITTED CLUSTERS REPRODUCE THAT SHAPE, THE
     GRADIENT IS A PROPERTY OF AGREEMENT ON GOOD TERRAIN RATHER THAN OF FROZEN SIGNAL SELECTION.**

  **5. AND REPORT COVERAGE, BECAUSE THAT IS WHY THIS MATTERS.** Option B's realised gated coverage is
     ~1.1% UP / 1.3% DOWN against a catalogue ceiling of 9.01% / 7.71%. **A terrain-admitted engine
     is not bound by which combinations happened to be searched.** If it reaches materially more
     terrain at comparable quality, that is the argument for it — and coverage is the axis where the
     frozen architecture is weakest.

**AND THE STANDARD THIS IS JUDGED BY IS NOT PROFIT.** The operator's stated objective is confidence
that the signals will keep performing, not more money. A terrain layer earns its place by making
FAILURES LEGIBLE: if a trade loses inside a state that was supposed to be favourable, that is
information, and the descriptor can be checked independently of P&L. **BOOK-50 went 6.40 -> 2.19 and
nobody could say why. That is the wound this addresses.** Report what each layer DESCRIBES, not only
what it scores.

**THE LOOK-AHEAD TRAP APPLIES DOUBLY HERE** and it is now the single most likely way this produces a
spectacular and worthless result. terrain_episodes.csv is FULL-SAMPLE and carries an eligibility_mask
and a population column. Four concurrence files carried causal=False on every row until this run;
concurrence_entry_order.csv now has a causal arm. **A bar-level licence derived from full-sample
labels and used to admit an entry is look-ahead. State the basis at every use or the result is
discarded.**

## 8.1 — THE PROPOSITION

Everything in this project so far freezes a combination. F0 searches `A + B + C`, ratifies the
triple as a SIGNAL, and then measures how many of those frozen signals co-fire on a bar. The
combination is the unit of discovery.

**The alternative inverts it. Do not discover signals. Discover which CONDITIONS are permitted
to speak in each direction, then let any stack of permitted conditions form its own cluster.**

    CURRENT      search C(239,3) -> freeze the triple -> count frozen signals per bar
    PROPOSED     license each CONDITION per direction -> count LICENSED conditions per bar
                 -> any bar where k+ licensed conditions agree IS the signal

No frozen combinations. No `signal_def`. The vocabulary itself becomes the book, and depth
emerges from whatever happens to align on a bar.

## 8.2 — WHY IT FAILED BEFORE, AND WHY THAT REASON MAY NO LONGER HOLD

This was attempted. F10 was built as a density-band family (`count >= k`, k = 2..5) and was
FUSED INTO F0 rather than run — see `cake_dictionary.md` section 4G. The recorded reason it
does not work naively:

> *"~24 features are always at extremes on any bar (120 thresholds x 20% = 24 expected). So raw
> density doesn't work."*

**Raw density over the whole vocabulary is a volatility proxy, not a signal.** Confirmed
independently: at 1,000 signals, 97.1% of bars reach depth >= 3. Density saturates.

**THE OPERATOR'S INSIGHT IS THAT THE PRIOR ATTEMPT HAD NO FILTER, AND NOW ONE EXISTS.** The
terrain map did not exist when F10 was designed. There are now 2,298 reachable episodes with
directional labels — 1,143 UP, 1,155 DOWN — so for the first time it is possible to ask of each
condition: **does this fire disproportionately in UP terrain, in DOWN terrain, or neither?**

Filter the vocabulary FIRST on directional licence, and density is measured over a smaller,
directionally-coherent set. **Whether that is small enough to stop saturating is the whole
question, and it is answerable.**

## 8.3 — WHAT TO MEASURE

**STEP 1 — LICENCE EVERY CONDITION AGAINST THE TERRAIN.** For each of the 249 conditions in the
pool, using `terrain_episodes.csv` and `regime_labels.csv`:

    fires_in_UP_episodes / total_fires        directional share
    fires_in_DOWN_episodes / total_fires
    fires_outside_any_episode / total_fires
    lift vs the base rate of UP/DOWN terrain

**Report the full distribution before applying any threshold.** If most conditions sit near
50/50, the licence carries no information and the phase ends there — say so.

**STEP 2 — PRICE THE LICENCE.** A condition firing 51% in UP terrain is not licensed, it is
noise. Use the project's own random-subset method (see `cake_dictionary.md` 4D): draw random
subsets of the same size from the same episode population and read where the candidate lands.
**A licence must clear a stated percentile, be OOS-positive, and have a stated mechanism** — the
same five gates the D2D conditioner sweep used.

**STEP 3 — THE SATURATION TEST, AND THIS IS THE ONE THAT DECIDES IT.** With the licensed set
only, report the base rate of k+ simultaneous agreement, per direction, for k = 2..10:

    licensed conditions per direction:  N_long, N_short
    bars where k+ licensed LONG conditions are simultaneously true, as a share of all bars
    same for SHORT

**If k=3 still occurs on a large share of bars, the filter has not solved the saturation problem
and the architecture fails again for the same reason.** Say so plainly and stop. If the licensed
set is small enough that k=3 is genuinely rare, proceed.

**STEP 4 — SCORE IT.** Treat every bar reaching k+ licensed agreement as an entry, gated by D2D
as usual, and run it through `portfolio_simulation_engine.py` with the full conviction stack and
the jar. Produce the same depth ladder every other book in this project has:

    k        trades    WR    PF    net    worst day    wins / losses    avg trade

**STEP 5 — COMPARE IT LIKE FOR LIKE.** Against Option B's ladder (`cake_dictionary.md` §5C):
same frame, same engine, same conviction, same jar, 1 lot. The comparison is not "does it make
money" — it is **does an adaptive engine reach the same payoff structure as a frozen-triple one,
and does it reach terrain the frozen book cannot?**

## 8.4 — WHAT WOULD MAKE THIS WORTH ADOPTING

**Not net.** Three things, in order:

1. **COVERAGE.** Option B touches 4.11% of reachable UP terrain and 4.16% of DOWN. The catalogue
   ceiling is 9.01% / 7.71%. **An adaptive engine is not bound by which combinations happened to
   be searched**, so if it reaches materially more terrain at comparable quality, that is the
   argument for it. Coverage is the axis where the frozen architecture is weakest.
2. **THE DEPTH LADDER SURVIVING.** Option B shows all 44 losses at depth 1-3 and 739 trades at
   depth 4+ with zero losses, average trade climbing $37 -> $97. **If licensed-condition depth
   reproduces that shape, the gradient is a property of AGREEMENT and not of frozen signal
   selection** — which would be a finding well beyond this project.
3. **NO SELECTION STEP.** A licensed vocabulary needs no book, no decorrelation, no argmax. It
   would remove the entire class of failure that produced BOOK-50's collapse — the selection
   process that was never validated.

## 8.5 — WHAT WOULD KILL IT, AND SAY SO EARLY IF IT DOES

- **Saturation survives the filter** (step 3). The most likely outcome and the fastest to test.
  Run step 3 BEFORE step 4 and stop there if it fails.
- **Licences do not price.** If no condition clears the random-subset bar, there is no licensed
  set to work with.
- **The ladder is flat.** If k+ agreement among licensed conditions shows no PF gradient, then
  the gradient really does belong to the frozen combination and the frozen architecture is
  vindicated.
- **The terrain labels leak.** `terrain_episodes.csv` is built on the FULL sample. A licence
  derived from full-sample episode labels and then used to gate entries is look-ahead. **State
  how you avoided that** — a causal licence must be derived on a training segment only.

**That last point is not optional. It is the single most likely way this produces a spectacular
and worthless result.**

## 8.6 — DELIVER

The licence distribution across all 249 conditions. The priced licensed set per direction. The
saturation table for k = 2..10. If and only if saturation is beaten: the scored depth ladder and
the terrain coverage, compared like-for-like against Option B.

**And a plain verdict: does the adaptive engine reach terrain the frozen book cannot, at
comparable quality — or does it saturate again?** Either answer is worth having. The first
changes the architecture; the second closes a question that has been open since F10 was designed
and never run.

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
