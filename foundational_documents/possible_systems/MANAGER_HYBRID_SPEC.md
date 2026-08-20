# MANAGER_HYBRID_SPEC.md — "REJ-197"
**A+B union at K=65, floor L8/S5, short d4/d5+ gated on a pre-registered mechanism.
Field: catalogue_F0.csv VALID, 1,840 rows. Cap 21. FLOORED admission.
Frame 177,251 x 172, 2026.01.19-2026.07.21. Oracle 518862bf19fb.
EVERY FIGURE AT 1.0 LOT. There is no max-lot column in this document.**

---

# 0. WHAT WOULD MAKE THIS SPECIFICATION WRONG

**0.1 IT DOES NOT BEAT MY OWN FIRST SYSTEM OUTRIGHT, AND I AM LEADING WITH THAT.**

    axis            PRICED-100      REJ-197        verdict
    loss events         14             10          HYBRID WINS
    worst day        -$612.0        -$637.0        first system wins
    losing weeks       0/25            0/25        tie
    losing months       0/7             0/7        tie
    days traded          95              88        FIRST SYSTEM WINS
    PF                13.30           22.53        hybrid
    margin            29.15           32.74        hybrid
    net             $61,406        $105,611        hybrid

**It wins axis one, loses axis two and axis four.** Under the stated ordering — events, tail,
persistence, days, net — that is a win on the leading axis and a loss on the second. **I am
not claiming a clean improvement and the honest reading is that the two systems sit on
different points of one frontier, not that this one supersedes the other.**

**0.2 THE SELECTION IS STILL NOT OUT OF SAMPLE. UNCHANGED FROM MY FIRST DOCUMENT.**
`pf_null_exceedance_pct` was computed on the whole frame, so objective B chose every member
knowing Jun-Jul. Objective A's loss-day matrix is likewise whole-frame. **Section 6's segment
scores are SEGMENT SCORES, NOT A WALK-FORWARD.** The Jun-Jul figures — 3 loss events, worst
day +$65.5 — are the most attractive numbers in this document and they are the least
trustworthy for exactly this reason.
**FALSIFIER: re-price the null on Jan-May only, rebuild A's matrix on Jan-May only, re-select,
score Jun-Jul. If held-out events exceed ~8 the procedure does not generalise.** NOT RUN.

**0.3 THE FLOOR AND THE SIZE WERE SWEPT TOGETHER AND NEITHER IS CORRECTED FOR.**
12 cells for the hybrid sweep alone (4 sizes x 3 floors), inside a cumulative ~170 scored
configurations across two documents. **No multiple-testing correction is applied to that
search and none is claimed.** A cell selected as the best of twelve on a frame this size is
a candidate, not a finding.
**FALSIFIER: Bonferroni at 12 cells requires p < 0.0042. I did not run a 500-draw control on
the adopted cell, so I cannot state whether it clears.** NOT RUN, and this is the single
biggest omission relative to the Quant's document, which did run it.

**0.4 THE SHORT GATE WAS CHOSEN FROM SIX CANDIDATES — BUT THE WINNER WAS PRE-REGISTERED.**
Six masks were tested on SHORT d4/d5+. `Micro_Rejection < p50` won on days-with-zero-losing-
weeks. **It is the only one of the six that was proposed and priced BEFORE this session** —
the record has it at the 98.6th random-subset percentile from a 360-test D2D sweep, and the
Supervisor independently re-confirmed it on this catalogue's short depth-3+ population
(PF 3.60 -> 5.00 at lo p50). **Had a fresh candidate won, the trial count would be six and
the result would be a fitted gate. It did not.**
**BUT `Micro_FailedBreak > p70` scored better on margin (42.76 vs 34.74) and I did not take
it, precisely because it is not pre-registered. That is a choice, and a reader may disagree.**
**FALSIFIER: a rarity-matched null on the 859 short d4/d5+ trades at p=0.05. If REJ does not
clear it, the gate is decoration.** NOT RUN.

**0.5 TEN LOSS EVENTS. A RATIO ON TEN OBSERVATIONS IS A COUNT.**
Every per-event figure here rests on ten. PF 22.53, margin 32.74, losing-bar rate — all of
them. The split-half gives 4 and 6. **That is the only stability evidence and it is thin by
construction.** The July holdout has ONE loss event; PF 46.20 on that window is declined and
must not be quoted.

**0.6 THE TAIL DID NOT MOVE AND THE FLOOR CANNOT MOVE IT.**
Across the entire floor sweep — L3/S3 through L7/S4 on three books — the worst day stayed
pinned at -$612, -$765 or -$850 depending on the book. **Raising the floor removes trades but
not the tail bar.** The tail is one simultaneous multi-signal stop-out and the jar's 21 live
lots gap together on it.
**FALSIFIER: remove the signals live on that bar and re-score. If the tail relocates to a
similar-sized bar the system is tail-robust; if it collapses, the reported tail is one
removable event.** NOT RUN — named in my first document and still not run.

