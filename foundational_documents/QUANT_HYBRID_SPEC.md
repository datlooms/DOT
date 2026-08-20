# QUANT_HYBRID_SPEC.md
# A30 + B at L6/S4 — three objectives reduced to two, on a corrected loss-day matrix
# Field: catalogue_F0.csv VALID, 1,818 rows. Frame 177,251 bars, 2026.01.19–2026.07.21.
# EVERY FIGURE AT 1.0 LOT. No max-lot column anywhere.
# ORDERING THROUGHOUT: loss events -> tail -> persistence -> days -> net LAST.

================================================================================
0. WHAT WOULD MAKE THIS SPECIFICATION WRONG
================================================================================

0.1  THE CORRECTION THAT PRODUCED THIS DOCUMENT

EVERY OBJECTIVE-A FIGURE IN `QUANT_SYSTEM_SPEC.md` WAS MEASURED ON A CONTAMINATED
MATRIX. I built the solo loss-day matrix in batches of 120 at cap 21. THE JAR BINDS
INSIDE A BATCH, so a signal's "solo" losing days depended on which 119 other signals
shared its batch. The matrix was never solo.

  measured: the 120-batch deleted 2,324 trades, 1.2% of solo volume.
  effect at K=45, scored identically at L3/S3 cap 21:

    matrix                  events   worst day   losing wks   PF
    -------------------------------------------------------------
    batch-120 cap 21 (old)      16      -$918          2      9.17
    true-solo unbounded         12      -$612          1     13.90

THE CLEAN MATRIX WINS ON EVERY AXIS AND REPRODUCES THE MANAGER'S FIGURES.
CONSEQUENCE: my §8.2 exclusion of objective A — "it costs margin faster than it buys
days" — WAS MADE ON CONTAMINATED DATA AND IS WITHDRAWN. A is the backbone of this book.

CORRECT BUILD: MAX_POSITIONS high enough that the jar never binds. Batch size is then
irrelevant. 78 seconds for 1,818 signals.

0.2  THE CLAIMS THAT COULD FAIL, AND THE FALSIFIER FOR EACH

1.  THE SPLIT-HALF IS THE SOFTEST RESULT IN THIS DOCUMENT AND IT DEGRADES MATERIALLY.
    Select A on half A, score on half B: MARGIN 30.71, 12 loss events, ONE LOSING WEEK,
    worst day -$1,224 against the full-frame -$391.
    MY PRIOR SYSTEM B+D HELD AT 34.46 WITH ZERO LOSING WEEKS AND -$612.
    FALSIFIER: a second independent split. IF THE LOSING WEEK AND THE TRIPLED WORST DAY
    REPRODUCE, THIS BOOK IS FITTED TO THE FULL FRAME AND B+D IS THE BETTER SYSTEM.
    This is the first thing a reader should attack.

2.  THE RANDOMISATION IS 100 DRAWS, NOT 500. Resolution floor 1/(n+1) = 0.0099. At a
    cumulative trial count of 24 + 12 cells the corrected threshold is tighter than
    0.0099 supports. DIRECTIONALLY DECISIVE AND FORMALLY UNDERPOWERED.
    FALSIFIER: extend to 500. My prior system got 500 draws and this one did not.

3.  NO ANTI-SYSTEM WAS RUN ON THIS BOOK. B+D cleared it — best pruned arm PF 6.08
    against 13.25, four losing weeks against zero. THIS BOOK IS UNTESTED ON THE ITEM
    THAT SEPARATES "the selection has an edge" from "the field is rich."
    FALSIFIER: run it. 1,680 excluded signals on the 335 bars this book never touches.

4.  THE GATES ARE INHERITED AND THREE OF SIX CELLS ARE DEAD CODE — see §5.
    They were derived on a 297-signal book with 42 loss events. This book has 14.
    Deriving gates on 14 events starves every cell; I did not attempt it.

5.  JULY IS ONE LOSS EVENT on 9 trading days. A COUNT. Every ratio on it, including
    PF 97.26 and MARGIN 67.07, is declined and must not be quoted.

6.  E_dir REBUILDS A PUBLISHED COLUMN. E_dir = n_dir x pf_null_exceedance_pct, taking
    exceedance from the catalogue. The 4,652-draw null is NOT recomputed per direction —
    only the multiplier is corrected. If a per-direction null moves SHORT exceedance,
    the 27 SHORT B-members are mispriced.

0.3  WHY L6/S4 — A DECISION, NOT A DEFAULT

THE BINDING CONSTRAINT IS THE LOSING WEEK, NOT THE EVENT COUNT. At K=30:

    floor     loss events   losing weeks   MARGIN
    ------------------------------------------------
    L3/S3            25          0          36.65
    L6/S4            14          0          45.85   <- ADOPTED
    L8/S5             6          1          49.13
    L10/S6            1          1          61.12

