# THE TURN RULE — PRE-REGISTERED, DERIVED IN-SEGMENT, WALKED FORWARD
**PUSH FAILED ON CREDENTIALS.** `git push origin HEAD:main` → `could not read Username for 'https://github.com'`; no token in the environment, no stored credentials, no `ssh` in the container. Two commits are shipped as `git am`-able patches: `0001` = `RULE_PREREG.md` (commit `aa15dbb`, 09:43:49 UTC — BEFORE scoring, which started 09:44:52 and ended 09:46:56 per the log timestamps), `0002` = results/derivations/script/log (`ebc5b70`). Base `f27840ef2683`. Until applied and pushed, everything below verifies from the shipped files, not from HEAD.

Quant seat · 2026-08-22 · frame `46586cbb1671` · oracle `518862bf19fb` (imported; no threshold recomputed) · `[adm_engine] rule=FLOORED cap=21 …` read on every run. All figures at 1.0 lot.
**Engine trials this report: 71** (1 control + 10 full-frame + 60 walk-forward). **Cumulative across my four reports: 137.** One swept dial (k, 5 values); three constants fixed before scoring (n = 6, suppression lift ≤ 0.25, suppression untraded-rate ≥ 0.05); one standing floor (20 traded bars). No fourth dial was added — see §F for the one place a fourth dial was wanted and why it was not taken.

## 0. CONTROL — reproduced to the cent through the scoring path used for every row
```
[adm_engine] rule=FLOORED cap=21 floor={1: 3, -1: 3} tier-gates=4 · GATE MASKS HU90 9.7478% FB20 80.1874% ATS90 6.2217%
42 loss events · worst bar -$1,224 · worst day -$346.60 · 0 losing weeks of 26 · 119 days · 973 entry bars · 5,776 trades · WR 96.12 · PF 14.53 · MARGIN 33.07 · $284,974
```

## THE RULE (RULE_PREREG.md, verbatim in structure)
R_d(k): participation floor unchanged (ADX ≥ 15, Volume > 50, post-warmup, not Friday-close) · D2D_Trend_Dir == d · ATR_1M ≥ 20 (engine global gate) · **at least k of the n = 6 TURN conditions true** · **no SUPPRESS condition true** · the 297's 5+ tier cell (LONG `Micro_FailedBreak > p20`, SHORT free) · cap 21 · FLOORED · conviction as config, recentfb False.
T_d = the six highest-lift conditions (hour-matched, 297-traded vs eligible-untraded, enumerated) among those true on ≥ 20 traded bars **of the training prefix**; S_d = conditions with lift ≤ 0.25 and untraded rate ≥ 0.05 in the prefix. Position conventions ONE (one open position per direction at a time) and RR (30 round-robin detectors: one 1.0-lot position per admitted bar, no lock), as in the flat-count report.

