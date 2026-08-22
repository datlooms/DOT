import os

_ENV_FRAME = 'DOT_FRAME_PATH'
_ENV_SHA = 'DOT_INPUT_SHA'
_ENV_FP = 'DOT_FRAME_FINGERPRINT'
_ENV_EMITALL = 'DOT_EMIT_ALL'
_ENV_F0OUT = 'DOT_F0_OUTPUT_DIR'
_STATE = {}


def configure_environment(frame_path, input_sha, fingerprint):
    os.environ[_ENV_FRAME] = str(frame_path)
    os.environ[_ENV_SHA] = str(input_sha)
    os.environ[_ENV_FP] = '|'.join(str(x) for x in fingerprint)
    here = os.path.dirname(os.path.abspath(__file__))
    parts = [p for p in os.environ.get('PYTHONPATH', '').split(os.pathsep) if p]
    for sub in ('orchestrator', 'scanners', 'engine', ''):
        d = os.path.join(here, sub) if sub else here
        if d not in parts:
            parts.insert(0, d)
    os.environ['PYTHONPATH'] = os.pathsep.join(parts)
    return dict(frame=frame_path, sha=input_sha, fingerprint=fingerprint)


def configure_emit_all(on):
    """Carry EMIT_ALL ACROSS THE SPAWN BOUNDARY, by the mechanism that carries the frame.

    THE DEFECT: discovery_orchestrator L1306 `f0m.EMIT_ALL = True` was the ONLY assignment
    in the package and it runs in the PARENT. Every ProcessPoolExecutor worker re-imports
    scanners/triple_convergence_and_d2ddir.py fresh at module-scope EMIT_ALL = False
    (scanner L31), and _min_trades/_min_pf/_overlap_threshold all branch on it. SO ALL 512
    CHUNKS OF THE 2h36m RUN OF 2026-08-21 EXECUTED AT MIN_TRADES 30 AND MIN_PF 2.0. The
    only lift that took effect was the dedup, which runs in the parent at collation - which
    is exactly why the three orphans that reappeared carry PF 4.09 / 10.2 / 3.17, all above
    the 2.0 floor, and the eleven at proxy PF 1.35-1.96 did not.

    AN ENVIRONMENT VARIABLE, NOT A PAYLOAD FIELD. A payload is opt-in per call site, and
    opt-in transport is precisely what produced this defect: a new entry point that forgets
    the field filters silently and emits an incomplete catalogue with no error.
    """
    if on:
        os.environ[_ENV_EMITALL] = '1'
    else:
        os.environ.pop(_ENV_EMITALL, None)


def emit_all_requested():
    return os.environ.get(_ENV_EMITALL) == '1'


def configure_f0_output_dir(path):
    """SECOND INSTANCE OF THE SAME CLASS, found by the sweep.

    orchestrate() sets f0m.OUTPUT_DIR = RESULTS_DIR in the PARENT, and the scanner reads it
    through _output_dir() inside run_search - WHICH RUNS IN THE WORKER. So under --emit-all
    every worker wrote raw_survivors.csv to the module default "dots_results" instead of
    the run's own results directory, which is why that file could not be found where it was
    expected.
    """
    if path:
        os.environ[_ENV_F0OUT] = str(path)
    else:
        os.environ.pop(_ENV_F0OUT, None)


def install_emit_all():
    """Re-establish EMIT_ALL at interpreter startup, inside every spawned worker.

    ASSERTS RATHER THAN TRUSTS, matching install_if_configured's shape at L38-41: a worker
    that reaches this with the flag set but the scanner unreachable must ABORT, because a
    worker running under --emit-all while silently filtering is the failure being fixed.
    """
    if not emit_all_requested():
        return None
    try:
        import triple_convergence_and_d2ddir as _f0
    except Exception as exc:
        raise SystemExit(
            f'ABORT [emit-all transport] {_ENV_EMITALL} is set but the F0 scanner could not '
            f'be imported in this process: {type(exc).__name__}: {str(exc)[:90]}. A worker '
            f'running under --emit-all with the flag unapplied would filter at '
            f'MIN_TRADES/MIN_PF and emit a SILENTLY INCOMPLETE catalogue.')
    _f0.EMIT_ALL = True
    return _f0