**0.7 OBJECTIVE A ENTERS THIS UNION AND ITS MATRIX WAS BUILT AT THE WRONG CAP.**
See 3.3. The loss-day matrix was built on `portfolio_simulation_engine` at cap 6 with no
floor and no gates; the book is scored on `adm_engine` at L8/S5, cap 21, with the full stack.
**Selection-time and scoring-time regimes differ, which the record's own instruction forbids.**
The Quant built its matrix at cap 21 floor 1/1. **THAT IS THE WHOLE A-RECONCILIATION AND THE
QUANT'S PARAMETERS ARE THE CORRECT ONES.** I am adopting cap 21 as the standard and flagging
that the A members in this book were selected under cap 6.
**FALSIFIER: rebuild A's matrix at cap 21 floor 1/1 and re-run. If membership shifts
materially, every A-sourced row here is provisional.** NOT RUN — 1,840 solo re-scores, ~5 min,
and it is the first thing to run next.

**0.8 A DEFECT FOUND AND CORRECTED SINCE MY FIRST DOCUMENT.** None new. The two corrected
there stand: the rank-share ranking key, and my false claim that 111 Whole DOT members did
not exist.

---

# 1. HEADLINE — 1.0 LOT

    signals                 197      101 LONG / 96 SHORT
    trades                2,263
    entry bars              426
    days traded              88      of 132
    win rate              97.66%
    profit factor         22.53
    LOSS EVENTS              10      distinct losing entry bars
    losing-bar rate        2.35%
    WORST BAR           -$773.4
    WORST DAY           -$637.0
    LOSING WEEKS              0      of 25
    LOSING MONTHS             0      of 7
    break-even WR         64.92%
    MARGIN                32.74 pp
    net                $105,611

---

# 2. THE RULE, IN EXECUTION ORDER

**STEP 1 — FIELD.** `catalogue_F0.csv`, 1,840 VALID F0 rows. 1,463 LONG / 377 SHORT.
**Do not substitute the raw scan** — 8.2 measures the cost.

**STEP 2 — SOLO DAILY P&L MATRIX.** Every field row scored ALONE. Parameters in 3.3.
Days keyed on EXIT date. Required by objective A only.

**STEP 3 — OBJECTIVE A, per direction, K=65.** Greedy loss-day decorrelation.

    pool   {c : net(c) > 0}
    seed   argmax(net - 50 * losing_days)
    loop   overlap = ((loss[c]==1) & (covered>0)).sum()      CUMULATIVE boolean
           score   = (overlap, -net[c])                       admit argmin
           covered = covered + loss[best]
    stop   len(chosen) == 65

**STEP 4 — OBJECTIVE B, per direction, K=65.**

    E_dir(s) = n_dir * pf_null_exceedance_pct(s)
    sort ascending E_dir, tie-break DESCENDING agg_pf, take 65
    NaN exceedance coerced to 1.0 (worst), never dropped

**STEP 5 — UNION.** A u B = 197 signals. 67 from A alone, 67 from B alone, 63 in both.
**63 of 130 overlap — the two objectives agree on 48% and disagree on 52%.**

**STEP 6 — BAR ELIGIBILITY.** Engine-level: `bar>=6900`, `ADX>=15`, `Volume>50`,
`Volume!=0`, NOT Friday-close.

**STEP 7 — DEPTH FLOOR. `ADM_FLOOR = {1: 8, -1: 5}`.** FLOORED admission: a bar admits only
if >=8 LONG or >=5 SHORT signals qualify simultaneously. **This is the single largest lever
in the document and it had never been applied to a book selected this way.**

**STEP 8 — GLOBAL GATE.** `ATR_1M >= 20`, raw.

**STEP 9 — PER-TIER GATE STACK.** `tier = min(depth, 5)`:

    tier   LONG                                      SHORT
    3      Micro_Hurst > p90                         Micro_Hurst > p90
    4      Micro_FailedBreak > p20 AND AT_Slope_ST>p90    Micro_Rejection < p50   <- NEW
    5+     Micro_FailedBreak > p20                   Micro_Rejection < p50   <- NEW

**The short d4 and d5+ cells were FREE in every prior system. This closes the largest
ungated surface in any of the four documents.**

**STEP 10 — JAR.** `MAX_POSITIONS = 21` live lots; a break-even'd trade leaves the jar.

---

# 3. THE CONSTANT REGISTRY

## 3.1 ADMISSION AND GATES

    constant             value        source
    ADMISSION_RULE      FLOORED       adopted, Whole DOT
    ADM_FLOOR         {1:8,-1:5}      DERIVED HERE, swept over 3 values x 4 sizes
    MAX_POSITIONS          21         adopted, Whole DOT
    atr_min              20.0         adopted, Whole DOT. Raw, not percentile
    HU90 / FB20 / ATS90     -         adopted. Pass rates 9.7478 / 80.1874 / 6.2217%
                                      VERIFIED identical to the Whole DOT's checksums
    Micro_Rejection p50     -         PRE-REGISTERED. Record: 98.6th random-subset pct,
                                      360-test D2D sweep; re-confirmed on this catalogue
    tier = min(depth,5)     -         INHERITED AND UNDOCUMENTED. Not derived for this book.
    SOLO_TICK_GATE     unreachable    n_qual==1 impossible at floor 8/5. DEAD CODE.

## 3.2 SELECTION

    K per direction         65        SWEPT over 50/65/80/100
    K_NULL                4652        catalogue
    long_share          0.7951        catalogue. THE NULL IS 80% LONG.
    floor LONG          0.3145        1463/4652
    floor SHORT         0.0810        377/4652
    seed penalty            50        recorded constant, not swept
    day key           exit_time[:10]  recorded