## A. THE MOMENT, NAMED — per training segment
```
segment (prefix)          traded bars   TURN set T (lift, traded bars)                                                                         SUPPRESS S
FULL frame          LONG  601    AT_Score_ST:lo x8.9 73b · Slope_EMA_ST:lo x8.3 174b · D2D_Signal:==1 x7.4 118b · D2D_Up_Count:hi x7.2 121b · EMA_Oscillator:lo x6.9 44b · AT_Slope_ST:lo x6.8 192b   Micro_FailedBreak:lo x0.16
                    SHORT 372    AT_Score_LT:hi x4.5 79b · D2D_Signal:==-1 x3.6 36b · VAL_Side:==0 x3.6 42b · D2D_Dn_Count:hi x3.4 37b · EMA_Oscillator:hi x3.3 27b · D2D_Dynamic_Sensitivity:lo x3.2 104b   —
< Feb  (Jan only)   LONG   32    VWAP_Side:==-1 x1.5 · Harmonic_D2D_Concordance:==0 x1.4 · Harmonic_OBVf_Concordance:==0 x1.4 · OR_High_Side:==-1 x1.3 · Sqz_State:==1 x1.2 · PrevDay_Close_Side:==-1 x1.2   11 conditions
                    SHORT   8    UNDERIVABLE (no condition on >= 20 traded bars)
< Mar  (Jan-Feb)    LONG  148    D2D_Up_Count:hi x8.3 · D2D_Signal:==1 x8.2 · AT_Score_ST:lo x7.4 · Slope_EMA_ST:lo x5.8 · AT_Slope_ST:lo x5.8 · AT_Score_LT:lo x3.6   12 conditions incl. Micro_FailedBreak:lo x0.12
                    SHORT  69    Sqz_Val:hi x3.0 · Momentum_Value:hi x2.9 · Micro_Hurst:hi x2.4 · OBVf_Signal:==1 x2.2 · OBVf_Trend_Dir:==1 x2.2 · Trend_Concordance:==0 x2.2   —
< Apr  (Jan-Mar)    LONG  297    AT_Score_ST:lo x7.6 · Slope_EMA_ST:lo x7.6 · D2D_Up_Count:hi x7.5 · D2D_Signal:==1 x7.5 · AT_Slope_ST:lo x6.8 · KAMA_Dist:lo x4.2   AT_Score_LT:hi · Micro_FailedBreak:lo · PrevDay_Low_Dist_ATR:hi
                    SHORT 164    D2D_Signal:==-1 x6.1 · D2D_Dn_Count:hi x5.8 · AT_Score_LT:hi x3.9 · KAMA_Dist:hi x3.7 · D2D_Dynamic_Sensitivity:lo x3.6 · OBV_Velocity:hi x3.4   —
< May  (Jan-Apr)    LONG  387    AT_Score_ST:lo x8.7 · D2D_Up_Count:hi x7.7 · D2D_Signal:==1 x7.7 · Slope_EMA_ST:lo x7.0 · AT_Slope_ST:lo x6.4 · KAMA_Dist:lo x4.5   Micro_FailedBreak:lo x0.13
                    SHORT 247    AT_Score_LT:hi x5.0 · D2D_Signal:==-1 x4.7 · D2D_Dn_Count:hi x4.3 · EMA_Oscillator:hi x3.8 · D2D_Dynamic_Sensitivity:lo x3.5 · KAMA_Dist:hi x3.2   —
< Jun  (Jan-May)    LONG  453    AT_Score_ST:lo x9.2 · D2D_Signal:==1 x7.9 · Slope_EMA_ST:lo x7.8 · D2D_Up_Count:hi x7.8 · AT_Slope_ST:lo x6.7 · EMA_Oscillator:lo x6.0   Micro_FailedBreak:lo x0.15
                    SHORT 292    AT_Score_LT:hi x4.4 · D2D_Signal:==-1 x4.0 · D2D_Dn_Count:hi x3.7 · VAL_Side:==0 x3.4 · EMA_Oscillator:hi x3.4 · D2D_Dynamic_Sensitivity:lo x3.3   —
< Jul  (Jan-Jun)    LONG  559    AT_Score_ST:lo x8.8 · Slope_EMA_ST:lo x8.0 · D2D_Signal:==1 x7.6 · D2D_Up_Count:hi x7.5 · AT_Slope_ST:lo x6.6 · EMA_Oscillator:lo x6.5   Micro_FailedBreak:lo x0.16
                    SHORT 350    AT_Score_LT:hi x4.5 · VAL_Side:==0 x3.6 · D2D_Signal:==-1 x3.5 · EMA_Oscillator:hi x3.4 · D2D_Dn_Count:hi x3.3 · D2D_Dynamic_Sensitivity:lo x3.2   —
```
The LONG moment is stable from the Jan–Feb prefix onward: five of six conditions (`AT_Score_ST:lo`, `Slope_EMA_ST:lo`, `D2D_Signal:==1`, `D2D_Up_Count:hi`, `AT_Slope_ST:lo`) appear in every derivation with ≥ 148 traded bars, with the sixth slot rotating among `EMA_Oscillator:lo`, `KAMA_Dist:lo`, `AT_Score_LT:lo`. `D2D_Up_Count` is non-zero only on flip bars (verified on the frame; Up_Count = 1 on every `D2D_Signal:==1` bar), so the first two flip terms are one condition stated twice. `Micro_FailedBreak:lo` is the LONG suppressor in every segment with ≥ 148 bars. SHORT stabilises from the Jan–Mar prefix (`AT_Score_LT:hi`, `D2D_Signal:==-1`, `D2D_Dn_Count:hi`, `D2D_Dynamic_Sensitivity:lo` constant; `VAL_Side:==0` / `EMA_Oscillator:hi` / `KAMA_Dist:hi` / `OBV_Velocity:hi` rotating) and has no suppressor at any segment. The January-only prefix (32 LONG / 8 SHORT bars) derives noise on LONG (lifts 1.2–1.5) and nothing on SHORT.

