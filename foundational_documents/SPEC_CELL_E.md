# SPECIFICATION — CELL E
## U299 union · Option B gate stack at p90 · LONG ≥ 4 / SHORT ≥ 3 · MAX_POSITIONS 12 · 1 lot

**Research document. Nothing implemented. In-sample on DOT_stitched172 TRUEEST, 177,251 × 172, 2026.01.19–2026.07.21. Gaps excluded, conviction stack as adopted.**

---

## 0. READ FIRST — THE LOSS SAMPLE

**THE 246 LOSSES ARE 56 DISTINCT BAR-EVENTS.** Cluster sizes run 3 to 9 (one bar of 9, three of 8, five of 6, ten of 5, twenty-two of 4, fifteen of 3).

**Unlike cell D — where 48 losses collapsed to 7 events — cell E's 56 events are a usable statistical sample.** This is the only union configuration in the study whose loss statistics can carry a confidence interval.

**All 56 events are `SL` exits.** No BE or LF exit ever lost. One failure mode.

**Direction: 38 LONG / 18 SHORT. Path: 38 BASE / 18 MOM.**

---

## 1. HEADLINE

| | |
|---|---|
| trades | **5,273** |
| winners / losers | 5,027 / **246** (56 events) |
| **win rate** | **95.33%** |
| gross profit / gross loss | $272,051 / $23,769 |
| **net** | **$248,282** |
| **profit factor** | **11.45** |
| $/trade | **$47.09** |
| average win / average loss | $54.12 / $96.62 |
| **win/loss ratio** | **0.56** |
| expectancy | $47.09/trade |
| largest single win / loss | $1,167.0 / **−$191.2** |
| longest winning streak | **411** |
| longest losing streak | 12 |
| bars traded | 1,081 |
| trading days used | **124 of 132** |

**Note the win/loss ratio of 0.56: average loss is 1.8× average win.** The book is a high-win-rate, small-edge machine — 95.33% WR carrying a negative payoff ratio.

## 2. PER MONTH

| month | trades | W | L | WR | PF | net | $/tr | worst day | best day | losing days | **worst intraday** | peak at-risk |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| 2026.01 | 195 | 191 | 4 | 97.9% | 10.44 | $7,219 | $37.0 | +$616.7 | $3,012 | 0 | −$639.3 | 12 |
| 2026.02 | 971 | 924 | 47 | 95.2% | 10.39 | $42,248 | $43.5 | +$54.3 | $10,862 | 0 | −$1,318.8 | 12 |
| 2026.03 | 1,268 | 1,228 | 40 | 96.8% | **16.82** | **$61,420** | $48.4 | +$427.0 | $18,927 | 0 | −$920.3 | 12 |
| 2026.04 | 958 | 926 | 32 | 96.7% | 17.26 | $40,912 | $42.7 | +$219.9 | $11,971 | 0 | −$796.5 | 12 |
| 2026.05 | 617 | 588 | 29 | 95.3% | 11.44 | $29,613 | $48.0 | −$337.1 | $11,820 | **2** | −$1,367.9 | 12 |
| **2026.06** | 808 | 745 | **63** | **92.2%** | **6.54** | $33,708 | $41.7 | −$303.0 | $11,496 | **3** | −$1,458.1 | 12 |
| 2026.07 | 456 | 425 | 31 | 93.2% | 11.41 | $33,163 | **$72.7** | **−$876.6** | **$20,018** | 1 | **−$2,352.1** | 12 |

**June is the weak month: 92.2% WR and PF 6.54, against March's 96.8% and 16.82.**

## 3. PER WEEK — **26 weeks, ZERO losing**

| week | trades | W | L | net | worst day |
|---|---|---|---|---|---|
| 01-26 | 195 | 191 | 4 | $7,219 | +$616.7 |
| 02-02 | 316 | 300 | 16 | $8,335 | +$77.4 |
| 02-09 | 338 | 326 | 12 | $10,439 | +$54.3 |
| 02-16 | 131 | 120 | 11 | $6,467 | +$84.4 |
| 02-23 | 186 | 178 | 8 | $17,007 | +$566.7 |
| 03-02 | 397 | 382 | 15 | $12,792 | +$1,058.7 |
| 03-09 | 223 | 214 | 9 | $11,871 | +$869.4 |
| 03-16 | 171 | 166 | 5 | $4,564 | +$427.0 |
| 03-23 | 310 | 302 | 8 | $27,264 | +$1,856.7 |
| 03-30 | 303 | 300 | 3 | $13,099 | +$2,136.1 |
| 04-06 | 315 | 309 | 6 | $16,740 | +$564.3 |
| 04-13 | 194 | 172 | **22** | $4,777 | +$340.2 |
| 04-20 | 229 | 225 | 4 | $9,008 | +$466.8 |
| 04-27 | 100 | 100 | **0** | $2,627 | +$219.9 |
| 05-04 | 85 | 82 | 3 | $2,401 | −$203.1 |
| 05-11 | 99 | 91 | 8 | $3,247 | −$337.1 |
| 05-18 | 278 | 268 | 10 | $19,399 | +$248.8 |
| 05-25 | 139 | 131 | 8 | $4,154 | +$190.4 |
| **06-01** | 164 | 130 | **34** | **$203** | −$303.0 |
| 06-08 | 323 | 319 | 4 | $16,360 | +$370.2 |
| 06-15 | 92 | 81 | 11 | $1,477 | +$46.4 |
| 06-22 | 200 | 186 | 14 | $14,406 | −$119.1 |
| 06-29 | 45 | 45 | **0** | $1,753 | +$239.6 |
| 07-06 | 191 | 176 | 15 | $3,094 | **−$876.6** |
| 07-13 | 223 | 207 | 16 | **$27,977** | +$623.1 |
| 07-20 | 26 | 26 | **0** | $1,601 | +$138.8 |

