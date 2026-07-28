# DEV_CHECKLIST

Sacred five byte-locked: 518862bf19fb / 793e6e5f8d9a / 6530e2508b17 / bb498eb13ce3 / 27af7acee824.
No scanner edits. No objective, no argmax, no quota, no floor. Operator picks N from the grid.
This file is SELF-CONTAINED. Everything needed to build is here, including the definitions appended at the end. CORRECTION_CHECKLIST.md and CATALOGUE_MEASUREMENT_SPEC.md are reference-only reasoning, not required reading.

BASELINE CORRECTION — apply wherever it appears: same-bar 3+ is 512 trades / PF 35.11, not 505 / PF 53.70. Mixed-family penalty is all-F0 PF 35.72 vs MIXED 29.35 — an 18% cost, not 49%.

## BROKEN — nothing runs correctly until these land

1. Paths — every read/write resolves inside `--out`. Delete legacy fallbacks at `master.py L858-860`, make `_find_outputs` (`family_evidence.py L73-77`) read the run tree only, point F13's `RESULTS_DIR` (`orchestrator L48`) at the run tree.
2. Glob — S4 excludes `*_part*.csv`, and drop the `F1_part*.csv` pattern at `family_evidence.py L53`.
3. Split — delete `split_tree()` (`master.py L111-122`) and `--chunk-mb`. Every artifact writes as one file.

## COUNTING — these nine ship together or not at all (4-12 INDIVISIBLE)

4. Count distinct `signal_idx` within a tolerance run, not entry rows, and build tolerance runs PER DIRECTION, never pooled across both. Same-signal re-fire is 1.8% at N=1, 15.4% at N=5 and 26.1% at N=30 per direction; pooled clustering gives 27.6% at N=30 and shifts every downstream count.
5. Per-signal terrain coverage against BOTH denominators, pinned cell W15/K85/E75 named in every column: raw terrain (7,490 episodes) and REACHABLE (episodes holding >=1 eligible bar where D2D agrees with episode direction), reachable computed PER GRID CELL and never once. Reachable is the primary figure; abort the run if any episode threshold is a local percentile rather than mechanism D.
6. Emit the unclaimed-reachable set — reachable episodes no catalogue signal touches, with direction, duration, displacement, time-of-day, plus `n_conditions_firing` and `n_valid_triples_touching`. Those last two separate a SEARCH gap (many conditions fire, no valid triple lands) from a GRAMMAR gap (few fire); without them the set shows what is unoccupied but not why.
7. Emit per-signal touched-episode IDs (or a bitmap) alongside the coverage percentages, so the union and saturation curves can be re-derived from the artifacts alone. Coverage % on its own makes the round's load-bearing evidence unreproducible.
8. Per-signal multiple-testing price — the eight columns defined in APPENDIX A at the end of this file. `N_F` computed per family at run time, never a literal, and Benjamini-Yekutieli not Hochberg.
9. Add `folds_plus`, `min_fold_pf` and OOS to the same per-signal row using segment-local buckets. `wf.FOLDS` is month-literal Jan-Jun and unusable in-segment; if they are dropped instead, say so in the header.
10. Score every signal BOTH ungated and with the conviction gate applied (solo: Hurst p90 + ticks>=300; double: Hurst p90; depth 3+ free), emitting trades, WR, PF, worst day and net for each arm plus the delta. This is what lets the operator set gating per signal instead of book-wide; it is a measurement, the gating itself lives in the EA.
11. Pool-level same-bar cohort table — family composition of each bar as a curve over depth, not fixed at 3. Counts only, no P&L: depth-3 has no discriminating power at pool scale and P&L needs a book.
12. `dilution_curve.csv` — admit signals best-first and re-score the same-bar 3+ population at each step, over the WHOLE catalogue, not a top-ranked subset. Name the ranking key and emit the curve under at least two keys (PF and `EXPECTED_ROWS_AT_OR_ABOVE_THIS_PF`), because the stop-point differs by key.

## SELECTION — stop it choosing

