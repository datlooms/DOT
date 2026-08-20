# MANAGER_SYSTEM_SPEC.md — "PRICED-100"
**Field: catalogue_F0.csv, 1,840 VALID F0 rows. Floor 3/3. Cap 21. FLOORED admission.
Frozen Whole-DOT gate stack. Every figure 1.0 lot. Frame 177,251 x 172,
2026.01.19 15:49 -> 2026.07.21 17:09. Oracle sha256[:12] 518862bf19fb.**

---

# 0. WHAT WOULD MAKE THIS SPECIFICATION WRONG

**Ranked by how much it would cost, with the falsifying check.**

**0.1 THE SELECTION IS NOT OUT OF SAMPLE. THIS IS THE LARGEST WEAKNESS AND IT IS NOT FIXABLE
FROM THIS DOCUMENT.** `pf_null_exceedance_pct` was computed on the whole frame by the pipeline
that produced the catalogue. Every member was therefore chosen with knowledge of Jun-Jul.
Section 6's segment scores are NOT a walk-forward and must never be read as one.
**FALSIFYING CHECK: re-price the null on Jan-May only, re-select 50/50, score Jun-Jul. If
loss events on the held-out segment exceed ~8 or the worst day exceeds -$900, the pricing
does not generalise and this system is an artifact.** NOT RUN — the 4,652-draw null cannot be
rebuilt from the catalogue, only re-derived from the scan.

**0.2 THE WORST DAY IS ONE BAR AND IT IS LOAD-BEARING.** worst bar -$644.5, worst day -$612.0.
The tail is a single simultaneous multi-signal stop-out, not an accumulation.
**FALSIFYING CHECK: remove the signals live on that bar and re-score. If the tail moves to a
different bar of similar size the system is tail-robust; if it collapses to -$300 the
reported tail is one removable event and the honest figure is the second-worst bar.** NOT RUN.

**0.3 THE 14 LOSS EVENTS SIT ON 13 DAYS. A RATIO ON 14 EVENTS IS A COUNT, NOT A RATE.**
Every per-event figure here — loss-event count, event-days, losing-bar rate 4.26% — rests on
fourteen observations. **FALSIFYING CHECK: the split-half in 6.2 gives 7 and 7. That is the
only stability evidence and it is weak by construction.**

**0.4 SHORT d4 AND d5+ RUN UNGATED AND CARRY 36 OF THE 58 LOSSES.** 859 short trades at tiers
4 and 5+ pass through no gate at all — inherited from the Whole DOT, where those cells were
measured on 4 and 2 events. Here they carry 24 and 12 losses.
**FALSIFYING CHECK: derive a short-side gate on these 859 trades and test it against a
rarity-matched null at the p=0.05 bar. If nothing clears, the cells are genuinely free; if
something clears, this system leaves the largest available improvement unclaimed.** NOT RUN.

**0.5 THE GATE STACK IS INHERITED, NOT DERIVED FOR THIS BOOK.** It was calibrated on a
297-signal book. This is 100 signals, and the record states the free-tier crossover moves
with book size. **FALSIFYING CHECK: sweep the tier boundary on this membership. If the
crossover is not at 3, the stack is fitted to a different system.** NOT RUN. Freezing is
still correct — re-deriving per candidate book reopens the unpriced-search problem — but the
mismatch is real and is stated rather than hidden.

**0.6 `E` IS A RANKING KEY HERE, NOT A HYPOTHESIS TEST.** `E_dir = n_dir x pf_null_exceedance_pct`
is the expected number of same-direction rows at or above this PF under 4,652 rarity-matched
nulls. **It is not corrected for the 1,840 trials that produced the catalogue**, and the
selection takes the 50 lowest per direction rather than applying a threshold.
**FALSIFYING CHECK: apply Benjamini-Yekutieli at q<0.10 to the same column and compare
membership. If the two sets diverge sharply the ranking is exploiting resolution noise near
the floor.** NOT RUN.

**0.7 THE PRICING FLOORS DIFFER 3.9x BETWEEN DIRECTIONS.** LONG 1,463/4,652 = 0.3145,
SHORT 377/4,652 = 0.0810. A short row can resolve below a long row purely because its
denominator is smaller. **FALSIFYING CHECK: re-rank with both directions on a common floor
and compare membership.** NOT RUN.

**0.8 A DEFECT ALREADY FOUND AND CORRECTED IN THIS SESSION.** An earlier version of objective
B ranked on rank-share instead of `pf_null_exceedance_pct`. It produced 25 loss events and a
-$765 worst day at 120 signals. The recorded statistic produces 14 and -$612 at 100.
**A ranking key that looks equivalent is not equivalent, and the artifact is the only way to
tell.**

