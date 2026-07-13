# equiDOT — Directional Observation Trigger System (BOOK-50)

**An automated convergence-signal engine for US30.cash (Dow Jones) on MetaTrader 4, running on a FTMO $100K funded account.**

equiDOT watches 171 variables of the market every minute — how price is moving, how volatile it is, where volume and order-flow are pointing, whether the trend is igniting, continuing, or exhausting — and enters a trade only when several of them line up at distributional extremes in the same direction, with the D2D SuperTrend confirming the bias. It runs the trading week with no human intervention; the EA's proven dynamic stop management handles every exit.

BOOK-50 is the final, validated signal set: **50 signals — 48 same-bar triple-convergence signals plus 2 sequential structure-fills — covering all 8 market structures.**

---

## What the numbers look like

Measured **in-book** (all 50 signals trading together) over the clean 6-month sample, **Jan 19 – Jun 25 2026**, which includes a real **March US-Iran-war crash of roughly −10%**. Scale is **true FTMO US30.cash $1 per point per 1.0 lot** — one point of index movement is one dollar at one lot.

| Metric | RUN (momentum-runner) | S.7 base (ratified) |
|---|---|---|
| Signals | 50 (37 long / 13 short) | 50 |
| Trades | 2,409 | 2,416 |
| Win rate | 91.9% | 91.9% |
| Profit factor | 5.83 | 5.66 |
| Net P&L (6 mo, 1 lot) | **+$58,685** | +$56,663 |
| Worst single day | −$127.5 | −$127.5 |
| Max drawdown | −$165.6 | −$165.6 |
| Monthly folds positive | 6 / 6 | 6 / 6 |
| ISO-weeks positive | 22 / 22 | 22 / 22 |
| Weekdays positive | 5 / 5 | 5 / 5 |
| March bear-fold PF | 5.47 | — |

**Out-of-sample (the fixed book measured on May–June, never used to select it):**

| Metric | RUN | S.7 base |
|---|---|---|
| Profit factor | **6.57** | 5.96 |
| Win rate | 92.2% | — |
| Net P&L | +$18,926 | — |

The out-of-sample profit factor is **higher** than the in-sample figure. A curve-fit book collapses out-of-sample; this one strengthens. That is the central evidence that the convergence edge is real, not fitted.

**Survival headroom (FTMO gates):** the worst day is **19× inside** the −$2,500 daily-loss ceiling; the max drawdown is **60× inside** the −$10,000 total-drawdown ceiling.

---

## How it works, in plain terms

**The trigger — two grammars.**

- **F0 (48 signals) — same-bar triple convergence.** On a single bar, three chosen variables all reach their extreme at once (e.g. momentum high, order-flow high, and a structural distance low), the D2D trend agrees with the direction, and the trade fires. It is a stateless one-bar check.
- **F1 (2 signals) — sequential structure-fills.** A different grammar: one condition fires, then a set number of bars later a second condition fires, latched on an ST_Flip event. This lagged pattern captures two market modes — a squeeze breakout and a trend exhaustion — that the same-bar trigger structurally cannot see. Each is as persistent and clean as the F0 signals.

**The gate.** A signal trades if at least two qualifying signals fire together, or a lone signal fires on a bar with Volume ≥ 300 ticks. Eligibility requires ADX ≥ 15 and Volume > 50. Every signal is D2D-direction-gated.

**The exit (universal).** All 50 signals share one exit system — S.7 dynamic stop management: an initial stop at min(ATR×2, 150), a break-even nudge that locks in one step of profit, and a LeapFrog trailing stop that ratchets up as the move tiers out. Fridays force-close at 16:45 EST. The optional **momentum-runner** upgrade widens the LeapFrog trail by one lag when the entry bar's directional log-return is strong — a runner-only change that lifts profit factor at zero cost to win rate or worst-day.

**The concurrency control.** Positions are bounded by a **6-lot live-risk jar** — the EA holds at most 6 lots of *live* (pre-break-even) risk at once. Each trade takes one lot of the jar; when a winning trade reaches break-even (its stop at entry, so it can no longer lose), its lot leaves the jar and frees a slot. A new signal opens only when there is room (fewer than 6 live lots). Because break-even'd winners no longer count as risk, the book works more of the session without ever holding more than 6 lots of live risk — it counts risk, not open positions. This is a strict improvement over a plain position cap: it takes 74 more trades over the sample at the identical 6-lot hard bound, with the same −$127.5 worst day and −$165.6 max drawdown.

**The eight structures.** Every DOTS market mode is covered: Trend Continuation (15), Momentum Ignition (14), Breakout Expansion (8), Structural Entry (6), Price Action (4), Squeeze Breakout (1, F1), Trend Exhaustion (1, F1), Volume Confirmed (1).

---

## Why the signals are trustworthy

- **Selected out-of-sample.** The core was chosen on January–April and measured on unseen May–June. It held and strengthened.
- **Loss-decorrelation, not sample-fit.** Signals were selected to have *independent losing days*, not to maximize in-sample profit factor. That is why 50 signals produce a −$127 worst day: their bad days don't stack. A naive union of every persistent signal collapses to profit factor 1.9; the decorrelated book holds above 5.
- **Persistence at three time-scales.** Each signal and the book hold across 6/6 monthly folds, the ISO-weeks, and all five weekdays — a bar a small-sample fluke cannot clear.
- **A real crash in the sample.** The March −10% war crash is inside the data, and the book stayed green through it (bear-fold PF 5.23).
- **The trim fought overfitting.** A leave-one-out pass *removed* several high-profit-factor signals precisely because their losses were book-correlated — the selection optimized against its own seductive numbers.

---

## Status

The BOOK-50 signal set and its trade management are validated in research and frozen. The remaining work is the EA build: implementing the momentum-runner trail, the small sequential-latch subsystem for the 2 F1 signals, swapping in the 50-signal panel, and replacing the position cap with the 6-lot live-risk jar — then the live FTMO demo, which is the true final test. See `DOT_dev_plan.txt`, `DOT_linear_development_schedule.txt`, and `DOT_handover_blueprint.txt`.

*Scale note: every figure in this document is true $1/point/1.0 lot. Per-signal statistics elsewhere in the record set are standalone (that signal alone); the numbers above are in-book (the whole portfolio together). The two are never mixed.*