## 3.3 THE LOSS-DAY MATRIX — THE OPEN RECONCILIATION, RESOLVED AGAINST ME

    parameter        MINE                     QUANT'S            correct
    engine           portfolio_sim_engine     adm_engine         adm_engine
    MAX_POSITIONS    6                        21                 21
    ADM_FLOOR        n/a (engine default)     {1:1,-1:1}         {1:1,-1:1}
    ATR gate         none                     none               none
    tier gates       none                     none               none
    batch size       1 (solo)                 1 (solo)           1
    day key          exit                     exit               exit

**The caps differ and mine is wrong.** The record's instruction is to select and run at the
same cap; this book is scored at 21. **The Quant's parameters are adopted as the standard.**
Note that with a single signal `signal_in_trade` permits only one open position, so
`live_lots <= 1` and neither cap binds — **which predicts the two matrices are IDENTICAL and
that the A-divergence lies elsewhere, most likely in the engine fork rather than the cap.**
**That prediction is testable in five minutes and is the first item in 7.2's NOT RUN list.**

---

# 4. THE FULL SIGNAL LISTING

197 rows. 101 LONG / 96 SHORT. All F0 triples plus the mandatory `D2D_Trend_Dir == direction`.
**Longest signal_def 72 characters; emitted at 83. Any copy that wraps is corrupt.**
Source column: A = decorrelation only, B = pricing only, A+B = both.

