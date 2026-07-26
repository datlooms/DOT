# equiDOT.cs CODEBASE MAP — BOOK-50 Build Reconnaissance
## File: equiDOT.cs | 11,673 lines | Target: BOOK-50 (48 F0 + 2 F1, momentum-runner, 6-lot live-risk jar)

This maps the CURRENT (frozen, compiles 0/0) equiDOT.cs and flags exactly where
the three authorized BOOK-50 changes land. It is a reading aid for the build; it
changes no code. The current EA is the live, working system — build first,
deconstruct second (see DOT_linear_development_schedule.txt).

---

## 1. SECTION INDEX

```
SECTION 1.0   [  102 –  536]  INITIALISATION (OnInit @195)
SECTION 1.1   [  537 – 1120]  EXPORTDATAFORANALYSIS (ExportDataForAnalysis @564)   !! CARRIES THE EXPORT-CLOCK DEFECT at L730-731 — see §5
SECTION 1.2   [ 1121 – 1413]  MAIN LOOP (OnTick @1123)              << change (e): gate Dots SL reposition to new-bar
SECTION 1.3   [ 1414 – 1425]  TIDYING UP & HEALTH CHECKS
SECTION 1.4   [ 1426 – 1511]  KAMA WARM-START PERSISTENCE            *** SACRED ***
SECTION 2.0   [ 1512 – 1725]  USER SETTINGS (externs)
SECTION 2.1   [ 1726 – 1780]  GLOBAL TIMING MODULE (EST / DST)      LIVE PATH IS CORRECT — DO NOT TOUCH (see §5)
SECTION 3.0   [ 1781 – 2147]  GLOBAL VARIABLES & MEMORY              << F1 state + jar counter
SECTION 4.0   [ 2148 – 2174]  TEMA-ATR CALCULATION
SECTION 4.1   [ 2175 – 2220]  TRUE RANGE CAPPING
SECTION 5.0   [ 2221 – 2254]  ADX CLASSIFICATION (InitADXClassifier)
SECTION 5.1   [ 2255 – 2286]  MOMENTUM CLASSIFICATION
SECTION 6.0   [ 2287 – 2945]  CALCULATION ENGINE HELPERS (STATEFUL)  *** SACRED chain ***
                              ResizeAllArrays @2289, ResizeAndSmartShift @2441
SECTION 6.1   [ 2946 – 3169]  ADAPTIVE TREND CALCULATION (STATEFUL)
SECTION 6.2   [ 3170 – 3263]  HISTORICAL DRAWING (STATEFUL)
SECTION 6.3   [ 3264 – 3348]  OBV CALCULATION (Calc_OBV_OnBar @3296)
SECTION 6.4   [ 3349 – 3403]  HARMONIC VOLUME LLEMA
SECTION 6.5   [ 3404 – 3729]  MICROSTRUCTURE (Calc_Microstructure_OnBar @3406)  << Micro_LogReturn, Micro_OrderFlowDelta
SECTION 6.6   [ 3730 – 3761]  DOTS DERIVED (Calc_Dots_Derived_OnBar @3732)
SECTION 6.7   [ 3762 – 4491]  DOTS FEATURE CONSTANTS (InitDotsThresholds @3856)  *** SACRED thresholds ***
SECTION 6.8   [ 4492 – 4588]  DOTS RULE TABLE (InitDotsRuleTable @4510)          << 50-signal swap + F1 register
SECTION 6.9   [ 4589 – 4848]  DOTS SIGNAL EVALUATION (Eval_Dots_Signals @4805)   << F1 latch write + lagged fire
SECTION 7.0   [ 4849 – 5331]  CHART ALERTS & VISUALS
SECTION 7.1   [ 5332 – 5573]  SUPERTREND VISUALS
SECTION 7.2   [ 5574 – 5671]  CUSTOM TRADE HISTORY VISUALS
SECTION 7.3   [ 5672 – 5866]  ADAPTIVE TREND VISUALS
SECTION 7.4   [ 5867 – 5991]  ADAPTIVE TREND (channels, direction indicators)
SECTION 7.5   [ 5992 – 6238]  POINT OF CONTROL CALCULATION & VISUALS
SECTION 7.6   [ 6239 – 6475]  SESSION VISUALS
SECTION 7.7   [ 6476 – 6991]  OBV VISUALS
SECTION 7.8   [ 6992 – 7005]  UI ANIMATION & TICK TRACKING
SECTION 7.9   [ 7006 – 7098]  VOLUME DOT MATRIX VISUALS
SECTION 7.10  [ 7099 – 7623]  DOTS VISUAL PANEL                       << panel -> 50 rows
SECTION 8.0   [ 7624 – 8031]  TRADE EXECUTION & MANAGEMENT (D2D + OBVfriend)
SECTION 8.1   [ 8032 – 8218]  ADVANCED TRADE MANAGEMENT              << momentum-runner + jar BE-free decrement
SECTION 8.2   [ 8219 – 8555]  PARTIAL TP VISUALS & LOGIC
SECTION 8.3   [ 8556 – 8613]  ORDER MANAGEMENT HELPERS
SECTION 8.4   [ 8614 – 8683]  SESSION ENFORCEMENT LOGIC (Friday close)   correct live (runs off GetEstTime); see §5
SECTION 8.5   [ 8684 – 8770]  DATA CACHING HELPER & RE-PAINT
SECTION 8.6   [ 8771 – 8852]  DOTS TRADE MANAGEMENT                  << lag entry + jar admission + momentum-conditional initial SL
SECTION 8.7   [ 8853 – 8926]  DOTS HELPERS
SECTION 8.8   [ 8927 – 9057]  DOTS POSITIONS & ALERTS               << ManageDotsPositions: per-tick -> per-bar SL reposition
SECTION 9.0   [ 9058 – 11020] STATISTICS PANEL
SECTION 9.1   [11021 – 11059] DAILY SUMMARY REPORTING
SECTION 10.0  [11060 – 11448] UI BUTTON EVENTS
SECTION 10.1  [11449 – 11499] TIMER EVENT
SECTION 11    [11500 – 11673] THE SHUTDOWN ROUTINE (STATEFUL)
```