**0.9 AND A DEFECT IN MY OWN REPORTING, CORRECTED.** I stated that 111 of the Whole DOT's 297
"do not exist" after checking only `catalogue_F0.csv`. 283 of 297 are present in
`results_F0_triple_convergence_and_d2ddir.csv`, in the same directory. Only 14 are genuinely
absent. **A negative claim carries the same burden as a positive one, and I did not test it.**

---

# 1. HEADLINE — 1.0 LOT

    signals                 100      50 LONG / 50 SHORT
    trades                1,648
    entry bars              329
    days traded              95      of 132
    win rate              96.48%
    profit factor         13.30
    LOSS EVENTS              14      distinct losing entry bars
    event-days               13
    losing-bar rate        4.26%     14 of 329 bars
    WORST BAR           -$644.5
    WORST DAY           -$612.0
    losing days               5      of 95
    LOSING WEEKS              0      of 25
    average win          $41.76
    average loss         $86.08
    break-even WR         67.33%
    MARGIN                29.15 pp
    net                 $61,406

---

# 2. THE RULE, IN EXECUTION ORDER

**STEP 1 — FIELD.** `catalogue_F0.csv`, 1,840 rows, verdict-VALID F0 triples only.
1,463 LONG / 377 SHORT. **Do not slice it further and do not substitute the raw scan** —
the raw scan is measured in 8.3 and objective A's tail is 9x worse there.

**STEP 2 — PER-SIGNAL SOLO DAILY P&L.** Not used by this system's selection, but required to
reproduce objective A for comparison. Parameters in 3.2.

**STEP 3 — SELECTION.** Per direction, independently:

    E_dir(s) = n_dir * pf_null_exceedance_pct(s)
    n_dir    = 1463 LONG, 377 SHORT
    sort ascending by E_dir; tie-break DESCENDING agg_pf
    take the first 50

`pf_null_exceedance_pct` is read from the catalogue, computed against K=4,652 rarity-matched
nulls at `null_direction_long_share = 0.7951`. **NaN is coerced to 1.0 (worst), never dropped.**
No RNG. No iteration. Deterministic given the catalogue.

**STEP 4 — UNION.** LONG 50 + SHORT 50 = 100. No overlap by construction.

**STEP 5 — BAR ELIGIBILITY.** Applied by the engine, not by this system:
`bar >= 6900` and `ADX_Value >= 15` and `Volume > 50` and `Volume != 0` and
NOT (Friday and (EST_Hour > 16 or (EST_Hour == 16 and EST_Minute >= 45))).

**STEP 6 — DEPTH FLOOR.** FLOORED admission, `ADM_FLOOR = {1: 3, -1: 3}`. A bar admits only
if at least 3 signals of that direction qualify simultaneously. **FLOORED, not CURRENT — the
depth floor is an admission rule, not a post-hoc filter.**

**STEP 7 — GLOBAL GATE.** `ATR_1M >= 20` at the entry bar. Raw ATR, not a percentile.

**STEP 8 — PER-TIER GATE STACK.** `tier = min(depth, 5)`:

    tier   LONG                                      SHORT
    3      Micro_Hurst > p90                         Micro_Hurst > p90
    4      Micro_FailedBreak > p20 AND AT_Slope_ST > p90   FREE
    5+     Micro_FailedBreak > p20                   FREE

Masks from `swept_thresholds.build_whole_dot_gates(df)`.
**Verified pass rates: HU90 9.7478%, FB20 80.1874%, ATS90 6.2217%** — identical to the
Whole DOT's recorded checksums, which is what verifies the scorer.

**STEP 9 — JAR.** `MAX_POSITIONS = 21` live lots. A break-even'd trade leaves the jar.

**STEP 10 — TRADE MANAGEMENT.** Unmodified `adm_engine` defaults. See 3.1.

---

# 3. THE CONSTANT REGISTRY

## 3.1 ADMISSION, STRUCTURE, TRADE MANAGEMENT

    constant              value      source
    ADMISSION_RULE      FLOORED      adopted, Whole DOT
    ADM_FLOOR          {1:3,-1:3}    adopted, Whole DOT
    MAX_POSITIONS           21       adopted, Whole DOT
    atr_min               20.0       adopted, Whole DOT. Raw, not percentile
    ELIG_ADX              15.0       engine, sacred
    ELIG_VOL              50.0       engine, sacred
    SPREAD                 3.0       engine, sacred
    RISK_MULT              2.0       engine, sacred
    MOMENTUM_SL_MULT       4.0       engine, sacred
    MOMENTUM_THRESHOLD  0.00012      engine, sacred
    MAX_RISK             150.0       engine, sacred
    STEP_PCT              0.30       engine, sacred
    BE_TRIG_FRAC / LOCK_FRAC 1.0     engine, sacred
    LAG_BASE / LAG_MOMENTUM  2 / 3   engine, sacred
    tier = min(depth,5)      -       INHERITED AND UNDOCUMENTED. Not derived for this book.
    SOLO_TICK_GATE      unreachable  n_qual==1 cannot occur at floor 3. DEAD CODE.