def install_f0_output_dir():
    d = os.environ.get(_ENV_F0OUT)
    if not d:
        return None
    try:
        import triple_convergence_and_d2ddir as _f0
    except Exception:
        return None
    _f0.OUTPUT_DIR = d
    return d


def assert_emit_all_applied(mod):
    """THE WORKER ASSERTS. Called by the scanner itself before it filters anything."""
    if emit_all_requested() and not getattr(mod, 'EMIT_ALL', False):
        raise SystemExit(
            f'ABORT [emit-all transport] {_ENV_EMITALL} is set in the environment but this '
            f'process has EMIT_ALL=False. The flag did not survive spawn, and this worker '
            f'would filter at MIN_TRADES/MIN_PF while the run claims to be lifting them - '
            f'the exact shape that voided the 2h36m run of 2026-08-21.')


def is_configured():
    return bool(os.environ.get(_ENV_FRAME))


def fingerprint_of(df):
    return (len(df), str(df['Time'].values[0]), str(df['Time'].values[-1]))


def install(df=None):
    import portfolio_simulation_engine as engine
    if _STATE.get('installed') and df is None:
        return _STATE['frame']
    expected = os.environ.get(_ENV_FP, '')
    sha = os.environ.get(_ENV_SHA, '')
    if df is None:
        path = os.environ.get(_ENV_FRAME, '')
        if not path:
            raise SystemExit(
                'ABORT — a worker process reached the frame binding with no DOT_FRAME_PATH set. '
                'It must never fall through to load_sealed_baseline, which hardcodes '
                'equiDOT_recon171_step7_* and would load a different dataset.')
        if not os.path.exists(path):
            raise SystemExit(f'ABORT — worker frame cache missing at {path}. Refusing to fall back '
                             f'to the hardcoded parts.')
        import pandas as pd
        df = pd.read_csv(path)
    got = fingerprint_of(df)
    if expected and '|'.join(str(x) for x in got) != expected:
        raise SystemExit(f'ABORT — frame fingerprint mismatch in pid {os.getpid()}: expected '
                         f'{expected}, got {"|".join(str(x) for x in got)}. The worker is holding a '
                         f'different dataset from the one S0 validated for input_sha {sha}.')

    def _bound_loader(*_a, **_k):
        return df

    engine.load_sealed_baseline = _bound_loader
    _STATE['installed'] = True
    _STATE['frame'] = df
    return df


def install_if_configured():
    if not is_configured():
        return False
    try:
        install()
        return True
    except SystemExit:
        raise
    except Exception as exc:
        raise SystemExit(f'ABORT — frame binding failed in pid {os.getpid()}: '
                         f'{type(exc).__name__}: {exc}')


_SCANNER_PATH_MODULES = (
    ('single_variable_extremes', {
        'RESULTS_DIR': '',
        'OUT_CSV': 'results_F13_single_variable_extremes.csv',
        'SHARD_DIR': '_f13_shards',
    }),
)


def install_scanner_paths():
    """THIRD INSTANCE of the parent-only-global class. Closed at startup, not at the call site.

    single_variable_extremes hardcodes RESULTS_DIR/OUT_CSV/SHARD_DIR at import
    (L90-92) against the LEGACY discovery_results/ directory. master.py reassigns
    all three, but that is a PARENT-SIDE attribute write and F13 spawns its own
    Pool: every worker re-imports the module fresh, gets the L90-92 defaults, and
    process_shard writes shards to a directory that does not exist inside the run
    tree.

    WHY NOT THE POOL INITIALIZER. Under spawn, `initializer=_init` is pickled by
    reference and the worker looks _init up on the freshly imported module, so
    patching _init in the parent does not survive either. The initializer is not
    reachable as a transport without editing the scanner, and SCANNERS ARE NOT
    EDITABLE. This hook runs at INTERPRETER STARTUP in every spawned process,
    before any worker code, driven by DOT_RESULTS_DIR which master already
    exports - the same mechanism that closed instance one.

    DECIDED - REVERSIBLE. The alternative was to create the legacy directory so
    the hardcoded path resolves, which is simpler but writes shards OUTSIDE
    --out and breaks item 1's guarantee that every read and write resolves inside
    the run tree. Reverting means deleting this function and creating that
    directory instead.
    """
    rd = os.environ.get('DOT_RESULTS_DIR')
    if not rd:
        return []
    done = []
    _missed = []
    for modname, fields in _SCANNER_PATH_MODULES:
        try:
            mod = __import__(modname)
        except Exception as exc:
            # A module absent and a rebind that failed were indistinguishable here too.
            _missed.append(f'{modname}: {type(exc).__name__}: {str(exc)[:70]}')
            continue
        for attr, leaf in fields.items():
            if not hasattr(mod, attr):
                continue
            setattr(mod, attr, rd if not leaf else os.path.join(rd, leaf))
        done.append(modname)
    return done