13. Remove `/n_signals_d` from any objective (`selection.py L290`). Keep it as a reported column that never gates inclusion.
14. Delete the `_sel_con` stub (`master.py L1174-1175` — a `def`, not a lambda). Constraint machinery moves to item 16.
15. The catalogue is emitted from VALID, never from an argmax: no selected-book file is written in discover-fresh at all, and `sel.greedy_direction` is retained IN-MEMORY ONLY as item 12's dilution-curve admission loop. S8's discover-fresh path (`master.py L472`) currently reads `selected_book.csv` and scores it — disable that path with a message pointing at item 16, and leave S8's FROZEN path (`L468-470`) untouched so the committed book still scores from its ratified file.
16. `score_book.py --book <csv> --data <frame> --out <dir>` — scores an operator-assembled book on the quantities and parameters defined in APPENDIX B, importing S5B's estimators rather than reimplementing them. Non-zero exit on breach, append-only `book_scored.jsonl`, and every catalogue header states a book is UNSCORED until it has been run.

## EVIDENCE — the deliverable the redesign exists for

17. S5C walk-forward must emit a number, not nan (`master.py L1460-1463`) — per split apply VALID on the training segment, score members on test, ratio against the seeded null (89/80/81 qualifiers, rates 0.236/0.2375/0.2593). It certifies the catalogue's INCLUSION RULE, not any book; re-scoring a hand-assembled book per split is prohibited.
18. S5C arms must agree — the `persist` definition and the `n_traded` denominator identical in book and null arms, asserted at run time with abort on mismatch. Verify by opening the artifact, not by reading a diff: this was reported fixed once while the artifact stayed byte-identical.

## SPEED

19. Parallelise every stage; `workers` currently reaches 2 of 15. Dedup stays serial in ascending chunk order with a mandatory parity proof, and greedy is sequential BETWEEN admission steps but parallel WITHIN a step — score all marginal gains concurrently, then admit one.
20. Profile all fifteen stages on the real pool and report the timing table BEFORE parallelising. Progress line, heartbeat and ETA on every long stage.
21. Auto-write the run log into the output tree; clean output in console, pipe and redirect; warnings to the log, errors only on stderr.

## RUN

22. Full run from an empty tree — fourteen per-family books, every valid signal, no pruning. UNEVALUABLE rows stay in the catalogue with a `reason_code`.
23. Deliver one `.zip` of the complete `dot_master_discovery/` directory plus a sha256[:12] manifest. Windows: `.zip`, not `.tar.gz`.

24. Edit the spec, do not merely flag it: §D.0's '89.8% of missed episodes had no qualifying signal' is a gate artifact and must be restated as a gate decomposition or deleted. Give §D.2 strata, §C.3 step-5 tolerance, §12 and mantra §2 their reachable counterparts in the same pass.

## PARKED — do not re-propose

