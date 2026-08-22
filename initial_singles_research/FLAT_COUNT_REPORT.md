# THE FLAT CONDITION COUNT — ONE DIAL, SWEPT
Quant seat · 2026-08-22 · HEAD `74be5e87` (2026-08-22 15:37 +0700; diff from `4ecd2ba0` is my decomposition artifacts only, engine untouched) · frame `46586cbb1671` · oracle `518862bf19fb` · `[adm_engine] rule=FLOORED cap=21 …` read on all 64 runs.
All figures at 1.0 lot, in-sample. **Cumulative engine trial count this report: 64** (2 matched-pair runs × 2 conventions, 15 LONG floors × 2, 15 SHORT floors × 2, 2 no-tier diagnostics). One free parameter swept; no gate added, no threshold tuned, no selection step.

## 0. CONTROL — reproduced to the cent through the same scoring path used for every row below
```
[adm_engine] rule=FLOORED cap=21 floor={1: 3, -1: 3} tier-gates=4   GATE MASKS HU90 9.7478% FB20 80.1874% ATS90 6.2217%
297: 42 loss events · worst bar -$1,224 · worst day -$346.60 · 0 losing weeks of 26 · 119 days · 973 entry bars · 5,776 trades
     WR 96.12 · PF 14.53 · BE-WR 63.05 · MARGIN 33.07 · net $284,974  (folds 6/6, min-fold PF 11.0)
BOOK-50 canary (prior run this session): 3,101 / 90.6 / 4.81 / $97,675 — ENGINE INTACT
297 at ONE 1.0-LOT POSITION PER ENTRY BAR (the comparator for the per-bar view): PF 10.25 · 2 losing weeks · worst day -$105.50 · net $33,633
```

## THE COUNT, AS BUILT
`sequential_temporal.build_condition_pool` (249 oracle masks) → `concurrence_profiler.align_pool` (the F12 rule: LONG = `:hi` + `==v>0`, 117 conditions; SHORT = `:lo` + `==v<0`, 110; 22 `==0` neutral excluded) → `depth_arrays`. Same pool, same alignment, same oracle as F12; nothing recomputed.

**Check against the flagged expectation:** on all 170,351 post-warmup bars this count gives LONG min 8 · max 81 · mean 29.26 · median 27 and SHORT min 2 · max 61 · mean 24.05 · median 23 — identical to the `concurrence_depth_bars.csv` figures, which are therefore the same construction on the non-eligible frame.

## STEP 1 — THE DISTRIBUTION, CAUSAL, ON THE ELIGIBLE FRAME
Eligible = entry_ok (ADX ≥ 15, Volume > 50, post-warmup, not Friday-close) & D2D agreement; "+ATR" adds ATR_1M ≥ 20 where the engine applies it.
```
LONG  eligible        n=50,091   min 9   max 81   mean 35.48  median 34   p75 43  p90 51  p95 56  p99 64  p99.5 67  p99.9 72
LONG  eligible + ATR  n=14,160   min 12  max 81   mean 41.87  median 41   p75 50  p90 57  p95 61  p99 68  p99.5 71  p99.9 75
SHORT eligible        n=53,103   min 5   max 61   mean 29.01  median 28   p75 35  p90 42  p95 46  p99 51  p99.5 53  p99.9 57
SHORT eligible + ATR  n=16,761   min 6   max 61   mean 32.65  median 32   p75 41  p90 47  p95 50  p99 54  p99.5 56  p99.9 58
```
Bars at or above each count, eligible + ATR (the population the floor admits):
```
LONG  k:  40:7954  45:5738  50:3715  55:2078  58:1362  60:975  61:816  62:672  63:563  64:467  65:370  66:295  67:229  68:181  70:114  72:56  75:16  78:5  81:2
SHORT k:  40:4695  45:2442  48:1383  50:860  51:664  52:465  53:345  54:230  55:149  56:88  57:52  58:25  59:14  60:7  61:5
```
A floor of 3 admits 100% of both; a floor of 8 admits 100% of LONG and all but a handful of SHORT. Confirmed: the two PF ≈ 1.3 runs in the record were measured below the floor of this distribution.

