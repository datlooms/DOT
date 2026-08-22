# SELECT — PROVENANCE, THE HIDDEN FOURTEEN, AND THE PATH TO COMPLETION

**Status:** archive document. Supersedes the `MIN_TRADES` explanation of the fourteen carried in
`FABLE_ANALYST_BRIEF.md` §11, `DOT_progress_and_rd_plan.md` 13a, spec v3 §4.2 and all four
non-negotiables.

**Read this before touching the gate layer or the orphan question.**

---

## 0. HOW TO READ THIS DOCUMENT

Every claim below is tagged. The tags are not decoration — three separate defects this month
came from a recorded claim being inherited without its evidence.

| tag | meaning |
|---|---|
| **MEASURED** | run on this frame, console output quoted in the session record |
| **RECORDED** | carried in a spec or prior session, not re-derived here |
| **FALSIFIED** | recorded somewhere in the repo, and contradicted by measurement |
| **OPEN** | not established either way |

If you intend to act on a **RECORDED** figure, re-derive it. That is not ceremony: the
`MIN_TRADES` explanation of the fourteen was RECORDED in five places for weeks and is
FALSIFIED below.

---

## 1. WHAT THE 297 IS — A FOUR-WAY SET UNION

**RECORDED.** Spec v3 §4.3 carries a `source` column on all 297 rows. Three seats parsed it
independently and agreed to the digit.

| source | memberships | objective |
|---|---:|---|
| S0-120 | 120 | loss-day decorrelation, seed set of size **zero** |
| OPTION-B | 119 | itself a fusion, then decorrelation to 70L/50S, then gates by coordinate descent over ~150 options, four passes |
| 60-priced | 60 | chance-pricing, `E_dir < 1` against 4,652 rarity-matched nulls |
| BOOK-50 | 48 | loss-day decorrelation (50 minus the two F1 pairs dropped on measurement) |
| **total** | **347** | across **297 unique rows** |

253 rows sit in one source, 38 in two, 6 in three. `347 − 297 = 50` extra memberships across 44
rows.

**Reconciliation:** §135's own correction records v1 saying 386 memberships when it was 350; the
two dropped F1 rows carried three between them. `350 − 3 = 347`. Consistent.

**PARSE HAZARD.** A naive regex over §4.3 also matches thirteen rows of §8.1's cap table and
returns 310 rows / 360 memberships. **Whitelist the four source tags.**

**The stated design reason:** *"four different objectives means four different error modes, and a
union has a chance of covering what each one misses."* That sentence is the whole architecture. It
is also why §3 below matters.

---

## 2. THE FOUR PROCESSES — TWO OF WHICH ARE THE SAME ALGORITHM

### 2.1 Loss-day decorrelation (S0-120, BOOK-50, and OPTION-B's second stage)

**RECORDED, recovered as executed code independently by two seats:**

```python
piv   = pivot(index='day', columns='signal_name', values='pnl', aggfunc='sum').fillna(0)
loss  = (piv < 0).astype(int)
first = max(cols, key=lambda c: net[c] - 50 * loss[c].sum())     # deterministic seed
while len(chosen) < n:
    overlap = ((loss[c] == 1) & (covered > 0)).sum()             # CUMULATIVE boolean
    score   = (overlap, -net[c])
```

Four properties that are easy to get wrong and change the answer:

- **DAYS, not bars.** Keyed on **exit** date.
- **CUMULATIVE coverage, not pairwise.** A day spoiled by five members costs exactly what a day
  spoiled by one costs.
- `net > 0` pool, **per direction**, fixed-count termination.
- **No RNG.** `seed 0` in `S0-120` is a *set size*, not an RNG seed — confirmed from `seed=()` and
  the `(0, 20, 35, 60)` sweep, where 35 is exactly the size of the `E == 0` group.