L8/S5 HAS FEWER EVENTS AND A HIGHER MARGIN AND IT LOSES A WEEK. The operator's ordering
puts persistence above performance, so L6/S4 is the deepest floor that holds zero.
I STOPPED THERE BY RULE, NOT BY ARGMAX.

================================================================================
1. HEADLINE — 1.0 LOT
================================================================================

  BOOK          138 signals   81 LONG / 57 SHORT
  FIELD         catalogue_F0.csv VALID, 1,818 rows
  FLOOR         L6/S4      CAP 21      GATES inherited (see 5)

  LOSS EVENTS                  14
  worst bar              -$612.00
  worst day              -$391.10
  LOSING WEEKS            0 of 26
  losing months            0 of 7
  days traded           86 of 132
  trades                    1,608
  entry bars                  335
  WR                       97.14%
  PF                        29.86
  MARGIN                    45.85
  net                    $122,221

  MARGIN 45.85 IS THE HIGHEST MEASURED IN THIS PROJECT ON ANY BOOK AT ANY FLOOR.
  WORST DAY -$391.10 IS THE SHALLOWEST OF ANY DEPLOYABLE CELL.

  AGAINST THE OTHER THREE SYSTEMS
    system                  ev   worst bar   worst day   lw   days   MARGIN      net
    ---------------------------------------------------------------------------------
    A30+B L6/S4 (THIS)      14      -$612       -$391     0     86    45.85   $122,221
    QUANT B+D L3/S3         21      -$918       -$452     0     92    34.38   $100,652
    MANAGER A+B L8/S5       10      -$773       -$637     0     88    32.74   $105,611
    WHOLE DOT L3/S3         42    -$1,224       -$347     0    119    33.07   $284,974

  IT LOSES TO B+D ON DAYS (86 v 92). It loses to the Manager on loss events (14 v 10)
  and days (86 v 88). IT LOSES TO THE WHOLE DOT ON DAYS AND ON NET, DECISIVELY.

================================================================================
2. THE RULE, IN EXECUTION ORDER
================================================================================

STEP 1  FIELD. catalogue_F0.csv rows with verdict == VALID. 1,818 of 1,840.
        NEVER a sub-slice — decorrelation breaks on a filtered field
        (worst day -$341 -> -$3,213 measured on the raw scan).

STEP 2  SOLO LOSS-DAY MATRIX.  Score every signal ALONE.
        MAX_POSITIONS = 999999   the jar MUST NOT bind. See 0.1.
        ADM_FLOOR {1:1,-1:1} · no gates · no ATR floor
        Aggregate pnl by (signal, EXIT-bar date). losing_days = days with net < 0.
        Restrict to the train window: exit_bar <= config.holdout_boundary.
        This frame: N=1, boundary bar 157,408, train 2026.01–06.

STEP 3  OBJECTIVE A — LOSS-DAY DECORRELATION, PER DIRECTION.
        pool  = signals with net > 0 in the train window, this direction
        seed  = argmax( net - 50 * len(losing_days) )
        step  = argmin( ( |losing_days & covered| , -net ) )
        covered is a SET THAT ONLY GROWS — a day already spoiled is free to spoil
        again. Max-pairwise or sum-across-set give a different book.
        stop  = config.A_target per direction.  This frame: K = 30.
        RESULT: 60 signals, 30 LONG / 30 SHORT.

STEP 4  OBJECTIVE B — CHANCE-PRICING, PER DIRECTION.
        E_dir[i] = n_dir(dir of i) * pf_null_exceedance_pct[i]
          n_LONG 1,445 · n_SHORT 373 · K 4,652 nulls
          resolution floors n_dir/K: LONG 0.3106, SHORT 0.0802 — PRINT BOTH
        SELECT E_dir < config.B_threshold.  This frame: 2.
        RESULT: 82 signals, 55 LONG / 27 SHORT.
        DO NOT use family-priced E: n_trials_family 1,840 with long_share 0.7951
        prices every SHORT row against an 80%-LONG null.

STEP 5  UNION.  BOOK = A | B.  138 signals. Overlap 4.
        OBJECTIVE D (terrain coverage) IS NOT IN THIS BOOK — see 8.2.

STEP 6  EXECUTE. FLOORED admission, floor L6/S4, cap 21, gates per §3.2,
        recentfb_sizing = False, conviction stack as adopted.
        SELECTION CAP AND EXECUTION CAP ARE DIFFERENT BY DESIGN HERE: the matrix is
        built unbounded so it measures the SIGNAL; execution runs at 21 because that
        is the account. A microscope is not the size of the specimen.

STEP 7  SCORE and emit the floor frontier (§7.3), not a point.