## STEP 2 — THE MATCHED FLOOR
Target per direction = the 297's entry bars (601 LONG / 372 SHORT). Nearest integer, not rounded:
```
LONG  k=63 admits 563 eligible bars   (k=62: 672, k=64: 467)
SHORT k=53 admits 345 eligible bars   (k=52: 465, k=54: 230)
pair (L63, S53) admits 908 against 973
```

## THE CONVENTION, NAMED (the complication the brief required)
`adm_engine`'s FLOORED rule means "admit at least *floor* lots", and the cap is 21, so `ADM_FLOOR = 63` refuses every bar (verified: 0 trades). The floor therefore lives in a synthetic column (`__FLAT_L = 1` where count ≥ k; D2D, entry_ok and ATR applied by the engine exactly as for the 297) and `ADM_FLOOR = {1:1, -1:1}`. A floor-level bar is deep by construction, so it takes the 297's **5+ cell** (LONG `Micro_FailedBreak > p20`, SHORT free), keyed to depth 1 where the detector lands. Every admitted bar opens one position, sized by the same conviction multipliers as the 297. Two executable forms of "no signals":
- **ONE** — one detector per direction. The engine's one-position-per-signal lock becomes *one open position per direction at a time*; a qualifying bar while that position is open is skipped.
- **RR** — the qualifying bars are assigned in time order to 30 detectors (bar *i* → detector *i* mod 30), so every qualifying bar fires exactly one detector: **one 1.0-lot position per admitted bar, no lock, cap 21.** This is the form the brief describes. Max concurrency reached 9–17; the cap never bound.
"One position per bar with no lock" is not expressible in the sacred engine through a single detector; RR expresses it without touching the engine. Both are reported on every row.

## STEP 3 — THE SWEEP
Order: loss events → worst bar → worst day → losing weeks of 26 → days of 132 → entry bars → trades → WR → PF → MARGIN → net last. Worst bar is −$306 at every floor (MAX_RISK 150 × conviction 2.0). `elig` = eligible+ATR bars at the floor. Rows under 20 loss events are COUNTS; rates on them are not quoted.

**LONG alone (SHORT off), convention RR:**
```
floor  elig  lossEv  worstDay  losingWk  days  entryBars  trades   WR%    PF   MARGIN    net$     maxConc  folds+
 40   7954   1374   -1831.8     14      126     6836     6836   79.90  1.01   +0.24    2,258      14       4
 45   5738   1026   -1773.1     16      126     5041     5041   79.65  0.97   -0.55   -3,922      13       2
 50   3715    705   -2135.5     17      122     3315     3315   78.73  0.89   -1.84   -8,912      13       1
 55   2078    422   -1433.9     16      104     1909     1909   77.89  0.82   -3.26   -9,440      11       2
 60    975    193   -1524.7     19       79      914      914   78.88  0.83   -3.02   -4,332      10       2
 61    816    153   -1254.9     18       71      770      770   80.13  0.88   -1.95   -2,447      10       3
 62    672    115   -1151.4     14       59      633      633   81.83  0.98   -0.31     -329      10       3
 63    563     92   -1012.7     12       51      534      534   82.77  0.94   -0.86     -796       9       4   <- matched
 64    467     74    -777.7     12       48      445      445   83.37  0.93   -0.91     -705       8       2
 65    370     53   -1092.1     12       39      355      355   85.07  1.01   +0.16      101       8       2
 66    295     46   -1118.7     11       32      285      285   83.86  0.93   -0.96     -471       8       3
 70    114      6    -140.3      2       20      113      113    (6 events — count)            1,455       7       5
 75     16      2    -131.6      1        5       16       16    (2 events — count)               33       3       1
 80      3      0      —         0        2        3        3    (0 events — count)               38       1       1
 85      0      —
```
**SHORT alone (LONG off), convention RR:**
```
floor  elig  lossEv  worstDay  losingWk  days  entryBars  trades   WR%    PF   MARGIN    net$     maxConc  folds+
 40   4695    893   -1322.4     13      121     4675     4675   80.90  1.01   +0.14      839      17       3
 45   2442    478    -842.9     15      112     2440     2440   80.41  0.95   -0.72   -2,457      14       1
 50    860    160    -722.1     17       90      860      860   81.40  0.97   -0.41     -569      12       2
 51    664    124    -722.1     14       84      664      664   81.33  0.99   -0.17     -186      12       2
 52    465     87    -722.1     14       76      465      465   81.29  1.02   +0.31      251      11       3
 53    345     66    -591.8     11       72      345      345   80.87  1.03   +0.51      309       8       2   <- matched
 54    230     37    -438.8     10       67      230      230   83.91  1.29   +3.70    1,515       7       2
 55    149     22    -306.0      9       55      149      149   85.23  1.39   +4.62    1,255       6       2
 56     88      8    -153.0      5       40       88       88    (8 events — count)            1,738       5       2
 60      7      1     -69.0      1        5        7        7    (1 event — count)               296       3       2
 65+     0      —
```
**Convention ONE** (one open position per direction at a time) on the same floors: LONG 40–66 PF 0.78–1.11, losing weeks 9–16, entry bars 123–1,991; SHORT 40–55 PF 0.87–1.22, losing weeks 9–16. Full table in `flat_sweep_results.csv`.