**LOSING WEEKS: 0 of 26. Longest winning run: 26 — the entire frame.**
**p10 $1,677 · p25 $3,132 · median $7,777 · p75 $14,080 · p90 $18,203 · best $27,977 · worst $203.**

## 4. PER DAY

**124 days · p5 $47.6 · p25 $396.2 · median $977.9 · p75 $2,186.6 · p95 $6,905.7 · best $20,018.1 (2026.07.17)**

**LOSING DAYS: 6 of 124 (4.8%)**

| date | net | trades | losses | L/S | median depth |
|---|---|---|---|---|---|
| 2026.05.05 | −$203.1 | 3 | 3 | 0/3 | 3 |
| 2026.05.13 | −$337.1 | 15 | 8 | 8/7 | 4 |
| 2026.06.02 | −$303.0 | 12 | 6 | 12/0 | 6 |
| 2026.06.05 | −$153.4 | 65 | 17 | 48/17 | 5 |
| 2026.06.23 | −$119.1 | 27 | 4 | 12/15 | 4 |
| **2026.07.10** | **−$876.6** | 12 | 7 | 4/8 | 4 |

**INTRADAY FLOOR (corrected marking — BE-locked positions at their lock level, not adverse extreme):**

**worst −$2,394.3 · p50 −$249.5 · p90 −$857.5 · p99 −$1,659.7**
**days below −$1,000: 9 · below −$2,500: 0 · below −$5,000: 0**

## 5. DEPTH LADDER

**LONG (floor 4):**

| depth | n | WR | PF | L | net | $/tr | worst bar |
|---|---|---|---|---|---|---|---|
| **4** | 800 | **89.5** | **3.15** | **84** | $19,823 | $24.78 | −$764.8 |
| 5 | 595 | 92.4 | 5.74 | 45 | $19,212 | $32.29 | −$956.0 |
| 6 | 366 | 93.4 | 6.34 | 24 | $14,629 | $39.97 | −$918.0 |
| **7** | 280 | **100.0** | — | **0** | $23,959 | $85.57 | +$80.5 |
| 8–9 | 409 | 91.9 | 8.55 | 33 | $21,372 | $52.25 | −$800.8 |
| **10–11** | 247 | **100.0** | — | **0** | $21,620 | $87.53 | +$126.0 |
| **12+** | 564 | **100.0** | — | **0** | **$54,599** | **$96.81** | +$142.8 |

**SHORT (floor 3):**

| depth | n | WR | PF | L | net | $/tr | worst bar |
|---|---|---|---|---|---|---|---|
| **3** | 954 | 95.3 | 8.19 | **45** | $25,783 | $27.03 | −$459.0 |
| 4 | 512 | 99.2 | **89.89** | 4 | $19,270 | $37.64 | −$216.8 |
| 5 | 255 | 98.0 | 25.37 | 5 | $8,895 | $34.88 | −$365.0 |
| 6 | 156 | 96.2 | 7.43 | 6 | $4,973 | $31.88 | −$773.4 |
| 7 | 91 | 100.0 | — | 0 | $3,012 | $33.10 | +$88.2 |
| 8–9 | 34 | 100.0 | — | 0 | $10,916 | $321.07 | +$123.2 |
| 10–11 | 10 | *n<20* | — | 0 | $219 | $21.90 | +$219.0 |

**BAR LEVEL: 1,081 bars · 56 losing bars · 5.18% · worst −$956.0**

**LONG d4 and SHORT d3 — the two floor tiers — carry 129 of the 246 losses (52%).** Above depth 7 there are zero losses in either direction.

## 6. DIRECTION AND SESSION

| | trades | W | L | WR | PF | net | $/tr | avg win | avg loss | worst bar |
|---|---|---|---|---|---|---|---|---|---|---|
| **LONG** | 3,261 | 3,075 | **186** | 94.30% | 10.31 | $175,213 | $53.73 | $63.10 | $101.22 | −$956.0 |
| **SHORT** | 2,012 | 1,952 | **60** | **97.02%** | **15.78** | $73,070 | $36.32 | $39.97 | $82.37 | −$773.4 |

**Trade split 1.62:1 against terrain's 0.99:1.** LONG carries 76% of the losses on 62% of the trades.

**By hour:** hour 5 is anomalous — 54 trades, 11 losses (20.75%), **net −$311, the only negative hour.** Hours 3 and 4 are loss-free; hour 6 gives $17,071 on 140 trades.