================================================================================
3. THE CONSTANT REGISTRY
================================================================================

3.1  SELECTION

  name                value       source
  ---------------------------------------------------------------------------
  field               VALID       catalogue_F0.csv verdict column
  holdout months N    1           CONFIG. Moves survivor count ~28% N=1 vs N=2.
  A_target (K)        30          DERIVED this session. K=20 gives 19 events at
                                  1 losing week; K=45 gives 35 at 1; K=65 gives 41.
  B_threshold         2           DERIVED. E_dir<3 breaks the tail (8 cells at
                                  -$1,760 to -$2,261 worst day). REFUTED above 2.
  seed weight         50          INHERITED from the recorded snippet. NOT swept.
  greedy tie-break    min index   DERIVED. Deterministic; no RNG anywhere.

3.2  EXECUTION — ALL INHERITED, NONE DERIVED FOR THIS BOOK

  floor               L6/S4       DERIVED this session (§0.3). The one execution
                                  constant this document sets.
  cap                 21          INHERITED. Located on a 297-signal book.
  ATR_1M floor        20          INHERITED.
  tier = min(depth,5) 5           INHERITED, UNDOCUMENTED. Makes 3 cells dead (§5).
  recentfb_sizing     False       INHERITED.
  gate LONG d3        Micro_Hurst > p90                    DEAD CODE at L6/S4
  gate LONG d4        FailedBreak > p20 AND AT_Slope > p90 DEAD CODE at L6/S4
  gate LONG d5+       Micro_FailedBreak > p20              LIVE
  gate SHORT d3       Micro_Hurst > p90                    DEAD CODE at L6/S4
  gate SHORT d4/d5+   FREE                                 UNGATED — see 3.3

3.3  THE SOLO MATRIX PARAMETERS — THE RECONCILIATION, CLOSED

    MAX_POSITIONS   999999      jar never binds. THIS IS THE CORRECTION (§0.1).
    ADM_FLOOR       {1:1,-1:1}
    ADM_GATES       None
    ADM_TIERGATES   None
    day key         EXIT bar date
    pool filter     net > 0, per direction
    seed            argmax(net - 50 * losing_days)
    coverage        len(losing_days & covered), covered grows only

  The Manager reached the same figures with batch size 1 at cap 6. Batch 1 and
  unbounded cap are equivalent: both prevent the jar binding. MINE AT BATCH 120 /
  CAP 21 DID NOT, AND THAT WAS THE ENTIRE DIVERGENCE.

3.4  GATE MASK SEMANTICS
  swept_thresholds.build_whole_dot_gates(df) substitutes dots_thresholds._D_SPEC,
  calls the sacred compute_adaptive_thresholds, restores in finally.
  CHECKSUMS on this frame (177,251 bars):
    HU90 9.7478%   FB20 80.1874%   ATS90 6.2217%
  A mask near 20% where HU90 belongs means the p80 series is live. ABORT.

================================================================================
4. THE FULL SIGNAL LISTING
================================================================================

4.1  PROVENANCE.  A only 56 · A and B 82 · TOTAL 138.
     Every B member is also an A member at this K — B is a strict subset of the
     union's B-tagged rows. 4 signals sit in both objectives' independent picks.

4.2  LISTING INTEGRITY. Fixed width. Longest signal_def 72 chars, longest line 83.
     140 rows. ANY COPY THAT WRAPS OR ELIDES IS CORRUPT.
     Source tag is A or A+B. A parser MUST whitelist those two tokens.