## 3.2 THE SELECTION CONSTANTS

    K_NULL                4652       catalogue, null draw count
    long_share          0.7951       catalogue. THE NULL IS 80% LONG; shorts are priced
                                     against a long-dominated null. See 0.7.
    floor LONG          0.3145       1463/4652
    floor SHORT         0.0810       377/4652
    target per direction    50       THE ONLY FREE PARAMETER. Chosen in 7.1.
    tie-break        agg_pf DESC     catalogue

## 3.3 THE LOSS-DAY MATRIX PARAMETERS — FOR THE OPEN RECONCILIATION

**Not used by this system, but stated because the outstanding A-implementation conflict most
likely lives here.** My solo daily P&L matrix was built with:

    engine              portfolio_simulation_engine.run_portfolio   NOT adm_engine
    admission           engine default (no depth floor)
    MAX_POSITIONS       6            engine default, UNMODIFIED
    ATR gate            NONE
    tier gates          NONE
    conviction          None
    batch size          1            each signal scored ALONE, so the jar never binds
    day key             exit_time[:10]
    pool filter         net > 0 over the frame, per direction
    seed                argmax(net - 50 * losing_days)
    coverage            cumulative boolean, ((loss==1) & (covered>0)).sum()

**Scoring the SELECTED book then uses adm_engine at floor 3/3, cap 21, with the gate stack.
Selection-time and scoring-time regimes therefore DIFFER, and that is a defect I am naming
rather than defending: the record's own instruction is to select and run at the same cap.**

---

# 4. THE FULL SIGNAL LISTING

100 rows. 50 LONG / 50 SHORT. All F0 triples, three conditions plus the mandatory
`D2D_Trend_Dir == direction`. **Longest signal_def 72 characters; emitted at 101 characters.
Any copy that wraps is corrupt and must not be built from.** `E` is `E_dir`; source is B.

