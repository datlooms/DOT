# WILL THE 297 HOLD — FIVE MEASUREMENTS
**PUSH FAILED ON CREDENTIALS** (`could not read Username for 'https://github.com'`; no token, no stored credentials, no `ssh`). Two commits shipped as `git am`-able patches on base `db3687a95957`: `0001` = `ADDITIONS_PREREG.md` (`a1bfdfe`, 11:16:59 UTC, before the additions run), `0002` = `initial_singles_research/hold/` (`b6a0ec4`: every table, script and log below). **The numbers cannot be verified at HEAD until these land.**

Quant seat · 2026-08-22 · frame `46586cbb1671` · oracle `518862bf19fb` imported · `[adm_engine] rule=FLOORED cap=21 floor={1: 3, -1: 3} tier-gates=4` read on every run · 1.0 lot.
**Trials.** Engine 13 this report (control + 9 single additions + 3 cumulative) → **cumulative 193**. Statistical 1,089 this report (996 Fisher on three populations, 66 conjunction cells, 24 k-of-n forms, 3 Spearman) → **cumulative 18,841**.
Control reproduced to the cent, twice (flat path and additions path): 42 · −1,224 · −346.60 · 0 of 26 · 119 · 973 · 5,776 · 96.12 · 14.53 · 33.07 · $284,974. **Every 297 figure in this document is IN-SAMPLE.**

## 1. WHAT 297\A IS — the mechanism the turn description misses
Populations (full frame): LONG 297\A 348 · A∩297 253 · eligible untraded 13,559; SHORT 297\A 288 · A∩297 84 · untraded 16,389. Each traded population hour-matched to the untraded, enumerated; 996 Fisher tests, BY q < 0.10 per direction (`m1_three_population_lift.csv`).