## B. BARS FOUND — full-frame derivation, before scoring
```
k   LONG rule bars   in the 297's 601   297 LONG bars missed   bars the book never traded   | SHORT rule bars   in the 297's 372   missed   never traded
2        949              253                 348                      696                  |      719                84              288         635
3        236              111                 490                      125                  |      216                24              348         192
4         41               35                 566                        6                  |       32                 3              369          29
5         17               16                 585                        1                  |        0                 0              372           0
6          0                0                 601                        0                  |        0                 0              372           0
```
At k = 2 the rule reaches 42% of the 297's LONG bars and 23% of its SHORT bars, and finds 1,331 bars the book never traded. At k = 4–5 it is almost entirely the book's own bars (35 of 41; 16 of 17) and there are fewer than fifty of them. There is no k at which the rule is both book-like and populous.

## C. FULL-FRAME SETUP (in-sample; rule derived on the whole frame and scored on it)
Order: loss events → worst bar → worst day → losing weeks of 26 → days of 132 → entry bars → trades → WR → PF → MARGIN → net last.
```
k  conv   lossEv  worstBar  worstDay  losingWk  days  entryBars  trades   WR%    PF   MARGIN    net$   maxConc  breach$2500
2  ONE      221    -306     -671.6       9      127    1,241     1,241   82.19  1.16   2.26    4,791      2        0
2  RR       285    -306   -1,327.9       7      127    1,668     1,668   82.91  1.27   3.65    9,922      7        0
3  ONE       60    -306     -521.2      11      112      344       344   82.56  1.30   4.16    2,677      2        0
3  RR        81    -306     -795.0      12      112      452       452   82.08  1.33   4.64    3,578      7        0
4  ONE       12    -306     -306.0      10       59       71        71   (12 events — count)    1,471      1        0
4  RR        12    -306     -306.0      10       59       73        73   (12 events — count)    1,503      3        0
5  ONE/RR     1    -108      -92.2       1       16       17        17   (1 event — count)      1,047      1        0
6  ONE/RR     —  no bars
```