---

## 2. WHERE THE SEVEN CHANGES LAND

### Change (a) — Momentum-runner TM branch
- **Primary: SECTION 8.1 ADVANCED TRADE MANAGEMENT [8032–8218]** — the LeapFrog
  trailing-stop arithmetic. Replace the fixed lag with a per-position
  `leapFrogLag` in the trail formula `SL = entry ± (TiersReached − lag) × step`
  (tier ≥ 3 only, favorable-move only).
- **Support: SECTION 8.6 DOTS TRADE MANAGEMENT [8771–8852]** — at the point a
  Dots position opens, capture `entryLogReturn = Micro_LogReturn` (the closed
  entry bar, from SECTION 6.5) and set `leapFrogLag = (entryLogReturn*dir >=
  0.00012) ? 3 : 2`. Store both on the position.
- **Untouched:** initial SL, BE nudge, tier detection (still in 8.0/8.1), Friday
  close (8.4), entry logic.

### Change (b) — Sequential-latch subsystem (2 F1 signals)
- **State: SECTION 3.0 GLOBAL VARIABLES & MEMORY [1781–2147]** — two 16-slot
  boolean ring buffers (`latchBuf_SqzHiFlip[16]`, `latchBuf_ADXris0Flip[16]`) +
  a rolling index. The only new persistent state; resized/shifted via the
  SECTION 6.0 `ResizeAndSmartShift` discipline (one shift per bar).
- **Latch write + lagged fire: SECTION 6.9 DOTS SIGNAL EVALUATION [4589–4848]**
  (`Eval_Dots_Signals` @4805) — after the bar's variables and `ST_Flip_Event`
  are final, write the two latches (Sqz_Val:hi & flip ; ADX_Rising==0 & flip),
  then evaluate the two F1 fires via lagged lookup (lag 13 / lag 8) AND the
  current leg-B condition AND the D2D gate.
- **Register: SECTION 6.8 DOTS RULE TABLE [4492–4588]** (`InitDotsRuleTable`
  @4510) — add the 2 F1 entries with a `triggerKind = F1_SEQ` flag so the
  dispatcher and panel treat them correctly. They feed the SAME gate and
  S.7/runner exit as every F0 signal.
