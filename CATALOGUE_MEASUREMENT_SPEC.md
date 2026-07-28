# CATALOGUE_MEASUREMENT_SPEC.md

**Seat:** Quant Analyst — specification only. No objective, no choice, no build.
**Against:** `CORRECTION_CHECKLIST.md` (root), `dot_master_discovery/POST_SCAN_DEFECTS.md`,
`DOT_signal_discovery_mantra.md` (standing doctrine).
**Repo:** `master.py` `3d45bd3b7f74`; sacred five verified intact
(`518862bf19fb` / `793e6e5f8d9a` / `6530e2508b17` / `bb498eb13ce3` / `27af7acee824`).
**Date:** 2026-07-28

Every figure below carries its parameters at point of use and is labelled **MARKET**
(price/full-population) or **BOOK** (a property of a specific signal set).

---

## 0. VERIFICATION PERFORMED BEFORE SPECIFYING

Four claims in the brief were checked against source and data rather than accepted. Three
reproduce; **three need correcting**, and two of the corrections change what must be built.

### 0.1 — Q4's hard constraint: CLEAN

`cluster_profiler.thrust_thresholds` builds a `_D_SPEC` and routes through
`dt.compute_adaptive_thresholds` via `_swept`. **K and E are mechanism-D (rolling-2500,
day-refreshed, floor-index). They are NOT local percentiles.** The terrain denominator is not
look-ahead-contaminated and every coverage % inherits a causal threshold.

One residual, flagged not fixed: `terrain.py` L160-161 (`q1/q3_disp_pts`) and L177 (the
largest-decile reporting cut) ARE local `np.percentile` calls. They are **descriptive output
statistics** and gate nothing. **If any catalogue column strata on displacement decile, that
cut must route through mechanism D or be declared descriptive in the column name.**

### 0.2 — CORRECTION: 7,490 is a SINGLE CELL, not a union, and the grid is FOUR cells

Measured, MARKET, corrected TRUE-EST frame, `terrain.build_terrain`:

| cell | episodes | UP | DOWN |
|---|---|---|---|
| **W=15 K=p85 E=p75** | **7,490** | **3,816** | **3,674** |
| W=15 K=p90 E=p75 | 6,003 | 3,025 | 2,978 |
| W=30 K=p85 E=p75 | 5,868 | 3,007 | 2,861 |
| W=30 K=p90 E=p75 | 4,874 | 2,415 | 2,459 |
| sum across cells | 24,235 | | |

`GRID_W=(15,30)`, `GRID_K=(0.85,0.90)`, `GRID_E=(0.75,)` — **2×2×1 = four cells.** The brief's
"7,490 as a union across a two-cell grid" is wrong on both counts. C4's own quoted 3,816 UP /
3,674 DOWN match the single cell **exactly**, so the pinning is already de facto correct and
only needs stating. §4 states it.

### 0.3 — CORRECTION: C3a is a TOLERANCE defect, not a within-bar defect

The checklist says one signal firing five times in five bars scores as five converging signals.
That is right, but the mechanism is the tolerance window, not row-vs-signal counting within a
bar. Measured, BOOK, incumbent book on the corrected frame:

**On a single bar, entry-rows and distinct-signals are identical on 0 of 1,859 bars**
(max 6 both) — `run_portfolio` admits at most one entry per signal per bar, so at `n_tol=0`
the two bases coincide exactly. They diverge as tolerance widens:

| n_tol | clusters | max size (ROW basis, current code) | max size (DISTINCT-SIGNAL basis) | size≥3 rows | size≥3 distinct |
|---|---|---|---|---|---|
| 0 | 1,859 | 6 | 6 | 138 | 138 |
| 1 | 1,555 | 18 | 14 | 227 | 219 |
| **5** | 974 | **39** | **18** | 300 | 267 |
| 10 | 730 | 55 | 26 | 306 | 275 |
| 30 | 425 | 61 | 29 | 235 | 220 |

