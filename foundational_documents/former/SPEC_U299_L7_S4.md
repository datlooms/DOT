# SPECIFICATION — U299 L≥7 / S≥4

**Research document. Nothing implemented. All figures in-sample on DOT_stitched172 TRUEEST, 177,251 × 172, 2026.01.19–2026.07.21, 1 lot, gap fillers excluded.**

---

## 0. THE FINDING THAT MUST BE READ FIRST

**THE 48 LOSSES ARE 7 EVENTS.**

| date | hr | dir | depth | ATR | path | exit | lots | pnl each | **count** |
|---|---|---|---|---|---|---|---|---|---|
| 2026.02.04 | 13 | LONG | 8 | 38.5 | BASE | SL | 1.25 | −$100.1 | **8** |
| 2026.02.26 | 10 | SHORT | 5 | 35.0 | BASE | SL | 1.00 | −$73.0 | **5** |
| 2026.04.08 | 8 | SHORT | 6 | 62.9 | BASE | SL | 1.00 | −$128.9 | **6** |
| 2026.06.05 | 15 | LONG | 9 | 32.8 | BASE | SL | 1.00 | −$68.7 | **9** |
| 2026.06.17 | 14 | LONG | 8 | 46.8 | BASE | SL | 1.00 | −$96.7 | **8** |
| 2026.06.25 | 14 | SHORT | 4 | 25.6 | BASE | SL | 1.00 | −$54.2 | **4** |
| 2026.07.14 | 12 | LONG | 8 | 30.4 | BASE | SL | 1.00 | −$79.7 | **8** |

**Every loss inside a group is identical — same bar, same ATR, same price, same P&L.** They are the concurrent signals on one losing bar, and the jar admitted 4–9 lots of the same trade.

**Consequences, and they are severe:**
- **The effective loss sample is 7, not 48.** Every loss-rate figure, CI, and PF in this document rests on **seven events in six months**.
- **All 7 are `SL` exits and all 7 are `BASE` path.** No BE or LF exit ever lost. The book has exactly one failure mode.
- **Losses occur on 7 of 103 active days.** The other 96 days had none.
- **The loss rate of 1.88% is not 48/2,558 in any meaningful sense — it is 7 bad bars out of ~2,000 entry bars.**

**Any confidence interval in this document is wrong by construction.** The Wilson CI on 48/2,558 gives 1.42–2.48%; on 7 clustered events the true interval is far wider and I cannot compute it properly without a block bootstrap over days.

---

## 1. WHAT THE BOOK IS

### Provenance — the union is the finding

| source | signals | unique to it |
|---|---|---|
| BOOK-50 | 50 (37L/13S) | 20 |
| 60-priced (E<1) | 60 (54L/6S) | 43 |
| OPTION-B | 120 (70L/50S) | 80 |
| S0-120 (decorrelation) | 120 (70L/50S) | **111** |
| **UNION** | **299 (193L/106S)** | ratio **1.82:1** |

86 duplicates collapsed from 385. **S0-120 overlaps every prior selection by 1–8%**, which is why fusion had room to work.

### Composition

| | LONG | SHORT | ratio |
|---|---|---|---|
| signals | 193 | 106 | 1.82:1 |
| **executed trades** | 1,500 | 1,058 | **1.42:1** |
| reachable terrain | 1,143 | 1,155 | 0.99:1 |

**The trade split self-corrects from 3.08:1 (at L≥4) to 1.42:1 under the floors, with no ratio imposed.**

### How much of the book is live

- **283 of 299 signals (95%) contribute ≥1 trade** under the floors. LONG 177/193 (92%), SHORT 106/106 (100%).
- **16 signals are dead weight** — they never reach depth 7 LONG.
- Trades per contributing signal: median **6**, min 1, max 38.

---

## 2. EVERY RULE, IN ORDER OF EVALUATION

Read from `portfolio_simulation_engine.py` at the ratified sha.

**1. Bar eligibility** (engine, L271 region): `ADX_Value >= 15` and `Volume > 50`, post-warmup (bar ≥ 6,900). Non-zero volume. Friday cutoff `EST_DayOfWeek==5 & (hour>16 | (hour==16 & min>=45))`.

