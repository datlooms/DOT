import os

_ENV_FRAME = 'DOT_FRAME_PATH'
_ENV_SHA = 'DOT_INPUT_SHA'
_ENV_FP = 'DOT_FRAME_FINGERPRINT'
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
    for modname, fields in _SCANNER_PATH_MODULES:
        try:
            mod = __import__(modname)
        except Exception:
            continue
        for attr, leaf in fields.items():
            if not hasattr(mod, attr):
                continue
            setattr(mod, attr, rd if not leaf else os.path.join(rd, leaf))
        done.append(modname)
    return done


SMOKE_ENV = 'DOT_SMOKE_CAP'


def install_smoke_caps():
    """FOURTH INSTANCE of the parent-only-global class. Same transport, same reason.

    --s3-limit caps ONE axis per family - the one the orchestrator chunks on. For
    F1 that is the A-label list only, so each of 40 chunks still scanned 239
    B-labels x 15 lags x 2 directions = 286,800 candidates and the smoke run never
    finished a chunk. F9 has the same shape on its B-label list; F13 fans over both
    directions inside its own pool.

    A PARENT-SIDE ATTRIBUTE WRITE DOES NOT SURVIVE SPAWN - that is the whole reason
    install_scanner_paths exists, and this reuses it rather than editing a scanner.
    Every worker re-imports the scanner fresh and this hook runs at INTERPRETER
    STARTUP before any worker code, so the caps are in place before the scan begins.
    NO SCANNER FILE IS MODIFIED.

    Reachable module attributes only - LAGS is a module list and scorable_pool a
    module function, so wrapping them is configuration, not a code change.
    """
    cap = os.environ.get(SMOKE_ENV)
    if not cap:
        return []
    k = max(2, int(cap))
    done = []
    try:
        import sequential_temporal as _st
    except Exception:
        _st = None
    if _st is not None:
        if hasattr(_st, 'LAGS') and len(_st.LAGS) > 2:
            _st.LAGS = list(_st.LAGS)[:2]
            done.append(f'sequential_temporal.LAGS -> {_st.LAGS}')
        if hasattr(_st, 'scorable_pool') and not getattr(_st, '_SMOKE_WRAPPED', False):
            _orig = _st.scorable_pool

            def _capped(pool, warmup, _o=_orig, _k=k):
                return _o(pool, warmup)[:_k]

            _st.scorable_pool = _capped
            _st._SMOKE_WRAPPED = True
            done.append(f'sequential_temporal.scorable_pool -> first {k} labels (B axis)')
    for modname, attr in (('session_temporal', 'scorable_pool'),
                          ('state_transition', 'scorable_pool'),
                          ('conditional_interaction', 'scorable_pool')):
        try:
            mod = __import__(modname)
        except Exception:
            continue
        if hasattr(mod, attr) and not getattr(mod, '_SMOKE_WRAPPED', False):
            _o2 = getattr(mod, attr)

            def _capped2(pool, warmup, _o=_o2, _k=k):
                return _o(pool, warmup)[:_k]

            setattr(mod, attr, _capped2)
            mod._SMOKE_WRAPPED = True
            done.append(f'{modname}.{attr} -> first {k}')
    # F9: the SESSION/WEEKDAY GATE axis. --s3-limit reaches base_labels only, so the
    # audit predicted 40 x 6 x 2 = 480 and the run printed
    #   'Search: 40 base x 35 session/weekday gates x 2 dir = 2800 candidates'
    # F9 took 476.4s of a 69-minute smoke run. IDENTICAL SHAPE TO F1: a two-axis
    # scanner where the cap reaches one axis. session_masks and weekday_masks are
    # module functions, so wrapping them is configuration, not a code change.
    for _mn, _fns in (('session_temporal', ('session_masks', 'weekday_masks')),
                      ('conditional_interaction', ('build_gate_masks',)),
                      ('divergence_nonconfirm', ('flow_pool',)),
                      ('rolling_leadlag', ('pair_pool',))):
        try:
            _m = __import__(_mn)
        except Exception:
            continue
        for _fn in _fns:
            if not hasattr(_m, _fn) or getattr(_m, f'_SMOKE_{_fn}', False):
                continue
            _o3 = getattr(_m, _fn)

            def _cap_dict(*a_, _o=_o3, _k=k, **kw_):
                r = _o(*a_, **kw_)
                if isinstance(r, dict):
                    return {kk: r[kk] for kk in list(r)[:_k]}
                if isinstance(r, (list, tuple)):
                    return type(r)(list(r)[:_k])
                return r

            setattr(_m, _fn, _cap_dict)
            setattr(_m, f'_SMOKE_{_fn}', True)
            done.append(f'{_mn}.{_fn} -> first {k}')
    for _mn, _attr in (('rolling_leadlag', 'WINDOWS'), ('rolling_leadlag', 'RELATIONS'),
                       ('divergence_nonconfirm', 'FLOW_FEATS')):
        try:
            _m2 = __import__(_mn)
        except Exception:
            continue
        _v = getattr(_m2, _attr, None)
        if isinstance(_v, list) and len(_v) > 2:
            setattr(_m2, _attr, _v[:2])
            done.append(f'{_mn}.{_attr} -> first 2')
    # S5C's RANDOM-TRIPLE NULL ARM. --smoke never reached it: the banner's
    # 'null K=40/family' is S5D's PRICING null, a different draw in a different
    # module. This arm draws until NULL_TARGET_QUALIFIERS triples QUALIFY, so at a
    # low pass rate it scores thousands - 2,424 of the 2,436 signals S5C scored on a
    # pool of TWELVE, 553s and 26.7% of the smoke run. A target of ~8 with a floor of
    # 4 proves the code path executes, which is the whole purpose of a smoke run.
    try:
        import wf_selection as _wfs
        for _a, _v in (('NULL_TARGET_QUALIFIERS', 8), ('NULL_FLOOR_QUALIFIERS', 4),
                       ('NULL_GEN_BATCH', 24), ('NULL_TRIPLES_CAP', 200)):
            if hasattr(_wfs, _a) and getattr(_wfs, _a) != _v:
                setattr(_wfs, _a, _v)
                done.append(f'wf_selection.{_a} -> {_v}')
    except Exception:
        pass
    # F12: THE CONDITION-POOL AXIS. Smoke already sets k=1..2 and n_perm=3, but F12
    # multiplies over its DIRECTIONAL LABEL LISTS in stages 3, 5 and 5b - 249
    # conditions - and nothing capped those. S3's stage time was 1253s against a
    # 169s chunk phase, so ~18 min sat after the family chunks. THIRD INSTANCE of a
    # cap reaching one axis of a multi-axis stage, after F1's B-labels and F9's
    # session gates. align_pool is a module function returning (long, short).
    try:
        import concurrence_profiler as _cp2
        if hasattr(_cp2, 'align_pool') and not getattr(_cp2, '_SMOKE_ALIGN', False):
            _oa = _cp2.align_pool

            def _capped_align(pool, _o=_oa, _k=k):
                lo, sh = _o(pool)
                return list(lo)[:_k], list(sh)[:_k]

            _cp2.align_pool = _capped_align
            _cp2._SMOKE_ALIGN = True
            done.append(f'concurrence_profiler.align_pool -> first {k} labels per direction')
        if hasattr(_cp2, 'MIN_STACK_BARS'):
            _cp2.MIN_STACK_BARS = 1
            done.append('concurrence_profiler.MIN_STACK_BARS -> 1')
    except Exception:
        pass
    try:
        import single_variable_extremes as _f13
        if hasattr(_f13, 'DIRECTIONS') and len(_f13.DIRECTIONS) > 1:
            _f13.DIRECTIONS = list(_f13.DIRECTIONS)[:1]
            done.append(f'single_variable_extremes.DIRECTIONS -> {_f13.DIRECTIONS}')
    except Exception:
        pass
    return done
