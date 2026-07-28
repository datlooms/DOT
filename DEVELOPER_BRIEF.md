# DEVELOPER — BUILD BRIEF

DOT master discovery corrections.

    git clone https://github.com/datlooms/DOT.git

**READ FIRST, IN FULL:**
`DEV_CHECKLIST.md` (repo root, 237 lines, sha256[:12] `9fb0e40253ef`, 24 items + Appendices A, B, C and D)

That file is the DO list. Build items 1-24 IN ORDER. This brief does not repeat it — it tells you what the checklist does not say. If the two disagree the checklist wins, EXCEPT where this brief marks an item **DECIDED — AUTHORISED DEVIATION**.

**The checklist is self-contained** — Appendix A defines item 8's pricing columns, Appendix B defines what item 16 computes, and **Appendix C defines `VALID`, the predicate the entire catalogue is emitted from, and Appendix D defines the local-percentile abort with its allowlist**. You do not need any other document to build.

Background reading only, never required: `CORRECTION_CHECKLIST.md`, `CATALOGUE_MEASUREMENT_SPEC.md`, `POST_SCAN_DEFECTS.md` (in `dot_master_discovery/`), `DOT_signal_discovery_mantra.md`.

---

## WHAT YOU ARE BUILDING

Read the PLAIN-ENGLISH SUMMARY section of `DEV_CHECKLIST.md` before writing code. It carries no item numbers and is not a build list — every "item N" reference in this brief means the numbered DO list, items 1-24.

You are NOT building a selection engine. You are building a **MEASURING INSTRUMENT**. It emits fourteen per-family catalogues holding EVERY valid signal, with the measurements needed to judge them, and the operator composes the final system himself.

**PRECISELY WHAT THAT MEANS FOR THE CHOOSER — read item 15 carefully:**

- Item 14 deletes `_sel_con` only. That is the CONSTRAINT STUB, not the chooser.
- `sel.greedy_direction` (`master.py L1185`) is NOT deleted. It feeds `chosen[d]` at L1187, which is written to `selected_book.csv` at L1198 — **and S8 reads that file at L472 and scores it.** That chain is what produced the 2-signal book.
- Item 15 governs it: **NO CATALOGUE MAY BE EMITTED FROM AN ARGMAX, AND NO SELECTED-BOOK FILE IS WRITTEN IN DISCOVER-FRESH AT ALL.** Do not rename or quarantine the artifact — do not write it. `greedy_direction` survives IN-MEMORY ONLY, as item 12's dilution-curve admission loop.
- **S8's discover-fresh path is disabled**, with a message pointing at item 16's `score_book.py`. Under a catalogue design S8 has nothing to score automatically: the catalogue is the deliverable, and scoring happens when the operator composes a book and runs it through item 16.
- **S8's FROZEN path (`L468-470`) is untouched.** It reads a ratified book file directly, not greedy output, and is how the committed book still scores.
- Item 19's greedy-parallelism clause therefore refers to the DILUTION-CURVE admission loop, not to a book-selection path.

If you find yourself adding a rank, a cap, a threshold nobody specified, or a "sensible default" that reduces what gets emitted — stop. That is the defect this rebuild exists to remove.

---

## ITEMS 4-12 ARE INDIVISIBLE. NINE ITEMS, ONE DELIVERY.

**THEY SHARE ONE CALL SITE. SHIPPING ANY ONE OF THEM ALONE PUTS A WRONG NUMBER IN A CATALOGUE THE OPERATOR WILL ACT ON.**

Item 4 sets the cluster count; items 11 and 12 consume it; items 5-10 share the per-signal row. Ship item 12 without item 4 and the dilution curve inherits an overstated depth. Ship item 5 without item 8 and coverage looks precise while the row is unpriced. All nine, or none.

---

## STANDING RULES

**SHIP EVERY TURN.** A patch is the minimum deliverable of every turn. Make decisions rather than asking — take the reversible option and flag it `DECIDED — REVERSIBLE`. If you run short, ship what works and label the rest `UNMEASURED`. **DO NOT CHOOSE WHICH OF THE OPERATOR'S ITEMS TO DROP.**

**VERIFIED IN ISOLATION IS NOT VERIFIED.** Fourteen times in this project a path was confirmed by a route other than the one production uses. Two you are touching directly: item 14's constraint gate was computed, written to CSV, audited across nine passes, and then passed a stub by the search that was supposed to use it. Items 17-18's walk-forward was REPORTED FIXED while its artifact stayed byte-identical to the broken version. **VERIFY BY OPENING THE ARTIFACT, NOT BY READING A DIFF.**

**WHAT "REAL SCALE" MEANS HERE — DECIDED**, so it does not collide with SHIP EVERY TURN:

- Every change is exercised on the FULL 177,251-row frame. Never a reduced frame.
- You are NOT required to re-run the 24-29h F0/F1 scan every turn. Exercise items 4-18 on a REAL candidate pool built from the ten fast families (~5,178 candidates, minutes), at full frame width.
- The DELIVERY must include one run reaching S9 on that pool, verbatim. The operator takes the first full-scope F0/F1 run.
- A reduced FRAME would be the isolation this rule warns against. A reduced POOL on a full frame is not — the code path and every row count are real.

**DELIVER ONE .ZIP** of the complete `dot_master_discovery/` directory plus a sha256[:12] manifest of every file. Not a patch, not loose files — the operator has no working tree to patch against. **WINDOWS: .zip, NOT .tar.gz.**

---

## CONSTRAINTS