--- LONG (81 signals) ----------------------------------------
   1  ATR_1M:hi + Slope_Accel_LT:lo + Micro_RollProxy:hi                        A
   2  AT_Lookback_LT:hi + DailyOpen_Dist_ATR:lo + OR_High_Side:==-1             A
   3  AT_Lookback_ST:hi + AT_Slope_ST:lo + Micro_RollProxy:hi                   A
   4  AT_Score_LT:lo + Micro_MicroGap:lo + OR_Low_Side:==-1                     A
   5  AT_Score_LT:lo + Slope_Accel_LT:hi + Volume:hi                            A
   6  AT_Slope_ST:lo + EMA_Oscillator:lo + Lower_Wick:hi                        A
   7  AT_Slope_ST:lo + Micro_AutoCorr:hi + Micro_LogReturn:hi                   A
   8  AT_Slope_ST:lo + Micro_VolOfVol:hi + Round_500_Dist_ATR:lo                A
   9  Bars_Since_Flip:hi + Session_Low_Dist_ATR:lo + Volume:hi                  A
  10  DailyOpen_Dist_ATR:lo + OR_Position:lo + VWAP_Side:==1                    A
  11  Micro_GarmanKlass:hi + Micro_TickIntensity:hi + WeeklyOpen_Side:==-1      A
  12  Micro_IBSP:lo + VWAP_Sigma:lo + Volume:hi                                 A
  13  Micro_OrderFlowDelta:hi + Micro_Rejection:lo + Lower_Wick:lo              A
  14  Micro_RangeAccel:hi + PrevDay_High_Dist_ATR:lo + PrevDay_Low_Side:==-1    A
  15  Micro_Rejection:lo + PrevDay_Low_Dist_ATR:lo + AT_Regime_ST:==1           A
  16  Micro_Rejection:lo + VWAP_Sigma:lo + Volume:hi                            A
  17  OBV_Macd:lo + ADX_Value:hi + WeeklyOpen_Side:==1                          A
  18  OBV_Macd:lo + Harmonic_LLEMA:hi + Micro_WickImbalance:lo                  A
  19  OBV_Macd:lo + Micro_Hurst:hi + VWAP_Dist_ATR:lo                           A
  20  PrevDay_High_Dist_ATR:lo + PrevDay_Low_Side:==-1 + OR_Low_Side:==1        A
  21  Session_High_Dist_ATR:lo + Upper_Wick:hi + D2D_Signal:==1                 A
  22  Session_Low_Dist_ATR:lo + OR_Position:hi + DailyOpen_Side:==-1            A
  23  Slope_EMA_ST:lo + Micro_HLAsymmetry:hi + Micro_Rejection:lo               A
  24  Slope_EMA_ST:lo + Micro_VolOfVol:hi + Harmonic_D2D_Concordance:==0        A
  25  Slope_EMA_ST:lo + Momentum_Value:hi + Micro_RangeVelocity:hi              A
  26  Volume_Avg_10:hi + DailyOpen_Side:==1 + OR_Low_Side:==-1                  A
  27  AT_Score_LT:lo + Slope_Accel_LT:hi + Body_Size:hi                         A+B
  28  AT_Score_LT:lo + Slope_Accel_LT:hi + Micro_RangeAccel:hi                  A+B
  29  AT_Score_LT:lo + Slope_Accel_LT:hi + VWAP_Sigma:hi                        A+B
  30  AT_Slope_LT:lo + AT_Slope_ST:lo + Micro_RollProxy:hi                      A+B
  31  AT_Slope_LT:lo + Micro_FailedBreak:hi + Micro_RangeAccel:lo               A+B
  32  AT_Slope_ST:lo + Micro_BarOverlap:lo + VWAP_Sigma:hi                      A+B
  33  AT_Slope_ST:lo + Micro_Hurst:hi + Round_1000_Dist_ATR:lo                  A+B
  34  AT_Slope_ST:lo + Micro_OrderFlowDelta:hi + Micro_WickImbalance:lo         A+B
  35  AT_Slope_ST:lo + Micro_RangeAccel:hi + RangeOsc_State:==2                 A+B
  36  AT_Slope_ST:lo + Micro_VPIN:lo + OBVf_Signal:==1                          A+B
  37  AT_Slope_ST:lo + Micro_VolOfVol:hi + OBVf_Signal:==1                      A+B
  38  AT_Slope_ST:lo + Momentum_Value:hi + Body_Size:hi                         A+B
  39  AT_Slope_ST:lo + Momentum_Value:hi + D2D_Signal:==0                       A+B
  40  AT_Slope_ST:lo + Momentum_Value:hi + Micro_RollProxy:hi                   A+B
  41  Bars_Since_Flip:hi + Micro_CSSpread:hi + Upper_Wick:hi                    A+B
  42  Bars_Since_Flip:hi + PrevDay_Low_Dist_ATR:lo + OR_Low_Side:==-1           A+B
  43  Bars_Since_Flip:hi + Volume_Avg_10:hi + MultiDay_Position:lo              A+B
  44  Bars_Since_Flip:hi + Volume_Avg_10:hi + Session_Low_Dist_ATR:lo           A+B
  45  D2D_ATR_MA:hi + Micro_VPIN:lo + ST_Flip_Event:==-1                        A+B
  46  Efficiency_Ratio:hi + Micro_RangeVelocity:lo + RangeOsc_State:==2         A+B
  47  KAMA_Dist:lo + Micro_WickImbalance:hi + ADX_Rising:==0                    A+B
  48  KAMA_Dist:lo + Micro_WickImbalance:hi + Harmonic_OBVf_Concordance:==0     A+B
  49  KAMA_Dist:lo + Micro_WickImbalance:hi + Lower_Wick:hi                     A+B
  50  KAMA_Dist:lo + Micro_WickImbalance:hi + Volume:hi                         A+B
  51  KAMA_Dist:lo + VWAP_Dist_ATR:lo + DailyOpen_Dist_ATR:lo                   A+B
  52  Micro_Amihud:hi + AT_Regime_ST:==1 + OR_Low_Side:==-1                     A+B
  53  Micro_Amihud:lo + Micro_RangeAccel:lo + Micro_VPIN:hi                     A+B
  54  Micro_AutoCorr:hi + Session_Low_Dist_ATR:lo + TChan_A15:hi                A+B
  55  Micro_GarmanKlass:hi + Micro_VolOfVol:hi + PrevDay_Close_Dist_ATR:hi      A+B
  56  Micro_LogReturn:hi + Micro_TickIntensity:hi + Micro_VolOfVol:hi           A+B
  57  Micro_MicroGap:lo + AT_Regime_ST:==1 + OR_Low_Side:==-1                   A+B
  58  Micro_MomoTransfer:hi + Micro_RangeVelocity:lo + AT_Regime_LT:==1         A+B
  59  Micro_PriceAccel:hi + VWAP_Sigma:hi + D2D_Signal:==1                      A+B
  60  Micro_Rejection:lo + AT_Regime_ST:==1 + PrevDay_Low_Side:==-1             A+B
  61  Micro_VPIN:lo + Volume:hi + ST_Flip_Event:==-1                            A+B
  62  Micro_WickImbalance:hi + Lower_Wick:lo + AT_Regime_LT:==1                 A+B
  63  Momentum_Value:hi + Micro_Hurst:hi + AT_Regime_ST:==1                     A+B
  64  OBV_Macd:lo + Harmonic_LLEMA:hi + Micro_Rejection:lo                      A+B
  65  OBV_Macd:lo + Micro_OrderFlowDelta:lo + Micro_WickImbalance:hi            A+B
  66  OBV_Macd:lo + Micro_WickImbalance:lo + ADX_Value:hi                       A+B
  67  OBV_Macd:lo + Sqz_State:==1 + RangeOsc_State:==-2                         A+B
  68  OBV_Macd:lo + Volume_Ratio_10:hi + AT_Regime_ST:==1                       A+B
  69  OBV_Velocity:hi + Micro_FractalDim:hi + OR_Low_Side:==-1                  A+B
  70  RangeOsc_Val:hi + Session_Low_Dist_ATR:lo + PrevDay_Close_Side:==-1       A+B
  71  Slope_Accel_LT:hi + AT_Regime_ST:==1 + OR_Low_Side:==-1                   A+B
  72  Slope_Accel_LT:hi + OBV_Macd:lo + Micro_TickIntensity:hi                  A+B
  73  Slope_Accel_LT:hi + Sqz_State:==1 + RangeOsc_State:==-2                   A+B
  74  Slope_Accel_ST:lo + Micro_VPIN:lo + ST_Flip_Event:==-1                    A+B
  75  Slope_EMA_ST:hi + Micro_Rejection:lo + Session_Low_Dist_ATR:lo            A+B
  76  Slope_EMA_ST:lo + Micro_Hurst:hi + PrevDay_High_Side:==-1                 A+B
  77  Slope_EMA_ST:lo + Micro_RangeAccel:hi + Micro_Rejection:lo                A+B
  78  Slope_EMA_ST:lo + Micro_VolOfVol:hi + AT_Regime_ST:==1                    A+B
  79  Sqz_Val:hi + Sqz_State:==1 + RangeOsc_State:==-2                          A+B
  80  Volume_Avg_10:hi + Micro_FailedBreak:hi + Micro_Rejection:lo              A+B
  81  Volume_Ratio_10:hi + AT_Regime_ST:==1 + OR_Low_Side:==-1                  A+B
