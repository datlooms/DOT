# How to Find Real Patterns (Not Curve-Fit Ones) — A Backtesting Guide
## The equiDOT method, in plain terms — for future reference

Curve-fitting = a system that looks amazing on the past but fails live, because it
memorised the noise of one sample instead of finding a real, repeatable edge.
Below are the filters that were used to tell the difference. Each one does double duty:
it screens OUT curve-fitting AND confirms genuine persistence. Use them as a checklist.

---

## 1. ENTRY GATES — only trade when the market is actually moving
Before a signal is even allowed to fire, the conditions must be worth trading:
- **ADX >= 15** — only trade when there's real directional strength, not dead chop.
- **Volume gate** — >= 50 ticks if two+ signals fire together (confirmation), OR
  >= 300 ticks if only one fires (a solo signal needs more proof it's a real move).
- **D2D direction gate** — the signal must agree with the D2D trend (bull or bear).
  No fighting the prevailing direction.

WHY IT FIGHTS CURVE-FITTING: gates stop the system trading noise. A fitted system
grabs profit from tiny meaningless wiggles; gating to real moves removes that
false profit before it can flatter the backtest.

---

## 2. TRIPLE CONVERGENCE — three things must agree at once, not one
A signal only fires when THREE separate market variables hit extremes on the same bar,
not one. Random depth (just "lots of stuff happening") was tested and rejected.

WHY IT FIGHTS CURVE-FITTING: one variable at an extreme happens constantly and by
chance. Three specific variables agreeing at once is rare and meaningful — it describes
a real, specific market condition, not a coincidence you can find anywhere in the data.

---

## 3. ADAPTIVE MEASUREMENT — 171 variables that self-calibrate
The past was measured through 171 variables, and the thresholds ADAPT to recent
conditions (rolling percentiles) rather than being fixed numbers hand-picked to fit.

WHY IT FIGHTS CURVE-FITTING: hand-picked fixed numbers ARE curve-fitting — you're
tuning constants to match history. Adaptive thresholds compute themselves from the
market, so nothing was hand-tuned to make the past look good. What the backtest
computes is exactly what runs live (no look-ahead, no peeking at the future).

---

## 4. DECORRELATION — pick signals whose LOSSES don't overlap
Signals were NOT chosen for highest profit. They were chosen so their LOSING DAYS
don't happen on the same days. (Measured: loss-correlation between signals 0.03-0.13,
near-independent.)

