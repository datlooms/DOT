# REPLACE THE FOURTEEN — RESULT
**IT WORKED ON THE BINDING CONSTRAINT, AND IT COST TAIL.** All 15 at-risk bars are coverable from the ordinary field at agg_pf ≥ 2.0; the swapped book holds **zero losing weeks** and breaches nothing; loss events rise 42 → 62 and the worst day moves −$346.60 → −$905.00. The emit-all dependency is closed for the purpose it was kept for — SELECT can reproduce a book with the fourteen's function from the 19,754 field — but the book it reproduces is not the 297's tail. Numbers below; the operator rules.

Built this turn (not landed — the operator pushes): commits `cdfc5fc` (`SELECT/SWAP14_PREREG.md`, 19:00:56 UTC, before scoring) and `d328be9` (`SELECT/swap14/`), shipped as patches `0001`/`0002` on base `4928020f5f2b`. File shas: `whole_dot_300_signals.csv` `40cecee94119` (the 297-row swap), `whole_dot_300_rows.csv` `abd0f857ea8c` (the 300-row extension), `replacements.csv` `dcfcf5eae43a`.
Quant seat · 2026-08-23 · frame `46586cbb1671` · oracle `518862bf19fb` imported · `[adm_engine] rule=FLOORED cap=21 floor={1: 3, -1: 3} tier-gates=4` read on both scored runs · 1.0 lot · **every figure IN-SAMPLE.**
Control reproduced to the cent first: `42 · −1,224 · −346.60 · 0 of 26 · 119 · 973 · 5,776 · 96.12 · 14.53 · 33.07 · $284,974`. The fourteen re-derived from the field at HEAD: `[37, 38, 48, 84, 149, 181, 205, 208, 226, 230, 231, 256, 270, 296]` — matches.
**Trials.** Engine 2 (the pre-registered 297-row swap; the 300-row extension) → **cumulative 203**. Statistical 0 → 18,841. Solo 1,818. Mask builds for 8,657 candidates are not trials.

## 1. THE AT-RISK BARS — enumerated from `build_signal_masks`, exactly 1 LONG + 14 SHORT
```
dir    bar      time               depth with / without   orphans firing   control lots   control bar P&L   exit day
LONG   100142   2026.05.01 16:56        3 / 2              [181]               3             +241.8        05.01
SHORT   10936   2026.01.29 19:55        3 / 2              [231]               3              +37.8        01.29
SHORT   10955   2026.01.29 20:14        3 / 2              [296]               3              +42.6        01.29
SHORT   39346   2026.02.27 19:36        3 / 2              [208]               3              +37.5        02.27
SHORT   65177   2026.03.26 17:53        3 / 2              [230]               3              +91.2        03.26
SHORT   74276   2026.04.06 17:53        3 / 2              [230]               3              +32.1        04.06
SHORT   79655   2026.04.10 16:32        4 / 2              [226, 230]          3             +105.3        04.10
SHORT   88008   2026.04.20 19:19        3 / 2              [230]               3              +30.3        04.20
SHORT   89305   2026.04.21 18:12        4 / 2              [208, 270]          4              +44.4        04.21
SHORT   89489   2026.04.21 21:16        3 / 2              [230]               3             +114.9        04.21
SHORT  100126   2026.05.01 16:40        3 / 2              [230]               3              +62.4        05.01
SHORT  138474   2026.06.10 22:55        3 / 2              [226]               3             +384.3        06.11
SHORT  153020   2026.06.25 18:56        3 / 2              [230]               3              +51.6        06.25
SHORT  171860   2026.07.15 18:18        3 / 2              [230]               3              +51.6        07.15
SHORT  176191   2026.07.20 22:15        3 / 2              [256]               3             −158.4        07.20
```
15 bars, 46 lots, +$1,169 in the control, 1 loss bar. Index 230 alone holds 9 of the 14 SHORT bars. **Two further mechanisms sit outside this list and are why −$11,565 is bigger than $1,169:** (a) 26 LONG and 15 SHORT control bars stay ≥ 3 but fall from the 5+ cell into the d3/d4 gate cells (LONG d3 = HU90, d4 = FB20∧ATS90 — stricter than the 5+ cell); (b) the one-position-per-signal lock re-shuffles: on 2026.06.05 the 297−14 admits a new LONG bar 134378 (3 lots, −$578) that the control never traded, and that single jar shift is what turned W23 from +$208.80 into −$342.20 — **the recorded losing week of the 297−14 is a lock cascade, not an at-risk bar.**

