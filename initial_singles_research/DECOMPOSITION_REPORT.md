# THE 297 DECOMPOSED — WHAT THE BOOK IS MADE OF, PER BAR
Quant seat · 2026-08-22 · repo HEAD `4ecd2ba0` (2026-08-22 14:09 +0700) · frame `46586cbb1671` (177,251 × 172) · oracle `518862bf19fb` · engine `7f66273011a2`
All figures at 1.0 lot, in-sample (Jan 19 – Jul 21). Cumulative trial count this report: **1** (one counterfactual engine run, §5c). Everything else is set arithmetic on data in hand.

## 0. CONTROL — reproduced to the cent before anything was measured
`python master.py --data data --workers 1 --out discovery/full --stage S8 --book engine/whole_dot_signals.csv` (236s)
```
[adm_engine] rule=FLOORED cap=21 floor={1: 3, -1: 3} tier-gates=4        <- read every run; CURRENT/6 line above it is the fork-parity check only
FORK PARITY (177,251 bars): sacred 4d656d88255b5417 | adm_engine(CURRENT) 4d656d88255b5417 -> IDENTICAL
GATE MASKS: HU90 9.7478%  FB20 80.1874%  ATS90 6.2217%
trades 5776 · WR 96.1% · PF 14.53 · net $284,974 · trade losses 224 · LOSS EVENTS 42 · loss days 35
worst bar -$1,224 · worst day -$346.60 · losing weeks 0 of 26 · days 119 of 132 · folds 6/6 min-fold PF 11.0
BOOK-50 canary (separate run, 37s): 3,101 tr · WR 90.6 · PF 4.81 · $97,675 — ENGINE INTACT
```
Skeleton reconciles to the same table: 973 bars, 5,776 trades, $284,974.00, 42 loss events.

**Masks:** `adm_engine.build_signal_masks` with `entry_ok` rebuilt verbatim from `run_portfolio` L208–215 (ADX ≥ 15, Volume > 50, not Friday-close, bar ≥ 6900; no ATR, no tier gate — those are admission). 297 × 177,251, 43,243 qualifying firings. **Qualifying depth Q** = book signals firing in the bar's direction, before admission. Admitted depth = trades on the bar.

**Parsing:** 297 × 3 = 891 slots, **177 distinct conditions from 105 base variables** (of 249 / 117). On every one of the 973 bars `distinct conditions == distinct variables` — structural: `V:hi` and `V:lo` cannot both be true on one bar, nor two `V:==N` values. The DISAGREE case the brief describes never occurs on a traded bar. Both levels are reported where they differ (book level); on bars they are one number.

---

## 1. THE SKELETON — `skeleton.csv`, one row per traded bar
Columns: time · direction · depth_q · admissible_nA (Q minus signals already in a trade) · admitted · capped · slots · distinct_conds · distinct_vars · conds/slot · vars/slot · e^H (conditions, variables) · bar P&L · loss_event · cond_set · sig_set · ADX · ATR · Volume · Micro_Hurst · Micro_FailedBreak · D2D_Trend_Dir · AT_Slope_ST · exp_conds_random · exp_vars_random.

```
Q distribution   min 3 · p25 4 · median 5 · p75 7 · mean 6.89 · max 47        LONG 601 bars / SHORT 372
admitted == Q on 798 bars; admitted < Q on 175 (18.0%): 36 capped (admitted < nA), 139 reduced only by signals already in a trade
mean Q 6.89 vs mean admitted 5.94 · bars with Q >= 10: 140, with admitted >= 10: 95
Q band:   3    4    5    6-7  8-9  10-12 13-16 17-22 23-30 31-47
admitted 3.0  4.0  4.8  6.0  7.4   9.2   9.9  16.1  15.5  17.6
```
The trade table undercounts depth from Q≈5 upward; at Q 13–16 it shows 9.9. Every table below is on Q.

---

## 2. THE KILLING PLOT — depth vs distinct conditions, 973 bars

`randC` = exact expectation of distinct conditions if Q signals were drawn uniformly from the same-direction book (enumerated hypergeometric, not sampled). `slope` = OLS within band; `marg` = change per +1 depth between consecutive depths.

