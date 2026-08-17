# equiDOT — Execution Sequence (current state → live)

*Updated 2026-07-02 (Step 12 LOCKED; discovery-pattern-map v4 adopted; new Step 13 = build the 12 family scanners; Family-0 script renamed). The full ordered path from the current state — threshold-unification EA built and compiled 0/0, the Step-6 candidate/precision EA window shipped, the sealed baseline re-built and parity-validated 88/88, A2.7 reconciliation PASS×4, Stage 5 CLOSED and the conditional D2D fork resolved (no recalibration — EA stays frozen), and the Step-10 Python-tool rekey RATIFIED (discovery + sim re-keyed to the sealed baseline + the 117 / 249 vocabulary; `wf.py` authored) — to a live, scaled FTMO deployment. The triple-convergence architecture and the unified threshold layer (mechanism D + structural) are locked; the signal SET is re-derived from scratch at Stage 8 on the sealed clean baseline. Step 11 (early execution-parity audit) is DEFERRED: the MT4 Strategy Tester cannot replicate live init (the EA seeds buffers forward from the run-start, whereas live starts pre-seeded with `Dots_InitBars`=6900 chart bars), and there is no finalised signal set or trade management to audit yet — parity is validated once, on the deployment build, at Step 17; the tester-init correction is recorded against the Stage 9 EA window (Step 16). Source of truth for build spec and mechanism map: `equiDOT_adaptive_thresholds_stage.md`; live status: `equiDOT_progress_and_rd_plan.md`; laws: `non_negotiables_*.txt`.*

**Discovery vocabulary: 117 candidates — 90 FEAT_ (hi/lo) + 27 binary/state/side (equality) — 249 scan conditions.**

---

- [x] **1. Ratify Stage 4.** Sealed clean baseline `equiDOT_recon171_part*` (150,599 × 171, Jan 19 → Jun 24); Auditor PASS; corrections recorded (epoch via `total_seconds()`, `ATR_1M` divisor everywhere, δ=−2, LLEMA deadzone → `ATR_1M`).

- [x] **2. Prep — DONE (no EA change).** `dots_thresholds.py` unified: all distributional candidates on mechanism D, VWAP_Z / OR_Position structural, A/B/C and the static-global path retired; validated bit-identical on the baseline.

- [x] **3. Non-negotiables rewrite (threshold-unification gate).** `non_negotiables_supervisor.txt` carries the unified layer (S.2), schema (S.8), feature system + 117 vocabulary (S.10), and A2.7 directive reconciliation. ⬜ **PRECONDITION CARRYOVER:** `non_negotiables_developer.txt` and `non_negotiables_auditor.txt` are NOT yet synced to 117 / A2.7 / 90 FEAT — bring both in sync before the next EA window (the gate is currently half-armed).

- [x] **4. Adaptive-threshold EA change (threshold unification) — BUILT, COMPILED 0/0.** 8 ratified passes; A/B/C + static-global retired; all distributional candidates on D; structural pair added; 21 candidates wired; `VWAP_Sigma_ATR` added as the 172nd export column; no exported value / state-Calc / rule change. Macro block relocated to the preamble to resolve the FEAT_* preprocessor forward reference.

- [x] **5. Re-export + parity validate — DONE.** Fresh 172-col export (`64_246_*`) + EA threshold dump captured; S.6 parity = 79/81 D features reproduce `dots_thresholds.py` exactly at 6dp over the seed window. The two failing ship-set features (`KAMA_Dist`, `Volume_Ratio_10`) were closed in step 6; final parity reached **88/88** in step 7 (the 7 Group-B adds expanded the D universe 81→88).

- [x] **6. Bundle the two parity fixes + the 7 continuous candidate adds (ONE EA window) — DONE, compiled 0/0.** Through Developer → Supervisor → Auditor → human-confirm → recompile, all complete. `DOTS_NUM_FEATURES` 83→90; export stays 172 columns. The `ADX_Value`/`VWAP_Sigma` export-precision gap was caught by A2.7 / VP-PRECISION before re-export and folded in (both now 6dp).
  - **`Volume_Ratio_10`** — align the live calc (`Calc_Dots_Derived_OnBar`) to the export window `i+1..i+10` (exclude the current bar). No re-discovery (export/baseline unchanged).
  - **`KAMA_Dist`** — export at 6dp (`DoubleToString(kamaDist, 6)`); precision now matches the EA's internal value.
  - **Group-B candidate adds (7):** register ADX_Value, Body_Size, Upper_Wick, Lower_Wick, TChan_A15, VWAP_Sigma, Volume as FEAT_ candidates (`DOTS_NUM_FEATURES` 83→90) — all already exported, so no Calc and no export-schema change; +7 `DotsGetFeatureValue` cases, +7 rolling buffers, +7 `dots_thresholds.py` routing rows.
  - Export stays **172 columns**; the only value change is the `KAMA_Dist` precision.

- [x] **7. Re-export + rebuild baseline + re-run parity — DONE, baseline SEALED, parity 88/88.** Re-exported (`64_256_*`, 78,624 rows, Apr 6 → Jun 25) + EA threshold dump (6,900 bars); `core.py` re-keyed to the 172-col export (171-wide stitch; oracle derives `VWAP_Sigma_ATR`); sealed baseline rebuilt (`equiDOT_recon171_step7_part*`, **152,983 × 171, Jan 19 → Jun 25**, strictly increasing, 0 dups, 0 NaN). **S.6 parity = 88/88 at 6dp** (both prior failures resolved; the 7 Group-B aligned bit-for-bit). The 5 KAMA_Dist triples re-validated on the fresh baseline. **`ADX_Value` recomputed at full precision in the Jan–Apr segment** (exact vs export: GATE 1 = 0.0, GATE 2 = 0.0; the 30 eligibility-boundary bars at 2dp 15.00 resolved to 0) — A2.7 absolute-precision election; one TARGETS addition, 15 existing recompute families byte-identical.

- [x] **8. Stage 5 — D2D band-calibration study — DONE** (analysis-only, no EA change). Verdict: **no recalibration warranted.** D2D has no standalone scale-free directional edge (known, confirmed); convergence *leads* D2D by 10–20 bars, so the mandatory 4th gate (`equiDOT.cs` L4818–4819) is a **latency-vs-protection trade-off** — it blocks the early new-direction entries convergence already sees AND the old-direction traps that fire at the flip. The posture engine captures the right dimension (volatility → convergence density, ρ≈0.50) but no posture-input change improves convergence *success* (max |ρ|=0.127; per-entry OOS AUC 0.520), and no band-constant change creates edge. confirmN reframed: raising it worsens gate latency → not a pure survival lever. Gate-variable optimization is unmeasurable on benchmark triples (no edge either direction) and is folded into Stage 8 co-selection. Full record in `equiDOT_progress_and_rd_plan.md` (2026-06-26).

- [x] **9. FORK — D2D recalibration (conditional) — RESOLVED: does NOT fire.** Stage 5 found no band-constant change that improves flip quality and no posture-input change that improves convergence success; the latency-vs-protection question is measurable only with production signals. No human-authorized, behavior-changing edit to any sacred element is warranted. **EA stays FROZEN; the sealed step-7 baseline is the dataset for Stages 8–10.** Gate-variable selection + confirmN are evaluated jointly with signal derivation at Stage 8; ATR-conditional sizing → Stage 9. This step collapses into Stage 8 co-selection.

- [x] **10. Rekey the discovery/sim/wf tools — DONE, Auditor RATIFIED.** `full_766K_convergence_discovery.py` and `portfolio_simulation_engine.py` re-keyed off the absent `65K.csv` / `DOTS_60_final_signals.csv` onto the sealed 8-part baseline (152,983 × 171); all 90 FEAT_ routed through `dots_thresholds` (mechanism D + structural), **zero** independent threshold computation. The **27 equality-condition type** added (`==value`, post-warmup scannable enumeration → 69 / **249 scan conditions, exact to S.10**; the 3 warmup-only dead values excluded). `wf.py` authored (6 monthly folds, thresholds computed once and sliced — no refit; survival-first scoring on the authoritative engine; `DAILY_LOSS_CEILING_USD`=2500 / `USD_PER_POINT_PER_LOT`=100 / `LOT_SIZE` configurable). S.7 TM untouched (verified vs frozen EA L1710–1714 / L4835 / L8934); scanner TM == engine TM (8/8 parity). 76-benchmark run-proof (engine 1,628 / PF 1.54 / WR 80.0% / 0 SL-bug; wf SURVIVAL REJECT at 1 lot). Full record in `equiDOT_progress_and_rd_plan.md` (2026-06-27).

- [~] **11. Stage 7 — execution-parity audit — DEFERRED to Step 16 (deployment build).** ATTEMPTED 2026-07-02 on a separate FTMO MT4 install with the captured `US30.cash1.hst` (6 Apr → 1 Jul, 84,166 M1 bars). BLOCKED at EA init: `initialization failed (1) — Not enough bars on chart to initialise EA. Bars: 1002`. ROOT CAUSE (verified in source): the `OnInit` guard `if(current_bars<ClusteringLookback+trainingBars)` (`equiDOT.cs` L356; requirement 1000+100=1100) reads `iBars()` which in the tester returns only the bars modelled up to the run-start (~1002), not the full file — and it lacks the `!IsTesting()` exemption that the calc-path guard at L1125 already has. STRUCTURAL: even with the guard fixed, the tester seeds buffers forward from the run-start (live starts pre-seeded with 6900 chart bars), so only the post-saturation portion would be parity-valid. Compounding: there is no finalised signal set (Stage 8) or trade management (Stage 9) to audit yet — the audit had nothing valid to compare against. DECISION: fold the early audit into the existing Step-17 execution-parity re-verification on the deployment build; record the tester-init correction against the Stage 9 EA window (Step 16). See `equiDOT_progress_and_rd_plan.md` (2026-07-02).

