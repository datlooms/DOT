"""Runs at interpreter startup in EVERY process, including spawned workers.

The warning filter lives HERE and not in master.py for the same reason the frame
binding and F13's scanner paths do: a spawned worker re-imports everything fresh
and never sees a parent-side setting. master.py could silence its own console and
every one of the 14 workers would still flood the pipe.

FILTERED ON THE MESSAGE, NOT THE CATEGORY. pandas has moved PerformanceWarning
between namespaces across versions, so `from pandas.errors import
PerformanceWarning` can raise on a different pandas and take the whole run with
it. A message match cannot fail that way.

EXACTLY ONE MESSAGE IS SILENCED. DtypeWarning and FutureWarning are information
the operator needs; a blanket filter would hide the next real problem. The
fragmentation warning is true but expected: S5D assigns a mask column once per
candidate on a 177,251 x 172 frame, ~39,000 times, and on a healthy run that
prints thousands of identical lines in red through PowerShell, which is
indistinguishable from failure.
"""

import warnings

warnings.filterwarnings('ignore', message='DataFrame is highly fragmented')

try:
    import dot_frame_binding
except Exception:
    dot_frame_binding = None
# EMIT-ALL TRANSPORT. Deliberately OUTSIDE the is_configured() guard: DOT_EMIT_ALL is
# independent of the frame binding, and a worker holding the flag but not the frame must
# still abort loudly rather than filter silently.
if dot_frame_binding is not None and dot_frame_binding.emit_all_requested():
    dot_frame_binding.install_emit_all()
if dot_frame_binding is not None:
    try:
        dot_frame_binding.install_f0_output_dir()
    except Exception:
        pass
if dot_frame_binding is not None and dot_frame_binding.is_configured():
    dot_frame_binding.install_if_configured()
    try:
        dot_frame_binding.install_scanner_paths()
    except Exception as _exc:
        print(f'  SCANNER PATH BINDING FAILED: {type(_exc).__name__}: {str(_exc)[:90]} - F13 '
              f'will write to its hardcoded legacy directory and its shards will not be found.',
              flush=True)
    try:
        _ap, _fl = dot_frame_binding.install_smoke_caps()
        for _f in _fl:
            print(f'  SMOKE CAP FAILED: {_f}', flush=True)
    except Exception as _exc:
        print(f'  SMOKE CAP INSTALLER RAISED: {type(_exc).__name__}: {str(_exc)[:90]}',
              flush=True)
