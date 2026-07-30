# BOOK COMPOSITION CHECKLIST — the final engine

Objective: **highest P&L at highest persistence, in balance across four axes.**
Nothing here changes the discovery run. This governs what is composed from its output.

## THE FOUR AXES

    direction   2   LONG / SHORT
    structure   9   see below
    session     8   overnight, europe/pre-mkt, pre-open, NY OPEN, morning,
                    lunch lull, pre-close, post-close   (terrain.session_of)
    regime      2   F12 causal labels, burn-in fitted, forward-only

## THE NINE MARKET STRUCTURES

    1  TC   TREND_CONTINUATION    riding an established trend, pullback entry while D2D holds
    2  MI   MOMENTUM_IGNITION     OBV / KAMA / flow confirming fresh ignition
    3  SE   STRUCTURAL_ENTRY      D2D structural state, value-area, level positioning
    4  BX   BREAKOUT_EXPANSION    range expanding, volatility released
    5  SB   SQUEEZE_BREAKOUT      compression resolving into a directional break
    6  PA   PRICE_ACTION          wick / body / impulse micro-structure at entry
    7  TE   TREND_EXHAUSTION      momentum fading, reversal at the turn
    8  VC   VOLUME_CONFIRMED      participation validates the move
    9  D2D  BREAK-OF-STRUCTURE    the founding adaptive break-of-structure / directional-flip engine

Source: DOT_signal_dictionary.xlsx "Name Key". The dictionary header reads "8/8
structures" — STALE. It counts the eight the book draws from and omits D2D, which the
same sheet names explicitly as the ninth.

## WHAT IS MISSING FROM THE CATALOGUE

- [ ] **C1. No `market_structure` column.** The 39,308 rows carry family, direction and
      signal_def — nothing says which structure a signal belongs to. BOOK-50's labels
      were assigned by hand.
- [ ] **C2. No `session` column** per signal.
- [ ] **C3. No `regime` column** per signal. F12 computed causal labels; they are not
      joined to catalogue rows.

All three are derivable from data already produced. None requires a re-scan.

## THE STRUCTURE CLASSIFIER

- [ ] **C4.** Classify each discovered signal by the VARIABLES in its definition. The
      mapping is recoverable from BOOK-50's own names:
          TC_SlpEMA / TC_SlpAcc / TC_ATSlp   -> Slope_EMA, Slope_Accel, AT_Slope
          MI_KDist / MI_VPIN / MI_OBVM / MI_OFDelta / MI_MomoTra / MI_IBSP
                                              -> KAMA_Dist, Micro_VPIN, OBV_Macd,
                                                 Micro_OrderFlowDelta, Micro_MomoTransfer,
                                                 Micro_IBSP
          SE_*                                -> AT_Score, VAH/VAL, PrevDay/OR/Session sides
          BX_*                                -> ATR, Bar_Range, RangeOsc
          SB_*                                -> Sqz_State, Sqz_Val
          PA_*                                -> wick / body / impulse micro variables
          TE_*                                -> decay / flattening variables
          VC_*                                -> Volume, Volume_Ratio
          D2D                                 -> D2D_* variables
      Derive the full map from the dictionary rather than this sketch.

- [ ] **C5. DECIDE: multi-structure triples.** A triple can carry variables from two
      structures. Primary label, or both? OPERATOR DECISION — do not default.

## THE COMPOSITION RULE

- [ ] **C6.** Rank and select PER DIRECTION separately. Not top-N overall — that is how
      37L/13S happened. No quota, no floor; separate ranking is the mechanism.
- [ ] **C7.** Rank on persistence FIRST, then P&L:
          1  EXPECTED_ROWS_AT_OR_ABOVE_THIS_PF   (low = diamond; prices the search)
          2  folds_plus / min_fold_pf / OOS      (holds across time)
          3  same-bar depth participation        (shows up when others do)
- [ ] **C8.** Then balance: fill each (direction x structure x session x regime) cell
      from its own ranked list.
- [ ] **C9.** Cap the book where the DILUTION CURVE says same-bar 3+ stops being
      selective. Measured: at ~1,840 F0 signals every active bar reaches depth 3+ and
      the tier stops meaning anything. Target is ~72-100 F0 signals for 4x the 505.

## TENSIONS TO DECIDE ON PURPOSE, NOT DISCOVER LATER

- [ ] **T1. Session balance costs P&L.** NY OPEN holds 225 of the largest-decile
      episodes; overnight holds 0. A session-balanced book deliberately takes worse
      episodes overnight to be even. Real trade, not free.
- [ ] **T2. Some structures are thin.** In BOOK-50: SQUEEZE_BREAKOUT 1L/0S,
      TREND_EXHAUSTION 1L/0S, VOLUME_CONFIRMED 0L/1S. Forcing equilibrium means taking
      the best AVAILABLE rather than the good. Decide the floor per cell, or accept
      unevenness where the market does not offer the setup.
- [ ] **T3. Short side is structurally thinner.** F12 measured, on the raw condition
      pool before any selection: depth p50 long 27 vs short 23, p99 long 59 vs short 48.
      Not a funnel artifact this time — a property of the vocabulary against this market.

## GATING TIERS (unchanged, per operator)

    triple+   fires free
    dual      Hurst p90
    solo      Hurst p90 + ticks >= 300

- [ ] **C10.** Item 10 scored every catalogue signal BOTH ungated and gated, with the
      delta. Decide gating PER SIGNAL from that column, not book-wide.

## BEFORE ANY OF THIS

- [ ] **C11.** Run must complete. Read the three acceptance conditions first:
      item 8 divergence on real edge, item 17 ratio on a discovered pool,
      F0 parity proof (ALREADY PASSED, IDENTICAL: True).
- [ ] **C12.** Score the composed book with `score_book.py` before trusting it. A book
      is UNSCORED until that has run and written to book_scored.jsonl.
