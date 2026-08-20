# SELECT — THE DETERMINISTIC SELECTION PROCEDURE
# Two objectives, a set union, frozen gates. No seed, no RNG, no path dependence.

## THE GAPS, ON THE FACE OF IT

|                | NEW (116 signals) | INCUMBENT (297) |
|----------------|-------------------|-----------------|
| MARGIN         | **34.38**         | 33.07           |
| loss events    | **21**            | 42              |
| worst bar      | **-$918**         | -$1,224         |
| **worst day**  | **-$452**         | **-$346.60**    |
| **days traded**| **92 of 132**     | **119 of 132**  |
| losing weeks   | 0 of 26           | 0 of 26         |

**IT LOSES ON WORST DAY BY $105 AND ON 27 TRADING DAYS. Those are the operator's to weigh.**

---

## THE PROCEDURE

```
1  SCREEN      raw F0 scan, TRAIN-WINDOW ONLY
               N holdout months from config; print N and the resolved window
               never the 6,488/6,034 pre-filter (full-sample stats over the
               months the screen is meant to validate against)

2  OBJECTIVE B chance-pricing, PER DIRECTION
               E_dir = n_dir x pf_null_exceedance_pct      threshold from config
               this frame: E_dir < 2   ->  82 signals (55L/27S)
               floors n_dir/K: LONG 0.3106, SHORT 0.0802 - print both
               NOT n_trials_family: 1,840 with long_share 0.7951 prices every
               SHORT row against an 80%-LONG null

3  OBJECTIVE D terrain coverage, greedy set cover over reachable episodes
               PER DIRECTION, target from config
               this frame: 20L/15S     ->  35 signals
               submodular; greedy carries the (1 - 1/e) bound
               supplies direction balance with NO QUOTA

4  UNION       B | D                    this frame: 116 signals (74L/42S)

5  EXECUTE     floor from config, cap 21, gates FROZEN
               ATR_1M >= 20 | D2D direction mandatory
               Micro_Hurst > p90 at LONG d3 and SHORT d3
               Micro_FailedBreak > p20 at LONG d4 and LONG d5+
               AT_Slope_ST > p90 at LONG d4 | SHORT d4/d5+ FREE
               recentfb_sizing = False
               SELECTION CAP = EXECUTION CAP = 21

6  SCORE       report the floor frontier, not a point
```

**EVERY THRESHOLD FROM CONFIG. NO LITERALS ANYWHERE.**

---

## THE FLOOR FRONTIER — THE OPERATOR PICKS THE POINT

| floor  | bars | events | losing wks | days | PF    | MARGIN    |
|--------|------|--------|-----------|------|-------|-----------|
| L3/S3  | 394  | 21     | 0         | 92   | 13.25 | 34.38     |
| L5/S3  | 316  | 17     | 0         | 85   | 15.46 | 37.84     |
| L7/S4  | 188  | **7**  | 0         | 68   | 34.60 | **47.12** |

*Incumbent for reference: L3/S3 margin 33.07 at 119 days; L7/S4 margin 38.25 at 110 days, 16 events.*

---

## THE BATTERY — ALL FIVE ITEMS, COMPLETE

**Base for every row: full valid field 1,818 · floor 3/3 · execution cap 21 · frozen gates · 1.0 lot.**

**RANDOMISATION — 500 size-matched draws, resolution floor 1/(n+1) = 0.0020.**

| metric        | book   | random med | beaten  | p          |
|---------------|--------|-----------|---------|------------|
| MARGIN        | 34.38  | 26.17     | 499/500 | **0.0020** |
| loss events   | 21     | 40        | 498/500 | 0.0040     |
| worst bar     | -$918  | -$1,530   | 483/500 | 0.0339     |
| losing weeks  | 0      | 2         | 498/500 | 0.0040     |

