# equiDOT — Development Progress & R&D Plan

*Status snapshot — 2026-07-02*

*Active phase: **Step 14 (Stage 8 discovery) CLOSED 2026-07-09 — 440,057 candidates. NEXT: step 15 — build + run `concurrence_profiler.py` (PRE-QUANT, BLOCKING), then step 16 — QRA selection.** Concurrent-convergence behaviour is the measurement this entire rebuild exists for and has still never been scored: the original 766K->4,773->76 pipeline deduped the overlap away before anyone measured it. Profile the stacking (depth, identity, variable recurrence, market-structure conditioning, depth-vs-outcome) over the F0 survivor pool BEFORE any selection. Preferred architecture is a rule-based concurrent-convergence engine, not a frozen regime-selected signal list. Sacred layer intact.*

---

## 2026-07-17 — D2D CROWN JEWEL: the founding signal added to the DOT system as conviction + gap-filler (both roles), honest +$3,080 lift

The D2D signal — Anthony's original, primary, custom-OBVf-driven concept, the founding gate the whole project grew from — was pushed to its standalone ceiling and then integrated into the DOT system in two complementary roles. D2D is a COMPLETE independent trade system already living in equiDOT.cs with its own native adaptive TM; this work re-gated it, scored it on the ratified TM, and added it to the book.

### D2D STANDALONE THRONE (the founding system, given its full due)
Config: raw D2D-flip signal, BOTH directions, ADX>=30 & Micro_Hurst>=p30 & Vol>=100, ratified TM (ATR×2/×4 momentum-SL, BE tier-1, LeapFrog), conviction 2× on the Hurst>=p90 slice. Result: **32 trades (18L/14S), WR 96.9%, PF 16.89, net $2,431, worst-day -153, max-DD -153, 6/6 folds, OOS 100% WR** — ONE loss in ~5 months (a single -153 March SL). Per-side standalone (source-confirmed via ratified engine, Hurst>=p30 throne): **LONG-only 18 tr / 100% WR / PF 999 / net $1,651 / NO losing day (max-DD 0); SHORT-only 14 tr / 92.9% WR / PF 6.10 / net $780 / worst-day -153** (the lone loss sits on the short side). (The 23-trade 13L/10S split is the SNIPER config, Hurst>=p50 — superseded by the adopted p30 throne.) The gentle lever was loosening HURST (persistence), not ADX (trend-strength): at ADX>=30 the flips are already elite, so Hurst>=p50→p30 added only winners (WR rose 95.7→96.9% as trades rose 23→32, OOS held at 999). Dropping ADX instead cracks WR and collapses OOS — rejected. Native D2D SuperTrend trail is DECISIVELY WORSE (net -$836, 28.8% WR — a low-WR trend-runner); the ratified TM's tight BE-banking is what makes D2D shine → the EA build strips the native trail, runs D2D on the ratified TM.

### THE TWO INTEGRATION ROLES (both adopted)
**Role 2 — D2D-CONVICTION (2× sizer on DOT book trades):** up-size a DOT book trade 2× when its entry bar satisfies `D2D_Signal == trade_direction & ADX_Value>=30 & Micro_Hurst>=p30`. D2D sizes BOTH directions (unlike Hurst/recentFB which are long-only) — this is its unique contribution: a SHORT conviction source. Higher-mult-wins with the existing Hurst/recentFB levers (a trade flagged by both takes 2×, never product/4×). Genuine-edge-confirmed (not a selection artifact): the flagged DOT trades avg $114 vs $30 random (100th percentile of 2,000 random-subset draws), and beat the strong-trend-NO-flip cohort ($114 vs $35) on identical ADX/Hurst bars — so it is the D2D FLIP specifically, not generic trend-strength, that discriminates. 28 of 42 flagged trades are winners the Hurst≥p90 lever misses. **CORRECTION during the build:** the initial +$6,014 lift was double-counted (it 2×'d trades already 2×'d by Hurst → 4×, violating higher-mult-wins); the honest higher-mult-wins incremental is **+$1,011** (mostly the shorts, which Hurst never sized). Small but real; the "100% WR" headline is fragile (n=55, ~4 losses erases the WR edge) but the avg-win effect is robust.

**Role 1 — D2D-GAP-FILLER (standalone FLAT 2-LOT D2D entry when DOT is flat):** the D2D throne entries that land in DOT's flat gaps (zero Dots positions open) fire as new entries, shared 6-lot jar, gap-gated exactly like the Hurst/FailedBreak singles. **14 additive trades (4L/10S, all 100% WR)**, +$2,070. These land on DOT's flat days and OFFSET its worst day — the combined daily worst-day IMPROVES -153.7 → -104.4. **SIZED AT FLAT 2 LOTS (adopted 2026-07-17):** D2D-gap fires at 2.0 lots because it clears the highest-conviction gate in the system (the throne, 96.9% WR — the highest-WR population in the book); S.20 doctrine applied to the best trades. DELIBERATE ASYMMETRY vs the S.20 Hurst/FailedBreak gaps which stay 1 lot (they clear a lower ~p90 gate, not the throne) — not a bug. Implemented as a FLAT 2-lot constant that BYPASSES the conviction arrays (NOT a 1-lot entry then conviction-scaled — that would 4× it, forbidden). Survival-confirmed: 2-lot gaps are always SOLO (fire only when DOT flat, can't stack onto a DOT trade), worst-case single gap SL ≈ -$300 (8.3× inside the -$2,500 gate); the 14 gaps are 100% WR in-sample and the lone D2D -$153 loss is an OVERLAP trade, not in the gap set. The modelled $92,567 already sized gaps 2× → adopting 2× made $92,567 the modelled canonical with no correction; the subsequent BUILT gap-path engine refined this to $92,347 (Auditor-ratified 2026-07-17; ~$220 admission-timing displacement, immaterial). $92,347 is the final canonical.

### CROWN JEWEL — full system (BOOK-50 + S.20 + D2D conviction + D2D gap-filler)
**RATIFIED BUILT-SYSTEM CANONICAL (Auditor PASS 2026-07-17): 2,698 trades, WR 92.3%, PF 6.40, net $92,347 (vs DOT-alone $89,432 → +$2,915 / +3.3%), daily worst-day -104.4 (improved from -153.7), daily max-DD -145.9, 6/6 folds (min fold PF 5.39), OOS PF 6.96 / net $29,190.** (The modelled $92,567 was superseded by the built gap-path engine — ~$220 admission-timing displacement, immaterial; $92,347 is canonical.) Additivity note (roles INTERACT via shared jar + short conviction, so combined verified DIRECTLY not summed): Role 2 conviction +$1,015, Role 1 gap +$1,900; the two roles fire on different bars (conviction up-sizes DOT trades that ARE trading; gap fires only when DOT is flat). Survival IMPROVES on every axis. Combined worst-day 25× inside the -$2,500 gate.

### THE 14-GAP CEILING (proven from BOTH directions — 14 is the honest number, not a limit to beat)
Chasing more gap-fills was tested exhaustively and 14 is optimal:
- LOOSER (ADX 30→25→20→15): gap count rises 14→158 but net-added FALLS +$3,084→-$342 and worst-day DEEPENS -100.9→-166.2 (worse than DOT-alone). The gap-gate protects against stacking, NOT against a loose gap trade taking a full SL — more low-WR gap trades = more losing days. Rejected.
- TIGHTER / SPLIT-GATE (shorts on Micro_Rejection instead of Hurst): cuts gap-fills 14→7 (shorts 10→3), net-added FALLS $3,084→$2,968. Reason: the 14 gap trades are ALREADY 100% WR — a loser-removal conditioner has nothing to remove and only throws away winners. Rejected.
- 14 is the genuine count of high-quality DOT-flat strong-trend D2D moments in ~5 months. Both looser and tighter make it worse. Do NOT chase a round number (30); 14 clean beats 30 fragile.

### BANKED LEAD (not adopted, filed for later): Micro_Rejection lo as a broad D2D-short conditioner
On the BROAD D2D pool (ADX>=15, real short losers present), `Micro_Rejection lo` (clean-flip decisiveness — low counter-wick = confirmed breakdown, no fakeout) is a genuine short-side conditioner Hurst can't provide (Hurst is long-biased): lifts D2D shorts 84.5%→94.7% WR, PF 1.39→6.98, 16/19 independent of Hurst, OOS-positive, coherent mechanism, corroborated by Lower_Wick lo. **Promising, mechanism-backed, NOT proven** (n=19, ~6 losses erases it, did not clear strict Bonferroni across 360 tests). Useless in the gap slot (already 100% WR) — belongs to a future broad-D2D-short use-case if ever run outside the pristine slot. Needs fresh-export confirmation before sacred.

### CORRECTIONS RECORDED
- Tradeable window is ~5.0 months (first scannable bar 2026-01-26 20:54 after the 6900 InitBars warmup; first book trade 2026-01-27 01:25; first D2D trade 2026-01-29 → Jun 25), NOT 5.2 or 6. Prior "6 months" mislabels corrected.
- WARM-UP GUARD BUG (found + to fix in build): the gap-single masks (Hurst/FailedBreak/D2D) skip the 6900-bar InitBars floor the book signals respect — 6 gap-singles leaked into warmup (entry bars 1195-6900). Tiny (6 of 2,691) but a real defect: ALL gap entries must inherit the same InitBars guard. DOT-alone with the guard applied = $89,432 (-$55 from the 6 removed leaks).

### STATUS
ADOPTED design decision (human-confirmed). D2D promoted from directional-gate-only to a THREE-role member of the DOT system: (1) directional gate (existing), (2) conviction 2× sizer on book trades both directions (Role 2), (3) gap-filler standalone entries (Role 1). Pending: blueprint (conviction-stacking precedence table, gap-priority, warm-up fix, per-bar order) → pack scripts represent D2D's two roles for the blind audit → Developer EA build → docs propagation → full pipeline audit. `D2D_Signal`, `D2D_Trend_Dir`, `ADX_Value`, `Micro_Hurst` are all in the 171-column baseline, so Role 2 is pure per-bar column logic; only Role 1 (gap entry) needs its own zero-position gate. BLUEPRINT REV 2 SIGNED OFF (2026-07-17): the six-section integration logic is build-ready — conviction precedence table (higher-mult-wins across Hurst/recentFB/D2D; D2D the only short source), D2D-gap FLAT 2 LOTS (bypass-conviction, no 4×), gap-firing priority (D2D-gap highest, one gap/flat-bar), warm-up InitBars guard on all three gap sources, full per-bar order, and the one structural build item: `short_mult` support in portfolio_simulation_engine.py + conviction.py (both longs-only today — without it D2D's short conviction is silently dropped). Crown jewel BUILT + Auditor-RATIFIED 2026-07-17 at $92,347 (built gap-path engine; modelled $92,567 superseded, ~$220 displacement); both build-guards proven (short_mult live — book shorts sized 2×; D2D-gap flat-2-lot bypasses conviction, no 4×); oracle/wf/core byte-identical. The pack now scores the full committed design (BOOK-50 + S.15 + S.12 + S.19 + S.20 + D2D). This is the founding concept earning its full place — not just a trend gate, but a conviction source and gap-filler. Diamonds, not curve-fitting.

## 2026-07-16 — CONVICTION SELF-SCALING + GAP-SINGLE SYSTEM ADOPTED (F13 single-variable scan → two D2D conditioners → the "G" configuration)

A single-variable extremes scan (F13, standalone, separate from the F0-F12 convergence families) was run to hunt a lone-variable "Heart of the Ocean" (100% WR + 100% persistence). Clean NEGATIVE on standalone diamonds — no single variable reaches book-level WR at full span; convergence is confirmed necessary. But the scan surfaced TWO genuine full-span single-variable persisters that became D2D CONDITIONERS, not standalone trades:
- **Micro_Hurst:hi (R/S Hurst exponent, trend-persistence meter)** — high Hurst at a book-LONG entry = that long RUNS FURTHER (OOS PF 16.6 vs 6.6, avg-win +47.5 vs +24.0). A winner-SIZE effect, not a WR effect. Long-only (no short edge: OOS PF 2.22 vs 4.99).
- **Micro_FailedBreak:hi (failed-breakout/rejection meter)** — a failed break-down that reverts up; book longs firing within 1-5 bars AFTER a FailedBreak-extreme run OOS PF 12-24 vs ~6. A reversion-long confirmer. NOT a leading predictor of the D2D flip (that claim was ~10% hit rate / ~90% false-positive — rejected).

