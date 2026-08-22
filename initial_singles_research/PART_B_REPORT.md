# PART B — THE DIFFERENTIATOR INSIDE A
**PUSH FAILED ON CREDENTIALS** (`could not read Username for 'https://github.com'`; no token, no stored credentials, no `ssh`). Two commits shipped as `git am`-able patches on base `13f41f61a34c`: `0001` = `RULE_B_PREREG.md` (commit `e72ef12`, **10:24:04 UTC, before scoring**; scoring log 10:27:22–10:28:58), `0002` = `initial_singles_research/partB/` (`67ebf2d`: populations, B1–B4 tables, A+B results, derivations, scripts, logs).

Quant seat · 2026-08-22 · frame `46586cbb1671` · oracle `518862bf19fb` imported · `[adm_engine] rule=FLOORED cap=21 …` read on every run · 1.0 lot throughout.
**Trial count.** Engine: 43 this report (1 control + 6 full-frame + 36 walk-forward) → **cumulative 180**. Statistical: B1 36 · B2 50 · B3 4,980 · B4 498 singles + 11,900 pairs · non-circular diagnostic 64 · in-scoring AUC derivations 224 → **17,752 this report**.
Control reproduced to the cent: 42 · −1,224 · −346.60 · 0 of 26 · 119 · 973 · 5,776 · 96.12 · 14.53 · 33.07 · $284,974.

## THE POPULATIONS — full frame, A = R_d(k=2) full-frame derivation
A-bar outcomes are the A-rule's RR trade on that bar (one position per bar, conviction lots); 297\A outcomes are the 297's own bar P&L per lot, since A never traded them. Ratios under 20 events are counts.
```
dir    population   bars  lossEv  worstBar  worstDay  days   WR(bars)   PF            net$
LONG   A∩297         253     14     -153     -129.6    87     94.5    (14 — count)   +12,540
LONG   A\297         696    151     -306     -693.4   121     78.3     0.77           -4,376
LONG   297\A         348     20     -153     -106.8    92     94.3     7.68          +11,062   (297 per lot)
SHORT  A∩297          84      1     -129     -104.7    53     98.8    ( 1 — count)    +4,085
SHORT  A\297         635    119     -306     -578.2   120     81.3     0.86           -2,327
SHORT  297\A         288      7     -140     -111.0    85     97.6    ( 7 — count)    +9,537   (297 per lot)
```
A∩297 under the 297's own per-lot path, same bars: LONG 14 events, −155.0 worst day, +$9,532; SHORT 1 event, +$3,501. Per month (all cells under 20 events except where a PF is printed):
```
LONG  A\297   Jan 35b/4ev   Feb 92b/21ev PF 0.58 -1,134   Mar 198b/42ev PF 0.77 -1,330   Apr 84b/15ev -363   May 80b/17ev -551   Jun 135b/37ev PF 0.61 -1,622   Jul 72b/15ev +305
LONG  A∩297   Jan 14b/0     Feb 55b/2 +1,987             Mar 43b/4 +3,056               Apr 25b/0 +1,019    May 34b/1 +1,738    Jun 55b/6 +2,462               Jul 27b/1 +1,929
SHORT A\297   Jan 17b/2     Feb 78b/16 -805              Mar 167b/29 PF 0.88 -494        Apr 112b/22 PF 0.91 -296   May 112b/25 PF 0.81 -587   Jun 93b/13 +381   Jul 56b/12 -634
SHORT A∩297   Jan 2b/0      Feb 13b/0 +1,484             Mar 16b/0 +469                 Apr 23b/1 +645      May 14b/0 +698      Jun 9b/0 +385     Jul 7b/0 +238
```
The split is not a June artefact: A\297 loses in every month but July (LONG) and June (SHORT); A∩297 never has a losing month on either side.

