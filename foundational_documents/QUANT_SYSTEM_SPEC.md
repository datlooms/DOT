# QUANT_SYSTEM_SPEC.md
# THE B+D UNION — a deterministic two-objective selection procedure and the book it produces
# Field: catalogue_F0.csv VALID, 1,818 rows. Frame: 177,251 bars, 2026.01.19-2026.07.21.
# EVERY FIGURE AT 1.0 LOT. There is no max-lot column in this document.

================================================================================
0. WHAT WOULD MAKE THIS SPECIFICATION WRONG
================================================================================

0.1 THE CLAIMS THAT COULD FAIL, AND THE MEASUREMENT THAT WOULD SHOW IT

1.  THE TRIAL COUNT IS LARGE AND THE MARGIN EDGE IS NOT.
    I ran 24 grid cells to arrive at this pick, inside a session of ~190 scored
    configurations. Margin 34.38 cleared 499 of 500 size-matched draws at
    p = 0.0020 against a Bonferroni requirement of p < 0.0021 at 24 cells.
    FALSIFIER: re-run the 500-draw control with a fresh seed block. If margin
    drops below ~499/500 the edge was search luck.

2.  THE TERRAIN OBJECTIVE DEPENDS ON AN EPISODE MAP I DID NOT REBUILD.
    D uses MU/MD/PU/PD from bookB_setup.pkl. I verified the targets are the
    catalogue rows; I did NOT verify the episode map itself was built on the
    VALID field rather than a slice.
    FALSIFIER: rebuild the episode map from the terrain artifacts and re-run D
    alone against its random control. If D stops beating chance, it does not enter.

3.  THE JULY HOLDOUT IS ONE LOSS EVENT.
    1 event on 11 trading days, worst day +$80.50. That is a COUNT. Every ratio
    on it - including PF 88.44 - is declined and must not be quoted.
    FALSIFIER: the next month of data. Nothing else settles it.

4.  E_dir REBUILDS A PUBLISHED COLUMN RATHER THAN RECOMPUTING THE NULL.
    E_dir = n_dir x pf_null_exceedance_pct, taking exceedance from
    catalogue_F0.csv. The null itself (4,652 rarity-matched draws) is NOT
    recomputed per direction - only the multiplier is corrected.
    FALSIFIER: draw a per-direction null. If SHORT exceedance moves materially,
    the correction is partial and the 27 SHORT members are mispriced.

5.  OBJECTIVE A IS UNRECONCILED BETWEEN TWO SEATS. See 8.4. It is NOT in this
    system, so the system does not depend on it - but the disagreement is open.

6.  THE ANTI-SYSTEM PF>=8 ARM RESTS ON 6 LOSS EVENTS. Its PF of 6.08 is a count
    result. The full and PF>=4 arms (799 and 234 events) carry the conclusion.

0.2 WHAT IS NOT TESTED

- No walk-forward on any cell other than the adopted one.
- The anti-system was run on the adopted cell only.
- L4/S3 is measured on other cells but NOT on this one.
- Objective A, co-fire affinity, and solo persistence (E) are excluded; each
  failed the alone-first gate or a control. See 8.2.
- No live data. Every figure is backtest on a sealed frame.

================================================================================
1. HEADLINE - 1.0 LOT
================================================================================

  BOOK              116 signals   74 LONG / 42 SHORT
  FIELD             catalogue_F0.csv VALID, 1,818 rows
  FLOOR             L3/S3          CAP 21          GATES frozen (see 5)

  trades                     2,065
  entry bars                   394
  days traded              92 of 132
  WR                        95.40%
  PF                         13.25
  LOSS EVENTS                   21   on 18 event-days
  losing-bar rate            5.33%
  worst bar               -$918.00
  worst day               -$451.90
  worst intraday        -$1,043.90
  losing days                    6
  LOSING WEEKS             0 of 26
  break-even WR             61.01%
  MARGIN                     34.38
  net                     $100,652

  MONTHLY (net / trades)
    2026.01    $2,659 /  87      2026.05   $16,249 / 185
    2026.02   $14,198 / 474      2026.06   $11,965 / 353
    2026.03   $27,489 / 541      2026.07   $15,529 / 175
    2026.04   $12,563 / 250
  NO LOSING MONTH. NO LOSING WEEK.

