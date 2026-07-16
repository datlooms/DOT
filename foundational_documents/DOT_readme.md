# DOT — Distributional Observation Triggers

*An autonomous trading system for the US30 (Dow Jones), told as a story.*

---

## What DOT is, in one breath

DOT is a piece of software that watches the Dow Jones one minute at a time, and — entirely on its own — decides when to buy, how much to buy, and when to get out. No human sits at the screen. It was built over seventeen months to trade a funded prop-firm account, where the first rule isn't "make money," it's "don't blow up." Everything about how DOT behaves flows from that order of priorities: survive first, profit second.

The rest of this document explains, in plain language, *why* every part of it works the way it does.

---

## What the numbers look like

Measured **in-book** over the clean 6-month sample, **Jan 19 – Jun 25 2026** (which includes a real **March US-Iran-war crash of roughly −10%**), at **true FTMO US30.cash $1 per point per 1.0-lot base**.

The full DOT system is the convergence book **plus conviction self-scaling and two gap-filling single entries**. These are the numbers of that full system — the design being built — with the pre-conviction convergence base shown alongside as a component:

| Metric | **DOT full system** | convergence base |
|---|---|---|
| Trades | **2,691** | 2,363 |
| Win rate | **92.2%** | 92.8% |
| Profit factor | **6.15** | 6.12 |
| Net P&L (6 mo, 1-lot base) | **+$89,487** | +$58,249 |
| Worst single day | **−$153.7** | −$127.5 |
| Max drawdown | **−$153.7** | −$165.6 |
| Monthly folds positive | **6 / 6** | 6 / 6 |

**Out-of-sample (May–June, never used to select the system):**

| Metric | **DOT full system** | convergence base |
|---|---|---|
| Profit factor | **6.81** | 6.99 |
| Net P&L | **+$28,447** | +$18,742 |

The out-of-sample profit factor is *higher* than the in-sample figure — the system strengthens on data it never saw rather than collapsing. That is the central evidence the edge is real, not fitted.

---

## Why "Distributional Observation Triggers"?

Start with the name, because the name is the whole idea.

Most trading systems watch **price**. They draw a line, and when price crosses it, they act. The trouble is that a line drawn today means something different next week — a Dow at 38,000 in a calm market and a Dow at 38,000 in a panic are not the same animal, but a fixed line treats them identically.

DOT doesn't watch price levels. It watches **distributions**. For every one of 171 things it measures about the market — momentum, volatility, order-flow, how far price has stretched from its own average, whether the trend is igniting or dying — DOT keeps a rolling memory of where that measurement *usually* sits. Then it asks a different question: not "is price above 38,000?" but "is this measurement, right now, at an **unusual extreme** compared to its own recent history?"

That's the "distributional observation." A **trigger** fires when several of these measurements are all at their extremes *at the same time, in the same direction*. The system is literally named after what it does: it observes where things sit in their distributions, and it triggers when enough of them are unusually stretched together.

This is why DOT self-calibrates. The extremes are always measured against the market's *own recent behaviour*, so a signal means the same thing in a quiet week and a violent one. The line moves with the market; DOT never trades a stale threshold.

---

## Why does it take three signals to open a trade, not one?

Here is the single most important discovery in the whole project, and it was proven the hard way.

You might think: if one measurement being extreme is a good sign, then find the *one best* measurement and trade it alone. Simpler is better. So we tested exactly that — every one of the 171 variables, at every extreme, in both directions. The hunt had a name: the "Heart of the Ocean," a single perfect signal.

**It doesn't exist.** No lone variable, at any extreme, wins reliably enough to trade on its own across the whole sample. Push one measurement to its 99th percentile and you get a rare, occasionally-perfect signal that fires a handful of times and then vanishes. Loosen it so it fires often, and the win rate collapses. For a single variable, being *reliable* and being *frequent* are mutually exclusive.

The reason is intuitive once you see it: any one measurement is mostly noise. Momentum spikes constantly for no reason. Order-flow lurches. Volatility jumps. Any single extreme is usually just the market twitching.