## B1 — GATE STATE ON THE A-BAR (AUC = P(A∩297 value > A\297 value); LONG 253 vs 696, SHORT 84 vs 635; Mann-Whitney, enumerated)
```
quantity                               LONG AUC   p          SHORT AUC   p          computable from frame?
ATR_1M                                  0.648    2.8e-12      0.732    4.7e-12     yes
Volume                                  0.658    7.9e-14      0.701    2.1e-09     yes
Bar_Range                               0.618    2.3e-08      0.680    8.5e-08     yes
Body_Size                               0.548    0.024        0.626    1.8e-04     yes
Bars_Since_Flip                         0.624    4.5e-09      0.518    0.59        yes   (LONG winners median 13 bars after flip vs 6)
Micro_Hurst (raw / mech-D 13-bucket)    0.588/0.583 3e-05    0.606/0.611 1e-03    yes
Micro_FailedBreak (raw / bucket)        0.450/0.455 0.02      0.547/0.552 0.16     yes
ADX_Value                               0.569    1.1e-03      0.545    0.18        yes
D2D_Dynamic_Sensitivity                 0.532    0.10         0.389    3.2e-04     yes
D2D_Persist                             0.486    0.52         0.451    0.14        yes
AT_Slope_ST (raw)                       0.382    2.8e-08      0.422    0.020       yes   (winners MORE pinned)
Slope_EMA_ST (raw)                      0.375    3.6e-09      0.442    0.086       yes
Micro_LogReturn                         0.529    0.16         0.413    9.2e-03     yes
Efficiency_Ratio                        0.515    0.48         0.577    0.021       yes
turn_count (of the 6 T conditions)      0.640    1.6e-18      0.491    0.73        yes
book signal depth on A                  0.883    7e-74        0.984    4e-50       NO — membership
tier cell on A (min(depth,5))  A∩297: LONG 5+ on 232/253, SHORT 5+ on 53/84  |  A\297: LONG depth 0–2 on 443/696, 5+ on 124; SHORT 0–2 on 551/635    NO — membership
```
Clearing 0.65 on ≥ 292 bars: **Volume (LONG 0.658), ATR_1M (SHORT 0.732), Volume (SHORT 0.701), Bar_Range (SHORT 0.680)** — one axis, size. Near 0.5 and reported as such: D2D_Persist, Efficiency_Ratio, Micro_LogReturn (LONG), ADX (SHORT), Bars_Since_Flip (SHORT), Micro_FailedBreak both sides.

## B2 — DEPTH ARRIVAL AND BUILD RATE
Book-signal depth after A (BOOK-DEPENDENT — measured as asked, not a B candidate):
```
                 LONG AUC  mean win/lose   share≥3 win/lose | SHORT AUC  mean win/lose  share≥3 win/lose
depth at A+0      0.883    11.6 / 3.8      1.00 / 0.36      |  0.984     5.5 / 1.2     1.00 / 0.13      (definition of the split)
depth at A+1      0.729     7.6 / 3.4      0.67 / 0.30      |  0.756     2.9 / 0.9     0.52 / 0.11
depth at A+2      0.694     6.1 / 3.0      0.60 / 0.25      |  0.731     2.2 / 0.7     0.41 / 0.09
depth at A+5      0.642     4.6 / 1.9      0.39 / 0.19      |  0.638     1.2 / 0.6     0.16 / 0.07
depth at A+10     0.633     2.8 / 1.0      0.24 / 0.11      |  0.577     0.9 / 0.4     0.12 / 0.05
peak depth A..A+10  0.821  13.5 / 5.9                       |  0.932     5.9 / 2.1
bars to peak        0.409   1.1 / 1.8                       |  0.333     0.5 / 1.9
build rate          0.420   0.94 / 1.03                     |  0.354     0.28 / 0.45
```
**The hypothesis "the 64 build faster or deeper after A" is false in its sequential form:** winners peak ON the A-bar (bars-to-peak AUC 0.41 / 0.33, build-rate 0.42 / 0.35 — losers build more after A, from a lower base). Everything after A+0 is the decay of a stack that was already there. Frame-only analogue (249-aligned condition count at A+L, computable without the book): LONG 0.53–0.59, SHORT 0.63–0.66 — weak. `concurrence_events.csv` joined at floor 20 (104,643 rows, `causal=False`, 249-condition depth, not signal depth): every A-bar falls inside an event; onset_depth 0.48 / 0.48, peak_depth 0.55 / 0.60, build_rate 0.49 / 0.62, duration 0.55 / 0.55, bars-since-onset 0.57 / 0.54. Nothing above 0.62.