```
--- LONG (50 signals) ---------------------------------------
   1  AT_Score_LT:lo + Slope_Accel_LT:hi + Body_Size:hi                         E=0.0000  PF=13.29  B
   2  AT_Score_LT:lo + Slope_Accel_LT:hi + Micro_RangeAccel:hi                  E=0.0000  PF=32.16  B
   3  AT_Score_LT:lo + Slope_Accel_LT:hi + OR_Low_Side:==-1                     E=0.0000  PF=0.00  B
   4  AT_Score_LT:lo + Slope_Accel_LT:hi + VWAP_Sigma:hi                        E=0.0258  PF=11.10  B
   5  AT_Slope_LT:lo + AT_Slope_ST:lo + Micro_RollProxy:hi                      E=0.0258  PF=11.68  B
   6  AT_Slope_LT:lo + Micro_FailedBreak:hi + Micro_RangeAccel:lo               E=0.0258  PF=11.18  B
   7  AT_Slope_ST:lo + Micro_BarOverlap:lo + VWAP_Sigma:hi                      E=0.0000  PF=25.55  B
   8  AT_Slope_ST:lo + Micro_Hurst:hi + Round_1000_Dist_ATR:lo                  E=0.0258  PF=12.78  B
   9  AT_Slope_ST:lo + Micro_OrderFlowDelta:hi + Micro_WickImbalance:lo         E=0.0000  PF=37.59  B
  10  AT_Slope_ST:lo + Micro_RangeAccel:hi + RangeOsc_State:==2                 E=0.0000  PF=15.05  B
  11  AT_Slope_ST:lo + Micro_VPIN:lo + OBVf_Signal:==1                          E=0.0258  PF=11.28  B
  12  AT_Slope_ST:lo + Micro_VolOfVol:hi + OBVf_Signal:==1                      E=0.0000  PF=14.93  B
  13  AT_Slope_ST:lo + Momentum_Value:hi + Body_Size:hi                         E=0.0258  PF=11.49  B
  14  AT_Slope_ST:lo + Momentum_Value:hi + Micro_RollProxy:hi                   E=0.0258  PF=12.03  B
  15  Bars_Since_Flip:hi + Micro_CSSpread:hi + Upper_Wick:hi                    E=0.0258  PF=12.92  B
  16  Bars_Since_Flip:hi + PrevDay_Low_Dist_ATR:lo + OR_Low_Side:==-1           E=0.0000  PF=14.34  B
  17  Bars_Since_Flip:hi + Volume_Avg_10:hi + MultiDay_Position:lo              E=0.0000  PF=14.93  B
  18  Bars_Since_Flip:hi + Volume_Avg_10:hi + Session_Low_Dist_ATR:lo           E=0.0000  PF=31.08  B
  19  D2D_ATR_MA:hi + Micro_VPIN:lo + ST_Flip_Event:==-1                        E=0.0000  PF=15.33  B
  20  KAMA_Dist:lo + Micro_WickImbalance:hi + ADX_Rising:==0                    E=0.0000  PF=19.26  B
  21  KAMA_Dist:lo + Micro_WickImbalance:hi + Lower_Wick:hi                     E=0.0000  PF=18.19  B
  22  KAMA_Dist:lo + Micro_WickImbalance:hi + OR_High_Side:==-1                 E=0.0000  PF=0.00  B
  23  KAMA_Dist:lo + VWAP_Dist_ATR:lo + DailyOpen_Dist_ATR:lo                   E=0.0258  PF=11.82  B
  24  Micro_Amihud:hi + AT_Regime_ST:==1 + OR_Low_Side:==-1                     E=0.0000  PF=14.74  B
  25  Micro_Amihud:lo + Micro_RangeAccel:lo + Micro_VPIN:hi                     E=0.0000  PF=15.24  B
  26  Micro_AutoCorr:hi + Session_Low_Dist_ATR:lo + TChan_A15:hi                E=0.0000  PF=17.85  B
  27  Micro_LogReturn:hi + Micro_TickIntensity:hi + Micro_VolOfVol:hi           E=0.0000  PF=15.92  B
  28  Micro_MicroGap:lo + AT_Regime_ST:==1 + OR_Low_Side:==-1                   E=0.0000  PF=16.87  B
  29  Micro_MomoTransfer:hi + Micro_RangeVelocity:lo + AT_Regime_LT:==1         E=0.0258  PF=11.02  B
  30  Micro_PriceAccel:hi + VWAP_Sigma:hi + D2D_Signal:==1                      E=0.0258  PF=12.19  B
  31  Micro_Rejection:lo + AT_Regime_ST:==1 + PrevDay_Low_Side:==-1             E=0.0000  PF=20.73  B
  32  Micro_VPIN:lo + Volume:hi + ST_Flip_Event:==-1                            E=0.0000  PF=15.44  B
  33  Micro_WickImbalance:hi + Lower_Wick:lo + AT_Regime_LT:==1                 E=0.0000  PF=21.53  B
  34  Micro_WickImbalance:hi + Lower_Wick:lo + VWAP_Side:==-1                   E=0.0000  PF=0.00  B
  35  Momentum_Value:hi + Micro_Hurst:hi + AT_Regime_ST:==1                     E=0.0000  PF=48.24  B
  36  OBV_Macd:lo + Harmonic_LLEMA:hi + Micro_Rejection:lo                      E=0.0000  PF=30.40  B
  37  OBV_Macd:lo + Micro_WickImbalance:lo + ADX_Value:hi                       E=0.0000  PF=13.35  B
  38  OBV_Velocity:hi + Micro_FractalDim:hi + OR_Low_Side:==-1                  E=0.0258  PF=11.03  B
  39  RangeOsc_Val:hi + Session_Low_Dist_ATR:lo + PrevDay_Close_Side:==-1       E=0.0258  PF=11.73  B
  40  Slope_Accel_LT:hi + AT_Regime_ST:==1 + OR_Low_Side:==-1                   E=0.0000  PF=13.37  B
  41  Slope_Accel_LT:hi + KAMA_Dist_ATR:hi + RangeOsc_State:==2                 E=0.0000  PF=0.00  B
  42  Slope_Accel_LT:hi + OBV_Macd:lo + Micro_TickIntensity:hi                  E=0.0000  PF=13.93  B
  43  Slope_Accel_LT:hi + Sqz_State:==1 + RangeOsc_State:==-2                   E=0.0000  PF=14.74  B
  44  Slope_Accel_ST:lo + Micro_VPIN:lo + ST_Flip_Event:==-1                    E=0.0000  PF=15.36  B
  45  Slope_EMA_ST:lo + Micro_Hurst:hi + PrevDay_High_Side:==-1                 E=0.0000  PF=15.10  B
  46  Slope_EMA_ST:lo + Micro_RangeAccel:hi + Micro_Rejection:lo                E=0.0000  PF=16.68  B
  47  Slope_EMA_ST:lo + Micro_VolOfVol:hi + AT_Regime_ST:==1                    E=0.0258  PF=11.17  B
  48  Sqz_Val:hi + Sqz_State:==1 + RangeOsc_State:==-2                          E=0.0258  PF=11.72  B
  49  Sqz_Val:lo + Micro_IBSP:hi + Micro_VolAccel:hi                            E=0.0000  PF=0.00  B
  50  Volume_Avg_10:hi + Micro_FailedBreak:hi + Micro_Rejection:lo              E=0.0258  PF=11.95  B
--- SHORT (50 signals) --------------------------------------
   1  AT_Slope_ST:hi + Micro_BarEntropy:lo + Sqz_State:==-1                     E=0.1033  PF=8.78  B
   2  AT_Slope_ST:hi + Micro_VPIN:lo + Sqz_State:==-1                           E=0.3874  PF=4.60  B
   3  Bar_Range:hi + D2D_Dynamic_Sensitivity:lo + Lower_Wick:lo                 E=0.3099  PF=4.94  B
   4  D2D_ATR:hi + D2D_Dynamic_Sensitivity:lo + Lower_Wick:lo                   E=0.3357  PF=4.89  B
   5  D2D_ATR:hi + Micro_ThrustEff:hi + PrevDay_Close_Dist_ATR:hi               E=0.3357  PF=4.87  B
   6  D2D_ATR_MA:hi + Sqz_Val:hi + DailyOpen_Dist_ATR:hi                        E=0.3099  PF=5.06  B
   7  D2D_Dynamic_Sensitivity:lo + Lower_Wick:lo + Volume:hi                    E=0.3099  PF=5.00  B
   8  D2D_Dynamic_Sensitivity:lo + Micro_GarmanKlass:hi + Lower_Wick:lo         E=0.3099  PF=4.95  B
   9  D2D_Dynamic_Sensitivity:lo + Micro_ThrustEff:lo + Micro_WickImbalance:lo  E=0.1808  PF=5.86  B
  10  D2D_Dynamic_Sensitivity:lo + Volume_Avg_10:hi + Efficiency_Ratio:hi       E=0.3874  PF=4.63  B
  11  DailyOpen_Dist_ATR:lo + Harmonic_OBVf_Concordance:==1 + VAH_Side:==0      E=0.2583  PF=5.20  B
  12  Micro_Amihud:hi + Micro_VolOfVol:lo + OR_Low_Side:==-1                    E=0.1808  PF=5.68  B
  13  Micro_BarEntropy:hi + ADX_Rising:==1 + VAH_Side:==0                       E=0.1550  PF=6.20  B
  14  Micro_BarEntropy:lo + Micro_Hurst:hi + Session_High_Dist_ATR:lo           E=0.0000  PF=21.28  B
  15  Micro_BarOverlap:hi + Micro_Entropy:lo + VWAP_Z:lo                        E=0.3615  PF=4.67  B
  16  Micro_BarOverlap:hi + Micro_FailedBreak:lo + Lower_Wick:hi                E=0.2841  PF=5.17  B
  17  Micro_BarOverlap:hi + Micro_HLAsymmetry:lo + PrevDay_High_Side:==1        E=0.1550  PF=6.13  B
  18  Micro_Entropy:lo + Micro_WickImbalance:lo + Lower_Wick:lo                 E=0.0000  PF=18.68  B
  19  Micro_FailedBreak:lo + Micro_FractalDim:lo + Micro_HLAsymmetry:hi         E=0.3357  PF=4.89  B
  20  Micro_LogReturn:hi + Micro_PriceAccel:lo + Micro_VolOfVol:lo              E=0.3615  PF=4.65  B
  21  Micro_MomoTransfer:hi + Micro_RangeAccel:lo + OBVf_Signal:==1             E=0.1033  PF=7.55  B
  22  Micro_PriceAccel:hi + Micro_RangeVelocity:hi + PoC_Side:==0               E=0.2583  PF=5.18  B
  23  Micro_RangeVelocity:lo + Lower_Wick:lo + AT_Regime_ST:==0                 E=0.0000  PF=23.74  B
  24  Micro_Rejection:hi + Body_Size:hi + PrevDay_High_Side:==1                 E=0.2325  PF=5.31  B
  25  Momentum_Value:hi + Micro_Lambda:lo + PrevDay_Low_Side:==-1               E=0.0000  PF=0.00  B
  26  Momentum_Value:hi + Micro_TickIntensity:hi + PrevDay_Low_Side:==-1        E=0.3357  PF=4.88  B
  27  Momentum_Value:hi + Micro_TickIntensity:hi + Session_High_Dist_ATR:hi     E=0.1033  PF=8.85  B
  28  Momentum_Value:hi + Session_High_Dist_ATR:hi + OR_Position:lo             E=0.1550  PF=6.36  B
  29  Momentum_Value:hi + Session_High_Dist_ATR:hi + TChan_A15:hi               E=0.1808  PF=5.71  B
  30  OBV_Velocity:hi + TChan_A15:hi + PrevDay_Close_Side:==1                   E=0.3357  PF=4.77  B
  31  OBV_Velocity:lo + Micro_BarEntropy:hi + PoC_Side:==0                      E=0.2841  PF=5.07  B
  32  OBVf_DirStepCount:hi + VAL_Side:==0 + WeeklyOpen_Side:==-1                E=0.1033  PF=7.99  B
  33  OBVf_DirStepCount:lo + Lower_Wick:lo + VWAP_Sigma:hi                      E=0.3874  PF=4.60  B
  34  Slope_Accel_LT:lo + Micro_AutoCorr:lo + Micro_FractalDim:lo               E=0.3874  PF=4.53  B
  35  Slope_Accel_LT:lo + Micro_OrderFlowDelta:hi + PoC_Side:==0                E=0.1550  PF=5.95  B
  36  Slope_Accel_LT:lo + Micro_Rejection:hi + PrevDay_High_Side:==1            E=0.3874  PF=4.61  B
  37  Slope_Accel_LT:lo + Sqz_Val:hi + Micro_CSSpread:lo                        E=0.0000  PF=0.00  B
  38  Slope_Accel_ST:hi + ADX_Value:lo + VWAP_Sigma:hi                          E=0.3615  PF=4.72  B
  39  Slope_Accel_ST:hi + MultiDay_Position:hi + RangeOsc_State:==2             E=0.2066  PF=5.59  B
  40  Slope_Accel_ST:hi + WeeklyOpen_Dist_ATR:hi + TChan_A15:hi                 E=0.0517  PF=10.03  B
  41  Slope_Accel_ST:lo + Volume_Avg_10:hi + VAL_Side:==-1                      E=0.2841  PF=5.13  B
  42  Slope_EMA_ST:lo + RangeOsc_State:==2 + PrevDay_High_Side:==1              E=0.3615  PF=4.66  B
  43  Slope_EMA_ST:lo + Slope_Accel_ST:hi + ADX_Value:lo                        E=0.2325  PF=5.50  B
  44  Sqz_Val:hi + Micro_OrderFlowDelta:hi + Session_High_Dist_ATR:hi           E=0.0000  PF=18.04  B
  45  Sqz_Val:hi + Micro_TickIntensity:hi + Session_High_Dist_ATR:hi            E=0.0000  PF=27.35  B
  46  Sqz_Val:hi + Momentum_Value:hi + Session_High_Dist_ATR:hi                 E=0.1033  PF=8.48  B
  47  Sqz_Val:hi + OR_Position:hi + RangeOsc_State:==2                          E=0.1550  PF=6.56  B
  48  Sqz_Val:hi + PrevDay_Close_Dist_ATR:hi + Volume:hi                        E=0.1550  PF=6.26  B
  49  Sqz_Val:hi + Volume_Avg_10:hi + PrevDay_Close_Dist_ATR:hi                 E=0.0000  PF=13.05  B
  50  VA_Position:hi + ADX_Value:lo + Sqz_State:==-1                            E=0.1550  PF=6.24  B
```