**THE MATRIX MUST BE TRUE-SOLO.** One seat built it in 120-signal batches; the jar bound, so a
signal's "solo" losing days depended on which 119 others shared its batch. **2,324 trades deleted.**
The clean rebuild moved K=45 from 16 events / −$918 to 12 / −$612 **and reversed that seat's
exclusion of the objective**. If you rebuild this matrix, build it one signal at a time.

### 2.2 Chance-pricing (60-priced)

**RECORDED.** `E` is the **expected number of rows** in that family-direction at or above this PF
under 4,652 rarity-matched nulls. **It is not a p-value.** Floor is `n_dir / K`.

**Known defect, RECORDED:** it is direction-blind as originally built. `n_trials_family = 1,840`
with `long_share = 0.7951` prices every SHORT row against a null four-fifths LONG. Correction moves
the split 9.0:1 → 4.5:1. Floors differ 3.9×: 0.3106 LONG, 0.0802 SHORT.

**And it is the only frame-stable objective.** `E` never reads the calendar, so the same signals are
selected on any window. By contrast **loss-day decorrelation churns 88% of membership between two
disjoint halves of the same market.** That asymmetry is the single most important fact for anyone
trying to make selection reproducible.

### 2.3 OPTION-B

**RECORDED, and partially undocumented.** `FUSED-50` union → decorrelation over all 1,818 VALID →
re-decorrelated per direction to 70L/50S → gate ladder by coordinate descent.

**`FUSED-50` is 97 signals, not 50** — two independently-selected parents of 50 with 3 overlapping.

**OPEN:** its own construction is not recorded. The competing objective, the field it drew from,
whether 97 was trimmed, and which seat built it are all absent after eight search queries. **If
anyone claims to know, ask for the file and line.**

### 2.4 S0-120

Loss-day decorrelation with a seed set of size zero. See 2.1.

---

## 3. FOURTEEN RECOVERY APPROACHES. ALL FAILED. THAT IS THE FINDING.

**RECORDED:**

| approach | result |
|---|---|
| rank by solo statistics | best rule recalls 78 of 280 against a chance 10.4 |
| rank by co-fire depth | 20 of 280 |
| leave-one-out contribution | split-half rho = −0.060 |
| random-subset ablation | no superadditive structure |
| affinity maximised | selects near-duplicates |
| affinity under a ceiling | anti-coupled sets, loses to random at every ceiling |
| loss-day decorrelation | 12% membership overlap between two halves of one market |
| chance-pricing `E_dir` | **60 of 60 exact, zero false picks** |
| supervised separator | AUC 0.80 held-out, 0.45 against near-misses |
| the region it defined | loses to its own size-matched random band |
| full universe at floor 3 | PF 1.39 ungated, 6.17 with six derived gates |
| full universe at matched floor | margin 32.75 but 210 concurrent positions, −$45,900 worst bar |
| direction balance | buys nothing |
| solo persistence | **zero of 1,818** clear folds AND weeks AND days |

**Every one of those is the correct signature of a union, not a failure of search:**

- members at median rank ~2,000 of 8,016 — individually unremarkable — **correct**
- `d_net` split-half rho = −0.060 — no member matters more — **correct**
- the set at the 4th percentile of random — **the set beats chance** — **correct**
- 12% overlap between halves — one objective, two halves, two picks — **correct**

> **DO NOT SEARCH FOR A SINGLE RULE THAT RECOVERS THE 297.**
> It is searching for the thing that was deliberately not used.

---

## 4. WHAT SELECT PRODUCED, AND IT WORKS

**RECORDED.** Two objectives, a set union, frozen gates. No seed, no RNG, no path dependence. It
runs unchanged on next month's scan.