================================================================================
2. THE RULE, IN EXECUTION ORDER
================================================================================

STEP 1  SCREEN
        Field = catalogue_F0.csv rows with verdict == VALID.  1,818 of 1,840.
        NEVER a sub-slice: decorrelation-class objectives break on a filtered
        field (worst day -$341 -> -$3,213 measured on the raw scan).
        Train window = all bars with exit_bar <= config.holdout_boundary.
        This frame: N=1 holdout month, boundary bar 157,408, train 2026.01-06.
        PRINT N AND THE RESOLVED WINDOW EVERY RUN.

STEP 2  OBJECTIVE B - CHANCE-PRICING, PER DIRECTION
        E_dir[i] = n_dir(direction of i) * pf_null_exceedance_pct[i]
          n_LONG = 1,445    n_SHORT = 373    K = 4,652 nulls
          resolution floors n_dir/K: LONG 0.3106, SHORT 0.0802 - PRINT BOTH
        SELECT: E_dir < config.B_threshold          this frame: 2
        RESULT: 82 signals, 55 LONG / 27 SHORT
        DO NOT use family-priced E: n_trials_family = 1,840 with
        null_direction_long_share = 0.7951 prices every SHORT row against an
        80%-LONG null, giving a 9.0:1 split against the pool's 2.36:1.

STEP 3  OBJECTIVE D - TERRAIN COVERAGE, PER DIRECTION
        Greedy set cover over reachable episodes, submodular, (1-1/e) bound.
          seed:  empty set
          step:  admit argmax |episodes(i) - covered|; stop at target or when
                 marginal gain reaches zero
          ties:  lowest catalogue index (deterministic, no RNG)
        TARGET: config.D_target_long / config.D_target_short   this frame 20/15
        RESULT: 35 signals, 20 LONG / 15 SHORT. Covered 115 of 498 UP,
                77 of 486 DOWN.
        Supplies direction balance WITH NO QUOTA.

STEP 4  UNION
        BOOK = B | D.  116 signals. Overlap is 1 signal.
        No ranking, no draw, no seed, no path dependence beyond the greedy.

STEP 5  EXECUTE
        FLOORED admission, floor from config (this system: L3/S3), cap 21.
        Gates frozen - see 3.2 and 5.
        recentfb_sizing = False. Conviction stack as adopted.
        SELECTION CAP == EXECUTION CAP == 21.

STEP 6  SCORE and emit the frontier (7.3), not a point.

================================================================================
3. THE CONSTANT REGISTRY
================================================================================

3.1 SELECTION CONSTANTS

  name                  value     source
  ---------------------------------------------------------------------------
  field                 VALID     catalogue_F0.csv verdict column
  holdout months N      1         CONFIG. Moves survivor count ~28% between
                                  N=1 and N=2 - never leave it implicit.
  B_threshold           2         DERIVED this session. E_dir<1 gives 67 days,
                                  <2 gives 73, <3 breaks the tail (8 cells at
                                  -$1,760 to -$2,261 worst day). REFUTED above 2.
  D_target_long         20        DERIVED. Saturates above ~75 (T75/55 identical
  D_target_short        15        to T90/65 on every figure).
  greedy tie-break      min index DERIVED. Deterministic; no RNG anywhere.

3.2 EXECUTION CONSTANTS - ALL INHERITED, NONE DERIVED HERE

  floor                 L3/S3     INHERITED from the adopted architecture.
  cap                   21        INHERITED. Located on a 297-signal book; NOT
                                  re-derived for this book. See 8.3.
  ATR_1M floor          20        INHERITED.
  tier = min(depth,5)   5         INHERITED, UNDOCUMENTED. Not re-derived here.
  recentfb_sizing       False     INHERITED.
  gate LONG d3          Micro_Hurst > p90
  gate LONG d4          Micro_FailedBreak > p20 AND AT_Slope_ST > p90
  gate LONG d5+         Micro_FailedBreak > p20
  gate SHORT d3         Micro_Hurst > p90
  gate SHORT d4/d5+     FREE
                        ALL INHERITED. NOT DERIVED FOR THIS BOOK. See 8.3.