---

# 5. WHICH GATES ARE LIVE AND WHICH ARE DEAD AT THIS FLOOR

    LONG   tier 3   HU90              114 trades   LIVE
    LONG   tier 4   FB20 + ATS90       44 trades   LIVE
    LONG   tier 5+  FB20              577 trades   LIVE
    SHORT  tier 3   HU90               54 trades   LIVE
    SHORT  tier 4   FREE              316 trades   ungated
    SHORT  tier 5+  FREE              543 trades   ungated
    tiers 1 and 2   unreachable under floor 3      DEAD CODE — must not be carried
    SOLO_TICK_GATE  n_qual==1 impossible           DEAD CODE

**Every live gate fires. Unlike the Whole DOT at L7/S4, no gate in this configuration is
inert.** 859 of 1,648 trades — 52% — pass through no tier gate at all, all of them short.

---

# 6. PERFORMANCE, OUT-OF-SAMPLE, RISK POSTURE

## 6.1 DEPTH LADDER, PER DIRECTION

    dir     tier   n      WR       PF      net       losses
    LONG    d3     114   94.74    14.85    $4,214       6
    LONG    d4      44   90.91     5.23    $2,518       4
    LONG    d5+    577   98.96    65.57   $26,034       6
    SHORT   d3      54   88.89     5.37    $1,423       6
    SHORT   d4     316   92.41     4.04    $7,310      24
    SHORT   d5+    543   97.79    21.66   $19,907      12