WHY IT FIGHTS CURVE-FITTING: choosing for highest in-sample profit IS fitting — you're
just picking whatever happened to win most in the past. Choosing for independent losses
optimises for a property that CAN'T be faked by memorising the sample. Proof it works:
throwing every signal together (the fitter's move) collapsed to PF 1.89; selecting for
independence recovered it to ~6. The edge lives in the independence, not the count.

---

## 5. PERSISTENCE AT THREE TIME-SCALES — must hold across folds, weeks, AND days
A signal had to be net-positive across:
- **6 monthly folds** (all 6 months green)
- **~22 weekly buckets** (green week after week)
- **all 5 weekdays** (pool every Mon together, every Tue, etc. — each weekday
  bucket is net-positive; catches the "secretly just a Monday thing" trap)
...all at once, plus a minimum of 30 trades (enough sample to trust it).

  NOTE: "all weekdays positive" is DAY-OF-WEEK (the five pooled buckets), NOT every
  individual calendar day. Individual losing days are normal and healthy — the book
  had ~7 losing days out of ~106 (a ~93% daily win rate). A system with ZERO losing
  days would be the suspicious one. What you want is no whole weekday that's
  structurally dead.

WHY IT FIGHTS CURVE-FITTING: a fluke can look good on ONE time-slicing. It cannot look
good on THREE different slicings at the same time. Requiring monthly + weekly + daily
persistence simultaneously is a filter a small-sample accident cannot pass.

NOTE ON INTERMITTENCY: individual signals do NOT fire every week — they burst and go
quiet. THIS IS GOOD. A real signal waits for its specific condition and stays silent
otherwise. A curve-fit signal fires constantly (wrapping around all the noise). Quiet
periods are honesty. The BOOK stays continuously green because the signals'
quiet periods interlock (that's the decorrelation working).

---

## 6. OUT-OF-SAMPLE (OOS) — pick on old data, test on data it never saw
The signals were selected using only the FIRST months, then measured on the LAST months
they had never seen. The edge held or got STRONGER on the unseen data.

WHY IT FIGHTS CURVE-FITTING: this is the single most important test. A curve-fit system
DIES out-of-sample (its profit collapses toward break-even) because noise doesn't repeat.
A real edge survives on data it wasn't selected on. If OOS holds, it's real. If OOS
collapses, it was fitted. Always keep a slice of data the selection never touches, and
judge on THAT, not on the data you chose from.

---

## 7. LEAVE-ONE-OUT TRIM — throw away the seductive signals
The final book was trimmed by removing each signal and checking if the book got BETTER
without it. Several signals with HUGE individual profit factors (9, 10, even 21) were
CUT — because their losses overlapped the book (dead weight dressed up as a star).

WHY IT FIGHTS CURVE-FITTING: a fitter KEEPS the highest-profit-factor names. Cutting them
because they don't add independent value is the opposite of fitting. If your trim keeps
throwing away your best-looking numbers, you're doing it right.

---

## 8. REPORT YOUR NULLS — kill the ideas that don't work
When a research idea failed (e.g. flip-prediction scored below random chance, AUC 0.492),
it was KILLED, not forced into the system.

WHY IT FIGHTS CURVE-FITTING: a process that only ever "finds signal" is manufacturing it.
A trustworthy process finds NULLS and honours them. If you never reject an idea, you're
fitting. Keep a record of what DIDN'T work — it proves the wins are real.

---

## 9. THE NULL / SHUFFLE TEST — prove the edge isn't luck or a fit
Take the FINISHED book's entries and BREAK the signal->outcome link three different ways,
keeping the EXACT same trade management and the same number of trades. Run 200+ iterations
of each to build a distribution of what RANDOM produces, then see where the real book lands:
- **Direction-shuffle** — keep the real entry bars, but randomize each trade's long/short.
  (Breaks direction-selection: does picking the right SIDE matter?)
- **Random-timing (the TM-alone test)** — enter on random eligible bars instead of real
  signal bars, same D2D gate, same count. (Breaks timing: does WHEN you enter matter, or is
  the trade management alone making the money?)
- **Fully random** — random bars AND random direction. (Breaks everything: the pure floor.)

WHY IT FIGHTS CURVE-FITTING: a curve-fit or lucky book sits INSIDE the null cloud — random
arrangements with the same trade management produce similar numbers, which means the "edge"
was really the trade management or chance, not the signals. A REAL edge sits far in the right
tail, BEYOND THE NULL MAXIMUM (not one random shuffle out of hundreds can match it). It also
ATTRIBUTES the edge: if random-timing + your real TM already makes most of the money, the TM
is the edge (bad — your signals aren't doing the work); if random entries lose or break even
and only the real signals win, the SIGNAL is the edge (good — exactly what you want).

WORKED EXAMPLE (BOOK-50): across 200 iterations per null, the real book landed BEYOND THE
MAXIMUM of all three nulls — 0 of 200 random shuffles matched it — sitting ~16 sigma above the
direction-shuffle mean and ~35 sigma above the random-timing and fully-random means (p < 0.005
on all three). Critically, TM-alone (random entries run through the real trade management) LOST
money: net -$678, PF 0.97. That proves the edge is the SIGNAL, not the trade management. Real
timing added the "when" (~$22k), and real direction added ~$36k more on top. Combined with OOS
strengthening (luck cannot improve out-of-sample), the result is decisive: the edge is real.

THE RULE: after building a book, run the null/shuffle test. If the real book is NOT far beyond
the null maximum, OR random entries with the same TM already make most of the money, the edge
is suspect — treat it as a fit until proven otherwise.

---

## THE ONE-LINE TEST
> A signal that fires all the time is fitted to the past.
> A signal that fires only in its moment is describing the market.

## THE HONEST LIMIT (always state this)
A backtest is proven only in the REGIMES the sample contained. Regimes not in your data
(e.g. a grinding multi-month bear, prolonged chop, a rate-shock) are UNPROVEN, not
disproven. The live demo is the final test — it's the normal doorway between a validated
backtest and real capital, not a flaw. More data spanning more regimes is always the
highest-value next step.

---
## CHECKLIST (tape this to the wall)
[ ] Gates: only trade real moves (trend strength + volume + direction agreement)
[ ] Convergence: multiple variables agree at once, not one
[ ] Adaptive thresholds: self-calibrating, no hand-tuned constants, no look-ahead
[ ] Selection: decorrelated losses, NOT highest in-sample profit
[ ] Persistence: holds across folds AND weeks AND days, min trade count met
[ ] OOS: selected on old data, judged on unseen data — held or strengthened
[ ] Trim: removed loss-correlated signals even with high standalone numbers
[ ] Nulls: failed ideas killed and recorded, not forced
[ ] Null/shuffle test: real book beyond the null maximum on all three shuffles; TM-alone on random entries loses money (signal is the edge, not the TM)
[ ] Limit stated: which regimes are proven vs unproven; demo is the final test