3.3 THE LOSS-DAY MATRIX PARAMETERS - STATED FOR THE OPEN RECONCILIATION (8.4)

  This system does NOT use objective A. These are the parameters I used when I
  built A, recorded so the two seats' figures can be compared directly:
    MAX_POSITIONS   21        (NOT 6)
    ADM_FLOOR       {1:1,-1:1}
    ADM_GATES       None      (no ATR floor)
    ADM_TIERGATES   None      (no gates)
    day key         EXIT bar date
    pool filter     net > 0, per direction
    seed            argmax(net - 50 * losing_days)
    coverage        len(losing_days & covered), covered grows only

3.4 GATE MASK SEMANTICS
  swept_thresholds.build_whole_dot_gates(df) -> substitutes dots_thresholds
  _D_SPEC, calls the sacred compute_adaptive_thresholds, restores in finally.
  Ring 2500, day-refreshed, floor-index percentile.
  CHECKSUMS on this frame (177,251 bars):
    HU90 9.7478%   FB20 80.1874%   ATS90 6.2217%
  A mask near 20% where HU90 belongs means the p80 series is live. ABORT.

================================================================================
4. THE FULL SIGNAL LISTING
================================================================================

4.1 PROVENANCE
  B only    81      D only    34      B and D    1      TOTAL 116

4.2 LISTING INTEGRITY
  Emitted at fixed width. Longest signal_def 72 chars; longest line 83 chars.
  118 rows. ANY COPY THAT WRAPS OR ELIDES IS CORRUPT.
  Source tag is B, D or B+D. A parser must whitelist those three tokens.