```
--- LONG (101 signals) ------------------------------------------------
   1  ATR_1M:hi + Micro_VolOfVol:hi + PrevDay_Close_Dist_ATR:hi                 B
   2  AT_Lookback_LT:hi + DailyOpen_Dist_ATR:lo + OR_High_Side:==-1             A
   3  AT_Lookback_ST:hi + Micro_FailedBreak:lo + OR_Low_Side:==-1               B
   4  AT_Lookback_ST:hi + Micro_OrderFlowDelta:lo + OR_Low_Side:==-1            A
   5  AT_Lookback_ST:hi + Slope_EMA_ST:hi + ADX_Value:lo                        A
   6  AT_Score_LT:lo + Slope_Accel_LT:hi + Body_Size:hi                         B
   7  AT_Score_LT:lo + Slope_Accel_LT:hi + Micro_RangeAccel:hi                  A+B
   8  AT_Score_LT:lo + Slope_Accel_LT:hi + OR_Low_Side:==-1                     A+B
   9  AT_Score_LT:lo + Slope_Accel_LT:hi + VWAP_Sigma:hi                        B
  10  AT_Slope_LT:lo + AT_Slope_ST:lo + Micro_RollProxy:hi                      B
  11  AT_Slope_LT:lo + Micro_FailedBreak:hi + Micro_RangeAccel:lo               A+B
  12  AT_Slope_ST:lo + Micro_BarOverlap:lo + VWAP_Sigma:hi                      A+B
  13  AT_Slope_ST:lo + Micro_FractalDim:lo + Lower_Wick:hi                      A
  14  AT_Slope_ST:lo + Micro_Hurst:hi + Round_1000_Dist_ATR:lo                  B
  15  AT_Slope_ST:lo + Micro_OrderFlowDelta:hi + Micro_WickImbalance:lo         A+B
  16  AT_Slope_ST:lo + Micro_RangeAccel:hi + RangeOsc_State:==2                 B
  17  AT_Slope_ST:lo + Micro_VPIN:lo + OBVf_Signal:==1                          B
  18  AT_Slope_ST:lo + Micro_VolOfVol:hi + OBVf_Signal:==1                      A+B
  19  AT_Slope_ST:lo + Momentum_Value:hi + Body_Size:hi                         B
  20  AT_Slope_ST:lo + Momentum_Value:hi + D2D_Signal:==0                       A+B
  21  AT_Slope_ST:lo + Momentum_Value:hi + Micro_RollProxy:hi                   B
  22  Bars_Since_Flip:hi + D2D_Signal:==0 + OR_Low_Side:==-1                    A
  23  Bars_Since_Flip:hi + Micro_CSSpread:hi + Upper_Wick:hi                    A+B
  24  Bars_Since_Flip:hi + PrevDay_Low_Dist_ATR:lo + OR_High_Side:==-1          A
  25  Bars_Since_Flip:hi + PrevDay_Low_Dist_ATR:lo + OR_Low_Side:==-1           A+B
  26  Bars_Since_Flip:hi + Round_500_Dist_ATR:lo + OR_Low_Side:==-1             A
  27  Bars_Since_Flip:hi + Volume_Avg_10:hi + MultiDay_Position:lo              B
  28  Bars_Since_Flip:hi + Volume_Avg_10:hi + Session_Low_Dist_ATR:lo           B
  29  D2D_ATR_MA:hi + AT_Lookback_ST:hi + Micro_Lambda:lo                       A
  30  D2D_ATR_MA:hi + Micro_VPIN:lo + ST_Flip_Event:==-1                        B
  31  D2D_Persist:hi + PrevDay_Low_Side:==-1 + OR_Low_Side:==1                  A
  32  Efficiency_Ratio:hi + Micro_RangeVelocity:lo + RangeOsc_State:==2         B
  33  Efficiency_Ratio:hi + PrevDay_High_Dist_ATR:lo + Sqz_State:==1            A
  34  KAMA_Dist:hi + AT_Regime_ST:==1 + PrevDay_Low_Side:==-1                   A+B
  35  KAMA_Dist:hi + D2D_Signal:==1 + Harmonic_D2D_Concordance:==0              A
  36  KAMA_Dist:lo + Micro_WickImbalance:hi + ADX_Rising:==0                    B
  37  KAMA_Dist:lo + Micro_WickImbalance:hi + Harmonic_OBVf_Concordance:==0     B
  38  KAMA_Dist:lo + Micro_WickImbalance:hi + Lower_Wick:hi                     B
  39  KAMA_Dist:lo + Micro_WickImbalance:hi + OR_High_Side:==-1                 A+B
  40  KAMA_Dist:lo + Micro_WickImbalance:hi + Volume:hi                         B
  41  KAMA_Dist:lo + ST_Flip_Event:==0 + AT_Regime_ST:==1                       A
  42  KAMA_Dist:lo + VWAP_Dist_ATR:lo + DailyOpen_Dist_ATR:lo                   B
  43  Micro_Amihud:hi + AT_Regime_ST:==1 + OR_Low_Side:==-1                     B
  44  Micro_Amihud:lo + Micro_RangeAccel:lo + Micro_VPIN:hi                     B
  45  Micro_Amihud:lo + WeeklyOpen_Dist_ATR:hi + OR_Low_Side:==-1               A
  46  Micro_AutoCorr:hi + Session_Low_Dist_ATR:lo + TChan_A15:hi                A+B
  47  Micro_Entropy:lo + ADX_Rising:==0 + OR_Low_Side:==-1                      B
  48  Micro_FailedBreak:hi + Micro_Rejection:lo + Volume:hi                     A
  49  Micro_FailedBreak:lo + Micro_Hurst:lo + OR_Low_Side:==-1                  A
  50  Micro_GarmanKlass:hi + Micro_VolOfVol:hi + PrevDay_Close_Dist_ATR:hi      A+B
  51  Micro_IBSP:lo + VWAP_Sigma:lo + Volume:hi                                 A
  52  Micro_LogReturn:hi + Micro_TickIntensity:hi + Micro_VolOfVol:hi           A+B
  53  Micro_MicroGap:lo + AT_Regime_ST:==1 + OR_Low_Side:==-1                   B
  54  Micro_MomoTransfer:hi + Micro_RangeVelocity:lo + AT_Regime_LT:==1         B
  55  Micro_OrderFlowDelta:hi + Micro_Rejection:lo + Lower_Wick:lo              A
  56  Micro_OrderFlowDelta:lo + Micro_WickImbalance:hi + OBVf_Signal:==-1       A
  57  Micro_PriceAccel:hi + VWAP_Sigma:hi + D2D_Signal:==1                      B
  58  Micro_Rejection:lo + AT_Regime_ST:==1 + PrevDay_Low_Side:==-1             A+B
  59  Micro_Rejection:lo + Session_Low_Dist_ATR:lo + OR_Low_Side:==1            A
  60  Micro_VPIN:lo + Volume:hi + ST_Flip_Event:==-1                            B
  61  Micro_WickImbalance:hi + Lower_Wick:lo + AT_Regime_LT:==1                 A+B
  62  Micro_WickImbalance:hi + Lower_Wick:lo + VWAP_Side:==-1                   A+B
  63  Micro_WickImbalance:hi + PrevDay_Close_Side:==1 + OR_Low_Side:==-1        A
  64  Momentum_Value:hi + Micro_Hurst:hi + AT_Regime_ST:==1                     A+B
  65  OBV_Macd:lo + Harmonic_LLEMA:hi + Micro_Rejection:lo                      A+B
  66  OBV_Macd:lo + Harmonic_LLEMA:hi + Micro_WickImbalance:lo                  A
  67  OBV_Macd:lo + Micro_Hurst:hi + Micro_VolOfVol:hi                          A
  68  OBV_Macd:lo + Micro_Hurst:hi + VWAP_Dist_ATR:lo                           A
  69  OBV_Macd:lo + Micro_Lambda:lo + Micro_ThrustEff:hi                        B
  70  OBV_Macd:lo + Micro_LogReturn:lo + Micro_Rejection:lo                     A
  71  OBV_Macd:lo + Micro_MicroGap:lo + Upper_Wick:hi                           A
  72  OBV_Macd:lo + Micro_OrderFlowDelta:lo + Micro_WickImbalance:hi            A+B
  73  OBV_Macd:lo + Micro_WickImbalance:lo + ADX_Value:hi                       B
  74  OBV_Macd:lo + Sqz_State:==1 + RangeOsc_State:==-2                         A+B
  75  OBV_Macd:lo + Upper_Wick:hi + WeeklyOpen_Side:==-1                        A
  76  OBV_Macd:lo + Volume_Ratio_10:hi + AT_Regime_ST:==1                       B
  77  OBV_Velocity:hi + Micro_FractalDim:hi + OR_Low_Side:==-1                  A+B
  78  OR_Position:lo + AT_Regime_ST:==1 + DailyOpen_Side:==1                    A
  79  OR_Position:lo + D2D_DirStep:==-1 + VWAP_Side:==1                         A
  80  PrevDay_High_Dist_ATR:lo + ADX_Rising:==1 + PrevDay_Low_Side:==-1         A
  81  PrevDay_High_Dist_ATR:lo + Sqz_State:==1 + PrevDay_Low_Side:==-1          A
  82  RangeOsc_Val:hi + Session_Low_Dist_ATR:lo + PrevDay_Close_Side:==-1       A+B
  83  Slope_Accel_LT:hi + AT_Regime_ST:==1 + OR_Low_Side:==-1                   A+B
  84  Slope_Accel_LT:hi + KAMA_Dist_ATR:hi + RangeOsc_State:==2                 A+B
  85  Slope_Accel_LT:hi + OBV_Macd:lo + Micro_TickIntensity:hi                  B
  86  Slope_Accel_LT:hi + OBV_Macd:lo + Sqz_State:==1                           A
  87  Slope_Accel_LT:hi + Sqz_State:==1 + RangeOsc_State:==-2                   B
  88  Slope_Accel_LT:lo + Micro_RangeAccel:lo + OBVf_Signal:==1                 A
  89  Slope_Accel_ST:lo + Micro_VPIN:lo + ST_Flip_Event:==-1                    B
  90  Slope_EMA_LT:hi + Momentum_Value:lo + Micro_TickIntensity:lo              A
  91  Slope_EMA_LT:lo + ADX_Value:hi + Sqz_State:==1                            A
  92  Slope_EMA_ST:hi + Micro_Rejection:lo + Session_Low_Dist_ATR:lo            A+B
  93  Slope_EMA_ST:lo + Micro_Hurst:hi + PrevDay_High_Side:==-1                 B
  94  Slope_EMA_ST:lo + Micro_RangeAccel:hi + Micro_Rejection:lo                B
  95  Slope_EMA_ST:lo + Micro_VolOfVol:hi + AT_Regime_ST:==1                    A+B
  96  Sqz_Val:hi + Sqz_State:==1 + RangeOsc_State:==-2                          B
  97  Sqz_Val:lo + Micro_IBSP:hi + Micro_VolAccel:hi                            A+B
  98  VAL_Dist_ATR:lo + OR_Position:hi + OR_High_Side:==-1                      A
  99  Volume_Avg_10:hi + Micro_FailedBreak:hi + Micro_Rejection:lo              A+B
 100  Volume_Ratio_10:hi + AT_Regime_ST:==1 + OR_Low_Side:==-1                  B
 101  WeeklyOpen_Dist_ATR:hi + Sqz_State:==1 + OR_Low_Side:==-1                 A
--- SHORT (96 signals) -----------------------------------------------
   1  ATR_1M:hi + WeeklyOpen_Dist_ATR:hi + PrevDay_High_Side:==1                B
   2  AT_Lookback_LT:hi + Micro_Amihud:lo + Body_Size:hi                        A
   3  AT_Score_LT:hi + EMA_Oscillator:lo + RangeOsc_State:==-1                  A
   4  AT_Score_ST:lo + VWAP_Sigma:hi + PrevDay_High_Side:==1                    A
   5  AT_Slope_ST:hi + Micro_BarEntropy:lo + Micro_HLAsymmetry:lo               A+B
   6  AT_Slope_ST:hi + Micro_BarEntropy:lo + Sqz_State:==-1                     A+B
   7  AT_Slope_ST:hi + Micro_VPIN:lo + Sqz_State:==-1                           A+B
   8  Bar_Range:hi + D2D_Dynamic_Sensitivity:lo + Lower_Wick:lo                 B
   9  Bar_Range:hi + PrevDay_Low_Dist_ATR:hi + PrevDay_High_Side:==1            A
  10  D2D_ATR:hi + D2D_Dynamic_Sensitivity:lo + Lower_Wick:lo                   B
  11  D2D_ATR:hi + Micro_ThrustEff:hi + PrevDay_Close_Dist_ATR:hi               B
  12  D2D_ATR:hi + Sqz_Val:hi + Session_High_Dist_ATR:hi                        B
  13  D2D_ATR:hi + WeeklyOpen_Dist_ATR:hi + D2D_DirStep:==1                     A
  14  D2D_ATR:hi + WeeklyOpen_Dist_ATR:hi + PrevDay_High_Side:==1               A+B
  15  D2D_ATR_MA:hi + Sqz_Val:hi + DailyOpen_Dist_ATR:hi                        B
  16  D2D_ATR_MA:hi + Sqz_Val:hi + Session_High_Dist_ATR:hi                     B
  17  D2D_ATR_MA:hi + WeeklyOpen_Dist_ATR:hi + D2D_DirStep:==1                  A
  18  D2D_DirStep:==1 + DailyOpen_Side:==-1 + OR_High_Side:==1                  A
  19  D2D_Dn_Count:hi + Volume_Avg_10:hi + Micro_VPIN:hi                        A
  20  D2D_Dynamic_Sensitivity:lo + Lower_Wick:lo + Volume:hi                    B
  21  D2D_Dynamic_Sensitivity:lo + Micro_CSSpread:lo + Micro_LogReturn:lo       A+B
  22  D2D_Dynamic_Sensitivity:lo + Micro_GarmanKlass:hi + Lower_Wick:lo         B
  23  D2D_Dynamic_Sensitivity:lo + Micro_IBSP:lo + Micro_WickImbalance:hi       A
  24  D2D_Dynamic_Sensitivity:lo + Micro_ThrustEff:lo + Micro_WickImbalance:lo  B
  25  D2D_Dynamic_Sensitivity:lo + Volume_Avg_10:hi + Efficiency_Ratio:hi       B
  26  D2D_Dynamic_Sensitivity:lo + Volume_Avg_10:hi + Micro_AutoCorr:hi         A+B
  27  DailyOpen_Dist_ATR:lo + Harmonic_OBVf_Concordance:==1 + VAH_Side:==0      A+B
  28  DailyOpen_Dist_ATR:lo + TChan_A15:hi + VWAP_Z:lo                          A
  29  DailyOpen_Dist_ATR:lo + VAH_Side:==0 + PrevDay_Low_Side:==1               B
  30  Micro_Amihud:hi + Micro_VolOfVol:lo + OR_Low_Side:==-1                    B
  31  Micro_BarEntropy:hi + ADX_Rising:==1 + VAH_Side:==0                       A+B
  32  Micro_BarEntropy:lo + Micro_Hurst:hi + Session_High_Dist_ATR:lo           A+B
  33  Micro_BarOverlap:hi + Micro_Entropy:lo + VWAP_Z:lo                        A+B
  34  Micro_BarOverlap:hi + Micro_FailedBreak:lo + Lower_Wick:hi                A+B
  35  Micro_BarOverlap:hi + Micro_HLAsymmetry:lo + PrevDay_High_Side:==1        A+B
  36  Micro_BarOverlap:lo + Micro_Hurst:hi + Session_High_Dist_ATR:lo           A
  37  Micro_BarOverlap:lo + RangeOsc_State:==2 + OR_High_Side:==1               B
  38  Micro_Entropy:hi + Micro_GarmanKlass:hi + RangeOsc_State:==2              A
  39  Micro_Entropy:lo + Lower_Wick:lo + Volume:hi                              A+B
  40  Micro_Entropy:lo + Micro_WickImbalance:lo + Lower_Wick:lo                 A+B
  41  Micro_Entropy:lo + PrevDay_High_Dist_ATR:lo + VWAP_Z:lo                   A
  42  Micro_FailedBreak:lo + Micro_FractalDim:lo + Micro_HLAsymmetry:hi         A+B
  43  Micro_FailedBreak:lo + Micro_PriceAccel:hi + Micro_WickImbalance:lo       B
  44  Micro_GarmanKlass:hi + WeeklyOpen_Dist_ATR:hi + ADX_Value:hi              A
  45  Micro_Hurst:hi + TChan_A15:hi + RangeOsc_State:==2                        A
  46  Micro_LogReturn:hi + Micro_PriceAccel:lo + Micro_VolOfVol:lo              B
  47  Micro_LogReturn:lo + WeeklyOpen_Dist_ATR:hi + Lower_Wick:hi               A+B
  48  Micro_MomoTransfer:hi + Micro_RangeAccel:lo + OBVf_Signal:==1             A+B
  49  Micro_PriceAccel:hi + Micro_RangeVelocity:hi + PoC_Side:==0               B
  50  Micro_RangeAccel:lo + Micro_ThrustEff:hi + Micro_VolAccel:hi              B
  51  Micro_RangeVelocity:lo + Lower_Wick:lo + AT_Regime_ST:==0                 A+B
  52  Micro_Rejection:hi + Body_Size:hi + PrevDay_High_Side:==1                 A+B
  53  Micro_VolAccel:lo + Upper_Wick:hi + PoC_Side:==0                          A
  54  Momentum_Value:hi + Micro_Lambda:lo + PrevDay_Low_Side:==-1               A+B
  55  Momentum_Value:hi + Micro_TickIntensity:hi + PrevDay_Low_Side:==-1        A+B
  56  Momentum_Value:hi + Micro_TickIntensity:hi + Session_High_Dist_ATR:hi     B
  57  Momentum_Value:hi + Session_High_Dist_ATR:hi + OR_Position:lo             A+B
  58  Momentum_Value:hi + Session_High_Dist_ATR:hi + TChan_A15:hi               B
  59  OBV_Velocity:hi + Micro_RollProxy:hi + PrevDay_High_Dist_ATR:lo           A
  60  OBV_Velocity:hi + TChan_A15:hi + PrevDay_Close_Side:==1                   A+B
  61  OBV_Velocity:hi + TChan_A15:hi + VWAP_Side:==1                            B
  62  OBV_Velocity:lo + Micro_BarEntropy:hi + PoC_Side:==0                      B
  63  OBVf_DirStepCount:hi + Micro_GarmanKlass:hi + WeeklyOpen_Dist_ATR:hi      A
  64  OBVf_DirStepCount:hi + VAL_Side:==0 + WeeklyOpen_Side:==-1                A+B
  65  OBVf_DirStepCount:lo + Lower_Wick:lo + VWAP_Sigma:hi                      B
  66  OBVf_DirStepCount:lo + Sqz_Val:lo + Micro_HLAsymmetry:lo                  A
  67  OR_High_Dist_ATR:hi + VWAP_Z:lo + OBVf_Signal:==1                         A
  68  Round_500_Dist_ATR:lo + VAH_Side:==0 + PrevDay_Low_Side:==1               A
  69  Slope_Accel_LT:lo + Micro_AutoCorr:lo + Micro_FractalDim:lo               B
  70  Slope_Accel_LT:lo + Micro_OrderFlowDelta:hi + PoC_Side:==0                B
  71  Slope_Accel_LT:lo + Micro_Rejection:hi + PrevDay_High_Side:==1            B
  72  Slope_Accel_LT:lo + Micro_RollProxy:lo + PoC_Side:==0                     A
  73  Slope_Accel_LT:lo + Sqz_Val:hi + Micro_CSSpread:lo                        A+B
  74  Slope_Accel_ST:hi + ADX_Value:lo + TChan_A15:hi                           A
  75  Slope_Accel_ST:hi + ADX_Value:lo + VWAP_Sigma:hi                          B
  76  Slope_Accel_ST:hi + Micro_AutoCorr:hi + ADX_Value:lo                      A
  77  Slope_Accel_ST:hi + Micro_Hurst:hi + VWAP_Sigma:hi                        A
  78  Slope_Accel_ST:hi + MultiDay_Position:hi + RangeOsc_State:==2             A+B
  79  Slope_Accel_ST:hi + OBV_Velocity:lo + Micro_MicroGap:lo                   A
  80  Slope_Accel_ST:hi + WeeklyOpen_Dist_ATR:hi + TChan_A15:hi                 A+B
  81  Slope_Accel_ST:lo + Volume_Avg_10:hi + VAL_Side:==-1                      A+B
  82  Slope_EMA_LT:hi + Micro_VPIN:lo + OR_High_Side:==-1                       A
  83  Slope_EMA_ST:lo + PrevDay_Close_Dist_ATR:lo + VAH_Side:==0                A
  84  Slope_EMA_ST:lo + RangeOsc_State:==2 + PrevDay_High_Side:==1              B
  85  Slope_EMA_ST:lo + Slope_Accel_ST:hi + ADX_Value:lo                        B
  86  Sqz_Val:hi + Micro_OrderFlowDelta:hi + Session_High_Dist_ATR:hi           A+B
  87  Sqz_Val:hi + Micro_TickIntensity:hi + Session_High_Dist_ATR:hi            A+B
  88  Sqz_Val:hi + Momentum_Value:hi + Session_High_Dist_ATR:hi                 A+B
  89  Sqz_Val:hi + OR_Position:hi + RangeOsc_State:==2                          A+B
  90  Sqz_Val:hi + PrevDay_Close_Dist_ATR:hi + Volume:hi                        B
  91  Sqz_Val:hi + Session_High_Dist_ATR:hi + Volume:hi                         A
  92  Sqz_Val:hi + Volume_Avg_10:hi + PrevDay_Close_Dist_ATR:hi                 A+B
  93  VA_Position:hi + ADX_Value:lo + Sqz_State:==-1                            B
  94  Volume_Avg_10:hi + DailyOpen_Dist_ATR:lo + VAH_Side:==0                   A+B
  95  Volume_Avg_10:hi + PrevDay_High_Dist_ATR:hi + Lower_Wick:lo               A
  96  WeeklyOpen_Dist_ATR:hi + ADX_Value:lo + TChan_A15:hi                      A
```

