# ONE ARTIFACT, FOUR ANSWERS
Quant seat · 2026-08-22 · HEAD `ed1fe4b3` (2026-08-22 16:16 +0700) · frame `46586cbb1671` · oracle `518862bf19fb`. All figures at 1.0 lot, in-sample. **Engine runs this report: 1 (the control).** Everything else is set arithmetic on oracle masks and the trade table already in hand. Cumulative across my three reports: 66.

## 0. CONTROL — reproduced to the cent at HEAD before anything was measured
```
[adm_engine] rule=FLOORED cap=21 floor={1: 3, -1: 3} tier-gates=4
42 loss events · worst bar -$1,224 · worst day -$346.60 · 0 losing weeks of 26 · 119 days · 973 entry bars · 5,776 trades · WR 96.12 · PF 14.53 · MARGIN 33.07 · $284,974
```

## THE ARTIFACT — `bar_condition_membership.csv`
12,348 rows = (bar, distinct condition), 973 bars, 177 conditions, columns `bar, time, direction, depth_q, condition`; join key `bar` = `skeleton.csv`. Per-bar row count verified equal to `skeleton.distinct_conds` on all 973 bars. File sha256 `c829ae787b0c`.

**PUSH STATUS — NOT AT HEAD, AND HERE IS WHAT WAS TRIED.** Committed locally as `96550d3` on top of `ed1fe4b3` (`initial_singles_research/bar_condition_membership.csv`). `git push origin HEAD:main` → `fatal: could not read Username for 'https://github.com'`. Checked: no `GITHUB_TOKEN`/`GH_*` in the environment, no `~/.git-credentials`, no `gh` config, no `ssh` binary in the container. Three paths tested, none available to this seat. The exact commit is delivered as `0001-quant-bar_condition_membership.csv-long-form-bar-con.patch` (`git am` applies it byte-for-byte) alongside the CSV itself. Until it is applied and pushed, the figures below are verifiable from the CSV, not from HEAD.

## BOOK VOCABULARY — re-derived, not taken
```
297 signals · 891 slots · 177 distinct conditions · 105 of 117 variables · 72 of 249 conditions never used
e^H over variable slots 83.2 · top variable AT_Slope_ST 4.3% (38 slots) · 7 variables appear exactly once
in LONG signals 141 · in SHORT signals 133 · LONG-only 44 · SHORT-only 36 · both 97
```
All six figures in the brief reproduce.

## 1. WHICH CONDITIONS, NOT HOW MANY
Presence in the FIRED signals of the 973 bars (`q1_presence_ranking.csv`, 274 rows), ranked by presence on profitable bars. **Every loss-event figure is below 20 and is a count; no rate is quoted on any row.**
```
LONG (601 bars, 141 conditions present)          bars  share  profBars  lossEv  meanQ   meanBar$
AT_Regime_ST:==1                                  226  .376     212      14    12.5     524
Volume:hi                                         197  .328     189       8    10.8     517
OBV_Macd:lo                                       185  .308     175      10    11.3     593
AT_Slope_ST:lo                                    184  .306     173      11    13.5     500
OR_Low_Side:==-1                                  171  .285     165       6    12.0     457
PrevDay_Low_Side:==-1                             169  .281     156      13     9.2     485
Slope_EMA_ST:lo                                   167  .278     155      12    14.3     525
Micro_Rejection:lo                                156  .260     148       8     8.1     349
Upper_Wick:hi                                     140  .233     136       4    11.5     772
Bars_Since_Flip:hi                                144  .240     134      10    13.5     432
Volume_Avg_10:hi                                  126  .210     124       2    10.3     445
Momentum_Value:hi                                 130  .216     123       7    15.5     687
SHORT (372 bars, 133 conditions present)
Volume_Avg_10:hi                                  113  .304     112       1     5.4     299
Bar_Range:hi                                      113  .304     109       4     5.0     288
Sqz_Val:hi                                         89  .239      87       2     4.7      98
Micro_Hurst:hi                                     88  .237      86       2     4.5     149
PrevDay_High_Side:==1                              85  .228      81       4     4.9     132
D2D_Dynamic_Sensitivity:lo                         80  .215      78       2     5.4     363
Lower_Wick:lo                                      76  .204      74       2     5.3     383
D2D_ATR_MA:hi                                      72  .194      71       1     5.1     147
Micro_RangeVelocity:lo                             72  .194      70       2     5.0     409
```
No condition reaches 40% of either side's bars; presence e^H is 107.7 of 141 (LONG) and 102.2 of 133 (SHORT).

