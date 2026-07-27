# POST-SCAN DEFECT REGISTER + REMEDIATION CHECKLIST
**First full-scope scan, `discovery\full`, 2026-07-28. 177,251 x 172, input_sha 46586cbb1671.**
Every figure below is OBSERVED from that run's own output or read from source. Nothing inferred.

---

## RUN RESULT FOR THE RECORD

```
F0  collated  19,757 rows  (31,231 raw -> 80% overlap dedup)  invariant 5,011,204 == 5,011,204
F1  collated 439,061 rows                                     invariant 1,713,630 == 1,713,630
    Collated 483,753 candidates -> discovery_master.csv
    MASTER COMPLETE in 0:47:58  (after ~24h of S3 across two sessions)

Per-family: F0 39,514 (DOUBLED — see D1) | F1 439,061 | F3 3,674 | F9 1,224
            F11 103 | F4 66 | F2 47 | F7 28 | F5 16 | F8 10 | F6 10

The ten previously-never-run families produced 5,178 candidates. They were NOT empty.

NEW DISCOVERED book vs the committed book on the SAME corrected data:
                        BOOK-50        DISCOVERED     delta
  book rows                  50                50
  trades                  3,101             4,359     +41%
  win rate                90.6%             85.5%     -5.1pt
  profit factor            4.81              3.13
  net                   $97,675           $79,342
  worst day             -$565.3           -$563.0
  min-fold PF              5.05              3.29
  OOS PF                   2.95              2.05
```

---

# D0 — THE DISCOVERED BOOK WAS NEVER SELECTED  *(CRITICAL — invalidates the entire result)*

**`master.py` L439-456, `_assemble_fresh_book()`:**
```python
c = pd.read_csv(os.path.join(out, 'results', 'candidates.csv'))
if 'worst_day_usd' in c.columns:
    c = c.sort_values(['worst_day_usd', 'agg_pf'], ascending=[False, False])
for _, x in c.iterrows():
    ...
    if len(rows) >= 50:
        break
```

**This is a hardcoded top-50 sort. It never calls the selection layer.**

Not used: `DepthYield`, coverage/REACH, `FailConc`, `TailDep`, `mCVaR`, the absolute survival bound, H.3 within-direction persistence, the per-direction greedy/CELF search, the lookahead-2 stopping rule, multiple-testing correction, stability selection. **Every one of those was computed by S5B, written to CSV — and then ignored by S8.**