Sacred five BYTE-LOCKED, verified before and after every change:

    dots_thresholds.py             518862bf19fb
    wf.py                          793e6e5f8d9a
    core.py                        6530e2508b17
    portfolio_simulation_engine.py bb498eb13ce3
    conviction.py                  27af7acee824

No scanner edited. No objective, no argmax, no quota, no floor, no target.
**THE OPERATOR PICKS N FROM THE TOLERANCE GRID — THE BUILD DOES NOT.**

All thresholds via `dots_thresholds`. No local percentile defining any object, event, cluster, episode or stratum. Every emitted table states its parameters at the point of use and carries a MARKET or BOOK label. LF endings, no code comments, complete files.

**DETERMINISM:** every ARTIFACT byte-identical across runs and across worker counts, and no wall-clock time in any artifact. **TIMINGS ARE NOT AN ARTIFACT** — they go to the run log (item 21). This is how the item-20 reinterpretation below stays compatible with the determinism assertion: the log is exempt, every CSV is not.

---

## THE THREE THINGS MOST LIKELY TO GO WRONG

**1. ITEM 4's PER-DIRECTION BASIS.** Tolerance runs built PER DIRECTION, never pooled. Per-direction gives 26.1% same-signal re-fire at N=30; pooled gives 27.6% and shifts every downstream count in items 11 and 12. This is the number the operator's whole decision rests on.

**2. THE GREEDY AXIS IN ITEM 19.** Sequential BETWEEN admission steps, parallel WITHIN a step — score all candidate marginal gains concurrently, then admit one. Parallelising across steps changes the result. Per item 15 this is the dilution-curve loop.

**3. THE DEDUP IN ITEM 19.** A global greedy pass against a running keep-set. It STAYS SERIAL in ascending chunk order. The parity proof against the serial result is mandatory.

---

## SEQUENCING — DECIDED, DO NOT RE-SEQUENCE

ONE DELIVERY, containing:

- Items **1-18 and 22-24** in full. These are what make the run valid and delivered.
- Item **19** parallelism applied ONLY where independence is structurally certain and cost is already known: the F0 re-score (5 hours measured, 19,757 independent re-scores), the S5C null (~1,950 sims), S7's six portfolio scores, S3B's four D2D variants, and the per-family scans. Dedup serial, parity proof mandatory.
- Item **20 — DECIDED, AUTHORISED DEVIATION FROM THE CHECKLIST:** instrument every stage so the RUN ITSELF emits the timing table into the run log. Do NOT treat profiling as a separate blocking pass. Do not revert this under the "checklist wins" rule.
- Item **21** progress, heartbeat, ETA, logging and clean console ships with it.

**WHY:** item 20 as written says profile before parallelising, and profiling needs a real run at real scale — taken literally it costs the operator two full runs. Instrumenting gives the table as an output of the run he is about to do. Speculative parallelism on stages of unknown cost is still forbidden; the next turn parallelises whatever the table exposes.

**IF BUDGET FORCES A CUT:** cut item 19's SCOPE — parallelise fewer stages. **NEVER cut items 1-18.** A valid slow run beats a fast invalid one, and a 29-hour run he must repeat is worse than either.

---

## REPORT BACK

- The `.zip` and its sha256[:12] manifest.
- Verbatim console output of a run reaching S9.
- **CATALOGUE ROW COUNT PER FAMILY.** This determines whether the operator can read the output at all — 37,000 rows and he needs the pricing column, 400 and the framing changes.
- **THE COVERAGE ACHIEVED, per direction, against BOTH denominators.** Reachable-denominated is the primary figure (incumbent 4.72% UP / 2.42% DOWN, ceiling 100%); terrain-denominated alongside. **30.0% UP / 31.4% DOWN of terrain is not a prediction — it IS the reachable set** (1,143/3,816 = 29.95%, 1,155/3,674 = 31.44%). Report the terrain figure and read it BY CAUSE, not by direction:
  - **ABOVE the plateau** — arithmetically impossible while the terrain and reachable definitions hold. It means a definitional or measurement bug in one of those denominators. Investigate that before reading anything else in the run.
  - **AT the plateau** — expected. The catalogue has saturated the reachable set.
  - **MATERIALLY BELOW** — three possible causes, and they are not interchangeable. **Report WHICH.** (a) `VALID` rejects more than the proxy admitted — the proxy required 30 qualifying fires, `VALID` requires 30 EXECUTED trades plus V2-V4, and executed <= qualifying. (b) The catalogue is too small to saturate. (c) **STRUCTURAL, and expected: F0's scanner carries an internal `MIN_PF` pre-gate (4.0 at `triple_convergence_and_d2ddir.py L25`, overridden to 2.0 in `run_f0_full.py`), unremovable under "no scanner edits", so F0's rows are a strict PF-filtered subset of VALID.** No other family scanner carries a PF filter. Cause (c) is not a defect and must not be reported as one.
- The stage timing table from the run log, with the fraction of total runtime that is genuinely concurrent stated explicitly.
- The walk-forward number, READ FROM THE ARTIFACT and quoted as it appears in the file.
- The dedup parity proof result.
- Confirmation the sacred five are unchanged and no scanner differs from HEAD.
- **ANYTHING NOT DONE, NAMED.** Do not silently defer.

---

*Numbering note: this brief supersedes any earlier reference placing the walk-forward at item 5, or parallelism at 18/20-22. The checklist has 24 items: correctness 1-18, speed 19-21, run and delivery 22-24.*
