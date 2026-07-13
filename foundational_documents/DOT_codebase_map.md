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
SECTION 1.1   [  537 – 1120]  EXPORTDATAFORANALYSIS (ExportDataForAnalysis @564)
SECTION 1.2   [ 1121 – 1413]  MAIN LOOP (OnTick @1123)
SECTION 1.3   [ 1414 – 1425]  TIDYING UP & HEALTH CHECKS
SECTION 1.4   [ 1426 – 1511]  KAMA WARM-START PERSISTENCE            *** SACRED ***
SECTION 2.0   [ 1512 – 1725]  USER SETTINGS (externs)
SECTION 2.1   [ 1726 – 1780]  GLOBAL TIMING MODULE (EST / DST)
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
SECTION 8.4   [ 8614 – 8683]  SESSION ENFORCEMENT LOGIC (Friday close)
SECTION 8.5   [ 8684 – 8770]  DATA CACHING HELPER & RE-PAINT
SECTION 8.6   [ 8771 – 8852]  DOTS TRADE MANAGEMENT                  << lag entry + jar admission / increment
SECTION 8.7   [ 8853 – 8926]  DOTS HELPERS
SECTION 8.8   [ 8927 – 9057]  DOTS POSITIONS & ALERTS
SECTION 9.0   [ 9058 – 11020] STATISTICS PANEL
SECTION 9.1   [11021 – 11059] DAILY SUMMARY REPORTING
SECTION 10.0  [11060 – 11448] UI BUTTON EVENTS
SECTION 10.1  [11449 – 11499] TIMER EVENT
SECTION 11    [11500 – 11673] THE SHUTDOWN ROUTINE (STATEFUL)
```

---

## 2. WHERE THE FOUR CHANGES LAND

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

The four changes are additive and bounded. They do not touch the calculation
memory chain, the threshold oracle, the KAMA persistence, or the export schema.

---

## 4. KEY FUNCTIONS (quick reference)

```
OnInit                     @195     seeds buffers, KAMA warm-start, init tables
ExportDataForAnalysis      @564     CSV export (parity path) — SACRED
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
```
