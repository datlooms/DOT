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