**Monotone on both sides from d4 to d5+. LONG d4 is the weakest long cell at PF 5.23 despite
carrying the heaviest gate. SHORT d4 carries 24 of 58 losses ungated.**

## 6.2 SEGMENTS — NOT A WALK-FORWARD, SEE 0.1

    window              tr     ev   days   WR      PF      worst day   lwk
    FULL Jan19-Jul21  1,648    14    95   96.48   13.30     -$612.0    0/25
    Jan-May           1,253    10    69   96.81   15.29     -$509.6    0/18
    Jun-Jul             395     4    26   95.44    9.79     -$612.0    0/7
    July                138     2    10   94.20   10.34     -$612.0    0/3
    H1 Jan19-Apr20    1,011     7    48   97.13   16.71     -$319.2    0/12
    H2 Apr21-Jul21      637     7    47   95.45   10.45     -$612.0    0/13

**Split-half gives 7 and 7 loss events on 48 and 47 days. Zero losing weeks in every window.
PF falls 16.71 -> 10.45 across the halves and the tail deepens -$319 -> -$612 — the second
half is materially harder and the system degrades into it, gracefully.**

## 6.3 MONTHLY

    month     n     WR       PF       net      worst day
    2026.01    60  100.00   999.00   $2,205      +$361.6
    2026.02   369   96.21    16.73  $11,688      -$319.2
    2026.03   456   97.81    20.61  $17,333       +$40.0
    2026.04   219   96.35     9.40   $6,709       +$54.4
    2026.05   149   94.63    10.99   $7,613      -$509.6
    2026.06   257   96.11     9.36   $8,486      -$171.8
    2026.07   138   94.20    10.34   $7,371      -$612.0