## D. WALK-FORWARD — THE RESULT THAT MATTERS
Derived on the months before each sacred `wf.FOLDS` month, scored on that month only (`mask_window`); Jan(19-31) has an empty prefix and is UNDERIVABLE; July (outside `wf.FOLDS`) is an extra window derived on Jan–Jun. Per-fold rows for every k and both conventions are in `turn_rule_results.csv`; k = 2 and 3 shown:
```
fold       k  conv  lossEv  worstBar  worstDay  losingWk/wks  days  entryBars  trades   WR%    PF   MARGIN    net$   ruleBars L/S  of which in 297 L/S
Feb        2  ONE      7     -153     -63.6        0/4         13       70       70   (7 — count)       1,374    198 / —        40 / —     derivation from 32/8 Jan bars
Feb        2  RR      26     -153    -177.6        0/4         13      198      198   86.87  2.01  10.19  2,471    198 / —        40 / —
Mar        2  ONE    115     -306    -397.1        0/5         22      572      572   79.90  1.16   2.50  1,876    165 / 1,346    33 / 51
Mar        2  RR     278     -306  -1,009.9        1/5         23    1,511    1,511   81.60  1.23   3.30  5,963    165 / 1,346    33 / 51
Apr        2  ONE     25     -306    -396.9        2/5         22      169      169   85.21  1.23   2.82    827     80 / 141      21 / 20
Apr        2  RR      34     -306    -482.1        3/5         22      221      221   84.62  1.22   2.75  1,008     80 / 141      21 / 20
May        2  ONE     31     -306    -519.7        3/5         21      163      163   80.98  1.20   3.03    789    110 / 128      33 / 8
May        2  RR      40     -306    -389.1        1/5         21      238      238   83.19  1.34   4.49  1,587    110 / 128      33 / 8
Jun(1-25)  2  ONE     38     -306    -355.9        2/5         22      209      209   81.82  1.18   2.66    861    190 / 102      55 / 9
Jun(1-25)  2  RR      56     -306    -775.8        2/5         22      292      292   80.82  1.25   3.66  1,606    190 / 102      55 / 9
Jul EXTRA  2  ONE     22     -306    -285.4        1/4         15      121      121   81.82  1.12   1.73    380     99 / 63       27 / 7
Jul EXTRA  2  RR      28     -306    -208.6        1/4         15      162      162   82.72  1.49   6.44  1,839     99 / 63       27 / 7
Mar        3  ONE     84     -306    -577.1        2/5         22      445      445   81.12  1.17   2.51  1,319     59 / 1,213    21 / 47
Mar        3  RR     231     -306  -1,128.3        2/5         23    1,272    1,272   81.84  1.23   3.34  4,841     59 / 1,213    21 / 47
Apr        3  ONE      7     -306    -186.1        1/5         20       57       57   (7 — count)         975     12 / 49        9 / 4
May        3  ONE     13     -306    -440.9        3/5         20       52       52   (13 — count)         76     23 / 40       15 / 0
Jun(1-25)  3  ONE     10     -153    -159.4        3/5         17       51       51   (10 — count)        519     50 / 25       26 / 3
Jun(1-25)  3  RR      18     -193    -795.0        3/5         17       75       75   (18 — count)        672     50 / 25       26 / 3
Jul EXTRA  3  RR      11     -306    -296.9        2/4         14       59       59   (11 — count)        769     34 / 25       15 / 4
```
**Aggregate out-of-segment (union of fold trades):**
```
                        lossEv  worstBar  worstDay  losingWk/wks  days  entryBars  trades   WR%    PF   MARGIN    net$
k=2 ONE  Feb-Jun          216    -306     -519.7       6/22       100    1,183    1,183   81.74  1.23   3.35    5,725
k=2 RR   Feb-Jun          434    -306   -1,009.9       6/22       100    2,460    2,460   82.36  1.29   3.95   12,635
k=2 ONE  Feb-Jul          238    -306     -519.7       6/25       115    1,304    1,304   81.75  1.22   3.16    6,105
k=2 RR   Feb-Jul          462    -306   -1,009.9       6/25       115    2,622    2,622   82.38  1.30   4.15   14,474
k=3 ONE  Feb-Jun          122    -306     -577.1       7/22        92      673      673   81.87  1.32   4.51    4,066
k=3 RR   Feb-Jun          297    -306   -1,128.3       8/22        92    1,659    1,659   82.10  1.32   4.44    8,944
k=3 ONE  Feb-Jul          130    -306     -577.1       9/25       106      717      717   81.87  1.32   4.46    4,415
k=3 RR   Feb-Jul          308    -306   -1,128.3      10/25       106    1,718    1,718   82.07  1.33   4.57    9,713
k=4 ONE  Feb-Jul           67    -153     -288.3       9/23        71      343      343   80.47  1.39   5.73    2,337
k=4 RR   Feb-Jul          131    -153     -709.7       7/23        71      727      727   81.98  1.30   4.16    3,544
k=5 ONE  Feb-Jul           26    -153     -155.0       3/16        40      154      154   83.12  1.85  10.39    1,887
k=5 RR   Feb-Jul           55    -153     -637.4       3/16        40      354      354   84.46  1.61   7.34    3,090
excluding the Feb fold (derived on 32 / 8 January bars):
k=2 ONE  Mar-Jul          231    -306     -519.7       6/21       102    1,234    1,234   81.28  1.18   2.59    4,731
k=2 RR   Mar-Jul          436    -306   -1,009.9       6/21       102    2,424    2,424   82.01  1.26   3.71   12,003
k=3 ONE  Mar-Jul          122    -306     -577.1       9/21        93      649      649   81.20  1.25   3.61    3,238
k=3 RR   Mar-Jul          282    -306   -1,128.3      10/21        93    1,530    1,530   81.57  1.28   3.96    7,498
```
No variant breaches −$2,500 on any day. Every variant with ≥ 20 events carries 6–10 losing weeks out of 21–25. WR per bar sits at 80–84% at every k, in-segment and out, on both conventions — the walk-forward numbers are the in-sample numbers; the rule did not degrade out of segment, because it had nothing to degrade from.