## B3 — THE LAG (condition TRUE at A+L, L = 1..10; 4,980 Fisher tests, enumerated; BY q < 0.10)
LONG: 602 cells significant, 99 conditions; SHORT: 275 cells, 46 conditions. Nearly all are **state that is also present at lag 0** — `D2D_ATR:hi` ×1.4 at every lag, `OBV_Macd:lo` ×1.8–2.0, `AT_Slope_LT:lo` / `Slope_EMA_LT:lo` ×1.8, `Volume:hi` ×1.4–1.5, `OR_Low_Side:==-1` ×1.7–1.9, `OR_Position:lo` ×1.65–1.86, `PrevDay_Low_Side:==-1` ×1.7, level-side states ×1.3 (LONG); `D2D_ATR:hi` ×1.5–1.7, `Bar_Range:hi` ×1.6, `RangeOsc_Val:lo` ×2.0 (SHORT). **Sequence-only conditions** — significant at ≥ 3 lags and NOT at lag 0 (Fisher p > 0.05):
```
LONG (15):  Micro_LogReturn:hi lags 1-4 ×1.4-1.6 · Micro_HLAsymmetry:hi 2-5 ×1.4-1.7 · Body_Size:hi 2-9 ×1.3-1.4 · Slope_Accel_LT:hi 5-10 ×1.6-2.0 · Sqz_Val:hi 6-10 ×1.2-1.5 · KAMA_Slope:hi 5-10 ×1.4-1.5 · OR_High_Dist_ATR:hi 4-10 ×1.4-1.5 · Micro_WickImbalance:lo 3-9 ×1.4-1.5
            and UNDER-represented on winners after A: Volume:lo 4-10 ×0.11-0.22 · Bar_Range:lo 3-9 ×0.00-0.18 · ATR_1M:lo 6-10 ×0.05-0.26 · Micro_GarmanKlass:lo 5-9 ×0.14-0.22 · Round_100/500/1000_Dist_ATR:hi 3-10 ×0.05-0.36
SHORT (4):  ADX_Value:hi lags 5-10 ×1.8-1.9 · Volume_Avg_10:hi 3-10 ×1.5-1.6 · AT_Slope_ST:lo 2-10 ×1.3-1.4 · OBV_Macd:lo 6-8 ×1.8-1.9
```
Read plainly: after a LONG A-bar, winners show positive log-return at lags 1–4, rising body size, accelerating long-term slope and rising squeeze from lag 5, and price moving away from round numbers; losers go quiet (low volume, low range, low ATR at lags 4–10). That is the trade working or not working, observed after entry — the outcome restated in variables, not a condition available at A. No lagged condition exists that is not either lag-0 state or a post-entry response.

## B4 — WHAT THE 297 SAW THAT A DID NOT (same bar, condition TRUE from the frame, like-for-like)
498 Fisher tests, BY q < 0.10: LONG 82 of 249 significant (51 over-represented on winners, 31 under), SHORT 44 (36 / 8). Top LONG: `D2D_ATR:hi` 88% vs 61% ×1.44 · `OBV_Macd:lo` ×1.87 · `Slope_EMA_ST:lo` ×1.65 · `Volume:hi` ×1.42 · `AT_Slope_LT:lo` ×1.77 · `OR_Low_Side:==-1` ×1.98 · `OR_Position:lo` ×1.87 · `RangeOsc_Val:lo` ×2.33 · `Slope_Accel_LT:lo` ×4.6 (30 vs 18 bars) · under: `RangeOsc_State:==1` 0.47, `PrevDay_High_Side:==1` 0.36, `VWAP_Side:==1` 0.66. Top SHORT: `Micro_RangeAccel:lo` 51% vs 17% ×2.98 · `Micro_VolAccel:lo` ×2.24 · `Micro_RangeVelocity:lo` ×2.35 · `PoC_Side:==0` ×2.76 · `VAH_Side:==0` ×2.95 · `VAL_Side:==0` ×2.12 · `Session_Low_Side:==0` ×7.0 (12 bars — count) · `Upper_Wick:hi` ×1.99 · `Lower_Wick:lo` ×2.33 · under: `D2D_Signal:==-1` 0.62, `D2D_Dn_Count:hi` 0.61 (SHORT winners are NOT on the flip bar itself).
**Pairs** (the 7,330 LONG / 4,570 SHORT pairs the book runs on, co-TRUE on the A-bar, BY per direction): LONG 1,902 significant (1,774 lift > 1), SHORT 740 (726). The strongest LONG pair is `AT_Slope_ST:lo + D2D_Signal:==1`: 31 winners vs 2 losers (×42.6) — the flip bar with the short-term slope still pinned, inside A. `D2D_ATR:hi` combinations ×1.5–2.4 dominate the rest. SHORT: `Micro_RangeAccel:lo` with Bar_Range/Body/ATR/Volume ×3.4–4.9. Pair-only structure (both members individually non-significant): 29 LONG, 53 SHORT, all on < 40 winners (counts).
**The caveat that governs B4:** A∩297 bars carry ≥ 3 firing book signals; every book condition is over-represented on them by construction, and the 177-pool result already showed that outside the fired signals' own conditions winners carry fewer extremes. B4 is therefore the book's vocabulary echoed back. The only conditions here that are not in the book at all are `Micro_RangeAccel:lo`, `Micro_VolAccel:lo`, `Micro_RangeVelocity:lo`, `Slope_Accel_LT:lo` — the first three on SHORT at 32–43 winners.