**2. Signal qualification:** F0 triple-convergence — three `V:hi` / `V:lo` / `V:==N` conditions ANDed, thresholds from `dots_thresholds` mechanism D (rolling-2500, day-refreshed). Plus the **mandatory D2D directional term**: `D2D_Trend_Dir == direction`.

**3. Concurrence depth** computed per (bar, direction) as **the count of distinct BOOK signals qualifying on that bar in that direction.** *Not* the 249-condition count — that quantity has median 27 and never falls below 8, and conflating the two caused a full rebuild earlier in this project.

**4. Global gate:** `ATR_1M >= 20`.

**5. Per-tier gates** (Option B's adopted stack at **p90**, all percentiles via mechanism D):

| tier | LONG | SHORT |
|---|---|---|
| solo | `Micro_Hurst > p90` | `Bar_Range < p95` **AND** `Micro_FailedBreak < p10` |
| dual | `AT_Slope_ST > p90` | `Efficiency_Ratio < p80` **AND** `Micro_VPIN > p70` |
| triple | `Bar_Range > p95` | FREE |
| quad | `Micro_FailedBreak > p20` | FREE |
| 5+ | `Micro_VolOfVol > p20` | FREE |

**6. PER-DIRECTION DEPTH FLOOR — LONG ≥ 7, SHORT ≥ 4.** The new rule. Justified structurally: LONG d4 loses at 10.50% and SHORT d4 at 0.78% on the unfloored union — a 13× gap in the same tier, measured before any of these books existed.

**7. Jar: `MAX_POSITIONS = 12`** non-BE lots. (Currently 6 — engine change required.)

**8. Conviction sizing:** `Micro_Hurst > p90` longs ×2, recent-FailedBreak ×1.25, D2D-agree ×2.

**9. Trade management — DO NOT CHANGE.** `RISK_MULT 2.0`, `MOMENTUM_SL_MULT 4.0`, `MAX_RISK 150.0`, `LOCK_FRAC 1.0`, `STEP_PCT 0.30`, `BE_TRIG_FRAC 1.0`, `LAG_BASE 2`, `LAG_MOMENTUM 3`. Momentum path: `(Micro_LogReturn × direction) >= 0.00012` at the entry bar. **All five constants swept in both directions this session; every alternative was negative.**

### DEAD CODE UNDER THIS CONFIGURATION

**LONG minimum depth traded is 7. SHORT minimum is 4.** Therefore:

| gate | status |
|---|---|
| LONG solo `Micro_Hurst > p90` | **DEAD** — tier 1 never trades |
| LONG dual `AT_Slope_ST > p90` | **DEAD** |
| LONG triple `Bar_Range > p95` | **DEAD** |
| LONG quad `Micro_FailedBreak > p20` | **DEAD** |
| LONG 5+ `Micro_VolOfVol > p20` | **LIVE** (applies at ≥5, so at 7+) |
| SHORT solo stacked pair | **DEAD** — tier 1 never trades |
| SHORT dual stacked pair | **DEAD** |
| SHORT triple+ | FREE — nothing to apply |
| `ATR_1M >= 20` global | **LIVE** |

**Six of the eight per-tier gates are unreachable. A build should not carry them.** Note the conviction `Micro_Hurst > p90` multiplier is a *separate* mechanism and remains live.

---

## 3. PERFORMANCE

### Headline

**2,558 trades · 48 losses (7 events) · 1.88% · PF 41.34 · net $168,836 · $66.00/trade · worst day +$40.8 · worst week +$723.2 · 0 losing weeks of 25 · 0 losing months · 0 losing days of 103 · wl3+ 0.79**

### Weekly — 25 weeks, none losing

| week | trades | L | net |
|---|---|---|---|
| 01-26 | 98 | 0 | $4,017 |
| 02-02 | 217 | 8 | $6,714 |
| 02-09 | 219 | 0 | $7,133 |
| 02-16 | 35 | 0 | $1,726 |
| 02-23 | 92 | 5 | $15,135 |
| 03-02 | 249 | 0 | $11,576 |
| 03-09 | 32 | 0 | $1,234 |
| 03-16 | 87 | 0 | $3,102 |
| 03-23 | 138 | 0 | $18,992 |
| 03-30 | 158 | 0 | $7,806 |
| 04-06 | 136 | 6 | $10,228 |
| 04-13 | 54 | 0 | $1,940 |
| 04-20 | 113 | 0 | $5,141 |
| 04-27 | 36 | 0 | $949 |
| 05-04 | 29 | 0 | $1,191 |
| 05-11 | 52 | 0 | $2,187 |
| 05-18 | 138 | 0 | $15,768 |
| 05-25 | 78 | 0 | $3,484 |
| **06-01** | 59 | **9** | **$723** |
| 06-08 | 146 | 0 | $8,797 |
| **06-15** | 64 | **8** | **$1,327** |
| 06-22 | 93 | 4 | $10,167 |
| 06-29 | 18 | 0 | $882 |
| 07-06 | 90 | 0 | $1,983 |
| 07-13 | 127 | 8 | $26,633 |

**p10 $1,046 · p25 $1,726 · median $4,017 · p75 $10,167 · p90 $15,515. Longest winning run: 25 of 25.**

### Monthly

| month | trades | L | net | $/trade |
|---|---|---|---|---|
| 2026.01 | 98 | 0 | $4,017 | $40.99 |
| 2026.02 | 563 | 13 | $30,708 | $54.54 |
| 2026.03 | 583 | **0** | $37,925 | $65.05 |
| 2026.04 | 416 | 6 | $22,877 | $54.99 |
| 2026.05 | 301 | **0** | $22,796 | $75.73 |
| 2026.06 | 374 | 21 | $21,644 | $57.87 |
| **2026.07** | 223 | 8 | **$28,868** | **$129.45** |

**July is the strongest $/trade month at $129.45** — the window BOOK-50 collapsed in.

### Daily

**103 active days · 0 losing · 0 flat · 103 winning.** p5 $89.8 · p50 $653.5 · p95 $7,645.9.

Three lowest days: **2026.04.28 $40.8** (4 trades, 0 losses, all SHORT d4), 2026.05.08 $62.0, 2026.02.02 $64.0.

### Depth ladder

**LONG (floor 7):**

| depth | n | WR | PF | L | net | $/tr | worst day |
|---|---|---|---|---|---|---|---|
| 7 | 280 | 100.0 | — | **0** | $23,959 | $85.57 | +$80.5 |
| **8** | 256 | 90.6 | 7.64 | **24** | $14,691 | $57.39 | **−$773.6** |
| **9** | 153 | 94.1 | 11.80 | **9** | $6,681 | $43.66 | −$618.3 |
| 10 | 170 | 100.0 | — | **0** | $10,567 | $62.16 | +$126.0 |
| 11–12 | 641 | 100.0 | — | **0** | $65,652 | **$102.42** | +$142.8 |

**SHORT (floor 4):**

| depth | n | WR | PF | L | net | $/tr |
|---|---|---|---|---|---|---|
| 4 | 512 | 99.2 | 89.89 | 4 | $19,270 | $37.64 |
| 5 | 255 | 98.0 | 25.37 | 5 | $8,895 | $34.88 |
| 6 | 156 | 96.2 | 7.43 | 6 | $4,973 | $31.88 |
| 7 | 91 | 100.0 | — | 0 | $3,012 | $33.10 |
| 9 | 18 | 100.0 | — | 0 | **$10,502** | **$583.45** |

**The ladder is non-monotone: LONG d7 and d10–12 have zero losses while d8–9 hold all 33 LONG losses.** That is 2 events (Feb 04 d8, Jun 05 d9, Jun 17 d8, Jul 14 d8 — 4 events). **SHORT d9's $583/trade is 18 trades on one day and must not be read as a tier property.**

### Direction and session

| | n | WR | PF | L | net | $/tr |
|---|---|---|---|---|---|---|
| LONG | 1,500 | 97.80 | 43.95 | 33 | $121,549 | $81.03 |
| SHORT | 1,058 | 98.58 | 35.89 | 15 | $47,286 | $44.69 |

**By hour (EST):** 09:00 is the engine — 544 trades, **0 losses**, $64,823, $119.16/trade. 06:00 gives $236.53/trade on 67 trades. **All 48 losses fall in hours 08, 10, 12, 13, 14, 15.** Hours 04, 06, 07, 09, 11, 16, 18, 20 are loss-free.

### Exit types

| exit | n | share | mean | total |
|---|---|---|---|---|
| **LF** | 483 | 18.9% | **$218.71** | $105,636 |
| BE | 2,027 | 79.2% | $33.24 | $67,386 |
| **SL** | **48** | 1.9% | −$87.20 | −$4,186 |

**18.9% of trades produce 62.6% of gross profit.**

---

## 4. THE THREE WEAKNESSES

### 4a. May–June — still the soft window

| | trades | L | loss% | net |
|---|---|---|---|---|
| May–June | 675 | **21** | **3.11%** | $44,440 |
| rest of frame | 1,883 | 27 | **1.43%** | $124,396 |

**21 of 48 losses (44%) in 2 of 7 months**, concentrated in **two weeks**: 06-01 (9 losses, net $723) and 06-15 (8 losses, net $1,327). **May itself has zero losses.** So it is June, not May–June, in this configuration — a narrowing against every prior book.

### 4b. OOS windows

| split | test trades | L | loss% | CI | PF | net | worst day | **train-eligible** |
|---|---|---|---|---|---|---|---|---|
| 0 | 232 | **0** | 0.00% | 0.00–1.63 | — | $20,191 | +$40.8 | **125/299 (42%)** |
| **1** | 347 | **17** | **4.90%** | 3.08–7.71 | 11.30 | $14,332 | +$67.2 | 233/299 (78%) |
| 2 | 304 | 12 | 3.95% | 2.27–6.77 | 46.74 | $39,076 | +$210.0 | 256/299 (86%) |

**All three test windows profitable with positive worst days.** Split 1 is the weak one at 4.90%, as in every configuration.

**Field look-ahead: only 42% of the 299 would have been in the field on split-0 train data**, rising to 86% by split 2.

### 4c. The positive worst day — **it came within ONE loss**

Minimum daily net **+$40.8** on 2026.04.28 (4 trades, all SHORT d4, zero losses). Mean loss size **$87.20**.

**One additional average-sized loss on that day would have made it negative.** The p5 day nets $89.8 — also inside one loss.

**Largest same-day loss cluster: 9 losses on 2026.06.05, gross $618.3, and that day still netted +$233.8.** Losses fell on 7 of 103 days.

**So "positive worst day" is true and is an extremum on 7 events with a one-loss margin.** It should not be treated as a property of the book.

---

## 5. WHAT THE BUILD NEEDS

**Two engine changes, neither authorised:**

**(a) `MAX_POSITIONS` 6 → 12.** Measured worth on comparable books: **+$52,722 at d≥4** and it repairs `wl3+` from 0.54 to 0.74. The worst day does not move with the cap in five of six configurations tested. **cap_blocks at 12 on this book: 3,186** — the jar still refuses that many entries.

**(b) The per-direction depth floor enforced at ADMISSION, not post-hoc.** Currently a filter: **6,954 trades pre-floor → 2,558 post-floor, 4,396 removed.** Removing LONG d1–6 and SHORT d1–3 frees jar slots that would admit deeper trades.

**Bound: 2,558 ≤ true admission-time book ≤ 6,954 trades.** I cannot narrow it further — the engine's tier is only known after admission, and my attempt to force it by rewriting `feat_3` destroyed the signals' third condition. **The real book is larger than 2,558 and the direction of every figure is favourable, but the magnitude is unmeasured.**

---

## 6. WHAT THIS BOOK IS NOT

- **Not walked forward as a process.** Only 1 of 4 union sources and none of the field is re-derivable per segment.
- **Not priced against a book-level null under these floors.** The 160 null draws were at L≥4/S≥4.
- **Not out-of-sample anywhere.** All figures in-sample; the three "test windows" come from a full-sample-selected book.
- **Not resting on 48 independent losses.** It rests on **7**.

**The three structural findings carry the result and none of them is fitted: the union of four independent objectives; 39 of 42 losses at depth ≤3 (measured before these books existed); and the 13× LONG/SHORT gap in the d4 tier. Everything else in this document is a consequence of those three plus two engine constants.**