## 7. EXPOSURE

| | |
|---|---|
| **peak OPEN** | **39 positions / 73.5 lots** |
| **peak AT-RISK** | **12 positions / 24.0 lots** |
| at-risk lots per bar | p50 5.0 · p90 10.0 · max 24.0 |
| bars at the cap (≥12 lots) | 94 of 1,081 (8.7%) |
| **THEORETICAL MAX LOSS AT ANY MOMENT** | **12 × $150 × 2 = $3,600** |
| as share of the $5,000 daily limit | **72%** |
| as share of the $10,000 total limit | **36%** |

**Two-thirds of the peak open exposure (73.5 − 24.0 = 49.5 lots) is BE-locked profit riding and cannot lose.** The jar counts only non-BE positions (engine L272).

## 8. EXITS AND MANAGEMENT

| exit | n | share | mean | total | median bars held |
|---|---|---|---|---|---|
| **BE** | 4,079 | 77.4% | $29.39 | $119,899 | **3** |
| **LF** | 948 | **18.0%** | **$160.50** | **$152,152** | **13** |
| **SL** | 246 | 4.7% | −$96.62 | −$23,769 | 11 |

**18.0% of trades produce 55.9% of gross profit.**

| path | n | share | mean | total | **loss rate** | median bars |
|---|---|---|---|---|---|---|
| **BASE** | 2,227 | 42.2% | $34.88 | $77,688 | **7.63%** | 4 |
| **MOM** | 3,046 | 57.8% | **$56.01** | **$170,594** | **2.50%** | 4 |

**Base path is 3.1× the loss rate for 61% of the $/trade** — the `RISK_MULT` 2.0 vs 4.0 effect, established as mostly stop-width rather than edge.

## 9. THE 56 LOSS EVENTS

Full listing in the run output. Summary:

- **All 56 are `SL`.** Zero BE, zero LF.
- **38 LONG / 18 SHORT.** **38 BASE / 18 MOM.**
- Largest single-bar loss: **−$956.0** (2026.05.27, LONG d5, ATR 45.7, MOM, 6.2 lots, 5 trades)
- Second: −$918.0 (2026.06.01, LONG d6, ATR 44.4, MOM)
- Third: −$800.8 (2026.02.04, LONG d8, ATR 38.5, BASE, 10 lots, 8 trades)
- **One event at ATR 184.9** (2026.07.10, SHORT d3) — an extreme-volatility bar
- June contains **11 of the 56 events**

## 10. OOS AND PERSISTENCE

| split | trades | L | loss rate | 95% CI | losing bars | PF | net | **train-eligible** |
|---|---|---|---|---|---|---|---|---|
| 0 | 511 | 11 | **2.15%** | 1.21–3.81 | 3 | 40.89 | $29,150 | **125/299 (42%)** |
| **1** | 718 | **57** | **7.94%** | 6.18–10.15 | 11 | **4.60** | $22,194 | 233/299 (78%) |
| 2 | 641 | 41 | 6.40% | 4.75–8.56 | 10 | 14.15 | $48,376 | 256/299 (86%) |

**All three windows profitable. Split 1 (May–June) is the weak one, as in every configuration.**

**May–June: 1,425 trades, 92 losses (6.46%), $63,320. Rest of frame: 3,848 trades, 154 losses (4.00%), $184,962.** The soft window is 1.6× the loss rate.

## 11. LIVE GATES

Under LONG ≥ 4 / SHORT ≥ 3, with `min(dep,5)` tier keying:

| tier | LONG | SHORT |
|---|---|---|
| 1 solo | **DEAD** | **DEAD** |
| 2 dual | **DEAD** | **DEAD** |
| 3 triple | **DEAD** | **LIVE** (`FREE` — a pass-through) |
| 4 quad | **LIVE** — `Micro_FailedBreak > p20` | LIVE (`FREE`) |
| 5 5+ | **LIVE** — `Micro_VolOfVol > p20` | LIVE (`FREE`) |

**Two real gates fire: `LONG quad Micro_FailedBreak > p20` and `LONG 5+ Micro_VolOfVol > p20`, plus the `ATR_1M ≥ 20` global.** All SHORT tier gates at 3+ are `FREE` by Option B's design, so nothing is applied to shorts beyond the global.

**Four gates are unreachable: LONG solo/dual/triple and SHORT solo/dual. A build should not carry them.**

**This is 2 live gates against cell D's 1** — the gates are more reachable here, which is the configuration's stated purpose.

---

## 12. WHAT THIS SPECIFICATION IS NOT

- **Not out-of-sample.** All headline figures in-sample; the three "test windows" come from a full-sample-selected union.
- **Not walked forward as a process.** Only 42% of the 299 would have been in the field on split-0 training data.
- **The depth floor is a post-hoc filter, not an admission rule.** A true build frees jar slots and would trade more.
- **The intraday figures are my reconstruction.** The engine contains zero equity/floating references; marking assumes adverse extremes simultaneous for non-locked positions — conservative.
- **Cells below 20 observations carry no rate**, and are marked `n<20`.