But when **three independent measurements** hit their extremes on the *same bar* — momentum surging, order-flow confirming, and price stretched to a structural edge, all at once — that's no longer a twitch. That's a genuine event that three different lenses agree on. The agreement is the signal. This is **convergence**, and it's why DOT requires three variables to line up (a "triple") before it will act.

There's one narrow exception. A trade can fire on fewer signals **if** it also has heavy volume behind it (300+ ticks) — because a big volume surge is itself a strong confirmation, standing in for one of the missing agreements. But the default, and the heart of the system, is: *three lenses must agree.*

The convergence requirement is DOT's answer to over-fitting. A single variable can be tuned to look brilliant on past data and then fail live. Three independent variables agreeing is far harder to fake — which is exactly why the system holds up on data it has never seen.

---

## Why 50 signals? Why not more, why not fewer?

DOT scanned an enormous space — thousands upon thousands of possible three-variable combinations. Many of them *passed the basic quality bar*: over two thousand combinations cleared the hurdles of "traded enough times, won often enough, held up across every month."

So why keep only 50, when thousands qualified?

Because "passed the bar" isn't the same as "belongs in the book." Two thousand signals sounds robust, but most of them are near-duplicates of each other — they fire on the same market moments, win and lose together, and pile risk onto the same handful of days. Stacking a thousand redundant signals doesn't diversify you; it just concentrates your bet while *looking* diversified. That's the trap that curve-fitting sets.

So DOT was trimmed, deliberately and against its own instincts. The selection ran a **leave-one-out** test: pull each signal out and see whether the book got *worse* — specifically, whether it lost its protection on the hardest, most bearish stretches. Signals that were only padding the profit in easy conditions got cut. Signals that quietly held the line when the market turned ugly were kept. The trim actively fought the temptation to keep high-profit signals whose losses were correlated with everyone else's.

What survived is **50 signals** that each earn their place: 48 same-bar triples, plus 2 of a special sequential kind (below). Together they cover all **eight distinct market structures** — trend continuation, momentum ignition, breakout expansion, structural entries, price-action micro-structure, squeeze breakouts, trend exhaustion, and volume-confirmed moves. Not one mode of the market is left uncovered, and not one redundant signal is carried.

The proof that the trim worked: BOOK-50 performs *better* on data it was never trained on than on the data it was built from. A curve-fit system does the opposite. The number isn't 50 because 50 is special — it's 50 because that's what was left after everything that didn't genuinely add was cut away.

---

## Why do two of the signals work differently? (The sequential patterns)

Forty-eight of the fifty signals are "same-bar" — everything happens in one minute: three variables spike together, the trend agrees, the trade fires. A single snapshot.

But some real market patterns *can't* be seen in a single snapshot, because they are events that unfold **over time**. A squeeze that builds and then breaks. A trend that runs and then exhausts itself. These have a "first this, then that" shape — one thing happens, and a specific number of bars later, a second thing happens. A single-bar check is structurally blind to them, the way a single photo can't show you a door opening.

So two signals use a different grammar: **sequential**. One condition fires and gets remembered ("latched"), and then a set number of bars later, a second condition completes the pattern. These two capture a **squeeze breakout** and a **trend exhaustion** — two market modes the same-bar triggers physically cannot see.

They were added last, and only because they filled two genuine gaps in the eight-structure coverage. They're held to the exact same standard as every other signal: clean, persistent, and validated across every fold of the data. DOT keeps precisely two of them — the two that fill real holes — and no more.

---

## How does the stop-loss work, and why is it built that way?

A stop-loss is the exit that caps a loss. On a funded account, it's the most important line of code in the system, because the account dies not from being wrong, but from being wrong *too big*. So DOT's stop is engineered around a single non-negotiable ceiling: **no trade ever risks more than $150.** That cap is inviolate. Every other piece of stop logic operates *underneath* it.

**Why ATR?** ATR (Average True Range) is a measure of how much the market is currently moving. A stop placed a fixed number of points away is naive: in a calm market it's needlessly wide, and in a wild market it's so tight it gets clipped by ordinary noise. So DOT sizes its stop by ATR — the stop breathes with the market. Base stop: two times ATR, capped at $150. When the market is jumpy, the stop sits a little further out (up to the cap) so a normal wobble doesn't stop you out of a good trade. When it's calm, the stop pulls in. The stop is measured in the market's own current units, not a fixed guess.