- **Thresholds:** Sqz_Val:hi and Micro_OrderFlowDelta:lo route through the
  existing adaptive table in **SECTION 6.7 [3762–4491]** (`InitDotsThresholds`).
  No new threshold source. ADX_Rising==0 and D2D_DirStep==−1 are direct state
  equalities on existing variables (SECTION 6.6 derived / 6.1 trend).

### Change (c) — The 50-signal swap
- **SECTION 6.8 DOTS RULE TABLE [4492–4588]** — build the 48 F0 triples
  (PART I of the spec) + the 2 F1 entries (PART II). 50 total (37L / 13S).
- **SECTION 6.7 [3762–4491]** — only if a leg threshold not already present in
  the table needs registering (still through the oracle).
- **SECTION 7.10 DOTS VISUAL PANEL [7099–7623]** — extend to 50 rows across the
  8 structures; show F0/F1 trigger type for the 2 sequential fills.

### Change (d) — 6-lot live-risk jar (replaces the MAX_POSITIONS=6 count cap)
- **State: SECTION 3.0 GLOBAL VARIABLES & MEMORY [1781–2147]** — a single running
  live-lot counter `int g_dots_live_lots = 0;` (the number of open Dots positions NOT
  yet at break-even; each = 1 lot). One integer — no ring buffer, no shift.
- **Admission + increment: SECTION 8.6 DOTS TRADE MANAGEMENT [8771–8852]** (Dots entry)
  — REPLACE the MAX_POSITIONS count check with the jar check: admit a new Dots signal
  ONLY IF `g_dots_live_lots < 6`; on open, `g_dots_live_lots += 1`. Always 1 lot (no
  fractional sizing). If the jar is full (== 6) the signal is skipped, exactly as the
  count cap skipped a 7th position.
- **BE-free decrement: SECTION 8.1 ADVANCED TRADE MANAGEMENT [8032–8218]** — at the exact
  bar a Dots position first reaches break-even (SL moved to entry±step, step 3 — it can no
  longer lose), `g_dots_live_lots -= 1`: the winner's lot leaves the jar. Decrement EXACTLY
  ONCE, guarded on the same `be` flag that gates the one-time BE nudge (no double-decrement).
- **Still-live exit: SECTION 8.6 / 8.1** — a position that CLOSES while still pre-BE (an SL
  hit) also decrements; one that closes AFTER break-even was already decremented at the BE
  transition, so it is NOT decremented again. Invariant: `g_dots_live_lots` == count of open,
  not-yet-BE Dots positions, and NEVER exceeds 6.
- **Untouched:** SL / step / BE nudge / tier / LeapFrog / runner / Friday close / entry gate /
  eligibility — the jar changes ADMISSION only, keyed on the live-lot count.

### Change (e) — Per-bar SL repositioning (per-tick -> per-bar; parity fix)
- **Function: SECTION 8.8 DOTS POSITIONS & ALERTS [8927–9057]** — `ManageDotsPositions`
  (@8929) is the DOTS SL manager. It currently reads the LIVE TICK (`RefreshRates();`
  `favourable=(Bid-entry)/Point` for long / `(entry-Ask)/Point` for short, @8961–8964) and
  repositions the SL — arm @8971–8976, LeapFrog trail @8988–9008 — on EVERY tick. Change it to
  reposition ONLY on a new bar, computing favourable / tiers / arm / trail off the CLOSED bar's
  `High[1]/Low[1]` — NOT Bid/Ask, NOT Close.
- **Call site / gate: SECTION 1.2 MAIN LOOP (OnTick @1123)** — the call `if(UseDots)`
  `ManageDotsPositions();` sits at @1380, at OnTick's TOP LEVEL, OUTSIDE the `if(isNewBar)`
  block (@1157). Gate the reposition logic to new-bar detection so it runs ONCE per closed bar
  (move the call inside the isNewBar block, or gate internally on a `Time[0]!=dotsLastBar` guard).
- **Per-bar sequence (in ManageDotsPositions, per open position):** (1) exit-check vs current SL
  -> (2) update tiers off closed-bar high/low -> (3) arm/lock BE -> (4) trail. Reproduces
  portfolio_simulation_engine.py.
- **UNTOUCHED:** the BROKER's hard SL (still executes intrabar on any tick — downside protection
  is always live per-tick), LOCK_FRAC / BE_TRIG_FRAC, and the SL / step / BE / tier / trail
  arithmetic. Change (e) alters WHEN the EA repositions the SL, not the values it computes.

