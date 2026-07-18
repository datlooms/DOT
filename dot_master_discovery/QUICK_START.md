# DOT MASTER DISCOVERY — QUICK START (2 brain cells, 6am edition)

Two commands, in order: `python rebuild.py` (prep the data), then `python master.py` (run it). That's the whole thing.

--------------------------------------------------------
## STEP 1 — get the data out of MT4
Open MT4. Load the chart/asset you want (e.g. **BTCUSD**).
- **NEW asset?** Set the EA lookback to **65K** once, so it builds that asset's KAMA `.bin` seed. Let it run and export.
- Already done before? Just let it export.

--------------------------------------------------------
## STEP 2 — find the file the EA wrote
It lands here (one file, named after the asset):
```
C:\Users\d\AppData\Roaming\MetaQuotes\Terminal\9303753997FFC7FE093FBA504590C18A\MQL4\Files\<ASSET>_AUTO_EXPORT.csv
```
Example: `BTCUSD_AUTO_EXPORT.csv`

--------------------------------------------------------
## STEP 3 — copy that CSV into the pack's raw\ folder
```
C:\Users\d\Documents\GitHub\DOT\dot_master_discovery\raw\
```
Just drop it in there. Any asset name is fine.

--------------------------------------------------------
## STEP 4 — prep the data
Open a terminal in the pack root (`dot_master_discovery`), run:
```
python rebuild.py
```
It corrects the data, splits it, and drops the parts into `data\`.
**Look for this line:**
```
invariants : PASS
```
If it says PASS -> good, the parts are in `data\`. If it says FAIL or ABORT -> the export is bad; re-export from MT4.

--------------------------------------------------------
## STEP 5 — run the system
Pick ONE:
```
python master.py
```
-> **Discover fresh** on the new data (1-2 DAYS, leave it running). If the PC crashes/reboots, run the SAME command again -- it resumes, does NOT start over.
```
python master.py --book engine\book50_signals.csv
```
-> **Score the ratified book** on this data instead (fast).

--------------------------------------------------------
## STEP 6 — read the answers
Results land in `discovery\`:
- `discovery\master_report.md`  -- the summary
- `discovery\committed\`        -- the committed-system score
- `discovery\contenders\`       -- the mechanism head-to-head

On the ORIGINAL US30 baseline you'll see a quiet line: `US30 baseline canary: $92,347 / 2,698 tr -- engine intact`. On any other data you just get the numbers -- that's normal (different data, different numbers).

--------------------------------------------------------
## THE ONLY DECISION YOU EVER MAKE
- `python master.py`                                  -> DISCOVER on new data (slow)
- `python master.py --book engine\book50_signals.csv` -> CHECK/score the known book (fast)
That's it.

--------------------------------------------------------
## 2 THINGS THAT CAN GO WRONG

1. Top of the output says **DRIFT** instead of a row of **OK**s
   -> STOP. A locked file got changed/corrupted. Don't use the result. (It refuses to run anyway.) Tell Ticky.
2. It says `python` is not recognised
   -> try `python3` instead of `python`. Same command otherwise.

--------------------------------------------------------
## HANDY (only if you care)
- `python rebuild.py --in D:\path\to\SOME_EXPORT.csv`  = prep a specific file (skip the raw\ folder)
- `python master.py --stage S8`   = skip everything, just re-score (fast check)
- `python master.py --data D:\somewhere`  = use a different data folder
- `--chunk-mb 9` = auto-cuts big files into <=9MB pieces so you can upload them without splitting by hand (already the default; both scripts use it)

Full version: `master_guide.md` (same folder).

That's it, Animal. Two commands. One decision. Sleep.
