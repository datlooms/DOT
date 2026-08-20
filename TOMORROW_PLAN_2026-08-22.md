# TOMORROW — THE PLAN
## 2026-08-22. Data freeze holds: no seventh month until SELECT is finalised and the engine is explored.

**THREE OBJECTIVES, IN ORDER. Do not start 2 before 1 lands. Do not start 3 before 2 lands.**

```
1  REMOVE THE MIN_TRADES BLINDNESS       cheap, changes the field everything else runs on
2  FINALISE SELECT                       one procedure, one frontier table, one stopping rule
3  EXPLORE THE ADAPTIVE ENGINE           minimum gate set, floor-only, no membership
```

---

# STEP 0 — SET UP THE INSTANCE

**One seat. Call it QUANT. Do not open a second until step 2 — two seats cost most of yesterday in
reconciliation, and the one thing it bought (the batch-120 catch) is already banked.**

Its opening prompt must carry:

```
git clone https://github.com/datlooms/DOT.git

READ IN FULL, IN THIS ORDER:
  DOT_progress_and_rd_plan.md            section 2026-08-21, subsections 12a-13a
  foundational_documents/DOT_anti_curvefit_guide.md
  foundational_documents/QUANT_HYBRID_SPEC.md      the current best book
  foundational_documents/The_Whole_DOT_spec_v3.txt  §4.2 only — the fourteen orphans

DO NOT re-derive anything in 12i. Fifteen doors are closed with the measurement
that killed each. Re-proposing one costs a turn and the record says so.

STANDING RULES:
  every figure at 1.0 LOT. No max-lot column, ever.
  ordering: loss events -> tail -> persistence -> days -> net LAST
  a ratio on fewer than 20 events is a COUNT. State the count, decline the rate.
  a p-value carries its quantity, its population, and sampled-or-enumerated.
  any rule with a threshold is RUN on this frame before it is treated as built.
  the alone-first gate binds on every objective: it beats size-matched random
    draws with gates derived PER DRAW, or it does not enter a union.
  state the cumulative trial count on every report.
```

**Give it the six-system table and the frontier so it starts from the answer, not from the search.**

---

# STEP 1 — THE `MIN_TRADES` BLINDNESS

**WHY FIRST: it changes the field that steps 2 and 3 both run on. Doing it after would invalidate them.**

### 1.1 — Measure the cost of the filter
```
re-run the F0 scan at MIN_TRADES = 1
report:  total rows          (was 19,754)
         rows with 1-9 trades solo on this frame
         how many of the 14 orphans reappear    (expect all 14)
         the LONG/SHORT split of the new rows
```

### 1.2 — Is rarity regime-dependent? THE TEST THAT MATTERS
**Of the signals that fire fewer than 10 times on this frame, how many fired 10 or more on the older
152,983-bar sealed baseline?**

**IF A MEANINGFUL FRACTION DID, `MIN_TRADES` IS DELETING REGIME COVERAGE AND THE NUMBER SAYS SO.**
If almost none did, the filter is removing genuinely dead signals and the limitation is smaller than feared.
**Either answer is worth having and neither has been measured.**

### 1.3 — What the orphans actually do
Already known from spec v3 §4.2 and not to be re-derived: **eight SHORT orphans contribute exactly ZERO
trades; six LONG orphans contribute 157 trades directly, and removing all fourteen costs $3,923 and three
trading days — more than their own trades earn, because they raise depth and let other signals clear the
floor.**

**WHAT IS NOT KNOWN: do the newly-recovered sub-10 signals behave the same way?** Score the expanded field's
low-trade population as a group: **do they contribute directly, or do they contribute by raising depth?**

### 1.4 — The ruling
**Then decide, and record it in the SELECT spec either way:**
- **remove the floor** and accept a larger, noisier field, or
- **keep it** and state in the specification that the procedure is structurally blind to rare signals

**A procedure built on scan statistics inherits this blindness. It must be named, not discovered again in
three months.**

---

# STEP 2 — FINALISE SELECT

### 2.1 — Rebuild both hybrids on the clean matrix
**Objective A's exclusion was withdrawn.** The true-solo matrix is `MAX_POSITIONS` high enough that the jar
never binds, batch size irrelevant, ~78 seconds. **Everything A-related from yesterday was measured on a
contaminated matrix and must be re-run.**

### 2.2 — Close the three outstanding battery items
```
[ ] anti-system on both hybrids       the item that separates edge from a rich field
[ ] gates DERIVED for the hybrid      currently inherited from a 297-signal book with 42 events
[ ] 500-draw control on the Quant hybrid   it ran 100; floor 0.0099; underpowered by its own account
```

### 2.3 — The split-half degradation — RESOLVE IT
**The Quant hybrid's split-half gives margin 30.71, ONE losing week, and a worst day of −$1,224 against
−$391 on the full frame. That is the softest number in any of the four new documents.**

**Run it on the other half and on a third split.** If the losing week and the tripled worst day reproduce,
**the hybrid is fitted to the full frame and `B+D` is the better system.** If they do not, it was one bad
split and the hybrid stands.