**Consequences, all three observed in this run:**
1. **Book size is exactly 50** because L454 says `>= 50: break`. Greedy would stop on marginal gain, not a count.
2. **F1 dominates the book** because F1 is 90.6% of the filtered pool and a naive sort returns proportionally.
3. **REACH did not move** (1.415%/0.762% against the incumbent's 1.389%/0.735%) because coverage was never consulted.

**So the PF 3.13 result says nothing about the selection layer.** It is the score of a top-50-by-worst-day sort. The comparison against BOOK-50 is not a comparison of methods.

**FIX.** S8's discover-fresh path must consume S5B's selected book. If S5B does not currently emit a book artifact, it must. `_assemble_fresh_book` is either deleted or demoted to an explicitly-labelled baseline contender in S7 — never the thing S8 scores as "the discovered book".

**VERIFY.** Book size must NOT be 50 unless the objective independently stops there. The book must span families in proportion to the objective's judgement, not the pool's composition.

---

# D1 — F0 IS DOUBLE-COUNTED IN THE POOL  *(blocking)*

**Observed:**
```
[F0] collated 512 chunks -> 19757 rows
[F0] ingested 19757 rows from results\results_F0_triple_convergence_and_d2ddir.csv
Per-family counts: {'F0': 39514, ...}          19,757 x 2 = 39,514
```

**Cause.** `collate_f0()` writes `results_F0_triple_convergence_and_d2ddir.csv`. That filename is also `F0_CSV`, which `ingest_f0()` reads. The two paths were designed as ALTERNATIVES — F0 out-of-process then ingested — and both now execute.

**Consequence.** `G.1` collapses exact duplicates by bitwise mask identity, so the book is not corrupted by duplicate signals. But pool size feeds §H.1's trial count for the empirical matched null and Benjamini-Yekutieli. **A doubled F0 makes the multiple-testing bar harder than the search warrants**, rejecting genuine F0 signals against phantom trials.

**FIX.** Skip `ingest_f0()` when the chunked path has already collated F0 — the family `.done` marker is the signal. Assert per-family count == collated row count and ABORT on mismatch.

---

# D2 — THE POOL IS 90.6% F1, AND F0 IS PRE-FILTERED TWICE  *(design defect — the reason the book is weak)*

**Observed, S5 grammar table, the filtered pool of 41,148:**
```
F1  13,763 + 9,954 + 9,484 + 4,075 = 37,276   90.6%
F0   1,888 + 1,466 +   304 +    22 =  3,680    8.9%
F9 133 | F3 53 | F4 3 | F11 2 | F2 1
```

**F1 outnumbers F0 ten to one at selection.** F1 is the family that contributed **2 signals** to BOOK-50, measured at **PF 1.85 out of sample** and **+$129** on the unseen June-July segment. F0 contributed 48 and carries the same-bar triple population at WR 98.0% / PF 53.70.

**Cause — an asymmetric funnel, already flagged in `F0_ASYMMETRY_NOTE` and now visibly distorting selection.** F0 passes its own fast scorer (`MIN_PF >= 2.0` after the `run_f0_full` override, `MIN_TRADES >= 30`) BEFORE re-scoring, then S5's filter. **F1 has no pre-screen at all.** 260,130 F0 combos -> 3,680 at selection; 1,713,630 F1 candidates -> 37,276.

**This is not a bug in either family. It is a bug in comparing them.** Two families are being ranked in one pool having passed different numbers of gates.

**FIX — decide deliberately, do not leave it incidental.** Options, with the trade stated:
  (a) apply an equivalent pre-screen to F1 so both face the same funnel depth;
  (b) remove F0's pre-gate and re-score everything (expensive — F0's fast scorer exists precisely to avoid that);
  (c) select per family with an explicit allocation, which reintroduces a quota and breaches doctrine rule 3;
  (d) leave the pool as-is and let the objective sort it — valid ONLY once D0 is fixed and the objective is actually running.

**Recommend (d) first, measured.** With real selection running, DepthYield may reject F1 candidates on merit. If it does not, revisit (a).

---

# D3 — THE WALK-FORWARD STILL PRODUCED NO VERDICT  *(blocking: this is the deliverable)*

**Observed:**
```
[S5C] splits derived 3 | embargo 1440 bars | oracle causal True
null arm: split 0 21/89=0.236 | split 1 19/80=0.2375 | split 2 21/81=0.2593
attestation records 3 (repeat groups 0) | pass criterion: UNEVALUABLE
```

The null arm ran correctly and landed at 23.6-25.9%, consistent with the recorded ~27% baseline. **The BOOK arm produced nothing, so the criterion is still UNEVALUABLE.**

**This is the single deliverable the entire redesign exists for.** The committed book degraded PF 6.40 -> 2.19 on first contact with unseen data because the SELECTION PROCESS was never validated. After a 24-hour scan it is still not validated.

**Almost certainly downstream of D0** — the book arm needs the funnel re-run per split, and if the funnel isn't being used to build the book at all, there is nothing to re-run.

**FIX.** After D0, the book arm must execute per split. If it still cannot, name the specific blocker in one line. **A run that completes without a pass-criterion number has not answered the question it was built to answer.**

---

# D4 — ONE MISSING CHUNK TRIGGERS A FULL SEQUENTIAL RE-SEARCH  *(blocking: cost a 17-day fallback)*

F1 had 3,584 of 3,585 chunks complete with valid `.done` markers. **Chunk index 1 was absent.** Collation returned false and the orchestrator fell through to:
```
[family 1 of 1] F1 (sequential_temporal) starting
Search: 239^2 = 57121 ordered pairs x 15 lags x 2 dir = 1713630 candidates
```
Full scope, one process. At the measured ~0.86s/candidate that is **~17 days.** Killed by the operator.

**The resume path then did it correctly in 5 minutes 23 seconds:**
```
RESUME: 3584 of 3585 chunks already complete on disk
running 1 pending chunks across 1 worker processes
[1/1 100.0%] F1 c0001 336 survivors in 311.3s
[F1] collated 3585 chunks -> 439061 rows | invariant 1,713,630 == 1,713,630
```

