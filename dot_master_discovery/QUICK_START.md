# DOT — QUICK START

Three steps. Nothing else is required.

1. Put the 10 data parts in `dot_master_discovery\data\`
2. `python master.py --data data --workers 14 --out discovery\full`
3. Upload everything in `discovery\full\data_for_analysis\`

---

## What step 2 does

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