| system | n | events | worst bar | worst day | −wks | days | PF | margin | net |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| QUANT A30+B L6/S4 | 138 | 14 | −$612 | −$391 | 0 | 86 | 29.86 | **45.85** | $122,221 |
| MANAGER A+B L8/S5 | 197 | 10 | −$773 | −$637 | 0 | 88 | 22.53 | 32.74 | $105,611 |
| QUANT B+D L3/S3 | 116 | 21 | −$918 | −$452 | 0 | 92 | 13.25 | 34.38 | $100,652 |
| MANAGER B K=50 | 100 | 14 | −$644 | −$612 | 0 | 95 | 13.30 | 29.15 | $61,406 |
| WHOLE DOT L7/S4 | 297 | 16 | −$1,224 | — | 0 | 110 | 25.91 | 38.25 | $70,614 |
| WHOLE DOT L3/S3 | 297 | 42 | −$1,224 | −$347 | 0 | 119 | 14.53 | 33.07 | $284,974 |

**Margin 45.85 and worst day −$391 are the best figures measured anywhere in this project.**

`B+D` cleared a full battery: 499 of 500 size-matched random draws beaten on margin at the
resolution floor; only 2 of 500 draws carry zero losing weeks and it does; split-half
select-on-A-score-on-B held at 34.46; all three walk-forward windows profitable with zero losing
weeks each; July 1 loss event on 11 days with a **positive** worst day; anti-system at PF 6.08
best-pruned-arm against 13.25.

**The selection problem is solved as a procedure. It is open as an optimisation** — the incumbent
still owns days (119 v 92) and net.

**The binding constraint is the losing week, not the event count.** `A30+B` at L8/S5 gives 6 events
and L10/S6 gives 1 — **both lose a week.** L6/S4 at 14 with zero was chosen **by rule**, not by
argmax.

---

## 5. THE FOURTEEN — THE RECORDED EXPLANATION IS FALSIFIED

### 5.1 What was recorded

> *"Fourteen appear in no scanner output because they fire fewer than `MIN_TRADES` times."*

Carried in `FABLE_ANALYST_BRIEF.md` §11, `DOT_progress_and_rd_plan.md` 13a, spec v3 §4.2 and all
four non-negotiables.

### 5.2 What is true — **MEASURED**

The eleven still missing after the emit-all run were built directly through the scanner's own
condition builder and `simulate_signal` on the 177,251-bar frame:

```
dir        raw   +elig    +Fri    +D2D  trades      PF
LONG       912     845     828     105      89    1.96   Micro_Entropy:lo + Micro_HLAsymmetry:lo + OR_Low_Side:==-1
LONG       447     414     396     330     158    1.95   Micro_TickIntensity:hi + PrevDay_Low_Side:==-1 + Slope_Accel_LT:hi
LONG       445     262     258     258     258    1.92   D2D_Signal:==1 + VWAP_Dist_ATR:lo + VWAP_Side:==1
SHORT      846     771     709      65      51    1.35   AT_Slope_ST:hi + Micro_MicroGap:lo + Upper_Wick:hi
SHORT      179     172     164     139      81    1.41   AT_Slope_ST:lo + Micro_CSSpread:lo + Sqz_Val:hi
SHORT      281     254     237     216     174    1.66   D2D_Dynamic_Sensitivity:lo + Upper_Wick:hi + VAH_Side:==-1
SHORT      451     329     321     309     183    1.73   ADX_Value:lo + EMA_Oscillator:lo + Micro_Hurst:hi
SHORT      476     388     239     120     108    1.94   Body_Size:hi + Efficiency_Ratio:hi + Micro_BarEntropy:hi
SHORT      214     209     201     195     176    1.46   Micro_IBSP:hi + Micro_VolAccel:hi + VWAP_Z:lo
SHORT      407     331     322     197     182    1.72   Body_Size:lo + Micro_VolAccel:hi + OR_Low_Side:==-1
SHORT      469     414     398     183     142    1.92   Micro_RollProxy:lo + Volume:hi + Volume_Ratio_10:lo
```

**No column goes non-zero to zero. Every one produces 51–258 trades. Every proxy PF lands between
1.35 and 1.96 against a floor of 2.0 — the highest misses by 0.04.**

**They are `MIN_PF` casualties, not `MIN_TRADES` casualties.**