## THE NON-CIRCULAR TEST — does any frame-only candidate predict OUTCOME where membership is absent?
Within A\297 alone (no book bar in it), AUC = P(value on winning bar > value on losing bar):
```
                 LONG A\297 (545 win / 151 lose)            SHORT A\297 (516 / 119)
ATR_1M            0.475  p 0.35                               0.395  p 3.6e-04   (high ATR LOSES more)
Volume            0.456  p 0.10                               0.455  p 0.13
Bar_Range         0.504  p 0.88                               0.424  p 9.7e-03
D2D_ATR           0.465  p 0.19                               0.404  p 1.0e-03
Micro_GarmanKlass 0.508                                       0.428  p 0.015
Micro_Hurst       0.488                                       0.473
Bars_Since_Flip   0.516                                       0.556  p 0.056
ATR_1M:hi TRUE    loss-bar rate 23.5% vs FALSE 16.6%          19.7% vs 15.5%
Volume:hi TRUE    23.3% vs 19.7%                               19.0% vs 18.5%
```
The size axis that separates A∩297 from A\297 (B1: ATR 0.65–0.73, Volume 0.66–0.70) **does not predict outcome inside A\297 at all, and on SHORT it points the wrong way.** It separates membership, not results: the book's conditions use Volume:hi / ATR / Bar_Range, so its bars are bigger, and bigness is not what makes them win. Within A∩297 (14 and 1 losing bars — counts) the same quantities lean the other way (Micro_FailedBreak 0.72, Micro_GarmanKlass 0.69 on LONG) on too few events to read.

## A+B — PRE-REGISTERED (`RULE_B_PREREG.md`, `e72ef12`) AND SCORED
B_d(j) = at least j of G_d; G_d = the three frame-only quantities with the largest |AUC−0.5| in the training prefix, mapped to their existing `:hi`/`:lo` condition (mechanism-D p80/p20 via the oracle); j ∈ {1,2,3} the only dial; k = 2 inherited. Derived per segment:
```
FULL        LONG D2D_ATR:hi 0.659 · Volume:hi 0.658 · ATR_1M:hi 0.648     SHORT ATR_1M:hi 0.732 · Volume:hi 0.701 · D2D_ATR:hi 0.699
< Feb       LONG Volume:hi 0.657 · ATR_1M:hi 0.651 · Micro_Hurst:hi 0.641 (26/79)   SHORT UNDERIVABLE
< Mar       LONG Micro_Hurst:hi 0.639 · D2D_Persist:hi 0.616 · Efficiency_Ratio:hi 0.597 (68/98)   SHORT Volume:hi 0.656 · Micro_Hurst:hi 0.644 · D2D_Persist:hi 0.636 (40/797)
< Apr       LONG Volume:hi 0.655 · Bars_Since_Flip:hi 0.637 · ATR_1M:hi 0.632     SHORT Volume:hi 0.71 · D2D_ATR:hi 0.643 · ATR_1M:hi 0.63
< May       LONG Volume:hi 0.678 · ATR_1M:hi 0.666 · Micro_GarmanKlass:hi 0.657   SHORT ATR_1M:hi 0.682 · Volume:hi 0.675 · D2D_ATR:hi 0.658
< Jun       LONG Volume:hi 0.684 · ATR_1M:hi 0.664 · D2D_ATR:hi 0.663             SHORT ATR_1M:hi 0.739 · Volume:hi 0.73 · D2D_ATR:hi 0.707
< Jul       LONG Volume:hi 0.68 · D2D_ATR:hi 0.666 · ATR_1M:hi 0.665              SHORT ATR_1M:hi 0.725 · Volume:hi 0.711 · D2D_ATR:hi 0.696
```
Bars under A+B, full-frame derivation: j=1 LONG 791 (239 in the 297) / SHORT 580 (80); j=2 667 (224) / 452 (67); j=3 528 (194) / 346 (60) — against A alone 949 (253) / 719 (84).
```
FULL FRAME (in-sample)    lossEv  worstBar  worstDay  losingWk/26  days  entryBars  trades   WR%    PF   MARGIN   net$
A alone   k2 ONE / RR      221 / 285   -306   -672 / -1,328    9 / 7     127   1,241 / 1,668      82.2 / 82.9  1.16 / 1.27  2.3 / 3.7   4,791 / 9,922
A+B j=1   ONE / RR         180 / 240   -306   -565 / -1,190   10 / 9     126     985 / 1,371      81.7 / 82.5  1.19 / 1.30  2.7 / 4.2   4,925 / 9,758
A+B j=2   ONE / RR         138 / 190   -306   -529 / -1,207    8 / 7     123     779 / 1,119      82.3 / 83.0  1.36 / 1.46  4.9 / 6.0   7,115 / 11,808
A+B j=3   ONE / RR         105 / 152   -306   -516 /   -802    8 / 8     118     589 /   874      82.2 / 82.6  1.49 / 1.56  6.6 / 7.4   7,212 / 11,228

WALK-FORWARD, aggregate OOS Feb–Jul (Jan UNDERIVABLE; July extra; per-fold rows in AB_results.csv)
A alone   k2 ONE / RR      238 / 462   -306   -520 / -1,010    6 / 6 of 25   115   1,304 / 2,622   81.8 / 82.4  1.22 / 1.30  3.2 / 4.2   6,105 / 14,474
A+B j=1   ONE / RR         191 / 329   -306   -596 /   -976    9 / 8 of 25   114   1,024 / 1,871   81.4 / 82.4  1.15 / 1.27  2.2 / 3.8   3,468 / 10,231
A+B j=2   ONE / RR         101 / 163   -306   -501 /   -776    7 / 6 of 25   110     618 /   999   83.7 / 83.7  1.45 / 1.54  5.8 / 6.8   5,833 / 10,370
A+B j=3   ONE / RR          56 /  88   -306   -293 /   -802    7 / 7 of 24    84     339 /   502   83.5 / 82.5  1.59 / 1.67  7.4 / 8.6   4,285 / 6,979
```
No breach of −$2,500 anywhere. Losing weeks 6–9 of 25 at every j; WR 81–84% at every j. June OOS split under A+B (RR): j=1 63 shared / 6 ev / +$2,836 vs 197 alone / 48 ev / −$1,638; j=2 59 / 6 / +$2,731 vs 157 / 37 / −$859; j=3 52 / 5 / +$2,683 vs 121 / 35 / −$1,513. **B removes winners and losers in proportion; the loss rate of the bars the book did not trade goes from 22% to 29% as j rises.**