--- SHORT (57 signals) ---------------------------------------
   1  ATR_1M:hi + DailyOpen_Dist_ATR:hi + ADX_Value:hi                          A
   2  ATR_1M:hi + OBV_Velocity:lo + PrevDay_High_Dist_ATR:hi                    A
   3  AT_Score_LT:hi + Volume_Avg_10:hi + RangeOsc_State:==-1                   A
   4  AT_Slope_ST:hi + Micro_WickImbalance:lo + Sqz_State:==-1                  A
   5  Bar_Range:hi + MultiDay_Slope:hi + RangeOsc_State:==2                     A
   6  Bar_Range:hi + Session_Low_Dist_ATR:hi + Harmonic_OBVf_Concordance:==1    A
   7  D2D_ATR:hi + D2D_Dynamic_Sensitivity:lo + Micro_VPIN:hi                   A
   8  D2D_ATR:hi + Micro_RangeAccel:lo + D2D_Signal:==-1                        A
   9  D2D_ATR:hi + Momentum_Value:hi + Session_High_Dist_ATR:hi                 A
  10  D2D_ATR:hi + Round_1000_Dist_ATR:lo + WeeklyOpen_Dist_ATR:hi              A
  11  D2D_ATR:hi + WeeklyOpen_Dist_ATR:hi + PrevDay_High_Side:==1               A
  12  D2D_ATR_MA:hi + D2D_Dynamic_Sensitivity:lo + Micro_VPIN:hi                A
  13  D2D_ATR_MA:hi + D2D_Persist:hi + Upper_Wick:hi                            A
  14  D2D_Dn_Count:hi + Volume_Avg_10:hi + Micro_VPIN:hi                        A
  15  D2D_Dynamic_Sensitivity:lo + AT_Slope_ST:lo + Micro_WickImbalance:lo      A
  16  D2D_Dynamic_Sensitivity:lo + Lower_Wick:lo + Volume:hi                    A
  17  D2D_Dynamic_Sensitivity:lo + Volume_Avg_10:hi + Efficiency_Ratio:hi       A
  18  DailyOpen_Dist_ATR:lo + TChan_A15:hi + VAH_Side:==-1                      A
  19  KAMA_Dist:hi + PrevDay_Low_Dist_ATR:lo + Session_High_Dist_ATR:lo         A
  20  Micro_Entropy:lo + Micro_WickImbalance:lo + VAL_Side:==0                  A
  21  Micro_Entropy:lo + TChan_A15:hi + VAL_Side:==0                            A
  22  Micro_GarmanKlass:hi + WeeklyOpen_Dist_ATR:hi + ADX_Value:hi              A
  23  Micro_GarmanKlass:hi + WeeklyOpen_Dist_ATR:hi + PrevDay_High_Side:==1     A
  24  Micro_HLAsymmetry:lo + Micro_LogReturn:hi + MultiDay_Slope:hi             A
  25  OBVf_DirStepCount:lo + Sqz_Val:lo + Micro_HLAsymmetry:lo                  A
  26  OR_High_Dist_ATR:hi + VWAP_Z:lo + OBVf_Signal:==1                         A
  27  Slope_Accel_ST:hi + VA_Position:hi + VWAP_Side:==-1                       A
  28  Slope_Accel_ST:lo + Volume_Avg_10:hi + VAL_Side:==-1                      A
  29  Sqz_Val:hi + Micro_TickIntensity:hi + PrevDay_Low_Side:==-1               A
  30  Sqz_Val:hi + Round_500_Dist_ATR:lo + RangeOsc_State:==2                   A
  31  AT_Slope_ST:hi + Micro_BarEntropy:lo + Sqz_State:==-1                     A+B
  32  D2D_Dynamic_Sensitivity:lo + Micro_ThrustEff:lo + Micro_WickImbalance:lo  A+B
  33  DailyOpen_Dist_ATR:lo + Harmonic_OBVf_Concordance:==1 + VAH_Side:==0      A+B
  34  Micro_Amihud:hi + Micro_VolOfVol:lo + OR_Low_Side:==-1                    A+B
  35  Micro_BarEntropy:hi + ADX_Rising:==1 + VAH_Side:==0                       A+B
  36  Micro_BarEntropy:lo + Micro_Hurst:hi + Session_High_Dist_ATR:lo           A+B
  37  Micro_BarOverlap:hi + Micro_HLAsymmetry:lo + PrevDay_High_Side:==1        A+B
  38  Micro_Entropy:lo + Micro_WickImbalance:lo + Lower_Wick:lo                 A+B
  39  Micro_MomoTransfer:hi + Micro_RangeAccel:lo + OBVf_Signal:==1             A+B
  40  Micro_PriceAccel:hi + Micro_RangeVelocity:hi + PoC_Side:==0               A+B
  41  Micro_RangeVelocity:lo + Lower_Wick:lo + AT_Regime_ST:==0                 A+B
  42  Micro_Rejection:hi + Body_Size:hi + PrevDay_High_Side:==1                 A+B
  43  Momentum_Value:hi + Micro_TickIntensity:hi + Session_High_Dist_ATR:hi     A+B
  44  Momentum_Value:hi + Session_High_Dist_ATR:hi + OR_Position:lo             A+B
  45  Momentum_Value:hi + Session_High_Dist_ATR:hi + TChan_A15:hi               A+B
  46  OBVf_DirStepCount:hi + VAL_Side:==0 + WeeklyOpen_Side:==-1                A+B
  47  Slope_Accel_LT:lo + Micro_OrderFlowDelta:hi + PoC_Side:==0                A+B
  48  Slope_Accel_ST:hi + MultiDay_Position:hi + RangeOsc_State:==2             A+B
  49  Slope_Accel_ST:hi + WeeklyOpen_Dist_ATR:hi + TChan_A15:hi                 A+B
  50  Slope_EMA_ST:lo + Slope_Accel_ST:hi + ADX_Value:lo                        A+B
  51  Sqz_Val:hi + Micro_OrderFlowDelta:hi + Session_High_Dist_ATR:hi           A+B
  52  Sqz_Val:hi + Micro_TickIntensity:hi + Session_High_Dist_ATR:hi            A+B
  53  Sqz_Val:hi + Momentum_Value:hi + Session_High_Dist_ATR:hi                 A+B
  54  Sqz_Val:hi + OR_Position:hi + RangeOsc_State:==2                          A+B
  55  Sqz_Val:hi + PrevDay_Close_Dist_ATR:hi + Volume:hi                        A+B
  56  Sqz_Val:hi + Volume_Avg_10:hi + PrevDay_Close_Dist_ATR:hi                 A+B
  57  VA_Position:hi + ADX_Value:lo + Sqz_State:==-1                            A+B