- [x] **12. Lock the walk-forward folds — DONE, LOCKED 2026-07-02 (Auditor + Manager sign-off, unanimous).** 6 monthly folds keyed Time[:7] (Jan(19-31)/Feb/Mar/Apr/May/Jun(1-25)); survival-first scoring (worst-day vs -$2,500 ceiling is the sole verdict gate; persistence + spread-stress reported, non-overriding); thresholds computed once on the full series and sliced per fold (no refit, no look-ahead); warmup floor 6900 covers the deepest-warmup variable (MultiDay_Slope first-valid bar 2759, WeeklyOpen bar 5). Run-proof reproduced: 1,628 trades, PF 1.54, WR 80.0%, 6/6 profitable folds, worst-day -$47,700, 33 hard-stop days -> SURVIVAL REJECT (the 76-benchmark's expected outcome at 1 lot — the gate works). LOT_SIZE / ceiling / spread-stress remain configurable post-Stage-8 by design (survival gate holds at any lot). See rd_plan (2026-07-02).

- [x] **13. Build + ratify the 12 family discovery scanners — DONE 2026-07-02.** All ratified (Developer → Supervisor → Auditor). F0 `triple_convergence_and_d2ddir.py` (F10 convergence-density FUSED in as a `density` dimension — degenerate over the raw pool, meaningful only over the selected set; standalone `convergence_density.py` retired). F1 `sequential_temporal` · F2 `state_transition` · F3 `conditional_interaction` · F4 `divergence_nonconfirm` · F5 `persistence_autocorr` · F6 `threshold_crossing` · F7 `mean_reversion` · F8 `cross_variable_structure` · F9 `session_temporal` · F11 `rolling_leadlag`. Hazards contained & verified: D2D column-reconstruction (F4/F7, gate-only in engine), forward-return quarantine (F5), smuggled-percentile (F8/F11), blind-sweep (F9), look-ahead (F11, truncation-test clean). D2D is a per-family search dimension (confirm/invert/exempt) per the map.

- [x] **14. Stage 8 — definitive discovery — DONE 2026-07-09.** Full 12-family search on the sealed baseline. Outputs: `discovery_master.csv` = 440,057 candidates, 14-col schema, persistence-first sorted, no rows dropped. Per-family: F0 51,311 (1,788 at 6/6 folds) | F1 384,553 (11,772 at 6/6; 1,903 of those >=100 trades) | F3 3,658 | F9 255 | F11 103 | F4 66 | F2 47 | F7 28 | F5 16 | F6 10 | F8 10. F1 rebuilt to ordered pairs (238 scorable conditions x LAGS 1..15 x 2 dir = 1,699,320) and run via `run_f1_parallel.py` (8 workers, 24h54m, parity-proven identical to serial). F0 density sweep run over the 51,311-survivor pool: DEGENERATE as expected (min co-fire 7 long / 2 short, so k=1..10 barely filters; PF 1.04, WR 75.3% long / 76.0% short at 1 lot with all signals firing). Density must be re-run over the SELECTED set, where it discriminates.

- [x] **14a. QRA COMPLETE + THE DIAMOND FOUND — 2026-07-11.** Steps 15/15a/15b (F12 concurrence profiler, all amendments) built, ratified, and run; step 16 (QRA selection) complete; step 17 OOS validation substantially done. Key outcomes:
  - **F12 raw-depth null:** undifferentiated depth does NOT beat a circular-shift null on agg_pf — a raw count is the wrong measurement. Composition DESCRIBES direction (all state-variable Cohen's d < 0.11 vs direction-matched baseline) but does NOT select winners; the '>=80%-present spine' is tautological with direction (lift ~1.0). Winning triplets do NOT sit in deeper stacks than baseline (median depth 33 vs 34). Depth-as-edge and ambient-composition-as-edge are both CLOSED.
  - **The edge is specific-combination identity, not count.** Persistence elite = F0 + F1 only (F2-F11 contribute ~1 signal at folds_plus=6 & WR>=90%). Long and short use DISTINCT vocabularies (buy-the-pullback vs flow-rollover) — confirms the mirror finding (variables regroup by direction).
  - **THE DIAMOND (leading engine candidate):** individual elite signals score PF 5-17 but naive union collapses to PF ~1.9 because losses concentrate on shared tail-days (mostly March bear fold). **Loss-decorrelation selection** — choosing a subset whose losing days don't overlap — recovers the PF class as a REAL portfolio. Recommended book L20+S15: 35 triples, WR 92.7%, PF 6.22, 6/6 folds, net ~$38,158/6mo @ 1 lot, worst-day -$77.80. **OOS-validated** (select Jan-Apr, measure May-Jun): WR 90.2%, PF 4.61, worst-day -$149.6 — generalises, not in-sample luck. Full roster + composition in rd_plan 2026-07-11.
  - **100x UNITS BUG corrected (wf.py USD_PER_POINT_PER_LOT 100.0 -> 1.0, $1/pt/lot FTMO spec).** Every prior worst_day_usd / P&L figure was inflated 100x. Survival was NEVER the binding constraint it appeared to be — the diamond risks -$77.80 on its worst day (32x headroom under the -$2,500 gate). Lot-sizing flips from survival constraint to profit-scaling lever (~16 lots OOS-anchored). WR/PF/folds/trade-counts were never affected. Prior worst-day dismissals (depth-K, build-up entry, scale-in) are re-openable at true scale, though the diamond leads on PF/persistence regardless.

- [x] **15. Build + run `concurrence_profiler.py` — FAMILY 12: Raw Variable Concurrence (PRE-QUANT, BLOCKING).** *The measurement this entire rebuild exists for, dismissed twice, still never made.*
  - **First dismissal:** the original 766K -> 4,773 -> 76 pipeline **deduplicated the overlap away** and never scored concurrence. Wiping the 76 and rebuilding the variables/history was done specifically to fix this.
  - **Second dismissal (2026-07-02, now CORRECTED):** F10 was ruled "degenerate over the raw 249 pool" because bands k=1..10 all selected the same bars — and folded into F0. **The bands were the error.** Measured per-bar direction-aligned co-fire distribution over the 249 pool: LONG min 8 / median 27 / p95 48 / max 81; SHORT min 2 / median 23 / max 61. Every tested band sits at/below the 5th percentile; `k>=10` captures 100.0% of bars. The discriminating range is ~k=20..60 (`k>=30` -> 40.8% of bars; `k>=50` -> 4.3%) and was **never entered**. Developer, Supervisor, and Auditor all missed it. See pattern map v7.
  - **F12 unfreezes the search.** The triplet (k=3) was an arbitrary imposition. F12 counts how many of the **249 conditions** are at their extremes on a bar, direction-aligned — no triplet, no signal list, no survivor filter. Primary pool = all 249 (117 long-aligned / 110 short-aligned); secondary comparison view restricted to F0-survivor conditions.
  - **Rationale.** A frozen list of triplets encodes the regime that produced it (F0's 6/6 >=30-trade survivors: 604 LONG / 98 SHORT — the bull tape's fingerprint). A RULE encodes market behaviour, which is what carries into an unseen future. **Persistence is the priority; the frozen list is the threat to it.** Production precedent: `Dots_MinConcurrent=2` @ volume>=50 already outperforms a solo signal @ volume>=300.
  - **Measures (MEASUREMENT ONLY — nothing pruned, nothing concluded):** (1) per-bar **depth**, raw and D2D-agreeing, k swept across the real ~15..65 range; (2) concurrence **events** — onset, build rate, peak depth, duration at depth, decay; (3) **entry order** — which conditions join first (leading indicators; ties to F1's lead-lag map); (4) **outcome map** `peak depth x duration -> WR/PF/forward return` scored through the ratified engine + locked wf 6-fold; (5) **composition** — cluster conditions by co-occurrence, do NOT hand-label (the 8 "market structures" in DOTS_76_signal_dictionary.xlsx classify SIGNAL types, were never a measured per-bar regime, and are not inherited); (6) **regime** — cluster BARS on state variables, k swept 2..12, chosen by silhouette/BIC, validated for recurrence across all 6 folds; (7) **reversion gate** — when composition is reversion-dominated and depth high while D2D still reads the old direction, test counter-D2D entries across all folds and whether those bars precede a flip within N bars. Earned, never crafted.
  - **Why it settles curve-fitting:** nothing is selected. Free parameters are `k` and `duration` — two integers, validated across six folds. Monotone improvement in ALL six folds = structural. Holds in three, reverses in three = noise, stated plainly.
  - Contract: oracle-only thresholds; `engine.run_portfolio` sole trade path; wf survival-first; zero TM reconstruction; D2D searched (confirm/invert/exempt). Developer -> Supervisor -> Auditor.

- [x] **15a. AMENDMENT 2 to `concurrence_profiler.py` (BLOCKING — the export must be self-sufficient).**
  **Trigger (measured 2026-07-10 from the existing F0 export; F0 enumerated every feature-triple across hi/lo products and BOTH directions, so every mirror was tested):** VARIABLES shared between LONG and SHORT survivor sets **97.4%**; mirrored CONDITIONS appearing anywhere on the other side **94.7%** (214/226); mirrored TRIPLETS that survive **3.3%** (1,712/51,311); among the 1,788 six-of-six persisters only **2.5%** (44).
  **The variables mirror almost perfectly. The combinations do not — a 28x gap.** The three variables that cluster together going long do not cluster together going short. Each variable still measures something real in both directions; it groups with different partners. **The triplet is the wrong unit.** A frozen list of 702 triplets is 702 photographs of how variables clustered in one bull tape.
  **Consequence:** the central question is no longer only *does depth persist?* — it is **how do variables cluster freely, and does that clustering change by regime?** The ratified script CANNOT answer this: stage 5 computes ONE co-occurrence clustering per direction over ALL bars, and `run()` calls it BEFORE stage 6, so it has no regime labels. A single global clustering averages the regimes together and hides the phenomenon.
  **Required:** (A) reorder — regimes before composition; (B) stage 5 clusters per (direction x regime): condition_membership, **variable_membership (hi/lo collapsed — does V group with W in regime 1 and X in regime 2?)**, top co-occurring pairs, **cross-regime stability**, **per-regime deep-stack composition**; thin cells emitted with `n_bars` + `skipped`, never hidden. (C) NEW stage 5b **category-depth -> outcome** — the engine concept scored (enough variables WITHIN a category at extremes + enough stacking = trade); marginal per-cluster sweeps + dominant-category variant; full per-fold columns. (D) NEW stage 8 **null baseline** by CIRCULAR SHIFT of the depth arrays (preserves autocorrelation/event shape, destroys alignment with price; an i.i.d. shuffle is an unfairly weak null); emits an empirical p-value per cell vs the observed stage-4 statistic. (E) **regime-tag every emitted row** (depth bars, events, entry order, D2D flips) so any table slices by regime post-hoc with no re-run; extend stage-6 `regime_outcome` to sweep k across the stage-4 range within each causal regime, confirm and invert.
  **CAUSALITY — repeat-defect guard.** Cluster membership in stage 5b decides WHICH CONDITIONS COUNT toward category depth, so it GATES ENTRIES and is a signal input. It MUST be fit on the leading burn-in window (fold 1) and assigned forward-only, burn-in fold excluded from scoring — identical treatment to the stage-6 regime labels the Auditor forced. **A full-sample clustering that gates a trade is the exact defect rejected on 2026-07-02.** Descriptive clustering (stage 5) gates nothing and may use full-sample labels, flagged `causal=False`.
  **Unchanged:** stage 4 (RATIFIED — K=15..81, 3,216 configs, per-fold columns from `wf.fold_metrics`), primary-view purity (249 -> 117 long / 110 short; no triplet, no signal list, no survivor filter), oracle-only, zero TM reconstruction, D2D snapshot -> feed -> restore-in-finally, parallel + live progress, proof mode with [PARITY] PASS, measures-never-selects.

- [x] **15b. Two follow-on analyses (require step 15's output; category-depth has MOVED INTO the script as stage 5b).**
  - **(ii) MIRROR-STATE TEST across ALL convergences.** Every variable has two states (upper/lower extreme), so every triplet has two states and every triplet + D2D has a symmetric partner (`hi/hi/hi + D2D-up LONG` <-> `lo/lo/lo + D2D-down SHORT`; equality `==v` -> `==-v`). The mirrors were searched but mostly failed the PF>=2 trim, so they are absent from the exports. **Score them regardless of survival across all six folds and compare each pair.** Note: the variable-level mirror question is already answered (94.7% of mirrored conditions appear on the other side). This task settles the combination-level question. Combinations are NOT required to transfer — freedom of the variables to form their own clusters is the design intent.
  - **(iii) NULL BASELINE on F0.** Naive reference already computed: 1,788 observed six-of-six vs ~360 expected under an independent-fold null at the empirical per-fold profit rate 0.437 -> **4.97x excess**. Formalise with a permutation test.

- [x] **16. Selection + structural analysis (Quantitative Research Analyst seat)** — drive the final signal set and trade-engine makeup from `discovery_master.csv` under the locked doctrine (persistence-primary: folds_plus then min_fold_pf; worst_day_usd minimized toward $0, not gated at a constant; agg_pf and WR as ranking axes; target lot 1.0; include-and-let-selection-sort). Four defined analyses, all post-discovery, none of which the scanners performed:
  - **(a) Overlap / dedup pass.** F0 deduplicated internally (>80% entry-overlap collapsed); **F1 did not** — its 384,553 rows contain heavy redundancy (lag-7 vs lag-8 twins of the same trade). Compute each survivor's entry-bar set, collapse candidates above the overlap threshold keeping the strongest representative, and produce a CROSS-FAMILY overlap report (does an F1 pair fire on the same bars as an F0 triple?). This answers whether the families found different edges or the same edge from different angles — the diversification question the multi-type engine depends on. Inspect the real overlap structure in the data BEFORE specifying the tool.
  - **(b) Lag distribution of the persistent F1 pairs.** Of the 11,772 six-of-six survivors, where does `k` cluster? Stage 5 found convergence LEADS the D2D flip by 12-20 bars. If the persistent pairs bunch in that window, F1 is not 'F0 with lags' — it is measuring the anticipatory structure that precedes the D2D turn.
  - **(c) 'The flip before the flip' — D2D anticipation.** Test whether surviving sequences fire BEFORE the D2D directional change. If so, D2D stops being an entry gate (enter after the trend declares) and becomes a target (enter into the ignition, D2D confirms you were right). Consequence: entries land nearer the turn -> tighter structural stops -> worst-day collapses toward $0 -> better R:R at the same WR.
  - **(d) Good-flip / bad-flip discrimination.** D2D standalone is ~76.7% WR, so ~23% of flips fail. Prior work tested only single-variable STATE at the flip bar (found nothing) — a snapshot, not a trajectory. Test whether an ordered SEQUENCE in the bars preceding a flip separates good flips from bad. Filtering failed flips serves worst-day-toward-zero more directly than entering good ones early. (a)-(d) are independent; any one is valuable, together they reshape the engine.
  - Then: re-run the F0 density dimension over the SELECTED set (where it discriminates, unlike the full pool), and assemble the final set — diversified by entry-overlap, not one edge wearing many labels.
  - **(e) Concurrence is a STRENGTH signal, not a dedup artefact.** When several independent triplets fire on the same bar, that bar is more significant — not redundantly counted. The dedup pass must do TWO opposite things: collapse trivially relabelled duplicates (e.g. F1's lag-7 vs lag-8 twins of one trade) while PRESERVING and MEASURING genuine multi-signal stacking. Do not conflate them.
  - **(f) The sample is ONE regime.** Jan 19 - Jun 25 2026 runs from the post-Iran/US-war trough (~45K) to ~53K — a sustained bullish recovery. F0's 6/6 survivors with >=30 trades split 604 LONG / 98 SHORT: that skew is the tape, not a defect. The 98 shorts that persisted AGAINST a strong uptrend are structurally interesting for exactly that reason. The baseline contains no sustained downtrend; the data cannot answer how these behave in one. Know what the sample can and cannot say.
  - **(g) PREFERRED ARCHITECTURE: a concurrent-convergence engine, not a frozen signal list.** A triplet at percentile extremes + D2D agreement has a symmetric partner (hi/hi/hi + D2D-up long <-> lo/lo/lo + D2D-down short). In a bull sample the long side accumulates persistence and the short side does not, purely because the tape rose — so selecting on persistence BAKES THE REGIME INTO THE SIGNAL LIST and forces perpetual re-backtesting. The robust alternative: the RULE is the engine. Any convergence at extremes with D2D agreeing may fire, either direction, weighted by concurrence depth. The 51,311 survivors evidence that the rule works; the specific 702 are a regime artefact. Selection's job becomes proving the rule and calibrating the density threshold + gates — not curating a list.

- [x] **17. Validate the discovered set** — DONE 2026-07-11. Diamond OOS-validated and LOCKED. A tic-proof full-field re-analysis (`analysis_engine.py` + `run_full_analysis.py`, F0-only, Auditor-signed-off) re-scored all 51,311 F0 candidates at true $1/pt with monthly+ISO-weekly+day-of-week persistence + per-day vectors (`signal_full_records.csv` 2,420 survivors, `signal_per_day_pnl.jsonl`). Three-task validation confirmed the diamond PARETO-OPTIMAL on (OOS PF, WR, worst-day) vs the full 2,420 field.

- [x] **17a. FINAL ENTRY ENGINE LOCKED — upgraded 35-signal diamond — 2026-07-11.** Applied one verified 2-short swap: OUT `Micro_BarEntropy:lo+Micro_Rejection:lo+Round_500_Dist_ATR:lo` and `Bar_Range:hi+Micro_VolAccel:hi+Body_Size:lo` (both daily/weekly-fragile — Task-1 flagged, Task-3 excluded); IN `EMA_Oscillator:lo+Micro_Hurst:hi+ADX_Value:lo` (PF 5.21, 6/6) and `Efficiency_Ratio:hi+Micro_BarEntropy:hi+Body_Size:hi` (PF 4.63, 6/6). FINAL 35 (20L/15S, ratio 1.37), S.7-base in-book: WR 92.7-92.8%, PF 6.43, 6/6 folds, 22/22 weeks, net ~$40,218/6mo @1lot, worst-day -$81.30, bear +$10,753. OOS (base TM): PF 5.54, WR 92.1%. Objective (most-persistent + highest-performance + future-proofed) MET. Four v2 record documents regenerated.
- [x] **17b. TM UPGRADE VALIDATED — momentum-conditional runner — 2026-07-11.** Rule: `v=Micro_LogReturn*dir; if v>=0.00012 LeapFrog lag=3 else lag=2` (widens runner room on high-momentum bars; runner trail ONLY — SL/BE/tier/Friday/entries all = S.7; Micro_LogReturn already computed live, no look-ahead). Passed all four stress tests (broad OOS plateau, all-6-fold incl. March, +/-20% perturbation-stable, clean runner attribution). QUANTIFIED: full PF 6.44->6.57, net +$1,021; OOS PF 5.54->5.91 (+6.7%); WR & worst-day unchanged; 6/6 held. STATUS: validated research, IMPLEMENTED AT EA-CONFIG TIME through the full pipeline (Developer to spec; Supervisor+Auditor verify against source). Entries unchanged.

- [x] **17c. FINAL ENGINE = BOOK-50 — 2026-07-12.** The engine grew past BOOK-35 through disciplined, OOS-gated, tic-proof-dataset steps: BOOK-35 -> +5 within-vocabulary co-firers = BOOK-40 (breadth-not-leverage; from the nq>=3 concurrence-elite investigation) -> +18 structure fillers = BOOK-58 (proved bear value: March net $16,460 vs $11,209) -> leave-one-out trim of the 10 worst book-level eroders = BOOK-48 (35L/13S; OOS PF 6.65, highest of project; peak-quality object) -> +1 SQUEEZE + 1 TREND_EXHAUSTION structure fill = **BOOK-50 (37L/13S), 8/8 market structures covered, FINAL.** BOOK-50 RUN-TM: WR 91.7%, PF 5.78, net $57,419/6mo @1lot, worst-day -$127.5 (held), max-DD -$165.6 (held), 6/6 folds, 22/22 weeks, all 5 weekdays, min-fold PF 5.15, March-bear PF 5.23, OOS PF 6.54, OOS WR 92.0%. Structure fills (F1-grammar, both clear the F0-grade persistence + clean-decorrelation bar, both hold worst-day -127.5): SQUEEZE `Sqz_Val:hi ->13-> Micro_OrderFlowDelta:lo` [LONG] (6/6, 20/20 wks); EXHAUSTION `ADX_Rising:==0 ->8-> D2D_DirStep:==-1` [LONG] (6/6, 18/21 wks — long-side so no counter-trend loss-stacking; short-exhaustion candidates rejected for deepening worst-day). Closed threads: D2D reversion (refuted), concurrence-depth (edge is the triples not depth; the '89%' was the 100x/small-sample artifact, does not reproduce), wholesale structure-balancing (fails; disciplined 1-per-structure fill succeeds), F1 family (real but weaker; redundant for net, uniquely fills the 2 structures only). BOOK-48 kept on record as the peak-OOS-PF alternative (6.65 vs BOOK-50's 6.54); BOOK-50 chosen for full structure-completeness at zero worst-day cost. Objective (most-persistent + highest-performance + future-proofed across regime + decorrelation + structure) MET.
- [ ] **17d. PRE-BUILD CONFIRM + CLEANUP (Monday 2026-07-13, before any build).** Three gates before Stage 9 begins:
    1. **Package the source of truth.** Go over all local content and confirm it is correctly packaged as the single authoritative source: rd_plan + execution_sequence (updated), the 11 DOT50_ documents (record set + development set), the 3 non-negotiables (BOOK-50 sacred registry, anti-curve-fit law, phase=build), the DOT50 signal records. Verify consistency across all — no stale BOOK-35/40/48 numbers where BOOK-50 should be.
    2. **Independent Quant-Auditor review (fresh Claude Opus 4.8 instance).** Fire up a new Opus 4.8 instance framed as a Quant Auditor, given ONLY the project-file data, tasked to independently verify that BOOK-50's 50 signals are the genuine diamonds — re-derive the persistence (3 scales), decorrelation, OOS, and anti-curve-fit conclusions from source, with no inheritance of this session's framing. Confirms (or challenges) the 50 from scratch. (Determine the correct 4.8 mode/config at the time.)
    3. **Comb + simplify + snapshot.** Once 1+2 pass: export/snapshot the ENTIRE project folder as a dated record of everything that led to completion (the full build-up, preserved). Then comb through and simplify the project files down to only what the BUILD requires — a clean working set — while the snapshot preserves the full history.
    ONLY after 17d (all three) is complete does the build (step 18) begin. Confirm -> clean up -> build.

- [x] **17e. PRE-BUILD RE-SCAN — three ratified changes + all research closed — 2026-07-14.** A full re-scan of every BOOK-50 assumption produced three ratified changes (roster unchanged):
    (1) **6-LOT LIVE-RISK JAR** replaces the MAX_POSITIONS=6 count cap. Jar holds 6 lots of LIVE (pre-BE) risk; 1 lot/trade; a new signal opens only when live lots < 6; a position reaching break-even frees its lot. Counts RISK not bodies (BE'd winners no longer block new entries). Validated (1 lot): 2,409 tr (+74), net $58,685 (+$1,418), PF 5.83, worst-day/max-DD identical (-$127.5/-$165.6), OOS PF 6.57. Strict improvement at the same 6-lot hard bound. EA: running live-lot counter (+1 open, -1 at BE, guarded once; post-BE close does NOT decrement). Sacred S.15.
    (2) **PER-BAR EXECUTION MODEL** reverts the EA from per-tick SL management. Reposition the SL only on a NEW BAR off the CLOSED bar's High/Low (arm/tier/trail on closed-bar extremes); broker keeps the HARD SL live intrabar (downside always per-tick) — only the EA's REPOSITIONING is per-bar. This is what makes live == the validated per-bar book (third parity leg with export=live and jar-parity). Per-tick + LOCK 1.0 = instant arm-stop, collapses runners 387->102 into an unvalidated bracket. A genuine parity fix caught before build. Sacred S.16.
    (3) **TM CONSTANTS CONFIRMED SWEPT-OPTIMAL** (no change): risk_mult 2.0, MAX_RISK 150, STEP_PCT 0.30, base lag 2, runner lag 3, momentum 0.00012, BE_TRIG 1.0, LOCK_FRAC 1.0 — all on optima. LOCK_FRAC swept independently (1.0 dominant; looser lock nets -$12.9k). A 'fantastic BE' result was caught as a lock-scaling BUG and inverted on fix. Sacred S.17.
    (4) **MOMENTUM-CONDITIONAL WIDER INITIAL SL** (S.7 change g, ADOPTED): on momentum entries (Micro_LogReturn x dir >= 0.00012, the same gate as the runner) the initial stop widens to min(ATR x 4, 150); non-momentum keeps min(ATR x 2, 150); the $150 cap is INVIOLATE (widen up to it, never beyond). -28 losers (194->166), WR 91.9%->92.8%, PF 5.83->6.12, OOS PF 6.57->6.99, min-fold 5.01->5.49, worst-day/max-DD held, net $58,685->$58,249 (-$436). A quality lift, not a profit upgrade — the ONLY loss-reducer that held net ('room helps, taking room hurts'; filters/tighter-stops/two-step-BE all failed by cutting or scratching winners). Runner interaction clean. Sacred S.19.
    CLOSED with mechanism: gates optimal (losers indistinguishable at entry); post-entry failure signature real but net-negative to act on (fires late, false-positives are runners); luck ruled out (null test 16-35 sigma, TM-alone loses); concurrence-depth a real causal quality gradient but up-sizing deep stacks is a correlated-reversal landmine (jar is the safe way to take them); double-BE/per-tick-cushion fails. Null/shuffle test ratified as a validation requirement (S.18). System confirmed at a genuine optimum — every loss-cut also cuts winners. DOT docs (11, DOT_ prefix) + 3 non-negotiables (sacred S.1-S.18) updated and frozen.

- [ ] **17f. DEVELOPER PRE-BUILD TASKS (engine parity + program map).** Two developer deliverables before/alongside the build, so future discovery runs stay valid:
    1. **Engine jar fix:** update portfolio_simulation_engine.py admission from body-count (len(active_trades) >= MAX_POSITIONS) to LIVE-lot count (active_trades where be_nudged is False; admit when < 6), derived from be_nudged each bar (no mutable counter, no double-decrement); confirm the trade-management pass runs BEFORE admission so a same-bar BE frees a slot; reproduce the jar book (2,409 tr, $58,685). Reflect S.16 (per-bar) and S.19 (momentum-SL) in the engine too, so a future re-run scores the ratified behavior. Sacred S.15/S.16/S.19 — full pipeline.
    2. **Program map:** Developer produces DOT_stage8_program_map.md — full instruction manual for the stage8_discovery pack (directory, every script purpose/inputs/outputs, pipeline order, exported artifacts + schemas, run-from-scratch runbook, worker counts, verification against known-good numbers, and the ratified-behavior requirement) so future diamond-hunts can be re-run from scratch and trusted.
    3. **CONDITIONER/OVERLAY DISCOVERY SCANNER (new, F14):** extend the pack to hunt the S.20-class behaviors — variables whose EXTREME state acts as a CONDITIONER on the book rather than a standalone trade: (i) conviction/sizing gates (does variable-X-high at a book-trade entry predict that trade runs further / higher-PF, by direction — the Micro_Hurst pattern), (ii) gap-fillers (does variable-X-extreme trade well ONLY in the book's flat gaps — the FailedBreak pattern), (iii) short-lead confirmers (do book trades within K bars of a variable-X-extreme outperform). Score by the IMPROVEMENT conferred on BOOK-50 (net/PF/OOS with the mandatory tail-check), not standalone WR/PF. This is what lets the blind auditor and future runs RE-FIND the conviction/gap behaviors (S.20 was found by hand from F13; F14 makes it systematic and re-discoverable). Same infra conventions (oracle unmodified, ratified engine, progress/ETA, parallel, crash-resume, results_F14_*.csv). Sacred-infra rules apply.

- [x] **17g. CONVICTION SELF-SCALING + GAP-SINGLE SYSTEM (S.20) — researched, adopted, built into the pack — 2026-07-16.** The F13 single-variable-extremes scan (standalone, separate from F0-F12) hunted a lone-variable "Heart of the Ocean" (100% WR + 100% persistence): clean NEGATIVE — no single variable reaches book-level WR at full span; convergence confirmed necessary. But it surfaced two full-span persisters that became D2D CONDITIONERS, not standalone trades: **Micro_Hurst:hi** (R/S trend-persistence meter — high Hurst at a book-LONG entry = that long RUNS FURTHER, OOS PF 16.6 vs 6.6; a winner-SIZE effect, long-only) and **Micro_FailedBreak:hi** (failed-breakdown reversion — book longs 1-5 bars after a FailedBreak-extreme run OOS PF 12-24). Adopted the "G" system: (1) conviction sizing — book-LONG entry with Micro_Hurst>adaptive-p90 → 2.0 lots (longs only), + recentFB (book long within 5 bars of Micro_FailedBreak>p90) → x1.25, higher-mult-wins; (2) two gap-only singles — Hurst>p97 & D2D=+1 → LONG, FailedBreak>p90 & D2D=-1 (counter/reversion) → LONG, both fire ONLY when zero Dots positions open, 1 lot, LOCK=3, gate ADX>=15 & Vol>=300; (3) per-bar sequence book→Hurst-size→else-if-flat→gap-singles, one shared 6-lot jar. **DESIGN RATIONALE: the EA scales ITSELF by lot multiplier only where probability is highest (95.7%-WR high-Hurst longs; gap-singles only when flat) — SAFER than blanket-2x, which would double every worst-day indiscriminately. Base lot never changes; the multiplier IS the scaling. DEPLOY 1-LOT BASE ONLY.** Ratified sacred S.20 (all three non-negotiables), recorded in rd_plan (2026-07-16), propagated to all foundational docs + records. Developer BUILT it into the stage8_discovery pack (portfolio_simulation_engine + conviction.py + G-scorer) so the pack scores the FULL committed design (export=live restored; the blind auditor and future re-scans need this). **CORRECTION surfaced by the build:** the quant's earlier MODELLED figure ($90,103 / 2,828 tr / x2=254 / wd -147.2) carried a jar-sharing artifact — the research harness double-counted book trades (book 2,511 WITH gap-singles present > 2,361 flat, impossible under a shared jar). The pack engine is jar-honest (gap-singles share the one jar and only DISPLACE book trades: book 2,358 <= 2,361 flat), so the HONEST canonical figure is **G = 2,691 tr, WR 92.2%, PF 6.15, net $89,487, worst-day -153.7, 6/6 folds** (option map: A flat $58,277 / B Hurst-only $66,434 / G' recentFB-off $84,554 wd -127.5 / G all-on $89,487). The 0.7% correction changes no conclusion; $89,487 re-baselined across all docs. Pack engine/conviction/scorer VALID + Supervisor-reviewed; pending Auditor sign-off against the honest $89,487 before final ratify.

- [x] **17h. D2D CROWN JEWEL — the founding signal added as conviction + gap-filler (both roles) — 2026-07-17.** D2D (Anthony's original custom-OBVf-driven concept, the founding gate; a complete independent trade system already in equiDOT.cs with its own native TM) was pushed to its standalone ceiling then integrated into the DOT system in two complementary roles. **D2D STANDALONE THRONE:** raw D2D-flip, both directions, ADX>=30 & Micro_Hurst>=p30 & Vol>=100, ratified TM, conviction 2× on Hurst>=p90 → 32 tr (18L/14S), WR 96.9%, PF 16.89, net $2,431, worst-day -153, 6/6 folds, OOS 100% WR, ONE loss in ~5 months. Gentle lever = loosen Hurst (persistence) not ADX (trend-strength) — added only winners. Native D2D SuperTrend trail decisively WORSE (net -$836, 28.8% WR) → EA build strips native trail, runs D2D on ratified TM. **ROLE 2 — D2D-CONVICTION (2× sizer, BOTH directions — its unique contribution, a SHORT conviction source Hurst/recentFB can't give):** up-size a DOT book trade 2× when entry bar has `D2D_Signal==dir & ADX>=30 & Micro_Hurst>=p30`; higher-mult-wins with Hurst/recentFB (never 4×). Genuine edge, not artifact: flagged trades avg $114 vs $30 random (100th pct), beat strong-trend-NO-flip cohort ($114 vs $35) — the FLIP discriminates, not trend-strength. Honest lift +$1,011 (initial +$6,014 was a double-count 2×'ing Hurst-already-2× trades → 4×; corrected). **ROLE 1 — D2D-GAP-FILLER:** D2D throne entries landing in DOT's flat gaps fire as standalone entries (zero Dots open, shared jar, gap-gated like Hurst/FailedBreak singles) → 14 additive trades (4L/10S, all 100% WR), +$2,070, and they OFFSET DOT's worst day (combined -153.7 → -100.9). **SIZED FLAT 2 LOTS (adopted 2026-07-17):** because they clear the throne gate (96.9% WR, highest in book); DELIBERATE asymmetry vs the S.20 gaps (1 lot, lower gate) — not a bug. FLAT 2-lot constant that BYPASSES the conviction arrays (not 1-lot-then-scaled → would 4×, forbidden). Survival-safe (always solo/fire-when-flat; worst single gap SL ≈-$300, 8.3× inside gate). $92,567 was the modelled 2× number; the BUILT gap-path engine refined it to $92,347 (Auditor-RATIFIED 2026-07-17, ~$220 displacement, immaterial) — $92,347 is canonical. **CROWN JEWEL (BOOK-50 + S.20 + D2D conviction + D2D gap):** 2,698 tr, WR 92.3%, PF 6.40, net $92,347 (+$2,915 / +3.3% vs DOT-alone $89,432), daily worst-day -104.4 (IMPROVED from -153.7), daily max-DD -145.9, 6/6 folds (min fold PF 5.39), OOS PF 6.96 / net $29,190. Additive, no double-count (Role 2 +$1,011 + Role 1 +$2,070; different bars); survival improves on every axis. **14-GAP CEILING (proven both ways):** looser (ADX→15) adds losers + deepens tail (rejected); tighter/split (shorts on Micro_Rejection) drops winners from an already-100%-WR set (rejected). 14 is the honest count of high-quality DOT-flat D2D moments in ~5 months — 14 clean > 30 fragile; do not chase a round number. **BANKED LEAD (not adopted):** Micro_Rejection lo is a genuine broad-D2D-SHORT conditioner (clean-flip decisiveness; 84.5%→94.7% WR, 16/19 independent of Hurst, OOS+, mechanism-backed) but useless in the already-pristine gap slot; promising not proven (n=19, ~6 losses erases, failed strict Bonferroni); filed for a future broad-D2D-short use-case, needs fresh-export confirmation. **CORRECTIONS:** tradeable window ~5.0 months (first scannable 2026-01-26 20:54; NOT 5.2/6); WARM-UP BUG — gap-single masks skip the 6900 InitBars floor (6 leaked into warmup); ALL gap entries must inherit the guard (to fix in build; DOT-alone with guard = $89,432). **STATUS:** adopted; D2D now a THREE-role member — directional gate (existing) + conviction 2× sizer both directions (Role 2) + gap-filler singles (Role 1). `D2D_Signal`/`D2D_Trend_Dir`/`ADX_Value`/`Micro_Hurst` all in the 171 columns → Role 2 is pure per-bar column logic; only Role 1 needs its own zero-position gate. Pending: quant blueprint (conviction-stacking precedence table, gap-priority, warm-up fix, full per-bar order) → pack scripts represent both D2D roles for the blind audit → Developer EA build → docs propagation → full pipeline audit.

- [ ] **18. Stage 9 install** — install the FINAL BOOK-50 engine: 50 signals (48 F0 triple-convergence + 2 F1 sequential structure-fills for SQUEEZE_BREAKOUT + TREND_EXHAUSTION) + S.7 base TM + the validated momentum-conditional runner upgrade (17b) into the EA, PLUS the two ratified execution changes from 17e: (d) the 6-LOT LIVE-RISK JAR replacing MAX_POSITIONS (running live-lot counter: +1 open, -1 at BE guarded-once, admit only when live lots < 6), and (e) PER-BAR SL REPOSITIONING (reposition only on new-bar off closed-bar High/Low; broker keeps the hard intrabar SL) reproducing portfolio_simulation_engine.py, PLUS (f) the MOMENTUM-CONDITIONAL WIDER INITIAL SL (at entry, if Micro_LogReturn x dir >= 0.00012 use catastrophe stop = min(ATR x 4, 150) else min(ATR x 2, 150), but BE-arm and step_size stay on base_risk = min(ATR x 2, 150) — NOT the widened risk; the $150 cap inviolate; reuses the runner's own momentum value, no new variable). The 2 F1 signals need a bounded sequential-latch subsystem (2 ring buffers <=15 bars on the Sqz_Val + ADX_Rising latches + lagged lookups, reusing the already-tracked ST_Flip anchor — NOT a second engine). PLUS (g) the S.20 CONVICTION SELF-SCALING + GAP-SINGLE SYSTEM (the adopted "G" config, honest reproduction net $89,487 / worst-day -153.7 @ 1 lot): conviction sizing (book LONG entry, Micro_Hurst > adaptive p90 → 2.0 lots else 1.0, longs only; + recentFB book-longs within 5 bars of Micro_FailedBreak-extreme → x1.25, higher-mult-wins); two gap-only singles (Micro_Hurst>p97 & D2D=+1 → LONG; Micro_FailedBreak>p90 & D2D=-1 → LONG; both fire ONLY when zero Dots positions open, 1 lot, LOCK=3, gate ADX>=15 & Vol>=300); per-bar sequence book-signal→Hurst-size→else-if-flat→gap-singles. DEPLOY AT 1-LOT BASE ONLY (the multiplier IS the scaling; no blanket-2x). Sacred S.20; full pipeline; pack implementation built + Supervisor-reviewed, pending Auditor sign-off. PLUS (h) the D2D CROWN JEWEL (17h): D2D promoted to a three-role member — (i) directional gate (existing), (ii) D2D-CONVICTION 2× sizer on book trades BOTH directions when `D2D_Signal==dir & ADX>=30 & Micro_Hurst>=p30` (higher-mult-wins with Hurst/recentFB, never 4×; pure per-bar column logic — D2D_Signal/ADX/Hurst all in the schema), (iii) D2D-GAP-FILLER standalone FLAT 2-LOT entries when zero Dots open (the 14 DOT-flat throne trades, shared jar, own zero-position gate like the Hurst/FailedBreak singles) — 2 lots because they clear the throne gate; implemented as a flat constant that BYPASSES the conviction arrays (never 1-lot-then-scaled → would 4×). S.20 Hurst/FailedBreak gaps stay 1 lot (deliberate asymmetry). Run D2D on the RATIFIED TM (strip its native SuperTrend trail — decisively worse). Crown jewel = net $92,347 / daily worst-day -104.4 / PF 6.40 / OOS 6.96 / 6-6 folds @ 1-lot base (D2D-gap at flat 2 lots) — RATIFIED (Auditor PASS 2026-07-17). BLUEPRINT REV 2 SIGNED OFF — build-ready; the one structural build item is `short_mult` support in portfolio_simulation_engine.py + conviction.py (both longs-only today; without it D2D's short conviction is silently dropped). The flat-2-lot D2D-gap must bypass the conviction arrays (no accidental 4×). ALSO FIX in this window: the WARM-UP GUARD BUG — all gap-single entries (Hurst/FailedBreak/D2D) must inherit the 6900-bar InitBars floor the book respects (6 leaked into warmup). Requires the quant blueprint first (conviction-stacking precedence table across Hurst/recentFB/D2D, gap-firing priority when multiple gap-conditions collide, per-bar execution order, warm-up guard) before the Developer builds. Sacred; full pipeline. Auditor confirms exact live performance numbers; then regenerate the four record documents with confirmed upgraded-TM figures. Original notes: through the gate; final non-negotiables amendment. BUNDLE the tester-init correction into this (already-unfrozen) EA window: add `!IsTesting()` to the `OnInit` bar-count guard at L356 to match L1125, AND have the Developer verify the tester init path (array sizing via `ResizeAllArrays(current_bars)` at reduced `current_bars`, forward buffer growth) so the Step-17 tester run reaches valid saturation — or author a tester-only warmup-injection path so the full window is parity-valid. EA change → full pipeline. Do NOT apply during the discovery freeze.

- [ ] **19. Execution-parity re-verification** on the deployment build (absorbs the deferred Step 11) — requires the Stage-9 tester-init correction in the build; compare tester trades vs `portfolio_simulation_engine.py` only over the post-warmup-saturation portion (or the full window if the warmup-injection path was built); export the tester Results trade list for the comparison. Then **one-week demo forward test** with the sim re-run over the same week → demo == sim; then **go live at minimum lot** on one FTMO account, scaling to the other two only once live matches sim.

- [x] **17i. PHASE-1 BLIND-AUDIT CLOSED + `reproduce_dot.py` delivered — 2026-07-18.** The standing blind Quant-Auditor's Phase-1 derivation was reconciled against the committed system (recorded: `quant_auditor_phase_1_closing.txt`). Verdict **CONSISTENT (scope difference, not disagreement)**: the auditor ran `reproduce_dot.py` itself + independently re-scored BOOK-50 flat ($58,277, matches to the dollar); the gap to $92,347 is exactly the four deferred layers ($58,277 + $34,070); its blind flat book and the committed book are the same edge, the committed a BETTER SELECTION from the identical 2,420 pool (gate hid nothing — all 48 F0 inside its field); OOS PF ~7 substantially clean (48/48 Jan-Apr-persistent, median Jan-Apr PF 9.13). Per-choice: 2 F1 sequential HOLDS UP (74%/86% additive coverage), gap-singles HOLDS UP (strongest, can't stack losses), D2D crown jewel DEFENSIBLE (improves worst-day), conviction 2× DISCIPLINED (named trade: ~20% deeper wd for +$13,100). **The committed $92,347 is real, reproducible, and confirmed by an adversarial blind instance.** `reproduce_dot.py` delivered as the permanent operator re-score tool (reproduces $92,347 from source; `--signals new.csv` re-scores any book; prints oracle sha each run) — AI-out-of-the-loop number verification. **Record correction: OOS net $29,326 → $29,190** (from source, n=815; ~$136 stale leftover; OOS PF 6.96 unaffected) across all docs.

- [x] **17j. MASTER PROGRAM (`master.py`) — consolidated, RATIFIED as the sole pipeline entry point — 2026-07-18.** The scattered stage8_discovery pipeline consolidated into ONE command. Structure: `/data/` (market-agnostic drop-in) + `/discovery/` (auto-split ≤9MB outputs) + `master_guide.md` + `master.py`. One command runs S0→S9 (ingest → oracle → all-family discovery → unify → gate → stale-regen → contenders → committed score → report); checkpoint/resume (`.done` keyed to input sha), progress+ETA, --workers ≤12. **ASSET-AGNOSTIC:** the only branch is "frozen book supplied (`--book`)?" — never "is this US30?"; S0-S7 are pure geometry, oracle self-calibrates to any data. `--book book50_signals.csv` → replay+verify (reproduces $92,347); no `--book` → discover-fresh book (survival-first, flagged "not yet data-validated"). IMPORTS the 5 sacred files (never rewrites) — byte-locks intact (9f1f6e3b08ee / 793e6e5f8d9a / 6530e2508b17 / bb498eb13ce3 / 27af7acee824); folds in `reproduce_dot.py` logic as S8. **Pipeline: quant spec (`master_stage_spec.md`) → Supervisor-verified → Developer built (554 lines) → Supervisor-verified acceptance → Auditor RATIFIED (PASS, all 8 checks).** Auditor independently ran it, mutated sacred files to prove abort-on-drift (exit 2), byte-reassembled splits, traced branch logic; $92,347 self-computed; S7 ladder attributes every dollar C0 $58,277→C4 $92,347. One scoped note (not a defect): S8 has no resume-skip — by design (a verification stage re-checks, never skips; cheap+idempotent). Docs reconciled master-centric: `master_guide.md` + `discovery_map.md` + `master_stage_spec.md` current; **RETIRED** `RUN_STAGE8.md`, `DOT_stage8_program_map.md` (was 17f.2 — superseded), `stage8.py` (absorbed into S0). F10 confirmed folded into F0 (concurrence null), not a gap. **Reusable analyst layer delivered:** `non_negotiables_master_analyst.txt` + `master_analyst_initialiser_prompt.txt` — any future instance runs the master on any data with ratified discipline, delivers the mechanical verdict, surfaces the human forks. The master computes; the analyst interprets; the human decides. **STATUS: master.py is the ratified single-command discovery/analysis engine, runnable on any 171-feature market export. Supersedes the multi-script pack. (Note: supersedes the 17f.2 program-map deliverable.)** **Post-ratification (2026-07-18): the Auditor-ratified master.py sha was `9f1f6e3b08ee`; two committed-path-preserving patches (Windows UTF-8 file-I/O; natural-sort + S0 header-handling for >9 split parts, plus rebuild.py integration via shared `_packutil.py`) supersede it — current master.py sha `9f1f6e3b08ee` (was `db8957587844` at that time), $92,347 re-verified REPRODUCED on Windows and from a clean clone; a fresh Auditor pass on `17acb49571fa` (was `db8957587844` at that time) is pending.**

- [x] **17k. NEW-DATA REVEAL — first true out-of-sample event — 2026-07-21/22.** A fresh EA export (100,000 rows, 2026.04.08 → 2026.07.21) extended the data past the sealed baseline for the first time. `rebuild.py` and `master.py` both worked first time on real-world use, no code changes. **Data integrity CONFIRMED:** 75,732 overlap bars, OHLCV Open/Low/Close max diff 0.000000, adaptive variables (KAMA/D2D_Trend/Micro_Hurst/ADX/PoC) median 0.000000 — export=live parity and the KAMA `.bin` seed working on fresh data. (Correction recorded: the non-zero maxes are NOT cold-start — the warm-up region is max diff 0.000000; they sit on a single day, 2026.06.17, entering via one High-bar discrepancy at 04:45 and re-converging the same session.) **RESULT on the genuinely-unseen segment (Jun25–Jul21, 18 days): BOOK-50 = 375 tr, PF 2.19, WR 81.1%, net +$8,407, worst-day −$565.** Profitable, survival intact (11% of the FTMO ceiling), did NOT invert — but materially degraded from PF 6.08, and trade rate was unchanged (~20/day) so the system did not self-throttle in the weaker regime. Full diagnosis: `DOT_new_data_reveal_2026-07-21.md`.

- [x] **17l. DIAGNOSIS + THE `AT_Regime_ST` GATE — 2026-07-22.** Persistence measured: **26 of 52 entities (50%)** held (positive, PF>=2, WR>=75 in both segments) against a **27% random-triple baseline** — selection works at ~2x chance, real but insufficient. **TREND_CONTINUATION (+60%) and BREAKOUT_EXPANSION (+39%) earned MORE per day on new data**; MOMENTUM_IGNITION, PRICE_ACTION and STRUCTURAL_ENTRY broke. **CONCURRENCE IS THE PERSISTENCE AXIS** — monotonic in both segments, depth 1 = PF 1.25 new, depth 3+ = ZERO losses in either period; solo entries carry avg loss 2–3x avg win (breakeven-WR 64–75%) so they collapse on any WR slip, while concurrent entries have balanced payoff. Hurst-p90 conviction held (PF 13.42 → 11.81). **77% of new-segment profit came from the largest-move quartile — the edge is big-move capture.** **THE `AT_Regime_ST` GATE IS RETRACTED (corrected 2026-07-23).** This step originally recorded a directional-alignment gate as the phase's fix, citing 267 tr / PF 3.83 / +$9,228 / wd −$116 on the new segment. Those figures are arithmetically correct but MISLABELLED — they came from a regime-STATE filter (`AT_Regime_ST == 1` regardless of trade direction), caused by a case bug (`'long'` vs the uppercase `'LONG'` column). Measured properly on the same 375 trades: **directional alignment with correct encoding = 171 tr, WR 74.9%, PF 0.97, net −$111, worst-day −$683** — it REMOVES the book's profit and DEEPENS the tail. Consistent with concurrent PF falling 9.76 → 6.85 (`AT_Regime_ST`) and 9.76 → 6.97 (panel-true `sign(AT_Slope_ST)`). The regime-state variant also fails regime-conditional persistence (effect confined to July; OLD-segment differential neutral 6.33 vs 6.25). **No AT gate is adopted; recorded decision 2 is REVERSED.** The variable stays in the vocabulary as a reported conditioner. **ENCODING CONFIRMED against DOT.cs L3102/L3113: `AT_Regime_ST == 0` is BULLISH (inverted from intuition). It is a LATCHED anchor, not `sign(AT_Slope_ST)` — the ~4% disagreement is hysteresis. RATIFIED GATE VARIABLE: the native binary state.**

- [x] **17m. STITCHED DATASET — single source of truth — 2026-07-22.** `DOT_stitched172_jan19_jul21_part01..09.csv`, **177,251 x 172, 2026.01.19 15:49 → 2026.07.21 17:09.** Sealed baseline for Jan19→Jun25 (152,983 rows, warm), fresh export for bars AFTER Jun25 only (24,268). Schema reconciled UP to 172 (`VWAP_Sigma_ATR` native on fresh rows, oracle-derived on baseline rows exactly as the engine does at load; verified max diff 1.84e-5). Text-level stitch, digit strings preserved byte-for-byte. **Supervisor-verified: baseline half 171/171 columns bit-identical, fresh half 172/172 bit-identical, seam clean (Δt 00:01:00, no gap/dup), 0 dup, 0 NaN.** 25MB parts + manifest with per-part sha256. Prior baseline and fresh-export packs retired from project files. **Never frame analysis as "old vs new" — one continuous geometric palette.**

- [x] **17n. GATE-FIRST DISCOVERY REDESIGN — CLOSED 2026-07-24 (spec + 3 builds + consolidation; the scan itself is step 17p).** Rebuild signal discovery around what demonstrably persists. **Gate stack:** ADX >= 15, ticks > 50 (both existing minimal-participation filters, unchanged) and D2D directional agreement. **NO AT gate** — the AT_Regime_ST addition recorded in 17l is retracted (directional gate = PF 0.97, net −$111; regime-state variant fails regime-conditional persistence). AT variables remain fully in the vocabulary as reported conditioners. **Depth >= 2 required — solo convergence excluded from the forward book.** **GATES ARE STATE COLUMNS, NEVER ROW FILTERS** — no bar deleted, every candidate scored both ungated and per-gate-subset (a row filter destroys the counterfactual, makes an inverted-encoding error silent and irreversible, and deleting rows upstream of the oracle breaks mechanism-D's rolling-2500 ring = export=live parity failure). **SELECTION PRINCIPLE CHANGED:** the old objective optimised for DECORRELATION, but overlap IS depth and depth is what persists — selecting against overlap selected against survival and concentrated the book into the fragile depth-1 population. Corrected objective: signals that **CO-FIRE often** (creating depth) while their **FAILURES remain uncorrelated**. Plus: minimum UNIQUE-variable count at depth (not merely signal count); multiple-testing correction across the funnel; stability selection across bootstrapped subsamples; regime-conditional rather than aggregate persistence. **SEQUENCE:** (1) Quant writes `discovery_redesign_spec.md` → (2) Supervisor verifies → (3) Developer builds → (4) Auditor ratifies → (5) full discovery run on the stitched dataset → (6) **WALK-FORWARD VALIDATION OF THE SELECTION PROCESS ITSELF across 3–4 time splits — select on A, test untouched on B** → (7) documentation propagation. **STEP 6 IS MANDATORY AND MUST NOT BE SKIPPED.** All prior validation (blind audit, OOS, folds, decorrelation) sat INSIDE the Jan–Jun window: the book was validated, the selection process never was. Target: persistence materially above the current 50%. If it is not achieved, iterate BEFORE committing a book.

- [x] **17o. DOCUMENTATION PROPAGATION — done 2026-07-24 for the redesign phase (rd_plan, execution_sequence, README, QUICK_START, master_guide, discovery_map). Seat non-negotiables and the signal dictionary/performance record still pending a ratified book.** Original scope: — after 17n produces a ratified book: rd_plan, execution_sequence, master_guide, discovery_map, signal dictionary/overview, performance record, readme, and all seat non-negotiables updated to encode the gate-first paradigm, the depth>=2 rule, the gates-as-columns law, and the corrected selection principle.

**NOTE ON STEP 18:** BOOK-50 remains the committed artifact and is NOT retired — it is profitable and survives on unseen data. But it is superseded as the FORWARD book pending 17n. Step 18's install list must be re-derived from the 17n output before any EA build begins.

- [x] **17n-A. THE SPEC + THE DOCTRINE — 2026-07-23.** `discovery_redesign_spec.md` (1,468 lines, sha `f325a9dfc4b6`) authored by the Quant, verified across FOUR Supervisor rounds, final verdict SHIP, amended nine times. Alongside it `DOT_signal_discovery_mantra.md` (305 lines, sha `fae943d40231`) — **STANDING DOCTRINE, not a finding, not superseded by measurement.** Five rules bind every seat on every run: measure the cake not the bite (every finding labelled MARKET or BOOK); include then let the evidence sort; no pre-set targets; depth is the unit of quality; **negative conclusions carry the same burden of proof as positive ones.** Plus the standing construction — when a finding depends on a filter, the filter is part of the finding.

- [x] **17n-B. BUILD 1 — MEASUREMENT AND PLUMBING — RATIFIED `eea3e3fe931a`.** New stage **S3B** (per-family evidence review across all fourteen families, INSUFFICIENT-EVIDENCE a permitted verdict), `trades.csv` emission, the §E.1 D2D four-part measurement, §D reach measurements, data-relative OOS, and the `discovery_map.md` F10 correction (F10 is fused into F0; the GAP flag was stale). **HEADLINE: the discovery scan has never been run.** `discovery_results/` held exactly one legacy file (F13), no `S3.done` marker exists, `discovery/results/` is empty — every run to date used `--book`, which skips discovery. So F2-F9/F11/F12's "exploratory" label rests on nothing measured. **D2D measured properly for the first time and is NOT inert** — gate on PF 5.14 / worst day −$639 vs long-ungated PF 1.53 / −$2,315; inverting collapses PF to 1.27, ruling out the AT-class encoding error. Closes reveal open item #3. **The OOS window was flattering:** May-June are now interior months, legacy OOS PF 5.54 vs data-relative 3.01.

- [x] **17n-C. BUILD 2 — THE SELECTION LAYER — REJECTED, THEN RATIFIED `296d612b7e9f`.** `engine/selection.py` and stage **S5B**: the lexicographic objective (survival hard constraint → FailConc bound → maximise DepthYield → coverage → FailCorr tie-break, the two properties never collapsed into a composite), per-direction greedy/CELF search, tail-dependence and mCVaR constraints, vocabulary hygiene, multiple-testing. **REJECTED on a search defect that would have eliminated the short side entirely:** greedy returned ZERO shorts — not a judgement, but because 0 of 13 short signals score above zero alone (one signal cannot stack with itself at S=5), so every first-step gain was exactly 0.0 and the search halted at step 0 without ever evaluating a pair. The best short PAIR scores 0.012295, **above the incumbent's own short reference of 0.00757** — greedy returned 0% of the achievable optimum. Fixed with a lookahead-2 stopping rule applied at every termination point, not just step 0. SHORT 0% → **100%** of the enumerated optimum; **and LONG used 2 pair escapes, so the old rule was silently costing the long side ~8% too.** Submodularity measured, not assumed (38-53% violations at stated trial counts) and the (1−1/e) bound claimed nowhere.

- [x] **17n-D. BUILD 3 — THE WALK-FORWARD ON THE SELECTION PROCESS — RATIFIED `ca4903e0ba0b`.** `engine/wf_selection.py` and stage **S5C**. Splits DERIVED from an executability floor (>=3 monthly buckets, >=60 post-warmup days, >=3 buckets per direction) giving **3 splits, train 60/83/105, test 21/20/20**, embargo ~2,730 bars against a 1,440 requirement. **Anti-leak proven at full coverage: 176 of 176 rolling thresholds bitwise identical whether computed on the full series or on the training prefix alone** — so computing the oracle once and masking cannot see a test segment. Every bound (F_max, T_max, C_max, hygiene, H.3 buckets) re-derived per training segment. Append-only attestation written BEFORE any test touch; `TestSegmentGuard` yields the slice once and raises thereafter while still incrementing the counter. §H.3 UNEVALUABLE raises rather than falling back to a full-series rule — **the specific collision by which this step would otherwise pass while fake.** Ten-item rejection list implemented as executable assertions with computed bases. **The pass criterion reports UNEVALUABLE and will until a candidate pool exists.**

- [x] **17n-E. CONSOLIDATION — RATIFIED `2c11b70871c4`.** Five patch layers merged into ONE authoritative `dot_master_discovery/` (105 files), byte-reproducible from a pristine clone via `DISCOVERY_REDESIGN_consolidated.patch`. Manifest verified accurate — 42 rows, zero sha or status mismatches. An over-deletion of `raw/` and the 60 F13 shards during assembly was caught and fully restored; net diff carries zero deletions. All 17 ratified changes survive.

- [x] **17n-F. S3 OPERABILITY — SIX DEFECTS — REJECTED, THEN RATIFIED `17acb49571fa` (final).** Four were operator requirements predating the redesign and absent from the spec, which is why nine audit passes did not raise them. **(1) S3 did not resume per family** — only F1 skipped and `all_rows` accumulated in memory; fixed with per-family `.done` markers carrying the CSV sha256 and atomic writes, **proven against a real crash: 7 families read back from disk, 3 re-scanned, result byte-identical to a clean run. Worst case is one family, never the stage.** **(2) No ETA or progress** on a two-day stage; fixed with `[family i of N]`, running ETA and a 60-second heartbeat, all flushed for Windows, with no wall-clock reaching any artifact. **(3) `--workers` accepted and ignored**; now plumbed to cross-family process concurrency. **(4) Parallelism was 3 of 14 scanners**; cross-family process parallelism added, within-family declined (nine ratified scanners, nine signatures, deterministic row order), determinism proven byte-identical across workers 1/2/3 and in mixed mode. **(5) Two false documentation claims** asserting resume worked. **(6) THE MOST SERIOUS, SELF-FOUND: `orchestrate()` loaded the sealed baseline itself** and never received master.py's ingested frame — **S3 could not run on the operator's data at all**, failing outright on the stitched series or silently scanning the wrong dataset; fixed by frame injection with the standalone fallback preserved. **A seventh, found in audit:** the parallel-worker frame cache was written only if absent, never cleaned, and not keyed on `input_sha` — the same failure as (6) one layer down and reachable by default; fixed three ways (sha-named, purge non-matching siblings, delete on completion). `--workers` default 12 → 2 after measuring **733 MB peak RSS per worker**.

- [ ] **17p. RUN THE DISCOVERY SCAN — THE NEXT ACTION.** `python master.py --workers 10` on the stitched series. 1-2 days. **This has never been run.** It produces the candidate pool that every unexercised component needs: the funnel re-run per split, entity persistence, §H.1's multiple-testing components, §H.2 stability selection, and **the pass criterion itself — the single number this redesign exists to produce.** Ceiling is 10 workers (`min(workers, pending_families)`, and there are 10); ~733 MB each, so ~7.3 GB on a 32 GB machine. Interruptible — re-run the same command and it resumes per family. One output directory is now safe across assets. **A FAIL is a legitimate result** and the code is built to report one rather than lower a bar. Read the artifact header on splits 0 and 1 before their numbers: they test a weaker constraint set than split 2.

- [ ] **17q. EVALUATE THE SCAN AND SELECT A BOOK** — quant reads the output, Supervisor verifies, and only then does step 18's install list get re-derived. BOOK-50 remains the committed artifact until superseded by a book that passes 17p's walk-forward.

- [ ] **17r. EXPORT-CLOCK DEFECT — `ExportDataForAnalysis()` WRITES SERVER TIME AS EST. MUST BE FIXED IN THE NEXT EA WINDOW.**
  **THE EA'S LIVE CLOCK IS CORRECT AND MUST NOT BE TOUCHED.** `GetEstTime()` at DOT.cs L1770 is `TimeGMT() + GetUSEasternOffsetSeconds()` — genuine GMT plus the US Eastern offset. The chart visuals, the session containers, the DST transition and the live Friday cutoff all run off this and all behave correctly; the operator has observed this over months.
  **THE EXPORT IS WRONG.** DOT.cs L730-731:
    `long estOffset = _GetEstOffsetForTime(Time[i]);`
    `datetime estBarTime = (datetime)(Time[i] + (datetime)estOffset);`
  `_GetEstOffsetForTime(datetime gmtTime)` at L1761 declares its parameter as **gmtTime**. The export passes `Time[i]`, the bar's **SERVER** time. **TWO ERRORS ARE STACKED:** (1) the offset is SELECTED on the wrong instant, so `_IsUSDST()` is evaluated on server time — this matters only within a few hours of the US DST boundary; (2) the offset is APPLIED to server time rather than GMT — this is the whole broker GMT offset and is the main error.
  **BLAST RADIUS: EXACTLY THREE COLUMNS.** `estOffset` and `estBarTime` appear only at L730-734 and `estDt` reaches only the emission of `EST_Hour`, `EST_Minute`, `EST_DayOfWeek`. Every other exported column derives from the EA's own internal state and is correct. Verified by grep of the whole file.
  **THE FIX:** convert the bar time to GMT before applying the offset. The broker runs GMT+2 winter / GMT+3 summer, so `true_EST = Time[i] - server_gmt_offset + _GetEstOffsetForTime(gmt_equivalent)`. Both errors must be fixed together — selecting the offset on GMT and applying it to GMT.
  **MEASURED ERROR — server-to-true-EST is a CONSTANT -7h. There is no DST gap.** An earlier calendar-derived rule in this document predicted a -6h window between US DST (2026-03-08) and EU DST (2026-03-29), on the assumption the broker follows the EU schedule. **MEASUREMENT REFUTES IT: the opening bell sits at broker 16:30 in EVERY week of the span.** Had the broker run EU DST the bell would sit at broker 15:30 during Mar 9-27; it does not. **The broker follows the US DST schedule.** So the correction is uniform: `true_EST = server_time - 7h`, which falls out as -2h on the 46,425 bars up to 2026-03-06 and -3h on the 130,826 bars from 2026-03-09 (the change is the US offset moving -5 to -4, not the broker moving).
  **THE EU/US DIVERGENCE IS REAL BUT SURFACES ELSEWHERE:** in the Mar 9-27 window London's open correctly reads **04:00 EDT, not 03:00**, because the US had sprung forward and Europe had not. Both facts hold; they concern different clocks.
  **TRANSITION BARS: ZERO.** 2026-03-07/08 and 2026-03-28/29 all carry 0 bars — weekend gaps. So error (1) — `_IsUSDST` evaluated on server time — **misassigns no bar**, because the boundary instant falls inside a weekend. It remains a latent defect and must still be fixed; it simply cost nothing on this dataset.
  **HOW IT WAS FOUND:** the S2B terrain reported peak median displacement at "12:00 midday" — the flattest hour of the NY session — which the operator identified as impossible from domain knowledge. The quant located the true opening bell from price alone: median Bar_Range 32.00 -> 101.69 and volume 239 -> 502 in a single minute at column 11:30 (winter) / 12:30 (summer). Six independent anchors corroborate: cash close at column 19:00, open-to-close exactly 6.5 hours, the CME equity-index maintenance break at column 20:05-21:04, the 08:30 ET data release, London open at column 06:00 (x2.26), Asia open at column 23:00 (x2.41). Corrected, peak displacement is **true 09:00 NY OPEN at 144.0pt with 225 largest-decile episodes**, decaying monotonically, and the true lunch lull at 12:00-13:00 carries 76.5 / 72.0pt — exactly what the operator's market knowledge says it should.
  **VERIFICATION AFTER THE FIX:** a fresh export must reproduce the corrected columns produced under 17s. That equality is the parity check on the fix and it is not optional.

- [x] **17s. CORRECTED DATA — DELIVERED 2026-07-27 as TEN parts (`DOT_stitched172_TRUEEST_*`), not nine: the 25MB ceiling is the binding constraint and the source parts were unevenly sized.** `EST_Hour` / `EST_Minute` / `EST_DayOfWeek` recomputed from the raw broker `Time` column via the three-regime calendar rule, all 169 other columns bit-identical, invariants held, re-split at 25MB with a per-part sha256 manifest. Verified against all six price anchors in all three DST regimes separately, including the 6-hour gap window. Transition bars on 2026-03-08 and 2026-03-29 handled explicitly, and bars affected by error (1) — offset selected on server time near the US boundary — identified separately because they do not follow the clean three-regime rule. **Excluding F9 from selection was proposed as a containment and REJECTED on doctrine grounds** (no book may be selected from a partial view of the search space); the data is corrected instead so all fourteen families stay scoreable.

- [ ] **17t. CONSEQUENCES OF THE CLOCK CORRECTION — three, all requiring action.**
  **(a) THE FRIDAY GATE WAS BLOCKING THE WRONG WINDOW — MEASURED. IT IS IN THE SCORING PATH.** `portfolio_simulation_engine.py` L148 blocks Friday entries at `EST_DayOfWeek==5 & (EST_Hour>16 | (EST_Hour==16 & EST_Minute>=45))`, reading the export clock. On the broken clock it fired at **true 13:00**, blocking **3,835 bars** — roughly the final three hours of every Friday cash session. On the corrected clock it blocks **115 bars** at true 16:45-16:49, exactly the intended window (this feed's Friday session ends 16:49; 5 bars x 23 Fridays; 3 Fridays are early closes at 09:14 once and 12:59 twice).
  **THE GATE WAS ALWAYS CORRECT — IT WAS READING A WRONG CLOCK.** That file is **SACRED and byte-locked at bb498eb13ce3 — DO NOT EDIT IT.** No code change was needed or made.
  **MEASURED HARNESS EFFECT:** broken clock 3,057 trades / WR 90.9 / PF 5.07 / net $98,205 -> corrected **3,101 trades / WR 90.6 / PF 4.81 / net $97,675**. **+44 trades, -$530.** Newly scoreable Friday afternoon: 101 trades, net $240, WR 74.3%.
  **EVERY FIGURE IN THE PROJECT RECORD BEFORE 2026-07-27 WAS MEASURED WITH FRIDAY AFTERNOONS EXCLUDED.** The delta is small; the point is that the restriction was unintended and undocumented. The EA's live gate at L8781/L8935 uses the correct `GetEstTime()` and has always blocked the intended window — so live and backtest disagreed on Friday afternoons throughout the project.
  **(b) F9 SESSION GATES WERE SCORED ON THE WRONG SESSION.** `session_temporal.py` anchors on `EST_Hour==8`, `EST_Hour==9 & Min==30`, `EST_Hour==10`. An F9 signal labelled `IN-SESSION 16:00` fires at true 13:00 in backtest and true 16:00 live — a genuine export=live parity break, not a labelling nuisance. Resolved by 17s.
  **(c) EVERY HOUR-LABELLED FINDING IN THE RECORD RELABELS.** Including the earlier "size>=8 clusters concentrate at 11:00-13:00 EST" result, which moves to the **NY OPEN** and **strengthens**. Restate it; do not let it be inherited. ATR-tercile findings are unaffected. `dots_thresholds.py` L104 derives its day boundary from `str(times[i])[8:10]` — the raw broker timestamp, not `EST_Hour` — so mechanism D's thresholds, the episode set, episode counts, the 3,816 UP / 3,674 DOWN split and the incumbent's measured REACH (1.389% UP / 0.735% DOWN) are all **UNAFFECTED**. Only conclusions about *when* were ever corrupt.

- [x] **17u. `terrain.py` ELIGIBILITY LABEL — RESOLVED 2026-07-27. DECISION: CORRECT THE LABEL, DO NOT APPLY THE FILTER.** New sha `dcaecaf7e8e1`. Reasoning, quantified: `ADX>=15 & Volume>50` is the ENGINE TRADABILITY predicate — a property of the BOOK, not the market. Applying it would cut the terrain 7,490 -> 4,105 episodes and **inflate reported reach x1.83 with no change whatsoever in the book** (UP 1.389% -> 2.539%, DOWN 0.735% -> 1.339%), making the reach problem look half as bad by definitional choice. The eligible subset stays recoverable via an additive per-episode `start_bar_eligible` flag plus summary columns; the eligible count is now reported at 4,105 (54.8%). `SESSION_BANDS` also corrected — on the fixed clock the old bands labelled the 09:00 open hour "pre-open". Original finding: L63/L108 label every emitted row `ADX_Value >= 15 & Volume > 50 & post-warmup`, while `cluster_profiler.thrust_events` L224 applies post-warmup only. **45.2% of episodes (3,385 of 7,490) start on bars outside the stamped mask.** Under the standing construction — when a finding depends on a filter, the filter is part of the finding — the label misstates the population. Either apply `eligible_universe` or correct the label; the two give materially different terrains and the choice must be deliberate. `terrain.py` is not sacred and may be edited.

- [x] **18. DISCOVERY PIPELINE REBUILT AND COLD-RUN ACCEPTED — 2026-08-13/15.** MASTER COMPLETE 1:35:35, all stages ok, 97.5% concurrent, 90 artifacts. Sweep coverage **71 of 71 artifacts, zero sentinel cells, zero missing-required columns.** Work delivered: `--smoke` path with 14 of 14 caps installed via `dot_frame_binding.py` and non-empty stage assertions (~20 min); `scanner_sha` in family markers so S3 self-heals on code change rather than counting files; all seven 999/inf producers converted to `PF_UNDEFINED = float('nan')` with a 10-site consumer table (admission passes explicitly, sorts rank highest, aggregates exclude with the count reported, emission blanks); coverage reporting — "N of N examined, N UNCHECKED" — on all four detectors, each now exiting 1 on a coverage gap; `pricing_resolution_floor = N_F/n_null_family` added to catalogues (F0 0.391 resolves, F1 22.21 does not and is select-only permanently, F9 0.224).
  **F12 GRID ERROR, FOUND AND FIXED:** the null baseline swept k=1..8 on *condition-count* depth when the book measures *signal-count* depth. `depth_long` is never below 8, so `depth>=k` was vacuous at every k<=8. Replaced with per-direction grids from the measured distribution — LONG [8,17,22,27,34,48,59], SHORT [8,12,18,23,29,40,48] — with k=8 retained as an explicit vacuity contrast and labelled per row. `bars_at_config` and `observed_trades` now emitted.

- [x] **19. THE EIGHT-PHASE QUANT BRIEF — ALL EIGHT CLOSED.** Phase 0: all headlines reproduce, six fixes verified. Phase 1: pricing floor corrected; 60 signals at E<1, 25 orderable, all LONG. Phase 2: the walk-forward decay is REGIME, not dilution — fixed-population decay 0.6738 -> 0.4882 -> 0.3557. Phase 3: two real tiers (35 at zero, 25 orderable); q<0.10 selects exactly the 35. Phase 4: depth and duration are ONE AXIS — 0 of 28 cells clear p<=0.05, min p=0.335; regime rotation refuted at constant 0.21pp occupancy spread while the book rate fell 42%. Phase 5: coverage saturates; **F5 and F8 CLOSED PERMANENTLY** (F8 is `Slope_EMA_ST > Slope_EMA_LT` — one must always exceed the other). Phase 6 skipped, answered by Phase 4. Phase 7: the tick gate is REDUNDANT against ATR>=20 (98.3% overlap; dropping it cost 14 trades and +$456). Phase 8: **THE ADAPTIVE CONVERGENCE ENGINE IS DEAD THREE WAYS** — raw condition depth is never below 8, the intersection curve collapses geometrically at 3-4 descriptors, and the whole field at triple+ is PF 1.29 with a -$6,230 day.
  **PERMANENT NEGATIVES — DO NOT RE-OPEN:** F5/F8 structural; stacked descriptor states dead on both selection rules; single-descriptor gates failing the cross-book check; the tick gate redundant; the adaptive engine dead.

- [x] **20. THE BOOK LINEAGE — BOOK-50 -> OPTION-B -> THE UNION -> THE WHOLE DOT.** BOOK-50: 3,101 trades, WR 90.6%, PF 4.81, net $97,675, folds 6/6, OOS PF 2.95 — the incumbent and the canary. OPTION-B: 120 signals with the p90 gate stack, 1,812 trades, 27 loss events, PF 16.29. **THE FUSION:** the union of BOOK-50, the 60-priced, OPTION-B and S0-120 gives **299 distinct signals from 385 (193L/106S, 1.82:1)** — later reduced to **297 (191L/106S, 1.80:1)** at step 28a, with S0-120 contributing **111 unique at 1-8% overlap with every prior selection** — four different objectives, four different error modes, and that diversity is the finding. The 35-q set contributes nothing unique and must stop being counted as a source.
  **AND EVERY LARGE DRAW FROM THE UNION BEATS OPTION B BY 55-87%.** The union is the achievement, not any particular selection from it.

- [x] **21. DEFECT ONE — THE BAR IS THE RISK UNIT, NOT THE TRADE. FOUND BY THE OPERATOR ASKING FOR ALL 48 LOSS ROWS PRINTED INDIVIDUALLY.** 48 losses collapsed to **SEVEN bar-events** — the jar had admitted 4-9 lots of the same losing trade at identical ATR, identical price, identical exit bar. Every loss rate, CI and PF in the project was computed on replicas. **ALL LOSS STATISTICS ARE NOW STATED AS EVENTS, AND EVERY RATIO CARRIES ITS EVENT COUNT.** Option B's 27 events is the only well-sampled figure in the study; the 7-event books cannot be audited inside a year.

- [x] **22. DEFECT TWO — THE JAR WAS ACTING AS A HIDDEN GATE, AND `CURRENT` ADMISSION IS NOT BUILDABLE. FOUND BY THE OPERATOR ASKING WHY THE CAP WAS 16.** The engine's per-signal loop refused at `live_lots >= MAX_POSITIONS` **mid-event**, so a chopped event recorded a LOWER depth and received a STRICTER gate — `AT_Slope_ST > p90` passes 6.2% of bars where `VolOfVol > p20` passes 82%. Re-tiered bars: 601 trades, 1.66% loss rate against 3.95% for correctly-tiered. **THAT ACCIDENT WAS WORTH 35 LOSS EVENTS AND NOBODY CHOSE IT.**
  **AND IT CANNOT SURVIVE TO A BUILD.** Executed depth is only knowable after the bar's admissions finish, so the post-hoc floor filter deletes **9,356 trades a live engine has already opened** — 1,391 of them from valid depth>=3 events. **A LIVE EA BUILT TO THE `CURRENT` SPEC RUNS A BOOK NOBODY HAS SCORED.**
  **FLOORED IS THE ONLY BUILDABLE RULE:** batch the bar, `n = min(admissible, free)`, refuse the group if `n < 3`, gate on `tier(n)`. Everything knowable at bar close. **NO POSITION IS EVER OPENED AND THEN REMOVED. BACKTEST EQUALS LIVE, EXACTLY.** Cost of honesty: 47 loss events -> 82 before repair.

- [x] **23. DEFECT THREE — `recentfb_sizing` WAS SIZING UP LOSERS MORE THAN WINNERS. FOUND BY THE OPERATOR ASKING WHETHER CONVICTION HAD EVER BEEN SWEPT.** It never had, in 17 months. Measured: removing it drops lots on winners 1.256 -> 1.183 (-5.8%) and lots on losers 1.186 -> 1.094 (**-7.8%**). It was adding 25% size to trades that lose more often than they win. Removing it takes **20% off the worst bar** (-$458.9 -> -$367.2) for 3.9% of net; setting it to 1.5 deepens the worst bar to **-$550.8**, confirming the direction both ways. **`Micro_Hurst x2.0` AND `D2D-agree x2.0` EARN THEIR PLACE DECISIVELY** — off, they cost $9,043 (with a losing week) and $3,824 respectively; flat sizing costs $20,380 and a -$211.1 worst day.
  **ALL THREE DEFECTS WERE FOUND BY THE OPERATOR'S QUESTIONS, NOT BY AN ANALYST'S SWEEP. THAT IS A PROCESS FINDING AND IT BELONGS IN THE RECORD.**

- [x] **24. THE DERIVATIONS THAT SURVIVED ATTACK.**
  **THE d2->d3 CLIFF, MEASURED INDEPENDENTLY IN BOTH DIRECTIONS** on the ungated field (floor=1, ATR>=20): LONG d2 16.55% -> d3 8.75%; SHORT d2 12.54% -> d3 5.40%. Depth 1 and 2 are indistinguishable; three is a different regime. The 16-cell floor grid confirms it — **any cell containing a 2 gives 3.5x to 7x the loss events.** Cliff below, plateau above. The best-evidenced constant in the stack, and it is the operator's original triple-triple.
  **`Micro_Hurst > p90` CLEARED TWO INDEPENDENT NULLS** — p = 0.000 at SHORT d3 (0 of 30 rarity-matched alternatives strictly better) and p = 0.022 at LONG d3 (1 of 45). **THE ONLY CONDITION IN THIS PROJECT EVER TO CLEAR ONE.** For contrast: `Bar_Range > p95` sits at p = 0.371, the median of its own null; `VolOfVol > p20` at p = 0.690 with 40 of 58 alternatives better.
  **`ADX >= 15` IS STRUCTURAL, NOT TUNED:** the threshold oracle builds its rolling percentile ring only from bars where `ADX >= 15`, so every threshold in all 297 signals is calibrated on that population. Trading outside it fires thresholds against a distribution they were never computed from. **THE SAME NUMBER TWICE.**
  **`ATR_1M >= 20` IS A KNEE, NOT AN OPTIMUM:** the curve is monotone in both directions, so it is a risk dial. 15->20 buys 13 loss events for $1,355 (9.6 per $1,000); 20->25 buys 12 for $4,467 (2.7 per $1,000). **THE PRICE OF AN EVENT JUMPS 3.5x IMMEDIATELY ABOVE 20.**

- [x] **25. THE CAP CURVE, 18 TO 96 — BOTH BOUNDARIES AT 22, AND IT IS A CLIFF.** First losing week: cap 22. Worst day first past -$150: cap 22. **THE SAME VALUE, AND THE BREAK IS ONE SLOT WIDE** — worst day -$104.0 -> -$277.5 and worst week +$16.6 -> -$110.9 for a single extra slot; 26 consecutive positive weeks becomes 25. **CAP 21 IS THE LAST SAFE VALUE, ADOPTED.** The cap stops binding entirely at 57 at-risk positions.
  **AND THE CAP NEVER TOUCHES THE TAIL:** worst bar -$367.2 and worst intraday -$845.91 are IDENTICAL at all thirteen cap values from 18 to 96. **THE CAP CONTROLS ACCUMULATION, NOT DEPTH.** The deepest single bar and the deepest intraday moment are properties of the book.
  **AND DIVISIBILITY DOES NOTHING** — swept 3/6/9/12/15/16/18/21 under all three admission rules, monotone throughout, no step at multiples of three. Events run 2 to 47 deep and cannot tile a fixed container. The operator's question was the right one; the measurement refutes it.

- [x] **26. THE SELECTION AND COVERAGE NEGATIVES — THE BOOK CONFIRMED FROM A SECOND DIRECTION.**
  **THE SELECTION LAYER WAS REBUILT FROM THE GROUND UP AND THE REBUILD LOST.** Train-only screen (proportional fold criterion, not a count — `folds_plus >= 4` is arithmetically unsatisfiable for 503 signals lacking 4 buckets), holdout Jun 1 - Jul 21 held back until scored once. **12 of 12 random draws beat the rebuilt selector on losing-bar rate; the ADOPTED book beat 11 of 12 on net and sits at roughly the 4th percentile of random selection.** The rebuild's objective — minimising the union of losing bars — selects for signals that LOSE TOGETHER, which is the many-clones-on-one-bar mechanism written into an objective function on purpose.
  **253 OF 299 CLEAR THE INDEPENDENT REBUILD (84.6%).** The book is not an artefact of the admission defect.
  **DECORRELATION ON SOLO LOSSES IS INVALID AS A SELECTION TOOL, REGARDLESS OF BASIS.** Solo, a signal trades alone on every bar its mask fires; in the book only depth-gated bars produce entries. Solo loss bars are not the population the book loses on. Solo pairwise Jaccard 0.0026 with 99.3% of pairs sharing zero losing bars; at book level 0.0247 and 94.8%. **THE STAGE SHOULD BE DELETED, NOT REPAIRED.**
  **COVERAGE SATURATES AT 68 SIGNALS.** Greedy marginal coverage over all 4,575 qualified survivors exhausts after 68 — no remaining signal reaches a single permitted episode the first 68 do not. Solo-reach union of the entire qualified field is **101 UP / 91 DOWN of 498/486 permitted (20%)**, flat at every book size from 299 to 4,575. **THE 4,276 UNUSED SIGNALS ARE NOT UNUSED COVERAGE — THEY ARE REDUNDANT ON THE COVERAGE AXIS.** And chasing it is catastrophic: a 500-signal coverage book gives 285 loss events, PF 1.78 and a -$4,352 intraday floor.
  **SOLOS AND DUALS CLOSED — STRICTNESS AND COVERAGE ARE ANTI-CORRELATED.** The two strictest gates in the library (0.489% and 0.684% pass rate) reach **ZERO** new episodes. A 2-bar episode requires firing on one of two specific bars; any condition rare enough to be trusted alone is too rare to be there. **RARITY AND PRESENCE ARE THE SAME AXIS POINTING OPPOSITE WAYS.**
  **SHORT d4 AND d5+ STAY FREE FOR A MEASURED REASON** — 20 relocations, every d4 candidate worsens the losing-bar rate, every d5+ candidate increases loss events. Not by Option B inheritance any more.
  **ADMISSION ORDER IS INERT** — LONG-then-SHORT, SHORT-then-LONG and depth-first are identical to the cent. It was ranked first in the defect-three hunt list and is now closed.

- [x] **27. WHAT THE SYSTEM IS BLIND TO — CHARACTERISED FOR THE FIRST TIME.** Traded episodes last 6-9 bars, 180 points, ATR 30-32. Unreached episodes last **2 bars**, 132 points, ATR 24. **ADX AND EFFICIENCY ARE INDISTINGUISHABLE BETWEEN THEM** — it is not a trend-quality filter, it is a DURATION filter. Session distribution is proportional, not selective; the blindness is structural, not temporal. **THE MECHANISM IS THE DEPTH FLOOR: CONCURRENCE TAKES TIME TO FORM.** A 2-bar move ends before three independent patterns can agree it is real. This is a property of concurrence as an entry mechanism, not a tunable parameter — to be accepted and documented, not engineered around.

- [x] **28. THE WHOLE DOT — ADOPTED CONFIGURATION, 297 SIGNALS.** Specification at
  `foundational_documents/The_Whole_DOT_spec_v2.txt` (1,524 lines, sha `1d0323b1ecfa`). It supersedes
  `The_Whole_DOT_spec.txt`, `The_Whole_DOT_rule_master_spec.txt` and `_2`, all three now in
  `foundational_documents/former/`.
  **297 signals — ALL F0 TRIPLES, ONE GRAMMAR · LONG depth >= 3, SHORT depth >= 3 · FLOORED admission ·
  `Micro_Hurst > p90` at LONG d3 AND SHORT d3 · `Micro_FailedBreak > p20 AND AT_Slope_ST > p90` at LONG d4 ·
  `Micro_FailedBreak > p20` at LONG d5+ · SHORT d4/d5+ FREE · `ATR_1M >= 20` global · `MAX_POSITIONS 21`
  counting at-risk positions only · conviction `Micro_Hurst x2.0` and `D2D-agree x2.0`, `recentfb_sizing =
  FALSE`.**
  At 1.0 lot: **5,776 trades, 5,552 winners, 224 losers, WR 96.12%, PF 14.53, net $284,974.00** — gross
  profit $306,043.80, gross loss $21,069.80, average win $55.123, average loss $94.062, win/loss ratio
  0.5860, expectancy $49.338, largest win $1,167.00, largest loss -$297.50, longest streaks 697 / 19.
  **42 LOSS EVENTS on 35 distinct days across 973 ENTRY BARS.** Worst bar -$1,224.00, worst day -$346.60,
  worst intraday -$2,819.70 with p50 -$295.45 / p90 -$944.65 / p99 -$1,784.60, 11 days below -$1,000, one
  below -$2,500, none below -$5,000. **0 losing weeks of 26**, 112 of 119 days-with-a-trade positive against
  132 trading days in the frame. Folds 6/6 at 14 trading days each, min-fold PF 11.0. **OOS final third
  (2026.05.25 -> 2026.07.21) PF 9.78, net $73,851** — event-basis rates 3.00 / 9.30 / 3.67. **Break-even WR
  63.05% against 96.12% actual — a 33.07-point MARGIN**; net at WR -1/-3/-5 gives $276,321 / $259,165 /
  $241,860. Peak 42 open positions, **21 at-risk**, 42.0 lots, ceiling $6,300. Event sizes 3:5 4:12 5:8 6:9
  7:2 8:2 9:3 10:1. Population 6,206 FULL rows -> 5,776 BOOK-only, 430 gap fillers excluded.
  **Linearity proved by direct re-run on the 297 book, not asserted** — path identical on all 5,776 rows,
  drift 0.0016%. **1.0 LOT IS NOT DEPLOYABLE — its ceiling is 126% of the FTMO daily.** At 0.30 lot the
  ceiling is $1,890 = 37.8% of the $5,000 daily.

- [x] **28a. THE TWO F1 SEQUENTIAL PAIRS DROPPED — 299 -> 297.** `Sqz_Val:hi ->13-> Micro_OrderFlowDelta:lo`
  (35 trades, PF 5.90, 6/6 folds) and `ADX_Rising:==0 ->8-> D2D_DirStep:==-1` (42 trades, PF 4.75, 6/6
  folds), both LONG, both inherited from BOOK-50 lines 50-51 and carried through every fusion because the
  union filtered by nothing. **THEY WERE NOT ORPHANS — both carry full statistics in
  `results_F1_sequential_temporal`.** In-book they were 12 trades and $476.2 combined, 0.21% by count and
  0.17% by net.
  **REMOVAL IS BETTER ON EVERY AXIS THE OPERATOR RANKS:** loss events 43 -> 42, event-days 36 -> 35,
  losing-bar rate 4.40% -> 4.32%, PF 14.31 -> 14.53 — and **identical to the cent on worst bar, worst day,
  worst intraday, losing days, losing weeks, worst week and days traded.** Cost $382.50, which is 0.13% of
  net and a lot-size dial. Their net depth contribution is about -$94, so **removing them very slightly
  helped the rest of the book.** The orphan count is unchanged at 14 (6 LONG, 8 SHORT) and the scan ratio
  moves 1.82:1 -> **1.80:1** because both drops were LONG.
  **AND THE LARGER PRIZE IS STRUCTURAL:** the F1 path leaves the configured scoring path entirely, so
  `score_g.build_book` writes nothing back into the frame and **THE FRAME-OBJECT TRAP IS NOW IMPOSSIBLE
  RATHER THAN AVOIDED.** That defect cost 11 trades and produced a plausible wrong answer rather than an
  error. `sequential_temporal.pair_mask` and `anchor_array(df,'ST_Flip')` are no longer needed here, build
  item B3 is retired, and **every row of the listing is now the same three-condition shape — checkable by a
  build script rather than by eye.** `_assert_book_grammar` aborts a configured book containing any
  non-triple.

- [x] **28b. JUNE INVESTIGATED — THE FLATNESS HYPOTHESIS IS REFUTED AND NO GATE WAS FITTED.** June carries
  **14 of the 42 loss events on 164 of 973 bar-events**, PF 5.34, and it is the weak split in every fold and
  walk-forward column. **BUT JUNE IS NOT FLAT — IT IS THE SECOND-BUSIEST MONTH OF SEVEN:** ATR 11.12,
  Volume 94 and Bar_Range 11.0 all rank 6 of 7, TickIntensity 6 of 7, ADX 22.61 ranks 4 of 7. All three
  operator hypotheses point the wrong way.
  **AND THE COUNTEREXAMPLE IS DECISIVE: MAY IS THE FLATTEST MONTH IN THE FRAME — lowest ATR 9.07, lowest
  Volume 67, fewest eligible bars 12.2% — AND MAY IS THE BEST MONTH AT PF 60.64 ON 2 LOSS EVENTS.**
  June's losers are separated from June's winners by the same variables in the same direction by the same
  magnitudes as everywhere else. The elevation is **uniform** — BASE 2.65x, MOM 1.56x, LONG 2.55x, SHORT
  1.80x — which is the signature of a month, not of a bar-level state. `Bars_Since_Flip` appeared to
  separate at 4.04x and collapsed on inspection: the fourteen values are bimodal (0,3,3,7,14,21,28,81,111,
  114,188,286,287,359), seven under 30 bars and seven over 80, and half the losses are on fresh flips.
  W23 is four days, not one, with 3 of its 6 events on 2026.06.05 and two of those on consecutive bars at
  22:52 and 22:53 — **one move, two minutes, 19 trades, -$1,290 combined, and it entangles the "weak d9 /
  strong d10" contrast from the ladder.**
  **TRIAL COUNT ZERO. NO GATE PROPOSED.** Eleven variables at bar level, six at event level, two path
  controls and two direction controls, on a base of fourteen events. `Efficiency_Ratio < p80` was the only
  free candidate matching the direction and it refuses 31% of all winning events. **FOURTEEN EVENTS CANNOT
  SUPPORT A THRESHOLD, AND THE NEXT VARIABLE TESTED WOULD EVENTUALLY SEPARATE THEM BY CHANCE.** June is a
  documented limitation in spec §9.1, not a defect: it remains profitable at net +$32,591, the fifth-best
  month of seven, and it never produced a losing week.

- [x] **28c. THE WEAK DEPTH CELLS ARE MOSTLY SMALL CELLS, AND THE TIER CAP IS WHY THEY CANNOT BE TARGETED.**
  The breakdown report exposed a strongly non-monotone LONG ladder — PF 7.20 / 5.10 / 10.59 / **3.26** /
  58.46 / 9.32 / **1.70** / 23.96 / inf at depths 3 through 11+. **BUT `tier = min(depth, 5)` MERGES LONG
  DEPTHS 5 THROUGH 21 INTO ONE GATE CELL — 3,559 trades, 86.7% of the LONG book, all gated by
  `FailedBreak > p20` alone.** So d6 at PF 3.26 and d9 at PF 1.70 are gated identically to d7 at 58.46 and
  d11+ at infinity. **THE TIER STACK IS STRUCTURALLY INCAPABLE OF DISTINGUISHING THEM, AND `min(depth,5)`
  APPEARS NOWHERE WITH A DERIVATION — IT IS INHERITED FROM OPTION B AND IS NOW LABELLED AS SUCH IN SPEC
  §3.3.**
  **AND THREE OF THE FIVE WEAK CELLS CANNOT BE READ AS RATES AT ALL.** LONG d3, LONG d9 and SHORT d7 rest on
  3, 3 and 1 loss events. **d9's PF 1.70 becomes 2.99 on one event fewer; SHORT d7 becomes infinite on
  zero.** Only LONG d6 has a base worth arguing about at nine events — and 5 of those 9 are in June.
  The neighbour comparison found **nothing to gate on**: d6 against d7, gated identically and 18x apart,
  matches within noise on ATR, ADX, Hurst, FractalDim, VolOfVol, EffRatio, bars-since-flip, session hour and
  momentum share. The only difference is 54 stop-outs against 7, **which is the outcome restated, not a
  cause.**
  **MOVING THE TIER CAP WAS CONSIDERED AND DECLINED ON ARITHMETIC:** opening d6 and d9 as their own cells is
  2 cells x 11 library relocations = **22 trials against 12 events, and Bonferroni needs p < 0.0023.**
  Nothing in this project has ever cleared that. **A GATE FITTED TO d6 WOULD BE A JUNE FILTER WEARING A
  DEPTH COSTUME, FITTED ON FIVE EVENTS.**
  And one ladder figure was checked before anything was built on it: **SHORT d11+ is thirteen replicas of a
  single trade** — all entering 2026.02.23 at 16:47 and exiting 18:20, 567.1 points over 93 bars, $1,128.30
  each. **Not a 14x per-trade edge. One bar.** Same caution applies to SHORT d8, d9 and d10.

- [x] **28d. THE ENTRY-BAR FUNNEL — THE CLEAREST STATEMENT OF WHAT THE SYSTEM IS.**
  177,251 frame bars -> 170,351 post-warmup -> **36,526 at `ATR_1M >= 20` (20.6%)** -> **973 ENTRY BARS.**
  That is 0.5% of the frame and **2.7% of the eligible population** — 8.2 entry bars and 49 trades per
  trading day, $293 of net per entry bar. **`ATR_1M >= 20` IS THE LARGEST FILTER IN THE SYSTEM AND IT
  REMOVES 76% OF REACHABLE TERRAIN BEFORE ANY SIGNAL IS CONSULTED.** By contrast `FailedBreak > p20` admits
  80.19% of bars and is barely a gate at all.
  **AND EVERY ATTEMPT TO WIDEN THE FUNNEL FAILED, MEASURED:** the 500-signal coverage book gave 285 loss
  events, PF 1.78 and a -$4,352 intraday floor; the strictly-gated solo/dual path found strictness and
  coverage **anti-correlated**, with the two strictest gates in the library (0.489% and 0.684% pass rate)
  reaching **zero** new episodes. **THE 0.5% IS THE EDGE, NOT A SHORTFALL — CONCURRENCE TAKES TIME TO FORM.**

- [x] **29. THE FINDING THAT REFRAMES THE PROJECT.** Across **60 complete compositions and three walk-forward windows, every single one is profitable** — field net $12,788-$18,532, survivor net $14,987-$19,119, spread 25-30%. And **24 random 299-signal draws from the field are all profitable out of sample.** **THE SIGNALS CARRY IT. THE COMPOSITION IS A PREFERENCE, AND SELECTION ABOVE A COMPETENT SCREEN IS WORTH SURPRISINGLY LITTLE.** This is the most important measurement in the project: it means the system does not depend on any clever choice being right.

- [x] **30. S8 SCORES THE WHOLE DOT — THE DETERMINISTIC LOCAL PATHWAY EXISTS AND THE OPERATOR HAS RUN IT.**

      python master.py --data data --workers 14 --out discovery\full --stage S8 --book whole_dot_signals.csv

  Reproduces spec §6 to the cent on the operator's own machine in 37 seconds: 5,776 / 96.1% / 14.53 /
  $284,974 / 42 events / 35 days / -$346.60 / -$1,224.00. **BOOK-50 unchanged at 3,101 / 90.6% / 4.81 /
  $97,675 with the canary firing.** Determinism identical at 14 and 4 workers. Sacred five all match.
  Delivered: `engine/adm_engine.py` (`6d1ed10a5f81`) — a FORK of the sacred engine implementing FLOORED
  admission, the per-direction floor, the per-tier stack and a configurable `MAX_POSITIONS`, defaulting to
  `CURRENT`/6/`None` so an unconfigured import behaves as sacred; `engine/swept_thresholds.py`
  (`4356d2bb9973`) — mechanism-D percentiles at arbitrary levels by substituting `dt._D_SPEC` and calling the
  sacred `compute_adaptive_thresholds`, so **the ring, eligibility mask, day-refresh and floor-index are
  bit-identical to production by construction rather than by inspection**; `whole_dot_config.json` carrying
  the rules with derivation labels inline; config-presence routing accepting both sidecar conventions; and
  `_metrics_from_trades` shared by both paths **so folds and OOS cannot drift between BOOK-50 and a
  configured book.**
  **TWO GUARDS, BOTH AGAINST SILENT FAILURE:** a startup fork-parity assertion re-running `adm_engine` under
  CURRENT against the sacred engine on the full frame and failing loudly on mismatch — **necessary because
  the sacred admission path is duplicated verbatim inside the fork's `elif` branch, which is why parity holds
  and is the landmine if sacred ever changes** — and config-not-found now aborting rather than falling
  through, after the sidecar name mismatched during development and the Whole DOT **silently took the sacred
  path and scored the wrong system with no error.**
  **AND A FULL BREAKDOWN REPORT ON EVERY RUN:** weekly (26 ISO periods) and monthly (7) tables with trades,
  wins, trade-losses, **loss EVENTS**, W/L ratio, WR, PF, net, LONG and SHORT counts and worst day; the depth
  ladder per direction; gate admit/refuse rates for all five live gates; top 5 signals by net with the
  ranking key stated; and the population and denominator lines named rather than implied. **THE PARITY SHA IS
  NOW STABLE ACROSS MACHINES** — it was float-repr and column-order dependent, and `_canon_trade_sha()` fixes
  it with an explicit ten-column list, an explicit sort and fixed 2dp formatting.
  **AND THREE DEFECTS THE DEVELOPER FOUND BY RUNNING RATHER THAN READING:** the sidecar-name fallthrough; a
  sliced frame against a full-length oracle; and the week counter using `exit_time[:8]`, a day-level slice
  rather than an ISO week, which reported 7 weeks where the spec has 26. **All three fixed. The 430-trade
  discrepancy that blocked acceptance for two turns was the FULL-versus-BOOK-only population — `master.py`'s
  own `trades.csv` header states the distinction and nobody had read it.**

- [x] **31. `whole_dot_signals.csv` BUILT AND VERIFIED — 297 rows.** Columns `trigger,direction,signal_def`, from §4 of `The_Whole_DOT_spec_v2.txt`. Verified mechanically at assembly: 297 rows, 191 LONG / 106 SHORT, trigger F0 throughout, **zero rows containing `->`, zero rows that are not a three-condition triple.** `_assert_book_grammar` aborts a configured book containing any non-triple.

- [ ] **32. THE GAP-FILLER LOT DEFECT.** `portfolio_simulation_engine.py` L302 hardcodes `glots = 1.0` and L306 reads module constant `D2D_GAP_LOTS = 2.0`; neither reads `long_mult`/`short_mult`. At a 0.30 base, `GAP_HURST` (18 trades) and `GAP_FB` (131 trades) run **more than three times intended size**. They are excluded from the reported book so no figure above is affected, but a live deployment carries them at full size. **BUILD ITEM.**

- [ ] **33. THE REJECTION LOG.** FTMO executes **Fill-or-Kill with no partial fills** — an order is filled only if the entire size executes at one price level, otherwise it is rejected. At 1-2 lots on US30 rejections should be rare, but they are **silent**, and a rejected entry is a trade the backtest assumed was taken. **THE EA NEEDS A REJECTION HANDLER AND A REJECTION LOG FROM DAY ONE.**

- [ ] **34. THE TWO ENGINE CHANGES FOR THE BUILD.** `MAX_POSITIONS` 6 -> 21, and the per-direction depth floor enforced at ADMISSION under the FLOORED rule rather than as a post-hoc filter. **AND STRIP THE UNREACHABLE GATES** — the tier-1 and tier-2 cells cannot fire under a floor of 3 and must not ship.

- [ ] **35. THE FILL MODEL IS THE LAST UNMEASURED ASSUMPTION.** The engine fills at `current_sl` exactly whenever the bar touches it, with no slippage and no gap modelling. **"LOCKED POSITIONS CANNOT LOSE" IS TRUE OF THE SIMULATOR BY CONSTRUCTION AND TRUE LIVE ONLY TO THE EXTENT FILLS ARE CLEAN.** Verified empirically within the simulator across all 5,317 be-nudged trades: exits strictly below the lock 0, minimum (exit - lock) 0.0000 points, minimum captured +12.02 points, minimum P&L +$9.00 per lot — **and +12.02 is exactly what `0.30 x base_risk` predicts at the ATR-20 gate on the base path, a prediction and a measurement agreeing to two decimals.** The $1,890 ceiling at 0.30 lot is exact under the fill assumption and a lower bound live. **THIS IS WHAT THE DEMO PERIOD IS FOR.**

- [x] **36. BOOK B — AN INDEPENDENT SECOND BOOK ON BARS THE PRIMARY NEVER TOUCHES. BUILT, SCORED, AND CLOSED.**
  The operator's question: can a full book be composed from solos, duals and every non-F0 family, under one
  hard rule — **no signal may trigger on any of the primary engine's 973 entry bars.** Exclusion asserted on
  every arm, aborting rather than warning, `gap_singles=False` and `d2d_gap=False` asserted absent each time.
  **AND AN ENGINE FACT WAS FOUND BY THE ASSERTION RATHER THAN BY READING:** `portfolio_simulation_engine.py`
  L299 checks only `conviction is not None`, `len(active_trades) == 0` and `bar >= warmup` — **the gap fillers
  never consult `entry_ok` or `mask_window`, so they bypass ADX>=15, Volume>50, the Friday cutoff and any bar
  exclusion.** They fired on 36 primary bars on the first attempt. Same class as everything in spec §0.1.
  **THE POOL:** 449,399 scanned non-F0 rows, 43,436 qualified at `trades >= 12`, `agg_pf >= 2.0`,
  `folds_plus >= 4` — F1 43,130, F9 143, F13 99, F3 58, F4 3, F11 2, F2 1.
  **BOOK B AT PF>=8:** 755 signals, 6,106 entry bars, 17,531 trades, WR 84.97%, **PF 1.70**, net $166,255,
  **1,012 LOSS EVENTS on 122 days**, worst bar -$1,606.80, worst day -$2,775.50, 4 losing weeks of 26, 127 of
  132 days, episodes 56 UP / 67 DOWN, **break-even WR 76.88% against 84.97% actual — an 8.09-point MARGIN**,
  **$164 net per loss event against the primary's $6,785.**

- [x] **36a. WHY 755 SIGNALS AT PF>=8 COMPOSE TO PF 1.70 — THE PF 8-10 BAND IS THE DILUTION.**
  Standalone `agg_pf` against in-book net per loss event: **Spearman 0.3315, p = 1.07e-17.** Not noise, and
  the band structure is monotone:

      PF band     n     median in-book net    % net-NEGATIVE in book
      8-10      261            $23.40                46%
      10-15     253           $261.40                22%
      15-20      85           $410.50                11%
      20+        34           $505.70                 3%

  **261 of 755 members sit in the band where nearly half lose money inside the book despite every one
  carrying `agg_pf >= 8` standalone.** 186 of 699 traders (27%) are net-negative in-book.
  **AND OVERLAP IS NOT THE CAUSE:** 2,635 losing trades collapse to 1,012 events — a **2.60 replica ratio
  against the primary's 5.33** — with 60.3% of events charged to more than one member. **THE EVENT COUNT IS
  GENUINE, NOT A REPLICA COUNT.**
  Mechanism: `Spearman(solo_pf, in-book trades) = 0.39` but `Spearman(solo_pf, in-book loss events) = 0.17` —
  **high-PF members trade more without proportionally more loss events.**

- [x] **36b. THE PRUNE — EIGHT ARMS, AND THE SPLIT-HALF CHECK THAT DISCIPLINED IT.**
  Contribution ranking persists out of sample — `Spearman(npe_A, npe_B) = 0.2228, p = 5.4e-6` — **unlike the
  F1 reach ratio at -0.064.** But **the top-10 contribution slice goes NEGATIVE out of sample (-$17.30 median
  half-B npe against the population's +$17.00)**, and **PF ranking beats contribution ranking at every matched
  size out of sample**, because PF is measured by the scan on a different population than the book.

      arm               sigs  events     PF   margin  worst day       net    npe  -wks  days
      CONTRIB top 10      10      42   4.34   17.79     -$370    $12,842   $306     0   116
      CONTRIB top 25      25      92   3.41   15.86   -$1,104    $26,825   $292     2   118
      CONTRIB top 50      50     184   3.51   16.75   -$1,250    $55,203   $300     0   125
      PF>=20              34     189   2.27   12.73   -$1,006    $33,251   $176     2   124
      CONTRIB top 100    100     396   3.11   16.10   -$1,578   $106,198   $268     1   126
      PF>=15             124     452   2.12   11.35   -$2,169    $89,078   $197     2   127
      CONTRIB top 200    200     621   2.71   14.38   -$2,463   $169,254   $273     1   126
      PF>=10             408     794   1.93    9.86   -$2,904   $156,687   $197     4   127
      FULL 755           755   1,012   1.70    8.09   -$2,776   $166,255   $164     4   127

  **CONTRIB top 10 and top 50 are the only arms with zero losing weeks. Top 10 carries exactly 42 loss events
  — the same as the entire primary book.** But the split-half says the extreme slice does not hold, so **top
  50 is the strongest non-extreme arm.**
  **AND JULY SPLITS THE RANKINGS:** all three PF-ladder arms are negative in July (-$363, -$1,239, -$1,073);
  **every contribution arm is positive** (+$263 to +$5,056). July is the primary's best month and the only
  genuine signal-layer holdout.

- [x] **36c. THE TRADE-MANAGEMENT SWEEP ON BOOK B — THE ATTRIBUTION IS NOW TESTED, NOT INFERRED.**
  The five constants were swept on a depth-3+ book at cap 21; **nobody had swept them for a depth-1 book at
  cap 6**, and the payoff ratio was named as the binding constraint. **EIGHTEEN SETTINGS, FIVE CONSTANTS, BOTH
  DIRECTIONS ON CONTRIB TOP 50.**
  **AND THE PREMISE THAT MOTIVATED THE RUN WAS REFUTED FIRST.** Book B is not ~100% BE: exit mix **72.1% BE at
  $19.51 / 12.8% LF at $69.81 / 14.8% SL** against the primary's 78.3% / 17.7% / **3.8%**. Winners do run.
  The average win of $27.11 is a blend, not the break-even lock.
  **AND THE PAYOFF GAP IS NOT TRUNCATION — average loss is nearly identical, $90.18 against $94.06.** The
  difference is entirely on the win side and decomposes into two parts: **SL rate 14.8% against 3.8% (3.9x)**
  and **LF mean $69.81 against $174.85 (2.5x lower)**. More stop-outs, and the trails that run capture less.
  **THE PAYOFF RATIO MOVES AND BUYS NOTHING.** `BE_TRIG 2.0` takes it 0.347 -> 0.522, a 50% improvement — and
  it is the worst row in the table at margin 10.27, 467 events and PF 1.65. `STEP_PCT 0.60` reaches 0.510 at
  margin 9.79. **EVERY SETTING THAT IMPROVES THE RATIO CONVERTS WINNERS INTO STOP-OUTS FASTER THAN INTO
  RUNNERS** — SL share 10% -> 19% and 10% -> 29% respectively. Tightening is symmetric: `STEP_PCT 0.15`
  nearly doubles LF share to 30.4% and takes the margin to 10.87.
  **THE BASELINE IS THE MAXIMUM OF THE MARGIN SURFACE IN EVERY DIRECTION TESTED. NOTHING BEATS 16.75.**
  **AND `LF_TIER_MIN` IS INERT** — at 1, 2 and 3 the net, events, PF and margin are identical to the cent at
  $55,203.40; only the BE/LF label moves (LF share 14.9% -> 26.3% -> 90.9%). **The trail already engages at
  `tiers >= lag + 1`; the `tiers < 3` literal only decides which bucket a completed trade is filed under.**
  That closes the most direct fix anyone proposed, on a measurement.

- [x] **36d. BOOK B CLOSES ON A MEASUREMENT: MOSTLY FAMILY, PROVEN.** The prune buys 9.7 points of margin
  (8.09 -> 17.79); the remaining 15.3-point gap to the primary's 33.07 is **structural and it is not the exit
  logic.** The trade management is already at its margin-maximising value on a depth-1 book at cap 6 — the
  same place it landed on a depth-3+ book at cap 21.
  **IT IS THE ENTRY POPULATION. 14.8% stop-out rate against 3.8%, and LF capturing 2.5x less. ONE SIGNAL
  FIRING ON A BAR IS A WORSE BAR THAN THREE SIGNALS AGREEING — WHICH IS THE d2->d3 CLIFF ARRIVED AT FROM THE
  OPPOSITE DIRECTION.** The depth thesis has now been confirmed by building the anti-system and watching it
  underperform.
  **AND ON THE OPERATOR'S ORDERING, ADDING B BUYS 19% MORE MONEY FOR 438% MORE LOSS EVENTS** — $55,203 and
  184 events against $284,974 and 42. **NOT ADOPTED.**
  **ONE PROPERTY OF B WORTH KEEPING: IT ACCUMULATES EVIDENCE ~9x FASTER THAN A.** 1,965 entry bars and 184
  events against A's 973 and 42 — 11 bars per event against 23. **A poor book and a good sensor: thin margin,
  high frequency, on bars A never sees. If the market shifts, B shows it first.** Worth running on demo
  alongside as a leading indicator; not worth running for money.

- [x] **37. COVERAGE IS CLOSED ON EVERY FAMILY. IT IS A SELECTIVITY CEILING, NOT AN F0 CEILING.**
  The density control — does a mask reach more permitted episodes than the same number of RANDOM available
  bars — was built for this question and it answered it at both ends of the selectivity axis.

      population                                    density control
      F3, F9, F4, F2, F11 as unions                 ALL at or below random
      43 PF>=4 members individually                 4 pass, ALL in one window (Thu 10:00), union 23 of 40
      F13 tight percentiles p95-p99                 0 of 15 pass; Micro_Hurst:hi@p99 at 0.65
      B-strict composed (4 F9)                      1.71 mask, but $93 per event and 8 losing weeks in 25 days
      B-full composed (103 signals)                 0.73
      F1 members k>=10                              1.19 / 1.28 across halves — REPLICATES
      F1 ratio-selected                             ANTI-PREDICTIVE, Spearman -0.064
      F1 composed, unbiased                         1.01
      F1 composed, executed                         0.94

  **F1 IS THE ONLY POPULATION EVER TO BEAT THE CONTROL AT THE MEDIAN — 1.092 on 748 members, 395 beating it,
  against ~0.6 and 4-of-43 for every state family. AND IT SCALES WITH k:** k<=3 at 0.782 with 18% beating,
  k>=10 at 1.141 with 61%, **Spearman(k, ratio) = 0.185 at p = 3.4e-7.** At short lags the pair is effectively
  a same-bar conjunction and fails exactly as state masks do; at long lags it stops tracking any single bar's
  state. **THE CONTROL HAD ONLY EVER SEEN STATE MASKS AND F1 IS THE FIRST NON-STATE GRAMMAR IT WAS GIVEN.**
  **BUT THE POPULATION EFFECT DOES NOT SURVIVE SELECTION OR COMPOSITION.** Reach efficiency does not persist
  across halves at all — Spearman -0.064 — and every top slice does **worse** out of sample than the
  population it came from. The unbiased composed arm fails at 1.01, and the executed book at 0.94 once
  admission, the depth floor and the jar reduce 1,668 mask bars to 758 entry bars.
  **AND THE FIGURE THAT SETTLES IT: ALL 29,948 AVAILABLE BARS TOGETHER REACH 491 OF 498 UP AND 480 OF 486
  DOWN. 98% OF THE PERMITTED LANDSCAPE IS REACHABLE BY BEING PRESENT ON ALMOST EVERY BAR.** Episode coverage
  is a near-linear function of how many bars you fire on. **THE F0 CEILING OF 101/91 WAS NEVER A REACH LIMIT —
  IT IS A SELECTIVITY CONSEQUENCE.**
  Selectivity-adjusted, the primary is in a different class: **41.1 episodes per 1,000 bars against F3's 18.7,
  F9's 21.4 and F11's 19.2 — twice the best alternative.**
  **DO NOT RE-OPEN COVERAGE WITHOUT A FAMILY WHOSE GRAMMAR IS NEITHER STATE-BASED NOR PAIR-BASED. NO SUCH
  FAMILY EXISTS IN THIS PROJECT.**

- [x] **38. THE GAP FILLERS — DEFINED, SCORED AND NULLED FOR THE FIRST TIME; AND BUILD ITEM B1 COMES OFF.**
  BOOK-50 was 48 triples + 2 sequential + **3 singles** = 53 members in a file named for 50. The three singles
  were implemented as hardcoded gap-fill paths rather than CSV rows, which is why the file shows no
  single-condition entries and why every "50-signal book" figure was really 53.
  **DEFINITIONS, WRITTEN DOWN FOR THE FIRST TIME** (`conviction.py` L51-56, firing at
  `portfolio_simulation_engine.py` L300-322). Common gate `ADX_Value >= 15 AND Volume >= 300` — **a tick gate,
  not the book's `Volume > 50` eligibility gate** — admitting 12,185 of 177,251 bars (6.87%), plus
  `len(active_trades) == 0`: **they fire only when the book is completely flat.** Priority is a strict elif
  chain, D2D -> HURST -> FB.
      GAP_D2D    `D2D_Signal != 0 AND ADX >= 30 AND Micro_Hurst >= p30`   2.0 lots, LOCK_FRAC 1.0, 36 bars
      GAP_HURST  `Micro_Hurst > p97 AND D2D_Trend_Dir == +1`              1.0 lot, GAP_LOCK 3.0, 170 bars
      GAP_FB     `Micro_FailedBreak > p90 AND D2D_Trend_Dir == -1`        1.0 lot, GAP_LOCK 3.0, 573 bars
  **SCORED AT 1.0 LOT:** GAP_HURST 55 trades / PF 6.48 / $3,036 / 7 events; GAP_FB 355 / PF 5.29 / $19,879 /
  32 events / **zero losing weeks across 103 trading days**; GAP_D2D 20 / PF 1.83 / $506. All three: 430
  trades, PF 5.04, $23,421, 41 events. **$54-56 per trade against the book's $49.34 — they beat the book per
  trade.**
  **AND AGAINST 5,160 SCANNED SINGLES, GAP_HURST AND GAP_FB ARE IN THE TOP 0.1%.** The highest-PF row in the
  entire F13 scan is `Micro_Hurst:hi@p99 LONG` at PF 8.199 — **the same variable at a marginally stricter
  percentile, found twice by different routes.**
  **NULL: both clear at p = 0.000 on profit factor** against 14 draws of the same firing count from the same
  gate population. GAP_FB clears on net as well; **GAP_HURST does not (p = 0.214) — its edge is per-trade
  quality, not aggregate.** A rarity-matched null could not be built: pool pass rates run 4.82%-18.62% and the
  fillers fire on 0.02-0.32%, two orders of magnitude apart.
  **THEY DO NOT BELONG IN THE BOOK.** Adding them takes 42 -> 83 loss events and degrades all three OOS
  windows, while improving worst day and losing days because they trade on 5 days the book never touches.
  **AND THEY CLEARED THEIR NULLS ON PROFIT FACTOR, NOT ON COVERAGE — excellent entries on bars that are not
  scarce, reaching no terrain the primary misses.** That distinction was conflated once and is now separated
  by measurement.
  **BUILD ITEM B1 IS RETIRED. `DOT.cs`, 11,673 lines, grepped exhaustively: GAP FILLING DOES NOT EXIST IN THE
  EA.** No `HURST_GAP`, no `GAP_LOCK`, no `D2D_GAP_LOTS`, no enter-when-flat logic; the EA's percentile calls
  are p80/p20 only. **The L302/L306 lot defect cannot affect a live deployment because the mechanism is not
  deployed.** It remains a simulator correctness note for anyone reading absolute-survival figures.

- [ ] **39. TWO BOOK B ITEMS NOT COMPLETED.** Per-direction scoring of CONTRIB top 50 — **Book B is SHORT-heavy
  at 56 UP / 67 DOWN, and every other book in this project is long-heavy** — and the A+B combined account:
  combined loss events, worst day, worst intraday, losing weeks, days traded, and the combined ceiling as a
  share of the $5,000 daily at 0.20 / 0.25 / 0.30 lot. Both are one call each; inputs checkpointed at
  `perB.pkl`, `bookB_scored.pkl` and `bk297.pkl`. **AND ALL BOOK B FIGURES ARE AT 1.0 LOT — LINEARITY HAS NOT
  BEEN VERIFIED ON THIS BOOK**, and its population differs from the primary's (depth-1, cap 6, more base-path
  trades), so it should be re-run at a smaller size rather than assumed.

- [ ] **40. THE F13 BRANCH IS NOT IN THE REPO AND IT BLOCKS TWO BOOK B ARMS.** Three edits to
  `engine/score_g.py`: `_F13 = re.compile(r'^(.+?):(hi|lo)@p(\d+)$')`, an
  `_F13_STRUCT = {'VWAP_Z': (2.0, -2.0), 'OR_Position': (0.80, 0.20)}` module constant, and a branch before
  the F0 branch delegating to `swept_thresholds.swept(df, {(feat, side): (feat, pct/100.0)})`. Verified in the
  Quant's clone at 99 of 99 members parsing, but never pushed. **Without it `family_mask` aborts on every F13
  member, which blocks the PF>=20 and CONTRIB top-50 arms specifically.**

- [x] **41. A THIRD PATH-RESOLUTION DEFECT, CAUGHT AND BOUNDED.** `sys.path` resolved `adm_engine` to the repo
  copy at `dot_master_discovery/engine/` rather than the working copy, so an `LF_TIER_MIN` edit went to a file
  that was not being imported. **Diffed before proceeding — otherwise byte-identical, so every prior Book B
  figure is unaffected.** Both copies synced. **THIRD DEFECT OF THIS SHAPE IN THE PROJECT: a thing resolving
  somewhere other than where it was written.** Same ancestor as the sidecar fallthrough and the swept-spec
  instruction — spec §0.3 R2.

- [x] **42. THE PER-SIGNAL QUALIFICATION QUESTION — ASKED, MEASURED ON 297 OF 297, AND CLOSED.**
  The objective was never a prune: **which tests must a signal pass to earn its place, so that a future month
  can be assessed by running a stage rather than repeating a week of conversation.** The 297 are a union of
  four different objectives and **no single reproducible procedure produces them**, which is why monthly
  re-qualification could not be run.
  **AND THE OBVIOUS METHOD WAS INVALID AND WAS CAUGHT BEFORE SHIPPING.** Attributing in-book trades to
  individual members — as was done for Book B, where the depth floor is 1 — **DOES NOT WORK IN A
  DEPTH-FLOORED BOOK.** A signal in a depth-5 bar opened one of five trades, but only because four others
  fired. **The proof was already in the record: the six LONG orphans earned $2,649 directly and removing them
  cost $3,923.** Attribution misses the entire depth effect and gets the sign wrong.
  **LEAVE-ONE-OUT IS THE ONLY VALID MEASURE HERE:** remove member X, re-run the full book, record `d_net`,
  `d_events`, `d_bars`. **297 runs, plus 5 parity re-runs, 50 ablation draws and 4 partition cuts — 356
  engine runs total. Zero gates fitted, zero thresholds changed, and nothing about the adopted 297 proposed
  to change.**

- [x] **42a. THE BOOK IS HOMOGENEOUS. NO DILUTION BAND, NO STARS, NOTHING TO PRUNE.**

      d_net    min -$902.90 | p10 $270.60 | p25 $531.60 | median $1,005.60
               p75 $1,798.90 | p90 $2,871.70 | max $8,952.30
               12 of 297 negative (4.0%) | 149 >$1k | 63 >$2k | 7 >$5k | 0 >$10k
               sum of all deltas $401,542 against a $284,974 book — ratio 1.41

  **A 3.4x interquartile band with no tail either side.** `d_events` moves by zero for **212 of 297** members;
  36 by -1, 42 by +1, 4 by -2, 3 by +2 — **every one a one-event observation on a 42-event base. Report the
  count, decline the rank.**
  **PROVENANCE DOES NOT SEPARATE EITHER:** medians $1,280 shared, $1,181 60-priced, $1,066 OPTION-B, $983
  BOOK-50, $759 S0-120 — a **1.7x spread across four selection objectives** on 19-111 members per cell.
  S0-120 has the lowest median and holds the single largest contributor at $8,952. **THE UNION ADDED VARIETY
  WITHOUT ADDING DIFFERENTIATION.**

- [x] **42b. THE -0.4594 WAS A 40-MEMBER ARTEFACT, AND THE MECHANISM PROPOSED FOR IT WAS WRONG TOO.**
  On 40 members the split-half was **Spearman -0.4594, p = 0.00286** — significantly anti-correlated and
  worse than the F1 reach ratio. **ON ALL 297 IT IS -0.0597, p = 0.305. NOT ANTI-CORRELATED. NOT CORRELATED
  AT ALL.**
  **AND THE SUBSTITUTABILITY HYPOTHESIS IS REFUTED BY ITS OWN PARTITION.** The prediction was that the
  anti-correlation would concentrate in HIGH depth-3 members, where the floor makes members interchangeable.
  It is confined to the **LOW** depth-3 group instead — **the opposite** — at +0.1363 (p = 0.079) high against
  -0.2811 (p = 0.0012) low, and after correcting for four overlapping cuts chosen post-hoc, **none survives.**
  **THE HONEST READING: `d_net` CARRIES NO PERSISTENT PER-MEMBER SIGNAL AT ALL.** And 13% of the book in
  `spec_idx` order was not safe to read as a sample — that caveat was stated in advance and it earned itself.

- [x] **42c. THE RANDOM-SUBSET ABLATION — NOTHING IS HIDING.** 50 draws, K = 30 members (10% of the book),
  observed loss against summed individual deltas:

      ratio   min 0.790 | p10 0.878 | p25 0.910 | median 0.967 | p75 0.999 | p90 1.044 | max 1.085
              mean 0.960, sd 0.066 | 12 of 50 above 1, 38 below

  **NO SUPERADDITIVE STRUCTURE.** Leave-one-out is not missing a set that is individually free but
  collectively load-bearing — the ratio would systematically exceed 1 and it does not. It falls slightly
  short, which is **mild substitutability**, consistent with the sum-of-deltas ratio of 1.41 against the book.
  Dropping 10% costs 137 entry bars and 1-8 loss events.

- [x] **42d. NONE OF THE SIX TESTS DISCRIMINATE. THE QUALIFIER IS BOOK-LEVEL ONLY.**

      test                  discriminating power                               verdict
      d_net                 flat, 3.4x IQR, 4% negative, no tail               does not discriminate
      d_net split-half      rho = -0.060, p = 0.305                            no persistence — cannot encode
      d_events              212 of 297 move by zero                            one-event obs — cannot rank
      per-signal OOS        rho 0.335 with full-sample d_net, shared data      not independent
      provenance            1.7x spread across four sources                    does not separate
      ablation              mean 0.960, sd 0.066                               no hidden structure

  **THE UNION IS HOMOGENEOUS. THERE IS NO PER-SIGNAL COMPONENT WORTH ENCODING. THE MONTHLY QUALIFIER IS THE
  BOOK-LEVEL BATTERY ALONE — SIMPLER, NOT WEAKER, AND MEASURED ON 297 OF 297 RATHER THAN INFERRED FROM 40.**

- [x] **42e. AND THE STRUCTURAL CONCLUSION THAT STANDS INDEPENDENTLY: `d_net` MEASURES FLOOR-CRITICALITY, NOT
  QUALITY.** 121 of 973 entry bars (12.4%) sit at depth **exactly 3**, so removing any member of such a bar
  drops it below the floor and kills every trade on it. **A member's delta is therefore largely how often it
  happens to be the marginal third signal — a structural property of which bars it co-fires on, not a
  statement about its merit. TWO SIGNALS OF IDENTICAL QUALITY WOULD SHOW DIFFERENT `d_net`.**
  **AND THERE IS NO UNCONTAMINATED ALTERNATIVE, STATED RATHER THAN INVENTED:** every per-member metric
  available is measured inside a book whose admission rule makes members interdependent by construction —
  **attribution misses the depth effect, leave-one-out measures floor position, and the split-half says
  nothing persists in either case.** A clean per-signal quality metric would need a scoring regime where
  members do not gate each other, **and that regime is not this system.**

- [x] **42f. AND THE PERFORMANCE DIAGNOSIS WAS WRONG TWICE BEFORE MEASUREMENT FOUND IT.**

      suspect                                       measured           verdict
      build_signal_masks rebuilding 296 per run     0.3s for all 297   not the bottleneck
      strided column slice x 177,251 bars           0.3s C / 0.2s F    not the bottleneck
      the analyst's own stats() function            6.10s of 11.68s    THIS

  The cost was never in the engine — it was `groupby('day')`, `dt.strftime`, `groupby(['entry_bar',
  'direction'])` and two `set(zip(...))` constructions on 5,776 rows, 297 times. Vectorised with
  `np.unique` + `np.bincount` it runs in 1.26s, taking the per-run cost **11.68s -> 2.50s.**
  **BOTH SHIPPED FIXES WERE INERT** — `PRE_MASKS` and `asfortranarray` changed nothing, 11.68s before and
  after, correct in isolation and irrelevant. **READ-BEFORE-YOU-OPTIMISE, LANDING ON THE ANALYST RATHER THAN
  THE CODE, TWICE IN ONE TURN, AND REPORTED.** Parity proved on five completed members before resuming —
  `d_net` identical to the cent at 2349.30 / 1430.80 / 1154.00 / 854.10, baseline reproducing at exactly
  $284,974.0 / 42 events / 973 bars / 5,776 trades.

- [ ] **43. BUILD `QUALIFY` — THE MONTHLY RE-QUALIFICATION STAGE. SPECIFIED BY 42, NOT YET WRITTEN.**
  **`master.py` CAN SCORE A BOOK IT IS HANDED. IT CANNOT SAY WHETHER THE ONE IN USE IS STILL RIGHT.** Every
  month brings ~21 new trading days and there is currently no way to ask that question without repeating a
  week of dialogue.
  **THE BATTERY, ALL BOOK-LEVEL, NO PER-SIGNAL COMPONENT:**
    - **the train-only screen** — how many of the 297 still clear `trades >= 12`, `agg_pf >= 2.0`, at least
      three monthly buckets present and profitable in **>= 2/3 of the buckets actually present.** The
      criterion is a **PROPORTION, NOT A COUNT** — `folds_plus >= 4` as a count is arithmetically
      unsatisfiable for 503 signals lacking four buckets. **253 of 299 cleared the last rebuild at 84.6%; a
      drop to 180 is a different signal from a drop to 250.**
    - **the d2->d3 cliff, re-derived per direction on the current data.** LONG 16.55% -> 8.75%, SHORT 12.54%
      -> 5.40% on the ungated field. **IF THAT CLIFF FLATTENS, THE FLOOR IS NO LONGER JUSTIFIED AND IT IS THE
      EARLIEST STRUCTURAL WARNING AVAILABLE.**
    - **the cap boundary re-located.** Both boundaries sit at 22 with cap 21 adopted on a **one-slot margin.**
      **IF THE CLIFF MOVES TO 21, THE ADOPTED CAP IS UNSAFE AND IT OUTRANKS EVERYTHING ELSE THE STAGE PRINTS.**
    - **the gate nulls re-run.** `Micro_Hurst > p90` cleared at p = 0.000 (SHORT d3) and p = 0.022 (LONG d3).
      **IF EITHER STOPS CLEARING, THE GATE IS NO LONGER EARNED.**
    - **the random-book null.** The adopted book sits at roughly the 4th percentile of random draws from the
      qualified field. **IF A FUTURE BOOK CANNOT BEAT ITS OWN NULL, THE STAGE SAYS SO.**
    - **AND THE BOOK'S PERFORMANCE ON THE NEW PERIOD ALONE**, beside the full-frame figures. **THE POINT IS TO
      SEPARATE "THE BOOK GOT WORSE" FROM "THE MARKET MOVED".**
  **ONE VERDICT LINE: PASS, PASS-WITH-WARNINGS, or FAIL, WITH THE REASON. Not a table to interpret at 2am.**
  **AND KEEP THE THREE MODES SEPARATE.** `QUALIFY` answers "does the current book still pass" with no new book
  produced and no churn — **that is the monthly job.** `RESELECT` produces a comparison. `REPLACE` requires
  **DOMINANCE on the stated criteria, not mere difference**, and must refuse rather than pick when the
  comparison is ambiguous. **A qualifier that re-selects every month chases the last month and never holds a
  position long enough to know if it was right.**
  Deterministic — byte-identical artifacts across runs and worker counts, every random draw explicitly seeded
  with the seed recorded in the artifact, no wall-clock in any CSV. Sacred five untouched; use `adm_engine`
  and `swept_thresholds` and keep the fork-parity assertion.

---

*Locked-for-real once the EA is frozen (step 9). Inventing a genuinely new variable past that point — not a new combination of the 117 — reopens a Stage-3-style EA change and another re-export loop, and requires human authorization.*