SMOKE_ENV = 'DOT_SMOKE_CAP'


def _cap_align_pool(k, applied, failed):
    """Cap F12's label lists WITHOUT emptying the secondary view.

    A HEAD-SLICE OF A SORTED LIST IS THE MOST BIASED POSSIBLE CHOICE, and it
    composed with a downstream filter to an empty collection:

        L1352  long_lbls, short_lbls = align_pool(pool)      <- cap: first 6 sorted
        L1353  if secondary:
        L1354      keep = survivor_conditions()
        L1355/6    long/short_lbls = [l for l in ... if l in keep]   <- EMPTY
        L1358  dl, ds, da = depth_arrays(...)  -> np.vstack([]) raises

    None of the first 6 sorted short labels is an F0 survivor, so the secondary
    view emptied and vstack raised. At the full 249 labels the intersection is
    never empty, which is why this is SMOKE-ONLY - and why two earlier smoke runs
    never saw it: the F12 caps were silently failing then, and the reporting fix
    is what exposed it.

    THE CAP'S PURPOSE IS TO REDUCE COUNT, NOT TO BIAS WHICH LABELS. So survivors
    are taken first and the remainder fills from the rest, which guarantees the
    intersection is non-empty whenever the direction has any survivor at all.
    Real-run behaviour is untouched: this wrapper only exists when DOT_SMOKE_CAP
    is set.
    """
    m = None
    try:
        m = __import__('concurrence_profiler')
    except Exception as exc:
        failed.append(f'concurrence_profiler: IMPORT FAILED {type(exc).__name__}: '
                      f'{str(exc)[:90]}')
        return
    if not hasattr(m, 'align_pool'):
        failed.append('concurrence_profiler.align_pool: ATTRIBUTE ABSENT')
        return
    if getattr(m, '_SMOKE_align_pool', False):
        applied.append('concurrence_profiler.align_pool -> already wrapped')
        return
    try:
        orig = m.align_pool

        def _survivors_first(seq, keep, n):
            surv = [x for x in seq if x in keep]
            rest = [x for x in seq if x not in keep]
            out = surv[:n]
            if len(out) < n:
                out += rest[:n - len(out)]
            return [x for x in seq if x in set(out)]

        def _capped(pool, _o=orig, _k=k, _m=m):
            lo, sh = _o(pool)
            try:
                keep = _m.survivor_conditions()
            except Exception:
                keep = set()
            if not keep:
                return list(lo)[:_k], list(sh)[:_k]
            return _survivors_first(list(lo), keep, _k), _survivors_first(list(sh), keep, _k)

        m.align_pool = _capped
        m._SMOKE_align_pool = True
        applied.append(f'concurrence_profiler.align_pool -> first {k} per direction, '
                       f'SURVIVORS FIRST so the secondary view cannot empty')
    except Exception as exc:
        failed.append(f'concurrence_profiler.align_pool: {type(exc).__name__}: {str(exc)[:70]}')


SMOKE_NULL_ARM = {'target': 8, 'floor': 4, 'cap': 200, 'gen_batch': 24}


