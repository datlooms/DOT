# PRE-REGISTRATION — PART B (the differentiator on top of A), quant seat 2026-08-22
Written before any A+B scoring run. Base HEAD 13f41f61a34c · frame 46586cbb1671 · oracle 518862bf19fb (imported) · engine adm_engine.py unchanged.
A = R_d(k=2) exactly as RULE_PREREG.md (aa15dbb), full-frame T/S for the in-sample setup, prefix-derived T/S in each walk-forward window.

## WHAT B IS
B_d(j): at least j of the m = 3 conditions in G_d are true on the A-bar. A+B admits a bar iff A admits it AND B_d(j) holds. Nothing else changes: participation floor, D2D, ATR>=20, the 297's 5+ tier cell, cap 21, FLOORED, conviction as config, recentfb False, conventions ONE and RR.

## HOW G_d IS DERIVED (mechanical, identical in every segment)
Fixed candidate list Q (frame-only continuous quantities, every one with an existing hi/lo condition in the 249):
  ATR_1M, Volume, Bar_Range, Body_Size, Micro_Hurst, Bars_Since_Flip, ADX_Value, D2D_Persist, D2D_Dynamic_Sensitivity,
  AT_Slope_ST, Slope_EMA_ST, Micro_LogReturn, Efficiency_Ratio, Micro_FailedBreak, D2D_ATR, Micro_GarmanKlass   (16)
In the training prefix P: A-bars of direction d that the 297 traded (A∩297) vs A-bars it did not (A\297). For each q in Q: AUC = P(value on A∩297 > value on A\297).
G_d = the 3 quantities with the largest |AUC - 0.5|, each mapped to its EXISTING condition: q:hi if AUC > 0.5, q:lo if AUC < 0.5 (mechanism-D p80/p20 via dots_thresholds; no new threshold).
If A∩297 has fewer than 20 bars in P, G_d is UNDERIVABLE in that segment and A+B is not scored there.

## CONSTANTS AND DIALS
  j   swept over {1, 2, 3}      THE dial (3 trials per derivation x convention)
  m = 3                         chosen before scoring
  k = 2                         inherited from A, not re-swept
  20                            standing floor
Bookkeeping before this file: 137 engine trials; Part B statistical trials so far 17,464 (B1 36 · B2 50 · B3 4,980 · B4 498 + 11,900) + 64 diagnostic = 17,528.

## SCORING PLAN
  control to the cent first
  C  full-frame derivation of A and G, scored on the full frame: 3 j x 2 conventions = 6 runs
  D  walk-forward on sacred wf.FOLDS: Feb, Mar, Apr, May, Jun(1-25) derived on prior months; Jan UNDERIVABLE; July EXTRA derived on Jan-Jun: 6 windows x 3 j x 2 = 36 runs
  planned engine trials: 1 + 6 + 36 = 43
Report order: loss events -> worst bar -> worst day -> losing weeks -> days -> entry bars -> trades -> WR -> PF -> MARGIN -> net. Survival first; losing week binds. A ratio under 20 events is a count.
Computability: every G_d member is a frame quantity and its threshold is the oracle's; A+B is computable without the book. The DERIVATION of G_d uses the book's traded set as the training label, as A's derivation does; the RULE does not.