---

# 5. GATE LIVENESS AT L8/S5

    LONG   tier 3    HU90               unreachable under floor 8   DEAD CODE
    LONG   tier 4    FB20 + ATS90       unreachable under floor 8   DEAD CODE
    LONG   tier 5+   FB20               LIVE — carries all LONG trades
    SHORT  tier 3    HU90               unreachable under floor 5   DEAD CODE
    SHORT  tier 4    Micro_Rejection    unreachable under floor 5   DEAD CODE
    SHORT  tier 5+   Micro_Rejection    LIVE — carries all SHORT trades
    tiers 1 and 2                       unreachable                 DEAD CODE

**THIS IS A MATERIAL FINDING AND IT CUTS AGAINST MY OWN DESIGN.** At floor 8/5 with
`tier = min(depth,5)`, every admitted trade lands in tier 5+. **Four of the six gate cells
never fire. The system reduces to two gates: `FB20` on longs and `Micro_Rejection < p50` on
shorts.** The inherited stack's tier structure is inert at this floor, exactly as v3's L7/S4
variant found `Micro_Hurst > p90` inert at its own floor.
**A BUILD MUST CARRY ONLY THE TIER-5 CELLS. Carrying the others is dead weight that will
mislead the next reader.**

---

# 6. PERFORMANCE AND SEGMENTS