**The matched pair, both conventions, with and without the 5+ tier cell:**
```
                      lossEv  worstBar  worstDay  losingWk  days  entryBars  trades   WR%    PF   MARGIN    net$    maxConc  | 1-lot/bar view: losingWk  PF    net
L63/S53 ONE  tier on     83    -306     -634.4      12      103     420      420   80.24  0.90   -1.64   -1,208       1     |     14      0.93   -749
L63/S53 RR   tier on    158    -306   -1012.7      13      103     879      879   82.03  0.98   -0.32     -487       9     |     13      1.02    414
L63/S53 ONE  tier off    85    -306     -718.7      12      103     425      425   80.00  0.87   -2.08   -1,551       1     |     13      0.92   -896
L63/S53 RR   tier off   167    -306   -1078.7      13      103     908      908   81.61  0.96   -0.55     -855       9     |     13      1.01    166
```
The shape: **a plateau, at break-even.** PF sits between 0.82 and 1.03 across every LONG floor from 40 to 66 and every SHORT floor from 40 to 53, with 9–19 losing weeks of 26 on every one of those rows, on both conventions. It is robust to the floor and robustly not working. The rising numbers at LONG ≥ 70 (114 bars, 6 events) and SHORT ≥ 56 (88 bars, 8 events) are counts, and the floor one step below each (LONG 66: PF 0.93 on 46 events; SHORT 55: PF 1.39 on 22) shows what using them would be — a knife-edge on a population too small to rate.

Survival: no floor breaches the $2,500 daily ceiling at 1.0 lot (worst day −$2,135.50 at LONG 50 RR). The binding constraint — the losing week — fails at every floor with ≥ 20 events.