The three that *did* reappear carry PF 4.09, 10.2 and 3.17 — **above 2.0**, so they were never
performance-filtered at all. They were **dedup** casualties, the one lift that ran in the parent
process at collation.

### 5.3 Which PF this is — and why the distinction matters

**MEASURED.** The table above is the **PROXY** PF: `compute_metrics`
(`scanners/triple_convergence_and_d2ddir.py` L293), whose `pf` at L299 is the exact value the
emission gate at L448 compares. **It is not the collated re-score.** The two are different
simulations and the collated catalogue carries the re-score — which is why the default 19,754
contains 5,132 rows at `agg_pf < 2.0` and 1,518 rows at zero trades. Those columns are
demonstrably not the gated quantity.

### 5.4 What is NOT contradicted

**Do not over-read the table.** The recorded line *"eight SHORT orphans contribute exactly zero
trades"* is a **book-level, jar-bound** measurement inside the 297 at cap 21. The table above is
**solo scan entries**. Both can be true simultaneously: a signal firing 176 times solo can open
zero positions in a full book because other signals take the slots.

Likewise *"removing all fourteen costs $3,923, three trading days, and worsens two of three OOS
windows"* stands — the mechanism is depth, not their own trades. Spec v3 labels that defence
*"weaker than a statistic"* and it is right to.

### 5.5 The structural consequence — unchanged and still the point

**Any procedure built on scan statistics is blind to signals the scanner never emitted.** `SELECT`
cannot select what is not in the field. Every seat that looked for the fourteen burned turns
concluding they did not exist.

And it is the operator's regime argument made concrete: **a signal firing rarely *now* is rare
*now*.** Rarity is regime-dependent. A fixed book locks in whatever fired often during the
selection window — which is the whole case for a gate architecture over a membership list.

---

## 6. WHY THE EMIT-ALL RUN DID NOT END THE BLINDNESS

**MEASURED.** The 2h36m F0 run of 2026-08-21 **was not an emit-all run.** All 512 chunks executed
at `MIN_TRADES 30` and `MIN_PF 2.0`.

**Cause:** `orchestrator/discovery_orchestrator.py` L1306 `f0m.EMIT_ALL = True` was the only
assignment in the package, and it runs in the **parent**. Every `ProcessPoolExecutor` worker
re-imports the scanner fresh at module-scope `EMIT_ALL = False`. `dot_frame_binding` carries
`DOT_FRAME_PATH`, `DOT_INPUT_SHA`, `DOT_FP` and `DOT_RESULTS_DIR` across spawn — **`EMIT_ALL` was
never added to that transport.** Parent-only-global class; the exact defect the transport exists to
solve, with the flag omitted.

**Corroborating evidence from the artifact itself:** the run emitted a clean superset (0 default
rows absent, 11,473 new), and the only orphans that returned were the three above 2.0 — because the
**dedup** lift runs in the parent at collation and did take effect.

### 6.1 The fix — MEASURED, built, bounded reproduction quoted

Environment-variable transport (`DOT_EMIT_ALL`), re-established at interpreter startup by
`sitecustomize`, with the worker asserting rather than trusting. **Not a payload field** — a payload
is opt-in per call site, and opt-in transport is what produced the defect.

```
[F0] effective filters: min_trades=30 min_pf=2.0 overlap_threshold=0.8   (EMIT_ALL=False)
MODE default  SERIAL   rows    0 | TARGET ABSENT
MODE default  PARALLEL rows    0 | TARGET ABSENT

[F0] effective filters: min_trades=0 min_pf=0.0 overlap_threshold=1.01   (EMIT_ALL=True)
MODE emitall  SERIAL   rows   15 | TARGET PRESENT pf=1.46 trades=176
MODE emitall  PARALLEL rows   15 | TARGET PRESENT pf=1.46 trades=176
SERIAL == PARALLEL: True
```

