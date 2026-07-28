# AUDITOR BRIEF — DOT master discovery rebuild

You are the Auditor. Your mandate is **blind re-derivation with a binding RATIFY / REJECT verdict.** You do not review upstream claims — you reconstruct from source and data and report what you find, including where the Supervisor or Quant are wrong. Both have been wrong this phase and both were corrected by measurement.

    git clone https://github.com/datlooms/DOT.git

Current state: HEAD `33e99ac`, `master.py` `3d45bd3b7f74`.
Sacred five, byte-locked, verify before and after anything:

    dots_thresholds.py             518862bf19fb
    wf.py                          793e6e5f8d9a
    core.py                        6530e2508b17
    portfolio_simulation_engine.py bb498eb13ce3
    conviction.py                  27af7acee824

**You are auditing two documents, not code.** Nothing has been built yet. Your verdict gates whether the Developer starts.

    DEV_CHECKLIST.md      108 lines, sha256[:12] 75f8ba2fafe2, 24 items
    DEVELOPER_BRIEF.md    118 lines, sha256[:12] 7badee7ded63

---

## PART 1 — WHY ANY OF THIS EXISTS

**The system.** DOT/equiDOT is an MQL4 Expert Advisor trading US30.cash on the 1-minute chart against an FTMO $100K account. Hard survival constraint: $2,500 daily loss ceiling. Signals fire on triple convergence — three variables simultaneously at distributional extremes — gated by a directional indicator called D2D. The committed book is 50 signals (48 triple-convergence + 2 sequential), assembled by hand over 17 months by a sole operator.

**The failure that started this.** BOOK-50 scored PF 6.40 on the data it was built on. On first contact with genuinely unseen data (25 Jun – 21 Jul) it scored **PF 2.19**. Profitable, survival intact, but materially degraded.

**The diagnosis, and it is the whole reason for the rebuild.** The cause was not the book. Every validation the project had ever run — blind audit, out-of-sample hold-out, six-fold walk-forward, decorrelation, null tests — sat **inside the window the book was built on**. The book was validated. **The selection process never was.** "Real in-window" and "persists across regimes" are different claims, and only the second was ever the goal.

**What the post-mortem found.** Concurrence — how many distinct signals fire on the same bar — is the dominant persistence axis, and it had been dismissed for months. Measured on the corrected frame, same-bar distinct signals:

| depth | trades | WR | PF | worst day | losses |
|---|---|---|---|---|---|
| 1 (solo) | 1,199 | 88.7% | 3.18 | −$574.0 | 136 |
| 2 | 974 | 91.0% | 5.27 | −$365.8 | 88 |
| 3+ | 512 | 97.3% | 35.11 | −$292.0 | 14 |
| 5+ | 160 | 100.0% | 999 | +$59.5 | 0 |

Monotonic on every axis. At 5+ there is no losing trade in 160. The June–July degradation was concentrated in the **solo** layer; triples never flinched.

---

## PART 2 — WHAT HAPPENED WHEN THE PIPELINE WAS FINALLY RUN

**Fourteen scanner families exist (F0–F13). Only two had ever been run.** The first full-scope scan took ~24–29 hours and produced 483,753 candidates: F0 19,757 from 260,130 triples, F1 439,061 from 1,713,630, and **5,178 from ten families that had never been run once in the project's history.** They were not empty.

**Eleven defects surfaced.** The ones that matter to you:

1. **The selection layer had never been called.** `_assemble_fresh_book()` was a hardcoded top-50 sort by worst-day, terminated by `if len(rows) >= 50: break`. Every artifact attributed to "selection" was scored against the incumbent book. The layer had been built, audited across nine passes, and ratified — and never once run against a candidate pool.
2. **The constraint gate was a stub.** FailConc, TailDep, mCVaR and absolute survival were computed, written to CSV, audited nine times, and then passed `def _sel_con(d, ss): return True, ''` by the search meant to use them. **This is the fourteenth instance of the project's defining failure mode.**
3. **The walk-forward never produced a number.** Its artifact still read "S3 discovery has never run" in a run that had just selected from 41,148 candidates. It was REPORTED FIXED once while the artifact stayed byte-identical.
4. **A packaging convenience destroyed the pipeline's own inputs.** S9's `split_tree()` shredded `discovery_master.csv` into 17 parts and deleted the originals; S4's glob then matched the headerless parts and `pd.read_csv` fused a data row onto the header. `candidates.csv` came back 46,245 rows against a reported 41,148, with 6,938 duplicates.
5. **Legacy directory fallbacks** made S3B read stale files from a different dataset entirely.
6. **The objective divided by signal count**, making its argmax minimal by construction. When selection was finally wired in properly, it returned **two signals**.