## E. JUNE
```
              297 control                       rule, June fold OOS (derived Jan-May) k=2 RR      k=3 RR
month  trades bars  WR%    PF  events worstDay |  trades  WR%    PF  events worstDay losingDays |  trades  WR%    PF  events worstDay
May      659  111  98.18 60.64    2    -73.3   |   238   83.19  1.34   40    -389.1      9      |    63   76.19  1.09   15    -404.4
Jun      965  164  91.19  5.34   14   -346.6   |   292   80.82  1.25   56    -775.8     10      |    75   76.00  1.35   18    -795.0
Jul      436   64  97.25 40.69    3    -56.0   |   162   82.72  1.49   28    -208.6      5      |    59   81.36  1.50   11    -296.9
```
(297 Jan 1 event / Jul 3 events are counts; rule k = 3 rows under 20 events are counts.) June is not the rule's bad month; March is (k = 2 RR: 278 events, worst day −$1,009.90, driven by 1,346 SHORT rule bars from the Jan–Feb-derived SHORT set). In June the rule split cleanly on the book's footprint: **k = 2 RR reached 64 of the 297's 164 June bars — 6 loss events, +$2,847 — and 228 bars the book did not trade — 50 loss events, −$1,241**; k = 3: 29 shared bars / 4 events / +$1,404 against 46 others / 14 events / −$732. The rule handles June neither better nor worse than any other month; the book's June bars remain good bars inside the rule's population.

## F. WHAT THE DATA GIVES
The moment is real, nameable and stable — the LONG description (`D2D_Signal:==1` · `AT_Score_ST:lo` · `Slope_EMA_ST:lo` · `AT_Slope_ST:lo` · suppress `Micro_FailedBreak:lo`) re-derives identically from every prefix with ≥ 148 traded bars, and every time a rule built on it reaches a bar the 297 traded, that bar wins at the book's rate. **But the description is necessary, not sufficient:** at k = 2 it admits 2–3 bars the book never traded for every bar it shares, and those bars trade at the state baseline (WR ≈ 82%, PF 1.2–1.3, 6–10 losing weeks of 25), in-sample and out-of-segment alike; at k ≥ 4 it collapses to a few dozen bars that are the book's own. The one place a fourth dial was wanted: something to separate the rule's 1,331 extra bars from its 337 shared ones — and the only thing known to do that is the triple membership itself, so it was not taken. Survival never failed; edge did. **A rule stated today admits correctly on data it has not seen — and what it admits is the 77.7%/PF-1.7 state, not the book.** The 297 stays, and what it does is now described: it trades a subset of the turn that a conjunction of named extremes does not isolate at any k.

## ARTIFACTS
`RULE_PREREG.md` (committed `aa15dbb`, before scoring) · `turn_rule_results.csv` (70 scored rows + control line in log) · `rule_derivations.json` (T and S for every segment, with lifts and bar counts) · `turn_rule.log` (full console incl. every `[adm_engine] rule=…` line) · `decomp_scripts/turn_rule.py` · patches `0001`, `0002`.