**Why does momentum make the stop wider?** Here's a subtle, hard-won insight. When a trade opens on a strong momentum burst — the kind of move that has real force behind it — it deserves more room to breathe, because strong moves often pull back sharply before continuing. So on high-momentum entries, DOT widens the *catastrophe* stop to four times ATR (still capped at $150). This is the single loss-management change that actually improved the system — because giving strong signals room to survive a wobble kept winners that a tight stop would have killed. Every *other* attempt to reduce losses — tighter stops, filters, early exits — made things worse by scratching out winners. The lesson, learned repeatedly: *room helps, taking room hurts.*

But — and this is the clever part — only the **catastrophe stop** widens. The **break-even trigger** stays on the *base* (two-ATR) distance. That means a momentum trade gets extra room to survive, but still locks in break-even at the normal, closer distance. It gets the benefit of room without giving up the protection of an early break-even. Getting this split wrong (widening everything) would have blown the worst day up by more than double; getting it right holds the worst day exactly where it was while cutting the number of losses.

**Break-even and the "LeapFrog" trail.** Once a trade moves in your favour by one step, the stop jumps up — but not to the raw entry price. It jumps to **tier 1**, which locks in *one step of actual profit*. The trade is now not just risk-free — it's already a guaranteed small winner. This matters more than it sounds: roughly **half of all DOT's winning trades are captured at this break-even tier alone** — a modest, locked, high-frequency profit that the tiering banks over and over. It is the quiet workhorse of the whole system, not a scratch. As the trade keeps running, the stop "leapfrogs" upward through further tiers (each step 0.30 of the risk unit, activating the full trailing mode at the third tier), locking in more profit as the move extends, but never so tight that it strangles a runner. And again, momentum matters: on a strong-momentum trade, the trail is given a longer leash (it "leapfrogs" every third bar instead of every second) — because a move with real force behind it should be allowed to run further before the trail tightens.

Put together: the stop breathes with volatility, gives strong moves room, locks in break-even *profit* early, trails the rest as the move extends, and *never*, under any circumstance, risks more than $150. Survival first.

**A note on the other end — very low volatility (tested, and left alone).** ATR-based stops have a theoretical mirror-image weakness to the wild-market case: when the market goes *unusually quiet* (ATR shrinks), a two-times-ATR stop pulls in very tight — tight enough, one might worry, that ordinary tick-noise could clip the trade before it works. So a **minimum stop floor** (a lower bound of $10–$30, mirroring the $150 upper cap) was tested directly on the full system. It was **rejected** — and the reason is instructive. Low ATR doesn't just mean a tight stop; it means *low noise*. In quiet stretches the price barely moves, so a small stop is *proportionate* to the tape — it isn't being clipped, because there's little to clip it. Forcing a wider floor onto those trades saved **zero** winners (not one, at any floor value tested) and only let the losing low-volatility trades bleed further before stopping. The floor degraded out-of-sample performance and helped nothing. The conclusion: the $150 *maximum* cap alone is sufficient; no *minimum* is needed. The stop breathes with volatility all the way down, because the noise breathes down with it.

---

## Why only 1 lot? And what is this "conviction scaling"?

You'd think the way to make more money is to trade bigger. Just double every position — twice the size, twice the profit. And that's true, but it's also true of the *losses*: double every position and you double every bad day too, indiscriminately. On a funded account with a hard daily-loss limit, blindly doubling is how you get killed.

DOT does something smarter. It trades a base of **1 lot** — and then lets the software *scale itself up only on the trades where the edge is strongest.* The base never changes. The multiplier is the scaling, and it's applied surgically.

**Which trades get scaled?**

- **Hurst scaling (2×).** `Micro_Hurst` is a reading of whether the current move is a *genuine, persistent trend* or just noise drifting. When a long opens and Hurst is in its top tail, the trend is real and running — so that trade takes **2 lots instead of 1**. This only happens on **longs** (high persistence carries no edge on the short side, which was tested and confirmed). And it lands on a remarkable subset: these 2× trades win about **96% of the time**, and — critically — they are *isolated* winners that never once sat on the worst day of the sample. So the book earns markedly more, while the worst day barely moves. That's the whole art of it: scale up exactly where the probability is highest *and* the risk is lowest.