================================================================================
5. WHICH GATES ARE LIVE AT L6/S4 — THREE OF SIX ARE DEAD CODE
================================================================================

  cell          trades   gated?   status
  -----------------------------------------------------------------
  LONG  tier 3       0   GATED    DEAD CODE — unreachable at floor 6
  LONG  tier 4       0   GATED    DEAD CODE — unreachable at floor 6
  LONG  tier 5   1,160   GATED    LIVE — Micro_FailedBreak > p20
  SHORT tier 3       0   GATED    DEAD CODE — unreachable at floor 4
  SHORT tier 4     448   free     UNGATED
  SHORT tier 5     512   free     UNGATED

  WITH tier = min(depth,5) AND A FLOOR OF 6/4, EVERY LONG TRADE LANDS IN TIER 5 AND
  EVERY SHORT TRADE IN TIER 4 OR 5. THE SYSTEM REDUCES TO ONE LIVE GATE.
  Micro_Hurst > p90 — the only CONFIRMED gate in the project — FIRES ZERO TIMES.
  Same shape as v3's L7/S4 finding. A specification that lists unreachable gates is
  the defect v1 had, and this section exists so it is not repeated.

  AND 960 OF 1,608 TRADES — 60% — RUN THROUGH NO TIER GATE AT ALL.
  That is the largest ungated surface in any of the four systems. See 6.2.