### Change (f) — Momentum-conditional wider initial SL
- **Landing: SECTION 8.6 DOTS TRADE MANAGEMENT [8771–8852]**, the order-open path where the
  initial risk is computed (~line 8794): `double atr = ATR_1M_Array[1];` then
  `double risk = MathMin(atr * Dots_SL_Mult, Dots_SL_Cap);`. Split it into TWO risks — keep a
  base_risk (always ×2) for BE/step, and a momentum-widened catastrophe risk for the SL only:
  `double base_risk = MathMin(atr * Dots_SL_Mult, Dots_SL_Cap);`
  `double mult = (v >= 0.00012) ? 4.0 : Dots_SL_Mult;`
  `double risk = MathMin(atr * mult, Dots_SL_Cap);`  (catastrophe stop uses `risk`;
  `rawSL = entryPrice ± risk`), and the step at ~line 8824 uses base_risk:
  `double step = Dots_StepFrac * base_risk;`  (NOT `risk`).
- **Reuses the runner's momentum value** — `v` is the SAME `Micro_LogReturn × dir` already
  computed at Dots entry for the runner lag (change a / S.12); NO new variable, NO new buffer.
- **UNTOUCHED:** `Dots_SL_Cap` = 150 (the inviolate $150 MAX_RISK ceiling — no trade risks more
  than $150+spread), and `Dots_SL_Mult` = 2.0 stays the base constant (S.17). Uses
  `ATR_1M_Array[1]` (closed bar) — consistent with the per-bar model (change e).
- **TWO-RISK SPLIT (EA-CRITICAL, S.19):** the momentum widening is the CATASTROPHE STOP ONLY.
  Compute a separate `base_risk = MathMin(atr * Dots_SL_Mult, Dots_SL_Cap)` (always ×2), and use
  base_risk for the BE-arm trigger and the step_size (`step = Dots_StepFrac * base_risk` — at
  ~line 8824, NOT the widened `risk`). ONLY the initial SL (`rawSL = entryPrice ± risk`) uses the
  momentum-widened `risk`. Wiring step/BE off the widened risk arms break-even too late and blows
  worst-day to ~-320 (SL up to 202) — the regression caught during the engine merge. Verify the
  `step` at line 8824 reads base_risk, not the momentum `risk`.

### Change (g) — Conviction self-scaling + gap-singles (S.20)
- **Landing (lot multiplier): SECTION 8.6 DOTS TRADE MANAGEMENT**, the order-open path where `lots`
  is set before `OrderSend` (~line 8824). For a book-LONG, read `Micro_Hurst` on the signal bar
  (`Micro_Hurst_Array[1]`) and its adaptive p90 via the oracle: if `Micro_Hurst > p90` -> `lots = 2.0`,
  else `1.0`; LONGS ONLY (no short edge). PLUS a recentFB flag (a book long within 5 bars of a
  Micro_FailedBreak-extreme) -> `lots = 1.25`; a long qualifying for both takes the higher (2.0), never
  the product. Base lot stays 1.0 — the multiplier IS the scaling (S.20 SACRED: 1-LOT BASE ONLY).
- **Landing (gap-single entries): the Dots entry qualification loop** (~line 8790), alongside the
  BOOK-50 signal checks. Two new single-condition entries: `Micro_Hurst > p97 & D2D_Trend_Dir==+1`
  (LONG) and `Micro_FailedBreak > p90 & D2D_Trend_Dir==-1` (LONG, counter/reversion). Gate
  `ADX_Value>=15 & Volume>=300`. Each opens 1.0 lot at LOCK=3. **Entry-gate:** a gap-single is admitted
  ONLY when the count of open Dots positions is zero (any open body — live-risk OR breakeven'd — blocks
  it). Per-bar order: book signals first (with the Hurst 2x/1x sizing); gap-singles only when no book
  signal qualifies AND the book is flat.
- **UNTOUCHED:** the 6-lot jar (S.15) is shared by all entries; the oracle thresholds (Hurst p90/p97,
  FailedBreak p90) are computed the same adaptive way as every book threshold — nothing new to calibrate.

---

## 3. SACRED — DO NOT MODIFY (behavior-changing edits INVALID without human sign-off)