--- LONG (74 signals) ----------------------------------------
   1  AT_Score_LT:lo + Slope_Accel_LT:hi + Body_Size:hi                         B
   2  AT_Score_LT:lo + Slope_Accel_LT:hi + Micro_RangeAccel:hi                  B
   3  AT_Score_LT:lo + Slope_Accel_LT:hi + VWAP_Sigma:hi                        B
   4  AT_Slope_LT:lo + AT_Slope_ST:lo + Micro_RollProxy:hi                      B
   5  AT_Slope_LT:lo + Micro_FailedBreak:hi + Micro_RangeAccel:lo               B
   6  AT_Slope_ST:lo + Micro_BarOverlap:lo + VWAP_Sigma:hi                      B
   7  AT_Slope_ST:lo + Micro_Hurst:hi + Round_1000_Dist_ATR:lo                  B
   8  AT_Slope_ST:lo + Micro_OrderFlowDelta:hi + Micro_WickImbalance:lo         B
   9  AT_Slope_ST:lo + Micro_RangeAccel:hi + RangeOsc_State:==2                 B
  10  AT_Slope_ST:lo + Micro_VPIN:lo + OBVf_Signal:==1                          B
  11  AT_Slope_ST:lo + Micro_VolOfVol:hi + OBVf_Signal:==1                      B
  12  AT_Slope_ST:lo + Momentum_Value:hi + Body_Size:hi                         B
  13  AT_Slope_ST:lo + Momentum_Value:hi + D2D_Signal:==0                       B
  14  AT_Slope_ST:lo + Momentum_Value:hi + Micro_RollProxy:hi                   B
  15  Bars_Since_Flip:hi + Micro_CSSpread:hi + Upper_Wick:hi                    B
  16  Bars_Since_Flip:hi + PrevDay_Low_Dist_ATR:lo + OR_Low_Side:==-1           B
  17  Bars_Since_Flip:hi + Volume_Avg_10:hi + MultiDay_Position:lo              B
  18  Bars_Since_Flip:hi + Volume_Avg_10:hi + Session_Low_Dist_ATR:lo           B
  19  D2D_ATR_MA:hi + Micro_VPIN:lo + ST_Flip_Event:==-1                        B
  20  Efficiency_Ratio:hi + Micro_RangeVelocity:lo + RangeOsc_State:==2         B
  21  KAMA_Dist:lo + Micro_WickImbalance:hi + ADX_Rising:==0                    B
  22  KAMA_Dist:lo + Micro_WickImbalance:hi + Harmonic_OBVf_Concordance:==0     B
  23  KAMA_Dist:lo + Micro_WickImbalance:hi + Lower_Wick:hi                     B
  24  KAMA_Dist:lo + Micro_WickImbalance:hi + Volume:hi                         B
  25  KAMA_Dist:lo + VWAP_Dist_ATR:lo + DailyOpen_Dist_ATR:lo                   B
  26  Micro_Amihud:hi + AT_Regime_ST:==1 + OR_Low_Side:==-1                     B
  27  Micro_Amihud:lo + Micro_RangeAccel:lo + Micro_VPIN:hi                     B
  28  Micro_AutoCorr:hi + Session_Low_Dist_ATR:lo + TChan_A15:hi                B
  29  Micro_GarmanKlass:hi + Micro_VolOfVol:hi + PrevDay_Close_Dist_ATR:hi      B
  30  Micro_LogReturn:hi + Micro_TickIntensity:hi + Micro_VolOfVol:hi           B
  31  Micro_MicroGap:lo + AT_Regime_ST:==1 + OR_Low_Side:==-1                   B
  32  Micro_MomoTransfer:hi + Micro_RangeVelocity:lo + AT_Regime_LT:==1         B
  33  Micro_PriceAccel:hi + VWAP_Sigma:hi + D2D_Signal:==1                      B
  34  Micro_Rejection:lo + AT_Regime_ST:==1 + PrevDay_Low_Side:==-1             B
  35  Micro_VPIN:lo + Volume:hi + ST_Flip_Event:==-1                            B
  36  Micro_WickImbalance:hi + Lower_Wick:lo + AT_Regime_LT:==1                 B
  37  Momentum_Value:hi + Micro_Hurst:hi + AT_Regime_ST:==1                     B
  38  OBV_Macd:lo + Harmonic_LLEMA:hi + Micro_Rejection:lo                      B
  39  OBV_Macd:lo + Micro_OrderFlowDelta:lo + Micro_WickImbalance:hi            B
  40  OBV_Macd:lo + Micro_WickImbalance:lo + ADX_Value:hi                       B
  41  OBV_Macd:lo + Sqz_State:==1 + RangeOsc_State:==-2                         B
  42  OBV_Macd:lo + Volume_Ratio_10:hi + AT_Regime_ST:==1                       B
  43  OBV_Velocity:hi + Micro_FractalDim:hi + OR_Low_Side:==-1                  B
  44  RangeOsc_Val:hi + Session_Low_Dist_ATR:lo + PrevDay_Close_Side:==-1       B
  45  Slope_Accel_LT:hi + AT_Regime_ST:==1 + OR_Low_Side:==-1                   B
  46  Slope_Accel_LT:hi + OBV_Macd:lo + Micro_TickIntensity:hi                  B
  47  Slope_Accel_LT:hi + Sqz_State:==1 + RangeOsc_State:==-2                   B
  48  Slope_Accel_ST:lo + Micro_VPIN:lo + ST_Flip_Event:==-1                    B
  49  Slope_EMA_ST:hi + Micro_Rejection:lo + Session_Low_Dist_ATR:lo            B
  50  Slope_EMA_ST:lo + Micro_Hurst:hi + PrevDay_High_Side:==-1                 B
  51  Slope_EMA_ST:lo + Micro_RangeAccel:hi + Micro_Rejection:lo                B
  52  Slope_EMA_ST:lo + Micro_VolOfVol:hi + AT_Regime_ST:==1                    B
  53  Sqz_Val:hi + Sqz_State:==1 + RangeOsc_State:==-2                          B
  54  Volume_Ratio_10:hi + AT_Regime_ST:==1 + OR_Low_Side:==-1                  B
  55  Volume_Avg_10:hi + Micro_FailedBreak:hi + Micro_Rejection:lo              B+D
  56  D2D_ATR:hi + Micro_Amihud:hi + AT_Regime_ST:==1                           D
  57  D2D_ATR_MA:hi + AT_Lookback_ST:hi + Micro_Lambda:lo                       D
  58  Efficiency_Ratio:hi + Micro_RangeAccel:lo + RangeOsc_State:==2            D
  59  KAMA_Dist:hi + D2D_Signal:==1 + Harmonic_D2D_Concordance:==0              D
  60  Micro_Entropy:lo + Micro_WickImbalance:hi + TChan_A15:hi                  D
  61  Micro_FailedBreak:lo + Micro_Hurst:lo + OR_Position:lo                    D
  62  Micro_GarmanKlass:hi + Micro_TickIntensity:hi + MultiDay_Slope:lo         D
  63  Micro_Lambda:lo + VWAP_Dist_ATR:lo + TChan_A15:hi                         D
  64  Micro_LogReturn:lo + Upper_Wick:hi + OR_High_Side:==0                     D
  65  Micro_MicroGap:lo + AT_Regime_ST:==1 + OR_High_Side:==-1                  D
  66  Micro_MomoTransfer:hi + Micro_RangeAccel:lo + PrevDay_High_Side:==-1      D
  67  Micro_OrderFlowDelta:hi + Micro_Rejection:lo + PrevDay_High_Side:==-1     D
  68  OBV_Macd:lo + OBV_Velocity:hi + WeeklyOpen_Side:==-1                      D
  69  Slope_Accel_LT:hi + Micro_AutoCorr:lo + Micro_Rejection:lo                D
  70  Slope_Accel_LT:hi + Micro_VPIN:lo + Harmonic_OBVf_Concordance:==0         D
  71  Slope_EMA_ST:hi + Efficiency_Ratio:lo + OBVf_Signal:==-1                  D
  72  Sqz_Val:lo + Micro_TickIntensity:hi + Round_100_Dist_ATR:lo               D
  73  Sqz_Val:lo + Momentum_Value:lo + MultiDay_Position:hi                     D
  74  VWAP_Dist_ATR:lo + AT_Regime_LT:==1 + OR_High_Side:==1                    D