================================================================================
6. PERFORMANCE AND RISK POSTURE
================================================================================

6.1  DEPTH LADDER — all cells carry fewer than 20 loss events. ALL COUNTS, NO RATES.
     LONG tier 5+ 1,160 trades · SHORT tier 4 448 · SHORT tier 5+ 512.

6.2  THE SHORT-SIDE GATE — MEASURED, NOT ADOPTED

  gate on SHORT d4/d5+          ev   worst day   lw   days   MARGIN      net
  --------------------------------------------------------------------------
  none (adopted)                14     -$391      0     86    45.85   $122,221
  Micro_Rejection < p50         9      -$290      0     77    48.01   $114,959
  Micro_FailedBreak > p70       7      -$186      0     71    51.38    $94,273

  BOTH IMPROVE EVENTS, TAIL AND MARGIN. BOTH COST DAYS — 9 and 15.
  I ADOPT NEITHER, AND THE REASON IS THE OPERATOR'S ORDERING: days is the primary
  agenda and this book is already 33 days behind the Whole DOT. Spending 9 more to
  move margin from 45.85 to 48.01 is the wrong trade at this participation level.
  IF THE OPERATOR RANKS TAIL ABOVE DAYS, Micro_Rejection < p50 IS THE PICK — it is
  PRE-REGISTERED (the record's 98.6th-percentile finding, re-confirmed on this
  catalogue) and it closes 60% of the book's ungated surface.
  Micro_FailedBreak > p70 SCORES BETTER AND IS NOT PRE-REGISTERED. I decline it on
  the same doctrine the Manager applied. THE DEPARTURE WOULD COST 3.4 MARGIN POINTS
  OF UPSIDE AND IT IS VISIBLE HERE SO THE OPERATOR CAN OVERRULE IT.

6.3  LOSING PERIODS.  0 losing weeks of 26. 0 losing months of 7.

================================================================================
7. OUT-OF-SAMPLE AND THE FRONTIER
================================================================================

7.1  WALK-FORWARD — zero losing weeks in every window

  window              bars   loss ev   MARGIN      net
  ------------------------------------------------------
  W1  6,900–63k        146         5    49.80   $51,542
  W2  63k–110k          69         1    26.20   $21,920
  W3  110k–157k         95         7    42.93   $31,664
  JULY holdout          25         1    67.07   $17,095
  July: 9 trading days, 1 loss event, worst day +$66.40. A COUNT OF ONE.

7.2  SPLIT-HALF — THE SOFTEST RESULT. See 0.2 item 1.
  select A on half A, score on half B: MARGIN 30.71, 12 events, ONE LOSING WEEK,
  worst day -$1,224. IT SURVIVES AND IT DEGRADES.

