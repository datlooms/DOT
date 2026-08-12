import re, os, hashlib, glob, sys
ACT={}
for p in glob.glob('dot_master_discovery/**/*.py', recursive=True):
    ACT[os.path.basename(p)]=hashlib.sha256(open(p,'rb').read()).hexdigest()[:12]
TOK=re.compile(r"([A-Za-z_0-9]+\.py)|\b([0-9a-f]{12})\b")
docs=sorted(set(glob.glob('**/*.txt',recursive=True)+glob.glob('**/*.md',recursive=True)))
APPLY = '--apply' in sys.argv
fixed=0; drifts=[]
for d in docs:
    try: txt=open(d,encoding='utf-8',errors='replace').read()
    except OSError: continue
    lines=txt.split('\n'); changed=False
    for i,ln in enumerate(lines):
        toks=[(m.start(),m.group(1),m.group(2)) for m in TOK.finditer(ln)]
        # positional: each filename claims the NEXT sha, if no filename intervenes
        for j,(pos,fn,sh) in enumerate(toks):
            if not fn or fn not in ACT: continue
            nxt=None
            for pos2,fn2,sh2 in toks[j+1:]:
                if fn2: break
                if sh2: nxt=(pos2,sh2); break
            if not nxt: continue
            declared=nxt[1]; actual=ACT[fn]
            if declared==actual: continue
            drifts.append((d,i+1,fn,declared,actual))
            if APPLY:
                lines[i]=lines[i][:nxt[0]]+actual+lines[i][nxt[0]+12:]
                changed=True; fixed+=1
                toks=[(m.start(),m.group(1),m.group(2)) for m in TOK.finditer(lines[i])]
    if APPLY and changed:
        open(d,'w',encoding='utf-8').write('\n'.join(lines))
print(f"  {'document':52}{'ln':>5}  {'file':32}{'declared':14}{'actual':14}")
for d,i,fn,dec,act in drifts:
    print(f"  {d:52}{i:>5}  {fn:32}{dec:14}{act:14}DRIFT")
_with = len({d[0] for d in drifts}) if drifts else 0
_scanned = len(docs)
_pairs = 0
for _d in docs:
    try:
        _t = open(_d, encoding='utf-8', errors='replace').read()
    except OSError:
        continue
    if re.search(r"[0-9a-f]{12}", _t) and re.search(r"[A-Za-z_0-9]+\.py", _t):
        _pairs += 1
print(f"  COVERAGE: {_scanned} documents scanned, {_pairs} carry a sha/filename pair and were "
      f"EXAMINED, {_scanned - _pairs} carry none. An unexamined document is not a passing one.")
_unread = []
for _d in docs:
    try:
        open(_d, encoding='utf-8', errors='replace').read()
    except OSError as _e:
        _unread.append(f'{_d} ({type(_e).__name__})')
if _unread:
    print(f"  *** {len(_unread)} DOCUMENT(S) UNREADABLE: {_unread}. An unexamined document is "
          f"not a passing one. ***")
print(f"  TRUE DRIFTS: {len(drifts)}" + (f" | REWRITTEN: {fixed}" if APPLY else ""))
import sys as _sys
_sys.exit(1 if (drifts and not APPLY) or _unread else 0)