**First, the obvious check — is 297\A the location layer without the turn? No. Location sits on both halves of the book at the same rate.**
```
location count (of LOC_d) >= k      297\A    A∩297    A\297 (losers)   eligible untraded (hm)
LONG  >=1                            0.670    0.644      0.474           0.336
LONG  >=2                            0.394    0.439      0.328           0.186
LONG  >=3                            0.178    0.281      0.208           0.092
SHORT >=1                            0.576    0.857      0.587           0.328
SHORT >=2                            0.163    0.500      0.216           0.055
SHORT >=3                            0.028    0.298      0.088           0.010
```
LOC_d (top location conditions by lift on all traded bars, ≥ 20 bars): LONG `VAH_Dist_ATR:lo` ×2.36 · `VAL_Dist_ATR:lo` ×2.33 · `Dist_To_PoC_ATR:lo` ×2.30 · `Session_Low_Dist_ATR:lo` ×2.25 · `PrevDay_Low_Side:==-1` ×2.23 · `MultiDay_Position:lo` ×2.04; SHORT `VAL_Side:==0` ×3.57 · `VAH_Side:==0` ×2.95 · `PoC_Side:==0` ×2.72 · `PrevDay_High_Side:==1` ×2.32 · `Session_High_Dist_ATR:lo` ×2.25. Location is a property of the book as a whole (and of A's losers, at 47–59%), not of the half the turn misses. On SHORT it is actually denser on A∩297 than on 297\A.

**What is true on 297\A and not on A∩297 (BY-significant, like-for-like bar-truth; untraded rate alongside so "book marker" is visible):**
```
LONG                         297\A rate   A∩297 rate   untraded(hm)   lift_297\A   lift_A∩297
Slope_EMA_ST:hi                 0.569        0.087        0.617         0.92         0.14
AT_Slope_ST:hi                  0.560        0.123        0.609         0.92         0.20
AT_Regime_ST:==0                0.670        0.245        0.835         0.80         0.29
RangeOsc_State:==1              0.491        0.194        0.670         0.73         0.30
Harmonic_OBVf_Concordance:==1   0.414        0.099        0.463         0.89         0.22
KAMA_Dist_ATR:hi                0.316        0.075        0.351         0.90         0.22
Slope_Accel_LT:hi               0.408        0.146        0.334         1.22         0.44
D2D_Persist:hi                  0.368        0.111        0.390         0.94         0.29
AT_Score_ST:hi                  0.187        0.020        0.241         0.78         0.08
SHORT
D2D_DirStep:==1                 0.500        0.012        0.282         1.77         0.04
Sqz_Val:hi                      0.406        0.012        0.114         3.56         0.11
Momentum_Value:hi               0.274        0.000        0.084         3.28         0.00
Micro_TickIntensity:hi          0.285        0.024        0.180         1.59         0.18
Micro_HLAsymmetry:hi            0.354        0.036        0.297         1.19         0.12
KAMA_Dist:hi                    0.122        0.012        0.036         3.41         0.30
DailyOpen_Dist_ATR:hi           0.292        0.060        0.119         2.44         0.65
```
On LONG, the short-term slope family on 297\A sits at the **untraded base rate** (0.57 vs 0.62) — it is not a selector there, it is simply not pinned. What selects 297\A against the untraded (top lifts, ≥ 20 bars): `KAMA_Dist:lo` ×5.4 (47b) · `KAMA_Slope:lo` ×4.4 (29b, not in book) · `ST_Flip_Event:==-1` ×4.4 (37b; 0 on A∩297) · `OBV_Macd:lo` ×2.8 (118b) · `AT_Score_LT:lo` ×2.6 · `PrevDay_Low_Side:==-1` ×2.4 (140b) · `Session_Low_Dist_ATR:lo` ×2.2 · `AT_Slope_LT:lo` ×2.2 (142b) · `Slope_EMA_LT:lo` ×2.1 (142b) · `Micro_Rejection:lo` ×2.0 · `Micro_Hurst:hi` ×1.8 (128b). That is: **long-term slope low, price under the day's references, KAMA distance low, OBV momentum low — with the short-term slope already turned up.** On SHORT, 297\A carries `D2D_DirStep:==1` on half its bars (vs 1% of A∩297), `Sqz_Val:hi` ×3.6 (117b), `Momentum_Value:hi` ×3.3, `OBV_Velocity:hi` ×3.7, `Micro_Hurst:hi` ×2.4 (132b), `DailyOpen_Dist_ATR:hi` ×2.4 — **squeeze and momentum high, far from the open, a D2D counter-step on the bar.** Neither half is the turn described by A; by A's construction 297\A carries ≤ 1 turn condition (LONG: 0 on 299 bars, 1 on 45; SHORT: 0 on 158, 1 on 130).

**The conjunction, priced (turn ∧ location as bar-truth on all 973 vs untraded-hm; 66 cells):**
```
LONG  36 cells, 29 with >= 20 traded bars: observed/product-of-marginals median 0.61 · above 1.25: 0 · below 0.80: 24
      best cell D2D_Signal:==1 ∧ PrevDay_Low_Side:==-1  lift 14.3 vs product 16.5 (0.87), 40 bars — 0 on 297\A, 40 on A∩297, 78 on A\297
SHORT 30 cells, 10 with >= 20 bars: median 0.82 · above 1.25: 0 · below 0.80: 5
      best cell D2D_Dynamic_Sensitivity:lo ∧ VAL_Side:==0  11.9 vs 11.3 (1.06), 27 bars — 0 on 297\A, 27 on A∩297, 88 on A\297
k-of-n: LONG turn>=2 ∧ loc>=1 lift 10.0 vs product 14.1 (164 bars: 1 on 297\A, 163 on A∩297, 330 on A\297); SHORT turn>=2 ∧ loc>=1 7.9 vs 11.1 (72: 0 / 72 / 373)
```
At or below the product everywhere. Turn and location are independent-to-sub-multiplicative; the conjunction is not the mechanism, and every conjunction cell lands on A's bars (both its winners and its losers), never on 297\A.

**Sign per direction, derived in-fold (measurement 3 sets, disjoint months):** LONG location is `==-1` / `Dist_ATR:lo` in every derivable month (under VWAP, under yesterday's low, under the open, near the levels). SHORT is NOT the mirror: on the pooled frame `==0` (sitting on VAL/VAH/PoC) and `PrevDay_High_Side:==1`; in-fold it alternates between `Dist_ATR:lo` months (Feb, Mar, May) and `Dist_ATR:hi` months (Apr) and never repeats a set — see §3. The asymmetry is real on the pooled frame (long trades beyond the level, short trades at it) and unstable month to month.

## 2. DOES THE MEMBERSHIP DECAY, OR THE BOOK? (in-sample book run, per signal per entry-month)
```
of 297 signals: present in all 6 sacred folds 92 · in 5 97 · in <=2 19
positive net in all 6 folds 66 · in 5 of 6 103 · in exactly 1 fold 4 · in 0 folds 0
positive in EVERY fold in which they trade: 247 of 297
split-half (Jan-Apr vs May-Jul; 282 signals present in both):
   Spearman per-signal NET        +0.2061   p 4.96e-04
   Spearman per-signal PF         -0.0237   p 0.69      (most signals have 0-2 losses per half; PF ties dominate)
   Spearman per-signal TRADE COUNT +0.5225  p 3.68e-21
```
```
month    book bars  events  book PF   net      signals active  positive  share   signals with a loss
Jan(19-31)   40       1     (1 ev)    7,470        142          138     0.972          4
Feb         177       6     21.77    56,734        261          253     0.969         29
Mar         244       9     12.50    70,257        268          260     0.970         38
Apr         173       7     16.58    42,842        240          237     0.988         31
May         111       2     (2 ev)   39,407        234          231     0.987         12
Jun(1-25)   164      14      5.34    32,591        256          227     0.887         71
Jul(1-21)    64       3     (3 ev)   35,674        188          179     0.952         12
```
The book's monthly PF does not track a decaying average signal: the share of signals positive is 0.97–0.99 in every month but June, and in June it is 0.89 because **71 signals lose in the same month** (vs 12–38 elsewhere) — the book's bad month is a common shock across members, not individuals rotting on their own schedules. Per-signal rank does not persist across halves (NET +0.21, PF −0.02), activity does (+0.52): the book is a portfolio whose members are near-interchangeable on performance rank and stable on how often they fire. No signal is negative in every fold; no signal is positive in only one fold except 4.

## 3. DOES THE DESCRIPTION HOLD WHILE MEMBERSHIP CHURNS? (same folds, side by side)
Turn set and location set re-derived IN-FOLD (disjoint months, ≥ 20 traded bars, top-6/5 by hour-matched lift); membership = the set of signals positive in the fold.
```
consecutive-fold Jaccard          01→02  02→03  03→04  04→05  05→06  06→07
LONG  TURN (in-fold)               0.00   0.50   0.00   0.09   0.33   0.33
LONG  LOCATION (in-fold)           0.20   0.20   0.20   0.09   0.50   0.20
SHORT TURN (in-fold)                nan   0.09   0.00   0.00   0.00   0.00
SHORT LOCATION (in-fold)            nan   0.00   0.25   0.00   0.00   0.00
MEMBERSHIP positive-set (all 297)  0.46   0.76   0.71   0.70   0.68   0.56
membership positive-set, Jan-Apr vs May-Jul: LONG 190 vs 173 signals, Jaccard 0.90 · SHORT 105 vs 97, Jaccard 0.91
```
Each fold's in-fold turn set vs the full-frame turn set: LONG 0.00 / 0.50 / 0.50 / 0.00 / 0.20 / 0.50 / 0.20; SHORT 0.00–0.09 every month. The cumulative-prefix derivation (TURN_RULE_REPORT §A) is stable on LONG from 148 traded bars because it accumulates; on disjoint months the LONG description churns at Jaccard 0.0–0.5 and the SHORT description never repeats (April's SHORT "turn" set is six `Dist_ATR:hi` location conditions; July's is `D2D_ATR:hi` ×1.07 — nothing). **Plainly: on the same folds the description churns MORE than the membership does.** The membership's positive-set is stable at 0.56–0.76 month to month and 0.90 across halves — because 247 of 297 never have a losing fold. The 88% churn on the record is re-DISCOVERED membership (which signals a fresh search selects), a different object from whether the selected 297 stay positive; both are now measured and they point opposite ways.

## 4. DOES THE 297 NEED ADDITIONAL TRIPLES? (`ADDITIONS_PREREG.md`, additions only, IN-SAMPLE)
Universe: catalogue_F0 VALID not in the 297 = 1,637. MISSING-TURN class (contains a non-book top-8 turn condition on its side): 14, all LONG, all carrying `KAMA_Slope:lo` — **no VALID triple contains `AT_Score_ST:lo`, `D2D_Up_Count:hi` or `D2D_Persist:lo` on LONG, nor any of the four SHORT missing conditions.** LOCATION class: 163 (103 L / 60 S). Selected by the pre-registered rule (top-3 per class × direction by NEW bars with the 297\A profile): 9.
```
addition                                                            lossEv  worstBar  worstDay  losingWk  days  entryBars(Δ)  trades   WR%    PF     MARGIN   net$ (Δ)
CONTROL 297                                                            42    -1,224    -346.6     0       119    973            5,776   96.12  14.53  33.07  284,974
L Bar_Range:hi + KAMA_Slope:lo + PrevDay_High_Side:==-1   (93 new q-bars)  42  -1,224  -346.6   0   119   977 (+4)   5,808  96.11  14.42  32.96  285,524 (+550)
L D2D_ATR:hi + KAMA_Slope:lo + PrevDay_High_Side:==-1     (98)             42  -1,224  -346.6   0   119   978 (+5)   5,816  96.11  14.42  32.94  285,631 (+657)
L ATR_1M:hi + KAMA_Slope:lo + PrevDay_Close_Side:==-1     (70)             42  -1,224  -346.6   0   119   976 (+3)   5,804  96.12  14.47  32.97  285,398 (+424)
L EMA_Oscillator:hi + PrevDay_Low_Side:==-1 + OR_Low_Side:==1  (426)       44  -1,224  -346.6   0   119   999 (+26)  5,904  96.02  13.97  32.69  290,875 (+5,901)
L Volume:hi + PrevDay_Low_Side:==-1 + OR_Low_Side:==1     (292)            42  -1,224 -1,020.4  0   119   991 (+18)  5,886  96.11  14.22  32.65  291,126 (+6,152)
L PrevDay_High_Dist_ATR:lo + WeeklyOpen_Dist_ATR:lo + PrevDay_Low_Side:==-1 (247)  45  -1,224  -346.6  0  120  988 (+15)  5,896  95.91  13.39  32.25  287,842 (+2,868)
S OBVf_Signal:==-1 + VAH_Side:==0 + PrevDay_Low_Side:==1  (175)            42  -1,224  -346.6   0   119   986 (+13)  5,842  96.17  14.60  32.96  286,543 (+1,569)
S Slope_EMA_ST:lo + VAH_Side:==0 + PrevDay_Low_Side:==1   (157)            42  -1,224  -346.6   0   119   987 (+14)  5,845  96.17  14.61  32.96  286,659 (+1,685)
S WeeklyOpen_Dist_ATR:hi + TChan_A15:hi + PrevDay_High_Side:==1 (147)      48  -1,224  -346.6   0   120   991 (+18)  5,866  95.82  13.66  33.14  285,094 (+120)
297 + MISSING-TURN class (3)                                           45    -1,224    -410.0     0       119    984 (+11)     5,880   95.85  13.63  32.96  286,019 (+1,045)
297 + LOCATION class (6)                                               54    -1,224  -1,020.4     0       120  1,078 (+105)    6,373   95.70  12.42  31.51  303,459 (+18,485)
297 + all 9                                                            57    -1,224    -881.5     0       120  1,089 (+116)    6,475   95.46  11.74  31.29  304,174 (+19,200)
```
A triple that qualifies on 70–426 new bars adds only 3–26 entry bars, because floor 3 admits nothing alone: an addition trades where it co-fires with two existing members, and otherwise adds a lot to bars already traded. Coverage rises 12% for all nine together; loss events rise 42 → 57; worst day −346.60 → −881.50 (one addition alone, `Volume:hi + PrevDay_Low_Side:==-1 + OR_Low_Side:==1`, moves it to −1,020.40); **zero losing weeks survives on every row — in-sample, selected on the same frame.** No addition reaches 297\A by construction (those bars are already traded); the additions reach A's losers' bars and new bars in the same state.

## 5. THE LABEL
Every 297 figure above is IN-SAMPLE: selected on Jan 19–Jul 21 and scored on it. There is no out-of-sample figure for the 297 in the record and this report does not create one. The closest proxy that exists is §2's per-fold consistency of the in-sample book — 0 losing months, share of members positive 0.89–0.99, June's 14 events on 164 bars — and it is reported as that, not as OOS. The only out-of-segment numbers in this program are the turn rule's (PF 1.22–1.49 per fold, 6 losing weeks of 25) and A+B's (PF 1.27–1.67, 6–9 of 25): those are what a rule derived without the book delivers on unseen months.

## WHAT THE FIVE SAY TO THE AGENDA QUESTION
- The book is two moments per side, not one: A's pinned-slope turn (42% LONG / 23% SHORT of its bars) and a second state — LONG: long-term slope down, price under the day's references, KAMA distance low, short-term slope already up; SHORT: squeeze and momentum high, D2D counter-step — that the turn description cannot see and that earns more (+$20,600 on 636 bars, 27 events) than the half it sees. Location sits on both halves and on the losers; the conjunction prices at or below independence.
- Membership does not decay in-sample: 247 of 297 never post a losing month; the book's one bad month is 71 members losing together.
- The description churns more than the membership on the same folds; the SHORT description does not re-derive in any single month. Refreshing from a stable description is not available — on this frame the stable object is the selected set, not the words for it.
- Additions from the VALID catalogue change coverage by single digits each and keep zero losing weeks in-sample; the missing turn conditions have no VALID triple at all except `KAMA_Slope:lo`.
- Whether the set holds out of sample is unmeasured by construction. It will be measured the first time the 297 trades a month it was not selected on.

## ARTIFACTS (`hold/`)
`m1_three_population_lift.csv` (498 rows) · `m1_conjunction_pricing.csv` (66) · `m1_kofn_forms.csv` (24) · `m2_per_signal_per_fold.csv` (1,589) · `m2_book_vs_signals_monthly.csv` · `m3_churn.json` · `additions_candidates.csv` (177) · `additions_selected.csv` · `additions_results.csv` (13 rows) · `measure_hold.py` + log · `additions.py` + log · `ADDITIONS_PREREG.md` · patches `0001`, `0002`.