**Cause.** The completeness test is all-or-nothing per family and its failure branch re-runs the whole family sequentially, rather than re-queueing the missing chunks. **The correct recovery already exists in the resume path and is not used.**

**FIX.** On incomplete collation, re-queue only the missing indices. NEVER fall through to a full sequential re-search of a chunked family. If a chunk still cannot be produced, ABORT naming the index.
**Also print the missing indices at the point of failure.** The operator had to derive index 1 with a PowerShell set-difference.

---

# D5 — `depth_yield_grid` DOES NOT MEASURE THE TARGET POPULATION  *(design gap — the operator's stated objective)*

**`selection.py` L307:** `tolerances=(5, 10)`, `S_GRID = (3, 4, 5, 6, 7)`. **The tightest tolerance is 5 bars.**

The operator's target is SAME-BAR concurrence — 3+ signals on the SAME bar. Measured on the committed book:
```
same-bar 1 (solo)  1199 tr  WR 88.7%  PF   3.18  net $27,958  wd -$574.0  136 losses
same-bar 2          974 tr  WR 91.0%  PF   5.27  net $22,665  wd -$365.8   88 losses
same-bar 3+         505 tr  WR 98.0%  PF  53.70  net $26,616  wd -$138.9   10 losses
same-bar 4+         256 tr  WR 98.4%  PF  62.00  net $17,812  wd  +$26.8    4 losses
same-bar 5+         160 tr  WR100.0%  PF 999     net $14,182  wd  +$59.5    0 losses
```
A 5-bar tolerance admits t, t+3, t+5 — a temporal cluster, not simultaneous agreement. The "9 variables agreeing on one bar" argument holds only at same-bar. Same-bar IS a subset of 5-bar, so the objective is not blind to it — but it does not distinguish it, so it does not maximise for it.

**FIX.** `tolerances=(1, 5, 10)`. N then reported across three values and chosen from evidence, exactly as S is across five. `selection.py` is not sacred. **S5B is minutes and re-runnable against the same pool — no re-scan.**

---

# D6 — S2B RESUME IS BROKEN, AND THE CHECKPOINT SAVES NOTHING  *(blocking on any resumed run)*

```
S2B already complete for this input (checkpoint) — reading terrain from disk.
pandas.errors.ParserError: Expected 3 fields in line 13, saw 18     (master.py L710)
```

`terrain.py` writes a metadata header block; the reader does a plain `pd.read_csv` with no `skiprows`/`comment=`. **S2B had never been resumed before.**

**And the checkpoint is pointless:** L711 calls `tr.build_terrain(df, w)[1]` regardless, so the resume branch recomputes the terrain anyway and reads the CSV back only to rebuild a frame it is already rebuilding. **S2B costs 3.7 seconds.**

**FIX.** Delete the checkpoint branch and always recompute. Fewer paths, no write/read asymmetry.
**Operator workaround used:** deleted `terrain_episodes.csv` and `.markers\S2B.done`.

---

# D7 — `f0_rows_from_raw` RE-SCORES SINGLE-THREADED, NO PROGRESS, NO RESUME  *(non-blocking: hours)*

19,757 deduped survivors re-scored one at a time through `run_portfolio` + wf primitives. Silent — no heartbeat, no progress, no ETA. Written when F0 collation meant ~168 rows. **No internal checkpointing: if it dies, the entire re-score restarts.**

**FIX.** Chunk the RE-SCORE onto the existing worker queue with per-chunk progress and markers.
**CONSTRAINT — the DEDUP must NOT be parallelised.** `deduplicate()` is a global greedy pass against a running `keep_sets` list; splitting it lets near-duplicates survive. Dedup stays single-pass in ascending chunk order; only the re-score parallelises. **Parity proof against the serial result is mandatory.**

---

# D8 — S3B REPORTS STALE FAMILY VERDICTS  *(reporting defect)*

`families reviewed: 14 | SELECTABLE 2 | INSUFFICIENT-EVIDENCE 10` — printed AFTER all fourteen families produced output in this very run. F0 gave 19,757 rows, F1 439,061, and the ten "insufficient-evidence" families produced 5,178 between them.

**FIX.** S3B must read the current run's family outputs. A verdict of INSUFFICIENT-EVIDENCE against a family that just produced thousands of rows is worse than no verdict.

---

