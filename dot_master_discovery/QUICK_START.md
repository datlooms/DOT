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

## Scoring a book you assembled yourself

`python score_book.py --book <your_book.csv> --data <frame> --out <dir>`

The catalogues state that any book assembled from them is UNSCORED until this has run.
It exits non-zero on a hard-constraint breach and appends to `book_scored.jsonl`.
