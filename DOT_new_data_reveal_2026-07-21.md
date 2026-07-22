# DOT — NEW-DATA REVEAL (2026-07-21)
## First true out-of-sample event: what the fresh US30 export exposed about signal selection

**Status:** diagnostic record. Not a verdict. Findings are computed, not asserted — every figure below
was produced by running the ratified engine (portfolio_simulation_engine, oracle `518862bf19fb`) on
the fresh export and splitting at the baseline boundary.

---

## 1. THE EVENT

A fresh EA export was taken 2026-07-21: **100,000 rows, 2026.04.08 17:32 → 2026.07.21 17:09**.
The sealed baseline ends **2026.06.25**. Therefore:

- **OVERLAP segment** (Apr 08 → Jun 25): 49 trading days — data the baseline already contained.
- **NEW segment** (Jun 25 → Jul 21): 18 trading days, ~25,302 bars — **genuinely unseen data the
  selection never touched.**

This is the first time the committed BOOK-50 has met data outside its selection window.
The prior 76-signal book was never tested this way on overlapping-then-extending data; the Jan19–Apr27
generation did not overlap cleanly. **This event is therefore the first honest reveal of what the
selection methodology actually produces.**

---

## 2. DATA INTEGRITY — CONFIRMED SOUND (checked before any performance reading)

| Check | Result |
|---|---|
| rebuild.py ingest | 100,000 rows x 172 cols, **invariants PASS**, 13 parts written |
| Overlap bars vs baseline | **75,732 shared timestamps** |
| Open / Low / Close diff | **0.000000** |
| High / Volume diff | max 2.0 / 10.0 on single worst bar (feed noise, not a shift) |
| KAMA_Value diff | **median 0.000000** |
| D2D_Trend diff | **median 0.000000** |
| Micro_Hurst diff | **median 0.000000** |
| ADX_Value diff | **median 0.000000** |
| PoC_Price diff | **median 0.000000** |

**Conclusion: export=live parity holds on fresh data. The adaptive layer and the KAMA `.bin`
warm-start seed are working correctly.** Any performance finding below is a property of the
signals/market, NOT a data artifact. The raw filename (`US30.cash_AUTO_EXPORT.csv`, dotted asset name)
required no adjustment.

---

## 3. HEADLINE — BOOK-50 ON THE NEW SEGMENT

| Segment | Trades | Net | PF | WR | Worst day | Days |
|---|---|---|---|---|---|---|
| Whole file (Apr08–Jul21) | 1,350 | +$39,768 | 4.01 | 89.0% | −$565 | 67 |
| Overlap (Apr08–Jun25) | 975 | +$31,360 | 6.08 | 92.1% | −$204 | 49 |
| **NEW (Jun25–Jul21)** | **375** | **+$8,407** | **2.19** | **81.1%** | **−$565** | **18** |

- **The system remained profitable on unseen data** and never approached the FTMO daily ceiling
  (worst day = 11% of the $5,000 limit).
- It did **not** invert (the prior 76-book failure mode). It softened.
- Trade rate was **unchanged**: ~20/day overlap vs ~21/day new. The system did **not** self-throttle
  in the weaker regime — it traded the same volume at lower win quality.
- Exit-type shift is the mechanism: **SL share rose 7.5% → 18.7%** while BE share fell 77% → 66%.
  Whipsaw reversals took entries to full stop before the BE tier could arm.

### Market context (Jun 25 – Jul 21)
Range-bound, geopolitically driven chop (US–Iran escalation, Red Sea shipping risk, Fed-hike
speculation). Daily moves alternated direction (−0.59%, +0.24%, −0.70%) inside a ~51,800–52,200 band.
This period also opens the seasonal **summer doldrums** (historically the flattest US-index stretch,
mid-July to late-August). A directional convergence system faces its worst conditions here.

---

## 4. WHAT PERSISTED (the core finding)

### 4.1 Signal-level persistence
**CORRECTED COUNT (this supersedes an earlier miscount of "51 signals").** The committed book is
**50 signals: 48 F0 triple-convergence + 2 F1 sequential.** The 3 GAP fillers (GAP_HURST, GAP_FB,
GAP_D2D) are separate gap-filler / conviction elements added after selection — they are NOT part of
the 50. Total trading entities = **53**; 52 fired in both segments.

