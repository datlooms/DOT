# DOT — QUICK START

Four steps. **Step 2 is not optional.**

1. Put the 10 data parts in `dot_master_discovery\data\`
2. **`python master.py --data data --workers 14 --out discovery\smoke --smoke`**  <- ~2 min
3. `python master.py --data data --workers 14 --out discovery\full`
4. Upload everything in `discovery\full\data_for_analysis\`

---

## STEP 2 — THE SMOKE RUN. RUN IT EVERY TIME, BEFORE EVERY FULL RUN.

**It executes EVERY stage S0 through S10 at reduced scale in about two minutes.** Same functions,
same stage order, same pool spawn, same workers — only the WORK is reduced. Nothing is skipped,
because a stage that does not execute is a stage that can still crash at hour nine.

    python master.py --data data --workers 14 --out discovery\smoke --smoke

**IF IT REACHES `MASTER COMPLETE` WITH EXIT 0, THE FULL RUN WILL COMPLETE. IF IT DOES NOT, YOU HAVE
LOST TWO MINUTES INSTEAD OF ELEVEN HOURS.**

Note the different `--out`. The smoke tree cannot contaminate the real one and needs no cleanup.

### Why this exists

Three full runs were lost to defects a two-minute smoke run would have caught in seconds:

    a NameError inside a pool worker    raised at F12 stage 4, ~40 minutes in
    a blank string hitting astype       raised at F12 stage 8's LAST line, after 8,800 configs completed
    a stale sacred-registry sha         aborted at second zero, but only after a fresh clone

**None was findable by importing, compiling, or testing one function.** A NameError inside a pool
worker is invisible until that worker runs, and the aggregation crash was 780 lines away from the
change that caused it. **The only thing that finds these is executing every stage.**

### What to look for

* **five OKs in the SACRED REGISTRY banner** — a DRIFT line means a lock was not updated, and it stops there
* every stage banner S0 -> S10 appearing in order
* `MASTER COMPLETE` and exit 0

If a stage aborts, the message names the file and the missing column or symbol. Fix that, re-run the
smoke, and only then start the full run.

---

## What step 3 does

Runs every stage S0 -> S10. Stages that already completed for this dataset resume
from their checkpoints, so a re-run after a crash costs only the work that was lost.
Each gate verifies **every** artifact the stage writes, not just one, so a stage whose
output is missing re-runs instead of skipping.

The final stage, **S10**, collects every `.csv` `.md` `.txt` `.jsonl` under
`discovery\full\` and `data\` into one flat folder, `discovery\full\data_for_analysis\`,
and splits anything over 26 MB so every file clears the 30 MB upload ceiling.

* CSV parts **repeat the header**, so each part opens standalone.
* Splits land on line boundaries — never mid-row.
* The oversized original is removed, so you never upload both.
* The folder is cleared on each run; a second run cannot leave stale parts behind.
* S10 **copies only**. It never modifies anything in `discovery\full\`.

S10 walks the tree and excludes by pattern, so any artifact added to the pipeline later
is collected automatically without editing anything.

## Running one stage

`python master.py --data data --workers 14 --out discovery\full --stage S5C`

Valid stages: S0 S1 S2 S2B S3 S3B S4 S5 S5D S6 S5B S5C S7 S8 S8B S9 S10

`--stage S10` re-collects at any time without re-running the pipeline.

## Scoring an existing book file — 16 SECONDS

**This is the fast path. It replays a committed book and prints its full scorecard.**

    python master.py --data data --workers 14 --out discovery\full --stage S8 --book book50_signals.csv

`--book` IS THE SWITCH. Without it S8 runs in DISCOVER-FRESH mode, has nothing to score, and
exits in 0.01s printing an explanation. `frozen = book_file is not None` at master.py L989 is
the whole mechanism — no `--book`, no scoring.

It prints book rows, trades, win rate, profit factor, net P&L, worst day, max drawdown,
folds positive with min-fold PF, and the OOS PF and net on the final third.

**BOOK-50 REFERENCE — RUN THIS FIRST EVERY SESSION:**

    book rows 50 | trades 3,101 | WR 90.6% | PF 4.81 | net $97,675
    worst day -$565.3 | max DD -$999.9 | folds 6/6 min-fold PF 5.05
    OOS (2026.05.25 -> 2026.07.21) PF 2.95 | net $22,688

**THE CANARY ONLY FIRES FOR `book50_signals.csv`** (master.py L1027, keyed on the exact
basename). It asserts `$92,347 / 2,698 tr — engine intact`. **Any other filename skips that
check entirely, so score BOOK-50 first to confirm the engine before scoring anything new.**

### Scoring a different book

Same command, different `--book`. The file lives in `engine\` and must carry these three
columns, in this order:

    trigger,direction,signal_def
    F0,LONG,KAMA_Dist:lo + Micro_WickImbalance:hi + OR_High_Side:==-1

**S8 SCORES AT THE ENGINE'S OWN SETTINGS — cap 6, 1.0 lot, no depth floor.** It does not read
a depth floor, a per-tier gate stack, or an admission rule. A book designed around those runs
here without them, so the figures are a like-for-like comparison against BOOK-50 and NOT the
book's own configured performance.

## THE TWO BOOK COMMANDS

**BOOK-50 — the incumbent and the engine canary. Run this first every session.**

    python master.py --data data --workers 14 --out discovery\full --stage S8 --book book50_signals.csv

    3,101 trades | WR 90.6% | PF 4.81 | net $97,675
    folds 6/6 min-fold PF 5.05 | OOS PF 2.95 net $22,688

**THE WHOLE DOT — the adopted system, 297 F0 triples.**

    python master.py --data data --workers 14 --out discovery\full --stage S8 --book whole_dot_signals.csv

    5,776 trades | WR 96.1% | PF 14.53 | net $284,974
    42 LOSS EVENTS on 35 days | worst day -$346.60 | worst bar -$1,224.00
    0 losing weeks of 26 | folds 6/6 min-fold PF 11.00
    OOS (final third) PF 9.78 net $73,851

Both at 1.0 lot. `whole_dot_config.json` must sit in the same directory as
`whole_dot_signals.csv` — routing is on the config, not the book name, and a missing
config aborts rather than silently scoring the sacred path.

## Scoring a book you assembled yourself

`python score_book.py --book <your_book.csv> --data <frame> --out <dir>`

The catalogues state that any book assembled from them is UNSCORED until this has run.
It exits non-zero on a hard-constraint breach and appends to `book_scored.jsonl`.