7.3  THE FLOOR FRONTIER — the operator picks the point

  floor     bars   loss ev   losing wks   days   PF      MARGIN      net
  --------------------------------------------------------------------------
  L3/S3      475        25        0         96   15.83    36.65   $134,373
  L6/S4      335        14        0         86   29.86    45.85   $122,221  <- ADOPTED
  L8/S5      177         6        1         66   40.10    49.13    $93,558
  L10/S6      95         1        1         48  213.34    61.12    $67,780

  ZERO LOSING WEEKS HOLDS AT L3 AND L6 AND BREAKS AT L8. That is the stopping rule.
  L10/S6 rests on ONE loss event; its PF of 213.34 is arithmetic, not evidence.

================================================================================
8. THE BATTERY, AND WHAT THE SYSTEM CANNOT SEE
================================================================================

8.1  THE BATTERY

  RANDOMISATION   RUN — 100 size-matched draws, n=138, VALID field, floor and gates
                  identical. RESOLUTION FLOOR 1/(n+1) = 0.0099.
                    MARGIN       45.85 vs rnd med 30.26      beats 100/100  p=0.0000
                    worst day  -$391.10 vs -$1,689.85        beats 100/100  p=0.0000
                    loss events     14 vs 23                 beats  98/100  p=0.0198
                    losing weeks     0 vs 2                  beats  98/100  p=0.0198
                  DIRECTIONALLY DECISIVE AND FORMALLY UNDERPOWERED — see 0.2 item 2.
  SPLIT-HALF      RUN — 7.2. PASSES AND DEGRADES.
  WALK-FORWARD    RUN — 7.1. Three windows, zero losing weeks each.
  JULY            RUN — 7.1. One loss event. A count.
  ANTI-SYSTEM     ** NOT RUN. ** See 0.2 item 3.
  GATE DERIVATION ** NOT RUN. ** 14 loss events starves every cell.

8.2  OBJECTIVES TESTED AND REJECTED — do not re-propose

  TERRAIN (D)        CLEARS ITS ALONE-GATE (21 events vs 31 and 39 on size-matched
                     controls) AND IS STILL EXCLUDED. A30+B+D at L6/S4: 29 events,
                     worst day -$1,132, ONE losing week, 99 days. D buys 13 days and
                     costs the tail and the clean week. THE THREE-OBJECTIVE FUSION
                     WAS RUN AND IT LOSES TO THE TWO.
  CO-FIRE AFFINITY   Statistic generalises (split-half rho +0.4892, p=9.6e-240);
                     every objective built on it fails. Alone: 60 events, 5 losing
                     weeks, -$3,213 worst bar.
  SOLO PERSISTENCE   Zero losing days+weeks+months admits 0 of 1,818. At <=1 losing
                     week, 57 signals produced a book with TWO. Not a book property.
  L2/S2 FLOOR        21 loss events -> 201.
  E_dir < 3          All 8 cells: worst day -$1,760 to -$2,261.

8.3  WHAT THE SYSTEM CANNOT SEE

  - THE 14 ORPHANS. 14 of the Whole DOT's 297 appear in NO scanner output
    (6 LONG / 8 SHORT). MIN_TRADES=10 never emitted them. NO SCAN-BASED PROCEDURE
    CAN REACH THEM.
  - NON-F0 FAMILIES. F1/F2/F3/F4/F9/F11/F13 were not searched.
  - THE GATES AND THE CAP ARE NOT DERIVED FOR THIS BOOK.
  - JUNE. The Whole DOT's documented June weakness was not re-examined here.
  - AND THE 33-DAY GAP TO THE WHOLE DOT IS NOT CLOSED BY ANYTHING IN THIS DOCUMENT.
    86 days against 119. Every dial that buys days on this procedure costs the tail.

================================================================================
9. CUMULATIVE TRIAL COUNT
================================================================================

  ACROSS BOTH QUANT DOCUMENTS, NOT THE CELLS IN THE FINAL TABLE:

    QUANT_SYSTEM_SPEC.md                    ~150 scored configurations
    matrix reconciliation                      4
    hybrid floor x K sweep                    30
    short-gate and liveness                    3
    randomisation draws (500 + 400 + 100 + 6) 1,006
  --------------------------------------------------------------
    SCORED CONFIGURATIONS EXCLUDING DRAWS    ~187
    TOTAL ENGINE SCORINGS                  ~1,193

  The adopted cell was chosen from a 30-cell floor x K sweep on top of a 24-cell
  grid. THE RANDOMISATION THAT SUPPORTS IT IS 100 DRAWS WITH A 0.0099 FLOOR.
  THAT IS THE SINGLE NUMBER A READER SHOULD ATTACK FIRST, AND 0.2 ITEM 1 IS THE
  SECOND.