- **SECTION 1.4 KAMA WARM-START PERSISTENCE [1426–1511]** — file write/read/
  validate/anchor; export cold-guard.
- **SECTION 6.0 CALCULATION ENGINE HELPERS [2287–2945]** — `ResizeAndSmartShift`
  (@2441) is the sole shift authority; the stateful pass order is sacred.
- **SECTION 6.7 DOTS FEATURE CONSTANTS [3762–4491]** — the adaptive threshold
  init (Mechanism D + structural). Retired mechanisms A/B/C must not reappear.
- **SECTION 1.1 EXPORTDATAFORANALYSIS [537–1120]** — the 171/172-column export
  schema and RebuildStateForExport (δ=0) parity path.
- **Locked tunables (SECTION 2.0):** Dots_RollingBufferSize = 2500,
  Dots_InitBars = 6900, and the S.7 trade-management constants.

The seven changes are additive and bounded. They do not touch the calculation
memory chain, the threshold oracle, the KAMA persistence, or the export schema.

---

## 4. KEY FUNCTIONS (quick reference)

```
OnInit                     @195     seeds buffers, KAMA warm-start, init tables
ExportDataForAnalysis      @564     CSV export (parity path) — SACRED, WITH ONE AUTHORISED
                                    EXCEPTION: the L730-731 clock defect (§5). Sacred here means
                                    schema/column-order/value-definition locked, NOT that the
                                    defect is preserved. The clock fix is the only permitted edit.
OnTick                     @1123    main loop
ResizeAndSmartShift        @2441    sole shift authority — SACRED
Calc_Microstructure_OnBar  @3406    Micro_LogReturn, Micro_OrderFlowDelta (F1/runner inputs)
Calc_Dots_Derived_OnBar    @3732    derived state (D2D_DirStep, etc.)
InitDotsThresholds         @3856    adaptive threshold table — SACRED source
InitDotsRuleTable          @4510    << signal set (50-signal swap + F1 register)
Eval_Dots_Signals          @4805    << F1 latch write + lagged fire; signal dispatch
(SECTION 8.1 trail block)  ~8032    << momentum-runner lag in LeapFrog trail
(SECTION 8.1 BE transition) ~8032    << jar BE-free decrement (winner's lot leaves jar)
(SECTION 8.6 Dots entry)   ~8771    << capture entryLogReturn + set leapFrogLag
(SECTION 3.0 state)        ~1781    << g_dots_live_lots counter (live-risk jar)
(SECTION 8.6 Dots admit)   ~8771    << jar admission (live_lots < 6) + increment
ManageDotsPositions        @8929    << change (e): per-tick -> per-bar SL reposition (closed-bar H/L)
(OnTick Dots mgmt call)    ~1380    << change (e): gate to new-bar (currently every tick)
(Dots entry risk calc)     ~8794    << change (f): momentum-conditional min(ATR x4,150) initial SL
(Dots entry lots calc)     ~8824    << change (g): conviction lot multiplier (Hurst>p90 -> 2x book longs)
(Dots entry qualification) ~8790    << change (g): 2 gap-single entries, gated to zero-Dots-open
(Dots entry lots calc)     ~8824    << change (h/S.21): D2D-CONVICTION 2x sizer, BOTH dirs
                                       (D2D_Signal==dir & ADX>=30 & Micro_Hurst>=p30); requires
                                       short_mult -- the EA must SIZE SHORTS. Higher-mult-wins, never 4x.
(Dots gap-entry, flat)     ~8790    << change (h/S.21): D2D-GAP-FILLER flat-2-lot standalone entries
                                       when zero Dots open; assign lots=2.0 DIRECTLY (bypass conviction,
                                       no 4x); book LOCK_FRAC=1.0 (S.20 gaps keep GAP_LOCK=3). 14 gaps.
```


---

## 5. THE EXPORT CLOCK DEFECT (found 2026-07-27; EA FROZEN, fix PENDING)

**READ THIS BEFORE EDITING SECTION 1.1. A developer following the rest of this map would
rebuild the defect.** The law is S.22 in `non_negotiable_prompts/non_negotiables_developer.txt`;
this section is the working detail at the point of use.

### 5.1 The defect — two errors stacked, L730-731