Persistence (positive net AND PF>=2 AND WR>=75% in BOTH segments), by group:

| Group | Persisted | Rate |
|---|---|---|
| F0 triple-convergence | 23 / 47 | 49% |
| F1 sequential | 1 / 2 | 50% |
| GAP fillers | 2 / 3 | 67% |
| **ALL entities** | **26 / 52** | **50%** |

Their combined NEW net: **+$9,841** (i.e. the persisters carried the whole book's +$8,407).

**Null baseline test (is 51% skill or luck?):** 400 random triple-convergence signals were generated
from the same condition pool and scored on both segments.

| | Passed OLD quality bar | Of those, persisted to NEW |
|---|---|---|
| Random triples | 15 of 201 (7%) | **4 (27%)** |
| **BOOK-50** | 52 fired in both | **26 (50%)** |

**Selection persists at ~2x the random rate (50% vs 27%).** The selection process captures real signal — but 2x
is not sufficient rigor for funded deployment. This is the measured size of the gap.

### 4.2 Market-structure persistence — normalised per trading day

| Structure | Signals | Persisted | Rate | OLD $/day | NEW $/day | |
|---|---|---|---|---|---|---|
| **TREND_CONTINUATION** | 15 | 10 | 67% | $165.8 | **$265.1** | **+60% BETTER** |
| **BREAKOUT_EXPANSION** | 8 | 6 | **75%** | $100.9 | **$140.7** | **+39% BETTER** |
| TREND_EXHAUSTION | 1 | 0 | 0% | — | +$46 total | |
| SQUEEZE_BREAKOUT | 1 | 0 | 0% | — | +$26 total | |
| VOLUME_CONFIRMED | 1 | 0 | 0% | — | −$42 total | |
| MOMENTUM_IGNITION | 13 | 3 | 23% | $124.8 | $17.4 | −86% |
| STRUCTURAL_ENTRY | 6 | 3 | 50% | $93.5 | −$10.0 | broke |
| PRICE_ACTION | 4 | 1 | 25% | $22.2 | −$9.2 | broke |
| GAP (singles) | 3 | 2 | 67% | $108.3 | $58.2 | −46% |

**The two directional-structure families EARNED MORE PER DAY on the new data than on the old.**
The book's overall degradation is attributable to MOMENTUM_IGNITION, PRICE_ACTION and
STRUCTURAL_ENTRY reversing, not to a uniform decay. MOMENTUM_IGNITION had the second-largest
old-segment net ($4,536) and delivered −$70 new — the single largest source of illusion.

**Structural class predicts persistence better than in-sample statistics do.**

### 4.3 Concurrence — validated
Trades sharing the same entry bar (concurrent convergence depth):

| Depth | OLD tr / WR / PF | NEW tr / WR / PF / net |
|---|---|---|
| 1 (solo) | 472 / 90% / 4.12 | 201 / 73% / **1.25** / $1,508 |
| 2 | 342 / 91% / 5.89 | 118 / 88% / 3.33 / $2,014 |
| 3–4 | 111 / 100% / 999 | 40 / 92% / 7.43 / $893 |
| **5–7** | 50 / 100% / 999 | **16 / 100% / 999 / $3,993** |

Depth >=2 alone lifts new-segment PF from 2.19 to **7.89**. Depth 5+ retained **100% WR** on new data.
**Concurrent convergence depth is a persistent, independent quality marker.**

### 4.4 Conviction — held almost perfectly

| Tier | OLD tr / WR / PF / net | NEW tr / WR / PF / net |
|---|---|---|
| **2x (Hurst p90 / D2D agree)** | 121 / 94% / 13.42 / $8,839 | **38 / 95% / 11.81 / $5,335** |
| 1.25x (recent FailedBreak) | 291 / 95% / 8.95 / $9,391 | 94 / 77% / 1.20 / $448 |
| base 1x | 563 / 90% / 4.07 / $13,130 | 243 / 81% / 1.61 / $2,625 |

**38 conviction-2x trades out-earned 243 base trades on the new segment.** Hurst p90 standalone:
50 trades, **94% WR, PF 15.96, $5,490**.

### 4.5 Where the money is — thrust
New-segment trades bucketed by the absolute 60-bar forward price move around entry:

| Quartile | Median move | Trades | WR | PF | Net |
|---|---|---|---|---|---|
| Q1 smallest | 19 pts | 94 | 80% | 1.80 | $1,189 |
| Q2 | 54 pts | 94 | 85% | 1.74 | $819 |
| Q3 | 120 pts | 94 | 78% | 0.97 | −$81 |
| **Q4 biggest** | **204 pts** | 93 | 82% | **4.27** | **$6,481** |

**77% of all new-segment profit came from the largest-move quartile.** Depth-5+ entries carried the
largest median forward move (131 pts). By hour (EST): 12:00 → $4,674 (102 tr); 10:00 → $1,621
(14 tr, **100% WR**, highest mean concurrency 2.43).

**The system is a big-move capture engine.** The variables and their stacking are *characteristics of
the most opportune moments*, not standalone edges. This also explains why March (the crash) was the
best month — maximum directional thrust.

---

## 5. THE GATE DISCOVERY

Gates applied post-hoc to the same BOOK-50 trades, normalised per trading day:

| Gate | OLD | NEW |
|---|---|---|
| none (current) | 975 tr, PF 6.08, $640/d, wd −$204 | 375 tr, PF 2.19, $467/d, wd −$565 |
| AT_Slope_ST aligned | 630 tr, PF 6.63, $436/d | 268 tr, PF 3.90, **$540/d** |
| AT_ST + ADX>=25 | 326 tr, PF 10.17, $310/d, wd −$88 | 102 tr, PF 8.28, **$383/d**, wd −$157 |
| **AT_ST + ADX>=25 + conc>=2** | 179 tr, PF **25.97**, $194/d | **54 tr, PF 94.14, $337/d, wd +$13** |
| AT_ST + Hurst p90 | 91 tr, PF 13.57, $138/d | 36 tr, PF **68.01**, **$286/d** |
| conc>=2 | 503 tr, PF 12.14, $343/d | 174 tr, PF 7.89, **$383/d** |

**Every gated variant earned MORE per day on the new segment than the old.** The gate removes
precisely the trades that fail under regime change. `AT_ST + ADX>=25 + conc>=2` produced **no losing
day at all** on the new segment (worst day +$13).

### Gate independence (overlap matrix, P(col | row))

| | AT | D2Ddir | Hurst90 | ADX25 | conc2 |
|---|---|---|---|---|---|
| AT | 100% | 46% | **14%** | 48% | 54% |
| D2Ddir | **90%** | 100% | 15% | 55% | 26% |
| Hurst90 | 63% | 33% | 100% | 46% | 44% |
| conc2 | 72% | 18% | 13% | 50% | 100% |

- **D2D_Trend_Dir and AT_Slope_ST are near-collinear** (D2D-aligned bars are AT-aligned 90% of the time).
- **Hurst p90 is largely independent of AT_ST** (14% overlap) — it measures persistence, not direction.
- **Concurrency is independent** of both.

### Marginal PF added to AT_ST alone (new segment)
| + gate | PF | delta |
|---|---|---|
| AT alone | 3.90 | — |
| + Volume>=300 | 3.44 | −0.46 |
| + ADX>=25 | 8.28 | +4.38 |
| **+ conc>=2** | **35.78** | **+31.88** |
| **+ Hurst p90** | **68.01** | **+64.10** |

**CAUTION — an earlier reading that "D2D_Trend_Dir adds nothing" is WITHDRAWN as mis-measured.**
The test compared D2D_Trend_Dir against trade direction at the entry bar. 889 of 1,350 trades
(66%) have D2D_Trend_Dir *opposed* to trade direction — and that group produced **$29,700 at PF 5.04**,
the best-performing bucket. Many committed signals are pullback entries that intentionally fire
against the short-term direction. The system's D2D gate operates on `D2D_Signal` (the flip event),
not on `D2D_Trend_Dir` agreement. **D2D's contribution must be re-measured correctly before any
conclusion is drawn.** Standalone raw D2D remains a ~77% WR behaviour over the full history.

### 5.1 THE AT_Regime_ST GATE — the headline actionable finding

`AT_Slope_ST` is continuous (848 distinct values, −0.00093 → +0.000813). The 171-variable set also
carries **`AT_Regime_ST`, a native binary state (0/1)** — no derivation required, already exported,
already live in the EA.

**ENCODING (verified against 100,000 bars — note the inversion):**
- `AT_Regime_ST == 1` → slope **NEGATIVE / bearish** (97% agreement: 47,237 of 48,813 bars)
- `AT_Regime_ST == 0` → slope **POSITIVE / bullish** (97% agreement: 48,809 of 50,272 bars)
- Split is near-even: 49% Regime==1, 51% Regime==0.

**IMPLEMENTATION WARNING: the intuitive reading is backwards. `AT_Regime_ST == 0` is BULLISH.**
CONFIRMED against DOT.cs (L3102: `curr_AnchorType_ST = (st_Slope > 0.0) ? 0 : 1`; corroborated
L3113). This open item is CLOSED.

**AT_Regime_ST IS NOT sign(AT_Slope_ST).** DOT.cs L3110 updates the anchor only when a slope sign
change coincides with a qualifying flip (`isStandardFlip_ST || isStrongFlip_ST`); otherwise the prior
state LATCHES through. The ~4% disagreement with slope sign is that hysteresis, not noise. Therefore
the cross-check below (267 tr native vs 268 tr derived) is **two related variables agreeing closely,
not one variable measured two ways.**

**GATE DECISION: use the native binary `AT_Regime_ST` state.** It is the EA's own trend-strength
output, requires no derivation, carries no export=live risk, and its latch resists whipsaw in flat
regimes. Do not substitute `sign(AT_Slope_ST)`.

**Gate result — BOOK-50 trades split by whether AT_Regime_ST direction matches trade direction:**

| Segment | Aligned | Misaligned |
|---|---|---|
| **OLD (Apr08–Jun25)** | 630 tr, WR 92%, PF 6.63, **+$21,377** | 345 tr, WR 92%, PF 5.21, +$9,983 |
| **NEW (Jun25–Jul21)** | **267 tr, WR 87%, PF 3.83, +$9,228, wd −$116** | 108 tr, WR 68%, PF 0.78, **−$821, wd −$535** |

*(Cross-checked two ways: the native `AT_Regime_ST` state gives 267 tr / PF 3.83 / $9,228 / wd −$116;
independently deriving `sign(AT_Slope_ST)` gives 268 tr / PF 3.90 / $9,729 / wd −$114. Same answer,
two methods.)*

**THE THREE CONSEQUENCES:**

1. **The aligned subset EXCEEDS the whole book's new-segment net.** Aligned = +$9,228 vs the full
   book's +$8,407. The misaligned trades subtract from the result; removing them adds ~$821.
2. **The worst-day problem lives entirely in the misaligned trades.** Book worst day = −$565.
   Aligned-only worst day = **−$116**. Misaligned worst day = −$535. Gating cuts tail risk ~5x.
3. **It holds in BOTH segments** — PF 6.63 old / 3.83 new, positive throughout.

**REGIME INTERPRETATION (why the misaligned group collapsed):** in the OLD segment the misaligned
(counter-slope) trades were perfectly healthy — WR 92%, PF 5.21, +$9,983. These are pullback entries:
buying a dip against short-term slope, which works when there is an established trend to pull back
*into*. In the NEW flat/directionless segment there was no trend to resume, so the "pullback" simply
continued and they bled (PF 0.78). **AT_Regime_ST alignment therefore functions as a trend-presence
filter, not as a signal-quality filter.** The signals need not be deleted — they need gating on trend
context.

**This is a single-variable gate addition using a variable already present in the export and already
computed live by the EA.** It is not a rebuild.

### Over-gating boundary
Stacking six conditions (conc>=2 + ADX>=30 + Vol>=300 + AT_ST + D2D + Hurst) collapses to **2 trades
old / 0 new**. Each gate filters 40–60%; six multiply to nothing. **2–3 gates is the workable range.**

### Sequence structure (lead–lag)

**CORRECTION — the earlier "all three aligned at bar X" test was conceptually wrong and its
conclusions are withdrawn.** D2D, AT_ST and Hurst are not coincident measures and cannot be tested
as an AND-gate at a single bar:

- **D2D** = break-of-structure, event-driven.
- **AT_Slope_ST / AT_Regime_ST** = logarithmic regression channel state, continuous.
- Both lag; **neither reliably leads.** Which fires first depends on how the turn forms (e.g. a
  downtrend reverting after a failed break may flip D2D before AT_ST, or the reverse).
- **Micro_Hurst p90** measures trend *continuation*, so by construction it can only register **after**
  direction is established. It cannot be high at reversion or at peak volatility.

The correct model is a **sequence with variable order**: direction is established (D2D and/or AT_ST,
either order) → *then* Hurst p90 confirms continuation. When D2D and AT_ST are in agreement —
regardless of which arrived first — Hurst p90 conviction is at its strongest. In a flat regime both
measures flip more frequently and independently, so they agree less often; in an established trend
they agree most of the time. **This is why the "AT_ST + Hurst p90" pairing scored PF 68 — it is not a
three-way coincidence, it is the post-confirmation state.**

Measured (noting the above caveat about what alignment-at-entry can and cannot tell us):
- **AT_Slope_ST aligns first in 49% of trades**; D2D first in 17%; equal 33%.
- **Winners entered with AT_ST aligned a median 4 bars; losers only 1 bar.** *Duration* of alignment
  predicts outcome, not merely the aligned state.
- ADX>=15 is **redundant** — every trade already clears it. ADX>=25 is the live threshold.
- Volume>=300 is **net negative** as a filter (−0.46 PF, and −$125/day vs Volume>=100).

---

## 6. WHAT THIS EXPOSES ABOUT THE SELECTION METHODOLOGY

The selection funnel ran **1.7M candidates → 89K → 51K → 1,788 → 50**. Three structural gaps:

1. **No multiple-testing correction.** With 1.7M trials, thousands of pure-noise signals will show
   PF 6+ in any 5-month window by chance alone; in-sample they are indistinguishable from real edges.
   Nothing in the funnel raised the significance bar in proportion to the number of trials.
   *Fixes: Deflated Sharpe Ratio, False Discovery Rate control (Benjamini–Hochberg), or a trial-count-scaled bar.*

2. **Single-pass selection, not stability selection.** The book was selected once on the full window.
   *Fix: re-run the entire selection on many bootstrapped subsamples (e.g. 100x on random 60% slices)
   and retain only signals selected in the large majority of runs.* Noise survives a few runs; real
   edges survive nearly all. **Highest-leverage single change.**

3. **Aggregate OOS, not regime-conditional persistence.** OOS was one contiguous block (2/3 → 1/3).
   A signal passes by being excellent in one regime and neutral in another.
   *Fix: bucket the training window by regime (trending / ranging / high-vol / low-vol) and require
   positive performance in EACH bucket.* This is precisely what would have caught the short-side
   signals, which were validated on a window containing enough downside and failed when that
   character disappeared.

**All validation to date (blind audit, OOS, 6-fold, decorrelation) occurred INSIDE the Jan–Jun window.
"Real in-window" and "persists across regimes" are different claims. Only the second was ever the goal.**

---

## 7. THE PROPOSED PARADIGM SHIFT — GATE FIRST, THEN SEARCH

Current: discover signals across ALL bars → filter afterwards.
Proposed: **define the tradeable universe first** (bars passing thrust conditions: AT_ST alignment +
ADX>=25, optionally conc>=2) → **then search for convergence only within those bars.**

Why this is materially different:
- The gates are **pre-trade market-state conditions**, not performance-selected outcomes. Conditioning
  on them therefore does **not** curve-fit in the way that selecting on returns does.
- The candidate universe shrinks dramatically → fewer trials → far less multiple-testing bias.
- Signals discovered inside the gate encode "what works when the market is genuinely moving" — a
  structural property — rather than "what worked on average across all conditions", which is where the
  regime-specific noise entered.

---

## 8. STATUS OF THE DATA ASSET

The stitched series now spans **2026.01.19 → 2026.07.21** — a single continuous ~6-month geometric
palette (baseline 152,983 bars + new export, overlapping and verified identical on 75,732 shared bars).
This should be treated as ONE dataset for rolling-window persistence testing, not as "old vs new".

---

## 9. OPEN ITEMS FOR THE QUANT

1. **Rolling-window persistence** across the full stitched Jan–Jul series (e.g. 4-week windows):
   which behaviours hold in 5 of 6 windows vs 2 of 6?
2. **Walk-forward on the SELECTION PROCESS** — select on split A, test untouched on split B, across
   3–4 distinct splits. Does the methodology produce out-of-split persistence, or fit in-window noise?
3. **Re-measure D2D correctly** (`D2D_Signal` flip-based, and pullback-entry aware) — the earlier
   agreement test was confounded.
4. **Lead–lag structure**: AT_ST aligns → how many bars to D2D confirmation → how many to Hurst p90?
   Is there a repeatable timing sequence that can be traded (enter on AT_ST, size on D2D, max
   conviction on Hurst) rather than an AND-gate?
5. **Quantify the gate-first paradigm**: run discovery restricted to gated bars and compare the
   resulting book's out-of-split persistence against the current 50%.
6. **Long/short asymmetry**: shorts collapsed (PF 5.38 → 0.74, negative) while longs held
   (6.97 → 3.20). Systematic, or an artifact of an up-drifting doldrums regime? Do shorts require the
   gating the longs implicitly have?
7. **Fix the hardcoded OOS window** in the scorer — currently fixed to May–Jun. Must become
   data-relative (last third of loaded data, or an `--oos-from` parameter).
8. **Add a per-trade CSV export** to master.py so performance can be sliced without re-running.
9. **Validate the `AT_Regime_ST` gate** (section 5.1) across rolling windows on the full stitched
   Jan–Jul series — does it hold continuously, and what is the optimal treatment of the misaligned
   pullback signals (gate on trend-presence vs remove)?
10. ~~Confirm the `AT_Regime_ST` encoding against DOT.cs~~ **CLOSED** — confirmed at DOT.cs L3102/
   L3113 and across 177,251 bars. Additionally established: AT_Regime_ST is a LATCHED anchor, not an
   instantaneous slope sign. The native binary state is the ratified gate variable.

---

## 10. SUMMARY POSITION

- The data pipeline is **sound and verified**.
- The system **made money on genuinely unseen data and never threatened the risk ceiling**.
- **Two market structures (trend continuation, breakout expansion) improved on new data.**
- **Concurrence, Hurst-p90 conviction, and AT_Slope_ST alignment all persisted** and are largely
  independent of one another.
- **77% of new-segment profit came from the largest-move quartile** — the edge is big-move capture.
- Selection persists at **~2x random**, which is real but insufficient; three specific, standard
  rigor upgrades are available.
- A **gate-first selection paradigm** is proposed as the structural fix.

### THE SINGLE MOST ACTIONABLE OUTCOME
**`AT_Regime_ST` alignment is a one-variable gate, already native to the 171-column export and already
computed live by the EA, that on the new segment:**
- **raises net above the ungated book** (+$9,228 vs +$8,407),
- **lifts PF 2.19 → 3.83 and WR 81% → 87%**,
- **cuts the worst day from −$565 to −$116 (~5x tail-risk reduction)**,
- **and holds in the old segment too** (PF 6.63, +$21,377).

The trades it removes are counter-slope pullback entries whose viability depends on an established
trend — healthy in the trending segment (PF 5.21), loss-making in the flat one (PF 0.78). The gate is
therefore a **trend-presence filter**, and the affected signals likely need gating rather than deletion.

Nothing here requires rebuilding the system from zero. The core is intact and measurable; the work is
to make selection target what demonstrably persists.