**p IS BOUNDED BELOW BY 1/(n+1). At 500 draws the floor is 0.0020 and MARGIN RETURNED IT — the
best value the test can produce. Bonferroni at 24 grid cells needs p < 0.0021, so margin clears.
The other three are at 2x the floor and CANNOT GO LOWER AT THIS DRAW COUNT — that is a
resolution limit, not a failed test. 2,000 draws would drop the floor to 0.0005.**
**Random margin range 20.90 - 35.22. Only 2 of 500 draws carry zero losing weeks; the book does.**

**SPLIT-HALF** — select on half A, score on half B: **margin 34.46, 9 events, 49 days.**
`b_S` died at this exact test at Spearman -0.400. **Terrain membership shares only 15 of 35
between half A and full train AND THE SCORE HOLDS — quality stable, membership unstable.**

**WALK-FORWARD** — three windows, all profitable, **zero losing weeks in each**:
W1 margin 28.37 (11 events) · W2 margin 44.62 (3 events) · W3 margin 33.22 (6 events).

**JULY HOLDOUT** — **1 loss event on 11 trading days; the worst day in July is +$80.50.**
That is a COUNT of one. Every ratio on it is declined, including the PF of 88.44.

**ANTI-SYSTEM** — 1,702 excluded signals on the 394 bars the book never touches:

| arm                | n     | events | PF   | MARGIN | $/event    |
|--------------------|-------|--------|------|--------|------------|
| ANTI full          | 1,702 | 799    | 1.67 | 7.59   | $621       |
| ANTI pruned PF>=4  | 563   | 234    | 1.99 | 10.39  | $1,040     |
| ANTI pruned PF>=8  | 49    | 6      | 6.08 | 30.79  | $2,715     |
| **BOOK**           | 116   | 21     |**13.25**|**34.38**| **$4,793** |

**The anti-system's best pruned arm reaches PF 6.08 against the book's 13.25, earning $2,715 per
loss event against $4,793, and carries FOUR losing weeks against zero.** Comparable in shape to
the incumbent's anti-system (PF 3.51 against 14.53). **THE EDGE IS IN THE SELECTION, NOT IN A
RICH FIELD.**

---

## THE ALONE-FIRST GATE — BINDING ON ANY FUTURE OBJECTIVE

**No objective enters the union until it beats size-matched random draws ALONE, with gates
derived per draw.**

It has caught two: **co-fire affinity** (60 events on 120 signals, 5 losing weeks, -$3,213
worst bar - dropped) and **terrain-as-a-constraint** (Spearman with loss events -0.184 at
p = 0.663; coverage correlated with a DEEPER worst day at +0.764, p = 0.027).

**Terrain-as-an-OBJECTIVE passed it** — 21 events against 31 and 39 on size-matched controls,
margin 30.68 against 28.18 and 22.54. **Different object from terrain-as-a-constraint; both
were tested; only one entered.**

---

## MEASURED PROPERTIES OF THE UNION MECHANISM

**FUSION IS REAL AND SCALES WITH TERRAIN SIZE:** union entry bars against sum-of-parts ran
+116, +156, +177, +216, +228, +268, +222, +292 across the grid. **Cross-source members complete
each other's depth floors.**

**BUT FUSION MAGNITUDE IS NOT THE OBJECTIVE — margin falls monotonically past 35/25 while fusion
keeps rising. Bigger terrain adds bars that cost more than they pay.**

**THE UNION ELIMINATES B's SINGLE LOSING WEEK (2026-W20) BY DILUTION, NOT BY DECORRELATION.**
Terrain 20/15 alone carries zero losing weeks; B<2 alone carries one; the union carries zero.
**This is a weaker claim than parents-lose-different-weeks and it is the correct one.**

---

## OPEN

- **`T35/25 + B<2`** (104 days, margin 33.36) sits **+0.29 margin points** above the incumbent -
  inside the noise of a 24-cell search. **Not adopted.**
- **The volatility-conditional short gate `b_S`** died on split-half at Spearman -0.400 **on a
  297-signal book with 42 loss events, where every conditioning scheme starves for power.**
  The new book has different statistics. **A gate that failed for lack of power on one book is
  not a finding about the gate — re-test it here.** Attack A's sign reversal remains a real
  property of the data at p = 0.0006 and p = 4.7e-18.