```
L1761  long _GetEstOffsetForTime(datetime gmtTime) { ... }    <-- PARAMETER DECLARED gmtTime
L1770  datetime GetEstTime() { return (datetime)(TimeGMT()+(datetime)GetUSEasternOffsetSeconds()); }   LIVE — CORRECT
L730   long estOffset=_GetEstOffsetForTime(Time[i]);          <-- Time[i] is the bar's SERVER time
L731   datetime estBarTime=(datetime)(Time[i]+(datetime)estOffset);
```

1. **SELECTED on the wrong instant** — `_IsUSDST` is evaluated on server time, not GMT.
2. **APPLIED to the wrong base** — the US-Eastern offset is added to server time, not GMT.

(2) is the whole error in practice. (1) is latent: on the Jan19-Jul21 span it misassigned
**zero bars**, because both DST boundaries fall inside weekend gaps (2026-03-07/08 and
2026-03-28/29 all carry 0 bars). It must still be fixed.

### 5.2 THE LIVE PATH IS CORRECT AND IS NOT TO BE TOUCHED

`GetEstTime()` at L1770 already feeds `TimeGMT()`. The chart visuals, the session containers,
the DST transitions and the live Friday cutoff at **L8781 / L8935** all run off it and all
behave correctly — observed by the operator over months of live use.

**THE MOST LIKELY WRONG FIX IS TO "HARMONISE" THE TWO BY CHANGING THE LIVE PATH. DO NOT.
Only `ExportDataForAnalysis()` changes.** One pre-existing imprecision is noted and is
explicitly OUT OF SCOPE: `_IsUSDST` compares against the second Sunday of March at 02:00 in
its argument's own clock, so even the live path flips ~5h early on that one day per year.

### 5.3 The fix, as code

```
long serverToGmtSeconds=_GetServerToGmtOffsetSeconds();
datetime gmtBarTime=(datetime)(Time[i]-(datetime)serverToGmtSeconds);
long estOffset=_GetEstOffsetForTime(gmtBarTime);
datetime estBarTime=(datetime)(gmtBarTime+(datetime)estOffset);
```

**RESIDUAL DESIGN DECISION — NOT PRE-DECIDED. How `_GetServerToGmtOffsetSeconds()` sources the
offset for a HISTORICAL bar.** `TimeGMT()` and `TimeCurrent()` give only the CURRENT offset, and
the broker's own DST schedule shifted mid-span. Three options; the choice is the human's:

- **(a) `(long)(TimeCurrent()-TimeGMT())` captured once at export.** Correct for bars in the
  same DST regime as the export run; wrong by an hour for bars in the other regime.
- **(b) Schedule-derived: `_IsUSDST(gmtBarTime) ? 3*3600 : 2*3600`.** Self-adjusting across
  regimes and valid because this broker switches on the US schedule — but it hardcodes two
  broker constants and breaks silently on a broker change.
- **(c) (a) as the anchor, stepped by the broker's own switch schedule.** Most correct, most code.

**DO NOT SHIP (b) WITHOUT RECORDING THAT IT ENCODES A BROKER PROPERTY.**

### 5.4 Measured facts — so the fix is verifiable, not asserted

- **Server-to-true-EST is a CONSTANT -7h. The broker follows the US DST schedule, not the EU
  one.** The opening bell sits at broker 16:30 in EVERY week of the span; under EU DST it would
  sit at 15:30 during 2026-03-09..03-27, and it does not.
- That falls out as **-2h on 46,425 bars** (to 2026-03-06) and **-3h on 130,826 bars** (from
  2026-03-09). The change is the **US offset moving -5 -> -4**, not the broker moving.
- **The EU/US divergence IS real** but surfaces in the London anchor: 2026-03-09..03-27 reads
  London open at **04:00 EDT, not 03:00**. A calendar rule predicting a -6h server window was
  refuted by measurement.
- **Blast radius: three columns only** — `EST_Hour`, `EST_Minute`, `EST_DayOfWeek`, at CSV field
  indices **6, 7, 8**. `estOffset`/`estBarTime` appear ONLY at L730/731/734; `estDt` ONLY at
  L732-734 and L942-944. Verified by grep, not assumed.

### 5.5 CONTAINMENT — do not re-derive the terrain