### THE ADOPTED SYSTEM — "G" (full bells and whistles), 1-LOT BASE
The EA scales ITSELF by lot multiplier only where probability is highest, rather than blanket-doubling. Three mechanisms, sharing the one 6-lot live-risk jar:
1. **Conviction sizing (modifies BOOK-50 lot size):** at a book-LONG entry, read Micro_Hurst on the signal bar; if > adaptive p90 (oracle rolling-2500 day-refreshed, typical value ~0.521; book-median ~0.418) → 2.0 lots, else 1.0. LONGS ONLY (shorts always 1.0). PLUS recentFB: book longs within 5 bars of a Micro_FailedBreak-extreme → x1.25. A long that is both takes the HIGHER multiplier (x2), never the product.
2. **Gap-only singles (NEW entries, fire ONLY when zero Dots positions open — even a BE'd one blocks):** Hurst single = Micro_Hurst > p97 (~0.550) AND D2D_Trend_Dir == +1 → LONG 1 lot, LOCK=3 (fires 43x/6mo); FailedBreak single = Micro_FailedBreak > p90 (~0.664) AND D2D_Trend_Dir == -1 (counter/reversion) → LONG 1 lot, LOCK=3 (fires 274x/6mo). Gate: ADX>=15 & Volume>=300 (solo). Gap-singles are 1.0 lot (not conviction-sized; the loose LOCK=3 is what lets their fat-tail winners run).
3. **Per-bar sequence:** STEP 1 book signal qualifies → open through ratified S.7/S.12/S.19 exits, if LONG & Hurst>p90 → 2 lots else 1; STEP 2 only if NO book signal AND zero Dots open → check the 2 gap-singles; STEP 3 gap-single BLOCKED while any Dots position is live.

### RESULT (final book, momentum-SL active, 1 lot, true $1/pt)
G (Hurst x2 + recentFB x1.25 + gap-singles), PACK-ENGINE HONEST REPRODUCTION: 2,691 tr, WR 92.2%, PF 6.15, net $89,487, worst-day -153.7, 6/6 folds. (The earlier MODELLED $90,103 / 2,828 tr / worst-day -147.2 carried a jar-sharing artifact in the research harness — it double-counted book trades, book 2,511 WITH gap-singles > 2,361 flat, impossible under a shared jar. The pack build surfaced it: gap-singles share the one jar and only DISPLACE book trades, so the honest total is $89,487, a 0.7% correction that changes no conclusion. $89,487 is now canonical.)
Trade population (honest): book x1.0=1,339, Hurst x2.0=210, recentFB x1.25=809, gap-Hurst=48, gap-FailedBreak=285. Option map (all pack-engine): A flat $58,277 / B Hurst-only $66,434 / G' recentFB-off $84,554 wd -127.5 / G all-on $89,487 wd -153.7.
Reference configs (all on the correct final book): A flat = $58,249 / OOS 6.99; B Hurst-x2-sizing-only = $66,407 / OOS 7.63 (book pure, OOS strengthens); G' (recentFB dropped) = $85,134 / worst-day -120.9 / OOS 5.94 (tail-cleaner); E crude bolt-in of standalone singles = $87,858 but OOS 5.42 / worst-day -187.9 (REJECTED — degrades book OOS below flat).

### RATIONALE (the design decision)
Conviction SELF-scaling at 1-lot base is SAFER than blanket-2x. Blanket-doubling the $58K book → ~$116K but DOUBLES every worst-day/drawdown/stacked-day indiscriminately (worst-day -127.5 → -255). The conviction system captures $89K SELECTIVELY — the x2 lands only on the 95.7%-WR high-Hurst longs (absent from the worst day entirely) and the gap-singles fire only when the book is flat (physically cannot deepen a stacked day). Net +54% while worst-day barely moves (-127.5 → -153.7, ~16x inside the FTMO -$2,500 gate). The risk-management IS the product: the EA presses where the edge is strongest, stays flat otherwise, base lot never changes.

### LOT-EXPOSURE (the survival check — operator was sensitive about concurrent lots)
Live-lot frequency (G, active time = the ~7% of bars with any position open): <=2 lots 88.2%; >6 lots 2.1% (221 bars/6mo); 10-12 lots 0.057% (6 one-minute bars in 6 months). Max ever = 12 lots ($1,800 theoretical). It NEVER bit — realized worst-day held -153.7, nowhere near $1,800, because the multiplied trades are 95.7%-WR isolated Hurst longs that almost never all stop together. At 1-lot base the worst-case 12-lot stack (-$1,800) is INSIDE the -$2,500 gate. (G' without recentFB: >6 lots only 0.6% / 64 bars — recentFB is what fills the >6-lot zone 3.5x more often; it is the tail-widener, dropped in G'.)

### DEPLOYMENT CONSTRAINT (sacred)
This system deploys at **1-LOT BASE ONLY**. The conviction multiplier IS the scaling mechanism — do NOT blanket-2x on top. At 2-lot base the x2 trades become effectively 4 lots and a 12-lot peak → 24 lots × $150 = -$3,600, OVER the gate. If 2-lot scaling is ever wanted, it requires the jar-tweak (count multiplied lots toward the 6-cap, hard-bounding real exposure at 6 lots) — a separate change, measured first. Until then: 1 lot, let the EA self-scale.

### STATUS
ADOPTED design decision (human-confirmed). Sacred registry S.20. Pending Stage-9 implementation + full pipeline audit (the quant MODELLED G via its analysis harness; the ratified-engine implementation of conviction-sizing + gap-singles + the pack scanner is still to be built and audited). recentFB is the max-net component AND the tail-widener — G ships it at 1-lot base (bounded); G' (drop recentFB) is the recorded tail-lighter alternative, one flag away. E (standalone bolt-in) REJECTED on OOS degradation.

### CLOSED: F13 SINGLE-VARIABLE EXTREMES INVESTIGATION
Standalone single-variable diamonds: clean negative (no 100%/100%; convergence necessary — proven, not assumed). The two persisters (Hurst, FailedBreak) live on as the S.20 conviction conditioners above. Bug caught during the scan: first pass reported 126 "stars" that were coincidences (total_firings counted raw condition-true bars up to 47,977 instead of actual entries; median 2 trades); the persistence co-factor (full-span presence required) collapsed all 126 — validating the persistence-co-equal-with-WR discipline. Scan is documented + standalone in the pack (results_F13_single_variable_extremes.csv).

---

## 2026-07-14 — PRE-BUILD RE-SCAN: three ratified changes (live-risk jar, per-bar execution, TM confirmed optimal); all remaining research CLOSED

A full pre-build re-scan of every assumption behind BOOK-50 (gates, losers, luck, trade management, concurrency, execution model). Outcome: three ratified changes, one bug caught, and every open research thread closed with mechanism. BOOK-50 signal roster UNCHANGED throughout (48 F0 + 2 F1). The re-scan confirmed the system sits at a genuine optimum — every attempted loss-reduction also cuts winners, because losses and wins live in the same structure.

### RATIFIED CHANGE 1 — 6-LOT LIVE-RISK JAR (replaces the MAX_POSITIONS=6 count cap)
The count cap counted BODIES; the jar counts RISK. The jar holds 6 lots of LIVE (pre-break-even) risk; each trade = 1 lot = 1 slot; a new signal opens (1 lot) ONLY when live lots < 6; when a position reaches break-even its lot LEAVES the jar and frees a slot. Hard bound: 6 live lots, never exceeded. Rationale: 6 already-profitable (BE'd) positions are not risk and should not block a 7th signal — the operator's insight, confirmed. Validated (1 lot, RUN TM): 2,409 trades (+74 vs count cap), net $58,685 (+$1,418), PF 5.83, WR 91.9%, worst-day -$127.5 (identical), max-DD -$165.6 (identical), 6/6 folds, 22/22 weeks, OOS PF 6.57. Strict improvement at the identical hard bound. EA needs a running live-lot counter (+1 open, -1 at BE, guarded so it fires exactly once; a post-BE close does NOT decrement). Sacred: S.15. Tail scales with lots: at 2 lots peak = 12 lots = -$1,800 theoretical (inside the -$2,500 gate); realized worst-day at 2 lots = -$255; live lots are 1-2 for 83% of time, hit 6 only 2.3%. At 3 lots respect cap x lots <= 16 (drop the effective jar).

### RATIFIED CHANGE 2 — PER-BAR EXECUTION MODEL (reverts the EA from per-tick SL management)
The EA currently repositions the SL PER-TICK; the validated book is PER-BAR. Ratified: the EA repositions the SL ONLY on a NEW BAR, off the CLOSED bar's High/Low (arm/tier/trail on closed-bar extremes, not the live Bid, not the Close); per-bar sequence = exit-check -> tiers -> BE-arm -> trail, reproducing portfolio_simulation_engine.py. CRITICAL: the broker still holds a HARD SL that executes intrabar on any tick — downside protection is always live per-tick; only the EA's REPOSITIONING is per-bar. WHY: with LOCK_FRAC=1.0 the lock sits on the tier-1 price, so per-tick repositioning places the SL on current price the instant it arms and the next adverse tick scratches it — collapsing runners (387->102) and draining net into an unvalidated bracket ($33.9k worst vs $58.7k per-bar). Per-bar management is the third parity leg (with export=live and jar-parity) — what makes live == the validated book. This was a genuine parity fix caught before build: the EA as-is would NOT have matched the backtest. Sacred: S.16.

### RATIFIED CHANGE 3 — TM CONSTANTS CONFIRMED SWEPT-OPTIMAL (no change; confirmed not inherited)
Full S.7 TM parameter sweep: initial SL risk_mult 2.0, MAX_RISK 150, trail STEP_PCT 0.30, base lag 2, runner lag 3, momentum threshold 0.00012, BE_TRIG_FRAC 1.0, LOCK_FRAC 1.0 — all on their optima; every deviation costs net or breaks the survival bound. Micro_LogReturn confirmed the best runner variable (8 tested). LOCK_FRAC swept independently (0.4-1.0, BE_TRIG held 1.0): 1.0 monotone-dominant — a looser lock (the operator's original 0.5 breathing-room design) rescues some worked-then-failed trades into runners (+$6.8k) but taxes the ~1,600 BE exits far more (-$19.4k), net -$12.9k, and 0.4 breaks persistence. Sacred: S.17. BUG CAUGHT: an earlier 'fantastic BE' result (PF 10.35, worst-day -38) was a lock-scaling artifact (BE trigger moved but lock left at full step, booking profit never reached); on fix it INVERTED (tightening halves net). A too-good number is a bug until proven.

### RATIFIED CHANGE 4 — MOMENTUM-CONDITIONAL WIDER INITIAL SL (S.7 change f)
On MOMENTUM entries (Micro_LogReturn x dir >= 0.00012 — the SAME gate as the runner lag, S.12), the initial stop widens to min(ATR x 4, 150); non-momentum entries keep min(ATR x 2, 150). The $150 MAX_RISK cap is UNCHANGED and INVIOLATE (no trade risks more than $150+spread); this only lets momentum entries widen UP TO the existing cap (mean stop $86 -> $131; ~54% pin to $150). This does NOT overturn S.17's risk_mult 2.0 — 2.0 stays the base, this adds a momentum branch (same architecture as the runner's base-lag-2/momentum-lag-3). Validated (BOOK-50, jar, RUN TM): SL hits 194 -> 166 (-28), WR 91.9% -> 92.8%, PF 5.83 -> 6.12, OOS PF 6.57 -> 6.99, min-fold PF 5.01 -> 5.49, worst-day -$127.5 HELD, max-DD -$165.6 HELD, 6/6 folds, 22/22 weeks. Net $58,685 -> $58,249 (-$436, -0.7%). A QUALITY lift (fewer losers, higher WR, stronger OOS), NOT a profit upgrade. Runner interaction confirmed clean (LF count/net held). RATIONALE (the through-line of the whole re-scan): 'room helps, taking room hurts' — this is the ONLY loss-reduction measure that held net. Every alternative failed by cutting or scratching winners: entry-gate tightening (loses ~10 winners per loser), price-action loser filters (remove more winners than losers), the two-step BE (WR 'improvement' to 93%+ was a mirage — it scratches ~800 winners to $0, net -$21k; the corrected banked-interim version still costs $14-19k net for the halved worst-day). Chosen dollar-ceiling V=150 (widen up to the already-safe $150 cap). Sacred: S.19. It is an S.7 change requiring full pipeline; the operator ADOPTED it for the OOS/quality lift. EA-CRITICAL SPLIT CONVENTION (discovered during the Stage-8 engine merge, verified by book reproduction): the widened ATR x 4 is the CATASTROPHE STOP ONLY; the BE-arm trigger and LeapFrog step_size stay on the BASE scale (base_risk = min(ATR x 2, 150)). This split is what converts would-be stop-outs to break-evens (SL 194 -> 166) while HOLDING worst-day -$127.5. Scaling BE/step off the widened ATR x 4 risk is WRONG — the Developer's first merge attempt did exactly that and worst-day blew to -$320 with SL rising to 202; the diagnostic (runner-only held -127.5) isolated the SL as the culprit and the base-scale-BE convention reproduces the record exactly. The Stage-9 EA MUST implement this split (base_risk computed separately for BE/step, catastrophe risk for the SL) or the live EA will not match the book. Recorded in S.19 and the four build-mechanics DOT docs.

### DEVELOPER TASKS QUEUED (engine + program map)
1. ENGINE FIX: portfolio_simulation_engine.py admission must implement the live-risk jar — replace the body-count guard (len(active_trades) >= MAX_POSITIONS) with a LIVE-lot count (active_trades where be_nudged is False; admit when < 6). Derive live_lots from be_nudged each bar (no mutable counter, no double-decrement). Confirm the trade-management pass (sets be_nudged) runs BEFORE admission so a same-bar BE frees a slot. Reproduces the jar book (2,409 tr, $58,685). This is needed so any FUTURE discovery run scores the jar concurrency, not the old count cap. Sacred S.15 change, full pipeline. (The momentum-SL S.19 and per-bar S.16 must likewise be reflected in the engine for future-run parity.)
2. PROGRAM MAP: Developer to produce DOT_stage8_program_map.md — a full instruction manual for the stage8_discovery pack (directory, every script's purpose/inputs/outputs, pipeline order, exported artifacts + schemas, run-from-scratch runbook, and the note that the engine must carry the ratified jar/per-bar/momentum-SL/swept-TM so future data reflects real behavior). So future diamond-hunts can be re-run from scratch and trusted.

### CLOSED RESEARCH THREADS (all investigated to completion with mechanism)
- **Entry gates** (ADX>=15, vol>=50 concurrent/>=300 solo, D2D dir): tested and CONFIRMED optimal. Losers are statistically indistinguishable from winners AT ENTRY (ADX 23.4 vs 24.3, vol 317 vs 346, depth 2 vs 2) — no gateable condition. Every gate change cuts ~10 winners per loser (L:W ~0.1). Solo>=300 floor confirmed (loosening to 250 adds junk, PF->4.41).
- **Post-entry failure signature** (the operator's 'track the turn' hypothesis): a REAL signature exists (failing trades show collapsing D2D_Persist/OBVf_Persist, drift from KAMA, momentum rollover; D2D-flip-hold catches 33% of failers at 8% recoverer cost). BUT early-exit on it is NET-NEGATIVE by dollar geometry: it fires late (SL already close, saves ~22pts) and its false-positives are recovering winners incl. runners (cost ~79pts each). Net -6,053 pts, OOS PF 6.54->3.3-4.8. Also tested as a TM MODULATOR (adaptive BE/trail/sizing): a signal-blind faster-BE control matched it, so the health signal adds nothing. SL is the sole reliable stop, confirmed at the level prior passes skipped.
- **Luck / null test**: BOOK-50 is BEYOND the max of every null (direction-shuffle, random-timing, fully-random; 200 iters each; 0/200), 16-35 sigma above null means, p<0.005. TM-alone on random entries LOSES money (-$678, PF 0.97) — the edge is the SIGNAL, not the TM. Combined with OOS strengthening, statistically distinguishable from luck. (S.18)
- **Concurrence-depth quality gradient**: depth is a REAL, causal (known-at-entry) monotonic quality predictor — solo WR 89.6%/PF 4.10 rising to depth-8+ 100% WR/zero losses; all 196 losers at depth 1-7, 89% at depth 1-3. But depth-conditional SIZING fails: the deep stacks are same-direction correlated (form in one-way regimes), so up-sizing them is a correlated-reversal landmine (-$10k to -$17k tail). The live-risk jar is the correct way to take the deep trades (uniform 1 lot, risk-bounded) — capture them, don't lever them. Double-BE / per-tick pre-tier cushion also tested, fails (cuts winners).
- **Position cap raise**: at 1 lot raising to 10-12 adds +7-9% net, but the tail scales with lots — at 2-3 lots it breaches the FTMO gate (cap x lots <= 16). Superseded by the jar, which captures the benefit safely at the same hard bound.

### DEPLOYMENT NOTE (lot sizing)
Operator intends 2 lots. At jar-6 that is a 12-lot theoretical peak (rare: 2.3% of time), -$1,800 theoretical tail (inside gate), -$255 realized worst-day. Recommended path: START at 1 lot for the challenge (target easily cleared, 6-lot peak, -$900 tail), confirm live == sim on a real challenge account, THEN scale to 2 lots once fills are proven. Survival-first: prove small, scale on evidence. Jar can drop to 5 at 3 lots if a lower peak is wanted.

### STATUS
All research objectives CLOSED. The system is at a genuine optimum (every loss-cut also cuts winners). BOOK-50 engine + momentum-runner TM + sequential-latch + 6-lot live-risk jar + per-bar execution model + momentum-conditional wider initial SL = the complete committed design. Full DOT document set (11 docs, DOT_ prefix) and all three non-negotiables (sacred registry S.1-S.18) updated and frozen. Next: pre-build gates (17d) then Stage 9 build (18).

---

## 2026-07-12 (FINAL — ENGINE FROZEN) — BOOK-50 LOCKED: the structure-complete engine, all 8 market structures covered

Supersedes the 2026-07-11 BOOK-35 lock. The engine grew 35 -> 40 -> 58 -> trimmed to 48 -> +2 structure fills -> BOOK-50, each step validated on the tic-proof dataset. BOOK-50 is the final frozen engine. Full arc below.

### The progression (each step data-driven, OOS-disciplined, on signal_full_records.csv + per-day vectors)
- **BOOK-35** (upgraded diamond, 20L/15S): RUN TM PF 6.57, net $41,239, worst-day -$81, OOS PF 5.91. The locked entry core.
- **BOOK-40** (+5 within-vocabulary co-firers, 25L/15S): investigation of the nq>=3 concurrence elite bars (PF 10-13, WR 95-96%, nq=4+ positive worst-day) found 88 persistent-but-excluded triples concentrating there. Adding 5 decorrelated co-firers (BREADTH not leverage): RUN PF 6.20, net $45,404, worst-day -$77.8 (best), OOS PF 6.54. Breadth beat leverage (sizing-up nq>=3 gave +net at deeper worst-day -126; breadth gave +net at BETTER worst-day).
- **BOOK-58** (+18 structure-balanced fillers, 41L/17S): the +18 PROVED their future-proofing in the actual March -10% war crash — bear net $16,460 (+47% vs BOOK-40's $11,209). Persistent (6/6, 22/22) but lower per-trade quality (PF 5.00, OOS PF 5.05). Net $65,200 (+57%). NOT bloat — the operator was right that they carried the bear.
- **BOOK-48** (leave-one-out trim of BOOK-58, 35L/13S): removed the 10 worst book-level ERODERS (several had high standalone PF — 9.3, 9.9, 21.7 — but loss-correlated dead weight; bear-net contributors PROTECTED). RUN TM: WR 91.9%, PF 5.94, net $55,562, worst-day -$127.5, max-DD -$165.6, 6/6 folds, 22/22 weeks, all 5 weekdays, min-fold PF 5.00, **March-bear PF 5.76 (highest of any book), OOS PF 6.65 (HIGHEST OF PROJECT), OOS WR 92.4%.** The peak-OOS-PF object. Structure coverage 6/8 (SQUEEZE_BREAKOUT + TREND_EXHAUSTION absent).
- **BOOK-50** (+1 squeeze +1 exhaustion, 37L/13S): FINAL. Fills both missing structures at the F0-grade persistence bar. See below.

### BOOK-50 — THE FINAL FROZEN ENGINE (RUN TM, true $1/pt)
- 50 signals (37 LONG / 13 SHORT). WR 91.7%, agg PF 5.78, net $57,419/6mo @1lot.
- Worst-day -$127.5 (HELD from BOOK-48, 19x inside -$2,500 gate), max-DD -$165.6 (HELD, 60x inside -$10,000 gate).
- 6/6 monthly folds, 22/22 ISO-weeks, all 5 weekdays positive, min-fold PF 5.15.
- March/bear PF 5.23, bear net $13,698. OOS PF 6.54, OOS WR 92.0%, OOS net $18,823, OOS worst-day -$127.5.
- **STRUCTURE COVERAGE 8/8 — nothing absent:** TREND_CONTINUATION 15, MOMENTUM_IGNITION 14, BREAKOUT_EXPANSION 8, STRUCTURAL_ENTRY 6, PRICE_ACTION 4, SQUEEZE_BREAKOUT 1, TREND_EXHAUSTION 1, VOLUME_CONFIRMED 1.

### The two structure fills (F1-grammar, each clears the persistence + clean-decorrelation bar)
- SQUEEZE_BREAKOUT: `Sqz_Val:hi ->13-> Micro_OrderFlowDelta:lo` [LONG] — 33 tr, WR 93.9%, PF 9.69, 6/6 folds, 20/20 weeks, 5/5 weekdays, streak 1, March +$184. In-book: +$1,002 net, worst-day HELD -127.5, OOS PF 6.65->6.52. F0's own 19 SB triples were rejected earlier (blew worst-day to -$221) — F1 fills this cleanly where F0 structurally could not.
- TREND_EXHAUSTION: `ADX_Rising:==0 ->8-> D2D_DirStep:==-1` [LONG] — 42 tr, WR 92.9%, PF 4.75, 6/6 folds, 18/21 weeks, 5/5 weekdays, streak 1, March +$379. In-book: +$855 net, worst-day HELD -127.5, OOS PF 6.65->6.66. CRITICAL: it's a LONG exhaustion (ADX flattening + D2D down-step = reversal-long), so in the bull-heavy tape its losses do NOT stack counter-trend. Short-exhaustion candidates were REJECTED (deepened worst-day to -$192). The discipline held: persistence alone wasn't enough, the worst-day hold was the deciding gate.

### CLOSED RESEARCH THREADS (all investigated to completion, engine frozen throughout)
- **D2D reversion early-warning** (flip-before-the-flip): REFUTED. Convergence leads D2D flips ~10-20 bars, but as a target it destroys the book (PF 2-3.5), as a gate it cuts good trades. The D2D gate confirmed doing real protective work. A freshness gate (bars-since-flip<=80) improved OOS PF but cost real net -> REJECTED (money-for-stat, not worth it).
- **Concurrence/depth**: the nq>=3 elite edge is the NAMED TRIPLES co-occurring, NOT raw depth. Decisive test: high-depth bars where NO named triple fires (nq=0) are net-LOSING (PF<1, loses in March). The original '89%' does NOT reproduce at true $1/pt (WR 76%, PF 1.1) — it was the small-sample/100x artifact. Depth as a conditional layer collapses the book (OOS PF 6.65->2.9). CLOSED with mechanism: depth is where the triples gather, not an independent signal.
- **Market-structure balancing (wholesale)**: forcing structure diversity via the full field FAILS survival+OOS (the other-structure triples don't loss-decorrelate). But the disciplined 1-per-structure fill (BOOK-50) succeeds where wholesale failed.
- **F1 family (full disciplined treatment)**: F1 has 1,267 persistent signals but is WEEKLY-INTERMITTENT (burst-fires around ST_Flip anchors; only ~20 hold all 3 scales). F1-standalone book survives (PF 2.3-2.7) but is a weaker parallel engine. F1 as wholesale breadth on BOOK-48 LOWERS OOS PF (dilutes). VERDICT: F1 is real+persistent but lower-quality — redundant for net (more F0 does it cheaper/safer at better worst-day), EXCEPT it uniquely fills SQUEEZE_BREAKOUT and TREND_EXHAUSTION (structures F0's same-bar trigger structurally cannot decorrelate into the book). Only the 2 structure-fill signals earn a place; the rest of F1 is documented tested-and-excluded.

### DOCTRINE CONFIRMED
- Persistence at three scales (folds + weeks + days) is the PRIMARY gate; nothing enters that isn't as persistent + clean-decorrelating as the F0 core. Structure coverage is taken only when it doesn't compromise persistence or worst-day.
- 'Breadth beats leverage' (add decorrelated signals at 1 lot; don't size up). 'High standalone PF != book value' (loss-correlated signals are dead weight; leave-one-out finds them).
- Loss-decorrelation is the selection method; survival (worst-day) is no longer binding (19-30x gate headroom) so net + OOS PF + bear coverage + structure completeness are the discriminators.
- BOOK-48 remains the peak-OOS-PF object (6.65); BOOK-50 gives up 0.11 OOS PF (->6.54) for full 8/8 structure coverage at ZERO worst-day cost. BOOK-50 is the chosen final engine (structure-complete future-proofing).

### TM: momentum-conditional runner (RUN) carried forward
Rule: v=Micro_LogReturn*dir; if v>=0.00012 LeapFrog lag=3 else lag=2 (runner trail only; all else S.7; no look-ahead). Validated (4 stress tests). All BOOK-50 stats above are on RUN TM. Applied at EA-build time through the full pipeline.

### VALIDATION SUMMARY (why BOOK-50 is real, not curve-fit)
OOS strength (clean diamond 5.54, BOOK-40 IS-selected 6.54 — held/strengthened on unseen data); near-zero loss correlation (0.03-0.13) making worst-day 19x smaller than the gate; full PF sustained through the real March -10% war crash; persistence locked at 3 scales; leave-one-out trim that discarded high-standalone-PF loss-correlated signals; nulls found and honored (depth PF<1, flip-trajectory AUC 0.492). Note: BOOK-48/50's exact OOS figure carries mild optimism (the trim/fills saw the full sample); the CLEAN OOS anchor is the diamond 5.54 and BOOK-40 6.54. The added structure signals independently hold 6/6 folds + weekly + weekday persistence measured on the full sample, unaffected by the OOS split.

### Standing caveat (until live)
Absolute $ carries M1-OHLC backtest optimism (intrabar fills, concurrent stacking); live slippage trims absolute $. The SHAPE (WR/PF/persistence/OOS-stability/decorrelation/structure) is the promise and is robust; exact $ confirm on demo. The demo is the final test — the normal boundary between validated backtest and live capital.

### NEXT (Stage 9 — EA build, engine now UNFROZEN for the build)
Install BOOK-50: 50 signals (48 F0 triples + 2 F1 sequential structure-fills) + S.7 base TM + momentum-runner upgrade, through the full pipeline. The F1 additions require a bounded sequential-latch subsystem (2 ring buffers <=15 bars on the Sqz_Val + ADX_Rising latches, lagged lookups, reusing the already-tracked ST_Flip anchor) — NOT a second engine. Bundle the !IsTesting() L356 OnInit guard fix. Auditor confirms exact live numbers; regenerate record docs with confirmed figures. Then execution-parity re-verify; one-week demo (demo==sim); live at minimum lot on one FTMO account; scale to the other two only once live matches sim.

---

## 2026-07-11 (FINAL) — OBJECTIVE MET: upgraded 35-signal diamond LOCKED as final engine + momentum-runner TM upgrade VALIDATED

This supersedes the working conclusions earlier today. The diamond is confirmed and locked; the units bug is corrected; the TM has a validated upgrade. Net of a long, error-prone session (recorded honestly below for future instances).

### Tic-proof full-field re-analysis (the artifact that ended the error class)
A new standalone analysis engine — `analysis_engine.py` + `run_full_analysis.py`, F0-only, Auditor-signed-off, in `engine/` — re-scored ALL 51,311 F0 candidates through the ratified S.7 TM at TRUE $1/pt scale, exporting a 68-column per-signal record (6 monthly folds + ISO-weekly + day-of-week + exit distribution + risk texture) plus per-day P&L vectors. Outputs (in `discovery_results/`): `signal_full_records.csv` (2,420 gated survivors: trades>=30 AND agg_pf>=2.0 AND folds_plus>=4) and `signal_per_day_pnl.jsonl`. Purpose: selection now READS pre-computed numbers; every signal resolves against source; worst-day / loss-correlation / any book's in-book behaviour is a LOOKUP on saved vectors, never a re-simulation. This directly closes the failure mode that dominated this session.

### Three-task validation over the 2,420 field
- Task 1 (finer lens on the 35): LONG 20/20 clean (fire all weekdays, no negative weekday, streak 1, week-positivity >=89%). Exactly TWO shorts daily/weekly-fragile — #31 `Micro_BarEntropy:lo+Micro_Rejection:lo+Round_500_Dist_ATR:lo` (Mon net-neg) and #34 `Bar_Range:hi+Micro_VolAccel:hi+Body_Size:lo` (Fri net-neg, streak 2) — the SAME two the in-book fold reconciliation independently flagged at 5/6.
- Task 2 (full sweep): ~100 bulletproof signals (84L/16S); the roster longs rank top 3.5% of 1,873. The WR>=90 discovery gate wrongly excluded strong 80-90 WR shorts.
- Task 3 (climate-corrected decorrelation): sample is bull-heavy (4 up / 2 down months, +8.3%) so long dominance is the MARKET's signature — L/S balance is NOT a goal (forcing it overfits the single March bear month). Wider search set the ratio ~2.4-3:1 long-heavy and did NOT beat the diamond on all three OOS axes. The diamond is PARETO-OPTIMAL on (OOS PF, WR, worst-day). The properly-armed search CONFIRMED the diamond rather than replacing it, surfacing only the 2-short swap as polish.

### THE FINAL SWAP (both replacements verified in-file, 6/6 folds, no negative weekday)
- OUT: `Micro_BarEntropy:lo + Micro_Rejection:lo + Round_500_Dist_ATR:lo` (SHORT); `Bar_Range:hi + Micro_VolAccel:hi + Body_Size:lo` (SHORT).
- IN: `EMA_Oscillator:lo + Micro_Hurst:hi + ADX_Value:lo` (SHORT, PF 5.21, 6/6, Mar +$639); `Efficiency_Ratio:hi + Micro_BarEntropy:hi + Body_Size:hi` (SHORT, PF 4.63, 6/6, Mar +$452).
- Correction logged: the quant first mistyped the second swap-in as `Body_Size:lo` (does not exist in-file); verified real signal is `Body_Size:hi`. Numbers below reflect the corrected, verified re-run — a caught error, not a silent one (the tic-proof dataset is what caught it).

### UPGRADED 35 — FINAL ENGINE
- ENTRIES (locked): 35 F0 triple-convergence signals, 20 LONG + 15 SHORT, L/S ratio 1.37, D2D-gated.
- IN-BOOK on S.7 base TM (ratified figure, true $1/pt): WR 92.7-92.8%, PF 6.43-6.44, 6/6 monthly folds, 22/22 ISO-weeks green, net ~$40,218/6mo @ 1 lot, worst-day -$81.30, March/bear +$10,753 (per-fold Mar PF ~5.15). ~30x headroom under the -$2,500 gate; scalable.
- OOS (May-Jun, base TM): PF 5.54, WR 92.1%, worst-day -$81.30.
- Swap tradeoff logged: vs the pre-swap 35, it lifts full-period PF/net/WR/bear coverage and removes two confirmed-fragile shorts, at the cost of a slightly softer May-Jun OOS slice on the base TM — adopted because removing structurally-fragile signals future-proofs more than the slice give-back costs.

### TM UPGRADE — momentum-conditional runner (VALIDATED; applied at EA-build time)
- EXACT rule: `v = Micro_LogReturn * direction; if v >= 0.00012 then LeapFrog lag = 3 else lag = 2`. Widens the runner trail on high-momentum in-direction bars. Touches ONLY the LeapFrog runner trail — initial SL, BE nudge, tier logic, Friday close, and ENTRIES all identical to S.7. `Micro_LogReturn` is already computed live (no new indicator, no look-ahead).
- Stress tests ALL PASSED: (1) BROAD OOS plateau of (threshold, lag) — not a fitted spike; (2) beats/matches S.7 in ALL 6 folds incl. March bear; (3) survives +/-20% perturbation; (4) gain attributes to the intended mechanism (high-momentum runners going further = fatter LeapFrog tail).
- QUANTIFIED (research-expected, pending EA implementation + Auditor confirmation of exact live numbers): full 6-month PF 6.44 -> 6.57, net $40,218 -> $41,239 (+$1,021); OOS PF 5.54 -> 5.91 (+6.7%); WR and worst-day (-$81.3) unchanged; 6/6 held.
- STATUS: VALIDATED RESEARCH, not yet in the ratified engine. The four v2 record documents carry S.7-base as the ratified figure with the runner upgrade shown as 'Expected (research)'. Implemented when the EA is configured for the 35, through the full pipeline (Developer to spec; Supervisor + Auditor verify against source).

### THE 100x UNITS BUG — and the session failure mode it exposed (recorded for future instances)
`wf.py` carried `USD_PER_POINT_PER_LOT = 100.0` (marked OPERATOR-CONFIRM, never confirmed). Correct FTMO US30.cash = **$1/point per 1.0 lot** (operator live experience + web-confirmed). Fixed to 1.0; `points_to_usd = points * LOT_SIZE * 1.0`; no other 100x conversion in the toolchain. CONSEQUENCE: every USD worst-day/P&L figure in Stages 5-8 was inflated 100x. Real worst-days were 1/100th (e.g. the depth engine's '-$29k' was -$290; scale-in '-$111k' was -$1,110). MULTIPLE ENGINE CONFIGS WERE DISMISSED ON SURVIVAL GROUNDS THEY NEVER ACTUALLY BREACHED. WR/PF/folds/trade-counts were NEVER affected.
BEHAVIORAL RECORD (honest, for future instances): during this session an AI instance (a) told the operator '89% = be excited' on a proxy metric that did not survive full-TM scoring, then (b) swung to dismissing every candidate system on inflated negative worst-days, and (c) when the operator directly and correctly challenged how '-$111k at 1 lot' was even possible, RATIONALIZED the impossible figure ('1000 trades failing on one day') instead of interrogating it. The operator was right; the number was a units bug. The corrective doctrine stands reinforced: never conclude on first findings; when the operator's instinct contradicts the data, check the data before defending it; positive above-coin-flip results are not license for hype, and negative figures are not license for dismissal — verify units and mechanism first.

### OBJECTIVE MET
Most persistent (6/6 folds + 22/22 weeks + all-weekday), highest performance (Pareto-optimal vs the full 2,420 field), future-proofed (OOS-validated, bear-month profitable, market-written ratio, ~30x survival headroom) — on a tic-proof dataset where every number traces to a saved per-day vector. Upgraded 35 entries + momentum-runner TM = the final engine.

### Standing caveat (until live)
Absolute $ carries M1-OHLC backtest optimism (intrabar fills, concurrent stacking); live slippage trims absolute $. The SHAPE (WR/PF/persistence/OOS-stability/decorrelation) is the promise and is robust; exact $ confirm on demo.

### NEXT
- (Operator) further signal-discovery checks BEFORE development — EA frozen.
- Stage 9: install locked 35 entries + S.7 base TM + momentum-runner upgrade, through the full pipeline; Auditor confirms exact live numbers; regenerate the four record docs with confirmed upgraded-TM figures.
- One-week demo forward test (demo==sim); live at minimum lot; scale to the other two FTMO accounts only once live matches sim.

---

## 2026-07-11 — TWO MAJOR EVENTS: the DIAMOND (validated engine candidate) found, and a 100x UNITS BUG corrected that invalidates prior worst-day conclusions

### A. UNITS BUG (wf.py USD_PER_POINT_PER_LOT) — corrected, and it re-writes prior survival conclusions
`wf.py` carried `USD_PER_POINT_PER_LOT = 100.0` (marked 'OPERATOR-CONFIRM', never confirmed). The correct FTMO US30.cash spec is **$1 per point per 1.0 lot** (operator live experience + web-confirmed; P&L in this system is measured in full points). Fixed 2026-07-11 to `1.0`; `points_to_usd` now returns `points * LOT_SIZE * 1.0`; grep confirmed no other 100x conversion in the toolchain. One-constant change, verified in source, Auditor waived by operator (trivial).
**CONSEQUENCE — this is not cosmetic. Every USD worst-day/P&L figure in Stages 5-8 was inflated 100x.** All prior 'worst-day $-XX,XXX' readings were 100x too large; true values are 1/100th. Concrete corrections:
- The depth-K / 55-variable engine's '-$29k worst-day' was really **-$290**. Its '-$45k' was **-$450**. 
- The scale-in engine's '-$88k to -$111k' was really **-$880 to -$1,110**.
- The raw-depth Phase-3 sweep '-$27k to -$111k' was really **-$270 to -$1,110**.
- The F0-90 union portfolio '-$77k to -$110k' was really **-$780 to -$1,100**.
**Engineering conclusions were made on the inflated figures.** Multiple engine configurations (the depth-K threshold engine, the build-up entry, scale-in, various union portfolios) were down-weighted or dismissed as 'unsurvivable' on worst-day grounds when their REAL worst-days were 1/100th and, in several cases, comfortably inside the -$2,500 survival gate. The survival-first gate — the primary filter for the entire project — was being evaluated against losses 100x larger than reality. **Any prior rejection that hinged on worst_day_usd must be re-read at true scale before it stands.** (Note: WR, PF, folds_plus, and trade counts were NEVER affected by this bug — only USD-denominated figures. So persistence/PF-based findings hold; only survival/worst-day/P&L-magnitude conclusions need re-reading.)

### B. THE DIAMOND — first validated real-portfolio engine candidate (loss-decorrelation selection)
The breakthrough method: individual elite signals (folds_plus=6, WR>=90%) show PF 5-17, but naively combining ALL of them collapses to PF ~1.9 because their (individually rare) losses concentrate on a few shared tail-days (5 of 6 worst LONG loss-days are in the March bear fold). **Selecting a subset whose LOSING DAYS do not overlap** inverts the collapse and recovers the individual-signal PF class as a REAL portfolio.
**The recommended book — L20+S15 (20 long + 15 short elite triples), corrected to true $1/point scale:**
- 35 signals, 1,655 trades, **WR 92.7%, PF 6.22**, 6/6 folds
- Net P&L 6 months @ 1 lot = **~$38,158** (38,158 points); worst day @ 1 lot = **~-$78**
- 99 of 104 active days green; profit broad (top-3 signals = 15% of net; top-5 days = 22%; 40 days carry 80%)
- Exit mix BE 1,263 / LF 272 / SL 118; top-1 winner = 2% of gross (not runner-inflated)
- Survival: worst day -$78 @ 1 lot is trivially inside the -$2,500 gate; survivable at up to ~10 lots (-$778 worst day). Lot-sizing is no longer the binding constraint.
**OOS VALIDATION (the decisive test):** subset selected on Jan-Apr only, measured untouched on May-Jun. WR barely moved (92.6% -> 90.2%), OOS PF held 4.0-6.6 (vs all-signal 2.1). Worst-day is the least-stable metric IS->OOS, so production lot-sizing must be set off the OOS worst-day. **The selection generalises — it is not in-sample luck.**
**Composition is coherent:** the subset is distinct premium triples from the Task-1 vocabulary (LONG buy-the-pullback signature; SHORT flow-rollover signature), e.g. LONG `KAMA_Dist:lo + Micro_WickImbalance:hi + OR_High_Side:==-1` (100% WR, 32 tr), SHORT `Slope_Accel_LT:lo + Sqz_Val:hi + Micro_CSSpread:lo` (100% WR, 30 tr).
**Caveat (stated once):** the $38k dollar magnitude carries standard M1-OHLC backtest optimism (intrabar LeapFrog tier-sequencing + up-to-6 concurrent positions on trend days); live fills/slippage will pull absolute dollars down. The SHAPE (WR 92.7%, PF 6.2, breadth, OOS-stability, decorrelation) is what is robust.

### C. FRONTIER (corrected scale) — the frequency<->quality trade is strict and monotonic
Combined decorrelated L+S, 1-lot, true scale: L5+S3 ~4/day PF 16 worst-day ~+$6 (positive, but in-sample-fit at K=5) ... L20+S15 ~16/day PF 6.2 worst-day -$78 ... L209+S45(all) ~121/day PF 2.1 worst-day -$778. No high-frequency-AND-high-PF corner exists. Best risk-adjusted OOS-holding band = L20-L30 (PF 5-8). K=5 positive-worst-day is in-sample fit and does NOT hold OOS — do not headline it.

### D. WHAT THIS CHANGES GOING FORWARD
1. The diamond (decorrelated premium-triple book) is the leading engine candidate. Architecture = a selected book of proven elite triples, chosen for loss-decorrelation, D2D-gated, run through the ratified S.7 TM.
2. Survival-first still governs, but at TRUE scale the gate is easily met — lot-sizing is a tuning knob, not a wall.
3. Prior dismissals that hinged on worst_day_usd (depth-K 55-variable engine, build-up entry, scale-in) are re-openable at true scale — though the diamond currently dominates them on WR/PF/persistence regardless.
4. NEXT: lock the diamond via a clean production spec — final subset (sized off OOS worst-day), the exact signals, and forward to Stage 9 EA install; the depth/build-up engines can be re-evaluated at true scale if the diamond needs more frequency.

---

## 2026-07-11 — QRA Phase 1 complete; regime map corrected; PF-lives-in-TM established; Phase 2 defined

**Regime structure of the sealed baseline (corrected from OHLCV — the earlier 'one bull regime' framing was WRONG).** Monthly US30.cash close-to-close: Jan -0.3%, Feb +0.2%, **Mar -4.3% (bear leg, sample low 44,826 on Mar 30 — the US-Iran war leg)**, Apr +7.5% (V-recovery), May +2.5%, Jun +2.6%. So: Jan/Feb flat, March genuine bear-V, Apr-Jun bull. wf fold 3 = March = the bear fold. The 604L/98S survivor skew reflects this real regime mix, not a uniform bull.

**QRA Phase 1 census (all 12 families, one lens).** 6/6 persistence is a two-family phenomenon: F1 11,772 + F0 1,788 = 13,560 of 13,810 total 6/6 (98.2%); F2-F11 tail = 250 (F3 202, F11 21, F9 13, F7 8, F2 4, F5 1, F6 1, F4/F8 0). F0 carries the best quality within 6/6 (median PF 5.21, WR 91.7, worst-day -$7,700); F1 broader/lower (median PF 2.21, WR 83.8, wd -$13,500).

**F12 null (circular-shift, confirm-mode depth ladder, 20 cells): raw undifferentiated depth does NOT beat noise on agg_pf (min p=0.10, 0 cells pass).** EXPECTED and correct: counting all 117 unorganised conditions dilutes signal-bearing variables among irrelevant ones. This does NOT say concurrence fails — it says a raw count is the wrong measurement. Category-depth and composition were NOT null-tested; that is open.

**Concurrence is always present (measured).** For the 702 F0 6/6 >=30-trade winners, on the bars they fire a MEDIAN of 29 (long) / 21 (short) direction-aligned conditions are simultaneously at extremes (baseline medians 27/23). The triplet is a 3-variable SAMPLE of a ~30-deep simultaneous convergence. Concurrence works — the 51,311 PF>=2 triple-convergences are proof; the open question was always which variables cluster and how to organise them, never whether concurrence occurs.

**Winning-stack composition is structured and regime-dependent (measured).** When LONG winners fire, the same conditions dominate every time: Session_Low_Side==1 (99%), VAL_Side==1 (93%), AT_Regime_ST==1 (82%), Trend_Concordance==1 (80%), PoC_Side==1, AT_Regime_LT==1, ADX_Rising, D2D_ATR:hi. The CORE set is stable across regimes; the PERIPHERY reorganises — March(bear) recruits Harmonic_OBVf_Concordance; Apr-Jun(bull) recruits PrevDay_Low_Side. Per-fold PF confirms regime ownership: LONG concurrence weak Jan-Mar (0.88/1.06/0.97) then alive Apr-Jun (1.44/1.25/1.19); SHORT strong Feb (1.65), dead in the bull.

**KEY REFRAME (from the original DOTS_76 stats + backtest guide, re-read 2026-07-11).** The original's PF 13.55 / WR 96.9% does NOT come from signal selection — the project's own Opus 4.8 review states 'PF ~11-14 is a ceiling set by trade management, not signal selection.' Exit distribution: 819 BE + 221 LF + 33 SL, zero SL-wins, zero BE-losses. The edge lives in the BE-nudge + LeapFrog trailing exit; the signals supply directional entries at ~16.8 trades/day. IMPLICATION: the entry side does not need a frozen 76-triplet list — it needs 'a real convergence, D2D-directional, selective enough (~16 trades/day).' A composition-based concurrence gate expresses the same thing WITHOUT freezing the list, and fed into the same proven exit can target the same stats. This is why a concurrence engine is viable, and it removes the prior objection that concurrence 'only scored PF ~1.1' — that PF was diluted entry quality tested without matching the 76's selectivity; the TM is where the PF is made.

**DIRECTION (operator, locked):** (1) organise ALL variables by market structure and regime (variable-state -> directional meaning -> structure category -> regime); (2) determine the most persistent behaviours across all 12 families and how the data converges; (3) design a bar-1-close trade engine that fires on which variables cluster in which regime/structure; (4) understand the trade management and whether it can be improved; (5) build. NOT a frozen list — freedom for all 171 variables to self-cluster, with a validity threshold per regime/structure.

**Phase status:** Phase 1 done. Phase 2 = organise variables by structure/regime + derive the characteristic winning-composition per regime, then test SUFFICIENCY (does the characteristic set predict wins, or merely accompany them — contrast winner-stacks vs loser/non-event bars). QRA-driven; no engine build until the organising structure is settled.

---

## 2026-07-10 — F12 RATIFIED. MEASURED: variables mirror, combinations do not. Amendment 2 issued.

**F12 `concurrence_profiler.py` — Auditor RATIFY** (sha256 55c2e672…). Two defects found and fixed at the root, both OUTSIDE the core measurement. (1) **Look-ahead in the signal path (material):** stage-6 regime labels were fit on the full sample and then GATED ENTRIES — bar t's eligibility depended on future bars. Fixed by burn-in fit (fold 1, Jan 19-31, 5,636 bars), forward-only assignment, burn-in fold excluded from scoring; verified `fold1_trades == 0` on every `regime_outcome` row, `folds_scored = Feb|Mar|Apr|May|Jun`. (2) **Secondary-lens rationale false:** the survivor set references **231 of 249 conditions (113 long + 103 short)** vs primary 117+110, max depth 84 — the ceiling is a no-op. The near-identity IS the finding: F0's survivors draw on no special subset of variables. (3) **Supervisor-added:** `K_MAX` 65 -> 81 (LONG's measured max depth is 81; truncating the deepest stacks was the F10 error in miniature). Grid = 3,216 configs.

**THE MEASUREMENT THAT CHANGED THE QUESTION.** Self-computed from the F0 export. F0 enumerated every feature-triple across hi/lo products and BOTH directions, so every mirror was tested:

| level | mirrors |
|---|---|
| VARIABLES shared between LONG and SHORT survivor sets | **97.4%** |
| mirrored CONDITIONS appearing anywhere on the other side | **94.7%** (214/226) |
| mirrored TRIPLETS that survive | **3.3%** (1,712/51,311) |
| mirrored triplets among the 1,788 six-of-six persisters | **2.5%** (44) |

**Reading (operator's hypothesis; the data supports it).** The variables mirror almost perfectly; the COMBINATIONS do not — a 28x gap. The three variables clustering together going long DO NOT cluster together going short. Each variable measures something real in both directions — it groups with different partners. **The triplet is the wrong unit.** A frozen list is a photograph of one regime's groupings, and the market will not reproduce them. An engine that lets any variables stack (depth >= k, D2D agreeing) does not care that partners change — exactly the property required for an unseen future. **Combinations are NOT required to transfer; the design intent is freedom for variables to form their own clusters per regime.**

**Superseded reading (recorded for honesty).** The 3.3% was first read as short-side mirrors merely lacking tape (regime-baking). That is weaker and partly wrong: 94.7% of mirrored conditions DO appear on the other side, in different combinations. Regrouping, not absence, is the phenomenon.

**Consequence.** The central question is no longer only *does depth persist?* but **how do variables cluster freely, and does that clustering change by regime?** The ratified profiler cannot answer it: stage 5 computes ONE co-occurrence clustering per direction over ALL bars (`stack = depth_aligned >= COMPOSITION_FLOOR` across every bar) and runs BEFORE stage 6, so it has no regime labels. A global clustering averages the regimes together and hides the phenomenon.

**AMENDMENT 2 (blocking, before the run) — the export must be self-sufficient so this matter is concluded and never revisited:**
- **(A) Reorder:** regimes before composition.
- **(B) Stage 5 per (direction x regime):** condition_membership; **variable_membership with hi/lo collapsed**; top co-occurring pairs; **cross-regime stability** (does a condition keep its partners?); **per-regime deep-stack composition** (which variables dominate deep stacks in THIS regime). Thin cells emitted with `n_bars` + `skipped`, never hidden.
- **(C) NEW stage 5b — category-depth -> outcome.** The engine concept scored: enough variables WITHIN a category at extremes + enough concurrent stacking = trade. Marginal per-cluster sweeps + dominant-category variant (a joint per-category threshold sweep is n-dimensional and intractable). Full per-fold columns.
- **(D) NEW stage 8 — null baseline.** CIRCULAR SHIFT of the depth arrays (preserves autocorrelation, event shape, marginal distribution; destroys alignment with price). An i.i.d. shuffle would be an unfairly weak null. Emits an empirical p-value per cell against the observed stage-4 statistic. Settles chance vs structure inside this export.
- **(E) Regime-tag every emitted row** (depth bars, events, entry order, D2D flips) so any table slices by regime post-hoc without re-running; extend stage-6 `regime_outcome` to sweep k across the stage-4 range within each causal regime, confirm and invert.

**CAUSALITY — repeat-defect guard (critical).** Cluster membership in stage 5b decides WHICH CONDITIONS COUNT toward category depth. It therefore GATES ENTRIES and is a signal input. It must be fit on the leading burn-in window and assigned forward-only, burn-in fold excluded from scoring, exactly as the stage-6 regime labels now are. **A full-sample clustering that gates a trade is the identical defect the Auditor rejected on 2026-07-02.** Descriptive clustering (stage 5) gates nothing and may use full-sample labels, flagged `causal=False`.

**Unchanged, not to be reworked:** stage 4 (RATIFIED — K_MIN/K_MAX/K_STEP = 15/81/1; 3,216 configs; per-fold columns lifted from `wf.fold_metrics`), primary-view purity, oracle-only thresholds, zero TM reconstruction, D2D snapshot -> feed -> restore-in-finally, parallel + live progress, proof mode with [PARITY] PASS, measures-never-selects.

**Step 15b (follow-ons, reduced to two — category-depth moved into the script):** (ii) mirror-state test across ALL convergences, scoring mirrors regardless of survival; (iii) formal null baseline on F0 (naive reference: 1,788 observed 6/6 vs ~360 expected under an independent-fold null at empirical per-fold rate 0.437 = **4.97x excess**).

---

## 2026-07-09 — CORRECTION: F10's "degenerate" verdict was a k-band artifact. Family 12 (Raw Variable Concurrence) created.

**The error.** On 2026-07-02 the standalone F10 convergence-density scanner was ruled degenerate over the raw 249-condition pool — "every band k=1..10 selects the same bars, identical trades/PF" — and was struck as a peer family and fused into F0. That verdict is WRONG. The bands were the defect, not the phenomenon.

**Measured evidence (self-computed, post-warmup 146,083 bars, 249-condition pool, direction-aligned):**
- LONG per-bar co-fire count: min 8 | p5 17 | p25 22 | **median 27** | p75 34 | p90 43 | p95 48 | p99 59 | max 81 (117 long-aligned conditions)
- SHORT per-bar co-fire count: min 2 | p5 12 | p25 18 | **median 23** | p75 29 | p90 36 | p95 40 | p99 48 | max 61 (110 short-aligned conditions)
- Every F10 band (k=1,2,3,4,5,6,8,10) sits at or below the 5th percentile. `k>=10` captures **100.0% of bars**. Degeneracy was guaranteed by construction.
- The discriminating range is ~**k=20..60**: `k>=30` -> 40.8% of bars; `k>=50` -> 4.3%. **Never entered.**

**Accountability.** Developer built the bands, Supervisor (this seat) verified the run and reported degeneracy, Auditor ratified the fusion. All three missed that the sweep never reached the distribution's body. This is the SECOND premature dismissal of concurrence in the project's history (the first: the original 766K -> 4,773 -> 76 pipeline deduped the overlap away before it could be measured). Recorded so it cannot happen a third time.

**Family 12 — Raw Variable Concurrence (new independent family, `concurrence_profiler.py`).** The triplet (k=3) was always an arbitrary imposition. F12 removes it: count how many of the 249 conditions are at their extremes on a bar, direction-aligned — no triplet, no signal list, no survivor filter. Primary pool = all 249 (117 long / 110 short aligned; `==0` neutral excluded). Secondary comparison = conditions appearing in F0 survivors; the difference between the views is itself a finding.

**Operator's thesis (recorded as the design rationale).** A frozen list of triplets encodes the regime that produced it. A RULE — "N variables at extremes, D2D agreeing, depth >= k" — encodes market BEHAVIOUR, which is what carries into an unseen future. **Persistence is the priority; the frozen list is the threat to it.** The EA already carries supporting evidence: `Dots_MinConcurrent=2` at volume>=50 outperforms a solo signal requiring volume>=300 — two signals stacking beat one signal shouting.

**F12 measures (measurement only):** per-bar depth (raw + D2D-agreeing, k swept ~15..65); concurrence EVENTS (onset, build rate, peak depth, duration at depth, decay); entry order (which conditions join first — leading indicators, ties to F1's lead-lag map); the outcome map `peak depth x duration -> WR/PF/forward return` scored through the ratified engine + locked wf 6-fold; composition (conditions clustered by co-occurrence — the 8 "market structures" in DOTS_76_signal_dictionary.xlsx are a human taxonomy of SIGNAL types, never a measured per-bar regime, and are NOT inherited); regime (BARS clustered on state variables, k swept 2..12 by silhouette/BIC, validated for recurrence across all folds); and an EARNED reversion gate (counter-D2D entries permitted only if reversion-dominated high-depth composition proves profitable across all six folds and precedes flips).

**Why F12 settles the curve-fitting question:** nothing is selected. The free parameters are `k` and `duration` — two integers, fold-validated. Monotone improvement across all six folds = structural property. Improvement in three folds and reversal in three = noise, reported plainly.

**Docs updated:** pattern map -> v7 (F10 verdict corrected, F12 added); execution sequence step 15 -> F12 build (blocking, pre-quant). Selection (QRA) is step 16 and does not begin until F12's measurement exists.

---

## 2026-07-09 — Step 15 DEFINED (blocking, pre-quant): concurrence profiling + engine-architecture direction

**Why this step exists (operator, and he is right).** The original pipeline went 766K convergences -> 4,773 -> 76 by **removing overlap** — it deduplicated the co-firing away and then never scored it. Wiping the 76, rebuilding the variables and the history, and re-running the whole discovery was done **specifically so concurrent-convergence behaviour could be measured properly**. After Stage 8 it still has not been measured. Concurrence has been dismissed once in this project's history; it gets scored independently this time, BEFORE selection.

**`concurrence_profiler.py` (to build: Developer -> Supervisor -> Auditor).** Input: F0's 89,529 raw / 51,311 deduped survivors, over the full sealed baseline. It MEASURES, it does not select — nothing pruned, nothing concluded:
1. **Stacking distribution** — per-bar co-fire histogram (LONG and SHORT separately). Not a k-sweep over a pre-chosen set; the actual distribution.
2. **Identity, not count** — which triplets stack together, and whether the same combinations recur or co-firing is incidental.
3. **Variable recurrence** — across the 90 FEAT_ + 27 equality conditions, which variables populate the deep stacks. Is there a core set?
4. **Market-structure conditioning** — cross-tab stacking against in-baseline structure variables: `AT_Regime_ST` x `AT_Regime_LT` (4 populated cells, 48,677 / 29,164 / 34,467 / 40,675 bars), `Sqz_State` (3), `RangeOsc_State` (5), `Trend_Concordance`, `Trend_Conflict`, `Harmonic_D2D_Concordance`, `ADX_Rising`, session anchors. Report the FULL cross-tab; do not fix a taxonomy up front. Which convergences stack in which structure, and how deep?
5. **Depth vs outcome** — do 20-deep stacks behave differently from 3-deep, and does the relation hold inside each structure cell? Scored through the ratified engine + wf, oracle-only, zero TM reconstruction.

**Clarification recorded (F10 fusion).** The F10 `density` mode fused into F0 measures co-firing WITHIN a passed signal set (it sweeps `k` over `deduped_survivors.csv`). It is a diagnostic over a chosen set — it is NOT the general concurrence profile described above, and the Stage-8 density run over the full 51,311 pool was degenerate (min co-fire 7 long / 2 short; k=1..10 barely filters). F11 is rolling lead-lag, unrelated. The profiler is new work.

**Concurrence is a STRENGTH signal, not a dedup artefact.** Several independent triplets firing on one bar makes that bar MORE significant, not redundantly counted. The dedup pass must therefore do two opposite things: collapse trivially relabelled duplicates (F1's lag-7 vs lag-8 twins of the same trade) while PRESERVING and MEASURING genuine multi-signal stacking. Do not conflate them.

**The sample is ONE regime.** Jan 19 -> Jun 25 2026 runs from the post-Iran/US-war trough (~45K) to ~53K — a sustained bullish recovery. F0's 6/6 >=30-trade survivors split **604 LONG / 98 SHORT**. That skew is the tape, not a defect. The 98 shorts persisted AGAINST a strong uptrend, which makes them structurally interesting. The baseline contains no sustained downtrend, so it cannot say how these behave in one — know what the sample can and cannot answer.

**PREFERRED ARCHITECTURE (operator direction): a concurrent-convergence engine, not a frozen signal list.** A triplet at percentile extremes + D2D agreement has a symmetric partner (hi/hi/hi + D2D-up long <-> lo/lo/lo + D2D-down short). In a bull sample the long side accumulates persistence and the short side does not, purely because the tape rose. Selecting on persistence therefore **bakes the regime into the signal list** and condemns the operator to perpetual re-backtesting. The robust alternative: **the RULE is the engine.** Any convergence at percentile extremes with D2D agreeing may fire, either direction, weighted by concurrence depth. The 51,311 survivors are evidence the rule works; the specific 702 are an artefact of the regime that produced them. Selection's job becomes proving the rule and calibrating the density threshold and gates — not curating signals.

**Sequence:** step 15 (profiler, blocking) -> step 16 (QRA selection + the (a)-(g) analyses) -> step 17 validate -> step 18 Stage 9 install -> step 19 execution-parity + demo + live.

---

## 2026-07-09 — Step 14 CLOSED — Stage 8 definitive discovery complete (440,057 candidates)

**Run.** F0 full search (5,007,850 variants, 2h54m) -> 89,529 raw -> 51,311 deduped survivors -> re-scored through the wf 6-fold by `f0_to_schema.py` (21h). F1 rebuilt to ordered pairs (238 scorable conditions x LAGS 1..15 x 2 dir = 1,699,320) and run by `run_f1_parallel.py` (8 workers, 24h54m; parity-proven row-for-row identical to serial). Orchestrator drove F2-F9/F11 in-process and ingested F0+F1 from CSV. `discovery_master.csv` = 440,057 rows, 14-col schema, persistence-first sorted, no rows dropped. Reconstructed and verified independently: row count and all 11 per-family counts match the orchestrator exactly.

**Per-family.** F0 51,311 | F1 384,553 | F3 3,658 | F9 255 | F11 103 | F4 66 | F2 47 | F7 28 | F5 16 | F6 10 | F8 10.

**Persistence (raw, pre-selection).** F0: 1,788 at 6/6 folds (24 with >=100 trades, 250 >=50, 702 >=30). F1: 11,772 at 6/6 folds (1,903 with >=100 trades, 6,843 >=50). 58,949 F1 pairs at 5/6.

**F0 density sweep** (over the full 51,311-survivor pool): degenerate, as the map predicted. Min co-fire count is 7 (long) / 2 (short), so bands k=1..10 barely filter (bars 152,983 -> 149,337 at most; trades ~47-48K throughout; every metric flat). At 1 lot with ALL signals firing: PF 1.04, WR 75.3% long / 76.0% short, worst-day -$629,350 long / -$356,300 short. Density must be re-run over the SELECTED set, where the co-fire range is meaningful. Engine `SPREAD = 3.0` (unchanged); the reported `1.04->0.86` is PF at spread 3.0 -> PF at spread 5.0 (the +2.0 robustness stress), not a spread value.

**Speed intervention.** F1 serial was measured at ~1.7 candidates/sec = ~11 days; killed at ~17% after 47h. `run_f1_parallel.py` (8 workers, ratified scoring reused byte-identically, Auditor RATIFY, parity PASS on the operator's machine) cut it to 24h54m at ~19-23 cand/s. Live progress + ETA added — the previous silent single-dump runs cost two days of blind guessing. Lesson recorded: long runs must print progress and, on crash-prone hardware, checkpoint per chunk.

**Known gap carried into Step 15 (operator-identified).** F0 deduplicates internally (>80% entry-overlap collapsed, greedy by PF). **F1 has no dedup at all** — 384,553 rows include heavy lag-twin redundancy (A->B at k=7 vs k=8 is substantially the same trade). No cross-family overlap check exists either. This asymmetry was not caught by the pattern-map review or the scanner audits. The overlap/dedup pass is Step 15(a) and must be specified against the REAL overlap structure observed in the data, not an assumed one.

**Next: step 15 — Selection + structural analysis (QRA seat).** Four defined analyses: (a) overlap/dedup + cross-family overlap report; (b) lag distribution of the 11,772 persistent F1 pairs vs the Stage-5 12-20 bar D2D lead; (c) D2D anticipation — do sequences fire BEFORE the flip (D2D as target, not gate); (d) good-flip/bad-flip discrimination via preceding sequence (prior work tested only single-variable state AT the flip bar — a snapshot, not a trajectory). Then re-run density over the selected set and assemble a diversified final set.

---

## 2026-07-02 — Step 13 CLOSED — all 12 family discovery scanners built + ratified

**Done.** The full Stage-8 search space is built and Auditor-ratified (Developer → Supervisor → Auditor per family). Each scanner: imports the oracle (`dots_thresholds`) + reuses the ratified TM (`portfolio_simulation_engine.run_portfolio`) + scores survival-first via `wf`; runs on the sealed baseline; zero independent threshold computation; zero TM reconstruction.

**Scanners (11 files + F0):** F0 `triple_convergence_and_d2ddir.py` — the ratified engine, now with F10 **convergence-density fused** as a `density` mode (over the selected set, not the degenerate 249 pool; standalone `convergence_density.py` RETIRED). F1 `sequential_temporal`, F2 `state_transition`, F3 `conditional_interaction`, F4 `divergence_nonconfirm`, F5 `persistence_autocorr`, F6 `threshold_crossing`, F7 `mean_reversion`, F8 `cross_variable_structure`, F9 `session_temporal`, F11 `rolling_leadlag`.

**Hazards contained (each audited hardest for its class):** D2D column-reconstruction for invert/exempt (F4/F7) — safe because D2D is gate-only in the engine (L79 read / L89 gate), original restored in `finally`; forward-return quarantine (F5) — post-run discovery target, absent from mask/sig/direction/size/exit/ranking; smuggled-percentile (F8, F11) — none, structural levels only; blind hour/minute sweep (F9) — named session anchors only, EST_Minute pins :30 events only; look-ahead (F11) — gold-standard truncation test `mask(df)[t]==mask(df[:t+1])[t]` clean, causal trailing window.

**Design decisions recorded:** D2D_Trend_Dir is a per-family SEARCH dimension (confirm/invert/exempt), not a fixed constant — resolves the directional bias and lets the counter-direction families (F4/F7) fire. F10 density is a discovery dimension of F0, not a peer family. F6 is the thinnest family (F0-velocity + F2-cross) — earns its own scanner at selection or folds into F0/F2. Parity: discovery carries no burden; production new-derived-input tier = F1/F2/F6/F8/F11 (Stage-9 concern).

**Note on run-proofs:** every scanner's `main()` run-proof is an ARBITRARY subset that REJECTS survival — proof of execution, not a claimed edge. No candidate numbers from run-proofs are findings. Real results come from the full Stage-8 run only.

**Next active step: step 14 — Stage 8 definitive discovery** over the full 12-family search space on the sealed baseline. Pattern map at v5.

---

## 2026-07-02 — Discovery pattern map v4 adopted; Step 13 (build 12 family scanners) inserted; Family-0 script renamed

**Pattern map.** Created `equiDOT_discovery_pattern_map.md` — the completeness reference for the Stage-8 search space. 12 families: 0 simultaneous convergence (implemented), 1 sequential/temporal, 2 state-transition, 3 conditional/interaction, 4 divergence, 5 persistence/autocorr, 6 threshold-crossing, 7 mean-reversion, 8 cross-variable structure, 9 session-temporal, 10 convergence-density, 11 rolling lead-lag/cross-correlation. Reviewed by Auditor + Developer + Manager over 4 revisions; verdict COMPLETE at v4. Every cited column verified live against the sealed baseline; DecayState_ST/LT struck (dead, nunique=1); EST_Minute struck (M1 noise); F9 restricted to session-anchor hours (08:00/09:30/10:00/15:30/16:00) + weekday.

**Frame decisions.** Held CONSTANT: ADX≥15 & Volume>50 eligibility, solo≥300/concurrent≥2 entry, oracle-only thresholds, locked S.7 TM, survival-first wf. SEARCHED (not constant): the D2D_Trend_Dir gate is a per-family parameter (confirm/invert/exempt) — resolves the directional bias and lets the counter-direction families (4 divergence, 7 mean-reversion) fire. Parity: discovery carries no parity burden (all Python on the sealed baseline); production tiers are no-burden {0,3,4,5,7,9,10} vs new-derived-input {1,2,6,8,11}.

**Scope ruling (operator).** ALL 12 families are in active Stage-8 scope — no deferral, no backlog (fullest-market-perspective directive). Build cost of the heavy families (1, 8, 11) is a reason to build them correctly, not to delay.

**Execution-sequence change.** Inserted **Step 13 — build + ratify the 12 family scanners** (Developer → Supervisor → Auditor; each imports the oracle, reuses the sim-engine TM, scores via wf, runs on the sealed baseline). Renumbered: old 13 (definitive discovery)→14, 14 (validate)→15, 15 (Stage 9 install)→16, 16 (execution-parity + demo + live)→17; cross-references updated.

**Rename (operator, for targeting clarity).** `full_766K_convergence_discovery.py` → `triple_convergence_and_d2ddir.py` (Family 0, byte-identical). New scanners named per family: `sequential_temporal.py`, `state_transition.py`, `conditional_interaction.py`, `divergence_nonconfirm.py`, `persistence_autocorr.py`, `threshold_crossing.py`, `mean_reversion.py`, `cross_variable_structure.py`, `session_temporal.py`, `convergence_density.py`, `rolling_leadlag.py`. Operator to update the Instructions PROJECT FILES reference to the new Family-0 name.

**No EA / oracle / data / sacred change** — documentation + tooling scope only. Next active step: step 13.

---

## 2026-07-02 — Step 12 CLOSED — walk-forward folds LOCKED (Auditor + Manager sign-off, unanimous)

**Locked.** The 6 monthly folds + survival-first scoring order + warmup floor in `wf.py` are final for Stage 8. Supervisor executed the lock verification; Auditor and Manager independently reconstructed from source and both returned LOCK with self-computed numbers.

**Verification (all PASS, 5/5).**
1. FOLDS — 6 monthly, keyed `Time[:7]`, Jan(19-31)/Feb/Mar/Apr/May/Jun(1-25) (L28-35, L98, L103); bounds are the baseline range; no bar double-counted or dropped.
2. SCORING ORDER — survival is the sole verdict gate: `survival_pass = worst_day_usd > -2500` (L126), verdict at L148; persistence (6/6, min fold PF) and spread-stress reported but never override.
3. WARMUP — floor `max(Dots_InitBars 6900, ring-sat 5204)=6900` covers the deepest-warmup variable: MultiDay_Slope first-valid bar 2759 (2026.01.21 20:08), MultiDay_Position + WeeklyOpen_Dist_ATR earlier — all < 6900.
4. NO REFIT / NO LOOK-AHEAD — `adaptive`/`structural`/`warmup` computed once on the full series (L90-92); fold loop slices via `mask_window` into `run_portfolio` (L102-104); zero per-fold recompute.
5. RUN-PROOF — `python3 wf.py` reproduces exactly: 1,628 trades, PF 1.54, WR 80.0%, per-fold PF [1.34,1.85,1.40,2.25,1.18,1.47], 6/6 profitable, worst-day -$47,700, 33 hard-stop days -> SURVIVAL REJECT.

**Not locked (configurable by design).** `LOT_SIZE`=1.0, `DAILY_LOSS_CEILING_USD`=2500, `SPREAD_STRESS_USD`=2.0 — operator-set after Stage 8; lot P&L is linear ($100/pt/lot) so the survival gate must hold at any lot value. Magnitudes are not lock elements; the fold boundaries + scoring order + warmup floor are.

**Meaning.** The 76-benchmark REJECT is the expected, correct outcome — 6/6 persistence but unsurvivable at size — which is exactly why Stage 8 re-derives the signal set from scratch. Next: step 13, Stage 8 definitive discovery on the sealed baseline over the 117-candidate pool.

---

## 2026-07-02 — Step 11 (execution-parity audit) DEFERRED to the deployment build; strategy-tester init finding recorded

**Attempt.** Set up a separate FTMO MT4 install (`C:\Users\d\...\mt4_strategy_tester`), imported the captured `US30.cash1.hst` (version 401, US30.cash, M1, 2dp, 84,166 bars, 6 Apr → 1 Jul), compiled the frozen EA (`equiDOT_strategy_tester`, 0/0), Strategy Tester set to US30.cash / M1 / Open prices only / inputs matching the frozen EA.

**Blocker.** Every run aborts in <1s at EA init: `initialization failed (1) — Not enough bars on chart to initialise EA. Bars: 1002`, constant across From dates 13/14/16/20 Apr (the constant proves it is not date- or history-quantity-driven).

**Root cause (verified in `equiDOT.cs`).** The `OnInit` guard at L356 `if(current_bars<ClusteringLookback+trainingBars)` requires `1000+100=1100` bars, reading `current_bars=iBars(Symbol(),Period())` (L351/354). In the Strategy Tester `iBars()` returns only the bars modelled up to the run-start (~1002), not the full file, and `RefreshRates()` is a no-op there — so the guard can never be satisfied at tester-init regardless of file depth. The calc-path guard at L1125 already carries the `!IsTesting()` exemption for this exact check; the OnInit guard does not. The `6900` figure (`Dots_InitBars`) is not the trigger here — the trigger is `1100`.

**Structural constraint.** Even with the guard exempted, the tester seeds the adaptive buffers FORWARD from the run-start, whereas live init starts pre-seeded with `Dots_InitBars`=6900 chart bars. So a fixed guard alone yields only post-saturation parity (~6900 bars in); the front of the run computes different variable values than live. Full-window parity would need a tester-only warmup-injection path.

**Decision.** (1) The early Step-11 audit is premature — there is no Stage-8 signal set and no Stage-9 trade management to compare against, so the tester Results report has nothing valid to check. (2) The execution-parity audit already exists at Step 16 (re-verification on the deployment build); the early audit folds into it. (3) The tester-init correction is an EA source change → it is recorded as PENDING and bundled into the Stage 9 EA window (Step 15, when the EA is already unfrozen), NOT applied now during the discovery freeze. The identified starting fix: add `!IsTesting()` to the L356 guard to match L1125; the Developer must additionally verify the tester init path (`ResizeAllArrays(current_bars)` at reduced count, forward buffer growth) or author a warmup-injection path. (4) Forward testing (demo, pre-seeded chart, real ticks) remains the primary live-parity instrument, as it reproduces live init conditions the tester cannot.

**Scope note (execution sequence).** Step 11 → `[~]` DEFERRED; Step 15 scope extended with the tester-init correction; Step 16 annotated with the post-saturation comparison constraint. No EA, oracle, data, or sacred element changed by this record — documentation only. Next active step: step 12 (lock the walk-forward folds).

---

## 2026-06-27 — Step 10 CLOSED — discovery/sim/wf tool rekey; Auditor RATIFIED; A2.7 PASS×4

**Scope.** Rekeyed the three Python tools that Stage 8 discovery, Stage 7 execution-parity, and the walk-forward run on — `full_766K_convergence_discovery.py`, `portfolio_simulation_engine.py`, and the net-new `wf.py` — to the sealed step-7 baseline and the unified 117-candidate vocabulary. No EA, data, or oracle change (`dots_thresholds.py` byte-unmodified, sha `518862bf…`). The two tools previously pointed at absent `65K.csv` / `DOTS_60_final_signals.csv` and could not run; `wf.py` did not exist.

**Shared (all three).** 8-part loader (part1 header + parts 2–8 headerless → 152,983 × 171; asserts strictly-increasing Time, 0 dup, 0 NaN), identical across the tools; `VWAP_Sigma_ATR` attached after concat via `dt._vwap_sigma_atr` (derived, not a sealed column) so candidate value and threshold share one series; oracle-only thresholds — all 90 FEAT_ routed through `compute_adaptive_thresholds` (+ `compute_structural_gates` for OR_Position), **zero** independent threshold computation (the static-global `np.percentile` path removed, not bypassed); warmup floor = max(Dots_InitBars 6900, ring-saturation 5204) = 6900, first scannable `2026.01.26 20:54`.

**Discovery.** Vocabulary rebuilt from the oracle: 90 FEAT_ = `_D_COLS` (88) + VWAP_Z + OR_Position; 27 equality hardcoded; EXCLUDE as the asserted complement. A2.7 partition guard: 89 baseline FEAT_ + `VWAP_Sigma_ATR` (derived) + 27 equality + 54 excluded + Time = 171, no overlap, no gap. Unified condition list `(feat,'hi')/(feat,'lo')/(feat,'==',v)`; `run_search` generalized over it. **Equality enumerated on the post-warmup SCANNABLE region → 69 equality / 249 total scan conditions, exact to S.10 (no amendment).** The 3 warmup-only dead values (`OBVf_Signal==0`, `Sqz_State==0`, `RangeOsc_State==0`) excluded; neutral 0 retained wherever live.

**Supervisor revision (one).** First pass enumerated equality on the FULL eligible baseline → 72 / 252, carrying the 3 dead values — a correction to the section-8 ruling, not a Developer error (the Developer followed spec and flagged the symptom). Re-issued: enumerate on the post-warmup scannable set → 69 / 249, matching the documented S.10 figure with no doc amendment. Change isolated to `build_conditions`; engine and wf unchanged.

**Engine.** `SIGNALS_PATH` parameterized (default `recommended_set_76.csv`); `condition_mask` routes `hi`/`lo` → adaptive (OR_Position gated), `==v` → raw equality, else raises; a triple may mix both types. Trade-generation core exposed as `run_portfolio(df, signals_df, mask_window, adaptive, structural, warmup, verbose)` for wf. **S.7 trade management untouched** and confirmed against the frozen EA: `MAX_POSITIONS=6` (L1710), `SPREAD=3.0` (L1711), `STEP_PCT=0.30` (L1714), `MAX_RISK=150.0` (L1713), `RISK_MULT=2.0`; concurrent/solo gate L4835 (`qualifying≥2 OR solo+Vol≥300`); Friday 16:45 EST close L8934; D2D gate via `D2D_Trend_Dir == direction`. SPREAD appears only in P&L, never in an exit decision.

**Walk-forward (`wf.py`, net-new).** 6 monthly folds (Jan 19–31, Feb, Mar, Apr, May, Jun 1–25) keyed by `Time[:7]`; thresholds computed once on the full series and sliced per fold via `mask_window` — no per-fold refit (causality preserved); drives the authoritative `run_portfolio` (no reconstructed logic). Named constants: `DAILY_LOSS_CEILING_USD=2500` (operator EA hard stop = half the FTMO $5,000), `USD_PER_POINT_PER_LOT=100` ($1/pt per 0.01 lot), `LOT_SIZE=1.0` (configurable post-Stage-8), FTMO context 5,000 / 10,000. Survival-first scoring: (1) worst-day $ vs ceiling + hard-stop-day count; (2) cross-fold persistence; (3) spread robustness (analytic — valid because SPREAD is a flat per-trade P&L deduction that alters no exit).

**Run-proof on the 76 benchmark (broken-data set; NOT a target).** Engine: 1,628 trades, PF 1.54, WR 80.0%, 0 SL-win bugs. wf: aggregate 1,628 / PF 1.54 (= full run), **SURVIVAL REJECT** at LOT_SIZE=1.0 (worst day −$47,700 in May, 33 hard-stop days), persistence 6/6 profitable (min fold PF 1.18), spread 1.54 → 1.39 @ 5.0. The 76-set correctly fails the survival gate at 1 lot — the engine and gate work.

**Pipeline.** Human → Manager → Supervisor → Developer → Supervisor (PASS, one revision) → Auditor → human confirm. The Auditor independently reconstructed every figure from source and **RATIFIED** all three as a set (8/8 scanner-vs-engine parity; partition, 249/69, warmup, benchmark all reproduced; one prompt-text arithmetic correction — the partition is 89 baseline + `VWAP_Sigma_ATR` + 27 + 54 + Time = 171, which the code asserts correctly).

**Deploy.** The ratified `_new` staging files promoted to canonical un-suffixed names (content byte-identical — sha `b840df94…` discovery, `f88d2f47…` engine, `c4337a15…` wf); the bit-rotted Jun-19 `full_766K_convergence_discovery.py` / `portfolio_simulation_engine.py` replaced; `wf.py` added; the three `_new` staging copies removed; `dots_thresholds.py` untouched. Promotion is required (not cosmetic): `wf` imports `portfolio_simulation_engine` un-suffixed.

**A2.7 directive reconciliation — PASS × 4.**
- (a) **Completeness — PASS.** The full 117-candidate / 249-condition vocabulary (90 FEAT_ + 27 equality) is wired in all three tools; partition guard exact; both condition types implemented; the two formerly-deferred tools and `wf.py` all resolved.
- (b) **Adaptivity — PASS.** Oracle-only thresholds (mechanism D + structural); zero independent threshold computation anywhere; the static-global path removed.
- (c) **Parity — PASS.** Oracle byte-unmodified (export==live precondition intact); scanner TM == engine TM (8/8); wf runs on the authoritative engine (live==sim path). No EA / data / oracle change.
- (d) **Consistency — PASS.** S.6 (oracle sacred), S.7 (TM verified vs frozen EA), S.10 (249/69 reproduced, no amendment; 27-list + exclusions correct) all honored; sacred layer intact; deploy is rename-only.

**Next active step: EXECUTION SEQUENCE step 11 — Stage 7 execution-parity audit** (frozen EA in a non-FTMO MT4 strategy tester vs `portfolio_simulation_engine.py`).

---

## 2026-06-26 — Stage 5 CLOSED — D2D standalone band-calibration study; A2.7 PASS×4

**Scope.** Stage 5 ran on the sealed step-7 baseline (`equiDOT_recon171_step7_part*`, 152,983 × 171, Jan 19 → Jun 25) with zero EA changes. A free-running Python replication of the full D2D SuperTrend band + posture engine was built and validated against the exported columns; convergence was evaluated through the unified oracle (`dots_thresholds.py`, mechanism D + structural) on the 76 benchmark triples (all 62 underlying variables adaptive). The Manager directive reframed the study: not "does D2D have standalone edge" (known null) but "can the variables driving D2D's adaptive band settings be re-chosen to make D2D a better gate for DOTS."

**Replication parity (byproduct).** The Python D2D reproduces the export at export precision — `D2D_Trend_Dir` 99.9993% (1 isolated bar), flip count exact (3,870 = 3,870; 1,935 up + 1,935 down), formula residuals at print-rounding only (`D2D_Basis` 0.0051 @ 2dp, `D2D_ATR` 1.6e-5 @ 6dp, `D2D_Persist` 1e-6 @ 6dp); the posture engine matches exported `D2D_Dynamic_Sensitivity` on 99.978% of bars. dynMult distribution: 2.5 on 66.6% of bars, 1.8 on 26%, 1.5 on 7%, 0.8 floor on 0.28% (422 bars). Stage 5 thus re-confirmed export==live parity as a side effect.

**Findings.**
- **D2D standalone — no scale-free directional edge (confirmed, expected).** Forward hold ≤ 50% at every horizon (h=5→240: 48.1 / 47.5 / 48.0 / 49.3 / 49.2 / 49.9%), MFE/MAE ≈ 1.0 throughout; hold-to-next-flip win 32.9%, sum −7,865 pts. Across all 117 candidates, max |Spearman| with normalized post-flip return = 0.118; nonlinear GBM OOS AUC 0.478 (< 0.5). Known before the study (D2D failed standalone in ranging markets — the reason OBVf and DOTS exist); recorded as baseline, not as a new result.
- **Convergence leads D2D by 10–20 bars (proven).** New-direction convergence is elevated 10–20 bars *before* the flip (~12% gate-open rate at offsets −20..−10), dips at the flip itself (~6–8%); old-direction convergence *spikes at the flip* (20.5% at offset 0 vs ~8% baseline). Convergence is the faster detector; D2D is a lagging directional confirmation of a move convergence already sees.
- **STRUCTURAL — the gate blocks the early good entries AND the old-direction traps (most significant finding).** The directional gate (`equiDOT.cs` L4818–4819, `if(dots_rules[i].direction != d2dDir) continue;`) discards any convergence whose direction ≠ current D2D direction. Because convergence detects the new direction 10–20 bars *before* D2D flips, those early new-direction entries are blocked — D2D still points the old way. DOTS literally cannot trade the early entries convergence identifies; by the time D2D catches up and flips, the initial move is partially spent. The same gate simultaneously blocks the old-direction mean-reversion burst that fires *at* the flip (the offset-0 spike). The gate is therefore a **latency-vs-protection trade-off** — it forgoes early new-direction entries in exchange for suppressing old-direction traps.
- **Raising confirmN worsens the latency.** `U_confirmN` currently = 1 (a single close across the ratcheted trail flips). Requiring more confirming bars delays the flip further, widening the gap between what convergence already sees and when the gate opens. confirmN is therefore **not a pure survival lever** — it is one side of the latency-vs-protection trade. (Sweep: confirmN {1,2,3,5} leaves forward hold at 49.2–49.6% in every config; confirmN>1 cuts whipsaw 12%→3%, worst-case flip −780→−476 pts, sum −7,868→−1,377 pts, at the cost of worse median and typical MAE.)
- **Posture engine captures the right dimension — volatility → density.** Convergence *density* (DOTS opportunity per regime) is strongly volatility-driven at the flip bar: D2D_ATR ρ = 0.50, ATR_1M 0.49, Volume 0.47, Bar_Range 0.47 vs regime convergence-entry count. The current posture inputs (OBV delta, OBV velocity, PoC distance, Volume, Efficiency) already pull in this dimension; flips in volatile conditions open opportunity-rich regimes.
- **No alternative posture input improves convergence success.** Convergence *success* (per-regime trailing-exit meanR, real spread p10 −1.5R → p90 +0.7R) is not predictable: max |Spearman| across all 117 = 0.127; per-entry meta-label OOS AUC 0.520 (decile lift 23.3% → 29.6% over a 24.4% base); the regime GBM 0.559 is entry-count variance, not signal. The two candidate alternatives both come back null-to-negative — MultiDay_Slope *agreeing* with flip direction: success −0.358R vs *against* −0.296R; high-ADX flips: more density (0.28 vs 0.23) but worse success (−0.358 vs low-ADX −0.192). No void-filter condition separates failing convergence strongly enough to act on.
- **Band formula robust — no constant change dominates.** Sweeps of confirmN, U_multMin, U_sensExp, and the combined design-intent variant leave forward hold at 49.2–49.6% in every configuration. No band-constant change creates directional edge; the only effect is the confirmN churn/whipsaw trade above.
- **Gate-value vs counter is confounded by the non-production triple set.** On generic mechanism-D p80/p20 triples, D2D-aligned convergence is ≤ counter-aligned across three exit models (symmetric ±2ATR 47.9% vs 52.4%; trailing TM −0.268R vs −0.166R; 1:3 trend barrier −0.071 vs +0.115). This does **not** settle gate value — generic confluence has no edge either direction, and the production set is co-selected with the gate at Stage 8. Recorded as a flag, not a verdict.
- **Survival levers identified.** (1) confirmN — latency-vs-protection trade, evaluate at Stage 8 against production-signal P&L. (2) ATR-conditional sizing — absolute adverse excursion rises monotonically with flip-bar ATR (median MAE 9 pt at ATR-decile 1 → 68 pt at decile 10; 62% of MAE > 250-pt flips sit in the top-2 ATR deciles) → a Stage-9 trade-management finding (size down on high-ATR flips), not a signal-voiding finding.
- **Gate-variable optimization deferred to Stage 8 co-selection.** The directive's affirmative — re-choose posture inputs to improve the gate — has no data support at any level tested (density predictable, quality not; current inputs not improvable; both proposed alternatives null). Structural reason: every convergence-success test rests on benchmark triples that carry no edge on mechanism-D thresholds (meanR negative both directions); a gate cannot be tuned to a signal that does not yet exist in production form. The valid affirmative test requires the Stage-8 signal set (derived/co-selected on clean data) run through `portfolio_simulation_engine.py` (Stage-10 rekey). Gate-variable selection is folded into Stage 8.

**Central open question (carried to Stage 8).** Is the gate's protection (suppressing old-direction traps at the flip) worth its latency cost (forgoing the early new-direction entries convergence already sees)? Measurable only with production signals + the authoritative sim. confirmN's optimal value is a sub-question of the same trade.

**A2.7 directive reconciliation — PASS × 4.**
- (a) **Completeness — PASS.** All 117 candidates (90 FEAT_ + 27 equality) and all 76 benchmark triples were exercised; the discovery vocabulary and 249 scan conditions are unchanged; no candidate added or excluded.
- (b) **Adaptivity — PASS.** No threshold change. Mechanism D + the two structural pairs (VWAP_Z ±2, OR_Position 0.80/0.20) intact; the forward Python replication confirmed the D2D band formula is causal (no look-ahead).
- (c) **Parity — PASS.** No EA or data change; the study re-confirmed export==live parity (Trend_Dir 99.9993%, flip count exact, posture 99.978%) as a byproduct; convergence was driven by the oracle.
- (d) **Consistency — PASS.** Findings consistent with the locked architecture; all three non-negotiables verified synced to 117 / A2.7 / DOTS_NUM_FEATURES = 90 (the "83 → 90" instances are transition text; every file carries the authoritative `= 90` registry line). The prior header precondition ("developer/auditor still need syncing") was stale and is cleared.

**DECISION — conditional D2D fork (EXECUTION SEQUENCE step 9): does NOT fire. EA stays FROZEN.** No band-constant change improves flip quality, no posture-input change improves convergence success, and the latency-vs-protection question is measurable only with production signals. No documented-merit, human-authorized, behavior-changing edit to any sacred element is warranted. Step 9 collapses into Stage 8 co-selection (gate variables + confirmN evaluated jointly with signal derivation); ATR-conditional sizing → Stage 9; dataset remains the sealed step-7 baseline. Next active step: EXECUTION SEQUENCE step 10 (rekey discovery/sim/wf tools).

---

## 2026-06-25 — Steps 6 & 7 CLOSED; baseline re-sealed; A2.7 reconciliation PASS×4; Stage 5 opens

**Step 6 — candidate + precision EA window (one pipeline pass) — CLOSED, compiled 0/0.** Through Developer → Supervisor → Auditor → human-confirm → MetaEditor compile (0/0). Three changes in one window:
- **`Volume_Ratio_10`** — live calc-window aligned to the export window `i+1..i+10` (excludes the current bar). Export/baseline unchanged; no re-discovery.
- **6dp export-precision fixes** — `KAMA_Dist`, `ADX_Value`, `VWAP_Sigma` raised 2dp → 6dp. `ADX_Value`/`VWAP_Sigma` were sub-2dp full-precision indicators exported at 2dp; the gap was caught by A2.7 / the Auditor's VP-PRECISION backstop before re-export and folded into the same window (would otherwise have failed parity 86/88).
- **7 Group-B candidate registrations** — ADX_Value, Body_Size, Upper_Wick, Lower_Wick, TChan_A15, VWAP_Sigma, Volume registered as FEAT_ (`DOTS_NUM_FEATURES` 83→90); all already exported → +7 `DotsGetFeatureValue` cases / +7 rolling buffers / +7 oracle routing rows, no Calc, no schema change. Export stays 172 columns.

**Step 7 — re-export, rebuild, parity, ADX recompute — CLOSED, baseline SEALED.**
- **Re-export:** `64_256_*` (78,624 rows, Apr 6 → Jun 25) + EA threshold dump (6,900 bars). Live-effectiveness confirmed: ADX_Value `7.142857` (was 7.14), VWAP_Sigma `4.498316` (was 4.50), KAMA_Dist 6dp; the 7 new candidate columns populated in the dump.
- **Parity → 88/88.** Oracle run cold over the 6,900-bar dump window vs the EA snapshot: all 88 D candidates × HI/LO at 6dp, max abs diff 0/0. Both prior failures (`KAMA_Dist`, `Volume_Ratio_10`) resolved; the 7 Group-B aligned bit-for-bit. The D universe expanded 81→88 with the Group-B adds, so the target moved 81/81 → 88/88.
- **Baseline rebuilt + sealed:** `core.py` re-keyed to the 172-col export (171-wide stitch; oracle derives `VWAP_Sigma_ATR`). Standing data-correction sequence applied — 15 cold-start families + 45 new columns recomputed full-precision over the original Jan–Apr OHLCV, δ=−2 state-column shift (97 shifted / 12 kept), stitched at seam `2026.04.13 01:05`. Result: **`equiDOT_recon171_step7_part*`, 152,983 × 171, Jan 19 15:49 → Jun 25 18:18**, strictly increasing, 0 dups, 0 NaN, schema == recon171. (Row count up from 150,599 — the fresh export reaches ~1 day further.) Gates: overlap OHLC bit-exact; GATE 2 KAMA_Dist max 0.0 / median 0.0 exact vs the 6dp export. **This supersedes the prior `equiDOT_recon171_part*` (150,599 × 171) as the sealed clean baseline.**
- **5 KAMA_Dist triples re-validated** (indices 6/7/15/29/54 of the 76-set) via `portfolio_simulation_engine.py` on the sealed baseline: qualifying bars 13/22/59/0/1, combined 37 trades, WR 81.1%, PF 1.91, +547.0 pts. Confirms the 6dp `KAMA_Dist` is consumed correctly; triple #4 (all-KAMA-family) now fires 0 — an expected consequence of the KAMA-family correction. Stats benchmark-only (the production set is re-derived at Stage 8).
- **`ADX_Value` full-precision recompute (A2.7 absolute-precision election).** ADX_Value sits in the original 126 columns and was carried at 2dp in Jan–Apr (not in TARGETS/NEW45). Per human direction it was added to `core.py`'s recompute targets — exact transcription of `equiDOT.cs` `Calc_ADX_OnBar` (period 14, Wilder α=1/14, +DM/−DM/TR recursive EMA, DI±, DX, ADX; TR floored at Point; oldest 2 bars = 0). Validation: **GATE 1 = 0.0, GATE 2 = 0.0** (exact vs export); OLD-segment precision 2dp → 16dp; the 30 eligibility-boundary bars at 2dp 15.00 resolved to 0. Scope-bounded: one TARGETS addition; the 15 existing recompute families byte-identical to the pre-ADX rebuild; the 5-triple re-validation re-confirmed unchanged on the ADX-recomputed baseline (eligibility 92,355→92,345, no triple firing-window affected).

**A2.7 DIRECTIVE RECONCILIATION — PASS on all four (verified against source/data, not asserted).**
- **(a) COMPLETENESS — PASS.** 90 FEAT_ (88 mechanism-D in `dots_thresholds.py._D_COLS` + 2 structural: VWAP_Z, OR_Position) + 27 Group-C equality = 117 candidates / 249 scan conditions. Every non-candidate export column has a documented structural exclusion.
- **(b) ADAPTIVITY — PASS.** 88 D at parity (88/88), 2 structural constants, 27 equality deterministic. The oracle is rolling-2500 floor-index + day-refresh only; the static-global percentile path and mechanisms A/B/C are retired — no static path remains.
- **(c) PARITY — PASS.** EA thresholds reproduced bit-for-bit (88/88). Full residue scan of all 87 D column-candidates across both baseline segments: **0** with OLD precision < NEW precision; ≤2dp columns are inherent (integer counts, price-diff wicks, coarse Micro_*), OLD==NEW==live, and 88/88 parity precludes export truncation. ADX was the final residue — closed. No 2dp residue anywhere.
- **(d) CONSISTENCY — PASS.** The ADX recompute was additive (one TARGETS addition; 15 existing families byte-identical); no new work contradicts prior completed work.

**Result: step 7 CLOSED, Stage 5 OPEN.** Standing precondition (does not block analysis-only Stage 5): sync `non_negotiables_developer.txt` / `non_negotiables_auditor.txt` to 117 / A2.7 / 90 FEAT before the next EA window.

---


## 2026-06-25 — Threshold unification: BUILT, COMPILED, PARITY-VALIDATED (79/81); two fixes for total parity

**The 8-pass build is complete and compiles 0/0.** The adaptive-threshold EA change (EXECUTION SEQUENCE step 4) retired mechanisms A (static hardcoded), B (ATR-scaled), C (regression-EMA) and the static-global percentile path, unifying all 83 candidate thresholds onto mechanism D (rolling-2500, day-refreshed floor-index percentile) for the 81 distributional features plus structural constants for VWAP_Z (±2.0) and OR_Position (0.80/0.20). Delivered through Developer → Supervisor → Auditor, all 8 passes ratified:

- **P1** feature registry (21 new `#define FEAT_*`, `DOTS_NUM_FEATURES` 62→83); **P2** `DotsGetFeatureValue` +21 cases (each reproduces the export row-prep formula); **P3** `VWAP_Sigma_ATR` as the 172nd export column (first 171 bit-identical, `RebuildStateForExport` δ=0 preserved); **P4** rolling-buffer infra 5→81; **P5** seed + OnTick wiring (C-free `SeedDotsRollingBuffers`, bar-day driver, insert-before-refresh — exact Python-loop parity); **P6** the 4 structural constants; **P7** A/B/C retirement (first deployable-coherent state, A/B/C grep-clean); **P8** the gated diagnostic threshold export (`Dots_ExportThresholds` extern + `ExportDotsThresholdSnapshots()` writing the 163-col dump — the EA side of the parity check).

- **Final assembly** = canonical + 6 sections (1.0, 1.1, 1.2, 2.0, 6.7, 6.9); Supervisor and Auditor each independently reconstructed and byte-matched it.

- **Compile fix (FEAT_* forward reference).** First compile threw 81 `undeclared identifier FEAT_*` errors: P5 rewired OnTick (SECTION 1.2) to reference the `FEAT_*` macros, but those macros are `#define`d in SECTION 6.7, ~2,500 lines *after* their use. MQL4 resolves globals/functions file-wide but the preprocessor is positional, so the macros were undeclared at the point of use. Fix = relocate the macro block (THR_HI/LO + 83 FEAT_* + DOTS_NUM_FEATURES) ahead of first use. The running `equiDOT.cs` places it in the preamble, directly below `#property strict` (co-located with `DOTS_NUM_RULES`) — forward-ref audit = 0, compiles 0/0. **Standing lesson: section-level diffs and byte-matching cannot see a cross-section preprocessor-ordering dependency; only the compile catches it. A "macro used before its #define position" audit is now a required check on any cross-section macro move.**

**S.6 parity result (live == sim):** the EA threshold dump vs the `dots_thresholds.py` oracle, joined on Time, both warmed from empty over the identical 6,900-bar seed window (eligibility 4,778 — matched the seed log exactly), all 81 D features × 2 sides per bar at 6dp = 1,117,800 cells.
- **79 / 81 features: perfect, per-bar, all 6,900 bars.** Construction validated.
- **2 features fail** (both in the 76-signal ship set):
  - **`KAMA_Dist`** (5 of the 76 triples) — **export precision.** Construction is correct: parity is *exact at 2dp* (0 mismatches), breaks only at 6dp. The export writes `KAMA_Dist` (and `KAMA_Value`) at `Digits` = 2dp (L981/L979), but the EA buffers full-precision `Close − KAMA` (L4549), and KAMA carries sub-cent precision. The 2dp export column can't represent the EA's value, and the oracle can't recompute it (KAMA_Value is also 2dp).
  - **`Volume_Ratio_10`** (2 of the 76 triples) — **averaging-window bug** (real value divergence; mismatches at 2dp too). Export averages bars `i+1..i+10` (L810–818): `Volume[i] / avg(i+1..i+10)`. Calc averages `i..i+9` (L3727–3734): `Volume[i] / avg(i..i+9)` — off by one and includes the current bar in its own denominator. The EA uses the calc value live; discovery used the export column → live ≠ sim.

### Two fixes for total parity (best-result only; both EA changes → full pipeline)

1. **`Volume_Ratio_10` — align the calc to the export window.** Change `Calc_Dots_Derived_OnBar` (L3727–3734) to average bars `i+1..i+10` (exclude the current bar), matching the export. The live EA then reproduces exactly what discovery validated; the export column and the recon baseline are unchanged, so **no re-discovery is needed**. (This is also the standard "current vs prior-N" ratio.)
2. **`KAMA_Dist` — export at 6dp.** Change the export row-prep (L981) from `DoubleToString(kamaDist, Digits)` to `DoubleToString(kamaDist, 6)`, bringing it in line with every other distributional feature (which match exactly at 6dp). Then re-export → rebuild the baseline with the 6dp `KAMA_Dist` column → **re-validate the 5 KAMA_Dist triples** on the fresh baseline. This is the principled, full-precision result, consistent with the standing ATR/value-precision rule (use the 6dp column; never the truncated one).

Sequence: both fixes go Developer → Supervisor → Auditor → human-confirm → recompile → re-export → re-run the S.6 parity (must come back 81/81). Fix 1 closes immediately; fix 2 also triggers a fresh baseline + re-validation of the 5 affected triples. After 81/81 parity, EXECUTION SEQUENCE step 5 is fully closed and Stage 5 (D2D study) opens.

---

## 2026-06-25 — Candidate completeness audit → 117 (90 FEAT_ + 27 equality)

A directive-reconciliation (A2.7) review of the full 172-column export against the "fullest possible market perspective" standing directive. Of 171 feature columns (Time excluded), 83 were candidates and 88 were excluded; the audit classified all 88 by a structural test — a column can be a candidate only if it is (a) stationary enough that a rolling percentile is meaningful and (b) not an exact duplicate of an existing candidate.

**Result: vocabulary expanded 83 → 117 candidates (249 scan conditions: 90×2 hi/lo + 69 equality values).**

- **Group A — 83 existing FEAT_** (mechanism D / structural, hi/lo). Unchanged.
- **Group B — 7 continuous adds** (mechanism D, hi/lo), all already exported (no new column): ADX_Value, Body_Size, Upper_Wick, Lower_Wick, TChan_A15, VWAP_Sigma, Volume. `DOTS_NUM_FEATURES` 83 → 90. Cost is low and **folds into the same EA window as the two parity fixes** (already-exported values → +7 `#define FEAT_`, +7 `DotsGetFeatureValue` cases, +7 rolling buffers, +7 `dots_thresholds.py` routing rows; no Calc, no export-schema change).
- **Group C — 27 binary/state/side equality candidates** (`==value`, Python-only, no FEAT_/buffer, no export change): D2D_Signal, D2D_DirStep, OBVf_Signal, OBVf_Trend_Dir, Harmonic_OBVf_Concordance, Harmonic_D2D_Concordance, ADX_Rising, Sqz_State, RangeOsc_State, PoC_Side, ST_Flip_Event, Trend_Concordance, Trend_Conflict, AT_Regime_ST, AT_Regime_LT, VWAP_Side, VAH_Side, VAL_Side, PrevDay_High_Side, PrevDay_Low_Side, PrevDay_Close_Side, DailyOpen_Side, OR_High_Side, OR_Low_Side, Session_High_Side, Session_Low_Side, WeeklyOpen_Side. These do **not** route through `dots_thresholds.py` (S.6); they are deterministic value-matches on already-exported columns and require the **new equality-condition type** in the discovery/sim/wf engines (the step-8 rekey).

**Excluded with documented structural cause** (per A2.7 completeness): raw OHLCV + non-stationary price/line/band/EMA/cumulative columns (drift ~1.8 std), the four `OBVf_*` columns byte-identical to D2D (`OBVf_ATR/ATR_MA/DirStep/Persist`), `Hist_Volume≡Volume`, `OBV_Zero_Value≡OBV_Line`, `TChan_B5≡OBV_Line`, `ATR_Assigned` (2dp twin of ATR_1M), `KAMA_Side`/`Harmonic_Sign` (100% sign-twins of candidates), `DecayState_ST/LT` + `Lock_Time` (dead/empty), `D2D_Trend_Dir` (the mandatory directional gate). Time fields (EST_Hour/Minute/DayOfWeek) held out as a filter dimension, not convergence ingredients (high-cardinality time equality is curve-fit-prone).

**EA-wiring flag (separate track):** the four `OBVf_*` columns carrying D2D values byte-for-byte in the *native* export indicates the OBVf channel's ATR/persist/step machinery is aliased to D2D — either intended or a latent bug. If the OBVf indicator is meant to compute these on the OBV signal line, fixing it would convert four dead duplicates into real distinct candidates (a gain). Logged for a separate EA-wiring investigation; do not delete the columns.

**A2.7 catch (consistency):** the expansion was written into `non_negotiables_supervisor.txt` (117, S.10, A2.7) but `non_negotiables_developer.txt` and `non_negotiables_auditor.txt` were not updated (0 refs to 117 / A2.7 / 90 FEAT). The gate is half-armed; both must be synced before the next EA window so the Developer builds and the Auditor verifies against 117.

---

## 2026-06-25 — Project file audit & data lineage

Full-folder audit. Data lineage — folder since consolidated to one live generation; the prior export and baseline generations have been removed:

- **CURRENT baseline:** `equiDOT_recon171_step7_part1..8` (171-col, Jan 19 → Jun 25 18:18, 152,983 rows; 6dp `KAMA_Dist`/`ADX_Value`/`VWAP_Sigma`, ADX_Value full-precision recompute in Jan–Apr) — the sealed clean baseline Stages 5–8 run on; S.6 parity oracle (88/88). Supersedes the prior `equiDOT_recon171_part1..8` (150,599 × 171, removed).
- **CURRENT build-inputs to `core.py`:** `first..fourth.csv` (126-col original, Jan 19 → Apr 27 — only its OHLCV is used; the Jan–Apr segment is reconstructed from these every rebuild) + `64_256_*` (172-col fresh export, Apr 6 → Jun 25 18:18 — the recent-segment oracle; `core.py` stitches the 171-wide reconstruction and writes `reconstructed_171_step7.csv`).
- **CURRENT latest export:** `64_256_*` (172-col, Apr 6 → Jun 25 18:18 — the step-7 fresh export; current `core.py` input, with the EA threshold snapshot `equiDOT_thresholds_US30_cash_1.csv` as the S.6 parity reference).
- **REMOVED (all prior generations, no remaining value):** the 126-col generation `64_186_*` (4) + `equiDOT_recon_part1..8` (8); the 171/172-col Apr–Jun exports `64_236_*` (4) + `64_246_*` (4) that built the prior baseline; and `equiDOT_recon171_part1..8` (8, 150,599 × 171). All superseded by the `64_256` / `recon171_step7` generation; no pipeline references them. `core_step7.py` folded into `core.py` (current).

Docs updated this audit: `equiDOT_README.md` (rewritten to mechanism D + structural pair, 172-col export / 171-col baseline, 117 candidates / 249 scan conditions, 2500/6900, DOTS_NUM_FEATURES=90) and `equiDOT_adaptive_thresholds_stage.md` (status → COMPLETE, candidate population → 90 FEAT + 27 equality = 117); the three non-negotiables synced (step-6 + clean-baseline state). `DOTS_76_handover_blueprint.txt` (June 22, pre-unification) and `DOTS_76_performance_stats_final.xlsx` (pre-clean, invalid stats) removed — no longer pending updates. `DOTS.cs` kept as cited legacy reference; `recommended_set_76.csv` + `DOTS_76_signal_dictionary/overview.xlsx` kept as Stage-8 benchmarks. Remaining gaps: `wf.py` is net-new and not yet written; the discovery/sim tools (`full_766K_convergence_discovery.py`, `portfolio_simulation_engine.py`) still point at absent inputs (`65K.csv`, `DOTS_60_final_signals.csv`) and won't run until the Stage-10 rekey; the project's standing custom-instructions block is pre-equiDOT (26-rule) and frames every instance — highest-impact staleness, should be rewritten to the equiDOT/117 state.

---



## EXECUTION SEQUENCE — Auditor sign-off → live

*The ordered path from the Stage-4 Auditor verdict to a live, scaled deployment. **The adaptive-threshold EA change for the new variables is the next major work — it comes first, ahead of the D2D study and the tool rekey, gated only by the non-negotiables rewrite.** It is NOT batched with the conditional D2D recalibration (batching would force it to wait on the Stage-5 study). Two structural forks remain: the D2D recalibration (conditional EA change, step 7) and Stage 9 install. The baseline is locked-for-real once the EA is frozen (after the adaptive change, and after D2D if it fires). Full mechanism map, build spec, validation pattern, and wf.py requirement: `equiDOT_adaptive_thresholds_stage.md`.*

1. **Ratify Stage 4.** The 150,599 × 171 stitched set (`equiDOT_recon171_part*`) becomes the sealed clean baseline, superseding the 146,815 × 126 set. Record the corrections folded into `core.py` (epoch via `total_seconds()`; `ATR_1M` as the dist divisor everywhere; δ=−2 shift; LLEMA deadzone → `ATR_1M`). This is the dataset the adaptive build and Stages 5–8 run on.

2. **Prep — parallel, no EA change (partly done).** ✅ `dots_thresholds.py` extended for the 19 new D vars + structural constants, validated regression-clean against the existing entries. **Next:** rework it to the unified state (route former-B/C/static features to D, remove the B/C compute and the static-global path, build the per-feature routing table). ✅ `wf.py` located — it does not exist as a persisted implementation (the four project scripts use a static-global percentile, no fold logic); it is net-new construction at step 8/9, and the threshold unification dissolves the prior B/C per-fold-refit fork. Draft the adaptive build spec into the Developer prompt.

3. ✅ **DONE — Non-negotiables rewrite — HARD GATE before any EA change.** All three docs (Developer/Supervisor/Auditor) rewritten and uploaded (gate armed) before the threshold-unification build. They encode the **unified threshold layer (mechanism D + structural constants) as the new sacred mechanism, explicitly retiring B/C/static**; plus the 21 feature-system registrations (`DOTS_NUM_FEATURES` 62→83), the new mechanism-D rolling buffers (seed/insert/day-refresh), `VWAP_Sigma_ATR` normalization, the OR-set gate — alongside the existing value/state sacred layer (VAH/VAL migration, the 15 arrays, InitBars=6900 floor, the stateful boundary-walk functions, the four Stage-3 touches, the KAMA-warm-start / cold-start family). No EA is touched until this is in place.

4. ✅ **DONE (2026-06-25) — ADAPTIVE THRESHOLD EA CHANGE (threshold unification) — first EA change, highest priority.** Built across 8 ratified passes, assembled, compiles 0/0 (see the 2026-06-25 record above). Unify every Stage-8 candidate threshold onto causal mechanism D (rolling-2500, day-refreshed percentile) for distributional features and structural constants for the natural-boundary set (VWAP_Z ±2, OR_Position gated). **Retire mechanism B (ATR-scaled fitted base), mechanism C (regression-EMA fitted coefficients), the static-global percentile path, and the ~100 stale hardcoded constants** — all calibrated on broken pre-reconstruction data and/or look-ahead. Wire the 21 new candidates; add `VWAP_Sigma_ATR` = VWAP_Sigma/`ATR_1M`. **No exported variable value changes** (the 171 columns stay bit-identical); no value/state Calc change; no new signal rules. Through Developer → Supervisor → Auditor → human-confirm. (Full spec: `equiDOT_adaptive_thresholds_stage.md`.)

5. ✅ **DONE — Re-export + validate (parity 88/88).** Closed via steps 6–7 of the EXECUTION SEQUENCE file: the two parity fixes + 7 Group-B adds shipped (EA window, 0/0), the baseline was re-exported/rebuilt/sealed (`equiDOT_recon171_step7_part*`, 152,983 × 171), and S.6 parity reached 88/88 at 6dp. See the 2026-06-25 “Steps 6 & 7 CLOSED” record above.

6. **Stage 5 — D2D band-calibration study** (analysis-only, no EA change; runs in parallel with steps 2–5, independent of the threshold work). Quantify whether `D2D_Trend_Dir` — the mandatory 4th directional gate on all triples — is correctly aimed on real data. Output: recalibration warranted or not, with recommended bands if yes.

7. **FORK — D2D recalibration (conditional, separate EA window).** From the Stage-5 evidence. If warranted: non-negotiables amendment → D2D recalibration EA change (Developer → Supervisor → Auditor → human-confirm) → re-export → rerun `core.py` → Auditor re-validates. If not warranted: skip; the post-adaptive baseline is already final. **After this point the EA is frozen and the dataset is FINAL.**

8. **Rekey the tools** (`full_766K_convergence_discovery.py`, `portfolio_simulation_engine.py`, `wf.py`) to the final dataset — now downstream of the EA changes. The 45 new variables (+ `VWAP_Sigma_ATR`) become eligible Stage-8 convergence ingredients on validated thresholds.

9. **Stage 7 — execution-parity audit.** Reformat the final stitched OHLCV to History-Center format, run the frozen EA on a non-FTMO MT4 build via the strategy tester, and confirm `engine.py` reproduces the EA's actual execution (entries, LeapFrog, BE nudge, Friday 16:45/17:00 close, spread, position cap, FTMO rules). Closes the one unverified layer — data fidelity is proven, execution parity is not yet.

10. **Lock the walk-forward folds** — the Stage-8 validation protocol; survival-first, FTMO worst-day filter applied before PF.

11. **Stage 8 — definitive discovery** on the final sealed dataset (171-wide + `VWAP_Sigma_ATR`, validated live-faithful thresholds). The signal set is re-derived from scratch — the prior 76 lived on the broken/partly-broken data and is not a valid baseline.

12. **Validate the discovered set** through the locked folds — OOS-dominant across every fold, survival-first.

13. **Stage 9 install** — final signals (the triples that use the new features) + trade management into the EA, through the gate. EA change → final non-negotiables amendment for any Stage-9 sacred additions before deployment.

14. **Execution-parity re-verification** on the deployment build (strategy tester vs `engine.py` for the final EA).

15. **One-week demo forward test** on live data; re-run the sim over that same week; confirm demo == sim.

16. **Go live at minimum lot size** on one FTMO account — real P&L, monitored against the survival constraints (worst-day filter, convergence behaviours holding). **Scale** to the other two accounts and larger size only once live behaviour matches the sim.

*Discovery (Stage 8) is constrained to combinations of the existing 171 variables. Inventing a genuinely new variable — not just a new signal combination — reopens a Stage-3-style EA change and another re-export loop, which is why new-feature invention is out of scope past Stage 4.*

---

## DECISION — Threshold unification (locked)

**Every Stage-8 candidate threshold is unified onto two causal mechanisms: mechanism D (rolling-2500, day-refreshed percentile) for distributional features, and structural constants for the few with a natural fixed boundary. Mechanism B (ATR-scaled fitted base), mechanism C (regression-EMA fitted coefficients), the discovery static-global percentile path, and the ~100 stale hardcoded `dots_threshold` constants are retired as discovery thresholds.**

Rationale (criterion: no deferred contamination, best Stage-8 outcome):
- Resolving only the new variables or only B/C would leave the static-global percentile path (look-ahead over the whole dataset, every test fold) and the hardcoded constants (calibrated on the broken δ+2/cold-start data) in place — deferred contamination. The criterion forbids that, so it forces the full unification.
- D is the unique mechanism satisfying all three constraints at once: **causal** (rolling window holds only past eligible bars; structural constants are fold-independent), **not stale** (computed live on the clean reconstructed data), and **live==sim** (EA and `dots_thresholds.py` run the identical computation).
- C's cross-feature coupling was an unproven, broken-data-fit hypothesis; it is not inherited into Stage 8, but may return as a validated Stage-9 refinement if the clean discovery surfaces it.
- **Consequence:** the prior B/C per-fold-refit walk-forward fork dissolves (no fitted constants remain → every candidate is causal by construction). No exported variable *value* changes (thresholds drive firing, not the 171 value columns), so the Stage-4 value seal holds; the change is confined to the threshold mechanism + 21 new feature registrations. End state: all 83 candidates on D or structural; zero fitted or stale thresholds.

---

## DONE ✅

### EA Code Fixes (all compiled 0/0, passed Developer → Supervisor → Auditor)

- ✅ **Export δ=0 rebuild (Section 1.1)** — `RebuildStateForExport()` standalone function inside `ExportDataForAnalysis()`, reseeds all state arrays at δ=0 before the CSV dump loop. Covers both OnInit auto-export and manual `btnDump`.
- ✅ **HarmVol EMA cold-start seed (Section 6.4)** — Relocated `if(i+erPeriod>=Bars) return;` guard from before the EMA block to after the EMAOsc line. EMAs now compute across full history with Close seed. Fixed: first 50 rows had EMA8=EMA21=EMAOsc=0.0.
- ✅ **KAMA anchor-seed (Section 6.4)** — Changed KAMA seed from `Close[i]` to `U_BasisBuffer[i]` (D2D_Basis) with Close fallback. Reduces cold-start convergence from ~74 bars to ~5 bars. Created load-bearing DAG constraint: D2D_ST must compute before HarmVol.
- ✅ **PoC cold-start (Section 7.5)** — Replaced history-dependent `iBarShift`/`iTime` day-boundary lookup with local timestamp walk `while(startBar+1<Bars && (Time[startBar+1]/86400)==(dt/86400)) startBar++`. PoC now resets daily, history-independent. Fixed: PoC was frozen at 47334.3 for 61% of bars.
- ✅ **KAMA persistence module (Section 1.4)** — Complete module: `WarmStateFileName()`, `WriteKamaWarmState()`, `ReadKamaWarmState()`, `ValidateKamaWarmState()`. Binary file with magic number, version, timestamp, KAMA value. Four-way cold-case advisories (predates, future, in-range missing, within oldest 50 bars). `HARMVOL_ER_PERIOD` shared `#define` (50). `Warm_KAMA_Deep_Bars` extern (50000). Suppression state init (warm→false/0, cold→true/N).
- ✅ **Warm-anchor branch (Section 6.4)** — Three-arm if/else: `if(g_warm_valid && g_warm_anchor_enabled && Time[i]==g_warm_ts)` → saved KAMA; `else if(isSeedBar)` → D2D_Basis/Close; `else` → KAMA recursion. Anchor is mathematically exact — no settling period.
- ✅ **HARMVOL_ER_PERIOD repoint (Section 6.4)** — `int erPeriod=50;` → `int erPeriod=HARMVOL_ER_PERIOD;`. Single-sourced constant shared between Section 6.4 and Section 1.4's anchorability guard (`matchBar<=Bars-1-HARMVOL_ER_PERIOD`).
- ✅ **OnInit wiring (Section 1.0)** — `ReadKamaWarmState()` + `ValidateKamaWarmState()` before the Pass 2 loop. Ordering verified: Read→Validate before first `Calc_HarmVol_LLEMA_OnBar` call.
- ✅ **Export cold-guard (Section 1.1)** — Save `state_HarmVol_KAMA` before `RebuildStateForExport`, set `g_warm_anchor_enabled=false`, run cold rebuild + CSV dump, restore array, set `g_warm_anchor_enabled=true`. Export is always cold regardless of warm state.
- ✅ **OnTick write hook (Section 1.2)** — `WriteKamaWarmState(Time[1], state_HarmVol_KAMA[1])` after each committed bar. Gated on `g_warm_valid || Bars>=Warm_KAMA_Deep_Bars`. Warm runs save every bar; deep first deploy seeds the file; cold shallow runs never overwrite a good file.
- ✅ **OnInit write (Section 1.0)** — Same write call after `ExportDataForAnalysis()`. First-time 65K deploy creates the file during OnInit without waiting for a live bar.
- ✅ **KAMA signal suppression (Section 6.9)** — `DotsRuleUsesKama(r)` helper tests feat1/feat2/feat3 against `FEAT_KAMA_Dist`/`FEAT_KAMA_Dist_ATR`/`FEAT_KAMA_Slope`. Skip line in eval loop: 13 rules suppressed on cold starts (rules 6,7,15,18,23,29,32,37,54,61,67,70,75). 63 trade normally.
- ✅ **Suppression lifecycle (Section 1.2)** — Counter decrement floored at 0. Clear only when `g_warm_valid==true` AND counter<=0. Cold starts: suppressed all session (`g_warm_valid` never true). Warm starts: no suppression (§1.4 init sets false/0 on warm). D3 skipped — confirmed no-op (warm anchor is exact, no settling period).
- ✅ **Export restore fail-safe (Section 1.1)** — At copy-back: `if(ArraySize(state_HarmVol_KAMA)==kamaBackupSize)` → restore; else → `g_warm_valid=false`; `g_warm_suppress_kama_signals=true`. Routes to cold-suppression, prevents writing bad data.
- ✅ **Boot-console logging (Section 1.4)** — Print statements in `ReadKamaWarmState` (no file / magic mismatch / loaded), `ValidateKamaWarmState` (matched bar + anchored / cold result), `WriteKamaWarmState` (seed written with time+KAMA). Every boot path narrated.
- ✅ **Buffer/InitBars tuning** — `Dots_RollingBufferSize` 5000→2500, `Dots_InitBars` 6900→4000. Halves computational load. Percentile thresholds stable at 2500. Production chart floor: 4K bars (5K setting for margin).

### Deploy Verification (all passed)

- ✅ **65K cold deploy** — `.bin` created, CSV exported, δ=0 confirmed (lag-0 residual 0.005 vs ~3 at lag±1), KAMA exact (max residual = rounding floor), EMA seeds near Close (no zero-head), PoC resets daily (1,016 distinct values).
- ✅ **.bin file decoded (base64)** — magic 0x6B616D77, version 1, timestamp 2026-06-18 20:19, KAMA 51683.10. Structurally correct.
- ✅ **2K warm-start** — file found, timestamp matched at bar 7, anchored KAMA=51689.11, WARM start active, all 76 signals from bar 1. Per-bar write hook firing.
- ✅ **7K depth test** — all signal-relevant variables match 65K baseline. Rolling buffers seed identically (scan capped at 6900→4000).
- ✅ **5K export at final config (2500/4000)** — 116-column sweep: 89 clean by pos600, all 6 signal-relevant variables converged before trading edge. Warm boot confirmed, export cold (cold-guard worked).
- ✅ **Production config (pre-Stage-3): 5K chart / 2500 buffer / 4000 InitBars.** Stage 3 raised the chart cap to 7,000 and `Dots_InitBars` to 6,900 (deepest new-variable lookback = WeeklyOpen ≈ 6,900 M1 bars); `Dots_RollingBufferSize` stays 2,500 — that is the percentile-threshold window (unchanged), and no exported column depends on it (export is raw calcs + SuperTrend states).

### Python Pipeline (all passed Developer → Supervisor → Auditor)

- ✅ **dots_thresholds.py** — Shared module replicating all four EA threshold mechanisms: static (existing global percentile path), ATR-scaled (25 bases × ATR_1M/15.04), regression-EMA (8 formulas, α=2/1001, 10 raw inputs), rolling-2500 floor-index day-refreshed (6 sides, `sorted[floor(count*pct)]` not `np.percentile`). 39 adaptive keys. Frozen coefficients transcribed from EA source.
- ✅ **full_766K_convergence_discovery.py** — Wired: imports dots_thresholds, calls `compute_adaptive_thresholds(df)` on full pre-trim dataframe, routes adaptive (variable, side) pairs to per-bar arrays, static to global percentile. Warmup = entry exclusion mask (first 4000 rows), not row drop.
- ✅ **portfolio_simulation_engine.py** — Wired: same adaptive routing, same warmup-as-mask approach. Elementwise threshold comparison for adaptive variables.
- ✅ **Verified:** 6 functional invariants on real data (alignment, 4000-row exclusion, per-side routing, static unchanged).

### Friday-Close Parity Fix (passed Developer → Supervisor → Auditor)

- ✅ **Both Python sims** (`full_766K_convergence_discovery.py`, `portfolio_simulation_engine.py`) — Force-close and entry block moved from 17:00 to 16:45 EST via `(dow==5)&((hr>16)|((hr==16)&(min>=45)))`, flatten ALL trades (BE-nudged exemption removed), `EST_Minute` threaded through. Mirrors EA L8043 `fridayClose`. Closes the backtest↔live exit-timing gap. Both gates PASS.

### Data Reconstruction — clean stitched Jan 19 → Jun 19 dataset (passed Developer → Supervisor → Auditor)

**This is the gating deliverable for fresh discovery. The full decision/method record is in the section "DATA RECONSTRUCTION — DECISIONS & METHOD" below.** Summary:

- ✅ **Built independent Python Calc** mirroring `equiDOT.cs` (KAMA/HarmVol §6.4, PoC §7.5, derived block L660–727) — verified against source, not assumed.
- ✅ **Gate 1 (Calc ≡ NEW export):** all 15 cold-start columns reproduce the new export at δ=0 to the documented residual classes (PoC/PoC_Side/KAMA_Side bit-exact beyond the partial day; KAMA/EMA at 2-dp print floor).
- ✅ **δ correction:** OLD state columns carried a uniform +2 offset; correction = `shift(-2)`. Empirically unanimous across all 61 detectable state columns. 12 columns kept at δ=0; 98 shifted (97 numeric + Lock_Time); 15 recomputed.
- ✅ **Gate 2 (corrected-OLD ≡ NEW on warm overlap, 15,260 rows):** medians at the noise floor.
- ✅ **Stitch:** corrected-OLD up to the seam + NEW from the seam = **146,815 × 126**, 2026.01.19 15:49 → 2026.06.19 02:40, strictly increasing, 0 dups, 0 NaN, 32 integer columns clean.
- ✅ **Two review-stage defects found & fixed:** (1) Hist_Volume was mislabeled raw — it is a δ+2 column, now shifted −2; (2) `shift(-2)` float-promoted 32 integer columns — restored to int64.
- ✅ **OBV accumulator level offset** (OBV_Accum/Fast/Slow, ~−38,967.8): resolved non-blocking — discovery's `EXCLUDE_COLS` excludes these; consumed OBV features (OBV_Macd, OBV_Velocity, OBV_Line) are clean δ+2.
- ✅ **Verified by all three instances** (Developer built; Supervisor + Auditor each independently reconstructed from source and compared). Auditor verdict: PASS.

### Project Cleanup

- ✅ Obsolete files removed (old scripts, intermediate exports, stale signal files).
- ✅ Non-negotiables confirmed SACRED versions in project folder.
- ✅ README rewritten to current state.
- ✅ New export verified on warm boot (76/76, cold export, δ=0).

### Stage 3 — EA change #1: new variables (COMPLETE — pending section-9 Auditor confirm + compile)

Nine build passes, each Developer → Supervisor → Auditor → human-confirm; every existing line bit-identical, sacred layer preserved, no `i*()` in any exported path. Export: **126 → 171 columns (45 new).**

- ✅ **Globals (§3.0)** — `LevelDayAnchorHourEST=18` + 15 `hist_` arrays (VWAP price/sigma, VAH/VAL, PrevDay H/L/C, DailyOpen, OR H/L, Session H/L, WeeklyOpen, MultiDay slope/position).
- ✅ **InitBars (§2.0)** — `Dots_InitBars` 4000 → 6900 (WeeklyOpen depth ≤ 7,000 chart). `Dots_RollingBufferSize` 2500 unchanged.
- ✅ **ResizeAllArrays (§6.0) [sacred-touch 1]** — 15 additive registrations; all existing registrations bit-identical.
- ✅ **Calc_VWAP_OnBar** — session, close-weighted, server-day; VWAP + volume-weighted σ. No `i*()`.
- ✅ **Calc_RefLevels_OnBar** — EST-anchored (day 18:00 ET, week Sunday 18:00 ET, DST-aware per-bar offset; epoch-verified): DailyOpen, Session H/L, OR H/L (09:30–10:30 ET), PrevDay H/L/C, WeeklyOpen.
- ✅ **Calc_MultiDay_OnBar** — 2-trading-day (2760-bar) OLS slope (ATR-normalised, slope>0 = rising) + position-in-range; translation-invariant under window clamp.
- ✅ **Calc_PoC_State_OnBar VA append [sacred-touch 4]** — two additive inserts: VAH=VAL=Close default + the 70% wider-neighbour-wins expansion on the volume-of-Close profile (byte-verbatim with CalculateDailyPoC incl. `else break`); VAL ≤ PoC ≤ VAH; existing PoC bit-identical.
- ✅ **CalculateDailyPoC** — removed the legacy `iBarShift`/`iTime` count-profile VA block; visuals now read `hist_VAH/VAL_Price[0]` (single source of truth with the export; last `i*()` removed from a migrated quantity — remaining `i*()` are pure chart cosmetics, deconstruction-phase cleanup).
- ✅ **Dispatch wiring (4 paths) [sacred-touch 2+3]** — VWAP/RefLevels/MultiDay appended after every `Calc_Dots_Derived_OnBar` (7 sites: OnInit ×2, RebuildStateForExport ×1, OnTick ×2, ForceRePaintSignals ×2 = 21 calls); pass order (B.2) preserved; RebuildStateForExport δ=0 (S.4) intact — the three are accumulator-free window functions. VAH/VAL ride the already-dispatched PoC.
- ✅ **ExportDataForAnalysis (§1.1)** — header + per-row assembly for the 45 new columns (`>0` warmup-sentinel guards on every level). Compiled 0/0; fresh 171-column export `64_236_*` produced (76,240 rows, Apr 6 → Jun 24) and verified. **Stage 3 closed.** *Stage-4 finding on the divisor: the EA source divides every dist-ATR by the `atrAssigned` local, and at runtime `assignedATR == ATR_1M_Array == atr_final_val` (L1993–94). The two export columns are the same value at different print precision — `ATR_1M` at 6dp, `ATR_Assigned` at 2dp — so the reproduction/recompute divisor is the 6dp `ATR_1M` column for all dist-ATR (the 45 new AND the existing `KAMA_Dist_ATR`/`Dist_To_PoC_ATR`, and the LLEMA deadzone). Do not use the 2dp `ATR_Assigned` column.*

**The 45 new columns (the export schema / Stage-4 contract):**
`VWAP_Price, VWAP_Sigma, VWAP_Dist_ATR, VWAP_Side, VWAP_Z | VAH_Price, VAH_Dist_ATR, VAH_Side, VAL_Price, VAL_Dist_ATR, VAL_Side, VA_Position | PrevDay_High (+Dist_ATR,+Side), PrevDay_Low (+Dist_ATR,+Side), PrevDay_Close (+Dist_ATR,+Side) | DailyOpen_Price, DailyOpen_Dist_ATR, DailyOpen_Side | Round_100_Dist_ATR, Round_500_Dist_ATR, Round_1000_Dist_ATR | OR_High (+Dist_ATR,+Side), OR_Low (+Dist_ATR,+Side), OR_Position | Session_High (+Dist_ATR,+Side), Session_Low (+Dist_ATR,+Side) | WeeklyOpen_Price, WeeklyOpen_Dist_ATR, WeeklyOpen_Side | MultiDay_Slope, MultiDay_Position`

Convention: each level → Price + Dist_ATR + Side, **all EA-computed via the `atrAssigned` local (≡ the `ATR_1M` column at runtime — see the divisor note above; the `ATR_Assigned` column diverges and is not the divisor)** (so the dist columns can't be losslessly recreated downstream without re-introducing float drift — they are exported, not derived later). The "~38" in the Stage-2 sketch became 45 under the fully-consistent convention; inclusive per the variable-inclusion philosophy.

### Stage 4 — re-export + Python re-reconstruction (BUILT & VALIDATED — awaiting the Auditor verdict on `core.py`)

The recent period was re-exported natively by the Stage-3 EA (`64_236_*`, 76,240 rows, Apr 6 → Jun 24, 171 cols) — the oracle. The old period (Jan 19 → seam) is reconstructed in Python by `core.py`, then stitched.

- ✅ **45 new columns validated against the oracle** (overlap, rows past warmup): every Price 100% exact, every Side 100%, all positions (VA / OR / MultiDay) 100%, VWAP_Z ≈ 99.6%, MultiDay_Slope ≈ 99.9%; VAH/VAL/PoC bit-exact (also matches the existing `PoC_Price`, confirming `Point=0.01`, the 5000-bin cap, and the 70% expansion). Dist-ATR columns exact in formula (divisor `ATR_1M`); residual is 6dp rounding + small-ATR amplification only.
- ✅ **`core.py` = the consolidated 171-column reconstruction** (supersedes `equiDOT_reconstruct_supervisor.py`, which **must be deleted from the project** — stale 126-col with the wrong `ATR_Assigned` divisor). It: δ=−2 shift-corrects the original state columns (RAW0 held at 0); recomputes the 15 cold-start families (KAMA/HarmVol/PoC), dividing the two dist columns **and the LLEMA deadzone** by `ATR_1M`; computes the 45 new columns from OHLCV + `ATR_1M`; stitches at the Apr 13 seam. VAH/VAL JIT-compiled (numba) for the full-history run.
- ✅ **Four corrections discovered & baked in:** (1) epoch via `total_seconds()` — `Series.astype('int64')` returns µs on this pandas build (a 1000× error that collapses every bar into a single server-day); (2) `ATR_1M` is the dist divisor everywhere — the `families()` recompute had used `ATR_Assigned` (wrong). **Precise mechanism (Auditor root cause):** L1993–94 sets `ATR_1M_Array` and `assignedATR` to the *same* `atr_final_val`; the two export columns differ only by print precision — `ATR_1M` at 6dp (L906), `ATR_Assigned` at 2dp (L907). Same value; the 2dp column is just the rounded one, so the 6dp `ATR_1M` is the faithful full-precision proxy (the earlier "column diverges" framing was imprecise — it's a print-precision difference, k=0 only, not a δ shift). (3) δ=−2 confirmed correct (corrected-old `ATR_1M` vs new: overlap median diff 0, vs median 2.09 unshifted). (4) **LLEMA deadzone → `ATR_1M`** (post-audit fix): the EA deadzone (L3220–22) divides `assignedATR/30`; `core.py` had used the 2dp `ATR_Assigned`, putting 6 borderline bars on the wrong side of the threshold → 6 `Harmonic_Sign` flips. Switching the deadzone to the 6dp `ATR_1M` (faithful proxy of the same full-precision `assignedATR`) eliminates all flips and makes `core.py` internally consistent (ATR_1M for divisor *and* deadzone).
- ✅ **Stitched baseline:** 150,599 × 171, 2026.01.19 15:49 → 2026.06.24 01:19, strictly increasing, 0 dups, 0 NaN, column order ≡ oracle. Seam at Apr 13 01:05 = the real Fri 04.10 23:49 → Mon 04.13 weekend gap (identical OHLCV in both sources). Split to `equiDOT_recon171_part1..8.csv` (header on part 1; parts 2–8 headerless, chronological; reassembles bit-identically).
- ✅ **Auditor independent validation of `core.py` — PASS (all six checks).** Independently reconstructed from `equiDOT.cs`; root-caused the ATR print-precision mechanism and the deadzone flip cause; confirmed the 45-col logic mirrors the EA, divisor `ATR_1M`, epoch→EST_Hour 100%, δ=−2 with the 12 RAW0 exemptions, the weekend-gap seam, and output bit-identical to the uploaded baseline; independent VAH/VAL/PoC recompute 0/0/0 mismatch. The deadzone fix above was the Auditor's one actionable finding, now applied. → EXECUTION SEQUENCE step 1 (ratify) on re-upload of the corrected files.
- **Residuals:** the Harmonic sign-flips are **resolved** by the deadzone fix (GATE-1 Harmonic max → 0). Remaining: the Jan 19 warmup head only — first EST-day PrevDay=0, first partial week WeeklyOpen truncated, leading EMA/PoC cold-start transient that collapses to the print floor by skip-60. Expected, confined to the warmup head.

---

## DATA RECONSTRUCTION — DECISIONS & METHOD

*Complete record so any instance can understand why the data exports were rewritten and exactly how.*

### The problem (why reconstruction was needed)

Two MT4 exports exist for the discovery window:

- **OLD** — `first.csv`, `second.csv`, `third.csv`, `fourth.csv` — 93,988 rows, 2026.01.19 15:49 → 2026.04.27 05:15. OHLCV is correct. But its feature columns are wrong in **two independent ways**: (a) the adaptive/cold-start family (KAMA/HarmVol/PoC) was computed during a cold boot with wrong warm-state, and (b) the export carried a uniform **+2 row-offset (δ) bug** on its state columns (a feature row holds the value belonging to the bar two rows later).
- **NEW** — `64_186_first.csv`, `64_186_second.csv`, `64_186_third.csv`, `64_186_fourth.csv` — 72,456 rows, 2026.04.06 19:16 → 2026.06.19 02:40. Clean, δ=0, produced by the current sealed build (the post-fix EA) — this is the oracle.
- **Overlap** — 19,629 shared timestamps (Apr 6 → Apr 27).

Fresh discovery needs one continuous Jan 19 → Jun 19 dataset where every feature column matches what the current EA produces, with no warmup exclusion. Neither export alone provides that: OLD has the early history but broken features; NEW has correct features but starts Apr 6.

### The strategy

Rebuild OLD's feature columns to match the current EA, then stitch corrected-OLD (Jan 19 → seam) to NEW (seam → Jun 19). Every one of the 126 columns falls into exactly one of four buckets:

| Bucket | Count | Treatment |
|---|---|---|
| Time | 1 | unchanged |
| Raw / on-row features (δ=0) | 12 | kept as-is |
| Cold-start-broken features | 15 | **recomputed** from OHLCV + corrected inputs |
| Other state features (δ+2) | 98 | **`shift(-2)`** (97 numeric + Lock_Time) |

### Decision 1 — Which 15 columns to recompute, and from what

The cold-start-broken family is recomputed from scratch using the OLD OHLCV (which is correct) plus the δ-corrected state inputs (D2D_Basis, `ATR_1M` (dist divisor), ATR_Assigned (deadzone), OBVf_Signal, D2D_Trend_Dir). The 15: `KAMA_Value`, `KAMA_Slope`, `KAMA_Dist`, `KAMA_Dist_ATR`, `KAMA_Side`, `HarmVol_EMA8`, `HarmVol_EMA21`, `EMA_Oscillator`, `Harmonic_LLEMA`, `Harmonic_Sign`, `Harmonic_OBVf_Concordance`, `Harmonic_D2D_Concordance`, `PoC_Price`, `Dist_To_PoC_ATR`, `PoC_Side`.

EA source mirrored exactly (verified against `equiDOT.cs`, not assumed):

- **§6.4 `Calc_HarmVol_LLEMA_OnBar` (L3025–3076):** EMA8 α=2/9, EMA21 α=2/22, EMA_Osc=EMA8−EMA21 (seeded at Close on the oldest bar). erPeriod=`HARMVOL_ER_PERIOD`=50. ER: direction=|Close[i]−Close[i+50]|, volatility=Σ|Close[k]−Close[k+1]| over 50, sc=(er·(2/3−2/31)+2/31)². Deadzone divisor 30 (`KAMA_DeadZone_Sensitivity=AGGRESSIVE`, L1382). KAMA seed = `U_BasisBuffer` (D2D_Basis = EMA-13 α=2/14, `U_emaLen`=13 at L1292/L2347) else Close. Recursion KAMA[i]=KAMA[i+1]+sc·(Close[i]−KAMA[i+1]). LLEMA=rawSlope unless |rawSlope|≤ATR_Assigned/30 → 0.
- **AS_SERIES → CSV mapping:** the export omits the absolute-oldest bar, so CSV row 0 = i=Bars−2. The return-guard (L3047) leaves **KAMA=0 on CSV rows 0–48**, the seed lands at **row 49** (= erPeriod−1, n-independent), recursion runs from row 50. This produces an unavoidable leading EMA transient (~first 150 rows) from the single omitted seed bar — deep in warmup, dropped by discovery's 4000-bar mask.
- **Derived block (L660–727):** KAMA_Slope=kama−kama[i+1]; KAMA_Dist=Close−kama; **KAMA_Dist_ATR and Dist_To_PoC_ATR are computed via the `atrAssigned` local in source (L672/L3071), but the reproduction/recompute divisor is the `ATR_1M` column** (runtime `assignedATR ≡ ATR_1M`; the `ATR_Assigned` column diverges and is not the divisor — Stage-4 finding, confirmed empirically: `/ATR_1M` meanAbs 2.7e-7 vs `/ATR_Assigned` 8.1e-4); KAMA_Side=sign(Close−kama). Harmonic_Sign=sign(LLEMA); Harmonic_OBVf_Concordance vs `OBVf_Signal` (=round(state_TChan_OC), L646); Harmonic_D2D_Concordance vs `D2D_Trend_Dir`; PoC_Side=sign(Close−PoC).
- **§7.5 `Calc_PoC_State_OnBar` (L5195–5223):** per-bar PoC over the expanding intraday window, day bucket = `Time/86400` (server-day floor), Point=0.01. **numBins capped at 5000** (= a 50-point band off the window low); Close bins beyond the cap are skipped; when all bins are empty, maxIdx stays 0 → PoC = minP. This cap is mandatory (~79% of bars mismatch without it) and is the full explanation of wide day-open bars (e.g. 2026.04.08 01:05: Close >50 pts above the bar low → PoC = bar low).

### Decision 2 — The δ correction (the +2 offset)

The offset was determined **empirically**, not assumed. For each state column, the median |OLD − NEW| residual on the overlap was measured at shift 0 vs shift −2:

- All **61 detectable state columns** (panel ADX_Value, ATR_1M, ATR_Assigned, OBV_Macd, Momentum_Value and 56 more) showed a clean **δ+2** (residual ≈0 at shift −2, large at shift 0). Unanimous — the bug is uniform.
- The **12 genuinely on-row columns** showed the opposite (residual 0 at shift 0, large at shift −2) and were kept: OHLCV, Volume, EST_Hour/Minute/DayOfWeek, Bar_Range, Body_Size, Upper_Wick, Lower_Wick.
- **Flat-on-overlap state columns** (locally constant over the overlap, so undetectable there) were defaulted to −2 under the uniform-bug prior (61/61 detectable columns agree → the bug applies to all state columns; keeping a flat-but-varying one at 0 would corrupt the non-overlap Jan 19 → Apr 6 region).

Correction applied = `df[col].shift(-2)`, which puts the 2-row NaN tail at the OLD end (Apr 27) — discarded by the seam. OHLCV exact across the overlap; Volume within 1 tick (feed difference).

### Decision 3 — The seam (where OLD ends and NEW begins)

Seam = the first NEW bar after the Apr 10–13 weekend: **2026.04.13 01:05** (clean server-day boundary; Apr 11–12 are Sat/Sun). NEW is the clean oracle, so from the seam onward the dataset is a **bit-identical NEW passthrough** (all 126 columns). Up to the seam it is corrected-OLD. Counts: corrected-OLD < seam = **80,093** (ends Apr 10 23:49) + NEW ≥ seam = **66,722** = **146,815**.

### Decision 4 — Integer dtype restoration

`shift(-2)` introduces a NaN tail that promotes integer columns to float64, and the dtype persists after the tail is dropped (concat of float-OLD + int-NEW upcasts to float). 32 integer columns were being serialized as "1.0"/"-1.0". Fix: derive the integer column set from the δ=0 NEW export's dtypes, assert no NaN in the kept region, cast back to int64 at the stitch.

### Two defects found during review

1. **Hist_Volume mislabeled raw.** It equals Volume only in NEW; in OLD it carries the +2 offset (overlap median 13–14 → 0, max 405 → 1 after shift −2). It was hardcoded into the raw-keys set, shielding it from the empirical test. Corrected: routed through the test → shifted −2. The canonical kept-at-0 set is exactly **12**; no 13th δ=0-native column exists.
2. **Integer dtype** (above) — caught in review; 32 columns restored.

### Verification — three independent gates, all PASS

- **Developer** built the reconstruction and ran both gates.
- **Supervisor** rebuilt the Calc independently from source and re-ran Gate 1, δ-detection, Gate 2, and the stitch; produced the canonical artifact; found and fixed the Hist_Volume + integer-dtype defects.
- **Auditor** rebuilt independently from source and verified bar-by-bar region provenance: ≥seam = bit-identical NEW passthrough; ≤seam = 12 exact OLD passthrough + 98 exactly `OLD.shift(-2)` + 15 recomputed bit-identical to an independent Calc. Confirmed both fixes and resolved the OBV FLAG at source. **Auditor verdict: PASS.**

Residual classes (all documented, none enter the artifact as inconsistencies): leading EMA transient (~first 150 rows, dropped by the 4000-bar warmup); 4 Harmonic_Sign deadzone-boundary flips in Gate 1 / 1 in Gate 2 (appear only in full-precision-Python vs 2-dp-stored-export comparison; |rawSlope| within ~1e-4 of atr/30; recomputed Harmonic_Sign matches 100% across the OLD region); ATR-amplification maxes at the discarded Apr 27 OLD tail.

### Deliverable files (in project folder)

- **`equiDOT_reconstruct_supervisor.py`** — the reconstruction script (independent Calc + δ-correction + stitch + integer restoration). Regenerates the dataset from the raw exports; prints both gate tables and stitch validation.
- **`equiDOT_recon_part1.csv` … `equiDOT_recon_part8.csv`** — the stitched dataset in 8 chunks (~18,352 rows each; part 8 = 18,351). **Header on part 1 only; parts 2–8 headerless; chronological** — same convention as the raw exports. Reassemble:

```python
cols=list(pd.read_csv('equiDOT_recon_part1.csv',nrows=0).columns)
dfs=[pd.read_csv('equiDOT_recon_part1.csv')]+[pd.read_csv(f'equiDOT_recon_part{i}.csv',header=None,names=cols) for i in range(2,9)]
df=pd.concat(dfs,ignore_index=True)   # 146,815 x 126, 0 NaN, 32 int columns
```

No EA code change in this phase — `equiDOT.cs` was read as the Calc spec only (no MetaEditor compile relevance).

---

## LEFT TO DO ⬜

### Principles (locked)

- **Open the search, rigorous on the findings.** Don't pre-constrain hypotheses; hold claims to a hard standard. Investigate everything: single-variable signals, classic triple + D2D direction, concurrence/density at every count, D2D married to better calibration variables, novel combinations, and the null that the original 76 are still strongest. Separate "should we look?" (always yes) from "is it overfit?" (held to the standard). **No projection in either direction.**
- **Survival first.** A system that runs within FTMO rules and survives is the objective. FTMO worst-day loss (2-lot, spread in) is a hard filter applied *before* PF; profit is selected among survivors. First live deployment at minimum size / one account.
- **Prior OOS numbers are invalid baselines.** Greedy collapse (PF ~3 / WR ~87%), conservative (PF 7–11), "density fails," and 5+→100% were on the broken (δ+2 + cold-start) data. Motivating priors only.
- **Validation = walk-forward folds**, not a single hold-out. Persistence scored on the segments where a pattern was OOS; the leader also gets an anchored train-past / test-future pass. Proposed monthly folds (Jan 19–31, Feb, Mar, Apr, May, Jun 1–19); boundaries locked at Stage 7.
- **HTF / longer-horizon elements** enter only as one optional convergence ingredient, **never a consensus/alignment gate** — a gate collapses firing frequency, which is the edge.
- **The sacred layer is untouchable without explicit human approval.** EA changes happen only in the designated build stages (3 and 6), each routed Developer → Supervisor → Auditor → human-confirm. The definitive discovery (Stage 8) runs on the final sealed dataset. *(This supersedes the earlier "discovery is analysis-only, no EA changes" framing: the perspective is completed first, then discovered on.)*

### Linear sequence (Stages 1–9)

**Stage 1 — EA & variable comprehension (foundation; before any research or change).**
Build a complete, source-grounded understanding of equiDOT before proposing to change any part of it. Claude produces; the human (the developer who built it) corrects and supplies the design intent only he can give.
- Architecture map: calc pipeline, state flow, signal/entry/exit logic, the four threshold mechanisms, the warm-start / sacred layer.
- Variable reference for the 126 columns: how each is computed, its role, typical range/values, and sacred/load-bearing status.
- Design-intent annotations: the *why* behind the choices (e.g. the D2D band relax/contract intent, the triple-convergence philosophy, why the EA went stateful, the survival-first goal).
- *Gate:* the human confirms Claude's understanding is correct before research begins. *Output:* a confirmed comprehension reference the whole pipeline inherits.

**Stage 2 — Completeness research** (analysis on the sealed 126; no EA change).
Decide whether the variable set is complete. Structural + proxy evaluation of the candidate additions (gap findings below). Tier-1 reference anchors justify largely a-priori; Tier-2 must earn it on proxy evidence. Additions are **hypothesis-driven, never bulk** — every new column inflates the discovery search space and the spurious-triple (curve-fit) risk, so each must carry a specific structural reason to matter. *Output:* a frozen build list. The human decides. *(Note: a new variable's value cannot be definitively tested before it exists — this stage justifies and prioritises; the definitive value test is Stage 8.)*

**Stage 3 — EA change #1: add the approved new-variable Calc functions** (VWAP, reference levels, value area, etc.). Sacred layer untouched. Full Developer → Supervisor → Auditor → human-confirm. — ✅ **CLOSED** (nine build passes; compiled 0/0; fresh 171-column export `64_236_*` produced and verified). See the Stage-3 DONE record above for the per-pass detail and the 45-column schema.

**Stage 4 — Re-export + re-reconstruction.** Produces the sealed **complete** 171-column dataset (all variables, full ~5-month history). — ⏳ **BUILT & VALIDATED; awaiting the Auditor verdict on `core.py`.** See the Stage-4 DONE record above for results and the three corrections.

*Revised method — the FTMO strategy tester is unavailable:* the FTMO MT4 `.exe` does not supply History-Center-formatted history, so the tester returns ~101 bars and cannot re-export the early period. Therefore:
- **New export** — the current (Stage-3) EA produces the recent period with all 171 columns natively (correct cold-start, new variables, VAH/VAL on the volume profile). This is the **oracle**.
- **Old period (Jan 19 → start of the new export)** — **cannot be re-exported**, so it is reconstructed in Python: `core.py` is extended to emit all 171 columns and run over the original first–fourth OHLCV (which is correct). Because the cold-start fixes now live in the EA logic `core.py` mirrors, this builds the existing 126 columns correctly **and** the 45 new ones in one pass — **superseding the old "δ=−2 shift + cold-start patch" correction** (nothing to offset in a clean OHLCV-up rebuild). Every new column is a pure function of OHLCV + `ATR_1M` (already an original column; the dist divisor — not `ATR_Assigned`, see the Stage-4 DONE record), so no tester/live history is needed.
- **Overlap validation (non-negotiable)** — on the bars where both the new EA export and `core.py` exist, all 171 columns must match before `core.py` is trusted on the deep pre-export history. This anchors the Python reconstruction to EA ground truth (execution-parity applied to the data layer). Claude specs the reconstruction; the Auditor validates against the EA export independently.
- **Stitch** — overlap-validated `core.py` (old) + new EA export → continuous 171-column dataset, no warmup exclusion. Early-history warmup cells (first ~week WeeklyOpen/PrevDay, first ~2 days MultiDay) are faithfully truncated and flagged, not leaned on.

Every Python tool keyed to the 126-column layout (`core.py`/reconstruction, `dots_thresholds`, `portfolio_simulation_engine`, discovery engine, `wf.py`) is rekeyed to the 171-column width here. **All final scripts are delivered to the human at the end for local records** — currently only `dots_thresholds.py` is held locally; the rest have lived in-session. The delivered set is the post-rekey versions used to produce the final stitch.

**Stage 5 — D2D band-calibration study** (Python analysis on the complete dataset; no EA change).
Band distance = `ATR × dynMult × expAdj × persistAdj` around an EMA(13) basis; a single close beyond the trailing band (`U_confirmN=1`) flips `D2D_Trend_Dir`, which gates every one of the 76 triples (the flip sets direction; converging triples execute within it). Current calibrators: ATR + `expAdj`=(ATR/ATR_MA)^1.25, basis-slope persistence (`persistAdj` = 1 + 0.60·|Persist|), and the posture composite `dynMult` (already consumes OBV delta, OBV velocity, PoC distance, volume, efficiency; clamped [0.8, 2.5]). Replicate the band formula in Python; test whether alternative or new variables drive the relax/contract better.
- Known tension to measure: the posture engine tightens on counter-move + high volume (→ 0.8 floor: "Tension" ×0.5, "Hyper-Sensitive" ×0.35) per the original design intent, but ATR and `expAdj` grow during volatility and dilute that tightening in absolute band-distance terms — does it actually clamp to price at flip-relevant events?
- Score candidates by flip quality, then **decisively by downstream trade outcome through the full harness** — D2D re-gates all 76, so a calibration change is system-wide, highest-leverage, and must clear survival-first folds, not just "look like better flips." Null that the current calibration is already best stays live.
- *Output:* a decision on whether a D2D change is warranted.

**Stage 6 — EA change #2 (conditional): recalibrate the D2D band inputs** — only if Stage 5 warrants it. Full pipeline + confirm → re-export → re-stitch → final dataset. May collapse to nothing (a live null).

**Stage 7 — Honest harness.** Execution-parity audit of the simulator against the **final** EA: eligibility (ADX ≥ 15 & Volume > 50), D2D direction match, fire gate (≥2 concurrent OR solo + Vol ≥ 300), spread (3.0 pt), fill model, exits, position/per-rule caps. (Parity *logic* is data-independent and can be drafted earlier; the *numbers* run on the final data.) Lock the fold scheme. Run the existing 76 through the trusted harness per fold — reference point + self-check, not a gate. **Live-execution alignment (deferred to here):** once development is complete, reformat the final stitched OHLCV into History-Center format and run the EA on a **non-FTMO MT4 build** (one that accepts imported History Center data — the FTMO `.exe` is live-only, which is why the tester currently fails) via the strategy tester; compare its real trades to the `engine.py` sim. This is the execution-parity ground-truth check at the EA level.

**Stage 8 — Definitive discovery on the final complete dataset** (analysis; no EA change).
- Characterization: per-bar convergence-count distribution; concurrence → outcome (WR / PF / N / freq per bucket per fold — the 5+ question with N shown; the production solo-Vol≥300 vs ≥2-concurrent gate is the in-production reference point on this curve); single-variable scan.
- Threshold sweep: p80→p90→p95 / p20→p10→p5 by ATR regime and fold — evaluate, conclude, lock, then discover at the locked setting.
- Fixed-set: scan all qualifying triples, rank by cross-fold persistence.
- Density engine — **the overlap L322 discards is the variable under test.** The existing engine is fixed-triple discovery + 0.80 entry-overlap dedup (`OVERLAP_THRESHOLD`, L~322); no density mechanism exists. Count co-firing convergences over the **full candidate space, without dedup**; sweep N; compute **raw and effective/independent (correlation-aware)** density to separate genuine independent confirmation from redundant same-move pile-up; per-fold persistence. Regime rationale: a frozen set is regime-coupled; density over the full pool measures "how much agreement" regardless of which variables — the future-proofing candidate **if** it persists OOS.
- D2D + filters: variables separating winning from losing D2D signals, per fold.
- Compare & validate: head-to-head (fixed-set vs density vs D2D-filtered vs hybrid), ranked **FTMO worst-day survival first**, then cross-fold persistence, then spread robustness; anchored walk-forward on the leader. Report the data; the human decides.

**Stage 9 — Post-discovery** (after a winner is chosen).
- Trade management B.1–B.5 on the chosen set: B.1 momentum k=1.5 BE (inert under LockFrac=1.0); B.2 ATR-conditional BE tiering (<8/0.15×, <12/0.3×, <18/0.5×, ≥18/1.0×); B.3 SL floor 20; B.5 UseD2D toggle.
- **Validation ladder to live:** (1) strategy-tester alignment — non-FTMO MT4 with History-Center data — of EA execution vs the `engine.py` sim on the stitched history; (2) a one-week demo forward test **and** re-run the sim over that same week — both must align; (3) on alignment, deploy at minimum size / one account to prove survivability live before full stake.

### Stage 2 outcome — frozen build list (pending human ratification)

Decided on proxy evidence (distinctness/collinearity matrix + HTF redundancy test on the sealed 146,815-row set; forward-return separation shown but weak and not used as a gate). Build cost is secondary; the gate is incremental information vs the existing 126. Inclusive per the "fullest market perspective" directive; only zero-information-gain redundancies excluded, since curve-fit control is validation discipline at Stage 8, not variable count.

**BUILD (one Stage 3 change → one re-export). All horizons capped to one trading week (chart at Max bars = 7,000); new level variables EST-anchored at 18:00 ET (tunable, via the existing DST-aware conversion); PoC/VAH/VAL keep the sacred server-day boundary. All dist-ATR columns divide by the `atrAssigned` local in EA source (≡ the `ATR_1M` column at runtime; the reproduction/recompute divisor is `ATR_1M`, never the `ATR_Assigned` column — Stage-4 finding). Built one section at a time through the pipeline:**
1. **VWAP** — session, close-weighted, server-day anchor. Exports: price, dist-ATR, side, ±1σ bands (volume-weighted std of price about VWAP). Distinct from PoC (corr +0.77).
2. **VAH / VAL** — scoped migration: the value-area expansion moves into the self-computing `Calc_PoC_State_OnBar` (the visual calc at L5228-66 used iBarShift — forbidden), unifying PoC/VAH/VAL on one volume-of-Close profile (chart lines move; VAL ≤ PoC ≤ VAH guaranteed). Exports: 2 prices, 2 dist-ATR, value-area side. The one sacred-touch.
3. **Prior-day H / L / C** — 3 levels, dist-ATR, side. Distinct (H↔C +0.49).
4. **Daily open** — price, dist-ATR, side. Re-included by human override (18% unshared vs VWAP/prev-close; gap-fill/opening-drive reactions).
5. **Round numbers 100 / 500 / 1000** — 3 dist-ATR, absolute (S/R either way). Mutually low-corr; 250 dropped (non-standard for US30).
6. **Opening range H / L** (9:30–10:30 EST) — 2 levels, dist-ATR, position. Distinct from prior-day high (+0.25).
7. **Session H / L** — self-computed (NOT the iHighest/iLowest visual at L5405-08). 2 levels, dist-ATR, position.
8. **Weekly open** — price, dist-ATR, side. Distinct (+0.18 vs day-open). (Prior-week H/L dropped — needs two weeks, exceeds the 1-week cap.)
9. **Multi-day trend + position** — 2-day window (fits the cap): ATR-normalised regression slope + position-in-range 0–1. The genuine HTF gap; convergence ingredient only, never a consensus gate.

**EXCLUDED (evidence):**
- **Sub-day resampled HTF trend (M5/M15/H1)** — 0.66 corr with AT_LT (redundant) and adds resampling lag that fights the M1 edge. HTF redundancy hypothesis confirmed: sub-day is subsumed by the AT engine; only the multi-day horizon (corr 0.10–0.14 with AT_LT) is uncaptured → item 9.
- **Prior-week H / L** — would need two weeks of lookback, beyond the locked 7,000-bar (one-week) chart window; revisit only at the deconstruction phase if the depth cap is lifted.

Net: **45 new columns (126 → 171)** under the fully-consistent Price + Dist_ATR + Side convention (the "~38" was an early sketch; the final schema is in the Stage-3 DONE record above). Accepted because curve-fit control is Stage 8 validation discipline, not variable count.

### Variable-completeness gap findings (reference for Stage 2)

**Hard data constraint:** export is M1 OHLCV only — no tick/L2/quote data. Every microstructure measure is necessarily an OHLCV proxy; true order-flow / quoted-spread / book-imbalance are impossible regardless of column count. The `Micro_*` layer is already close to the full standard OHLCV-microstructure toolkit (Kyle λ, VPIN, Amihud, Roll, Corwin–Schultz, Garman–Klass, Hurst, FractalDim, VolOfVol, AutoCorr, entropy, IBSP polarity, signed-volume OrderFlowDelta). More microstructure is **not** where the gap is.

Gaps confirmed absent in source (0 references; all features computed on the M1 series, no `PERIOD_*` access):

- **Tier 1 (clearest, non-redundant, lag-free) — horizontal reference anchors.** Levels, not trend reads, so no resampling lag and regime-stable:
  - VWAP family — session/anchored VWAP, distance-to-VWAP (ATR), ±σ bands. PoC (mode) ≠ VWAP (volume-weighted mean). Single clearest omission.
  - Prior-day / prior-week H/L/C, daily/weekly open, round-number proximity (US30: 40000/41000…), opening-range / initial-balance, time-since-open, exposed session high/low (computed at L5407 for PoC but not surfaced).
  - Value Area (VAH/VAL) + profile shape/skew — only PoC (mode) captured.
- **Tier 2 (additive but partly redundant):** realized variance & jump separation (bipower variation; jump detection genuinely uncaptured, Parkinson/Rogers–Satchell redundant with Garman–Klass); cumulative volume delta signed by IBSP polarity (minor); continuous cross-variable interactions (lowest value — the convergence architecture already captures interactions implicitly).
- **Sub-day HTF trend (M5/M15/H1) — redundancy hypothesis to test, not a closed exclusion.** `AT_ST` (≤~3.3h) / `AT_LT` (≤~20h, flip-capped) + dual SuperTrend + period-1000 EMA likely subsume it; resampled HTF trend adds lag that fights the M1 edge. Confirm via incremental-value test.
- **Multi-day / swing positioning — genuine open gap.** `AT_LT` resets on every flip (effectively intraday); no exposed multi-day context. Enters lag-free as levels/bias (already in Tier 1), never as a resampled-HTF gate.

**Incremental-value test (settles redundant-vs-real):** a candidate's predictive contribution *conditional on* what `AT_LT`/`D2D` already provide. Near-zero → redundant, drop. Material → real gap, worth the pipeline cost.


### Documentation

- ✅ **Rewrote the three non-negotiables docs** (Developer / Supervisor / Auditor) to encode the unified threshold layer + the adaptive-calibration / warm-start layer as SACRED — `RebuildStateForExport`, HarmVol EMA cold-start seed, KAMA anchor-seed, PoC day-boundary, KAMA warm-start persistence, export cold-guard, KAMA signal suppression, `HARMVOL_ER_PERIOD`. Must not be modified by any future AI development without explicit human approval.
- ⬜ Update `equiDOT_dev_plan.md` and `equiDOT_development_map.md` — stale content contradicts the sacred layer (e.g. "3 of 116 adapt", "thresholds must not be frozen").

---

## Operational Behaviour (sealed)

- Warm boot → 76/76 from bar 1, KAMA exact.
- Cold boot → 63/76, 13 KAMA rules suppressed all session.
- Size mismatch → cold-suppression fallback.
- Export → always cold, always δ=0.

## 2026-07-09 — DEFINED FOLLOW-UP: cluster->cluster sequential (post-F0)

**Operator observation (valid):** F1 as single-variable ordered pairs (A@t-k -> B@t) is thin per leg — a richer, more realistic sequential pattern is multi-variable: a CLUSTER of variables aligns (A), then k bars later a second CLUSTER aligns (B), then enter. F1 was built as pairs ONLY because the multi-variable sequential search is combinatorially intractable to brute-force (cluster-A combos x cluster-B combos x lags = trillions; the same wall that forced F1 off 3-condition sequences).

**Defined follow-up (do NOT forget):** after F0 selection produces the winning same-bar convergence clusters, run SEQUENTIAL-BETWEEN-F0-CLUSTERS as a post-F0 analysis: "F0-cluster-A fires, then k bars later F0-cluster-B fires -> enter." This is tractable because it sequences already-proven clusters rather than searching all combos blind, and it delivers the operator's intended cluster->cluster pattern. F1's surviving pairs also serve as the lead-lag map (which variables lead which) to inform cluster construction.

**Interpretation flag (unconfirmed, to verify in data):** F1's large survivor count is likely a function of how permissive a simple 2-leg A->B pattern is, not necessarily strong edge. Persistence + worst-day + PF on the F1 survivors (via the quant, survival-first) is what determines value — the raw survivor range is not itself a finding.

## 2026-07-18 — PHASE-1 BLIND-AUDIT CLOSED + MASTER PROGRAM RATIFIED

**Two closures today: the blind quant-audit Phase 1 is closed with an independent CONSISTENT verdict, and the scattered stage8_discovery pipeline is consolidated into a single ratified master orchestrator.**

### Blind Quant-Audit — Phase 1 CLOSED (independent CONSISTENT verdict)
The standing blind Quant-Auditor's Phase-1 derivation was reconciled against the committed system (recorded: `quant_auditor_phase_1_closing.txt`). The auditor ran `reproduce_dot.py` itself (deterministic over the sealed baseline), then independently re-scored BOOK-50 flat through the engine ($58,277, PF 6.12, WR 92.8%, wd −$128 — matches to the dollar). **Verdict: CONSISTENT (scope difference, not disagreement).** The gap between its Phase-1 flat book and the committed $92,347 is fully accounted numerically — flat $58,277 + $34,070 (the four layers it deferred) = $92,347. Its blind flat book and the committed flat book are the same idea (F0 triple-convergence, D2D-confirm, decorrelated, survival-first); the committed book is a BETTER SELECTION from the identical 2,420 pool (all 48 F0 signals live in its own field — the gate hid nothing). OOS PF ~7 is substantially clean: all 48 F0 signals individually Jan-Apr-persistent (median Jan-Apr PF 9.13). Per-choice verdicts (each on its own objective): (a) 2 F1 sequential HOLDS UP (74%/86% additive structure coverage); (b) gap-singles HOLDS UP (strongest layer, +$18,055, cannot stack losses by construction, improves max-DD); (c) D2D crown jewel DEFENSIBLE low-risk small (+$2,915, improves worst-day); (d) conviction 2× DISCIPLINED, named trade = ~20% deeper worst-day for +$13,100 net. Gate benign-to-aligned. The committed $92,347 is **real, reproducible, and confirmed by an adversarial blind instance.** Confidence resolved.

**Record correction (everywhere):** OOS net $29,326 → **$29,190** (from source, both entry-month and exit-month attribution, n=815; the $29,326 was a stale modelled leftover, ~$136 / 0.5%). OOS PF 6.96 unaffected. Corrected across all 13 foundational docs + records + rd_plan + execution_sequence.

### `reproduce_dot.py` — permanent operator re-score tool (delivered)
Single self-contained script reproducing the full committed system from source. `python3 reproduce_dot.py` → net $92,347 / 2,698 tr → REPRODUCED with the full build-up (BOOK flat $58,277 → +S.20 conv $71,377 → +S.20 gaps $89,432 → +D2D-conv $90,447 → +D2D-gap $92,347). `--signals new.csv` re-scores any new book; prints the oracle sha each run (parity drift check). Independently run and confirmed. Removes AI-in-the-loop for number verification — the operator proves $92,347 himself on demand.

### MASTER PROGRAM (`master.py`) — built, Supervisor-verified, Auditor-RATIFIED
The scattered stage8_discovery pipeline (many scripts across families/folders + stale intermediates) is consolidated into ONE command. Design: dead-simple structure — `/data/` (market-agnostic drop-in, any CSV, 1 or N files) + `/discovery/` (all outputs, auto-split into ≤9MB upload-friendly chunks) + `master_guide.md` + `master.py`. One command runs S0→S9: ingest → oracle thresholds → all-family discovery → unify → gate → stale-regen → contenders → committed score → report. Checkpoint/resume (`.done` markers keyed to input sha), progress+ETA, --workers ≤12.

**ASSET-AGNOSTIC — the core principle.** The only branch is "was a frozen book supplied (`--book`)?" — NEVER "is this US30?". Stages S0-S7 are pure geometry (percentile-relative distributional patterns); the oracle self-calibrates to whatever data is ingested. `--book book50_signals.csv` → replay+verify a ratified book (US30 today; any future ratified market identically). No `--book` → discover a fresh book by the survival-first rule, flagged "designed, not yet data-validated." US30 is not special to the code — it is simply the dataset that currently has a ratified frozen book.

**The master IMPORTS the sacred files (never rewrites them)** — the five byte-locks stay intact (dots_thresholds 518862bf19fb, wf 793e6e5f8d9a, core 6530e2508b17, portfolio_simulation_engine bb498eb13ce3, conviction 27af7acee824). It folds in the `reproduce_dot.py` committed-score logic as stage S8, so one master run also reproduces $92,347.

**Pipeline: quant stage-spec (`master_stage_spec.md`) → Supervisor-verified → Developer built (`master.py`, 554 lines) → Supervisor-verified acceptance → Auditor RATIFIED (PASS).** Auditor independently ran it, mutated sacred files to confirm abort-on-drift (exit 2, proven by direct mutation), byte-reassembled split files, traced branch logic. All 8 verification points PASS: sacred byte-lock+abort, two-path branch, S8 canonical reproduction ($92,347 self-computed), S7 contender ladder (every dollar attributed C0→C4), discover-fresh honesty (no false REPRODUCED on a discovered book), checkpoint/resume, auto-split byte-clean, S0 invariants + isolation. **One scoped note (not a defect): S8 has no resume-skip — by design; a verification stage must always re-check, not skip (cheap ~16s, idempotent). Correct behaviour.** `master.py` RATIFIED as the sole pipeline entry point.

**Docs reconciled to master-centric reality:** `master_guide.md` (single how-to-run + doc-index), `discovery_map.md` (inventory + carry/drop manifest), `master_stage_spec.md` (build contract) are the current surface. Retired (removed): `RUN_STAGE8.md`, `DOT_stage8_program_map.md` (the 17f.2 deliverable, superseded by discovery_map + master_guide), `stage8.py` (absorbed into master.py S0). Stale artifacts `signal_full_records`/`signal_per_day_pnl` flagged for S6 regeneration — never carried as canonical. F10 confirmed folded into F0 (concurrence null) — not a gap.

### Reusable master-analyst layer (delivered)
`non_negotiables_master_analyst.txt` + `master_analyst_initialiser_prompt.txt`: a generic, market-agnostic operating manual + kickoff so any future Claude instance can run the master on any dataset and interpret the output with the ratified discipline (survival-first, too-good-is-a-bug, OOS-strengthening = edge signal, decorrelation > stacking, convergence necessary), delivering the mechanical verdict and surfacing the genuine human forks (mechanism adoption on coverage/survival grounds, book size, deploy-lot) rather than deciding them. The master computes; the analyst interprets; the human decides.

### DOC/RECORD PROPAGATION (today)
- `DOT_readme.md` rewritten with the full 17-month origin arc (D2D-first at ~64% on old corrupted cascading data → the visual-container-caps-the-math insight → the pivot to measuring everything at full precision, 126→171 columns, 117 discovery candidates → pairs→triples→gating) + the D2D crown-jewel chapter (founding signal full circle). Numbers to crown-jewel canonical.
- `DOT_signal_overview.xlsx` WR number-format defect fixed (F65-67/F69 `'0.000'`→`'0.0%'`, now render 96.9%/100.0%/92.9%/100.0%); full format-consistency sweep across all three xlsx D2D cells.
- rd_plan D2D throne L/S corrected to the 18L/14S throne split (source-confirmed: LONG 18tr/100%/$1,651/no-loss; SHORT 14tr/92.9%/PF 6.10/$780/−153), the 23-trade 13L/10S labelled superseded sniper.

**Micro-regime note (recorded against the auditor's macro-regime caveat):** the 6-month sample is calendar-short (bull-heavy, one crash — no other-macro-regime certification possible), BUT at M1 it is 152,983 bars = thousands of micro-regimes (session opens, liquidity vacuums, trend-to-chop transitions) — geometric saturation far exceeding multi-year higher-timeframe samples. The macro caveat stands; the micro-regime saturation is high.

**STATUS:** Phase-1 blind audit CLOSED (system independently confirmed real). Master program RATIFIED as the sole discovery entry point. reproduce_dot + master-analyst layer delivered. The path to live is unchanged (Stage 9 EA build → sim↔MT4 parity → demo → live); the discovery/analysis infrastructure is now a single ratified command runnable on any data.

**Post-ratification patches (2026-07-18):** master.py received two fixes after the Auditor's PASS on sha a1366ac45130 — (a) Windows UTF-8 file-I/O (all writes encoding='utf-8'; cp1252 crashed on the '→' char), and (b) natural-sort + S0 header-handling for >9 split parts, plus rebuild.py integration (shared _packutil.py). Current master.py sha: a1366ac45130. The committed-path logic is unchanged; $92,347 re-verified REPRODUCED on Windows and from a clean clone. A fresh Auditor pass on db8957587844 is pending to re-bless the new sha.

---

## 2026-07-21/22 — THE NEW-DATA REVEAL: first true out-of-sample event, and the gate-first redesign it triggered

**The single most consequential session since discovery closed.** A fresh EA export extended the data
past the sealed baseline for the first time. The committed BOOK-50 met genuinely unseen bars, degraded
materially but stayed profitable, and the diagnosis exposed both a methodology gap and a one-variable
fix already sitting in the export. Full detail: `DOT_new_data_reveal_2026-07-21.md` (404 lines).

### 1. THE EXPORT AND THE PIPELINE — rebuild.py's first real use
Fresh export taken 2026-07-21: `US30.cash_AUTO_EXPORT.csv`, 100,000 rows, 2026.04.08 17:32 →
2026.07.21 17:09. Run through the ratified data-prep layer:
- `python rebuild.py` — validated and split first time, no adjustment needed. The dotted asset name
  (`US30.cash`) required no rename. **invariants PASS**, 13 parts written to `data/`.
- `python master.py --book engine/book50_signals.csv` — ran clean end-to-end on Windows.
**The rebuild → master chain worked on first real-world use.** No code changes required.

### 2. DATA INTEGRITY — CONFIRMED, with a correction to the initial reading
Overlap with the sealed baseline: **75,732 shared bars.**
- OHLCV: Open/Low/Close max diff **0.000000**. High max diff 2.0 on one bar; Volume max 10 on three.
- Adaptive variables (KAMA_Value, D2D_Trend, Micro_Hurst, ADX_Value, PoC_Price): **median 0.000000**.
**Export=live parity holds on fresh data. The adaptive layer and the KAMA `.bin` warm-start seed are
working correctly.**

**CORRECTION (recorded because the first attribution was wrong):** the non-zero maxes were initially
attributed to cold-start effects in the fresh export's early bars. That was a guess, not a
measurement. Re-checked: the warm-up region has **max diff 0.000000** on ATR_1M, D2D_Trend and
ADX_Value. The divergences are concentrated on **a single day — 2026.06.17** — entered via one
High-bar discrepancy at 04:45, propagating through the recursive adaptive chains and **re-converging
within the same session**. A one-bar feed discrepancy between two separate exports, self-healing.
Not a cold-start problem and not a parity concern (live runs one continuous chain off one feed).

### 3. THE HEADLINE — BOOK-50 on genuinely unseen data
Segment split at the baseline boundary (2026.06.25):

| Segment | Trades | Net | PF | WR | Worst day | Days |
|---|---|---|---|---|---|---|
| Overlap (Apr08–Jun25) | 975 | +$31,360 | 6.08 | 92.1% | −$204 | 49 |
| **NEW (Jun25–Jul21)** | **375** | **+$8,407** | **2.19** | **81.1%** | **−$565** | **18** |

Profitable on unseen data, survival intact (worst day = 11% of the FTMO $5,000 ceiling), and it did
**not** invert (the prior 76-book failure mode). But materially degraded, and the trade rate was
unchanged (~20/day both segments) — **the system did not self-throttle in the weaker regime.**
Mechanism: SL share rose 7.5% → 18.7% while BE share fell 77% → 66%; whipsaw reversals took entries
to full stop before the BE tier could arm.

Market context: range-bound geopolitical chop (US–Iran escalation) opening the seasonal summer
doldrums — the worst conditions a directional convergence system can face.

### 4. WHAT PERSISTED — the findings the redesign is built on
**Signal-level persistence.** The committed book is **50 signals: 48 F0 triple-convergence + 2 F1
sequential.** The 3 GAP fillers (GAP_HURST, GAP_FB, GAP_D2D) are separate post-selection additions.
Total entities 53; 52 fired in both segments.

| Group | Persisted (net>0, PF>=2, WR>=75 in BOTH) | Rate |
|---|---|---|
| F0 triple-convergence | 23 / 47 | 49% |
| F1 sequential | 1 / 2 | 50% |
| GAP fillers | 2 / 3 | 67% |
| **ALL** | **26 / 52** | **50%** |

**Null baseline (is 50% skill or luck?):** 400 random triple-convergence signals generated from the
same condition pool and scored on both segments. 15 of 201 (7%) passed the old-segment quality bar;
of those, **4 (27%) persisted.** **Selection persists at ~2x the random rate — real, but insufficient
for funded deployment.**

**Market-structure persistence, normalised per trading day:**

| Structure | Persist rate | OLD $/day | NEW $/day | |
|---|---|---|---|---|
| TREND_CONTINUATION | 67% | $165.8 | **$265.1** | **+60% BETTER** |
| BREAKOUT_EXPANSION | 75% | $100.9 | **$140.7** | **+39% BETTER** |
| MOMENTUM_IGNITION | 23% | $124.8 | $17.4 | −86% |
| STRUCTURAL_ENTRY | 50% | $93.5 | −$10.0 | broke |
| PRICE_ACTION | 25% | $22.2 | −$9.2 | broke |

**The two directional-structure families EARNED MORE PER DAY on new data than on old.** The book's
degradation is attributable to MOMENTUM_IGNITION, PRICE_ACTION and STRUCTURAL_ENTRY reversing — not
to uniform decay. **Structural class predicts persistence better than in-sample statistics do.**

**CONCURRENCE IS THE PERSISTENCE AXIS** (the operator argued this for months against repeated
dismissal; it is now measured):

| Depth | OLD WR / PF | NEW WR / PF |
|---|---|---|
| 1 (solo) | 90% / 4.12 | 73% / **1.25** |
| 2 | 91% / 5.89 | 88% / 3.33 |
| 3–4 | 100% / no losses | 92% / 7.43 |
| 5–7 | 100% / no losses | **100% / no losses** |

Monotonic in both segments. **Depth 3+ produced ZERO losing trades in either period.** Depth >=2
alone lifts the new-segment PF from 2.19 to 7.89.

**Mechanism (measured, not assumed):** solo entries carry avg loss 2–3x avg win — solvent only on an
extreme win rate, so a modest WR drop collapses them (exactly the 90% → 73% observed). Concurrent
entries have balanced payoff and degrade gracefully. Solo breakeven-WR is 64–75%; concurrent is 39–52%.

**Conviction held almost perfectly.** Hurst-p90 2x tier: PF 13.42 → **11.81**. 38 conviction trades
out-earned 243 base trades on the new segment. Hurst p90 standalone: 94% WR, PF 15.96, $5,490.

**The edge is BIG-MOVE CAPTURE.** Bucketing new-segment trades by the absolute 60-bar forward move:
**77% of all profit came from the largest-move quartile** (PF 4.27). Deep stacks carry the largest
forward moves. By hour: 12:00 EST $4,674; 10:00 EST $1,621 at **100% WR** and the highest mean
concurrency. This also explains why March (the crash) was the best month — maximum directional thrust.

### 5. `AT_Regime_ST` — THE GATE CLAIM IS RETRACTED

**An earlier version of this section recorded an `AT_Regime_ST` directional-alignment gate as the
phase's headline fix, citing 267 tr / PF 3.83 / +$9,228 / wd −$116 on the NEW segment. THE FIGURES ARE
ARITHMETICALLY CORRECT BUT MISLABELLED.** They came from a filter that kept trades where
`AT_Regime_ST == 1` **regardless of trade direction** — a regime-STATE filter, not directional
alignment. Cause: a case bug (`direction == 'long'` tested against an uppercase `'LONG'` column) which
silently typed every trade as short.

**All three readings on the same 375 NEW-segment book trades, independently reproduced twice:**

| Reading | n | WR | PF | Net | Worst day |
|---|---|---|---|---|---|
| **Directional alignment, correct encoding** | 171 | 74.9% | **0.97** | **−$111** | **−$683** |
| Directional alignment, inverted encoding | 204 | 86.3% | 3.98 | +$8,518 | −$476 |
| **Regime-state `== 1`, no direction test** | 267 | 86.5% | 3.83 | +$9,228 | −$116 |

Row 3 reproduces the previously recorded figures exactly. Row 1 is the gate as it was actually decided.

**The directional AT gate REMOVES the book's profit** — PF 0.97, net −$111, worst day −$683 (deeper
than the ungated −$565). Consistent with three later independent measurements: concurrent PF falls
9.76 → 6.85 under `AT_Regime_ST` and 9.76 → 6.97 under the panel-true `sign(AT_Slope_ST)`.
**No directional AT gate is supported on any variable tested.**

**The regime-state operation also fails scrutiny:** `== 0` beats `== 1` on PF in three of seven
calendar buckets, the OLD-segment differential is neutral (6.33 vs 6.25), and the entire effect is
July — exactly what regime-conditional persistence testing exists to catch. Retained as a REPORTED
CONDITIONER for further measurement, not adopted.

**The encoding and latch findings below stand** — verified against DOT.cs source, unaffected by this
retraction. The variable stays fully in the vocabulary and the export; nothing is deleted.

**ENCODING — CONFIRMED AGAINST SOURCE:** `AT_Regime_ST == 0` is **BULLISH**, `== 1` is **BEARISH**
(DOT.cs L3102 `curr_AnchorType_ST = (st_Slope > 0.0) ? 0 : 1`, corroborated L3113; 96%+ agreement
across 177,251 bars). The intuitive reading is inverted.

**AT_Regime_ST IS NOT sign(AT_Slope_ST).** DOT.cs L3110 updates the anchor only when a slope sign
change coincides with a qualifying flip; otherwise the prior state **latches**. The ~4% disagreement
is hysteresis, not noise. **RATIFIED GATE VARIABLE: the native binary `AT_Regime_ST` state** — no
derivation, no export=live risk, and the latch resists whipsaw in flat regimes.

### 6. THE METHODOLOGY GAP
All prior validation — blind audit, OOS May–Jun, 6/6 folds, decorrelation, null/shuffle — occurred
**INSIDE the Jan–Jun window**, downstream of a funnel that had already run across that whole window.
**"Real in-window" and "persists across regimes" are different claims, and only the second was ever
the goal.** The selection funnel (1.7M → 89K → 51K → 1,788 → 50) never applied multiple-testing
correction, used single-pass rather than stability selection, and used aggregate rather than
regime-conditional OOS.

**The book was validated. The selection process never was.**

### 7. THE STITCHED DATASET — one continuous series
Built by the Quant, independently verified by the Supervisor:
`DOT_stitched172_jan19_jul21_part01..09.csv` — **177,251 x 172, 2026.01.19 15:49 → 2026.07.21 17:09.**
- Sealed baseline for 2026.01.19 → 2026.06.25 (152,983 rows, warm throughout)
- Fresh export for bars AFTER 2026.06.25 only (24,268 rows)
- Schema reconciled UP to 172: `VWAP_Sigma_ATR` native on fresh rows, oracle-derived on baseline rows
  exactly as `portfolio_simulation_engine.load_sealed_baseline` does (verified max diff 1.84e-5)
- Text-level stitch preserving original digit strings byte-for-byte — no float re-serialisation drift
- **Verified: baseline half 171/171 columns bit-identical to source; fresh half 172/172 bit-identical.
  Seam clean (Δt 00:01:00, no gap, no duplicate). Time strictly increasing, 0 dup, 0 NaN.**
- Split at 25MB max per part + manifest with per-part sha256.
**This is now the single source of truth. Never frame analysis as "old vs new" — it is one geometric
palette.**

### 8. DECISIONS TAKEN
1. **Solo (depth-1) convergence is excluded** from the forward book. PF 1.25 on new data with a 2–3x
   loss/win ratio is structurally fragile, not merely unlucky.
2. **REVERSED (2026-07-23). `AT_Regime_ST` does NOT join the gate stack.** The decision rested on a
   mislabelled measurement (see §5 retraction above): the directional gate is PF 0.97 / net −$111 /
   worst-day −$683 on the NEW segment and removes the book's profit. The regime-state variant fails
   regime-conditional persistence. The gate stack therefore remains as it was: **ADX >= 15 and
   ticks > 50** (minimal-participation filters removing dead zero-participation bars, nothing more)
   plus **D2D directional agreement**. `AT_Regime_ST` is retained as a reported conditioner and stays
   in the vocabulary; requirements to establish it are specified in `discovery_redesign_spec.md`.
3. **GATES ARE STATE COLUMNS, NEVER ROW FILTERS.** No bar is ever deleted. Every candidate is scored
   both ungated and per-gate-subset. Rationale: a row filter destroys the counterfactual, makes an
   inverted-encoding error silent and irreversible, and — critically — deleting rows upstream of the
   oracle changes what mechanism D's rolling-2500 ring sees, which is an **export=live parity failure**
   of the same class as a global percentile.
4. **Discovery becomes GATE-FIRST** — search within the gated universe rather than searching all bars
   and filtering afterwards. Gates are pre-trade market-state conditions, not performance-selected
   outcomes, so conditioning on them does not curve-fit the way selecting on returns does; and the
   narrower universe directly reduces multiple-testing bias.
5. **THE SELECTION PRINCIPLE CHANGES.** The old objective optimised for DECORRELATION — signals that
   do not overlap. But overlap IS depth, and depth is what predicts persistence. **Selecting against
   overlap systematically selected against the property that survives, concentrating the book into the
   fragile depth-1 population.** The corrected objective: a book whose signals **CO-FIRE often**
   (creating depth) while their **FAILURES remain uncorrelated**. Two separate properties; the old
   method collapsed them into one and optimised the wrong one.
6. **Minimum UNIQUE-VARIABLE count at depth**, not merely a signal count — measured depth-2 events
   with only 3 unique variables (the same read counted twice).
7. **Walk-forward validation OF THE SELECTION PROCESS** is mandatory before any new book is committed.

### 9. SUPERVISOR HANDOVER
The outgoing Supervisor moved to **Manager**; a new Supervisor was initialised against the repo
non-negotiables, the reveal document, both plan docs, and a handover brief encoding four documented
failure modes from this session — negative overreach, premature closure, unverified attribution,
anxiety amplification. Pipeline is now **Operator → Manager → Supervisor → specialists.**

On initialisation the new Supervisor independently verified the `AT_Regime_ST` encoding against DOT.cs
source (closing an open item), **discovered the latch/hysteresis property** the outgoing Supervisor had
missed, and caught two documentation errors (a signal-count discrepancy and a stale $29,326 in the root
README). Both corrected.

**STATUS:** BOOK-50 remains the committed artifact and is NOT retired — it is profitable and survives
on unseen data. But it is superseded as the *forward* book pending the gate-first rediscovery. The
data pipeline, the adaptive layer and export=live parity are all confirmed working on fresh data.
The next phase rebuilds signal selection around what demonstrably persists.

---

## 2026-07-23/24 — THE GATE-FIRST REDESIGN: SPEC, THREE BUILDS, CONSOLIDATION

**Step 17n is CLOSED.** The discovery pipeline has been rebuilt, audited across nine passes, and
consolidated. The scan has not yet been run.

### 1. TWO NEW GOVERNING DOCUMENTS

**`DOT_signal_discovery_mantra.md` (305 lines) — STANDING DOCTRINE.** Not a finding, not superseded
by measurement. It governs how signal discovery is approached by every seat on every run. Its five
rules bind every deliverable:
  1. **MEASURE THE CAKE, NOT THE BITE** — every finding labelled MARKET (price-only or
     full-population) or BOOK. The recurring project failure is measuring the market THROUGH the
     book and reporting the reading as a property of the market.
  2. **INCLUDE, THEN LET THE EVIDENCE SORT** — nothing removed on a single measurement; gates are
     state columns, never row filters.
  3. **NO PRE-SET TARGETS** — composition is an OUTPUT, never an INPUT.
  4. **DEPTH IS THE UNIT OF QUALITY, NOT THE SIGNAL.**
  5. **NEGATIVE CONCLUSIONS CARRY THE SAME BURDEN OF PROOF AS POSITIVE ONES.**
Plus the standing construction: **when a finding depends on a filter, threshold or restriction, the
filter is part of the finding.** Its own §4.2 is the worked example — omit the >=15-trade filter from
that table and the stated conclusion inverts.

**`discovery_redesign_spec.md` (1,468 lines, sha `f325a9dfc4b6`) — the build document.** Authored by
the Quant, verified across four Supervisor rounds, final verdict SHIP. Amended nine times.

### 2. THE THREE BUILDS — ALL RATIFIED

| Build | Contents | Verdict |
|---|---|---|
| **1** | S3B family evidence review, `trades.csv`, D2D measurement, §D reach, data-relative OOS, F10 map correction | RATIFY `eea3e3fe931a` |
| **2** | `selection.py`, stage S5B, the objective, per-direction greedy/CELF | REJECT, then RATIFY `02cd79ad914b` |
| **3** | `wf_selection.py`, stage S5C, walk-forward on the selection process, attestation, single-touch guard | RATIFY, then remediation RATIFY `e0a6d884e477` |
| **Consolidation** | one authoritative directory, 105 files | RATIFY `2c11b70871c4` |
| **S3 operability** | six defects — see §5 | REJECT, then RATIFY `17acb49571fa` |

**Final pipeline sha: `master.py a1366ac45130`.** Sacred five byte-locked throughout every pass.

### 3. WHAT THE PIPELINE NOW CONTAINS

**Three new stages**, none of which existed before this phase:
- **S3B** — per-family evidence review across all fourteen families, with INSUFFICIENT-EVIDENCE a
  permitted verdict.
- **S5B** — the selection layer: the lexicographic objective, per-direction greedy/CELF search,
  tail-dependence and mCVaR constraints, vocabulary hygiene, multiple-testing.
- **S5C** — the walk-forward on the selection process: derived splits, embargo, per-segment
  re-derivation, the random-triple null arm, the attestation trail and single-touch guard.

**THE HEADLINE FINDING FROM BUILD 1, and it reframes the whole project:** `discovery_results/`
contained exactly ONE output file (F13, a legacy artifact predating master.py). **The discovery scan
has never been run.** No `S3.done` marker exists and `discovery/results/` is empty. Every run to date
used `--book`, which scores the committed book and skips discovery entirely.
So F2-F9, F11 and F12's "exploratory" classification in `discovery_map.md` **rests on nothing
measured** — nobody looked and judged them weak; nobody looked at all. Verdict emitted:
INSUFFICIENT-EVIDENCE, per rule 5. This is the redesign's central premise established rather than
assumed.

### 4. FINDINGS AND RETRACTIONS FROM THIS PHASE

**D2D IS NOT INERT — measured properly for the first time.** BOOK population, N=5:

| variant | trades | net | PF | worst day |
|---|---|---|---|---|
| gate on (baseline) | 2,678 | $77,239 | **5.14** | **−$639** |
| inverted (polarity probe) | 11,827 | $56,008 | 1.27 | −$3,626 |
| long ungated | 13,088 | **$111,439** | 1.53 | −$2,315 |
| short ungated | 1,223 | $14,351 | 1.92 | −$390 |

Removing the gate multiplies trades ~5x and collapses PF 5.14 → 1.53. Inverting collapses it to 1.27,
which rules out the encoding failure that produced the AT retraction. **Note the survival-first trap
in row 3:** ungated longs make MORE net with a worst day 3.6x deeper — under survival-first that is
worse, not better. This closes reveal open item #3 and definitively corrects the earlier "D2D adds
nothing" claim, which was measured on the wrong column.

**THE OOS WINDOW WAS FLATTERING.** `OOS_MONTHS` was hardcoded to May-June, set when the data ended in
June. The stitched series runs to 21 July, so those are now INTERIOR months and the window is not
out-of-sample. **Legacy OOS PF 5.54 vs data-relative 3.01.** Legacy values retained byte-identical
with a staleness flag (doctrine rule 2 forbids deleting a measurement); `oos_rel_*` added alongside.

**THE RANDOM-TRIPLE NULL, MEASURED PER SPLIT.** At n=16 the rates read 31.3-43.8% and were briefly
reported as "a higher bar" than the recorded 27%. That reading was WITHDRAWN: the three values were
consecutive integers (6/16, 7/16, 5/16), the spread was ±1 triple, and the exact binomial intervals
all contained 0.27. With an adequate denominator (~85 qualifiers per split): **22.5% / 23.2% / 26.5%**
— tight around the recorded 27%. **The 27% baseline needs no restating.**

**FIVE SPEC CORRECTIONS**, all raised downstream and confirmed:
1. §C.2's exclusion-bias claim does not reproduce — it is ANTI-CONSERVATIVE, not conservative. Root
   cause ran deeper than reported: the "daily-loss series" implemented as `min(pnl, 0)` is
   degenerate (73.4% of pairs returned coexceed exactly 1.0). Corrected to raw daily P&L. **The build
   was already correct** — `selection.py` uses `groupby(...)['pnl'].sum()`. The degenerate estimator
   was in the verification, not the code.
2. §C.2's "1.0 == independence" annotation is wrong, not the formula.
3. CoFire must be treated within direction, as DepthYield already is. Cross-direction co-firing is
   structurally zero — **this is D2D's alternating bias behaving as designed**, not a market finding.
4. §I.1 must specify the post-floor partition rule; only equal partition reproduces its stated 3 splits.
5. §I.3's pass criterion restated as a **ratio to each split's own measured null** (mean >= 2.40,
   min >= 1.85, 95% LB > 1.0), with a minimum denominator of 80 qualifiers and a hard floor of 40.

**THE TAIL-DEPENDENCE FINDING WAS OVERSTATED BY ~2x.** Re-derived on the corrected estimator: tail
lift 3.548x → **1.69x**, Pearson +0.0604 → +0.0992, pairs misread by Pearson 24.6% → 20.5%. The
finding survives; its strength did not. Cross-checked from two directions — the build's independently
emitted `lambda_over_independence` is 1.6831.

**A SEARCH DEFECT THAT WOULD HAVE ELIMINATED THE SHORT SIDE.** Greedy selection returned ZERO short
signals. Not a judgement about shorts: **0 of 13 short signals score above zero alone** (one signal
cannot stack with itself at S=5), so every first-step gain was exactly 0.0 and the search halted at
step 0 without ever evaluating a pair. The best short pair scores 0.012295 — **above the incumbent's
own short reference of 0.00757.** Greedy returned 0% of the achievable optimum.
Fixed with a lookahead-2 stopping rule ("no addition of size <= 2 improves"), applied at every
termination point rather than only step 0. Result: SHORT 0% → **100%** of the enumerated optimum.
**And LONG used 2 pair escapes** — the old rule was silently costing the long side ~8% as well. The
defect was never short-specific; shorts merely failed loudly first.

### 5. SIX S3 OPERABILITY DEFECTS

Four were operator requirements predating the redesign and absent from the spec, which is why nine
audit passes did not raise them. All six are fixed and ratified.

1. **S3 did not resume per family.** Only F1 skipped; every other family re-ran, and `all_rows`
   accumulated in memory so completed families' CSVs were never read back. **Fixed:** per-family
   `.done` marker carrying the CSV's sha256, atomic write (tmp → fsync → `os.replace`). Proven
   against a real crash — 7 families read back from disk, 3 re-scanned, result byte-identical to a
   clean run. **Worst case is now one family, never the stage.**
2. **No ETA or progress output.** Two-day stage with nothing printed until a family finished.
   **Fixed:** `[family i of N]`, running ETA, per-family completion times, 60-second heartbeat, all
   flushed for Windows. No wall-clock reaches any artifact.
3. **`--workers` accepted and ignored.** Declared, capped, echoed at startup, never used. **Fixed:**
   plumbed to cross-family process concurrency.
4. **Parallelism was 3 of 14 scanners.** **Fixed:** cross-family process parallelism, touching no
   scanner file. Within-family declined — nine ratified scanners with nine signatures and
   deterministic nested-loop row order. Determinism proven byte-identical across workers 1/2/3 and in
   mixed parallel-then-sequential mode.
5. **Two false documentation claims** asserting resume worked. **Fixed**, with the true granularity
   stated.
6. **THE MOST SERIOUS, AND SELF-FOUND: `orchestrate()` LOADED THE SEALED BASELINE ITSELF.** It called
   `engine.load_sealed_baseline()`, hardcoded to `equiDOT_recon171_step7_part1..8.csv`, and never
   received master.py's ingested frame. **S3 could not run on the operator's data at all** — it fails
   outright on the stitched series, or silently scans the wrong dataset if the old recon parts happen
   to be present. The pipeline's entire purpose is market-agnostic discovery. **Fixed** by injecting
   the frame plus its adaptive/structural/warmup, with fallback preserved for standalone CLI use.

**A SEVENTH, FOUND IN AUDIT AND REJECTED ON:** the parallel-worker frame cache was written only if
absent, never cleaned up, and not keyed on `input_sha` — so scanning asset A then asset B into the
same output directory left every worker silently reading A. **The identical failure mode as defect 6,
reintroduced one layer down and reachable by default.** Fixed three ways at once: sha-named filename,
purge of any non-matching sibling, and deletion on S3 completion. `--workers` default also dropped
12 → 2 after measurement showed **733 MB peak RSS per worker** (10 workers ≈ 7.2 GB, which explains
the silent OOM deaths observed on a 4 GB box).

### 6. WHAT IS PROVEN AND WHAT IS NOT

**Proven and exercised:** the anti-leak architecture at full coverage (176 of 176 rolling thresholds
bitwise identical whether computed on the full series or the training prefix); per-segment
re-derivation of every bound from training bars only; split derivation with floors and a real bar
embargo (3 splits, train 60/83/105, test 21/20/20); the attestation trail and repeat detection; the
single-touch guard; §H.3 UNEVALUABLE raising rather than falling back; the random-triple null arm end
to end; crash resume; determinism across worker counts; the committed path unchanged at
**3,057 trades / $98,205**.

**Never exercised, because it needs a candidate pool:** the funnel re-run per split, per-split
candidate generation, entity persistence, lexicographic ranking across books, §H.1's multiple-testing
components (empirical null, White's Reality Check, Hansen SPA, Romano-Wolf, PBO/CSCV), §H.2 stability
selection, **and the pass criterion itself — the single number this redesign exists to produce.** It
reports UNEVALUABLE until the scan runs. A FAIL is a legitimate outcome and the code is built to
report one rather than lower a bar.

**Splits 0 and 1 test a weaker constraint set than split 2** — TailDep is not meaningfully binding at
17.7% and 33.0% retention, so an early-split pass is evidence about FailConc and absolute survival,
not about tail dependence. Stated in the artifact header.

### 7. STATUS

The pipeline is consolidated into a single authoritative `dot_master_discovery/` (105 files) in the
DOT repo, byte-reproducible from a pristine clone via `DISCOVERY_REDESIGN_consolidated.patch`. Sacred
five intact. **The next action is the 1-2 day discovery scan**, which has never been run.

---

## 2026-07-27 — THE EXPORT CLOCK DEFECT: TERRAIN ANOMALY TRACED TO SOURCE, DATA CORRECTED, EA FIX DEFERRED

### 1. HOW IT WAS FOUND

S2B's terrain hour profile (W=15 / K=p85 / E=p75, MARKET) reported peak median displacement at
"12:00 midday" — 140.4pt against ~48.5pt at the claimed open, with 204 of the largest-decile
episodes at 12:00 against 9 overnight. **That reads backwards against the instrument.** 12:00-13:00
EST is the lunch lull on US30; the NY cash open at 09:30 is the highest-volume, highest-range
stretch of the session. The operator trades this instrument daily and flagged it.

Four candidate mechanisms were tested and ALL FOUR EXONERATED before the clock was implicated:

- **ATR normaliser — not the cause, and it runs the opposite way.** Median ATR_1M by true hour is
  26.40 at 09:00 and 28.87 at 10:00 against 10.74 at 03:00, so ATR peaks WITH the move and
  normalising by it DAMPENS the open. The magnitude statistic |disp|/ATR is near hour-neutral
  (1.56-2.12). Re-running the magnitude test on RAW absolute displacement through mechanism D
  (no ATR at all): 3,630 episodes, peak median at 09:00 (142.4pt), peak largest-decile at 09:00
  (141). Midday does not survive; the open does.
- **Efficiency filter — not the cause.** Median directional efficiency is flat by hour (0.21-0.25);
  the "the open is chop" hypothesis is NOT supported on this data. Removing the filter entirely
  (7,490 -> 9,068 episodes) leaves the peak at 10:00 with 09:00 second.
- **Episode bounding — not the cause.** Median duration is 2 bars at EVERY hour; mean duration is
  3.28 at 09:00 versus 2.97 at 12:00, so the open chains slightly MORE, not less.
- **Not a handful of days.** The 225 largest-decile episodes at true 09:00 spread over 106 distinct
  days; heaviest single day 2.7%, top-5 days 11.6%.

### 2. THE DEFECT, LOCATED IN SOURCE

`DOT.cs` L730-731, `ExportDataForAnalysis()` only. `_GetEstOffsetForTime` is declared at L1761 with
parameter `gmtTime` and requires GMT. The live path (L1767-1772) satisfies that with `TimeGMT()`.
The export passes `Time[i]` — the bar's SERVER time. **The EA is correct; the export is not.**

Magnitude derived from price, not assumed: the opening bell sits at **broker 16:30 in every week of
the span**, so server -> true EST is a **constant -7 hours**. The exported column therefore ran
**+2h fast to 2026.03.06 and +3h fast from 2026.03.09**. There is no -6h window — the broker follows
the US DST schedule, not the EU one. The EU/US divergence is real but appears in the LONDON anchor:
in 2026.03.09-03.27 London's open correctly reads 04:00 EDT rather than 03:00.

Both DST transition dates carry **zero bars** (2026.03.07/08 and 03.28/29 are weekend gaps), so the
`_IsUSDST`-selected-on-server-time error affects **zero bars** and no bar is ambiguous.

Full detail, the fix as code, the residual design decision on sourcing the server-GMT offset for
historical bars, the acceptance parity gate, and the complete relabelling list are recorded as
**S.22 in non_negotiables_developer.txt**. THE EA IS FROZEN AND WAS NOT EDITED.

### 3. THE CORRECTED DATASET

`DOT_stitched172_TRUEEST_jan19_jul21_part01..10.csv` + `DOT_stitched172_TRUEEST_manifest.csv`.
177,251 x 172, 2026.01.19 15:49 -> 2026.07.21 17:09, strictly increasing, 0 duplicates, 0 NaN.

- `EST_Hour` recomputed as true EST = server `Time` - 7h: **177,251 bars changed** (-2h on 46,425,
  -3h on 130,826).
- `EST_Minute`: **0 bars changed** — confirmed, not assumed; the offset is whole hours.
- `EST_DayOfWeek`: **21,539 bars changed**.
- **All 169 other columns BIT-IDENTICAL.** Text-level field edit, no float round-trip, verified
  field-by-field across all 177,251 rows: 0 rows with drift.
- Ten parts at the 25MB ceiling (the source was nine unevenly-sized parts; the ceiling is the
  binding constraint and the count follows from it). Round-trip verified: the parts reassemble to a
  frame identical to the pre-split frame.

**Six anchors PASS on the corrected column, in all three regimes:** bell 09:30; closing-auction
volume peak at 15:59 with collapse at 16:00, i.e. last cash bar 15:59; open-to-close span exactly
6.50h; CME break 17:05-18:04; 08:30 ET release at 08:30; London 03:00 (04:00 in the EU/US window);
Asia 20:00.

**On the day-of-week distribution:** the corrected split is Sun 9,227 / Mon 35,615 / Tue 36,090 /
Wed 35,487 / Thu 35,489 / Fri 25,343. **"Flat Mon-Fri" is NOT the correct invariant** and the
expectation that it should be was wrong. Sunday carries EST hours 18-23 only and Friday 0-16 only,
because the CME week opens Sunday 18:00 EST and closes Friday 17:00 EST. The correct invariant is
**Sun + Fri = one full trading day**: 9,227 + 25,343 = 34,570 against a Mon-Thu mean of 35,670. It
holds.

### 4. THE HARNESS CONSEQUENCE

`portfolio_simulation_engine.py` L148's Friday gate reads the export clock. On the broken clock it
blocked 3,835 bars from true EST 13:00; on the corrected data the **same unchanged sacred line**
blocks 115 bars at true 16:45-16:49, exactly as intended. The gate was always correct; it was
reading a wrong clock. **The file is byte-locked at bb498eb13ce3 and was not touched.**

Committed book, BOOK population, full conviction stack:

| | trades | WR | PF | net |
|---|---|---|---|---|
| broken clock (the record) | 3,057 | 90.9 | 5.07 | $98,205 |
| corrected clock | 3,101 | 90.6 | 4.81 | $97,675 |

Delta +44 trades, -$530. Newly scoreable Friday afternoon (true 13:00-16:59): 101 trades, net $240,
WR 74.3%. **Every figure in the project record before this date was measured with Friday afternoons
excluded.** The delta is small; the point is that the restriction was unintended.

### 5. THE RESTATED TERRAIN

S2B on corrected data, W=15 / K=p85 / E=p75, MARKET — reproduced from the column, not from a manual
correction. Episode set **identical** (7,490; 3,816 UP / 3,674 DOWN), confirming the clock was
label-only:

| TRUE EST | session | episodes | median disp | largest-decile |
|---|---|---|---|---|
| 08:00 | pre-open | 353 | 63.1 pt | 40 |
| **09:00** | **NY OPEN** | **383** | **144.0 pt** | **225** |
| 10:00 | morning | 283 | 123.6 pt | 138 |
| 11:00 | morning | 275 | 89.0 pt | 63 |
| **12:00** | **lunch lull** | **287** | **76.5 pt** | **42** |
| **13:00** | **lunch lull** | **295** | **72.0 pt** | **38** |
| 23:00 | overnight | 348 | 24.8 pt | 0 |

Peak hour is **09:00 in all four grid cells** (144.0 / 147.8 / 195.4 / 187.8 pt), with up/down
staying 49.5-51.2 / 48.8-50.5. The lunch lull carries roughly half the open's displacement. **The
anomaly was the clock, and corrected the terrain says exactly what the instrument says it should.**

### 6. terrain.py — THE ELIGIBILITY LABEL DECIDED

`terrain.py` stamped `ADX_Value >= 15 & Volume > 50 & post-warmup` on every row while
`cluster_profiler.thrust_events` (L224) applied post-warmup only — 45.2% of episodes started on bars
outside the stamped mask. Under the standing construction the filter is part of the finding, so the
label misstated the population.

**DECISION: correct the label; do NOT apply the filter.** ADX/Volume is the ENGINE TRADABILITY
predicate — a property of what the BOOK can enter, not of the market. Applying it to the denominator
would make a market map partly book-derived, the exact circularity the price-anchored basis exists to
escape. Quantified: it would cut 7,490 -> 4,105 episodes and **inflate reported reach by x1.83 with
no change whatsoever in the book** (UP 1.389% -> 2.539%, DOWN 0.735% -> 1.339%) — making the reach
problem look half as bad by definitional choice.

The eligible subset stays fully recoverable: a per-episode `start_bar_eligible` flag and
`eligible_start_episodes` / `eligible_start_share_pct` in the summary. Additive, not subtractive.
`SESSION_BANDS` also corrected — on the fixed clock the old bands labelled the 09:00 open hour
"pre-open". New sha `dcaecaf7e8e1`.

### 7. STATUS

The corrected dataset supersedes `DOT_stitched172_jan19_jul21_part01..09.csv` for all future runs.
The EA fix is documented in S.22 and deferred while the EA is frozen; the acceptance gate is that a
fresh export reproduce the corrected columns bit-for-bit. Sacred five intact and unmodified.


---

## 2026-08-13/17 — THE DISCOVERY REBUILD, THREE ADMISSION DEFECTS, AND THE WHOLE DOT

*The record from the cold-run acceptance through to the adopted 299-signal system. Three structural
defects were found in this period, all three by the operator's questions rather than by an analyst's
sweep, and each one invalidated figures that had already been reported as final.*

### 1. THE PIPELINE, COLD-RUN ACCEPTED

MASTER COMPLETE 1:35:35, all stages ok, 97.5% concurrent, 90 artifacts. Sweep coverage **71 of 71
artifacts, zero sentinel cells, zero missing-required columns.**

The work that made that possible: a `--smoke` path with 14 of 14 caps installed through
`dot_frame_binding.py` and non-empty stage assertions; `scanner_sha` written into family markers so S3
invalidates on a code change instead of counting files; all seven 999/inf producers converted to
`PF_UNDEFINED = float('nan')` with a ten-site consumer table; and coverage reporting on all four
detectors, each now exiting 1 on a gap rather than passing silently.

**F12 GRID ERROR.** The null baseline swept k=1..8 on *condition-count* depth while the book measures
*signal-count* depth. `depth_long` is never below 8, so `depth>=k` was vacuous at every k tested.
Replaced with per-direction grids from the measured distribution — LONG [8,17,22,27,34,48,59], SHORT
[8,12,18,23,29,40,48] — with k=8 kept as an explicit vacuity contrast and labelled per row.

### 2. THE EIGHT-PHASE BRIEF — CLOSED, WITH FIVE PERMANENT NEGATIVES

All eight phases closed. The results that matter for future work are the negatives, because each cost
real time and each will look re-openable to a fresh instance:

- **F5 and F8 are structurally dead.** F8 is `Slope_EMA_ST > Slope_EMA_LT` — one must always exceed the
  other. It is not a weak signal, it is a partition of the space.
- **Stacked descriptor states are dead on both selection rules.** The intersection curve collapses
  geometrically at 3-4 descriptors.
- **Single-descriptor gates fail the cross-book check.** The one result strong enough to price —
  F4 as a negative screen on SHORT triple — inverted on the second book, and a rarity-matched null put
  the expected number of such results from that search at 0.70. One was found.
- **The tick gate is redundant against `ATR_1M >= 20`** — 98.3% overlap, and dropping it cost 14 trades
  for +$456.
- **The adaptive convergence engine is dead three ways:** raw condition depth is never below 8, the
  intersection curve collapses, and the whole field at triple+ is PF 1.29 with a -$6,230 worst day.

And two positives that carried forward: depth and duration are **one axis** (0 of 28 cells clear
p<=0.05, min p=0.335), and the walk-forward decay is **regime, not dilution** — fixed-population decay
0.6738 -> 0.4882 -> 0.3557.

### 3. THE FUSION — 299 SIGNALS FROM FOUR OBJECTIVES

BOOK-50 (3,101 trades, WR 90.6%, PF 4.81, net $97,675, folds 6/6, OOS PF 2.95) remains the incumbent
and the engine canary. OPTION-B — 120 signals with the p90 gate stack — gave 1,812 trades and 27 loss
events at PF 16.29.

**The union of BOOK-50, the 60-priced, OPTION-B and S0-120 gives 299 distinct signals from 385
(193L/106S, 1.82:1).** S0-120 contributes **111 unique signals at 1-8% overlap with every prior
selection**. Four different selection objectives, four different error modes, and that diversity is the
finding rather than any one member. The 35-q set contributes nothing unique — it is a strict subset of
the 60 — and must stop being counted as a source.

**Every large draw from the union beats OPTION B by 55-87%.** The union is the achievement.

### 4. DEFECT ONE — THE BAR IS THE RISK UNIT, NOT THE TRADE

Found by the operator asking for all 48 loss rows to be printed individually rather than summarised.

**The 48 losses collapsed to SEVEN bar-events.** The jar had admitted 4-9 lots of the same losing trade
— identical ATR, identical entry price, identical exit bar. Every loss rate, confidence interval and
profit factor in the project to that point had been computed on replicas of one decision.

**All loss statistics are now stated as events, and no ratio is reported without its event count.**
Under the adopted configuration 224 trade-losses are 43 events on 36 distinct days. Option B's 27
events remains the only well-sampled loss figure produced in this project; a 7-event book cannot be
audited inside a year of live trading, which is why the operator rejected the safest-looking books.

### 5. DEFECT TWO — THE JAR WAS ACTING AS A HIDDEN GATE, AND `CURRENT` ADMISSION IS NOT BUILDABLE

Found by the operator asking why `MAX_POSITIONS` was 16.

The engine's per-signal loop refused at `live_lots >= MAX_POSITIONS` **mid-event**. A chopped event
therefore recorded a LOWER executed depth and received a STRICTER gate: `AT_Slope_ST > p90` passes
6.2% of bars where `VolOfVol > p20` passes 82%. Measured, re-tiered bars gave 601 trades at a **1.66%
loss rate against 3.95% for correctly-tiered ones.**

**That accidental filter was worth 35 loss events. Nobody chose it, nobody could state its mechanism in
advance, and it would not survive a change of cap, lot size or signal count.**

**And it cannot survive to a build.** Executed depth is only knowable after a bar's admissions finish,
so the post-hoc floor filter deletes **9,356 trades that a live engine has already opened** — 1,391 of
them from valid depth>=3 events. A live EA built to the `CURRENT` specification would run a book that
had never been scored.

**FLOORED is the only buildable rule:** batch the bar, take `n = min(admissible, free)`, refuse the
whole group if `n < 3`, gate on `tier(n)`. Everything is knowable at bar close. **No position is ever
opened and then removed. Backtest equals live, exactly.**

The honest cost, before repair: 47 loss events -> 82, PF 16.23 -> 9.74. The repair — gating LONG d3,
which the jar had been covering — brought it back to 43 events at PF 13.09.

**AND THE REFERENCES DID NOT SURVIVE EITHER.** Re-scored under FLOORED, OPTION B goes 27 -> 79 loss
events and PF 16.29 -> 7.65, because its gate stack lives at solo and dual and depended entirely on the
jar chopping deep events down into those tiers. CELL D degrades least (7 -> 14 events) because a deep
floor produces rare, large events the jar rarely chops. **Depth confers robustness to this defect, and
that is the mechanism behind the whole table.**

### 6. DEFECT THREE — `recentfb_sizing` WAS SIZING UP LOSERS MORE THAN WINNERS

Found by the operator asking whether conviction sizing had ever been swept. It never had, in 17 months.

Removing `recentFB x1.25` drops lots on winners 1.256 -> 1.183 (-5.8%) and lots on losers 1.186 ->
1.094 (**-7.8%**). **It was adding 25% size to trades that lose more often than they win.**

Removing it takes **20% off the worst bar** (-$458.9 -> -$367.2) for 3.9% of net. Setting it to 1.5
instead deepens the worst bar to **-$550.8** — the direction is confirmed both ways.

**The other two multipliers earn their place decisively.** `Micro_Hurst x2.0` off costs $9,043 and
introduces a losing week; `D2D-agree x2.0` off costs $3,824 and worsens the worst day. Flat sizing costs
$20,380 with a -$211.1 worst day. **Two of three sound, one a tail-deepener.**

### 7. THE DERIVATIONS THAT SURVIVED

**The d2->d3 cliff, measured independently in both directions** on the ungated field: LONG d2 16.55% ->
d3 8.75%; SHORT d2 12.54% -> d3 5.40%. Depth 1 and 2 are indistinguishable; three is a different
regime. The 16-cell floor grid confirms it — any cell containing a 2 gives 3.5x to 7x the loss events.
**Cliff below, plateau above: the best-evidenced constant in the stack, and it is the operator's
original triple-triple.**

**`Micro_Hurst > p90` cleared two independent nulls** — p = 0.000 at SHORT d3 (0 of 30 rarity-matched
alternatives strictly better) and p = 0.022 at LONG d3 (1 of 45). **The only condition in this project
ever to clear one.** For contrast, `Bar_Range > p95` sits at p = 0.371, the median of its own null, and
`VolOfVol > p20` at p = 0.690 with 40 of 58 alternatives doing better.

**`ADX >= 15` is structural rather than tuned.** The threshold oracle builds its rolling percentile ring
only from bars where `ADX >= 15`, so every threshold in all 299 signals is calibrated on that
population. Trading outside it fires thresholds against a distribution they were never computed from.
The same number twice.

**`ATR_1M >= 20` is a knee, not an optimum.** The curve is monotone in both directions — it is a risk
dial. 15->20 buys 13 loss events for $1,355 (9.6 per $1,000); 20->25 buys 12 for $4,467 (2.7 per
$1,000). The price of an event jumps 3.5x immediately above 20.

**And the cap curve, 18 to 96, locates both boundaries at 22.** First losing week: 22. Worst day first
past -$150: 22. The break is a cliff, not a drift — worst day -$104.0 -> -$277.5 and worst week +$16.6
-> -$110.9 for one extra slot, turning 26 consecutive positive weeks into 25. **Cap 21 is the last safe
value and the margin is one slot wide.** The cap stops binding entirely at 57 at-risk positions.

**The cap never touches the tail.** Worst bar -$367.2 and worst intraday -$845.91 are identical at all
thirteen cap values. The cap controls accumulation, not depth. **And divisibility does nothing** — swept
3/6/9/12/15/16/18/21 under all three admission rules, monotone throughout, no step at multiples of
three, because events run 2 to 47 deep and cannot tile a fixed container.

### 8. THE SELECTION LAYER REBUILT — AND THE REBUILD LOST

The selection layer was the one thing never redone honestly, and all four union sources had been chosen
while the `CURRENT` admission defect was silently filtering. It was therefore rebuilt from the ground
up: a train-only screen with a **proportional** fold criterion (`folds_plus >= 4` as a count is
arithmetically unsatisfiable for 503 signals that lack four buckets), holdout Jun 1 - Jul 21 held back
and scored once.

**12 of 12 random draws beat the rebuilt selector on losing-bar rate. The adopted book beat 11 of 12 on
net and sits at roughly the 4th percentile of random selection.** The rebuild's objective — minimising
the union of losing bars — selects for signals that *lose together*, which is the many-clones-on-one-bar
mechanism written into an objective function on purpose. It was reported as worse than random rather
than dressed up.

**253 of the adopted 299 clear the independent rebuild (84.6%),** with every failure accounted: 25 fail
the train screen, 16 were never scanned (14 BOOK-50 orphans plus 2 F1 pairs correctly absent from an F0
scan), 5 failed the field filters. **The book is not an artefact of the admission defect, and it is now
confirmed from a second, independent direction.**

**Decorrelation on solo losses is invalid as a selection tool regardless of basis.** Solo, a signal
trades alone on every bar its mask fires; in the book only depth-gated bars produce entries. Solo loss
bars are not the population the book loses on — solo pairwise Jaccard 0.0026 with 99.3% of pairs sharing
zero losing bars, against 0.0247 and 94.8% at book level. **Delete the stage, do not repair it.**

### 9. COVERAGE SATURATES AT 68 SIGNALS

The terrain map was built to expose the incumbent's poor participation, and the question was finally
asked of the adopted book.

**Greedy marginal coverage over all 4,575 qualified survivors exhausts after 68 signals.** After that,
no remaining signal in the field reaches a single permitted episode the first 68 do not already reach.
The solo-reach union of the entire qualified field is **101 UP / 91 DOWN of 498/486 permitted (20%)**,
and it is flat at every book size from 299 to 4,575. **The 4,276 unused signals are not unused coverage
— they are redundant on the coverage axis.**

And chasing coverage is catastrophic: a 500-signal coverage book gives 285 loss events, PF 1.78 and a
-$4,352 intraday floor. **More signals does not mean more distinct opportunities; it means more signals
piling onto the same bar.** Size costs concentration, not exposure — the jar holds at 16 either way.

**Solos and duals were then tested as a separate strictly-gated admission path and closed.** Strictness
and coverage are anti-correlated and the correlation is near-perfect: the two strictest gates in the
library (0.489% and 0.684% pass rate) reach **zero** new episodes. A 2-bar episode requires firing on
one of two specific bars, and any condition rare enough to be trusted alone is too rare to be there.
**Rarity and presence are the same axis pointing opposite ways.**

**`ATR_1M >= 20` removes 76% of reachable terrain before any signal is consulted** — 2,087 UP episodes
become 498. That is a structural ceiling no selection can lift, and it is the gate doing its job.

### 10. WHAT THE SYSTEM IS BLIND TO

Characterised for the first time, and the answer is clean.

| | traded | unreached |
|---|---|---|
| duration | 6-9 bars | **2 bars** |
| thrust size | ~180 pts | 132 pts |
| ATR at start | 30-32 | 24 |
| ADX / efficiency | 25-27 / 0.45-0.52 | 24-25 / 0.50 |

**ADX and efficiency are indistinguishable.** It is not a trend-quality filter — it is a **duration**
filter. Session distribution is proportional rather than selective, so the blindness is structural, not
temporal.

**The mechanism is the depth floor: concurrence takes time to form.** A 2-bar move ends before three
independent patterns can agree it is real. **This is a property of concurrence as an entry mechanism,
not a tunable parameter — to be accepted and documented rather than engineered around.**

### 11. THE ADOPTED SYSTEM

`foundational_documents/The_Whole_DOT_spec_v2.txt` — 1,524 lines, sha `1d0323b1ecfa`. It supersedes
`The_Whole_DOT_rule_master_spec.txt` and `_2`, and both of those superseded
`DOT_rule_master_spec_OPTION_B.txt`.

**297 signals, ALL F0 TRIPLES · LONG depth >= 3, SHORT depth >= 3 · FLOORED admission · `Micro_Hurst > p90` at LONG d3
and SHORT d3 · `Micro_FailedBreak > p20 AND AT_Slope_ST > p90` at LONG d4 · `Micro_FailedBreak > p20` at
LONG d5+ · SHORT d4 and d5+ FREE · `ATR_1M >= 20` global · `MAX_POSITIONS 21` counting at-risk positions
only · conviction `Micro_Hurst x2.0` and `D2D-agree x2.0`, `recentfb_sizing = FALSE`.**

At 1.0 lot, as adopted at 297 signals: 5,776 trades, WR 96.12%, PF 14.53, net $284,974.00, **42 loss events on 35 days across 973 entry bars**, worst bar
-$1,224.00, worst day -$346.60, worst intraday -$2,819.70, **zero losing weeks of 26**, 119 of 132 days
traded, durability margin 32.91 points against a break-even win rate of 63.18%.

**Linearity is proved by direct re-run rather than asserted.** 1.0 and 0.30 run independently give 5,799
rows both, identical on every row for entry bar, exit bar, entry price, exit price, exit type, depth and
stop distance; lots ratio uniformly 0.300; drift $1.65 (0.0019%), which is engine P&L rounding.
`MAX_RISK` is **150 POINTS, not dollars** — it enters the decision path nowhere, and `lots` appears at
exactly two lines, both P&L multiplications.

**1.0 lot is not deployable** — its ceiling is 126% of the FTMO daily. At 0.30 lot: net $85,607, worst
day -$104.00, worst intraday -$845.91, ceiling $1,890 = **37.8% of the $5,000 daily and 18.9% of the
$10,000 total.**

### 12. THE FINDING THAT REFRAMES THE PROJECT

Across **60 complete compositions and three walk-forward windows, every single one is profitable** —
field net $12,788-$18,532, survivor net $14,987-$19,119, spread 25-30% of net. And **24 random
299-signal draws from the qualified field are all profitable out of sample.**

**The signals carry it. The composition is a preference, and selection above a competent screen is worth
surprisingly little.**

This is the most important measurement produced in this project. It means the system does not depend on
any clever choice being right — which is also why the composition arguments of this period, however
long they ran, moved the result around inside a band where every point works.

### 13. WHAT IS NOT PROVEN

- **Nothing is out-of-sample in the strict sense.** The three walk-forward windows come from a book
  selected on the full frame. July is the one genuine signal-layer holdout — no fold touched its 19,842
  bars — and it is the best month in the book at PF 41.35 on 3 loss events. Small base, right direction.
- **The process is not walked forward.** Only S0-120 can be re-derived inside a training segment; the
  field and three of four union sources are full-sample.
- **43 loss events bound every risk claim and 36 distinct days bound every day-keyed one.**
- **The fill model is the last unmeasured assumption.** The engine fills at `current_sl` exactly whenever
  the bar touches it, with no slippage and no gap modelling. "Locked positions cannot lose" is true of
  the simulator **by construction** and true live only to the extent fills are clean. Verified within the
  simulator across all 5,317 be-nudged trades — exits strictly below the lock 0, minimum captured +12.02
  points, minimum P&L +$9.00 per lot, **and +12.02 is exactly what `0.30 x base_risk` predicts at the
  ATR-20 gate on the base path.** A prediction and a measurement agreeing to two decimals. The ceiling is
  exact under the fill assumption and a lower bound live. **This is what the demo period is for.**
- **Three defects have now been found by readers rather than by the analyst, and all three share a
  shape:** the specification described something the engine computed differently, or a story got attached
  to a number nobody traced. **Expect a fourth and read `§0.3` of the spec first.**

### 11a. THE BOOK REDUCED TO 297 — THE TWO F1 SEQUENTIAL PAIRS DROPPED

The operator asked why two of the 299 were not triples. Both entered with BOOK-50 at lines 50-51 and
survived every fusion because the union filtered by nothing — `Sqz_Val:hi ->13-> Micro_OrderFlowDelta:lo`
(35 trades, PF 5.90, 6/6 folds) and `ADX_Rising:==0 ->8-> D2D_DirStep:==-1` (42 trades, PF 4.75, 6/6 folds),
both LONG, both `d2d confirm`. **THEY WERE NOT ORPHANS** — both carry full statistics in
`results_F1_sequential_temporal`, so the orphan count stays at 14 (6 LONG, 8 SHORT) and only the scan ratio
moves, 1.82:1 -> **1.80:1**, because both drops were LONG.

In-book they were 12 trades and $476.2 combined — 0.21% by count, 0.17% by net.

**REMOVAL IS BETTER ON EVERY AXIS THE OPERATOR RANKS AND IDENTICAL ON THE REST:**

| | 299 | 297 |
|---|---|---|
| loss events | 43 | **42** |
| event-days | 36 | **35** |
| losing-bar rate | 4.40% | **4.32%** |
| PF | 14.31 | **14.53** |
| worst bar / day / intraday | -$1,224.0 / -$346.6 / -$2,819.7 | identical |
| losing weeks / worst week / days traded | 0 / +$55.2 / 119 | identical |
| net | $285,356.5 | $284,974.0 |

Cost $382.50, which is 0.13% and a lot-size dial. **Their net depth contribution is about -$94, so removing
them very slightly helped the rest of the book** — the orphan effect that made the six LONG BOOK-50 members
cost $3,923 to remove while earning $2,649 does not repeat here.

**AND THE LARGER PRIZE IS STRUCTURAL RATHER THAN NUMERICAL.** The F1 path leaves the configured scoring path
entirely, so `score_g.build_book` writes nothing back into the frame and **the frame-object trap becomes
impossible rather than avoided.** That defect cost 11 trades and produced a plausible wrong answer rather
than an error. `sequential_temporal.pair_mask` and `anchor_array(df,'ST_Flip')` are no longer needed, build
item B3 is retired, and **every row of the listing is now the same three-condition shape — verifiable by a
build script rather than by eye.**

### 11b. JUNE INVESTIGATED — THE FLATNESS HYPOTHESIS REFUTED, NO GATE FITTED

June carries **14 of the 42 loss events on 164 of 973 bar-events**, PF 5.34, the book's worst day at
-$346.60, and it is the weak split in every fold and walk-forward column. The operator's hypothesis was that
the month is too flat and that some regime characteristic — insufficient momentum, ADX too high, ticks too
low — should be factored into a global gate.

**ALL THREE POINT THE WRONG WAY. JUNE IS THE SECOND-BUSIEST MONTH OF SEVEN.** ATR 11.12, Volume 94 and
Bar_Range 11.0 all rank 6 of 7; TickIntensity 6 of 7; ADX 22.61 ranks 4 of 7, and May has a higher share of
ADX>=30 bars. Bars at `ATR_1M >= 20` are 19.6% in June, second only to March's 40.7%.

**AND THE COUNTEREXAMPLE IS DECISIVE. MAY IS THE FLATTEST MONTH IN THE FRAME — lowest ATR at 9.07, lowest
Volume at 67, lowest Bar_Range at 8.5, fewest eligible bars at 12.2% — AND MAY IS THE BEST MONTH IN THE BOOK
AT PF 60.64 ON TWO LOSS EVENTS.** Flatness is not the mechanism.

At event level, June's losers are separated from June's winners by the same variables, in the same direction,
by the same magnitudes as everywhere else. ATR ratio 0.96 against 0.82 elsewhere; ADX 1.13 against 1.17;
EffRatio 1.60 against 1.23; Hurst 0.96 against 0.98; VPIN 0.91 against 0.87. **There is no June-specific
signature.**

**AND THE ELEVATION IS UNIFORM, WHICH IS THE SIGNATURE OF A MONTH RATHER THAN OF A BAR-LEVEL STATE:**
BASE 13.75% against 5.18% (2.65x), MOM 3.57% against 2.29% (1.56x), LONG 11.32% against 4.44% (2.55x),
SHORT 3.45% against 1.91% (1.80x). **Every sub-population is worse and none is untouched.**

The one apparent exception collapsed on inspection. `Bars_Since_Flip` showed 4.04x in June against 1.00x
elsewhere, but the fourteen values are bimodal — 0, 3, 3, 7, 14, 21, 28, 81, 111, 114, 188, 286, 287, 359 —
seven under 30 bars and seven over 80, with **half the losses on fresh flips.** The median of 54.5 is an
artefact of a 7-and-7 split on fourteen observations, June's winning events already have an upper quartile of
69, and no threshold separates them. `Bars_Since_Flip` is also not in the Option B library, so a gate there
would be a new threshold on a bimodal split of 14 events.

W23 is four days, not one. Six events across 06.01, 06.02, 06.04 and 06.05, with 3 of the 6 on 06.05 — the
book's worst day — and two of those three on **consecutive bars at 22:52 (depth 9) and 22:53 (depth 10): one
move, two minutes, 19 trades, -$1,290 combined.** That entangles the "weak d9 / strong d10" contrast from
the depth ladder and is worth recording for that reason alone.

**TRIAL COUNT ZERO. NO GATE PROPOSED.** Eleven variables at bar level, six at event level, two path controls
and two direction controls, on a base of fourteen events. `Efficiency_Ratio < p80` was the only free library
candidate matching the direction and it refuses 31% of all 931 winning events to catch half the June losses.
**FOURTEEN EVENTS CANNOT SUPPORT A THRESHOLD, AND THE NEXT VARIABLE TESTED WOULD EVENTUALLY SEPARATE THEM BY
CHANCE. THAT IS THE POINT AT WHICH A SEARCH BECOMES FITTING.**

**June is therefore a documented limitation and not a defect.** It is 33% of the loss events on 17% of the
bar-events, it costs nothing in profitability at net +$32,591 — the fifth-best month of seven — it never
produced a losing week, and **no bar-level state would have warned the engine.**

### 11c. THE DEPTH LADDER — MOSTLY SMALL CELLS, AND A TIER CAP THAT CANNOT TARGET THEM

The full depth ladder now prints on every run, and the LONG side is strongly non-monotone: PF 7.20 at d3,
5.10 at d4, 10.59 at d5, **3.26 at d6**, 58.46 at d7, 9.32 at d8, **1.70 at d9**, 23.96 at d10 and infinite
at d11+, where 1,202 trades carry $96,712 with **zero loss events** — a fifth of the trades and a third of
the net.

**THE FIVE WEAK CELLS — LONG d3, d4, d6, d9 AND SHORT d7 — ARE 1,445 TRADES, 24 OF THE 42 LOSS EVENTS AND
ONLY $44,635 OF $284,974.** That reads as a targetable inefficiency and it is not.

**FIRST, `tier = min(depth, 5)` MERGES LONG DEPTHS 5 THROUGH 21 INTO ONE GATE CELL — 3,559 trades, 86.7% of
the LONG book, all gated by `FailedBreak > p20` alone.** So d6 at PF 3.26 and d9 at PF 1.70 are gated
identically to d7 at 58.46 and d11+ at infinity. **THE TIER STACK IS STRUCTURALLY INCAPABLE OF
DISTINGUISHING THEM**, and `min(depth,5)` appears nowhere with a derivation — it is inherited from Option B
and now carries a registry line saying so.

**SECOND, THREE OF THE FIVE CANNOT BE READ AS RATES.** LONG d3, LONG d9 and SHORT d7 rest on 3, 3 and 1 loss
events. **d9's "PF 1.70" becomes 2.99 on one event fewer and 1.30 on one median event more; SHORT d7 becomes
infinite on zero.** Only LONG d6 has a base worth arguing about at nine events, and five of those nine are in
June.

**THIRD, THE NEIGHBOUR COMPARISON FOUND NOTHING TO GATE ON.** d6 and d7 are gated identically and perform 18x
apart, and they match within noise on ATR (36.63 vs 40.33), ADX (23.79 vs 21.67), Hurst (0.4260 vs 0.4235),
FractalDim, VolOfVol, EffRatio, bars-since-flip (7 vs 7), session hour (10 vs 10) and momentum share (64.8%
vs 66.7%). **The only real difference is 54 stop-outs against 7, which is the outcome restated, not a cause.**
The same holds for d9 against d10 and SHORT d7 against d6, where ATR and VolOfVol run *higher* in the strong
cell — the opposite of a "weak cells are quieter" story.

**MOVING THE TIER CAP WAS CONSIDERED AND DECLINED ON ARITHMETIC.** Opening d6 and d9 as their own cells is
2 cells x 11 library relocations = **22 trials against 12 loss events, and Bonferroni needs p < 0.0023.**
Nothing in this project has cleared that; `Micro_Hurst` reached p = 0.000 only because it was one relocation
at one cell on 15 events. **A GATE FITTED TO d6 WOULD BE A JUNE FILTER WEARING A DEPTH COSTUME, FITTED ON
FIVE EVENTS.**

And one ladder figure was checked before anything was built on it. **SHORT d11+ is thirteen replicas of a
single trade** — all entering 2026.02.23 at 16:47 at 49427.90 and exiting 18:20 at 48860.75, 567.1 points
over 93 bars, trailed out at 26 tiers, $1,128.30 each, identical to the cent. **Not a 14x per-trade edge
against LONG d11+'s $80. One bar, thirteen times** — the same replica structure every loss event has, on the
winning side. Same caution applies to SHORT d8 (72 trades), d9 (18) and d10 (10), all printing infinite PF on
a handful of bars.

### 11d. THE ENTRY-BAR FUNNEL

    177,251 frame bars
    170,351 post-warmup (bar >= 6900)
     36,526 at ATR_1M >= 20                     20.6% of the frame
        973 ENTRY BARS                           0.5% of the frame, 2.7% of the eligible population

**973 entry bars over 119 trading days — 8.2 bars and 49 trades per day, $293 of net per entry bar.**

**`ATR_1M >= 20` IS THE LARGEST FILTER IN THE SYSTEM.** It admits 20.6% of bars and removes 76% of reachable
terrain before any signal is consulted. By contrast `FailedBreak > p20` admits 80.19% and is barely a gate;
`Micro_Hurst > p90` admits 9.75%; `AT_Slope_ST > p90` 6.22%; the stacked LONG d4 form 0.68%.

**AND EVERY ATTEMPT TO WIDEN THE FUNNEL FAILED, MEASURED.** The 500-signal coverage book gave 285 loss
events, PF 1.78 and a -$4,352 intraday floor. The strictly-gated solo/dual path found strictness and coverage
**anti-correlated** — the two strictest gates in the library, at 0.489% and 0.684% pass rate, reached **zero**
new episodes, because a 2-bar episode requires firing on one of two specific bars and any condition rare
enough to be trusted alone is too rare to be there. **THE 0.5% IS THE EDGE, NOT A SHORTFALL. CONCURRENCE
TAKES TIME TO FORM.**

### 11e. THE DETERMINISTIC LOCAL SCORING PATHWAY — BUILT AND RUN BY THE OPERATOR

    python master.py --data data --workers 14 --out discovery\full --stage S8 --book whole_dot_signals.csv

Reproduces the specification to the cent on the operator's own machine in 37 seconds. **Before this, every
figure in the project was something an analyst reported; now it is something he can produce.**

Delivered: **`engine/adm_engine.py`** (`6d1ed10a5f81`) — a FORK of the sacred engine implementing FLOORED
admission, the per-direction depth floor, the per-tier gate stack and a configurable `MAX_POSITIONS`,
defaulting to `CURRENT`/6/`None` so an unconfigured import behaves as sacred, and **proved byte-identical to
the sacred engine under CURRENT admission on the full frame.** **`engine/swept_thresholds.py`**
(`4356d2bb9973`) — mechanism-D percentiles at arbitrary levels by substituting `dt._D_SPEC` and calling the
sacred `compute_adaptive_thresholds`, so **the ring, the eligibility mask, the day-refresh and the
floor-index are bit-identical to production by construction rather than by inspection.**
`whole_dot_config.json` carries the rules with derivation labels inline; routing is on config presence and
accepts both sidecar conventions; and `_metrics_from_trades` is shared by both paths **so folds and OOS
cannot drift between BOOK-50 and a configured book.**

**TWO GUARDS, BOTH AGAINST SILENT FAILURE.** A startup fork-parity assertion, necessary because **the sacred
admission path is duplicated verbatim inside the fork's `elif` branch** — that duplication is why parity
holds and is the landmine if sacred ever changes. And config-not-found now **aborts**, after the sidecar name
mismatched during development and the Whole DOT silently took the sacred path, scoring the wrong system with
no error.

**AND A FULL BREAKDOWN REPORT ON EVERY RUN:** weekly (26 ISO periods) and monthly (7) tables carrying trades,
wins, trade-losses, **loss EVENTS**, W/L ratio, WR, PF, net, LONG and SHORT counts and worst day; the depth
ladder per direction; admit and refuse counts for all five live gates; the top five signals by net with the
ranking key stated; and the population and denominator lines named rather than implied.

**THREE DEFECTS WERE FOUND BY RUNNING RATHER THAN READING:** the sidecar-name fallthrough, a sliced frame
against a full-length oracle, and a week counter using `exit_time[:8]` — a day-level slice, not an ISO week —
which reported 7 weeks where the spec has 26. **And the 430-trade discrepancy that blocked acceptance for two
turns was the FULL-versus-BOOK-only population: `master.py`'s own `trades.csv` header states the distinction
and nobody had read it.** The parity sha is now stable across machines — it was float-repr and column-order
dependent, and `_canon_trade_sha()` fixes it with an explicit column list, an explicit sort and fixed 2dp
formatting.

**AND ONE INSTRUCTION-CLASS DEFECT WORTH RECORDING AS ITS OWN THING.** The Quant's handover said the gate
masks were "built via the swept-spec mechanism, not the default p80/p20 hi/lo" — **a description of something
only its author could execute.** The Developer fell back to `ad[(var,'hi')]`, which is p80, and a `> p90`
gate against a p80 threshold is a looser gate: 6,753 trades against 5,799 and 100 loss events against 42.
**IT IS THE SAME CLASS AS THE SIDECAR FALLTHROUGH AND THE `_adm_pass` PERMISSIVE LOOKUP — A GAP THAT READS AS
COMPLETE — AND SPEC §0.3 R2 IS NOW "ANY INSTRUCTION IN THIS DOCUMENT THAT A SECOND PARTY CANNOT EXECUTE",
with the check being to demand a symbol rather than a prose description.**

### 11f. BOOK B — THE ANTI-SYSTEM, BUILT AND CLOSED

The operator's question was direct: can a full book be composed from solos, duals and every non-F0 family,
under one hard rule — **no signal may trigger on any of the primary engine's 973 entry bars.** Not a research
question about whether it should exist, but a build.

It was built. **755 signals at `agg_pf >= 8`** — F1 750, F9 3, F13 2, drawn from 449,399 scanned non-F0 rows
of which 43,436 qualify. Exclusion asserted on every arm, aborting rather than warning.

    755 signals · 6,106 entry bars · 17,531 trades · WR 84.97% · PF 1.70 · net $166,255
    1,012 LOSS EVENTS on 122 days · worst bar -$1,606.80 · worst day -$2,775.50
    4 losing weeks of 26 · 127 of 132 days · episodes 56 UP / 67 DOWN
    break-even WR 76.88% against 84.97% actual — an 8.09-point MARGIN
    $164 net per loss event, against the primary's $6,785

**AND THE EXCLUSION ASSERTION FOUND AN ENGINE FACT BY FIRING RATHER THAN BY BEING READ.**
`portfolio_simulation_engine.py` L299 checks only `conviction is not None`, `len(active_trades) == 0` and
`bar >= warmup`. **The gap fillers never consult `entry_ok` or `mask_window`, so they bypass ADX>=15,
Volume>50, the Friday cutoff and any bar exclusion.** They fired on 36 primary bars on the first attempt.
Same class as everything in spec §0.1 — a population the specification describes differently from what the
engine does.

### 11g. WHY 755 SIGNALS AT PF>=8 COMPOSE TO PF 1.70

Every member carries `agg_pf >= 8` standalone. The composed book is 1.70. That paradox had to be resolved
before pruning anything, because if standalone PF were noise then the whole ladder was the wrong tool.

**IT IS NOT NOISE.** Standalone `agg_pf` against in-book net per loss event: **Spearman 0.3315,
p = 1.07e-17**, with a monotone band structure:

| PF band | n | median in-book net | % net-NEGATIVE in book |
|---|---|---|---|
| 8-10 | 261 | $23.40 | **46%** |
| 10-15 | 253 | $261.40 | 22% |
| 15-20 | 85 | $410.50 | 11% |
| 20+ | 34 | $505.70 | **3%** |

**261 of 755 members sit in the band where nearly half lose money inside the book** despite every one
clearing PF 8 standalone. 186 of the 699 that traded (27%) are net-negative in-book.

**AND OVERLAP IS NOT THE CAUSE.** 2,635 losing trades collapse to 1,012 events — a **2.60 replica ratio
against the primary's 5.33** — with 60.3% of events charged to more than one member. The event count is
genuine, not a replica count, so the prune target is quality rather than co-firing.

The mechanism is visible in two correlations: `Spearman(solo_pf, in-book trades) = 0.39` but
`Spearman(solo_pf, in-book loss events) = 0.17`. **High-PF members trade more without proportionally more
loss events.**

### 11h. THE PRUNE, AND THE SPLIT-HALF CHECK THAT DISCIPLINED IT

Two rankings were available — standalone PF from the scan, and in-book net per loss event. **The second is
in-sample and unpriced, which is the shape that produced a selector worse than random earlier in this
project**, so it was split-half tested before being trusted.

**The contribution ranking persists** — `Spearman(npe_A, npe_B) = 0.2228, p = 5.4e-6` — unlike the F1 reach
ratio at -0.064. **But the top-10 slice goes NEGATIVE out of sample**, at -$17.30 median half-B npe against
the population's +$17.00. And **PF ranking beats contribution ranking at every matched size out of sample**,
because PF is measured by the scan on a different population than the book.

| arm | sigs | events | PF | margin | worst day | net | npe | -wks | days |
|---|---|---|---|---|---|---|---|---|---|
| CONTRIB top 10 | 10 | 42 | 4.34 | **17.79** | -$370 | $12,842 | $306 | **0** | 116 |
| CONTRIB top 25 | 25 | 92 | 3.41 | 15.86 | -$1,104 | $26,825 | $292 | 2 | 118 |
| CONTRIB top 50 | 50 | 184 | 3.51 | 16.75 | -$1,250 | $55,203 | $300 | **0** | 125 |
| PF>=20 | 34 | 189 | 2.27 | 12.73 | -$1,006 | $33,251 | $176 | 2 | 124 |
| CONTRIB top 100 | 100 | 396 | 3.11 | 16.10 | -$1,578 | $106,198 | $268 | 1 | 126 |
| PF>=15 | 124 | 452 | 2.12 | 11.35 | -$2,169 | $89,078 | $197 | 2 | 127 |
| CONTRIB top 200 | 200 | 621 | 2.71 | 14.38 | -$2,463 | $169,254 | $273 | 1 | 126 |
| PF>=10 | 408 | 794 | 1.93 | 9.86 | -$2,904 | $156,687 | $197 | 4 | 127 |
| FULL 755 | 755 | 1,012 | 1.70 | 8.09 | -$2,776 | $166,255 | $164 | 4 | 127 |

**Top 10 and top 50 are the only arms with zero losing weeks, and top 10 carries exactly 42 loss events — the
same as the entire primary book.** But the split-half says the extreme slice does not hold, so **top 50 is
the strongest defensible arm.**

**And July splits the two rankings.** All three PF-ladder arms are negative in July (-$363, -$1,239,
-$1,073); every contribution arm is positive (+$263 to +$5,056). **July is the primary's best month and the
only genuine signal-layer holdout.**

### 11i. THE TRADE-MANAGEMENT SWEEP — THE ATTRIBUTION TESTED RATHER THAN INFERRED

The margin verdict after the prune was "partly size, mostly family" — 9.7 points bought by shrinking, 15.3
points remaining. **That attribution was untested, and the five trade-management constants had been swept on
a depth-3+ book at cap 21, never on a depth-1 book at cap 6.** The payoff ratio was named as the binding
constraint, so the sweep aimed at it.

**THE PREMISE THAT MOTIVATED THE RUN WAS REFUTED FIRST, WHICH IS THE RIGHT ORDER.** The hypothesis was that
Book B's winners never run — that every exit is at the break-even lock and `LF` never fires because a depth-1
book cannot reach `tiers >= 3`. **False.** Exit mix is **72.1% BE at $19.51 / 12.8% LF at $69.81 / 14.8% SL**
against the primary's 78.3% / 17.7% / **3.8%**. Winners do run.

**And the payoff gap is not truncation.** Average loss is nearly identical — $90.18 against $94.06. The
difference is entirely on the win side and decomposes into two: **an SL rate of 14.8% against 3.8% (3.9x)**
and **an LF mean of $69.81 against $174.85 (2.5x lower)**. More stop-outs, and the trails that do run capture
less.

**THE PAYOFF RATIO MOVES AND IT BUYS NOTHING.** `BE_TRIG_FRAC 2.0` takes it from 0.347 to 0.522 — a 50%
improvement — and it is the worst row in the table at margin 10.27, 467 loss events and PF 1.65.
`STEP_PCT 0.60` reaches 0.510 at margin 9.79. **Every setting that improves the ratio converts winners into
stop-outs faster than into runners**, SL share going 10% -> 19% and 10% -> 29%. Tightening is symmetric:
`STEP_PCT 0.15` nearly doubles LF share to 30.4% and drops the margin to 10.87.

**The baseline is the maximum of the margin surface in every direction tested. Eighteen settings, five
constants, both directions. Nothing beats 16.75.**

**And `LF_TIER_MIN` is inert.** At 1, 2 and 3 the net, events, PF and margin are identical to the cent at
$55,203.40; only the BE/LF label moves, with LF share running 14.9% -> 26.3% -> 90.9%. **The trail already
engages at `tiers >= lag + 1`; the `tiers < 3` literal only decides which bucket a completed trade is filed
under.** That closes the most direct fix anyone proposed, on a measurement rather than an argument.

### 11j. BOOK B CLOSES: MOSTLY FAMILY, PROVEN

The prune buys 9.7 points of margin. **The remaining 15.3-point gap to the primary's 33.07 is structural and
it is not the exit logic** — the trade management is already at its margin-maximising value on a depth-1 book
at cap 6, which is the same place it landed on a depth-3+ book at cap 21.

**It is the entry population.** A 14.8% stop-out rate against 3.8%, and LF capturing 2.5x less. **ONE SIGNAL
FIRING ON A BAR IS A WORSE BAR THAN THREE SIGNALS AGREEING — WHICH IS THE d2->d3 CLIFF ARRIVED AT FROM THE
OPPOSITE DIRECTION.** The depth thesis has now been confirmed by building the anti-system and watching it
underperform, which is a stronger form of evidence than the original measurement.

**Not adopted.** On the operator's ordering, adding Book B buys **19% more money for 438% more loss events** —
$55,203 and 184 events against $284,974 and 42.

**One property of B is worth keeping.** It accumulates evidence roughly **nine times faster** than A: 1,965
entry bars and 184 loss events against A's 973 and 42, which is 11 bars per event against 23. **A poor book
and a good sensor** — thin margin, high frequency, trading bars A never sees. If the market shifts, B shows
it first. Worth running on demo alongside as a leading indicator; not worth running for money.

### 11k. COVERAGE IS CLOSED ON EVERY FAMILY — IT IS A SELECTIVITY CEILING

The density control — does a mask reach more permitted episodes than the same number of **random** available
bars — was built for this question and answered it at both ends of the selectivity axis.

| population | density control |
|---|---|
| F3, F9, F4, F2, F11 as unions | ALL at or below random |
| 43 PF>=4 members individually | 4 pass, **all in one window** (Thu 10:00), union 23 of 40 summed |
| F13 tight percentiles p95-p99 | 0 of 15 pass; `Micro_Hurst:hi@p99` at **0.65** |
| B-strict composed (4 F9) | 1.71 on the mask — but $93 per event and 8 losing weeks in 25 days |
| B-full composed (103 signals) | 0.73 |
| F1 members, k>=10 | **1.19 / 1.28 across halves — REPLICATES** |
| F1 ratio-selected | **ANTI-PREDICTIVE, Spearman -0.064** |
| F1 composed, unbiased | 1.01 |
| F1 composed, executed | 0.94 |

**F1 is the only population ever to beat the control at the median** — 1.092 on 748 members with 395 beating
it, against ~0.6 and 4-of-43 for every state family. **And it scales with k:** k<=3 at 0.782 with 18%
beating, k>=10 at 1.141 with 61%, **Spearman(k, ratio) = 0.185 at p = 3.4e-7.** At short lags the pair is
effectively a same-bar conjunction and fails exactly as state masks do; by k=10-15 it stops tracking any
single bar's state. **The control had only ever seen state masks, and F1 is the first non-state grammar it
was given.**

**But the population effect survives neither selection nor composition.** Reach efficiency does not persist
across halves at all, and every top slice does *worse* out of sample than the population it was drawn from.
The unbiased composed arm fails at 1.01, and the executed book at 0.94 once admission, the depth floor and
the jar reduce 1,668 mask bars to 758 entry bars.

**AND THE FIGURE THAT SETTLES IT: all 29,948 available bars together reach 491 of 498 UP and 480 of 486 DOWN.
98% of the permitted landscape is reachable by being present on almost every bar.** Episode coverage is a
near-linear function of how many bars you fire on. **The F0 ceiling of 101/91 was never a reach limit — it is
a selectivity consequence.**

Selectivity-adjusted, the primary is in a different class: **41.1 episodes per 1,000 bars against F3's 18.7,
F9's 21.4 and F11's 19.2 — twice the best alternative.**

**Coverage should not be re-opened without a family whose grammar is neither state-based nor pair-based, and
no such family exists in this project.**

### 11l. THE GAP FILLERS — DEFINED, SCORED, NULLED, AND A BUILD ITEM RETIRED

BOOK-50 was **48 triples + 2 sequential + 3 singles = 53 members** in a file named for 50. The three singles
were implemented as hardcoded gap-fill paths rather than CSV rows, which is why the file shows no
single-condition entries and why every "50-signal book" figure in this project was really 53.

**Their definitions were written down for the first time** (`conviction.py` L51-56, firing at
`portfolio_simulation_engine.py` L300-322). Common gate `ADX_Value >= 15 AND Volume >= 300` — **a tick gate,
not the book's `Volume > 50` eligibility gate** — admitting 6.87% of bars, plus `len(active_trades) == 0`:
**they fire only when the book is completely flat.** Priority is a strict elif chain, D2D -> HURST -> FB.

    GAP_D2D    D2D_Signal != 0 AND ADX >= 30 AND Micro_Hurst >= p30    2.0 lots, LOCK_FRAC 1.0,  36 bars
    GAP_HURST  Micro_Hurst > p97 AND D2D_Trend_Dir == +1               1.0 lot,  GAP_LOCK 3.0, 170 bars
    GAP_FB     Micro_FailedBreak > p90 AND D2D_Trend_Dir == -1         1.0 lot,  GAP_LOCK 3.0, 573 bars

Scored at 1.0 lot: **GAP_HURST 55 trades / PF 6.48 / $3,036 / 7 events**; **GAP_FB 355 / PF 5.29 / $19,879 /
32 events / zero losing weeks across 103 trading days**; GAP_D2D 20 / PF 1.83 / $506. All three together: 430
trades, PF 5.04, $23,421, 41 events. **At $54-56 per trade they beat the book's $49.34.**

**Against 5,160 scanned singles, GAP_HURST and GAP_FB sit in the top 0.1%** — and the highest-PF row in the
entire F13 scan is `Micro_Hurst:hi@p99 LONG` at PF 8.199, **the same variable at a marginally stricter
percentile, found twice by different routes.**

**Both clear their null at p = 0.000 on profit factor** against draws of the same firing count from the same
gate population. GAP_FB clears on net as well; **GAP_HURST does not, at p = 0.214 — its edge is per-trade
quality, not aggregate.** A rarity-matched null could not be built: pool pass rates run 4.82%-18.62% and the
fillers fire on 0.02-0.32%, two orders of magnitude apart.

**They do not belong in the book.** Adding them takes loss events 42 -> 83 and degrades all three OOS
windows, while improving worst day and losing days because they trade on five days the book never touches.
**And they cleared their nulls on profit factor, not on coverage — excellent entries on bars that are not
scarce, reaching no terrain the primary misses.** That distinction was conflated once and is now separated by
measurement.

**AND BUILD ITEM B1 IS RETIRED.** `DOT.cs`, 11,673 lines, grepped exhaustively: **gap filling does not exist
in the EA.** No `HURST_GAP`, no `GAP_LOCK`, no `D2D_GAP_LOTS`, no enter-when-flat logic, and the EA's
percentile calls are p80/p20 only. **The L302/L306 lot defect cannot affect a live deployment because the
mechanism is not deployed.** It remains a simulator correctness note for anyone reading absolute-survival
figures.

### 11m. A THIRD PATH-RESOLUTION DEFECT

`sys.path` resolved `adm_engine` to the repo copy at `dot_master_discovery/engine/` rather than the working
copy, so an `LF_TIER_MIN` edit went to a file that was not being imported. **Diffed before proceeding —
otherwise byte-identical, so every prior Book B figure is unaffected.** Both copies synced.

**Third defect of this shape in the project: a thing resolving somewhere other than where it was written.**
Same ancestor as the sidecar fallthrough and the swept-spec instruction, and it is spec §0.3 R2 — an
instruction or a reference that resolves somewhere the author did not intend.

### 14. STATUS

The Whole DOT is specified and the research is closed. What remains is engineering: S8 cannot yet score
the book (`master.py` L1005 hardcodes `recentFB=True`, and S8 reads no floor, no tier gates and no
admission rule — it scores at cap 6, 1.0 lot, flat rules), `whole_dot_signals.csv` is unbuilt, the
gap-filler lot defect is unfixed, and the rejection log for FTMO's fill-or-kill execution does not
exist. Steps 30-35 of the execution sequence carry these. **Sacred five intact and unmodified
throughout.**
