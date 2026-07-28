# DEV_CHECKLIST

Sacred five byte-locked: 518862bf19fb / 793e6e5f8d9a / 6530e2508b17 / bb498eb13ce3 / 27af7acee824.
No scanner edits. No objective, no argmax, no quota, no floor. Operator picks N from the grid.
Sources: CORRECTION_CHECKLIST.md + CATALOGUE_MEASUREMENT_SPEC.md. This file supersedes both as the DO list.

BASELINE CORRECTION — apply wherever it appears: same-bar 3+ is 512 trades / PF 35.11, not 505 / PF 53.70. Mixed-family penalty is all-F0 PF 35.72 vs MIXED 29.35 — an 18% cost, not 49%.

## BROKEN — nothing runs correctly until these land

1. Paths — every read/write resolves inside `--out`. Delete legacy fallbacks at `master.py L858-860`, make `_find_outputs` (`family_evidence.py L73-77`) read the run tree only, point F13's `RESULTS_DIR` (`orchestrator L48`) at the run tree.
2. Glob — S4 excludes `*_part*.csv`, and drop the `F1_part*.csv` pattern at `family_evidence.py L53`.
3. Split — delete `split_tree()` (`master.py L111-122`) and `--chunk-mb`. Every artifact writes as one file.

## COUNTING — these five ship together or not at all (4-8 INDIVISIBLE)

4. Count distinct `signal_idx` within a tolerance run, not entry rows (`selection.py L272-282`). Same-signal re-fire is 1.8% at N=1, 15.4% at N=5, 26.1% at N=30, so the error scales with tolerance.
5. Per-signal terrain coverage, pinned cell W15/K85/E75 named in every column. Abort the run if any episode threshold is a local percentile rather than mechanism D.
6. Per-signal multiple-testing price per spec §5 — `n_trials_family`, `null_valid_rate_family`, `pf_null_exceedance_pct`, `q_value_BY_family`, `EXPECTED_ROWS_AT_OR_ABOVE_THIS_PF`. N computed per family at run time, never a literal.
7. Pool-level same-bar cohort table — family composition of each bar reported as a curve over depth, not fixed at 3. Counts only, no P&L: depth-3 has no discriminating power at pool scale and P&L needs a book.
8. `dilution_curve.csv` — admit catalogue signals best-first and re-score the same-bar 3+ population at each step. Columns: rank, signal_id, family, direction, n_admitted, bars_3plus, trades_3plus, WR, PF, worst_day, losses, net, median_depth; repeat at 4+ and 5+.

## SELECTION — stop it choosing

9. Remove `/n_signals_d` from any objective (`selection.py L290`). Keep it as a reported column that never gates inclusion.
10. Delete the `_sel_con` stub (`master.py L1174-1175` — a `def`, not a lambda). Constraint machinery moves to item 11.
11. `score_book.py --book <csv> --data <frame> --out <dir>` — scores an operator-assembled book on FailConc, TailDep, mCVaR, survival, union coverage, same-bar ladder and L/S split. Non-zero exit on breach, append-only `book_scored.jsonl`, and every catalogue header states a book is UNSCORED until it has been run.

## SPEED

12. Parallelise every stage; `workers` currently reaches 2 of 15. Dedup stays serial in ascending chunk order with a mandatory parity proof against the serial result.
13. Progress line, heartbeat and ETA on every long stage.
14. Auto-write the run log into the output tree; clean output in console, pipe and redirect; warnings to the log, errors only on stderr.

## RUN

15. Full run from an empty tree — fourteen per-family books, every valid signal, no pruning. UNEVALUABLE rows stay in the catalogue with a `reason_code`.
16. Deliver one `.zip` of the complete `dot_master_discovery/` directory plus a sha256[:12] manifest. Windows: `.zip`, not `.tar.gz`.

## PARKED — do not re-propose

P1. 6- and 9-variable scanners by direct enumeration are impossible: C(117,6) = 3.13e9 (~33 years at F0's measured rate), C(117,9) = 8.26e12 (~87,000 years).
P2. Item 7 already yields observed 6s and 9s free — two triples on one bar is six variables agreeing, three triples is nine. Only combinations whose 3-subsets do not individually survive are unreachable, and those need greedy extension from surviving triples.

## NOTES

- Family attribution comes from the `trigger` column, never name parsing.
- Every change exercised on the real path at real scale before it is called done. Verifying that a function computes is not verifying that anything consumes it.