def _all_module_objects(basename):
    """EVERY module object this file is registered as. DUAL MODULE IDENTITY IS REAL.

        import wf_selection        -> id 140449280990864
        import engine.wf_selection -> id 140449281207536   *** DIFFERENT OBJECT ***

    setattr on one succeeds and the pipeline runs from the other, so the cap
    REPORTED SUCCESS WHILE DOING NOTHING - worse than failing loudly. Resolve
    through sys.modules under every registered name and set on all of them.
    """
    import sys as _s
    out = []
    for nm, mod in list(_s.modules.items()):
        if mod is None:
            continue
        if nm == basename or nm.endswith('.' + basename):
            if mod not in out:
                out.append(mod)
    if not out:
        try:
            out.append(__import__(basename))
        except Exception:
            pass
    return out


def _cap_null_arm(applied, failed):
    """Cap S5C's random-triple null arm. THE VALUES ARE DEFAULT ARGUMENTS.

        def score_null_arm(..., target=NULL_TARGET_QUALIFIERS, floor=NULL_FLOOR_QUALIFIERS,
                           cap=NULL_TRIPLES_CAP, gen_batch=NULL_GEN_BATCH, ...)

    Defaults evaluate when the enclosing def EXECUTES - at import - so no later
    setattr on the module can reach them. That is why the constants read as capped
    and the run still scored 150-signal batches. The function is therefore WRAPPED
    and the values injected at CALL time, which is the only thing that binds.

    Set on every module object so both identities agree, then READ BACK from each.
    """
    mods = _all_module_objects('wf_selection')
    if not mods:
        failed.append('wf_selection: NOT RESOLVABLE under any name in sys.modules')
        return
    ok, names = [], []
    for m in mods:
        nm = getattr(m, '__name__', '?')
        names.append(nm)
        for a_, v_ in (('NULL_TARGET_QUALIFIERS', SMOKE_NULL_ARM['target']),
                       ('NULL_FLOOR_QUALIFIERS', SMOKE_NULL_ARM['floor']),
                       ('NULL_TRIPLES_CAP', SMOKE_NULL_ARM['cap']),
                       ('NULL_GEN_BATCH', SMOKE_NULL_ARM['gen_batch'])):
            if not hasattr(m, a_):
                failed.append(f'{nm}.{a_}: ATTRIBUTE ABSENT')
                continue
            setattr(m, a_, v_)
            if getattr(m, a_) != v_:
                failed.append(f'{nm}.{a_}: READ-BACK MISMATCH, still {getattr(m, a_)}')
        if not hasattr(m, 'score_null_arm'):
            failed.append(f'{nm}.score_null_arm: ATTRIBUTE ABSENT')
            continue
        if getattr(m, '_SMOKE_null_arm', False):
            ok.append(nm)
            continue
        orig = m.score_null_arm

        def _wrapped(*a_, _o=orig, **kw_):
            kw_.setdefault('target', SMOKE_NULL_ARM['target'])
            kw_.setdefault('floor', SMOKE_NULL_ARM['floor'])
            kw_.setdefault('cap', SMOKE_NULL_ARM['cap'])
            kw_.setdefault('gen_batch', SMOKE_NULL_ARM['gen_batch'])
            return _o(*a_, **kw_)

        m.score_null_arm = _wrapped
        m._SMOKE_null_arm = True
        ok.append(nm)
    if ok:
        applied.append(f'wf_selection null arm -> target={SMOKE_NULL_ARM["target"]} '
                       f'floor={SMOKE_NULL_ARM["floor"]} cap={SMOKE_NULL_ARM["cap"]} '
                       f'gen_batch={SMOKE_NULL_ARM["gen_batch"]}, score_null_arm WRAPPED '
                       f'(defaults captured at import) on {len(ok)} module identity/ies: {names}')


EXPECTED_SMOKE_CAPS = 11