**The row basis overstates maximum cluster size by up to 2.2× at grid tolerances.** The fix is
therefore: **count DISTINCT `signal_idx` within the tolerance run, not entry rows.** Stating it
as "same-bar" understates the defect, because at same-bar there is nothing to fix.

### 0.4 — Q3's set-dependence: CONFIRMED independently, with one structural simplification

Same signal (BOOK, incumbent, `signal_idx=14`, 90 entries, LONG), same-bar distinct-signal
co-occurrence profile `[+0,+1,+2,+3,+4plus]`:

| reference set | profile |
|---|---|
| full book (n=50) | `[58, 29, 1, 2, 0]` |
| 10-signal subset (n=10) | `[81, 9, 0, 0, 0]` |
| same-direction only (LONG, n=37) | `[58, 29, 1, 2, 0]` |

Set-dependence is real. My figures differ from the brief's `[2,16,13,8,12]` / `[15,23,11,2,0]`
because that is a different target and subset; **the principle is what I verified, and it holds.**

**Structural simplification worth building on:** same-direction-only returns the *identical*
profile to the full book. Cross-direction co-firing is **structurally zero** —
`build_signal_masks` applies `mask & (d2d_dir == direction)`, `D2D_Trend_Dir` is scalar per bar,
so LONG and SHORT masks are disjoint by construction. **Therefore "full pool" and
"same-direction pool" are the same reference set for co-occurrence purposes.** The only
meaningful distinction is **family vs pool**, which halves the column count in §3.

### 0.5 — CORRECTION: the pool size for §5 is ambiguous between three numbers

- **41,148** — the S5 grammar-table filtered pool. Composition sums exactly:
  F1 37,276 (90.6%) + F0 3,680 (8.9%) + F9 133 + F3 53 + F4 3 + F11 2 + F2 1 = 41,148.