# D9 — S5's FILTER LINE HAS A MALFORMED DENOMINATOR  *(cosmetic)*

`filter (trades>=30 & folds_plus>=4 & agg_pf>=2.0): 41148/6 candidates scoreable by S8`

41,148 of 483,753 passed. The `/6` is wrong in every run observed (legE printed `23/6`). **FIX:** print passed/total.

---

# D10 — CHUNK 1 VANISHED WITH NO REPORT  *(diagnostic gap)*

F1 chunk 1 produced no marker and the queue ran 3,584 further chunks without surfacing it. Cause undetermined — either it raised in its worker (the F0 `AssertionError` pattern, buried in 4,279 lines) or was mid-flight at an earlier Ctrl-C.

**FIX.** At the end of the chunk queue, print per-family completeness — `F1: 3584/3585 complete, MISSING [1]` — BEFORE collation is attempted.

---

# NOT A DEFECT, BUT RECORD IT

**PBO (CSCV) = 0.4286 against a bar of 0.10.** Reported not enforced per §H.1. **4.3x over.** On a top-50 sort that is unsurprising — a naive sort is exactly what PBO is designed to catch. Re-measure after D0; if it stays above 0.10 with real selection running, that is a genuine finding about the search, not the sort.

**CANARY LONG 86.12% / SHORT 100.0%** — both healthy, no plateau regression. The canary runs on the incumbent fixture, so it says nothing about the discovered book.

---

# REMEDIATION CHECKLIST — ORDERED

**PHASE 1 — make the selection layer actually select.** Nothing else matters until this is done; every result above is the score of a sort.
```
[ ] D0   S8 consumes S5B's selected book. Delete or demote _assemble_fresh_book.
[ ] D1   Stop double-counting F0. Assert per-family count == collated count.
[ ] D5   tolerances=(1, 5, 10). Report N across three values.
[ ] D3   Book arm runs per split so the pass criterion produces a number.
```
**PHASE 1 requires NO re-scan.** S3's 483,753 candidates are on disk with valid markers. S4 -> S9 is under an hour.

**PHASE 2 — operability, before the next full scan.**
```
[ ] D4   Re-queue missing chunks; never fall through to sequential re-search.
[ ] D7   Chunk the F0 re-score. Dedup stays serial. Parity proof mandatory.
[ ] D6   Delete the S2B checkpoint branch.
[ ] D10  Per-family completeness line before collation.
```

**PHASE 3 — reporting.**
```
[ ] D8   S3B reads the current run's outputs.
[ ] D9   Filter line prints passed/total.
```

**PHASE 4 — then measure, in this order.**
```
[ ] Re-run S4->S9 on the existing pool with PHASE 1 applied. No re-scan.
[ ] Read the DepthYield grid at N=1 vs N=5 vs N=10 and CHOOSE from evidence.
[ ] Read the pass criterion. A FAIL is a legitimate answer.
[ ] Read REACH. If it has not moved from ~1.4%/0.76%, coverage still is not binding.
[ ] Compare like-for-like against BOOK-50 on the SAME corrected data:
       3,101 tr | WR 90.6% | PF 4.81 | net $97,675 | wd -$565.3 | OOS PF 2.95
[ ] D2   Only then decide whether F1's 10:1 pool dominance needs an explicit remedy.
```

**PHASE 5 — the operator's gating scheme, measured but not built.**
```
solo    -> Hurst p90 + ticks >= 300
double  -> Hurst p90
triple+ -> free
```
On the unseen June 25 - July 21 segment, applied to the committed book:
```
ungated  334 tr  WR 81.1%  PF  2.25  net $7,359  wd -$639.1  63 losses
gated     89 tr  WR 94.4%  PF 16.63  net $6,307  wd -$101.9   5 losses
gated @2  89 tr  WR 94.4%  PF 13.50  net $7,045  wd -$176.0   5 losses
```
96% of the money, 28% of the worst day, 8% of the losses. **Out of sample, gates specified from reasoning rather than fitted.**

**STILL OPEN — operator decisions.**
```
[ ] Flat 2 lots vs conviction sizing   (+$10,777 net vs +8 points of PF)
[ ] Drop F1 sequential?                 ($129 on unseen data, PF 1.85)
[ ] Keep gap fillers?                   (tripled the worst day for +17% net)
```