## 6.1 SEGMENTS — NOT A WALK-FORWARD, SEE 0.2

    window              tr     ev   days   WR      PF      worst day   lwk    lmo    net
    FULL Jan19-Jul21  2,263    10    88   97.66   22.53     -$637.0   0/25   0/7  $105,611
    Jan-May           1,661     7    64   97.77   20.79     -$637.0   0/18   0/5   $78,655
    Jun-Jul HELD-OUT    602     3    24   97.34   29.99      +$65.5   0/7    0/2   $26,956
    July HOLDOUT        199     1     9   96.98   46.20     +$210.0   0/3    0/1   $12,042
    H1 Jan19-Apr20    1,307     4    45   98.39   25.63      +$70.0   0/12   0/4   $62,235
    H2 Apr21-Jul21      956     6    43   96.65   19.24     -$637.0   0/13   0/4   $43,376

**The held-out Jun-Jul window has a POSITIVE worst day. So does July, and so does H1.**
**These are the strongest figures in the document and 0.2 explains why they cannot be
trusted: the selection saw those months.** PF 46.20 on July rests on one loss event and is
declined.

## 6.2 RISK POSTURE

Worst day -$637.0 at 1.0 lot. **At the operator's 0.20-0.25 lot deployment that is -$127 to
-$159 against an FTMO $2,500 daily ceiling — 5.1% to 6.4%.** At 1.0 lot, 25.5%; headroom 3.9x.
**Worst bar -$773.4 exceeds worst day, so the tail is a single simultaneous event.**
Slippage is not modelled anywhere in this project.