- **FailedBreak scaling (1.25×).** A `Micro_FailedBreak` spike is a failed break-*down* — the market tried to break lower, failed, and tends to revert upward. A long that arrives just after such a spike is a higher-conviction long, so it gets a smaller **1.25× nudge.**

**Can they both scale at once?** If a single trade qualifies for *both* conditions, it takes the **higher** multiplier (2×), never the product. DOT will never multiply 2× by 1.25× to get 2.5×. The multipliers don't compound — the strongest applicable one wins, and that's the end of it. This keeps the total exposure bounded and predictable.

The reason DOT deploys at **1 lot base specifically** is that the conviction multiplier *is* the scaling mechanism. If you also doubled the base to 2 lots, a 2× trade would become effectively 4 lots, and a rare stack of them could breach the account's daily-loss ceiling. So the rule is firm: let the software scale itself intelligently from a 1-lot base; don't blanket-multiply on top. The risk management isn't a constraint bolted on afterwards — the *sizing itself* is the risk management.

---

## The gap-fillers: two signals that only fire when nothing else does

There's a final, elegant piece. Those same two readings — Hurst and FailedBreak — turned out to be strong enough to open trades *on their own*, but only in specific, careful circumstances.

The rule: these two single-variable entries fire **only when DOT has no other position open at all.** They fill the *gaps* — the flat moments when none of the 50 convergence signals are firing. A Hurst-persistence long when the book is idle and a trend is genuinely running; a FailedBreak reversion long when the book is idle and a break-down has just failed and is bouncing.