## 2. DOES ANYTHING STACK THE SAME WAY?
Per direction, Jaccard on the distinct-condition sets, all pairs enumerated (LONG 180,300; SHORT 69,006). Families are single-linkage.
```
LONG  601 bars   exact-unique 591, largest exact repeat x3
                 J>=0.8: 570 families, largest 6/5/4/4, 552 singletons, 22 bars have any partner
                 J>=0.6: 444 families, largest 23/19/11/6, 384 singletons, 101 bars have any partner
                 J>=0.4:  50 families, largest 546 (single-linkage chain), 498 bars have any partner
                 pairwise J mean 0.0821 · median 0.0556 · p99 0.412 · 25.5% of pairs share nothing
SHORT 372 bars   exact-unique 348, largest exact repeat x3
                 J>=0.8: 313 families, largest 22/4/3/3, 283 singletons, 37 bars have any partner
                 J>=0.6: 190 families, largest 64/58/11/8, 160 singletons, 140 bars have any partner
                 J>=0.4:  29 families, largest 342 (chain), 315 bars have any partner
                 pairwise J mean 0.0737 · median 0.0400 · p99 0.545 · 47.4% of pairs share nothing
```
The largest families at J ≥ 0.6 and their core (conditions on ≥ 80% of the family's bars):
```
SHORT 64 bars  mean Q 5.0 · 2 loss events · $7,859 · all 7 months · core: Bar_Range:hi
SHORT 58 bars  mean Q 4.7 · 2 loss events · $4,577 · Feb-Jul    · core: D2D_ATR_MA:hi + Session_High_Dist_ATR:hi + Sqz_Val:hi
LONG  23 bars  mean Q 4.7 · 3 loss events · $1,431 · Mar-Apr    · core: ADX_Rising:==1 + OR_Low_Side:==1 + PrevDay_High_Dist_ATR:lo + PrevDay_Low_Side:==-1
LONG  19 bars  mean Q 6.5 · 3 loss events · $4,652 · Jan-Jul    · core: ADX_Rising:==0 + Harmonic_OBVf_Concordance:==0 + KAMA_Dist:lo + Lower_Wick:hi + Micro_WickImbalance:hi + Volume:hi
LONG  11 bars  mean Q 4.6 · 0 loss events · $2,701              · core: PrevDay_Close_Side:==-1 + PrevDay_High_Dist_ATR:lo + PrevDay_Low_Side:==-1 + Sqz_State:==1 + VWAP_Side:==-1 + WeeklyOpen_Side:==-1
```
Plainly: at J ≥ 0.8 nothing clusters (largest 6 and 22 bars; 96% of LONG and 90% of SHORT bars have no partner). At J ≥ 0.6 two SHORT families of 64 and 58 bars exist — one third of the SHORT side — both shallow (Q ≈ 5) and both spanning the whole frame; the LONG side's largest is 23 bars. At J ≥ 0.4 the families are single-linkage chains, not states. The 0.4 chain is the only "cluster" covering most bars and it is an artifact of the linkage.

## 3. WHY THESE BARS — THE DISCRIMINATOR
Condition TRUE on the bar (oracle mask, all 249), 973 traded bars vs **every** eligible untraded bar (same eligibility line: entry_ok, D2D agreement, ATR_1M ≥ 20; LONG 13,559, SHORT 16,389 — enumerated, no sampling), the untraded population reweighted to the traded (direction × EST hour) distribution. Hour-matching moved little: Spearman(hour-matched lift, raw lift) 0.98 / 0.97; both are in `q3_discriminator_lift.csv`. Fisher p is exact on the raw two-by-two; Bonferroni at 249 tests per side is p < 2.0e-04. Ranked among conditions true on ≥ 20 traded bars (212 LONG, 213 SHORT).
```
LONG  top 20 by lift                 group        tradedTrue  tradedRate  untradedRate(hm)  LIFT   p
AT_Score_ST:lo                       not-in-book      73       .1215        .0136           8.91  <1e-4
Slope_EMA_ST:lo                      both            174       .2895        .0348           8.32  <1e-4
D2D_Signal:==1                       LONG-only       118       .1963        .0266           7.37  <1e-4
D2D_Up_Count:hi                      not-in-book     121       .2013        .0279           7.22  <1e-4
EMA_Oscillator:lo                    both             44       .0732        .0106           6.88  <1e-4
AT_Slope_ST:lo                       both            192       .3195        .0467           6.84  <1e-4
KAMA_Dist:lo                         LONG-only        74       .1231        .0249           4.94  <1e-4
D2D_Persist:lo                       not-in-book      23       .0383        .0083           4.60  <1e-4
RangeOsc_Val:lo                      both             75       .1248        .0292           4.28  <1e-4
OBV_Macd:lo                          both            267       .4443        .1209           3.68  <1e-4
KAMA_Slope:lo                        not-in-book      40       .0666        .0185           3.60  <1e-4
AT_Score_LT:lo                       both            119       .1980        .0598           3.31  <1e-4
RangeOsc_State:==-1                  both            158       .2629        .0821           3.20  <1e-4
AT_Regime_ST:==1                     LONG-only       306       .5092        .1661           3.07  <1e-4
Slope_Accel_LT:lo                    both             52       .0865        .0286           3.03  <1e-4
Harmonic_LLEMA:lo                    not-in-book      21       .0349        .0133           2.63  <1e-4
ST_Flip_Event:==-1                   LONG-only        37       .0616        .0242           2.55  <1e-4
AT_Slope_LT:lo                       LONG-only       283       .4709        .1920           2.45  <1e-4
Slope_EMA_LT:lo                      not-in-book     282       .4692        .1988           2.36  <1e-4
VAH_Dist_ATR:lo                      not-in-book     116       .1930        .0820           2.36  <1e-4
LONG  bottom 8 (under-represented)
PrevDay_Low_Dist_ATR:hi              SHORT-only       24       .0399        .1281           0.31  <1e-4
VWAP_Z:hi                            not-in-book      44       .0732        .2306           0.32  <1e-4
Session_Low_Dist_ATR:hi              not-in-book      36       .0599        .1655           0.36  <1e-4
PrevDay_High_Side:==1                both             70       .1165        .3186           0.37  <1e-4
OBVf_DirStepCount:hi                 SHORT-only       41       .0682        .1845           0.37  <1e-4
Dist_To_PoC_ATR:hi                   not-in-book      47       .0782        .1897           0.41  <1e-4
VAL_Dist_ATR:hi                      not-in-book      43       .0715        .1734           0.41  <1e-4
VAH_Dist_ATR:hi                      LONG-only        50       .0832        .1924           0.43  <1e-4
LONG: 51 conditions Bonferroni-significant at lift >= 1.5; 37 at lift <= 0.67 (under-represented).

SHORT top 20 by lift                 group        tradedTrue  tradedRate  untradedRate(hm)  LIFT   p
AT_Score_LT:hi                       both             79       .2124        .0476           4.46  <1e-4
D2D_Signal:==-1                      SHORT-only       36       .0968        .0271           3.57  <1e-4
VAL_Side:==0                         SHORT-only       42       .1129        .0316           3.57  <1e-4
D2D_Dn_Count:hi                      SHORT-only       37       .0995        .0295           3.38  <1e-4
EMA_Oscillator:hi                    LONG-only        27       .0726        .0223           3.26  <1e-4
D2D_Dynamic_Sensitivity:lo           both            104       .2796        .0885           3.16  <1e-4
OBV_Velocity:hi                      both             56       .1505        .0477           3.16  <1e-4
VAH_Side:==0                         SHORT-only       38       .1022        .0346           2.95  <1e-4
Sqz_Val:hi                           both            118       .3172        .1122           2.83  <1e-4
PoC_Side:==0                         SHORT-only       51       .1371        .0505           2.72  <1e-4
Slope_EMA_ST:hi                      both             46       .1237        .0457           2.71  <1e-4
ST_Flip_Event:==-1                   LONG-only        20       .0538        .0200           2.68  1e-4
KAMA_Dist:hi                         both             36       .0968        .0364           2.66  <1e-4
Momentum_Value:hi                    both             79       .2124        .0825           2.57  <1e-4
AT_Slope_ST:hi                       SHORT-only       55       .1478        .0589           2.51  <1e-4
Lower_Wick:lo                        both             98       .2634        .1081           2.44  <1e-4
PrevDay_High_Side:==1                both            132       .3548        .1527           2.32  <1e-4
KAMA_Slope:hi                        not-in-book      25       .0672        .0290           2.32  <1e-4
Micro_Hurst:hi                       both            159       .4274        .1867           2.29  <1e-4
Session_High_Dist_ATR:lo             both            105       .2823        .1253           2.25  <1e-4
SHORT bottom 8 (under-represented)
PoC_Side:==-1                        not-in-book      21       .0565        .1403           0.40  <1e-4
Efficiency_Ratio:lo                  SHORT-only       38       .1022        .1948           0.52  <1e-4
Micro_Hurst:lo                       SHORT-only       39       .1048        .1969           0.53  <1e-4
VAH_Side:==-1                        SHORT-only       41       .1102        .2009           0.55  <1e-4
OBVf_DirStepCount:hi                 SHORT-only       39       .1048        .1795           0.58  1e-4
D2D_Persist:lo                       not-in-book      81       .2177        .3672           0.59  <1e-4
Micro_FractalDim:hi                  LONG-only        45       .1210        .2019           0.60  1e-4
RangeOsc_State:==-2                  both             31       .0833        .1353           0.62  5e-4
SHORT: 47 conditions Bonferroni-significant at lift >= 1.5; 8 at lift <= 0.67.
```
What the list shows, without a conclusion attached: the LONG selectors are the D2D flip itself and its first bars (`D2D_Signal:==1` ×7.4, `D2D_Up_Count:hi` ×7.2, `ST_Flip_Event:==-1` ×2.6) with the short-term slope/oscillator family LOW (`AT_Score_ST:lo` ×8.9, `Slope_EMA_ST:lo` ×8.3, `EMA_Oscillator:lo` ×6.9, `AT_Slope_ST:lo` ×6.8, `KAMA_Dist:lo` ×4.9, `OBV_Macd:lo` ×3.7) and the level-distance family LOW; the under-represented set is price already extended above levels (`VWAP_Z:hi` 0.32, `PrevDay_High_Side:==1` 0.37, `Dist_To_PoC_ATR:hi` 0.41). SHORT is the mirror at lower lift (max 4.5 vs 8.9): the down-flip and its count, `AT_Score_LT:hi`, the `==0` level-side states, `Sqz_Val:hi`, `Micro_Hurst:hi` ×2.3. Four of the top-eight LONG selectors are not in the book at all (`AT_Score_ST:lo`, `D2D_Up_Count:hi`, `D2D_Persist:lo`, `KAMA_Slope:lo`) — they ride with book conditions.

**The asymmetry.** Median hour-matched lift by book group, conditions on ≥ 20 traded bars:
```
                 LONG bars                      SHORT bars
both (97)        1.272  (mean 1.50, max 8.3)    1.222  (mean 1.38, max 4.5)
LONG-only (44)   1.095  (mean 1.50, max 7.4)    0.997  (mean 1.13, max 3.3)
SHORT-only (36)  0.777  (mean 0.90, max 1.9)    1.332  (mean 1.52, max 3.6)
not-in-book      0.889  (mean 1.47, max 8.9)    0.961  (mean 1.14, max 2.3)
```
Both-sides conditions lift on both sides; one-sided conditions lift only on their own side and sit below 1 on the other. On the LONG side the both-sides group is the strongest; on the SHORT side the SHORT-only group edges it.

## 4. THE 177-POOL COUNT — separability, not a strategy
Raw condition count over the book's own vocabulary, traded vs hour-matched eligible untraded (same populations as §3). AUC = exact P(traded count > untraded count), ties half, weighted, enumerated over value histograms.
```
pool                                         LONG traded        LONG untraded      AUC    rank   | SHORT traded      SHORT untraded     AUC    rank
249-aligned (F12 rule)                       median 41 mean 41.0  median 42 mean 42.8  0.452  0.440  | median 30 mean 32.6  median 33 mean 33.2  0.469  0.386
177 pool, direction-specific (141 / 133)     median 51 mean 51.0  median 45 mean 44.4  0.752  0.801  | median 43 mean 43.2  median 37 mean 36.5  0.759  0.803
177 pool, all 177 both sides                 median 57 mean 57.1  median 52 mean 51.6  0.699  0.730  | median 58 mean 58.2  median 52 mean 52.1  0.700  0.745
one-sided only (44 / 36)                     median 16 mean 16.3  median 15 mean 15.0  0.642  0.606  | median  8 mean  7.6  median  6 mean  6.3  0.664  0.721
both-sides only (97)                         median 35 mean 34.8  median 30 mean 29.5  0.738  0.785  | median 35 mean 35.6  median 31 mean 30.2  0.729  0.739
```
(rank = median percentile-rank of the 973 within the untraded distribution.) The direction-specific 177 count moves the 973 from the median to the 80th percentile — **and the next line is why that number cannot be read as separability:**

**Non-circular cut.** A traded bar carries ≥ 3 firing book signals, so ≥ 7 of its "177-pool conditions true" are true by construction. Removing, on every bar in both populations, the conditions belonging to any book signal firing on that bar:
```
                              traded residual                  untraded residual (hm)          AUC     fired-signal conds on bar: traded / untraded
LONG                          median 37  mean 36.8             median 42  mean 41.9            0.313   14.22 / 2.56
SHORT                         median 33  mean 32.9             median 35  mean 35.0            0.401   10.23 / 1.49
restricted to untraded bars with depth_q >= 3 (LONG n=1,596, SHORT n=667 — the bars the gates/ATR/lock refused):
LONG                          median 37                        median 39  mean 38.5            0.436   (249-aligned count on the same cut: 0.541)
SHORT                         median 33                        median 34  mean 34.0            0.451   (249-aligned count on the same cut: 0.513)
```
The entire separation of the 177-pool count is the fired signals' own conditions (+11.7 LONG, +8.7 SHORT); outside them the traded bars carry FEWER book-vocabulary extremes than the untraded bars they are matched to (AUC 0.31 / 0.40), and against the untraded bars that also reached depth 3 the count is at chance (0.44 / 0.45). Untraded eligible bars have depth_q = 0 on 54% (LONG) / 65% (SHORT), 1–2 on 33% / 30%, ≥ 3 on 13% / 5%.

**Q4 closes as the brief allowed it to:** the pool restriction separates only through the groupings it was meant to free the vocabulary from. Stopped here; nothing scored.

## ARTIFACTS
`bar_condition_membership.csv` (+ `0001-…patch`, commit `96550d3` on `ed1fe4b3`) · `q1_presence_ranking.csv` · `q3_discriminator_lift.csv` (498 rows: direction × 249, hour-matched and raw lift, exact Fisher p, book group) · `four_answers.log` (full console incl. the 249-pool and family cores) · `decomp_scripts/four_answers.py`.