--- SHORT (42 signals) ---------------------------------------
   1  AT_Slope_ST:hi + Micro_BarEntropy:lo + Sqz_State:==-1                     B
   2  D2D_Dynamic_Sensitivity:lo + Micro_ThrustEff:lo + Micro_WickImbalance:lo  B
   3  DailyOpen_Dist_ATR:lo + Harmonic_OBVf_Concordance:==1 + VAH_Side:==0      B
   4  Micro_Amihud:hi + Micro_VolOfVol:lo + OR_Low_Side:==-1                    B
   5  Micro_BarEntropy:hi + ADX_Rising:==1 + VAH_Side:==0                       B
   6  Micro_BarEntropy:lo + Micro_Hurst:hi + Session_High_Dist_ATR:lo           B
   7  Micro_BarOverlap:hi + Micro_HLAsymmetry:lo + PrevDay_High_Side:==1        B
   8  Micro_Entropy:lo + Micro_WickImbalance:lo + Lower_Wick:lo                 B
   9  Micro_MomoTransfer:hi + Micro_RangeAccel:lo + OBVf_Signal:==1             B
  10  Micro_PriceAccel:hi + Micro_RangeVelocity:hi + PoC_Side:==0               B
  11  Micro_RangeVelocity:lo + Lower_Wick:lo + AT_Regime_ST:==0                 B
  12  Micro_Rejection:hi + Body_Size:hi + PrevDay_High_Side:==1                 B
  13  Momentum_Value:hi + Micro_TickIntensity:hi + Session_High_Dist_ATR:hi     B
  14  Momentum_Value:hi + Session_High_Dist_ATR:hi + OR_Position:lo             B
  15  Momentum_Value:hi + Session_High_Dist_ATR:hi + TChan_A15:hi               B
  16  OBVf_DirStepCount:hi + VAL_Side:==0 + WeeklyOpen_Side:==-1                B
  17  Slope_Accel_LT:lo + Micro_OrderFlowDelta:hi + PoC_Side:==0                B
  18  Slope_Accel_ST:hi + MultiDay_Position:hi + RangeOsc_State:==2             B
  19  Slope_Accel_ST:hi + WeeklyOpen_Dist_ATR:hi + TChan_A15:hi                 B
  20  Slope_EMA_ST:lo + Slope_Accel_ST:hi + ADX_Value:lo                        B
  21  Sqz_Val:hi + Micro_OrderFlowDelta:hi + Session_High_Dist_ATR:hi           B
  22  Sqz_Val:hi + Micro_TickIntensity:hi + Session_High_Dist_ATR:hi            B
  23  Sqz_Val:hi + Momentum_Value:hi + Session_High_Dist_ATR:hi                 B
  24  Sqz_Val:hi + OR_Position:hi + RangeOsc_State:==2                          B
  25  Sqz_Val:hi + PrevDay_Close_Dist_ATR:hi + Volume:hi                        B
  26  Sqz_Val:hi + Volume_Avg_10:hi + PrevDay_Close_Dist_ATR:hi                 B
  27  VA_Position:hi + ADX_Value:lo + Sqz_State:==-1                            B
  28  AT_Slope_ST:hi + Micro_Lambda:hi + PrevDay_High_Side:==-1                 D
  29  Bar_Range:hi + Micro_Amihud:hi + Micro_VolOfVol:lo                        D
  30  D2D_ATR_MA:hi + Body_Size:lo + PrevDay_High_Side:==1                      D
  31  DailyOpen_Dist_ATR:hi + Body_Size:lo + Volume:hi                          D
  32  Efficiency_Ratio:hi + Micro_CSSpread:lo + Body_Size:hi                    D
  33  Micro_FailedBreak:lo + Micro_HLAsymmetry:hi + Micro_VolAccel:hi           D
  34  Micro_GarmanKlass:hi + Micro_HLAsymmetry:hi + Lower_Wick:lo               D
  35  OBV_Velocity:hi + Micro_OrderFlowDelta:hi + Sqz_State:==-1                D
  36  Slope_Accel_LT:lo + Micro_Entropy:lo + AT_Regime_LT:==0                   D
  37  Slope_Accel_LT:lo + Micro_HLAsymmetry:hi + DailyOpen_Dist_ATR:hi          D
  38  Slope_Accel_ST:hi + Micro_AutoCorr:hi + ADX_Value:lo                      D
  39  Slope_Accel_ST:hi + Micro_BarEntropy:lo + VA_Position:hi                  D
  40  Slope_Accel_ST:hi + Micro_MicroGap:lo + Micro_MomoTransfer:lo             D
  41  Slope_EMA_ST:lo + VAH_Side:==0 + PrevDay_Low_Side:==1                     D
  42  Volume_Ratio_10:hi + VWAP_Sigma:hi + RangeOsc_State:==-2                  D

