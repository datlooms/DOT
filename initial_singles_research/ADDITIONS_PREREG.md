# PRE-REGISTRATION — MEASUREMENT 4, ADDITIONS TO THE 297 (quant seat, 2026-08-22)
Written before any addition is scored. Base HEAD db3687a95957 · frame 46586cbb1671 · oracle 518862bf19fb imported · engine unchanged.
ADDITIONS ONLY. No row of the 297 is removed. Every scored set = the 297 + additions, scored with the 297's own object: FLOORED, cap 21, floor {1:3,-1:3}, tier gates as whole_dot_config.json, ATR>=20, conviction as config, recentfb False.

## UNIVERSE
catalogue_F0.csv rows with verdict VALID (1,818) whose (signal_def, direction) is not in engine/whole_dot_signals.csv. Qualifying masks built with AE.build_signal_masks (entry_ok, D2D agreement, the oracle's thresholds).

## TWO CANDIDATE CLASSES (per direction)
  MISSING-TURN: the candidate contains at least one condition that is in the direction's top-8 hour-matched lift list (q3_discriminator_lift.csv, >=20 traded bars) AND absent from the 297's vocabulary on that side.
    LONG: AT_Score_ST:lo, D2D_Up_Count:hi, D2D_Persist:lo, KAMA_Slope:lo     SHORT: EMA_Oscillator:hi, AT_Score_ST:hi, KAMA_Dist_ATR:hi, KAMA_Slope:hi
  LOCATION: the candidate contains at least one condition of LOC_d as derived in measurement 1 (m3_churn.json LOC_full).
    LONG: VAH_Dist_ATR:lo, VAL_Dist_ATR:lo, Dist_To_PoC_ATR:lo, Session_Low_Dist_ATR:lo, PrevDay_Low_Side:==-1, MultiDay_Position:lo
    SHORT: VAL_Side:==0, VAH_Side:==0, PoC_Side:==0, PrevDay_High_Side:==1, Session_High_Dist_ATR:lo

## SELECTION (mechanical)
For every candidate: qualifying bars split into (i) already a 297 entry bar, (ii) an A\297 bar (A = R_d(k=2) full-frame), (iii) NEW — neither. "Reach into 297\A-shaped bars" is measured as (iii) restricted to bars where the location count >= 1 and the turn count <= 1 (the 297\A profile from measurement 1).
Per class and direction, take the top 3 by NEW bars with that profile -> up to 12 candidates. Ties broken by catalogue agg_pf descending.

## SCORING PLAN (engine)
  control to the cent (1)
  each candidate alone added to the 297 (up to 12)
  cumulative: 297 + MISSING-TURN class (1), 297 + LOCATION class (1), 297 + all (1)
  planned: up to 16 engine runs. All IN-SAMPLE (selected and scored on the full frame); labelled as such.
Report per row: loss events -> worst bar -> worst day -> losing weeks of 26 -> days of 132 -> entry bars -> trades -> WR -> PF -> MARGIN -> net; delta vs the 297 control; whether ZERO losing weeks survives.
Bookkeeping before this file: engine 180 (+1 control this task = 181); statistical 17,752 + 1,089 (measurements 1-3) = 18,841.