## 2. CANDIDATES — the ordinary field, no new scan
Field rows at agg_pf ≥ 2.0: 8,934 of 19,754; ∪ catalogue VALID (1,818); minus the 297 → **8,657 candidates (5,773 LONG / 2,884 SHORT), masks built for all of them.**
```
LONG  : 81 candidates reach the 1 at-risk bar; greedy set cover = 1 candidate (EMA_Oscillator:hi + Upper_Wick:hi + D2D_Signal:==1, PF 12.32)
SHORT : 209 candidates reach ≥ 1 of the 14; every one of the 14 is reachable; greedy set cover = 8 candidates
        top by reach: OBVf_DirStepCount:lo + EMA_Oscillator:lo + ADX_Value:lo  5 bars  PF 2.86 (fires on only 7 control bars)
                      EMA_Oscillator:lo + Micro_ThrustEff:hi + ADX_Value:lo      4      PF 10.83
                      EMA_Oscillator:lo + ADX_Value:lo + D2D_DirStep:==1         3      PF 4.21
                      13 candidates reach exactly 2; no single candidate reaches index 230's nine bars
```
Full table (8,657 rows: at-risk bars reached, control bars reached, qualifying bars, PF, trades) in `candidate_coverage.csv`.

## 3. THE FOURTEEN REPLACEMENTS — `SWAP14_PREREG.md` rule, 6 LONG / 8 SHORT
```
dir    signal_def                                                        role   at-risk   control bars hit   qualifying   PF
LONG   EMA_Oscillator:hi + Upper_Wick:hi + D2D_Signal:==1                COVER    1            24               48      12.32
LONG   OBV_Macd:lo + Micro_VolOfVol:hi + Harmonic_D2D_Concordance:==0    FILL     1            81              557       2.09
LONG   KAMA_Dist:hi + Volume:hi + D2D_Signal:==1                         FILL     1            70              174       2.44
LONG   KAMA_Dist:hi + D2D_Signal:==1 + Harmonic_D2D_Concordance:==0      FILL     1            66              198       2.99
LONG   OBV_Macd:lo + Sqz_Val:hi + Micro_Hurst:hi                         FILL     1            64              411       2.86
LONG   Bar_Range:hi + Upper_Wick:hi + D2D_Signal:==1                     FILL     1            62              166       2.86
SHORT  OBVf_DirStepCount:lo + EMA_Oscillator:lo + ADX_Value:lo           COVER    5             7              128       2.86
SHORT  D2D_ATR:hi + Sqz_Val:hi + Session_High_Dist_ATR:hi                COVER    2            59              265       4.22
SHORT  Micro_AutoCorr:hi + VA_Position:hi + OR_Position:lo               COVER    2             7              130       3.99
SHORT  RangeOsc_State:==-2 + PrevDay_Close_Side:==-1 + OR_High_Side:==1  COVER    2             3              289      13.18
SHORT  Micro_Amihud:hi + Micro_VolOfVol:lo + OR_Low_Side:==-1            COVER    1             3              172      12.75
SHORT  D2D_Dynamic_Sensitivity:lo + Dist_To_PoC_ATR:lo + VWAP_Sigma:hi   COVER    2             8              102       8.08
SHORT  DailyOpen_Dist_ATR:lo + TChan_A15:hi + PoC_Side:==0               COVER    1            15               76       7.33
SHORT  Micro_MicroGap:lo + VWAP_Sigma:lo + OR_High_Side:==-1             COVER    1             7              138       5.55
```
All 15 at-risk bars covered; none uncoverable. Note the asymmetry the rule produced: the SHORT eight fire on 3–59 control bars each (the fourteen they replace fired on 80), while the LONG fills fire on 62–81.