**Every month positive. Three of seven carry a positive worst day.**

## 6.4 RISK POSTURE

Worst day -$612.0 at 1.0 lot. At the operator's deployment size of 0.20-0.25 lots that is
**-$122 to -$153 against an FTMO $2,500 daily ceiling — 4.9% to 6.1% of it.**
At 1.0 lot it is 24.5%. **Headroom to the ceiling at 1.0 lot is 4.1x.**
Worst bar -$644.5 exceeds the worst day, so the tail is a single simultaneous event and the
jar's 21 live lots gap together. **Slippage is not modelled anywhere in this project.**

---

# 7. THE BATTERY

## 7.1 RANDOMISATION — 40 SIZE-MATCHED DRAWS, 50L/50S, FROM THE SAME VALID FIELD

    resolution floor      1/40 = 0.0250
    random median         ev 21   wd -$695   PF 7.20   days 88   lwk 2
    PRICED-100            ev 14   wd -$612   PF 13.30  days 95   lwk 0

    loss events    10.0% of draws better    PASSES the <=10% bar exactly at the floor
    worst day      20.0% of draws better    FAILS the <=10% bar
    losing weeks    0.0% of draws better    1 draw of 40 also reached zero

**The pass is marginal and I am not overstating it. 10.0% is 4 draws of 40, one resolution
step from failing. The tail does NOT clear the bar I set.** Gates were the frozen stack on
both sides — not derived per draw — which is a weaker control than the brief specifies, and
is the correct reading of why the tail percentile is unimpressive.

## 7.2 WHAT WAS RUN AND WHAT WAS NOT

    RUN         randomisation 40 draws · split-half · segment scores · July segment ·
                depth ladder · monthly · weekly · gate liveness · alone-first gate on all
                four objectives · fusion A+B, A+D, A+B+D · full-field replication
    NOT RUN     true walk-forward with train-only pricing (0.1) · anti-system negative
                control · tail-bar removal (0.2) · short-side gate derivation (0.4) ·
                tier-boundary sweep (0.5) · BY q<0.10 comparison (0.6) · common-floor
                re-rank (0.7) · slippage · Bonferroni/Romano-Wolf on the gate search

**Every NOT RUN item is a named gap, not an omission.**

## 7.3 TRIAL COUNT — THE HONEST NUMBER

    scored configurations this session                          ~118
      four objectives x 3 sizes, VALID field                      12
      alone-first gate, 15 draws                                  15
      unions A/B/A+B/A+B+C/A+B+C+D x 3 sizes                      15
      objective A frontier K=20/30/45/60                           4
      losing-week re-runs                                          4
      four objectives x 3 sizes, RAW field                        12
      fusion with reconciled A and recorded B, 6 cells x 3         18
      target sweep                                                 7
      final sweep with lwk                                        14
      battery on the pick                                          9
      randomisation                                               40  (controls, not trials)
    per-signal solo scoring runs                              21,594  (1,840 + 19,754)