`dots_thresholds.py` L104 takes its mechanism-D day boundary from the **raw broker timestamp**
(`str(times[i])[8:10]`), NOT from `EST_Hour`. Therefore **mechanism D, every adaptive threshold,
the episode set, episode counts, the 3,816 UP / 3,674 DOWN split and the incumbent's measured
REACH are ALL UNTOUCHED.** The clock entered only as a label.

**ONLY CONCLUSIONS ABOUT *WHEN* WERE EVER CORRUPT.** Nobody needs to rebuild the terrain.

### 5.6 The relabelling list — restate, do not inherit

- S2B terrain (W=15/K=p85/E=p75, MARKET) reported peak median displacement at "12:00 midday"
  (140.4pt, 204 largest-decile). **CORRECTED it is 09:00, THE NY CASH OPEN — 144.0pt, 225
  largest-decile** — decaying through the morning, with the TRUE lunch lull at 12:00-13:00
  carrying 76.5/72.0pt, roughly half. Peak is 09:00 in all four grid cells.
- **"size>=8 clusters concentrate at 11:00-13:00 EST" RELABELS TO THE NY OPEN AND STRENGTHENS.**
  Deep clusters form where the market's largest clean directional runs are. It was never a
  midday effect.
- **ATR-tercile and monthly-bucket findings are NOT hour-derived and are unaffected.**

### 5.7 SACRED FRIDAY GATE — `portfolio_simulation_engine.py` L148 — DO NOT TOUCH

Byte-locked at `bb498eb13ce3`. It reads the export clock and **needs no change**. On the broken
clock it blocked **3,835 bars from true 13:00**, removing roughly the last three hours of every
Friday cash session. On the corrected data the **same unchanged line blocks 115 bars at true
16:45-16:49** — exactly the intended window (this feed's Friday session ends 16:49; 5 bars x 23
Fridays; 3 Fridays are early closes).

**THE GATE WAS ALWAYS CORRECT — IT WAS READING A WRONG CLOCK. A developer who "fixes" that file
will break it.**

### 5.8 EVERY HISTORICAL FIGURE WAS MEASURED WITH FRIDAY AFTERNOONS EXCLUDED

| dataset | broken clock | corrected clock |
|---|---|---|
| stitched 177,251 (Jan19-Jul21) | 3,057 tr / WR 90.9 / PF 5.07 / $98,205 | **3,101 / WR 90.6 / PF 4.81 / $97,675** |
| sealed-baseline window 152,983 (Jan19-Jun25) | 2,698 tr / WR 92.3 / PF 6.40 / $92,296 | **2,739 / WR 91.9 / PF 5.92 / $91,506** |

Newly scoreable Friday afternoon (true EST 13:00-16:59): 101 trades, net $240, **WR 74.3%** —
materially worse than the book's 90.6%, which is a real session finding rather than noise.

**EVERY FIGURE IN THE PROJECT RECORD BEFORE 2026-07-27, INCLUDING THE $92,347 CROWN-JEWEL
CANARY, WAS MEASURED WITH FRIDAY AFTERNOONS EXCLUDED.** The recorded $92,347 reproduces at
$92,296 on the broken clock (the ~$51 is seam displacement from scoring the full stitched frame
and splitting). Corrected, the same window is **PF 5.92 / $91,506**.

### 5.9 ACCEPTANCE GATE ON THE FIX — PARITY, NOT REVIEW

After the EA fix, a fresh export must reproduce the corrected columns in
`DOT_stitched172_TRUEEST_jan19_jul21_part01..10.csv` (manifest
`DOT_stitched172_TRUEEST_manifest.csv`) **exactly**, on overlapping bars. That equality IS the
test. Walk the checklist items in `DOT_post_update_checklist.txt` PHASE K.

### 5.10 How it was found — the method matters more than the defect

The terrain reported peak displacement at "12:00 midday", the flattest hour of the NY session.
**The operator rejected it from domain knowledge.** Four candidate mechanisms — ATR normaliser,
efficiency filter, episode bounding, day concentration — were each tested and each exonerated
before the clock was implicated. The opening bell was then located **from price alone** (median
range stepping 36.50 -> 93.00 and median volume 224 -> 490 in a single minute) and six
independent anchors confirmed the conversion. A calendar-derived rule was **refuted by
measurement**. That sequence — domain-knowledge challenge, mechanism elimination, price-anchored
location, multi-anchor confirmation — is the standing method.