## 4. THE ACCEPTANCE LINE
```
                                     events  worstBar  worstDay   -wks   days   bars   trades   WR%    PF    MARGIN   net$       breach
297 control (IN-SAMPLE)                42    -1,224    -346.60     0     119    973   5,776   96.12  14.53  33.07   284,974      0
297 − 14 (measured last turn)          41    -1,224    -827.40     1     116    905   5,389   96.07  14.27  32.94   273,409      0
283 + 14 replacements (pre-registered) 62    -1,530    -905.00     0     121  1,044   6,345   94.91   9.44  28.52   299,713      0
300 rows: + 3 more by the same FILL rank 69   -2,082    -736.00     0     121  1,087   6,560   94.57   8.75  28.00   303,483      0
```
The +3 (2 SHORT / 1 LONG: `EMA_Oscillator:lo + Micro_ThrustEff:hi + ADX_Value:lo`, `EMA_Oscillator:lo + ADX_Value:lo + D2D_DirStep:==1`, `OBV_Macd:lo + Volume_Avg_10:hi + Micro_Hurst:hi`) is a post-pre-registration extension to reach a literal 300 rows, labelled as such; the pre-registered object is the 297-row swap.
What the swap does underneath: 13 of the 15 at-risk bars are traded again; 72 control bars are lost and 143 new bars appear (23 events, +$5,920, worst −$1,530 among them). The new bars are where the events and the tail come from.

## 5. MONTHLY — June is where it shows
```
month      297: trades  bars  events  worstDay  losingDays  net     |  swap: trades  bars  events  worstDay  losingDays  net
Jan(5d)        279     40    (1 ev)   +651.0       0      7,470   |        312     47   (3 ev)    +23.6       0      7,059
Feb          1,117    177    (6 ev)   +253.0       0     56,734   |      1,168    181  (10 ev)   +253.0       0     55,390
Mar          1,370    244    (9 ev)   −102.4       1     70,257   |      1,526    261  (12 ev)    +97.6       0     79,592
Apr            950    173    (7 ev)    +89.6       0     42,842   |      1,049    188  (10 ev)   −736.0       1     46,177
May            659    111    (2 ev)    −73.3       1     39,407   |        732    117   (4 ev)   −326.0       1     40,565
Jun            965    164   (14 ev)   −346.6       4     32,591   |      1,043    172  (18 ev)   −905.0       4     33,830
Jul(13d)       436     64    (3 ev)    −56.0       1     35,674   |        515     78   (5 ev)    −56.0       1     37,100
```
Every monthly cell is under 20 events on both books — counts throughout; Jan and Jul rates declined as instructed. June's event count goes 14 → 18 and the book's worst day relocates into June at −$905; April acquires a −$736 day it did not have. No month loses.

## 6. PLAINLY
- **Coverable:** yes — 15 of 15 at-risk bars, 1 LONG + 8 SHORT candidates from the ordinary field at PF ≥ 2.0. The fourteen are replaceable for the function of holding depth on those bars.
- **Constraint:** holds — zero losing weeks, zero breaches, on both the 297-row swap and the 300-row extension.
- **Cost:** 20 more loss events, worst day −$905 (2.6× the control's), worst bar −$1,530, PF 14.53 → 9.44, margin −4.5, for +$14,739 net and +2 days. The tail is the price of the new bars the replacements admit through the same floor-3 jar.
- **Emit-all:** the dependency that existed so SELECT could reproduce the 297 is closed — SELECT can produce a book from the 19,754 field that holds the constraint. It does not reproduce the 297's tail, and the operator should rule knowing that.

## ARTIFACTS (`SELECT/swap14/`)
`at_risk_bars.csv` · `candidate_coverage.csv` (8,657) · `setcover_LONG.json`, `setcover_SHORT.json` · `replacements.csv` · `whole_dot_300_signals.csv` (297 rows, the pre-registered swap) · `whole_dot_300_rows.csv` (300 rows) · `result_300.json`, `result_300rows.json` · `monthly_297_vs_300.csv` · `find.py`, `score300.py`, `round300.py` + logs · `SWAP14_PREREG.md` · patches `0001`, `0002`.