P1. 6- and 9-variable scanners by direct enumeration are impossible: C(117,6) = 3.13e9 (~33 years at F0's measured rate), C(117,9) = 8.26e12 (~87,000 years).
P2. Item 11 already yields observed 6s and 9s free — two triples on one bar is six variables agreeing, three triples is nine. Only combinations whose 3-subsets do not individually survive are unreachable, and those need greedy extension from surviving triples.
P3. Stability selection, PBO/CSCV, White's Reality Check, Hansen SPA and Romano-Wolf are DEFERRED, not dropped. Item 8's per-family BY pricing is this run's correction; these are the next tier if it proves insufficient.

## NOTES

- Family attribution comes from the `trigger` column, never name parsing.
- WHOLE CAKE = the reachable universe, not all price action. Measured: all episodes 100% -> eligible (ADX>=15, ticks>50, post-warmup) 60.5/60.8% -> D2D agrees 30.0/31.4%. The two exclusions are deliberate, measured decisions (gating PF 16.63 vs 2.25 OOS; D2D gated PF 5.14 vs long-ungated 1.53).
- EVIDENCE FOR 'not a vocabulary limit' IS THE SATURATION CURVE, not condition density: valid triples plateau at exactly 30.0/31.4 and do not move from 1,000 to 6,000 signals per direction, measured twice independently. Do NOT cite 'any of the 249 fires' — it fires on 100.00% of all 103,214 eligible bars and is tautological.
- BOOK-50 occupies 1.415%/0.762% of raw terrain = 4.72% (54/1,143) UP and 2.42% (28/1,155) DOWN of reachable. UNCLAIMED REACHABLE: 1,089 UP / 1,127 DOWN episodes. The constraint is signal count, not vocabulary, not gates, not search.
- RECORD CORRECTION: spec §D.0's '89.8% of missed episodes had no qualifying signal' was computed against raw terrain and is a GATE artifact, not signal absence. Restate as a gate decomposition or delete; §D.2 strata, §C.3 step-5 tolerance, §12 and mantra §2 reach figures all need reachable counterparts alongside the raw ones.
- Same-signal re-fire is RESOLVED at 1.8% / 15.4% / 26.1% for N=1 / N=5 / N=30, per-direction clustering. The 27.6% figure came from pooling directions and is wrong for this pipeline.
- Every change exercised on the real path at real scale before it is called done. Verifying that a function computes is not verifying that anything consumes it.

---

## PLAIN-ENGLISH SUMMARY — tick all before running master

### What this build fixes

- [ ] 1. The pipeline was reading old files from a leftover folder, data from a different dataset entirely. It now reads only from the run it just did.
- [ ] 2. A convenience feature chopped the pipeline's own output files into pieces, and the next step then read those broken pieces as if they were real. That feature is gone.
- [ ] 3. When counting how many signals agreed at once, the same signal firing repeatedly was counted as several. On the same bar it was exactly zero error; it grows to about 15% at a five-bar window and 26% at thirty.
- [ ] 4. The part meant to pick signals never actually ran, and it is not being repaired. It is being deleted: nothing chooses signals any more, you do.
- [ ] 5. A safety check on assembled books was wired permanently to "pass". It is removed, and those checks move into a separate tool you run against whatever book you build.
- [ ] 6. The test that proves the method works, not just that one book worked, never produced a number. It now does.
- [ ] 7. Only two of fifteen stages used more than one processor core. All stages get parallelised.
- [ ] 8. One exception: the step that strips near-duplicate signals stays sequential, or duplicates slip through. It ships with a proof that the parallel result matches the sequential one exactly.
- [ ] 9. Stages are timed first and parallelised second, so the speed-up is measured rather than guessed.
- [ ] 10. Every long stage prints progress, a heartbeat and an estimated finish time. You have twice had to guess whether a healthy run was stuck.
- [ ] 11. The run writes its own log into the output folder, and the console stays readable whether you watch it, pipe it or redirect it.

### What you are trying to achieve

13. See the whole reachable map, not occupy it. You decide what is worth trading; the tool measures and does not choose.
14. Keep solos, doubles and triples all firing, gated by conviction rather than thrown away. That gating lives in the EA; this build gives you the measurements to set it.
15. The catalogue is emitted from VALID, never from an argmax: no selected-book file is written in discover-fresh at all, and `sel.greedy_direction` is retained IN-MEMORY ONLY as item 12's dilution-curve admission loop. S8's discover-fresh path (`master.py L472`) currently reads `selected_book.csv` and scores it — disable that path with a message pointing at item 16, and leave S8's FROZEN path (`L468-470`) untouched so the committed book still scores from its ratified file.
16. Stop the short side being an afterthought. It was never weak, it just never had enough signals to stack.

### What you will see when it finishes

- [ ] 15. Fourteen books, one per family, every valid signal kept. Thousands per family and tens of thousands for the largest: last run 41,148 candidates cleared the filter, over 37,000 of them from a single family.
- [ ] 16. Every signal scored both ungated and gated, so you can see per signal whether the conviction filter helps it, rather than assuming it book-wide.
- [ ] 17. Every row carries a price: how many rows that good come up by chance alone. At fifty rows you can eyeball, at thirty-seven thousand you cannot.
- [ ] 18. Coverage against what is actually reachable. You hold 82 of 2,298 episodes today, 4.7% on the long side and 2.4% on the short.
- [ ] 19. The unclaimed list: 2,216 episodes you could legitimately trade and do not, each marked whether nothing searched there or nothing could express it.
- [ ] 20. A walk-forward number saying whether your inclusion rule beats chance.
- [ ] 21. A dilution curve in two versions, ranked two different ways, showing what happens to the triple edge as weaker signals join. The gap between the two curves is the overfit estimate.
- [ ] 22. Reach stops near 30% of the terrain, the market's clean directional moves, which is already a filtered set. That ceiling is the dead-bar filter plus the alternating bias, a trade you chose rather than a fault.
- [ ] 23. The other thirteen families add depth, timing and diversity inside that ceiling. They do not add reach.

### Hold onto this

- 2,216 unclaimed is what is visible, not what is tradable. Reachable only means the bar was eligible and the direction agreed, not that a signal exists there which survives the statistical bars. The count of valid triples per episode splits it, and the real number will land well below 2,216.
- Whether the same-bar edge survives as weaker signals join is unknown until the dilution curve exists. That number decides whether the 4x target is real.

---

## APPENDIX A — item 8, the pricing columns

For family `F` on segment `T`, with `N_F` trials and a MATCHED NULL — random signals drawn from the same post-hygiene vocabulary, fire-rate matched to `F`'s candidates, run through the IDENTICAL `VALID` predicate — every row carries:

    n_trials_family                    N_F, computed at run time, stamped
    null_valid_rate_family             fraction of matched-null signals passing VALID in F
    expected_valid_by_chance_family    N_F * null_valid_rate_family
    pf_null_p50_family / p90 / p99     PF quantiles among null signals that passed VALID in F
    pf_null_exceedance_pct             P(null PF >= this row's PF)
    EXPECTED_ROWS_AT_OR_ABOVE_THIS_PF  N_F * pf_null_exceedance_pct    <-- THE PRICING COLUMN
    q_value_BY_family                  Benjamini-Yekutieli q at family stratum

`EXPECTED_ROWS_AT_OR_ABOVE_THIS_PF` does the work: at this family's trial count, how many rows this good does chance alone produce? PF 12 beside an expected-count of 340 is not elite; PF 6 beside 0.4 is.

Benjamini-Yekutieli, NOT Benjamini-Hochberg: measured signed pairwise dependence on the live vocabulary is 49.6% positive / 50.4% negative across 29,161 pairs, so PRDS fails and BH is invalid. BY holds under arbitrary dependence.

Every catalogue CSV carries this header line:

> This catalogue contains `N_F` rows for this family. Reading it and selecting rows IS a search of size `N_F`. `EXPECTED_ROWS_AT_OR_ABOVE_THIS_PF` prices that search on every row. A row whose expected-count exceeds 1 is not evidence of an edge.

The matched null is generated on the same segment and shares its structure. That is deliberate — it is a null for THIS market, not a universal one.

## APPENDIX B — item 16, what `score_book.py` computes

Input: a CSV of `signal_id` plus optional `direction` for assertion. Nothing else. No option changes the verdict.

| quantity | parameters, stated at point of use |
|---|---|
| `TailDep` | `tau = 0.20`, `MIN_SHARED = 10`, RAW daily P&L, never `min(pnl,0)` |
| exclusion-bias diagnostics | `exclusion_bias_degeneracy_guarded`, `degenerate_excluded_pairs_k_lt3` |
| `FailConc` | worst single-day loss as a multiple of mean daily loss |
| `mCVaR_i` | per-signal marginal tail contribution, worst 5% of book days |
| absolute survival | worst modelled day against the FTMO ceiling, FULL population |
| union terrain coverage | pinned cell W15/K85/E75, per direction |
| same-bar depth ladder | distinct-signal basis, T=0 and the full tolerance curve |
| directional composition | LONG/SHORT split, reported and never targeted |

These are SET properties: they have no per-signal value and fabricating one would be worse than omitting it. The exposure this closes is union collapse — a 448-signal persistent union scored PF 1.82 against the curated 50-signal book's PF 6.40.

Three enforcement mechanisms, because a convention is what failed fourteen times: non-zero exit on breach with the breach named on stdout; append-only `book_scored.jsonl` carrying `book_sha256`, `input_sha`, `code_sha`, UTC timestamp and every constraint verdict; and the catalogue header declaring any assembled book UNSCORED until the tool has run on it.
