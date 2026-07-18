# DOT Master Report — US30 (sealed baseline)

## 1. Ingest attestation
- files: equiDOT_recon171_step7_part1.csv, equiDOT_recon171_step7_part2.csv, equiDOT_recon171_step7_part3.csv, equiDOT_recon171_step7_part4.csv, equiDOT_recon171_step7_part5.csv, equiDOT_recon171_step7_part6.csv, equiDOT_recon171_step7_part7.csv, equiDOT_recon171_step7_part8.csv
- shape: 152,983 rows × 172 cols · range 2026.01.19 15:49 → 2026.06.25 18:18
- path: sealed-baseline (load_sealed_baseline invariants) · invariants: PASS

## 2. Sacred parity (byte-lock)
- `dots_thresholds.py` `518862bf19fb` OK
- `wf.py` `793e6e5f8d9a` OK
- `core.py` `6530e2508b17` OK
- `portfolio_simulation_engine.py` `bb498eb13ce3` OK
- `conviction.py` `27af7acee824` OK

## 3. Component build-up / contenders
| id | contender | net | Δ | WR | PF | daily wd | daily mDD | folds+ | min-PF | OOS PF | OOS net |
|---|---|---|---|---|---|---|---|---|---|---|---|
| C0 | Flat book (1-lot, no conviction/gaps) | $58277 | +58277 | 92.8 | 6.12 | -127.5 | -165.6 | 6/6 | 5.49 | 7.04 | $18893 |
| C1 | + S.20 conviction (Hurst/recentFB longs) | $71377 | +13100 | 92.8 | 6.58 | -153.7 | -191.8 | 6/6 | 5.93 | 7.88 | $24500 |
| C2 | + S.20 gap-singles (Hurst-gap, FB-gap) | $89432 | +18055 | 92.3 | 6.23 | -153.7 | -153.7 | 6/6 | 5.29 | 6.81 | $28447 |
| C3 | + S.21 D2D-conviction (2x both dir) | $90447 | +1015 | 92.3 | 6.29 | -153.7 | -153.7 | 6/6 | 5.34 | 6.9 | $28894 |
| C4 | + S.21 D2D-gap (flat 2-lot) = FULL | $92347 | +1900 | 92.3 | 6.4 | -104.4 | -145.9 | 6/6 | 5.39 | 6.96 | $29190 |
| C5 | sizing variant (conviction-off, gaps-on) | $78243 | +19966 | 92.3 | 5.99 | -122.6 | -140.6 | 6/6 | 5.08 | 6.18 | $23136 |

## 4. Committed-system headline
- book: FROZEN ratified book (book50_signals.csv)
- **net $92347 | 2698 tr | WR 92.3% | PF 6.4 | daily wd -104.4 | daily mDD -145.9 | 6/6 folds min-PF 5.39 | OOS PF 6.96 | OOS net $29190**
- US30 baseline canary: $92,347 / 2,698 tr — engine intact

## 5. Per-family coverage
- families: F0 (committed) + F1 (2 pairs committed) + F2–F9/F11 (exploratory) + F12 (concurrence diagnostic) + F13 (documented negative). **F10 folded into F0** (concurrence null) — complete, not gapped.

## 6. Stale-artifact note
- signal_full_records / signal_per_day_pnl: regenerated fresh this run (S6) — stale 746102aae415 / 0910f360a628 NOT inherited