Target is `Micro_IBSP:hi + Micro_VolAccel:hi + VWAP_Z:lo` SHORT — one of the eleven, **absent →
present inside a spawned worker**, with the lifted values on the console.

**A second instance of the same class was found by the sweep and fixed:** `f0m.OUTPUT_DIR =
RESULTS_DIR` (L1312) is also parent-only and is read at scanner L492/L493 **inside the worker** —
so every worker wrote `raw_survivors.csv` to the module default `dots_results` rather than the run's
own directory. That is why the file could not be found where expected.

`f0m.MIN_TRADES = F0_MIN_TRADES_OVERRIDE` (L415) is **safe** — it sits inside `run_f0_chunk`, which
executes in the worker.

### 6.2 Consequences for the artifact on disk

**`results_F0_EMITALL.csv` (31,227 rows) is NOT the full catalogue and must not be treated as one.**
It is the default field plus whatever the dedup lift admitted. The full field is **5,011,204
triples** (run log L884, candidate invariant matched), not 19,754.

**The default 19,754 catalogue remains byte-reproducible and is unaffected** — proven before/after
at `c34e86983cebcbb0` on the orchestrated path. Six specifications rest on it.

---

## 7. THE FILTER LADDER — EVERY POINT A SIGNAL CAN VANISH

**MEASURED, exhaustive over the F0 path.**

| # | filter | site | lifted by `--emit-all`? |
|---:|---|---|---|
| 1 | `MIN_TRADES` (5 sites) | scanner L285/375/429/594/597 via `_min_trades()` | yes → 0 |
| 2 | `F0_MIN_TRADES_OVERRIDE = 30` | orchestrator L395 → L415 | yes, by short-circuit |
| 3 | `MIN_PF` | scanner L448 via `_min_pf()` | yes → 0.0 |
| 4 | `OVERLAP_THRESHOLD` dedup | scanner L482 via `_overlap_threshold()` | yes → 1.01 |
| 5 | collation dedup | orchestrator L628 → `f0m.deduplicate` | yes, same helper |
| 6 | `eligible = ADX ≥ 15 & Volume > 50` | scanner L139 | **no — base gate, by design** |
| 7 | volume-zero, Friday 16:45 cut, warmup trim | scanner entry mask | **no — by design** |

**Filters 6 and 7 still make signals invisible.** They shrink the bar universe, so a marginal signal
can reach zero trades with every liftable gate lifted. **If a recovered count comes back short, look
there first** — the missing one will be a signal whose entries sit almost entirely on ineligible
bars, not one a filter dropped.

**Frame-sensitivity sweep:** the equality pool *is* enumerated from the data
(`build_conditions`, `vals_scan` from `scannable` bars), which makes the search space
frame-dependent in principle. **MEASURED and eliminated as a cause here:** pool = 249 conditions,
**0 of 11 unformable**, all of `==-1/==0/==1` present for every equality feature the eleven use.
Only 3 warmup-only values excluded (`OBVf_Signal:0`, `Sqz_State:0`, `RangeOsc_State:0`), none used
by the eleven.

---

## 8. THE ANOMALY THAT REMAINS OPEN

**OPEN.** The F0 candidate field **shrank when the dataset grew**:

- 152,983-bar sealed baseline → **51,311** candidates (execution sequence step 14; non-negotiables
  quant auditor B.1)
- 177,251-bar stitched frame → **19,754** candidates, on **16% more data**

**Thirty-one thousand candidates disappeared between frames and nobody has explained it.**

Hypotheses not yet closed:

1. **The clock correction (17r/17s).** `ExportDataForAnalysis()` wrote SERVER time as EST; the fix
   recomputed `EST_Hour`, `EST_Minute`, `EST_DayOfWeek` from raw broker `Time`, all 169 other
   columns bit-identical. The Friday cut and entry eligibility read those columns. **Quantify: how
   many bars changed `EST_DayOfWeek`, and how many changed entry-eligibility.**