### 2.4 — Build the frontier as ONE table
**From one loss event to forty-two. Every row: n, events, event-days, losing-bar rate, worst bar, worst day,
worst intraday, losing days, LOSING WEEKS, days of 132, WR, PF, break-even WR, MARGIN, net. All at 1.0 lot.**

```
A30+B L10/S6      1 event    — one losing week
A30+B L8/S5       6 events   — one losing week
A30+B L6/S4      14 events   86 days   ZERO weeks   MARGIN 45.85
MANAGER 197      10 events   88 days   ZERO weeks
B+D L3/S3        21 events   92 days   ZERO weeks
T35/25 L3/S3     27 events  104 days   ZERO weeks
T60/45 L3/S3     44 events  119 days   ZERO weeks
WHOLE DOT        42 events  119 days   ZERO weeks   net $284,974
```

**AND THE STOPPING RULE IS MECHANICAL AND ALREADY KNOWN: the losing week binds, not the event count.**
L8/S5 gives 6 events and loses a week; L6/S4 gives 14 and holds. **The pick is the widest configuration
holding zero losing weeks. Write that into the spec as the rule.**

### 2.5 — Ship it
**`SELECT_PROCEDURE_FINAL.md`** — the procedure, the frontier, the stopping rule, the constant registry with
the true-solo matrix parameters, the alone-first gate, and a §0 naming what would make it wrong.

**Then the Developer wires it: `master.py --stage SELECT` runs the procedure and emits the frontier table.**

---

# STEP 3 — THE ADAPTIVE CONVERGENCE ENGINE

**The evidence for it is accidental and it is the strongest thing yesterday produced.**

**At L6/S4 and L8/S5 — the two best books measured — `Micro_Hurst > p90` fires ZERO times, three of six gate
cells never fire, and 60% of trades pass through no tier gate at all. THE FLOOR IS ALREADY DOING THE
SELECTING AND THE GATES ARE MOSTLY DECORATION AT DEPTH.**

### 3.1 — The question, stated so it can be answered in one sweep
**What is the minimum gate set that makes a floor-only system work, and at what floor?**

**No book. No membership. Every valid signal admitted. Two dials: the floor and the gate set.**

### 3.2 — Why it is measurable now and was not before
```
297-signal book at L3/S3     42 loss events   every conditioning scheme starves
full universe at a high floor   thousands     every scheme becomes measurable
```

**Attack A's volatility reversal is REAL — Q1/Q2 negative at p = 0.00061 and 1.6e-05, Q4/Q5 positive at
6.0e-08 and 4.7e-18 — and three attempts to exploit it died for lack of power. ON A UNIVERSE WITH THOUSANDS
OF EVENTS IT BECOMES TESTABLE FOR THE FIRST TIME.**

### 3.3 — The sweep
```
floor      per direction, from where bars become rare to where they vanish
           set it by MATCHING ENTRY-BAR COUNT, not by picking an integer —
           the same number means different things at different universe sizes
           and that error cost four runs yesterday

gate set   start from the base gates already in the scan (ADX>=15, Volume>50)
           add ONE dial at a time and report what each buys ALONE:
             ATR_1M       20 -> 25 -> 30 -> 35
             Micro_Hurst  p90 -> p92 -> p95 -> p97
             the volatility-conditional form Attack A motivates
             D2D          direction only -> require agreement

           IF ONE DIAL MOVES MARGIN TEN POINTS AND ANOTHER MOVES HALF A POINT,
           THE STACK HAS ONE REAL GATE AND SEVERAL DECORATIONS. That is the
           single most useful thing this sweep can produce.

cap        sweep it. Cap 21 was located on a 297-signal book at 973 entry bars
           and there is no reason it transfers.
```

### 3.4 — What would make it the answer
**A floor-and-gate configuration with no membership that reaches 110+ days at zero losing weeks with a tail
inside −$600.** If it exists, **selection stops being a problem** — the pipeline points at any instrument,
derives the stack, and runs.

**If it does not, the honest result is that membership carries something a rule cannot, and the frontier from
step 2 is the deliverable. Both outcomes close the question and neither is a failure.**

---

# THE THINGS THAT DO NOT MOVE TODAY

```
[ ] the seventh month of data      FROZEN until steps 2 and 3 are done
[ ] the deployed book              nothing changes; the 297 is live and untouched
[ ] the sacred five                byte-locked
[ ] determinism at two worker counts    standing, still unmeasured
[ ] the power-failure sweep        Bar_Range > p95 at LONG d3 and VolOfVol > p20
                                     at LONG d5+ are both live in the adopted
                                     stack's justification and neither has had
                                     its event count checked
[ ] Attack B part 2                firing-matrix dimensionality on deep bars
```

---

# THE ONE LINE FOR THE DAY

**Take the filter off the field, turn the frontier into a table with a rule, and find out whether the floor
can do the selecting on its own. Three questions, each with a defined answer, and none of them requires
another fortnight of search.**