## WHAT THE FOUR AXES SAY
- **B1 (gate state):** one axis clears 0.65 — size (ATR, Volume, Bar_Range). It separates membership and not outcome; inside A\297 it is flat on LONG and inverted on SHORT. Scored as B, walked forward: PF 1.30 → 1.67 at the cost of 62% of the bars, losing weeks 6 → 7 of 25. Not B.
- **B2 (arrival):** winners peak on the A-bar; nothing builds after it. The frame-only count after A is ≤ 0.66. Empty.
- **B3 (lag):** 99 / 46 conditions clear BY, all either lag-0 state or the post-entry response (log-return, body, acceleration, round-number distance). No condition available at A and absent at lag 0 that predicts the split. Empty as a rule; non-empty as a description of what a winning turn does next.
- **B4 (same bar):** the book's vocabulary echoed (membership by construction). The non-book conditions it surfaces are SHORT `Micro_RangeAccel:lo` / `Micro_VolAccel:lo` / `Micro_RangeVelocity:lo` on 32–43 winners, and the LONG pair `AT_Slope_ST:lo + D2D_Signal:==1` at 31 vs 2 — both inside A already.

All four measured. None names a frame-computable B that separates outcome where membership is absent. What this narrows: B is not a gate level, not an arrival rate, not a lagged condition, not a same-bar single or pair from the 249. The one thing that separates the 64 from the 228 in June — and the 337 from the 1,331 on the frame — remains the fact of three specific book signals agreeing on that bar, and the measurements above say that agreement is not reducible to the bar's own 249-state at any lag in 0..10. Survival never failed on any row; edge did.

## ARTIFACTS (`partB/`)
`populations.json` · `populations_outcomes.csv` · `B1_gate_auc.csv` · `B2_depth_arrival.csv` · `B3_lagged_conditions.csv` (5,478 rows incl. lag 0, BY flags) · `B4_samebar_conditions.csv` · `B4_pairs.csv` (11,900 rows) · `measure_B.py` + `.log` · `diag_B.py` · `AB_results.csv` (42 rows) · `AB_derivations.json` · `score_AB.py` + `.log` · `RULE_B_PREREG.md` · patches `0001`, `0002`.
