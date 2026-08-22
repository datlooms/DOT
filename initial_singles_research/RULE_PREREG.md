# PRE-REGISTRATION — THE TURN RULE (quant seat, 2026-08-22)
Written BEFORE any scoring run. The scoring script reads this file's constants; nothing below is changed after a result is seen.
Base: HEAD f27840ef2683 · frame 46586cbb1671 (177,251 x 172) · oracle dots_thresholds.py 518862bf19fb (imported; no threshold recomputed) · engine adm_engine.py (unchanged).

## WHAT THE RULE IS
R_d(k): a bar is admitted in direction d if
  (1) PARTICIPATION, unchanged: ADX_Value >= 15, Volume > 50, post-warmup (bar >= 6900), not Friday-close; D2D_Trend_Dir == d; ATR_1M >= 20 (applied by the engine as the 297's global gate)
  (2) TURN: at least k of the n TURN conditions T_d are true on the bar   (k is THE swept dial)
  (3) NOT SUPPRESSED: no condition in S_d is true on the bar
  (4) GATES, as the 297 has them for a deep bar: the 5+ tier cell (LONG Micro_FailedBreak > p20 via swept_thresholds; SHORT free), cap 21, FLOORED admission, conviction hurst/d2d on, recentfb False.
No membership, no signal names, no triples. Every condition is one of the existing 249 (90 hi/lo via mechanism D at p80/p20 + 69 equality label-values). No new percentile, no tuned cutoff.

## HOW T_d AND S_d ARE DERIVED (mechanical, identical in every segment)
Given a training prefix P (all post-warmup bars with Time strictly before the scoring window; for the full-frame setup, P = the whole frame):
  traded(P,d)   = entry bars of the 297 control replay (973 bars this session) that lie in P with direction d
  untraded(P,d) = eligible bars in P (participation line + D2D == d + ATR_1M >= 20) that are not 297 entry bars
  For every condition c of the 249: t_c = share of traded(P,d) where c is true; u_c = share of untraded(P,d) where c is true, reweighted so the untraded (EST_hour) distribution equals the traded one; lift_c = t_c / u_c.
  T_d = the n = 6 highest-lift conditions among those true on >= 20 traded(P,d) bars. If fewer than 6 qualify, T_d is whatever qualifies; if fewer than k qualify, R_d(k) is UNDERIVABLE in that segment and is reported as such, not scored.
  S_d = every condition with lift_c <= 0.25 AND u_c >= 0.05.
The derived T_d and S_d for every segment are written to rule_derivations.json and printed in the report. They are NAMED conditions.

## CONSTANTS, AND WHICH ARE DIALS
  k       swept over {2, 3, 4, 5, 6}                     THE dial. 5 trials per (derivation x convention).
  n = 6   size of the TURN set                           chosen before scoring, not swept
  0.25    suppression lift ceiling                       chosen before scoring, not swept
  0.05    suppression untraded-rate floor                chosen before scoring, not swept
  20      minimum traded bars for a TURN candidate       standing rule (ratios below 20 are counts)
  K_RR = 30  round-robin detector count                  position convention, not a rule parameter
Three chosen constants, one swept dial, one standing floor. If a result turns out to need any of n / 0.25 / 0.05 moved, that is reported as a fourth dial and the run stops.

## POSITION CONVENTIONS (both reported on every row)
  ONE  one detector per direction; the engine's one-position-per-signal lock becomes one open position per direction at a time.
  RR   qualifying bars assigned in time order to 30 detectors (bar i -> detector i mod 30): one 1.0-lot position per admitted bar, no lock, cap 21.
Lots per trade are the engine's conviction multipliers, as for the 297. Concurrency and lots-per-bar reported per row.

## SCORING PLAN, IN ORDER
  0  reproduce the 297 control to the cent (must be 5,776 / 973 / 42 / -1,224 / -346.60 / 0 weeks / 119 / 96.12 / 14.53 / 33.07 / 284,974)
  B  bars found per direction, overlap with the 973, bars the book misses — before any scoring
  C  full-frame derivation, scored on the full frame (in-sample setup): k in {2..6} x {ONE, RR} = 10 runs
  D  walk-forward on the sacred wf.FOLDS (calendar months by Time year.month): for each fold m in {Feb, Mar, Apr, May, Jun(1-25)} derive on P = months before m, score with mask_window = bars in month m only. Jan(19-31) has an empty prefix: UNDERIVABLE, reported. July (2026.07, outside wf.FOLDS) is scored as an EXTRA out-of-segment window derived on Jan-Jun and labelled as such. 6 windows x 5 k x 2 conventions = 60 runs. Aggregate OOS = union of the fold trades.
  E  monthly table on the 297's own rows, June stated separately; Jan (5 days) and Jul (13 days) are counts.
Planned engine trial count: 1 + 10 + 60 = 71. Reported figures: loss events -> worst bar -> worst day -> losing weeks of 26 -> days of 132 -> entry bars -> trades -> WR -> PF -> MARGIN -> net last, at 1.0 lot. A ratio on fewer than 20 events is stated as a count.
Survival first: any variant with a day below -$2,500 has failed regardless of net; the binding constraint is the losing week.