---

# 7. THE BATTERY

## 7.1 RUN

Floor sweep 15 cells · days-push 6 cells · short-gate 7 candidates · hybrid sweep 12 cells ·
segment scores on both finalists · split-half on both · July holdout · gate liveness ·
depth-ladder and monthly (first document) · randomisation 40 draws (first document, on
PRICED-100 not on this book) · alone-first gate on all four objectives · full-field
replication of all four.

## 7.2 NOT RUN — EVERY ONE A NAMED GAP

**1. Rebuild A's matrix at cap 21 / floor 1/1 on `adm_engine` and re-run. FIRST ITEM.
Predicted to be identical; if not, every A-sourced row here is provisional.**
2. 500-draw randomisation on THIS book with gates derived per draw. **The Quant ran this on
its own system and I did not run it on mine. That is the biggest asymmetry between the two
documents.**
3. True walk-forward with train-only pricing and a train-only loss-day matrix.
4. Rarity-matched null on the short-side gate (0.4).
5. Tail-bar removal (0.6).
6. Anti-system negative control.
7. Bonferroni or Romano-Wolf across the 12-cell hybrid sweep.
8. BY q<0.10 comparison against the E ranking.
9. Slippage, partial fills, gap risk at the jar's 21 live lots.

## 7.3 TRIAL COUNT — CUMULATIVE ACROSS BOTH DOCUMENTS

    first document, scored configurations                ~118
    this document                                          ~52
      floor sweep 3 books x 5 floors                        15
      days push 2 books x 3 floors                           6
      short-gate candidates                                  7
      hybrid sweep 4 sizes x 3 floors                       12
      battery on two finalists                              12
    CUMULATIVE                                            ~170
    plus per-signal solo scoring runs                    21,594