- **46,245** — rows in the corrupted `candidates.csv`, with 6,938 duplicates. This is the C2
  file-format defect (headerless part files admitted by S4's glob). **Superseded artifact.**
- Neither is clean: **D1** doubles F0 (19,757 × 2 = 39,514 in per-family counts) and **D2**
  records the pool as 90.6% F1 because F0 passes a pre-screen F1 never faces.

**Consequence for §5: `N` is load-bearing and cannot be a literal.** A false-discovery
expectation computed against 41,148 prices a search that may not be the search performed.
**`N` must be computed at run time, per family, after C1/C2/D1 are fixed, and stamped in the
artifact.** §5 specifies it that way.

---

## 1. Q1 — THE `VALID` PREDICATE

### 1.1 The governing distinction

**`VALID` IS A MEASURABILITY-AND-SURVIVAL PREDICATE, NOT A QUALITY PREDICATE.** This is the
whole reason the catalogue is not a chooser. A signal is `VALID` when there is *enough evidence
on the training segment to state its statistics honestly and it does not breach the account
constraint*. It is **not** "good".

Quality — PF, WR, terrain coverage, co-occurrence, null pricing — goes in **columns the
operator reads**. Nothing in `VALID` ranks, caps or argmaxes. `NOT VALID` means *not reliably
measurable here*, never *rejected on merit*.

This keeps doctrine rule 2 (include, then let the evidence sort) and makes rule 5 tractable:
"this family catalogues nothing valid" becomes a statement about measurability with a stated
reason code, not a negative verdict asserted without evidence.

### 1.2 The predicate — computed on the training segment alone

For signal `s`, training segment `T`, direction `d = dir(s)`:

```
VALID(s | T) := SUFFICIENCY(s,T) AND SURVIVAL(s,T) AND MEASURABILITY(s,T) AND REGIME_EVALUABLE(s,T)
```

**V1 SUFFICIENCY** — `trades(s,T) >= MIN_TRADES`, `MIN_TRADES = 30`.
A sample-size floor, not a fitted quality bar. It is the existing S5 constant and is retained
*as a sufficiency floor only*; it is structural, so it is not a number derived from the segment.
Report `trades(s,T)` in the catalogue regardless, so the operator sees marginal cases.

**V2 SURVIVAL** — `worst_day(s,T) >= -(FTMO_DAILY_CEILING * (1 - MARGIN))`.
`FTMO_DAILY_CEILING = 2500` and `MARGIN` are **external account facts**, not data-derived, so
they are admissible per "no numbers you don't derive from the train segment". Evaluated on the
**FULL population** basis (gap fillers included) because the ceiling does not distinguish
sources. Survival-first ordering: this is checked before any profitability quantity.

**V3 MEASURABILITY** — the signal's headline statistics must be computable and finite on `T`:
- `>= 1 losing trade` (else `PF` is undefined; report `PF = inf` with a `pf_undefined` flag
  rather than a number that ranks first).
- `>= MIN_ACTIVE_DAYS = 10` distinct **entry-basis** trading days (the entry basis is the
  coherent one — clusters are built from entry bars).

**V4 REGIME_EVALUABLE** — segment-local monthly bucketing per the amended §H.3:
- buckets = whatever calendar months `T` contains, **minimum 3**;
- evaluated **within direction** per §H.3.1;
- if `T` yields fewer than 3 buckets **for that direction**, the signal is
  **`UNEVALUABLE`, not `INVALID`** — reported as such, never silently culled.

**V4 sets a bar but does not gate:** `regime_positive_buckets` and `regime_total_buckets` are
**columns**. Whether "positive in all but at most one" is required for inclusion is the
operator's call and is **not** specified here — the catalogue reports the counts.

### 1.3 Nothing full-span, and the three-value return

**No component reads a bar outside `T`.** No `wf.FOLDS` (month-literal Jan–Jun, sacred, unusable
in-segment per §B.1), no `OOS_MONTHS`, no full-series percentile, no incumbent-anchored bound.

`VALID` returns one of **three** values, and the third is the point:

| value | meaning |
|---|---|
| `VALID` | measurable and survives; enters the catalogue |
| `UNEVALUABLE` | insufficient structure to judge (V3/V4 unmet); **enters the catalogue flagged**, with `reason_code` |
| `INVALID` | V2 breached — fails the account constraint on train |

`UNEVALUABLE` rows are **retained in the catalogue with their statistics blank and a
`reason_code`**, not dropped. Dropping them would make "this family catalogues nothing" and
"this family could not be measured" look identical, which is precisely the rule-5 failure.

*Falsification:* if `VALID` admits ≥95% of every family's candidates on every split, it is not a
predicate, it is a formality, and the sufficiency floors need re-derivation. The build reports
the admit rate per family per split.

*Fit risk:* `MIN_TRADES=30` and `MIN_ACTIVE_DAYS=10` are inherited constants. They are
sufficiency floors rather than quality bars, so a wrong value costs coverage, not correctness —
but they must be **restated, not re-fitted**, inside each split.

---

## 2. Q2 — THE WALK-FORWARD VALIDATES THE INCLUSION RULE

### 2.1 The one-line statement the artifact must carry

> **S5C certifies the CATALOGUE'S INCLUSION RULE, not any book. It measures whether signals
> admitted by `VALID` on a training segment persist out of sample above chance. Any book the
> operator hand-assembles from the catalogue afterwards is EXPLICITLY NOT WHAT S5C CERTIFIES.
> Re-scoring a hand-assembled book per split is rejection-list item 1 and must not be done.**

### 2.2 Per split `s`

1. Build the train catalogue: apply `VALID(· | T_s)` to every candidate. Denote the admitted
   set `A_s` (`VALID` only; `UNEVALUABLE` excluded from the arm and **reported separately**).
2. Score every member of `A_s` on the test segment `E_s`, embargo ≥1,440 bars, single touch.
3. `persist(x, E_s)` is defined exactly as:

```
persist(x) := trades(x, E_s) >= 1
              AND net(x, E_s) > 0
              AND worst_day(x, E_s) >= -(FTMO_DAILY_CEILING * (1 - MARGIN))
```

**Deliberately excluded from `persist`:** any PF threshold, any WR threshold, any comparison to
the train value. Persistence is *positive and survives*, nothing more. A PF bar would import a
quality judgement into a measurement of the inclusion rule.

4. Three counts, all reported, never collapsed:

```
n_included(s)      = |A_s|
n_traded(s)        = #{x in A_s : trades(x, E_s) >= 1}
n_zero_trade(s)    = n_included(s) - n_traded(s)
book_rate(s)       = #{x in A_s : persist(x)} / n_traded(s)
```

**The denominator is `n_traded`, not `n_included`.** A signal that never fired on test did not
fail — it was not given the opportunity, and folding it in as a failure would conflate silence
with loss. `n_zero_trade(s)` is reported beside the rate so the reader sees how much of `A_s`
went untested.

### 2.3 The null arm, and the one thing that must match

The existing null stands: same-bar-qualifying random triples, seeded, **89 / 80 / 81 qualifiers**
across the three derived splits at rates **0.236 / 0.2375 / 0.2593** — consistent with the
recorded ~27% baseline and comfortably above the `n_null >= 40` hard floor.

**`null_rate(s)` MUST be computed with the identical `persist` definition and the identical
`n_traded` denominator convention.** If the book arm excludes zero-trade signals and the null
arm does not, the ratio compares two different quantities. The build asserts this and aborts on
mismatch.

### 2.4 The verdict

```
ratio(s) = book_rate(s) / null_rate(s)
VERDICT  = mean_s ratio(s), min_s ratio(s), and the 95% lower bound on the mean
```

Pass thresholds are **already specified** in `discovery_redesign_spec.md` §I.3 (mean ≥ 2.40,
min ≥ 1.85, 95% LB > 1.0) and are **not re-specified here**. If any split has
`n_traded(s) < 40` the split is `UNEVALUABLE` and says so — it is not imputed.

*What a fail looks like:* `ratio` at or below 1.0 means the inclusion rule surfaces signals that
persist no better than same-bar-qualifying random triples. **That is a legitimate, reportable
result about the generator, and no bar is lowered to avoid it.**

---

## 3. Q3 — EVERY CO-OCCURRENCE COLUMN CARRIES ITS REFERENCE SET

### 3.1 The rule

**A co-occurrence count is meaningless without its reference set, and the catalogue has no book
to supply one.** Therefore the reference set is **part of the column name**, not the header.

Two reference sets only — §0.4 established that same-direction and full-pool are identical by
construction, so a third would be a duplicate:

```
cofire_ntol{T}_famvalid_k{0|1|2|3|4plus}     reference = the VALID set of THIS family on THIS segment
cofire_ntol{T}_poolvalid_k{0|1|2|3|4plus}    reference = the VALID set of the WHOLE pool on THIS segment
```

`{T}` is the tolerance in bars and is **in the name**, because §0.3 shows the counts move by
2.2× across the grid.

### 3.2 What is counted

**Distinct `signal_idx` within the tolerance run, never entry rows** (§0.3). For each entry of
signal `s`, count the number of *other distinct signals* in the reference set with an entry
within `T` bars; bucket into `k = 0, 1, 2, 3, 4+`.

At `T=0` this is same-bar and the two bases coincide (measured: 0 of 1,859 bars differ). At
`T>0` they diverge and the distinct basis is the correct one.

### 3.3 Reference-set pinning, and the recursion

The family reference set **is the catalogue**, which depends on `VALID`, which depends on the
segment. That is a recursion and it must be pinned explicitly:

- Both reference sets are computed **once per (family, segment)** from the `VALID` set,
  **before** any co-occurrence column is written.
- The artifact header records `famvalid_n` and `poolvalid_n` for that segment.
- **`UNEVALUABLE` rows are NOT in either reference set** but DO receive co-occurrence columns
  measured against it, so their profile is comparable.

### 3.4 The tolerance curve — reported, not chosen

Per doctrine rule 3 and the brief's explicit instruction, **`T` is not chosen here.** The
catalogue emits the co-occurrence block at **every** `T ∈ {1, 5, 10, 15, 20, 25, 30}` plus
`T = 0` (same-bar), and the operator reads the curve. Illustrative shape, BOOK, incumbent book,
corrected frame — clusters and max distinct-signal size: `T=0` 1,859/6; `T=5` 974/18;
`T=30` 425/29.

---

## 4. Q4 — TERRAIN COVERAGE

### 4.1 The pinned denominator

**PINNED CELL: `W=15, K=p85, E=p75` — 7,490 episodes, 3,816 UP / 3,674 DOWN (MARKET,
price-only).**

Reason, stated so it is not mistaken for a preference: it is the **largest** cell, therefore the
**most conservative** denominator (the hardest coverage percentage to achieve), and it is the
cell every existing figure in the record already uses — C4's own 3,816/3,674 pin it. Choosing a
smaller cell would inflate every coverage % by definitional choice, which is the §D.3.3 error.

**The cell is in the column name, always:**

```
terrain_touch_pct_W15K85E75_UP
terrain_touch_pct_W15K85E75_DOWN
```

The other three cells are emitted as a **sensitivity block** (`terrain_touch_pct_W15K90E75_*`
etc.), clearly separated, so grid-dependence is visible without competing with the pinned column.

### 4.2 Touch, position, and capture — three columns, one threshold-free

**Touch is presence and is unambiguous** — an entry bar inside `[b0, b1]` of an episode of
matching direction. Keep it, name it `touch`, and do not let it be read as capture.

Presence is genuinely weak: the incumbent's `entry_pos_median` is **0.343 UP / 0.400 DOWN**
(BOOK) — it arrives roughly a third of the way in. So position must be its own column:

```
terrain_entry_pos_median_W15K85E75_{UP|DOWN}    0 = at b0, 1 = at b1; median over touched episodes
terrain_entry_pos_q1_/_q3_...                    the spread, because a median hides bimodality
```

**Capture is NOT thresholded**, because any capture bar is a number I would be choosing.
Instead:

```
terrain_capfrac_median_W15K85E75_{UP|DOWN}
    = median over touched episodes of ( realised directional move from entry to trade exit )
                                      / ( episode absolute displacement )
```

reported with q1/q3. The operator reads the distribution and applies whatever capture standard
he wants. **No `capture_pct` column exists**, because it cannot exist without a chosen threshold.

### 4.3 Causality — verified, and the standing requirement

Verified clean (§0.1): K and E route through mechanism D. **The build must assert this at run
time** — that `_D_SPEC` was populated for the thrust columns and `compute_adaptive_thresholds`
produced the arrays — and **abort** if any episode-defining threshold is a local percentile.

The terrain is **MARKET** and its denominator is **not** filtered by ADX/Volume: that predicate
is engine tradability, a BOOK property, and applying it inflates reported reach ×1.83 with no
change in the book. The `start_bar_eligible` flag makes the eligible subset recoverable without
redefining the map. (`terrain.py` `dcaecaf7e8e1`.)

---

## 5. Q5 — WHAT A PER-SIGNAL STAT CAN SUPPORT IN A CATALOGUE OF ~41,148

### 5.1 The problem stated precisely

Nothing is chosen by the machine, **so the multiple-testing burden transfers to the operator's
eye.** If he reads the catalogue and picks the twenty best-looking PFs, he has run a selection
of size ~41,148 with no correction, and the highest PFs in a pool that size are substantially
chance. The catalogue's job is to **make each row carry the price of the search that produced
it**, so eyeballing is not an unguarded search.

### 5.2 Pricing is PER FAMILY, and that is not a convenience

`N` is per-family, and this **resolves D2 as a side effect**: the pool is 90.6% F1 / 8.9% F0
because F0 passes a pre-screen F1 never faces, so a pool-wide correction would be dominated by
F1's 37,276 trials and would price F0's rows against a search F0 never underwent. **A per-family
catalogue never compares across families, so per-family pricing is both correct and sufficient.**
No cross-family adjustment is specified, and none should be added.

**`N_family` is computed at run time, never a literal** (§0.5): the count of candidates in family
`F` actually *tested* on that segment, after C1 path resolution, after C2's part-file exclusion,
after D1's F0 de-duplication. It is stamped in the artifact header per family per segment.

### 5.3 The columns each row must carry

For family `F` on segment `T`, with `N_F` trials and a **matched null** — random signals drawn
from the same post-§G vocabulary, **fire-rate matched** to `F`'s candidates, run through the
**identical `VALID` predicate** — the following are mandatory per row:

```
n_trials_family                    N_F, computed, stamped
null_valid_rate_family             fraction of matched-null signals passing VALID in F
expected_valid_by_chance_family    N_F * null_valid_rate_family

pf_null_p50_family / p90 / p99     PF quantiles among null signals that passed VALID in F
pf_null_exceedance_pct             P(null PF >= this row's PF)
EXPECTED_ROWS_AT_OR_ABOVE_THIS_PF  N_F * pf_null_exceedance_pct     <-- THE PRICING COLUMN
q_value_BY_family                  Benjamini-Yekutieli q at family stratum
```

**`EXPECTED_ROWS_AT_OR_ABOVE_THIS_PF` is the column that does the work.** It answers, on the row
itself: *at this family's trial count, how many rows this good does chance alone produce?* A PF
of 12 beside an expected-count of 340 is not elite. A PF of 6 beside an expected-count of 0.4 is.
**The operator can read the shortlist and the pricing is already done.**

**Benjamini–Yekutieli, not Benjamini–Hochberg** — measured on the live vocabulary, signed
pairwise dependence is 49.6% positive / 50.4% negative across 29,161 pairs, so PRDS fails and BH
is invalid. BY is valid under arbitrary dependence.

### 5.4 The header line the artifact must carry

> **This catalogue contains `N_F` rows for this family. Reading it and selecting rows IS a
> search of size `N_F`. `EXPECTED_ROWS_AT_OR_ABOVE_THIS_PF` prices that search on every row.
> A row whose expected-count exceeds 1 is not evidence of an edge.**

*Fit risk:* the matched null is generated on the same segment and shares its structure — which
is the point; it is a null for *this* market, not a universal one. Stated openly.

---

## 6. Q6 — THE POST-HOC PAIRWISE WORKFLOW

### 6.1 Why a column cannot do this

`FailConc`, `TailDep` and `mCVaR` are **set properties**. They have no per-signal value and
fabricating one would be worse than omitting it. That is correct in the checklist and is not
disputed.

But the omission is exactly the union-collapse exposure: **a 448-signal persistent union scored
PF 1.82 against the curated 50-signal book's PF 6.40.** An operator assembling from fourteen
catalogues can build a set that fails every pairwise constraint and see nothing in the columns
warning him.

### 6.2 The tool

A single command, one input, no options that change the verdict:

```
python score_book.py --book my_book.csv --data <frame> --out <dir>
```

**Input:** a CSV of `signal_id` (plus optional `direction` for assertion). Nothing else.

**Computes, on the assembled set:**

| quantity | parameters, at point of use |
|---|---|
| `TailDep` | `tau = 0.20`, `MIN_SHARED = 10`, **raw daily P&L, not `min(pnl,0)`** |
| exclusion-bias diagnostics | `exclusion_bias_degeneracy_guarded`, `degenerate_excluded_pairs_k_lt3` |
| `FailConc` | worst single-day loss as a multiple of mean daily loss |
| `mCVaR_i` | per-signal marginal tail contribution, worst 5% of book days |
| absolute survival | worst modelled day vs the FTMO ceiling, **FULL population** |
| union terrain coverage | pinned cell `W15K85E75`, per direction |
| same-bar depth ladder | distinct-signal basis, `T=0` and the full tolerance curve |
| directional composition | LONG/SHORT split — **reported, never targeted** |

**Uses the identical estimators as S5B** — imported, not reimplemented. A second implementation
is how the two arms drift.

### 6.3 "Refuses to run silently" — made concrete

Three mechanisms, because a convention is what failed fourteen times:

1. **Non-zero exit on breach.** Any hard-constraint breach → exit code ≠ 0 and the breach named
   on stdout. A wrapper script cannot ignore it by accident.
2. **Append-only attestation.** `book_scored.jsonl` — `book_sha256`, `input_sha`, `code_sha`,
   UTC timestamp, every computed constraint and its verdict. Append-only; repeats reported, not
   blocked.
3. **The catalogue names the gap.** Every catalogue CSV carries, in its header:

> **PAIRWISE CONSTRAINTS ARE NOT IN THESE COLUMNS.** `FailConc`, `TailDep` and `mCVaR` are set
> properties and have no per-signal value. **Any book assembled from this catalogue is UNSCORED
> until `score_book.py` has been run on it and an entry exists in `book_scored.jsonl`.**

That third mechanism is what closes "skipped by omission": the omission is stated on the
artifact the operator is reading at the moment he would omit it.

### 6.4 On the stub

`master.py` L1174 `_sel_con = lambda d, ss: (True, '')` must be **deleted, not left dangling**.
Under the catalogue there is no search for it to gate, so a stub that returns `True` is a
verified region unreachable from production — the exact pattern that produced the fourteenth
defect. The constraint machinery lives in `score_book.py` and is reachable there.

---

## 7. WHAT THIS SPECIFICATION DOES NOT DO

- **No objective.** No argmax, no ranking that gates inclusion. `VALID` is measurability and
  survival; every quality statistic is a column.
- **No choice of `N`.** The tolerance curve `{0,1,5,10,15,20,25,30}` is reported in full.
- **No quota, floor, target or expected composition.** Directional composition is a reported
  output. **A family or a direction cataloguing zero `VALID` signals is a legitimate reported
  outcome** — and per rule 5 it must arrive with the same evidence as a rich one, which is why
  `UNEVALUABLE` is a distinct third return value carrying a `reason_code` (§1.3).
- **No build.**

---

## 8. GROUNDED vs PROPOSED

**Grounded — measured this session, reproducible through the ratified path:**

- §0.1 mechanism-D routing of K and E, verified in `cluster_profiler._swept` / `thrust_thresholds`.
- §0.2 the four-cell grid and 7,490 as a single cell (7,490 / 6,003 / 5,868 / 4,874).
- §0.3 the row-vs-distinct divergence across the tolerance grid (max 39 vs 18 at `T=5`).
- §0.4 co-occurrence set-dependence (`[58,29,1,2,0]` vs `[81,9,0,0,0]`) and the structural
  equality of same-direction and full-pool reference sets.
- §0.5 the pool-composition arithmetic summing to 41,148, and the three-way ambiguity.
- The null arm's 89/80/81 qualifiers at 0.236/0.2375/0.2593, read from the run record.

**Proposed — awaiting a first run, no result claimed:**

- That `VALID` admits a workable number of signals per family. If it admits ≥95% everywhere it
  is a formality (§1.3 falsification); if near zero, the sufficiency floors are wrong for these
  families. **Unknown until it runs.**
- That the inclusion rule clears `ratio > 1.0` at all. **The generator may not beat chance, and
  that is a reportable result.**
- That `EXPECTED_ROWS_AT_OR_ABOVE_THIS_PF` is small enough anywhere to make a row defensible.
  At `N_F = 37,276` for F1 it may not be for any row, which would itself be the finding.
- That the matched null can be generated at sufficient volume per family within the compute
  budget.

**Standing limitation:** six months, one instrument, two partial months, one dominant crash
month. The corrected clock has re-based every historical figure; the incumbent reference is now
3,101 tr / WR 90.6 / PF 4.81 / $97,675 (BOOK, corrected frame), not the pre-2026-07-27 numbers.