Why gate them so tightly? Because on their own they're good but not book-quality (~90% win rate versus the book's ~93%). Letting them fire *on top* of open positions would pile lower-quality, correlated risk onto busy days — exactly the wrong time. But letting them fire *only in the gaps* means they physically cannot deepen a bad day (there's nothing else open to deepen), and they add profit in moments the book would otherwise sit out. They improve the worst day rather than worsening it, purely because of *when* they're allowed to act.

They fill the quiet. That's the whole trick.

---

## Why the US30? Does DOT only work on the Dow?

DOT was built, tuned, and validated exclusively on the **US30.cash (the Dow Jones cash index), on the 1-minute timeframe.** Every threshold, every signal, every number in this document is specific to that instrument and that timeframe.

But here's the reasoning worth thinking through carefully, because it points somewhere interesting. **DOT does not trade price. It trades geometry.** Every entry is formed from *distributional percentiles* — where a measurement sits relative to its own recent history — not from the price of the asset. The signals are, in effect, measuring the *shape* of the market: how stretched momentum is, how order-flow is leaning, whether the trend is persisting or exhausting, all expressed as position-in-distribution. The actual price level of the instrument is irrelevant to the calculation; a signal on the Dow at 38,000 and the same geometric configuration on a different index at a different price are the *same event* to DOT.

Now consider the other top US indices — the **US100 (Nasdaq)** and **US500 (S&P 500)**. These are extraordinarily *highly correlated* with the Dow — they are three windows onto the same US large-cap market, moving together tick-for-tick most of the day. They share the same session, the same liquidity profile, the same order-flow character, and — crucially — the same *structural geometry*: the same trends, squeezes, breakouts and exhaustions, unfolding at the same times for the same reasons. And on FTMO, all three trade at **0% commission**, the same cost structure DOT was built for.

So the rational expectation is this: because DOT's fifty signals measure *geometric, distributional, correlated percentile structure* — not price, and not anything Dow-specific in kind — there is no obvious reason they wouldn't measure the *same elements* on the US100 and US500. The market geometry those signals detect is present in all three indices, because all three are the same market seen through slightly different weightings. The **exact profit and loss and statistics would differ** — each index has its own volatility and personality — but the *underlying edge*, the thing the geometry is detecting, should be present.

This is a reasoned expectation, not a proven result — it would still need to be validated forward on each instrument before any capital is committed, and the thresholds re-confirmed. But it reframes the honest answer to "does DOT only work on the Dow?" The signals aren't fitted to the *Dow specifically* — they're fitted to a kind of *market geometry* that the Dow, the Nasdaq, and the S&P all share. DOT is a specialist in that geometry, and the Dow is simply where it was proven first. The strong prior is that the same distributional structure lives in its correlated cousins.

A note on the timeframe, because it matters: the 1-minute chart is not a "small" dataset dressed up. Six months of 1-minute Dow bars contains the equivalent of centuries of daily bars by sheer count of distinct market events, and — because market structure is self-similar — a single trading session on the 1-minute chart contains the full grammar of trends, reversals, squeezes and exhaustions that a daily chart spreads across months. DOT didn't learn from a thin slice of history. It learned from the densest, most structurally complete view of the market available.

---

## Why the signals are trustworthy

Anyone can produce a good backtest by accident or by over-fitting. Here is why DOT's is different — the specific structural reasons the edge is believed to be real rather than a lucky curve fit:

- **Selected out-of-sample.** The core was chosen on January–April data and then measured on unseen May–June data. It didn't just survive the unseen stretch — it *strengthened* on it. A curve-fit system does the opposite; it looks brilliant on its training data and falls apart on new data.
- **Loss-decorrelation, not sample-fit.** Signals were selected to have *independent losing days* — not to maximise in-sample profit. That is precisely why 50 signals produce only a small worst day: their bad days don't stack on top of each other. A naive union of every persistent signal you can find collapses to a profit factor of about 1.9 (barely profitable); the deliberately *decorrelated* book holds above 6. The difference is entirely in *how* the signals were chosen.
- **Persistence at three time-scales.** Each signal, and the book as a whole, holds up across all six monthly folds, across the individual weeks, and across all five weekdays. A small-sample fluke cannot clear all three bars at once.
- **A real crash inside the sample.** The March ~10% US-Iran-war crash is *in* the data, not excluded from it, and the book stayed green straight through it. The system has already been tested against a genuine market shock, not just calm conditions.
- **The trim fought overfitting.** The leave-one-out selection *removed* several high-profit signals precisely because their losses were correlated with the rest of the book. The selection process actively optimised *against* its own most seductive numbers — the opposite of what over-fitting does.

---

## The order of everything: survival first

If you take one thing from this document, take this. Every design choice in DOT resolves the same way when there's a conflict: **survival beats profit, always.**

- The stop is capped at $150 before any profit logic runs.
- Signals must converge (three agree) before a trade is allowed — because a wrong-but-confident single signal is how accounts die.
- Sizing scales up only where the edge is strongest *and* the risk is lowest, never blindly.
- The book was *trimmed*, cutting profitable-but-correlated signals, to protect the hardest days.
- The gap-fillers are caged so they can only ever help the worst day, never hurt it.

The daily-loss ceiling of the funded account is the one number the system is truly built around. Everything else — the 50 signals, the ATR stops, the momentum room, the conviction scaling — is downstream of a single question asked before every trade: *does this keep us alive?*

Make money second. Stay in the game first. That's DOT.

---

## The numbers (for the record)

On the validated six-month sample, at 1 lot base, with the full system (BOOK-50 + dynamic stops + momentum room + conviction scaling + gap-fillers):

- **~2,700 trades, ~92% win rate**
- **Profit factor above 6** (six dollars won for every dollar lost)
- **Every one of six monthly folds profitable; every week positive**
- **Stronger on unseen data than on the data it was built from** — the signature of a real edge, not a curve-fit
- **Held through a real ~10% bear crash** (the US-Iran war stretch) still profitable
- **Worst single day well inside the account's daily-loss ceiling** — by a factor of roughly sixteen

These are backtested figures. The final word belongs to live execution, which trims a proven edge but does not invert one. That's what the demo phase is for.

---

*DOT watches the distributions, waits for the lenses to agree, sizes itself to the strength of the moment, and above all, stays alive. Seventeen months of asking "does this keep us in the game?" — and only then, "does this make us money?"*