================================================================================
5. WHICH GATES ARE LIVE AT L3/S3
================================================================================

  gate    pass rate on frame    pass rate on ADMITTED entry bars    status
  --------------------------------------------------------------------------
  HU90            9.7478%                    29.95%                LIVE
  FB20           80.1874%                    93.15%                LIVE, SOFT
  ATS90           6.2217%                    23.10%                LIVE

  SHORT d4 and SHORT d5+ ARE UNGATED and carry 59 and 46 entry bars - 27% of
  the book's bars run through no tier gate at all.
  FB20 admits 93% of the bars it sees. It is the softest constant in the system
  and it has no null behind it anywhere in the record.

================================================================================
6. PERFORMANCE AND RISK POSTURE
================================================================================

6.1 DEPTH LADDER - 1.0 LOT

  cell        trades   bars   loss ev      WR        net
  ---------------------------------------------------------
  LONG  d3       138     46         3   93.48%    $7,651
  LONG  d4       148     37         1   97.30%    $7,080
  LONG  d5+    1,195    179         9   95.82%   $69,980
  SHORT d3        81     27         3   88.89%    $2,988
  SHORT d4       236     59         3   94.92%    $5,692
  SHORT d5+      267     46         2   95.88%    $7,259

  LONG d5+ carries 70% of net on 45% of bars.
  Every per-cell event count is BELOW 20. All are COUNTS; no cell rate is quoted.

6.2 EXPOSURE
  peak open positions 31 | peak at-risk 21 | p99 at-risk 12
  bars exceeding the cap: 0. The jar never binds on this book.

6.3 LOSING PERIODS
  0 losing weeks of 26. 0 losing months. 6 losing days of 92.

================================================================================
7. OUT-OF-SAMPLE AND THE FRONTIER
================================================================================