```
ALL 973         bars  meanQ  slots  distinct  c/slot   e^H   randC   obs/rand   slope-in-band
  3               94   3.0    9.0     7.2    0.801    6.8    8.7     0.82        —
  4              256   4.0   12.0     9.1    0.754    8.4   11.5     0.79        —
  5              210   5.0   15.0    10.5    0.703    9.5   14.1     0.75        —
  6              116   6.0   18.0    12.3    0.684   10.9   16.6     0.74        —
  7               78   7.0   21.0    13.6    0.650   11.9   19.1     0.71        —
  8-9             79   8.3   24.9    15.5    0.621   13.1   22.1     0.70       1.72
  10-12           55  10.8   32.5    18.7    0.576   15.4   27.5     0.68       1.89
  13-16           28  14.5   43.6    22.1    0.511   17.2   35.1     0.63      -0.03
  17-22           23  19.0   57.1    25.7    0.451   18.7   43.0     0.60       0.99
  23-30           22  25.8   77.5    31.8    0.412   22.5   53.6     0.59       0.84
  31-45           10  34.6  103.8    39.0    0.377   27.1   65.0     0.60       0.92
  46-47            2  47.0  141.0    47.5    0.337   31.3   78.6     0.60        —

marginal distinct-per-depth, consecutive depths (ALL):
  3->4 1.84 · 4->5 1.50 · 5->6 1.77 · 6->7 1.32 · 7->8 1.30 · 8->9 1.72 · 9->10 0.43 · 10->11 2.04 · 11->12 1.69
  12->13 1.18 · 13->14 0.50 · 14->16 ~0 · 16->17 1.83 · 19->20 2.40 · 23->24 -1.57 · 29->31 ~1.0 · 44->47 ~0.5
random-draw marginal at the same depths: 2.78 (3->4) · 2.5 (6->7) · 2.2 (10->11) · 1.7 (20->21) · 1.1 (40->41)
```
LONG and SHORT separately track ALL (LONG 601 bars carries every bar above Q 16; SHORT's deepest band is 13–16 with 2 bars).

**Multiplicity inside the bar** (top3share = share of the 3Q slots taken by the three most-repeated conditions; C≥Q/2 = conditions carried by at least half the signals on the bar):
```
band    bars  maxMult  top3share  sharedConds  singletonConds  C>=Q/2
3         94   2.14     0.532       1.51          5.70          1.51
4        256   2.61     0.488       2.21          6.84          2.21
5        210   3.14     0.475       2.92          7.63          1.05
6-7      194   3.72     0.437       3.79          9.06          1.18
8-9       79   4.62     0.415       4.97         10.49          0.94
10-12     55   5.47     0.380       6.87         11.82          0.64
13-16     28   7.68     0.369       9.39         12.71          0.64
17-22     23  10.43     0.368      11.39         14.30          0.74
23-30     22  13.09     0.325      15.68         16.14          0.68
31-47     12  16.75     0.285      21.50         18.92          0.33
```
What the numbers say, without interpretation beyond them: a depth-3 bar already shares ~1.8 of its 9 slots (7.2 distinct); a depth-25 bar has 31.8 distinct conditions from 77.5 slots, with the single most-repeated condition on ~13 of ~26 signals and **16 conditions appearing on exactly one signal**. Observed distinct sits at 0.82× the random-draw expectation at Q3 and settles at ~0.60× from Q17 up. The marginal slope runs 1.3–1.9 through Q3–12 against a random-draw 2.2–2.8, and ~1 from Q17 up against a random-draw 1.1–1.7. Neither the "2–3 per step" nor the "flat" signature in the brief is what the data shows; it is between them at a stable ~0.6 of random, and the number of singleton conditions keeps rising with depth (5.7 → 18.9).

---

## 3. DO THE CONDITION SETS REPEAT?
```
unique condition sets            939 of 973 bars
sets occurring more than once    30 sets, 64 bars, largest repeat x3 (four sets at x3, all depth 4)
unique SIGNAL sets               939 of 973 (identical — a repeated condition set is a repeated signal set)
repeats that are the previous traded bar within 5 bars (same event)   4 of the 64
pairwise Jaccard, 472,878 bar-pairs ENUMERATED   mean 0.0596 · median 0.0435 · p90 0.148 · p99 0.385 · max 1.0
pairs sharing NO condition       37.23%
within-LONG mean 0.082 · within-SHORT 0.074 · cross-direction 0.037 (max 0.40)
single-linkage families   J>=0.8: 883 families, largest 22, 835 singletons
                          J>=0.6: 634, largest 64/58/23
                          J>=0.5: 330, largest 174/75/74/70/69
                          J>=0.4: 78, largest 888  (single-linkage chaining; not a family)
```
The four x3 sets: three are SHORT depth 4 (8, 6, 7 conditions), one LONG depth 4 (7 conditions); P&L on them $129, –$71, $297, $385.

---

## 4. WHAT DRIVES THE TRADES — condition level
`conditions_on_bars.csv` (274 rows), `pairs_on_bars.csv` (11,900 rows). **No condition and no pair sits on ≥ 20 loss events; the maximum on any condition is 14 (LONG) and 4 (SHORT). Every loss figure below is a COUNT.**

**LONG, ranked by presence on profitable bars (601 bars, 141 conditions present):**
```
condition                 bars  share  profBars  lossEv  meanQ   meanBar$  meanMult  bookSlots
AT_Regime_ST:==1           226  .376     212      14    12.45     524       2.38      13
Volume:hi                  197  .328     189       8    10.80     517       1.32      10
OBV_Macd:lo                185  .308     175      10    11.29     593       1.77      14
AT_Slope_ST:lo             184  .306     173      11    13.50     500       6.78      32
OR_Low_Side:==-1           171  .285     165       6    11.96     457       2.21      14
PrevDay_Low_Side:==-1      169  .281     156      13     9.22     485       2.09      10
Slope_EMA_ST:lo            167  .278     155      12    14.28     525       3.72      19
Micro_Rejection:lo         156  .260     148       8     8.10     349       2.42      16
Upper_Wick:hi              140  .233     136       4    11.52     772       1.24       6
Bars_Since_Flip:hi         144  .240     134      10    13.50     432       2.53      11
Volume_Avg_10:hi           126  .210     124       2    10.35     445       1.48       9
Momentum_Value:hi          130  .216     123       7    15.52     687       2.62       8
Micro_FailedBreak:hi       114  .190     110       4    13.39     588       1.68       6
Micro_Hurst:hi             104  .173      99       5    11.55     698       2.92      10
presence concentration: top 5 conditions = 11.3% of 8,544 condition-presences; top 10 = 20.4%; e^H over presence 107.7 of 141
```
**SHORT (372 bars, 133 conditions present):**
```
Volume_Avg_10:hi           113  .304     112       1     5.40     299       1.42       8
Bar_Range:hi               113  .304     109       4     5.02     288       1.32       4
Sqz_Val:hi                  89  .239      87       2     4.74      98       3.17       8
Micro_Hurst:hi              88  .237      86       2     4.55     149       1.65       5
PrevDay_High_Side:==1       85  .229      81       4     4.86     132       1.62       6
D2D_Dynamic_Sensitivity:lo  80  .215      78       2     5.42     363       1.89       8
Lower_Wick:lo               76  .204      74       2     5.33     383       2.08       5
D2D_ATR_MA:hi               72  .194      71       1     5.11     147       1.00       2
Micro_RangeVelocity:lo      72  .194      70       2     5.04     409       1.40       3
presence concentration: top 5 = 12.8% of 3,804; top 10 = 22.6%; e^H over presence 102.2 of 133
```
Two facts the operator will want raw. (i) `AT_Slope_ST:lo` holds 32 of 573 LONG slots (5.6%) and carries mean multiplicity 6.78 on the bars where it appears — it is the book's one genuinely repeated condition; it is on 184 bars, but 417 LONG bars do not carry it. (ii) The LONG side's most-present conditions are `:lo` on the short-term slope family plus `AT_Regime_ST:==1`. The record (quant non-negotiables §F) states AT_Regime_ST==1 is the BEARISH label; verified on part 1 of the frame: 95.7% of ==1 bars have AT_Slope_ST < 0. On the 601 LONG traded bars AT_Slope_ST < 0 on 320. That is what the LONG book fires on — reported, not interpreted.

**Pairs co-occurring on traded bars (read from the 973, not searched):** LONG 7,330 distinct pairs, SHORT 4,570 (of 30,876 possible). **LONG: 6,860 of 7,330 exist only ACROSS signals — never inside any one signal. SHORT: 4,279 of 4,570.**
```
LONG pair                                  bars profBars lossEv meanQ  meanBar$  insideOneSignal  crossOnly
AT_Slope_ST:lo + Slope_EMA_ST:lo            135   125     10    15.6     591           0            135
AT_Regime_ST:==1 + OR_Low_Side:==-1         121   117      4    14.4     569         107             14
AT_Regime_ST:==1 + Slope_EMA_ST:lo          121   113      8    16.9     631          57             64
AT_Regime_ST:==1 + AT_Slope_ST:lo           116   109      7    17.2     658          68             48
AT_Regime_ST:==1 + Bars_Since_Flip:hi        97    90      7    17.4     568           0             97
AT_Regime_ST:==1 + Volume:hi                 84    80      4    16.0     806           0             84
AT_Regime_ST:==1 + AT_Slope_LT:lo            83    78      5    19.4     842           0             83
AT_Slope_LT:lo + Momentum_Value:hi           78    74      4    20.0     910           0             78
  pairs on >= 20 bars: 1,072 · pairs on >= 20 loss events: 0
SHORT pair
Session_High_Dist_ATR:hi + Sqz_Val:hi        70    68      2     4.8     100          70              0
D2D_ATR_MA:hi + Session_High_Dist_ATR:hi     63    62      1     4.9     106          63              0
Bar_Range:hi + Lower_Wick:lo                 52    50      2     5.4     474          49              3
Bar_Range:hi + D2D_ATR:hi                    41    40      1     5.5     163           0             41
  pairs on >= 20 bars: 142 · pairs on >= 20 loss events: 0
```
The most frequent LONG pair in the working system, `AT_Slope_ST:lo + Slope_EMA_ST:lo` (135 bars, mean Q 15.6), is not a member of any signal. It exists only because two signals carrying each half stack on the same bar. The SHORT top pairs are the opposite shape — they live inside single signals at depth ~5.

---

## 5. WHICH GATES ARE WORKING

### 5a. Gate state on the 973 traded bars, by Q band
Hurst / FailedBreak are mechanism-D percentile buckets at the entry bar via `swept_thresholds.swept` (sacred walk, not recomputed); checksums matched the S8 banner (9.7478 / 80.1874 / 6.2217).
```
LONG   band  bars lossEv  ATRmed ATR<25  ADXmed ADX<20  HU>p90 HU>p80 HU<=p50  FB>p20  ATS90
        3     44    3     36.4   23%     22.8   34%     100%   100%    0%      64%    25%
        4     84    6     33.0   18%     29.9   15%      19%    26%   49%     100%    98%
        5    134    7     35.1   16%     24.3   32%      14%    24%   50%      98%    36%
        6-7  138   10     37.5   13%     23.1   31%      19%    27%   43%     100%    25%
        8-9   65    4     37.5   22%     24.3   28%      23%    37%   40%     100%    22%
       10-12  53    2     41.1   11%     21.6   36%      15%    30%   32%     100%     4%
       13-16  26    0     29.0   27%     22.4   42%      15%    23%   50%     100%     4%
       17-22  23    2     43.4    9%     25.0   13%      13%    26%   39%     100%     0%
       23-30  22    0     40.0   14%     28.1   27%      23%    41%   32%     100%     0%
       31-47  12    0     64.9    0%     26.5   25%      33%    50%    8%     100%     0%
SHORT   3     50    2     31.2   24%     23.6   36%     100%   100%    0%      62%     2%
        4    172    4     33.2   23%     24.0   27%      15%    35%   45%      76%    12%
        5     76    1     37.3    9%     25.5   18%      20%    36%   32%      59%    11%
        6-7   56    1     44.0    9%     25.8   30%      16%    27%   41%      64%     9%
        8-9   14    0     41.6   21%     20.9   36%      29%    36%   57%      50%    14%
```
Hurst at d3 is 100% > p90 by construction (it is the gate). From d4 upward the traded bars sit at 13–33% > p90 — the base rate of HU90 on qualifying bars is 11–18% (5b) — and 32–50% of deep bars are at or below the Hurst median. ATS90 on LONG traded bars falls from 98% at d4 (gated) to 0% from d17. ATR median rises with depth (33 → 65); ADX does not.

### 5b. Where each gate binds — the qualifying population (Q ≥ 3, base gates, D2D agreement; 2,877 LONG bars, 1,316 SHORT)
```
LONG   band   qual  ATRrefused  tierRefused  tierRef%ofATRok  passAll  traded  passAll-not-traded
        3     1107     348         676           89.1%           83       44          39
        4      666     160         356           70.4%          150       84          66
        5      355      77          61           21.9%          217      134          83
        6-7    310      43          48           18.0%          219      138          81
        8-9    151      32          29           24.4%           90       65          25
       10-12   109      12          20           20.6%           77       53          24
       13-16    53       7           9           19.6%           37       26          11
       17-22    62       1          11           18.0%           50       23          27
       23-30    45       0           7           15.6%           38       22          16
       31-47    19       0           2           10.5%           17       12           5
SHORT   3      683     167         455           88.2%           61       50          11
        4      342      65           0            0.0%          277      172         105
        5      159      20           0            0.0%          139       76          63
        6-7    104      20           0            0.0%           84       56          28
        8-9     23       4           0            0.0%           19       14           5
gate pass rates on ATR-ok qualifying bars:  HU90 10.9–17.6% (LONG), 11.8–21.1% (SHORT) across Q bands
                                            FB20 75.6–82.1% LONG · ATS90 41.4% at Q3 -> 5.2% at Q10-12
ATR_1M >= 20 refuses 31% of LONG Q3 bars, 24% at Q4, 12% at Q6-7, 1.6% at Q17-22, 0 of 64 bars at Q >= 23
```
**The passAll-not-traded column is a gate nobody configured:** 594 qualifying bars (382 LONG, 212 SHORT) pass ATR and the tier gate on Q and are not traded. Decomposed with the in-trade set reconstructed from the trade table: **401 of them (220 LONG, 181 SHORT) fell below the floor because the signals that qualified were already holding a position** — Q 4–7 bars arriving with nA < 3. The one-position-per-signal rule, not the cap (36 bars), is the binding constraint at mid depth.

### 5c. What the tier gates refuse — one counterfactual run (trial count 1)
Same book, same engine, same floor/cap/ATR/conviction; `tier_gates` emptied. Control reproduced first in the same process (5,776 / 14.53 / $284,974 / 42).
```
NO_TIER_GATES: 9,123 trades · WR 93.8 · PF 7.59 · net $352,355 · 562 trade losses · 145 LOSS EVENTS · 70 loss days
               worst bar -$1,224 · 0 losing weeks of 26 · 125 days · 1,990 entry bars
entry bars: control 973 · no-tier 1,990 · shared 876 · only-control 97 (jar-state drift, $12,124, 11 events) · only-no-tier 1,114
bars present ONLY with tier gates off, by direction x tier cell (tier = min(admitted depth, 5)):
  cell       bars  lossEv   net $    mean $  | control cell: bars lossEv net $
  LONG  d3   461     57    20,305     44    |  59    3    8,375
  LONG  d4   213     26    12,400     58    |  92    8   14,488
  LONG  d5+  121     10    35,286    292    | 450   23  190,961
  SHORT d3   314     19    21,277     68    |  62    2    5,543
  SHORT d4     4      2      -688           | 181    4   22,894
  SHORT d5+    1      0        77           | 129    2   42,714
LONG refused, by admitted depth: d3 461/57ev · d4 213/26ev · d5 45/6 · d6 21/2 · d7 18/1 · d8+ 37/1ev/$21,959
loss-bar rate on refused bars 10.23% vs 4.32% on control bars
```
Where each gate stops binding, from 5b + 5c: **HU90 at d3** refuses 9 of 10 floor-level bars on both sides and those bars carry 76 of the 114 refused loss events. **FB20 & ATS90 at LONG d4** refuses 7 of 10 and 26 events. **FB20 at LONG d5+** refuses ~1 in 5 bars; those 121 bars carry 10 events and $35,286 at $292/bar — at d8+ it refused 37 bars with 1 loss event and $21,959. **SHORT d4/d5+ are free and refuse nothing.** **ATR ≥ 20 stops binding at Q ≈ 23 on the LONG side** (0 refusals in 64 bars).

---

## ARTIFACTS
`skeleton.csv` · `conditions_on_bars.csv` · `pairs_on_bars.csv` · `trades_NO_TIER_GATES.csv` · `decomp_scripts/` (build_skeleton.py, out2_killing_plot.py, out3_sets.py, out4_conditions.py, out5_gates.py, out5c_counterfactual.py). Run logs: S8 control 236s, canary 37s, counterfactual pair ~8 min, all on one core.

## GAPS, STATED
- The in-trade set on non-traded bars is reconstructed from `trades.csv` as `entry_bar < bar < exit_bar`; cap state (live lots not yet BE-nudged) is not reconstructible from the table, so "cap/other" residuals (4 LONG) are labelled, not resolved.
- 5b classifies tier gates on Q; the engine applies them on n_plan. That is why traded LONG d4 bars show ATS90 at 98%, not 100%.
- Output 4 e^H contribution is given as presence-share concentration (e^H over presence per direction) rather than a per-condition decomposition, which has no single accepted form.