**Free parameters swept: K (4 values), floor (3-5 values), short-gate mask (6 candidates).
No correction applied to any of it.**

---

# 8. WHAT THIS SYSTEM CANNOT SEE

**8.1 THE 14 ORPHANS.** 6 LONG / 8 SHORT, never emitted by the F0 scanner at `MIN_TRADES=10`.
No scan-based procedure reaches them.

**8.2 THE RAW SCAN.** Objective A on all 19,754 gives worst day -$3,213 and 6 losing weeks
against -$341 and 1 on the VALID field. **Loss-day decorrelation minimises the NUMBER of
losing days, not their SIZE**; the VALID screen does work the objective depends on.

**8.3 EVERY FAMILY EXCEPT F0.** F1's 37,276 rows, plus F2/F3/F4/F9/F11 — none searched.

**8.4 THE OBJECTIVES THAT FAILED.** VALID field, 120 signals, 15 size-matched draws:
C co-fire affinity 119 events / -$4,524 and D terrain coverage 98 events / -$2,911, both
worse than 100% of draws. **D was my own proposal and the (1-1/e) bound holds — the objective
it guarantees is simply the wrong one.**
**AND THE QUANT'S D PASSES WHERE MINE FAILS.** Its episode map comes from `bookB_setup.pkl`;
mine from `touched_episode_ids`. **One of the two maps is wrong and neither seat has verified
which. That is the single most consequential unresolved disagreement between the documents**,
because D contributes to the Quant's adopted book and is refuted in mine.

**8.5 THE FUSION IS REAL ON BARS AND NOT ON EVENTS.** Union bars run +376 to +2,063 beyond
sum-of-parts at every configuration. **But loss events do not fuse** — A 15 + B 14 = 29 and
the union gives 31, with a tail worse than either parent. **The floor is what recovers it:
A+B at L3/S3 has 37 events; the same members at L8/S5 have 10.** The union buys reach and the
floor buys back the tail.

---

# 9. THE FRONTIER

    config                          sig   ev   worst_day   PF      days  lwk   lmo   MAR     net
    A+B K=50 L8/S5 +REJ             159    3     -$723.5   35.18    71   0/25  0/7  28.46  $61,274
    A+B K=65 L8/S5 +REJ   ADOPTED   197   10     -$637.0   22.53    88   0/25  0/7  32.74 $105,611
    A+B K=50 L7/S4 +REJ             159   12     -$654.3   21.63    95   0/25  0/7  34.71  $88,401
    A+B K=50 L6/S4 +REJ             159   13     -$654.3   20.59    96   0/25  0/7  33.16  $91,853
    A+B K=80 L8/S5 +REJ             244   18     -$648.5   16.59    97   1/25  0/7  33.20 $139,200
    A+B K=65 L7/S4 +REJ             197   23     -$637.0   16.30   106   0/25  0/7  34.74 $122,895
    A+B K=100 L8/S5 +REJ            302   32   -$1,006.2   11.36   111   0/26  0/7  30.37 $177,530

**WHY K=65 L8/S5.** Under the stated ordering it leads on loss events (10) and on tail
(-$637) among every cell reaching both zero losing weeks and zero losing months.
**THE CLOSE ALTERNATIVE IS K=50 L7/S4 AT 95 DAYS**, which trades 2 more loss events and
$17 of tail for 7 more days and 2 points of margin. **If the operator's days agenda outranks
the ordering as written, that is the pick, and I would not argue against it.**
**K=50 L8/S5 has THREE loss events on 71 days** — the fewest of anything measured in this
project — and is the tail-minimal extreme.

**AND THE TARGET IS STILL NOT MET.** 115+ days with a tail inside -$600 and zero losing weeks
does not exist in anything I have measured across two documents. The nearest is
**A+B K=65 L7/S4 +REJ at 106 days and -$637** — nine days short, $37 outside.