7.1 WALK-FORWARD - all windows, 1.0 lot, zero losing weeks in each

  window                bars   loss ev   PF      MARGIN
  ------------------------------------------------------
  W1  bars 6,900-63k     192        11   10.46    28.37
  W2  bars 63k-110k       69         3   20.79    44.62
  W3  bars 110k-157k     102         6   10.46    33.22
  JULY holdout            31         1     -        -
  July: 11 trading days, 1 loss event, worst day +$80.50. A COUNT OF ONE.

7.2 SPLIT-HALF
  Select terrain on half A only, score on half B: MARGIN 34.46, 9 loss events,
  49 days, worst day -$612.
  Terrain membership shares only 15 of 35 between half A and full train.
  MEMBERSHIP IS UNSTABLE AND THE SCORE IS STABLE. That is the property this
  project measured nine times as a failure; here it is on the paying side.

7.3 THE FLOOR FRONTIER - the operator picks the point

  floor    bars   loss ev   lw   days    PF     MARGIN      net
  ----------------------------------------------------------------
  L3/S3     394        21    0     92   13.25    34.38   $100,652   <- ADOPTED
  L5/S3     316        17    0     85   15.46    37.84    $88,420
  L7/S4     188         7    0     68   34.60    47.12    $61,489

  WHY L3/S3: days is the operator's primary agenda and this is the widest
  participation that holds zero losing weeks on this book. L7/S4 is a better
  book per trade and trades 24 fewer days.

7.4 THE PARTICIPATION FRONTIER - other cells, same procedure, larger D target

  config              n   loss ev   lw   days   worst day   MARGIN      net
  ---------------------------------------------------------------------------
  T20/15 B<2 L3/S3  116        21    0     92    -$451.90    34.38   $100,652
  T35/25 B<2 L3/S3  139        27    0    104    -$563.50    33.36   $133,710
  T60/45 B<2 L3/S3  175        44    0    119    -$825.60    28.09   $182,322
  T60/45 B<2 L4/S3  175        36    1    115    -$612.00    31.21   $178,278

  T60/45 L3/S3 reaches 119 days with zero losing weeks at double the events and
  double the tail. IT IS NOT BATTERY-TESTED. Only the adopted cell is.

================================================================================
8. THE BATTERY, AND WHAT THE SYSTEM CANNOT SEE
================================================================================

8.1 THE BATTERY - RUN

  RANDOMISATION  500 size-matched draws (n=116) from the VALID field, gates and
                 floor identical. RESOLUTION FLOOR 1/(n+1) = 0.0020.
    MARGIN        34.38 vs random median 26.17   beats 499/500   p = 0.0020
    loss events      21 vs 40                    beats 498/500   p = 0.0040
    worst bar     -$918 vs -$1,530               beats 483/500   p = 0.0339
    losing weeks      0 vs 2                     beats 498/500   p = 0.0040
    Random margin range 20.90 - 35.22. Only 2 of 500 draws hold zero losing
    weeks; this book does.
    p IS BOUNDED BELOW BY 1/(n+1). MARGIN RETURNED THE FLOOR - the best value
    the test can produce. The other three sit at 2x the floor and CANNOT GO
    LOWER AT THIS DRAW COUNT. That is resolution, not failure.

  SPLIT-HALF     PASSED - 7.2.
  WALK-FORWARD   PASSED, three windows - 7.1.
  JULY           1 loss event on 11 days, worst day positive - 7.1.
  ANTI-SYSTEM    1,702 excluded signals on the 394 bars this book never touches:
                   full 1,702      799 events   PF 1.67   MARGIN  7.59
                   pruned PF>=4      234 events   PF 1.99   MARGIN 10.39
                   pruned PF>=8        6 events   PF 6.08   MARGIN 30.79 (COUNT)
                   THIS BOOK          21 events   PF 13.25  MARGIN 34.38
                 The best pruned arm reaches PF 6.08 against 13.25 and carries
                 FOUR losing weeks against zero. THE EDGE IS IN THE SELECTION.