**~78 genuine configuration trials excluding controls and per-signal scoring. One free
parameter was swept (target size) across 7 values on 2 objectives and 3 unions.
No multiple-testing correction is applied to that search and none is claimed.**

---

# 8. WHAT THIS SYSTEM CANNOT SEE

**8.1 THE 14 ORPHANS.** Fourteen Whole DOT members — 6 LONG, 8 SHORT — were never emitted by
the F0 scanner because `MIN_TRADES = 10`. They exist only in BOOK-50's lineage.
**No scan-based procedure can reach them and this one does not.**

**8.2 EVERY FAMILY EXCEPT F0.** F1's 37,276 rows, F3, F9, F11, F2, F4 — none searched.
The record shows F3 and F9 outperforming F0 on convergence participation per row.

**8.3 THE RAW SCAN'S 17,914 NON-VALID ROWS.** Deliberate — 8.3 measures the cost of using
them — but they are unseen.

**8.4 THE OBJECTIVES THAT FAILED, AND WHY.** Measured this session, VALID field, 120 signals,
against 15 size-matched draws:

    A  loss-day decorrelation   23 ev  -$444    passes both leading axes
    B  chance-pricing           25 ev  -$765    passes events, fails tail  (rank-share version)
    C  co-fire affinity        119 ev  -$4,524  100% of draws better. REFUTED
    D  terrain coverage         98 ev  -$2,911  100% of draws better. REFUTED

**D was my own proposal, argued on submodularity and the (1-1/e) bound. The bound holds; the
objective it guarantees is the wrong one. Coverage buys reach by admitting signals that fire
everywhere.**

**8.5 THE FUSION DOES NOT REPRODUCE ON THIS FIELD.** Union bars run +376 to +2,063 over
sum-of-parts — the recorded effect is real — **but loss events do not fuse.** A 15 + B 14 = 29;
the union gives 31, and the union tail is worse than either parent at every cell tested.
**A+B buys days by paying tail. That is a trade, not a free lunch.**

**8.6 THE RAW-FIELD REPLICATION.** Objective A on all 19,754: worst day -$341 -> -$3,213,
losing weeks 1 -> 6. **Loss-day decorrelation minimises the NUMBER of losing days, not their
SIZE.** The VALID screen was doing work the objective silently depended on. This confirms the
record's constraint in the opposite direction from the one proposed.

**8.7 THRESHOLDS NEVER RUN ON THIS FRAME FOR THIS BOOK.** The tier boundary, any short-side
gate, any alternative to `min(depth,5)`, and every gate percentile other than the inherited
p90/p20/p90.

---

# 9. THE FRONTIER BENEATH THE PICK

    config        sig   ev   worst_day   PF      days   lwk    net
    B  K=30        60    6     -$273.6   14.58    51    2/24   $19,062
    A  K=30        60    3     -$377.6   26.46    58    1/24   $32,018
    A  K=40        80   10     -$341.2   16.80    77    1/25   $46,585
    B  K=40        80   14     -$612.0   10.39    84    1/25   $41,776
    A  K=45        90   15     -$341.2   12.75    79    1/25   $49,827
    B  K=45        90   14     -$612.0   12.30    90    0/25   $51,557
    B  K=50 PICK  100   14     -$612.0   13.30    95    0/25   $61,406
    A  K=60       120   23     -$444.0   10.99    94    1/26   $78,128
    B  K=60       120   27     -$918.0    8.94   100    0/26   $77,942
    A+B K=50      159   37     -$765.0   10.24   109    0/26  $119,803
    B  K=80       160   43     -$808.3    8.43   113    0/26  $119,881

**WHY K=50.** Of every cell reaching ZERO losing weeks, B K=50 has the fewest loss events (14)
and the shallowest tail (-$612). It dominates B K=45 outright — same events, same tail, five
more days and a higher PF. **A K=30 is the tail-and-events champion at 3 events and -$377 but
carries a losing week and only 58 days; A K=40 likewise.** Persistence is axis three and the
operator's standing requirement is zero losing weeks, so the A cells are excluded on that
axis despite leading on axes one and two.

**AND THE TARGET WAS NOT REACHED.** 115+ days with a tail inside -$600 and zero losing weeks
does not exist in anything I measured. The nearest are B K=80 at 113 days / -$808 and
A+B K=55 at 112 days / -$918. **Both hit zero losing weeks and land 2-3 days short with tails
35% outside the bar.** The binding constraint is B's own worst bar: A+B's -$918 at K=55 and
K=60 IS B's worst day, so one bar sets the floor for every book containing it. **0.2 is the
measurement that would break it and it is not run.**
