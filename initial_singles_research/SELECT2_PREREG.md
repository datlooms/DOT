# PRE-REGISTRATION — SELECT §9.3 UNION ARM AND THE TWO-MOMENT CHECK (quant seat, 2026-08-22)
Written before the union arm is scored. Base HEAD 1c8f29d80cd4 · frame 46586cbb1671 · oracle 518862bf19fb imported · engine unchanged.

## THE ARM (select_two_objectives.py)
  LOSS-DAY DECORRELATION  §2.1 verbatim: TRUE-SOLO matrix, every VALID catalogue signal (1,818) run ALONE through run_portfolio
                          (FLOORED, floor {1:1,-1:1}, cap 21, ATR>=20, no tier gates, conviction as config, recentfb False);
                          P&L keyed on EXIT date; net>0 pool per direction; seed = argmax(net - 50*solo_loss_days), ties by index;
                          greedy add = fewest loss-days already covered (CUMULATIVE boolean), ties by higher net, then index;
                          fixed counts K = 70 LONG / 50 SHORT (the recorded OPTION-B per-direction counts). No RNG anywhere.
  CHANCE-PRICING          catalogue EXPECTED_ROWS_AT_OR_ABOVE_THIS_PF < 1.0 (expected rows, not a p-value).
                          DIRECTION CORRECTION NOT APPLIED: catalogue.py persists only family percentiles of the 4,652 nulls; the
                          per-direction null PF vector does not exist on disk, so E_dir cannot be computed without re-drawing the null.
                          The module accepts --null-by-direction <json> and applies the correction when that vector is supplied.
  UNION                   set union of the two, deduplicated on (signal_def, direction); source tag kept per row.
  SCORING OBJECT          L3/S3, cap 21, the 297's tier gates and ATR>=20 — stated as the default object, NOT derived for this arm
                          (relocation-only gate derivation per arm is outside this turn and is said so). IN-SAMPLE: the catalogue that
                          feeds both objectives was built on the full frame.

## THE TWO-MOMENT CHECK (two_moment_check.py, definitions v1 frozen in the file header)
  A count and a coverage ratio against the incumbent's entry bars per moment per direction. Never an objective.
  Run on every arm whose membership file exists at HEAD: WHOLE DOT L3/S3, WHOLE DOT L7/S4, BOOK-50, 297 minus the fourteen, the union arm.
  The four §4 arms without a membership file at HEAD (QUANT A30+B, MANAGER A+B, QUANT B+D, MANAGER B K=50) are NOT reconstructed; files are requested.

## TRIALS PLANNED
  engine: 4 arms + union arm = 5 scored runs (control is arm 1, reproduced to the cent); the solo matrix = 1,818 single-signal runs, counted separately as the objective's input, not as selection trials.
  statistical: none new.
Bookkeeping before this file: engine 193, statistical 18,841.