8.2 OBJECTIVES TESTED AND REJECTED - do not re-propose

  CO-FIRE AFFINITY   Split-half rho +0.4892 (p = 9.6e-240) - the statistic
                     GENERALISES. Every objective built on it fails:
                     maximised -> near-duplicates (internal Jaccard 5-12x pool);
                     ceiling-constrained -> anti-coupled sets BELOW pool baseline
                     that lose to random at every ceiling 0.02-0.20.
                     Alone: 60 events on 120 signals, 5 losing weeks, -$3,213.
  SOLO PERSISTENCE   Zero losing days+weeks+months on the train window admits
  (objective E)      0 of 1,818. Relaxed to <=1 losing week: 57 signals, margin
                     25.09, TWO losing weeks - beats 1 of 3 random draws.
                     57 signals each losing at most one week produced a book
                     with two. Solo persistence is not a book property.
  LOSS-DAY DECORR    Excluded from THIS system: across K=30..70 its margin runs
  (objective A)      43.57 -> 24.93 and every point above K=30 sits below this
                     book's 34.38. It is not refuted; it is not additive here.
  TERRAIN AS A       Spearman with loss events -0.184 (p=0.663); correlates with
  CONSTRAINT         a DEEPER worst day +0.764 (p=0.027). Different object from
                     terrain as an OBJECTIVE, which passed.
  L2/S2 FLOOR        21 loss events -> 201. A floor of 2 on 116 signals admits
                     nearly every bar where any pair agrees.
  E_dir < 3          All 8 cells: worst day -$1,760 to -$2,261.

8.3 WHAT THE SYSTEM CANNOT SEE

  - THE 14 ORPHANS. 14 of the incumbent's 297 appear in no scanner output
    (6 LONG / 8 SHORT). MIN_TRADES=10 never emitted them. NO SCAN-BASED
    PROCEDURE CAN REACH THEM. If they matter, this procedure cannot find out.
  - NON-F0 FAMILIES. F1/F2/F3/F4/F9/F11/F13 were not searched by this procedure.
  - THE GATES ARE NOT DERIVED FOR THIS BOOK. They were derived on a 297-signal
    book with 42 loss events. This book has 21. Deriving gates on 21 events
    starves every cell; I did not attempt it and the inherited stack may be
    wrong for this book in ways I cannot measure at this event count.
  - THE CAP IS NOT DERIVED FOR THIS BOOK. Peak at-risk is 21 and the jar never
    binds, so the cap is not currently a constraint - but it was located
    elsewhere and no sweep was run here.
  - JUNE. The incumbent's documented June weakness was not re-examined on this
    book. June is its second-weakest month at $11,965.

8.4 THE OPEN RECONCILIATION

  Objective A, 120 signals, VALID field, four greedy rules confirmed identical
  between seats (cumulative boolean, EXIT-date days, net>0 pool, seed
  argmax(net-50*ld)):
      MANAGER   23 loss events   worst day -$444   PF 10.99   1 losing week
      QUANT     31 loss events   worst day -$829   PF  7.58   0 losing weeks
  At K=45 I measure 16 events / -$918 / PF 9.17 / 2 losing weeks against a
  reported 15 / -$341 / 10.99 / 1.
  A(K=45) is a strict subset of A(70L/50S) - 90 of 90 - so the greedies agree
  and the SEED AND COVERAGE RULES ARE NOT THE DIVERGENCE.
  THE REMAINING CANDIDATE IS THE LOSS-DAY MATRIX BUILD. Mine is in 3.3.

================================================================================
9. TRIAL COUNT
================================================================================

  THE HONEST NUMBER, not the final table.

  Selection-objective work this session, scored configurations:
    grid cells (terrain x threshold x floor)                  24
    objective A cap sweep                                     12
    objective A target-size sweep                              5
    B threshold sweeps (family-priced and per-direction)       8
    terrain alone + controls                                   5
    objective E + controls                                     6
    parents/union/region/full-universe/other                  ~90
    randomisation draws (500 + 400 + 6)                      906
  ----------------------------------------------------------------
    SCORED CONFIGURATIONS EXCLUDING RANDOM DRAWS             ~150
    TOTAL ENGINE SCORINGS                                  ~1,056

  The adopted cell was chosen from a 24-cell grid. Bonferroni at 24 requires
  p < 0.0021 and margin returned p = 0.0020. THAT IS A PASS AT THE RESOLUTION
  FLOOR AND NOTHING MORE. It is the single number a reader should attack first.