## STEP 4 — THE DIAGNOSTICS
**A. Bar overlap with the 297.** The 297's 973 entry bars, placed in the flat-count distribution of their own eligible population:
```
LONG  601 bars: count min 16 · p25 34 · median 41 · p75 47 · max 74 · mean 41.0  — eligible-population median is 41
      median percentile-rank 0.470 · 13 of 601 reach the matched floor (63)
      by count band  <35: 163 bars / 11 loss events · 35-44: 230 / 13 · 45-54: 154 / 9 · 55-64: 43 / 0 · 65+: 11 / 1
      Spearman(count, bar P&L per lot) +0.2008 p 7.0e-07 n 601 · Spearman(count, admitted signal depth) -0.1422
SHORT 372 bars: count min 9 · p25 23 · median 30 · p75 42 · max 61 · mean 32.5  — eligible-population median is 32
      median percentile-rank 0.404 · 29 of 372 reach the matched floor (53)
      by count band  <35: 232 / 5 · 35-44: 64 / 2 · 45-54: 56 / 1 · 55-64: 20 / 0
      Spearman(count, bar P&L per lot) +0.2864 p 1.9e-08 n 372 · Spearman(count, admitted signal depth) +0.2041
matched floor RR: 879 entry bars · 42 shared with the 297 · 931 of the 297's 973 missed · 837 flat bars the 297 never traded
      shared 42 bars: 1 loss event, net $1,696 (the 297 on the same 42: 1 loss event, $8,452)
      the other 837: 157 loss events, PF 0.90, net -$2,183
```
The 297's bars sit at the median of the count distribution. The flat rule at the matched floor finds different bars (42 of 973 in common), and on the bars both systems reach, both are fine on a count of 1 loss event; the 837 bars only the count reaches are where the 158 events live. Within the 297's own bars the count is positively associated with per-lot P&L (p ~1e-7, n 601/372), and on the LONG side it is negatively associated with signal depth.

**B. Monthly, same table as the 297 (exit month, 1.0 lot). January 4–5 days, July 11–13: counts.**
```
              297 control                        |  flat L63/S53 RR                          |  flat L63/S53 ONE
month  trades bars  WR%    PF   ev  worstDay    |  trades  WR%    PF   ev  worstDay  losD  |  trades  WR%    PF   ev  worstDay  losD
01       279   40  98.57 19.49   1    +651.0    |    10   90.00  1.14   1   -142.7    1    |     8   87.50  0.90   1   -142.7    1
02     1,117  177  97.22 21.77   6    +253.0    |   166   88.55  2.10  19   -239.8    7    |    82   87.80  1.71  10   -153.0    5
03     1,370  244  96.64 12.50   9    -102.4    |   209   77.51  0.69  47  -1012.7   11    |   113   76.11  0.80  27   -634.4   11
04       950  173  96.42 16.58   7     +89.6    |   199   82.91  1.12  34   -581.7   10    |    75   77.33  0.88  17   -234.7    8
05       659  111  98.18 60.64   2     -73.3    |    95   78.95  0.64  20   -591.8    9    |    49   81.63  0.70   9   -306.0    7
06       965  164  91.19  5.34  14    -346.6    |   143   79.72  0.88  29   -612.0    8    |    55   74.55  0.72  14   -306.0    8
07       436   64  97.25 40.69   3     -56.0    |    57   85.96  1.10   8   -150.3    5    |    38   86.84  1.12   5    -99.0    5
```
June is not the flat rule's bad month; March is (PF 0.69, 47 events, worst day −$1,012.70, 11 losing days), and March is the 297's third-best month. The flat rule's only month above PF 2 is February. The two systems' loss structures do not share a calendar.

## WHAT THE DATA GIVES
The flat count of aligned conditions, with the 297's participation floor, frame, engine, cap, ATR gate and 5+ tier cell, at the rarity-matched floor and across 40–85 on both sides and both executable conventions, trades at **PF 0.82–1.03 with 9–19 losing weeks of 26** wherever there are 20 or more events to rate, and reaches 42 of the 297's 973 bars. The 297's bars are ordinary in condition count (median rank 0.47 / 0.40). Per the brief's own test: **coverage by a raw count of extremes does not do the work the book does; the detectors carry something the count does not.** No second dial was wanted: the sweep was flat, not a knife-edge, so there was nothing a second parameter could have rescued. Reported as the data gives it.

## ARTIFACTS
`flat_sweep_results.csv` (64 rows, every column above plus the 1-lot/bar view) · `trades_L63_S53_RR.csv` · `trades_L63_S53_ONE.csv` · `decomp_scripts/flat_engine.py` (modes: dist / control / matched / sweepL / sweepS / matched_notier). Console for step 1 carries the full count histogram from 35 upward per population.