def install_smoke_caps():
    """SIXTH use of the startup-hook transport. EVERY FAILURE IS REPORTED, NEVER SWALLOWED.

    A cap that failed to install and a module legitimately absent were
    INDISTINGUISHABLE: every block ended in `except Exception: pass`, so 12 caps
    installed where 14 were written and the gap was found only by counting. That is
    the permissive-fallback class - a filter that silently becomes a pass-through -
    and the standing rule covers it: A LOOKUP THAT MISSES MUST ABORT, NOT DEFAULT
    TO PERMISSIVE.

    Under --smoke a cap that does not install is a FAILED SMOKE RUN: reductions
    silently not applying makes a 35-minute run pretend to be a 5-minute one.
    Returns (applied, failed); the caller asserts the total.
    """
    cap = os.environ.get(SMOKE_ENV)
    if not cap:
        return [], []
    k = max(2, int(cap))
    applied, failed = [], []

    def _mod(name):
        try:
            return __import__(name)
        except Exception as exc:
            failed.append(f'{name}: IMPORT FAILED {type(exc).__name__}: {str(exc)[:90]}')
            return None

    def _wrap_list(modname, attr, n):
        m = _mod(modname)
        if m is None:
            return
        if not hasattr(m, attr):
            failed.append(f'{modname}.{attr}: ATTRIBUTE ABSENT - the cap targets a symbol that '
                          f'no longer exists')
            return
        try:
            v = getattr(m, attr)
            if isinstance(v, list) and len(v) > n:
                setattr(m, attr, v[:n])
            applied.append(f'{modname}.{attr} -> {getattr(m, attr)}')
        except Exception as exc:
            failed.append(f'{modname}.{attr}: {type(exc).__name__}: {str(exc)[:70]}')

    def _wrap_call(modname, attr, n, note=''):
        m = _mod(modname)
        if m is None:
            return
        if not hasattr(m, attr):
            failed.append(f'{modname}.{attr}: ATTRIBUTE ABSENT')
            return
        flag = f'_SMOKE_{attr}'
        if getattr(m, flag, False):
            applied.append(f'{modname}.{attr} -> already wrapped')
            return
        try:
            orig = getattr(m, attr)

            def _capped(*a_, _o=orig, _k=n, **kw_):
                r = _o(*a_, **kw_)
                if isinstance(r, dict):
                    return {kk: r[kk] for kk in list(r)[:_k]}
                if isinstance(r, tuple) and len(r) == 2 and all(
                        isinstance(x, (list, tuple)) for x in r):
                    return list(r[0])[:_k], list(r[1])[:_k]
                if isinstance(r, (list, tuple)):
                    return type(r)(list(r)[:_k])
                return r

            setattr(m, attr, _capped)
            setattr(m, flag, True)
            applied.append(f'{modname}.{attr} -> first {n}{note}')
        except Exception as exc:
            failed.append(f'{modname}.{attr}: {type(exc).__name__}: {str(exc)[:70]}')

    def _set_const(modname, attr, val):
        m = _mod(modname)
        if m is None:
            return
        if not hasattr(m, attr):
            failed.append(f'{modname}.{attr}: ATTRIBUTE ABSENT')
            return
        try:
            setattr(m, attr, val)
            applied.append(f'{modname}.{attr} -> {val}')
        except Exception as exc:
            failed.append(f'{modname}.{attr}: {type(exc).__name__}: {str(exc)[:70]}')

    _wrap_list('sequential_temporal', 'LAGS', 2)
    _wrap_call('sequential_temporal', 'scorable_pool', k, ' (B axis)')
    _wrap_call('session_temporal', 'session_masks', k)
    _wrap_call('session_temporal', 'weekday_masks', k)
    _wrap_call('conditional_interaction', 'build_gate_masks', k)
    _wrap_list('rolling_leadlag', 'WINDOWS', 2)
    _wrap_list('rolling_leadlag', 'RELATIONS', 2)
    _wrap_list('divergence_nonconfirm', 'FLOW_FEATS', 2)
    _cap_null_arm(applied, failed)
    _cap_align_pool(k, applied, failed)
    _set_const('concurrence_profiler', 'MIN_STACK_BARS', 1)
    # F13's directions are a HARDCODED TUPLE inside a loop at
    # single_variable_extremes.py L291 - ('LONG', 'SHORT') - with no module-level
    # symbol to rebind, so the transport cannot reach it without a scanner edit and
    # F13 keeps both directions under smoke. F13 cost 2:52 of the last run, which is
    # acceptable; capping it would require authorising that scanner.
    return applied, failed