**The recurring pattern, named because you will be checking for a fifteenth instance.** Fourteen times a code path was verified by a route other than the one production uses: a function checked by direct call, a frame binding verified in the parent process only, an unreachable function region, a duplicate definition, a hardcoded `False`, a UTF-8 fix that worked on console but not pipe, an auto-split never tested against a run that had to read its own output. **Verifying that a function computes is not verifying that anything consumes it.**

**The operator has deleted his entire working directory.** No cached scan, no markers, no artifacts. He runs the whole pipeline from scratch, overnight, on whatever ships.

---

## PART 3 — THE DESIGN CHANGE

**The selection layer stops choosing. It becomes a catalogue.**

Fourteen books, one per family, containing **every valid signal** — no top-N, no cap, no argmax. The operator reads all fourteen and composes the final system himself.

His reasoning, and it is grounded: the committed book was never selected by an objective — he assembled it by hand, and the finding that mattered (same-bar 3+) was noticed *after the fact* by looking at the data. Every automated selection attempt has produced something worse than his hand-assembly. The top-50 sort gave PF 3.13 against the incumbent's 4.81; the wired search gave two signals.

`VALID` is a **measurability-and-survival predicate, not a quality predicate** — sufficiency, survival against the FTMO ceiling, measurability, regime-evaluability. No PF bar, no WR bar; those are columns. Rows that cannot be measured stay in the catalogue with a `reason_code`, because dropping them makes "catalogued nothing" and "could not be measured" look identical.

---

## PART 4 — WHAT THE OPERATOR IS TRYING TO ACHIEVE

Stated in his own terms, because your verdict should be measured against his aims and not an inferred version of them:

- **"I want to see the whole cake."** Visibility, not occupancy. He wants the map of what is legitimately reachable, then decides himself what to trade and what to leave.
- **Whole cake ≡ the reachable universe, not 100% of price action.** Measured decomposition, corrected frame, pinned cell W15/K85/E75 (7,490 episodes, 3,816 UP / 3,674 DOWN):

| stage | UP | DOWN |
|---|---|---|
| all episodes | 100% | 100% |
| survive eligibility (ADX≥15, ticks>50, post-warmup) | 60.5% | 60.8% |
| **+ D2D agrees with episode direction** | **30.0%** | **31.4%** |

  The ~39% and ~30% exclusions are **deliberate, measured decisions**, not defects: the gating trade is PF 16.63 vs 2.25 out of sample; D2D gated is PF 5.14 vs long-ungated 1.53.
- **BOOK-50 occupies 4.72% (54/1,143) UP and 2.42% (28/1,155) DOWN of reachable.** 2,216 reachable episodes are unclaimed. The constraint is signal count — not vocabulary, not gates, not search.
- **He wants convergence kept whole:** solos, doubles and triples all firing, gated by conviction rather than filtered out. Solos did not need deleting, they needed a permission filter — Hurst p90 plus ticks for solos, Hurst p90 for doubles, depth 3+ free. Out of sample that gating held 96% of the money at 28% of the worst day and 8% of the losses.
- **He wants the short side to stop being an afterthought.** 37 LONG / 13 SHORT is a selection artifact, not terrain: the candidate pool was 51.3% / 48.7%, and terrain is near-symmetric. 13 signals cannot stack into same-bar depth the way 37 can — 70% of book shorts fire solo against 36% of longs.
- **His target: 4× the same-bar 3+ population** (~2,020 trades) with the stats intact. Superlinear scaling — three-way coincidence goes as C(n,3) — puts that at roughly 72–100 signals, not 192.
- **No quota, floor or target of any kind.** He overruled a proposed minimum short-signal count on doctrine grounds: a floor is a pre-set target, and one calibrated on the incumbent's 13 shorts is calibrated on an artifact of the funnel being replaced.

---

## PART 5 — STANDING DOCTRINE

`DOT_signal_discovery_mantra.md` (repo root, 305 lines) is **doctrine, not a finding, and is not superseded by measurement.** Read it in full. It binds your verdict as much as any deliverable. Five rules:

1. **Measure the cake, not the bite.** Every finding labelled MARKET (price-only or full-population) or BOOK. The recurring project failure is measuring the market *through* the book and reporting the reading as a property of the instrument.
2. **Include, then let the evidence sort.** Nothing removed on a single measurement. Gates are state columns, never row filters.
3. **No pre-set targets.** Composition is an output to be reported, never an input to be constrained.
4. **Depth is the unit of quality, not the signal.**
5. **A negative conclusion carries the same burden of proof as a positive one.** This binds your verdict: a manufactured concern is as much a breach as a missed defect.

Plus the standing construction: **when a finding depends on a filter, threshold or restriction, the filter is part of the finding.**

---

## PART 6 — WHAT YOU ARE AUDITING

Read both documents in full, then re-derive rather than review.

**1. Every load-bearing figure, against source and data.** Both prior seats have shipped figures that did not reproduce. Known corrections already applied, which you should confirm independently: the same-bar 3+ headline moved from 505/PF 53.70 (pre-clock-fix) to **512/PF 35.11** on the corrected frame; all-F0 PF 35.72 against MIXED F0+F1 29.35; reachable 4.72%/2.42%; unclaimed 1,089 UP / 1,127 DOWN; same-signal re-fire 1.8% / 15.4% / 26.1% at N=1/5/30 **per direction** (pooled gives 27.6% and is wrong for this pipeline).

**2. Every line number and function name in both documents.** Stale references have reached the Developer twice this phase. `master.py` is `3d45bd3b7f74`; documents citing other shas are describing a different file.

**3. The indivisible block, items 4–12.** Nine items sharing one call site. Confirm the dependency is real and that shipping any subset produces a wrong number in a catalogue the operator will act on — or say it is overstated.

**4. Item 15 specifically, and the branch it rests on.** It claims: `sel.greedy_direction` (L1185) feeds `chosen[d]` (L1187), written to `selected_book.csv` (L1198), **which S8 reads at L472 and scores**; that disabling S8's discover-fresh arm leaves the FROZEN arm (L467–470) untouched; and that no selected-book file should be written at all rather than renamed or quarantined. Verify the branch exists as described and that the cut is clean. An earlier version of this item labelled the artifact DIAGNOSTIC ONLY while S8 still consumed it — a contradiction caught by the Quant.

**5. The fifteenth instance.** Assume one exists and go looking. The catalogue is new code touching a region that has produced four defects in two days.

**6. Causality and parity.** No local percentile may define any object, event, cluster, episode or stratum. Item 5 requires an abort if any episode threshold bypasses mechanism D. Confirm the terrain thresholds route through `dots_thresholds`, and that nothing in the design introduces look-ahead.

**7. Doctrine compliance.** Does anything get removed on a single measurement? Does any pre-set target, floor or quota survive? Is every figure carrying its parameters at point of use? Is every finding labelled MARKET or BOOK?

**8. What is missing.** Gaps matter as much as errors. Name anything a Developer would have to invent because both documents are silent on it.

---

## PART 7 — TWO ITEMS CARRIED FORWARD

**A. The 30.0/31.4 plateau is an upper bound, not a target.** It was measured on random VALID-passing triples as a proxy. Real S5-filtered candidates additionally cleared PF≥2, so they are a strict subset — **the true figure should land at or below it.** A materially higher number is not the catalogue outperforming; it means the terrain or the reachable denominator moved, and it needs the same scrutiny as a shortfall. Confirm this reasoning holds, or refute it.

**B. Item 20 is an authorised deviation.** The checklist says profile before parallelising; the brief overrides it — instrument every stage so the run itself emits the timing table to the run log, because profiling requires a real run and taking the item literally costs the operator two. Timings go to the **log**, not an artifact, so the determinism assertion (every artifact byte-identical across runs and worker counts, no wall-clock in any artifact) still holds. Confirm the override is sound and the determinism carve-out is coherent.

---

## DELIVER

**RATIFY**, or **RATIFY WITH AMENDMENTS** listing each precisely, or **REJECT** with the specific defect.

Report every check and its result, including passes. Where a figure does not reproduce, give your number and the construction that produced it. Where you cannot measure something, say what would settle it.

**Do not run S3 discovery.** Measurement on the existing frame and the committed book only.

The operator commits a full night to the run that follows this pipeline. An unnamed gap costs him the night; a manufactured one wastes a seat. Rule 5 binds the verdict.

Report and hold.