2. **Threshold drift across frames.** Mechanism D is causal rolling-2500 day-refreshed, so bars
   after Jun 25 should not move any threshold before it. `wf_selection` proved 176/176 identical
   between a full series and a training prefix — **prove the same across the two frames on the
   overlapping Jan 19–Jun 25 span.**
3. **Chunk tiling.** 512 chunks slice `combinations(...)[lo:hi]`. The log's `candidate invariant
   5011204 == 5011204` proves the *count* was covered; **it does not prove the slices tile without
   gap or overlap.** Say which claim you are making.

**Note the arithmetic tension:** 51,311 came from a frame where `MIN_PF` was 4.0 in source but 2.0
via the override, same as now. So the shrink is **not** explained by the filter ladder, and that is
what makes it interesting.

---

## 9. NEXT STEPS TO COMPLETE `SELECT`

In order. Each one has a stated acceptance test.

### 9.1 Re-run emit-all with the transport fix — **~100 minutes**

```
python master.py --data data --workers 14 --out discovery\emitall --stage S3 --family F0 --emit-all
```

Artifact: `discovery\emitall\results\results_F0_triple_convergence_and_d2ddir.csv`.

**Acceptance:** every worker's console line reads `min_trades=0 min_pf=0.0
overlap_threshold=1.01`. If any chunk prints `min_pf=2.0`, the transport failed and the run is void
— **check the first chunk's line before letting it run 100 minutes.**

**Expect all fourteen to return.** Eleven at PF 1.35–1.96, three already recovered. If the count
comes back at thirteen, see §7 filters 6–7.

**Row estimate 45,000–55,000 — ARITHMETIC, NOT MEASURED.** Do not quote it as a measurement.

### 9.2 Decide what the fourteen mean for selection

They are now visible. **They are also, on this frame, sub-2.0 PF signals.** That is a decision for
the operator, not a defect to fix:

- include them and the field carries signals that would fail any performance floor
- exclude them and `SELECT` cannot reproduce the incumbent 297

**This is the honest tension at the centre of the whole exercise** and it should be recorded as a
ruling, not resolved by whoever writes the config.

### 9.3 Wire the two proven objectives into `SELECT`

§4 shows the procedure works. **Loss-day decorrelation** and **chance-pricing** are the two that
earned their place — the first for coverage, the second because **it is the only frame-stable one**.
Implement both, emit the union as an arm, and let the operator rule on the arm.

**Do not implement anything from §3.** Those fourteen approaches are documented negatives.

### 9.4 Close the 51,311 anomaly

Until §8 is answered, **nobody knows whether the current field is the right field.** That question
sits underneath every selection decision.

### 9.5 Housekeeping, both small and both outstanding

- The collation log line prints a literal `80% overlap dedup` regardless of the effective value —
  **stale text under `--emit-all`**, same class as the canary that read 2,698 / $92,347 for weeks.
- **`LOADER AUDIT` printed to the console but was never written to `run_log_EMITALL.txt`.** The
  attestation record is incomplete.

---

## 10. STANDING CAUTIONS FOR THE NEXT SEAT

1. **Proxy PF ≠ re-score PF.** Always say which. `compute_metrics` L299 is what the gate compares.
2. **Solo scan entries ≠ book-level trades.** The jar binds; a 176-trade solo signal can contribute
   zero inside a 297-book at cap 21.
3. **Parent-only globals do not survive spawn.** Two instances found; the class is every
   `<module>.<CONST> =` executed before a pool starts. Sweep it on any new pool entry point.
4. **A count in an allowlist gets bumped; a reason gets read.** Applies to `LOADER_ALLOWLIST` and to
   every registry.
5. **The default 19,754 path must stay byte-reproducible** at `c34e86983cebcbb0` on the orchestrated
   comparison. Prove it before and after, never assert it.
6. **Do not inherit a recorded claim you have not re-derived.** The `MIN_TRADES` explanation stood
   in five documents and was wrong.
