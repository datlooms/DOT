"""sweep_undefined.py — THE UNDEFINED-SYMBOL GATE.

    python sweep_undefined.py [--dir <package root>]
    exit 0 = clean, exit 1 = at least one undefined symbol

WHY THIS EXISTS, AND WHY THE EXISTING CHECKS ARE BLIND TO IT.

The never-called sweep finds symbols DEFINED AND NEVER USED. This is the exact
inverse: USED AND NEVER DEFINED. Neither that sweep nor a symbol-table diff can
see it, and the diff especially cannot, because nothing was LOST - something was
never ADDED.

The case that motivated it: _blank_pf was CALLED at concurrence_profiler.py L314
and L319 and DEFINED NOWHERE. The calls were written, the function was not, and it
was reported as done. Every existing check passed:

  * the module imports fine - a NameError is raised at CALL time, not import time
  * py_compile passes - it is valid syntax
  * the banner passes, S0/S1/S2 pass, F0 and F1 scan for forty minutes
  * then it dies inside a POOL WORKER at stage 4 of F12

THE CHEAPEST POSSIBLE DEFECT FOUND AT THE MOST EXPENSIVE POSSIBLE MOMENT. This
check finds it in about a second, before anything runs.

It resolves, per function: parameters, every local assignment, comprehension
targets, except-handler names, nested defs and lambdas, walrus targets, global
and nonlocal declarations, imports at any scope, class attributes, and every
module-level name. What remains is genuinely unresolvable at runtime.
"""

import argparse
import ast
import builtins
import os
import sys

BUILTINS = set(dir(builtins)) | {'__file__', '__name__', '__doc__', '__spec__',
                                 '__package__', '__loader__', '__builtins__'}
# Names a pool worker inherits from a module-level global set by an initializer.
WORKER_GLOBALS = ('_CTX', '_G', '_PAYLOAD', '_COUNTER')


def _bound_in(node):
    """Every name this scope binds, by any mechanism."""
    out = set()
    args = getattr(node, 'args', None)
    if args is not None:
        for a in (list(args.args) + list(args.posonlyargs) + list(args.kwonlyargs)):
            out.add(a.arg)
        if args.vararg:
            out.add(args.vararg.arg)
        if args.kwarg:
            out.add(args.kwarg.arg)
    for x in ast.walk(node):
        if isinstance(x, ast.Name) and isinstance(x.ctx, (ast.Store, ast.Del)):
            out.add(x.id)
        elif isinstance(x, ast.NamedExpr) and isinstance(x.target, ast.Name):
            out.add(x.target.id)
        elif isinstance(x, (ast.Import, ast.ImportFrom)):
            for al in x.names:
                out.add((al.asname or al.name).split('.')[0])
        elif isinstance(x, ast.comprehension):
            for t in ast.walk(x.target):
                if isinstance(t, ast.Name):
                    out.add(t.id)
        elif isinstance(x, ast.ExceptHandler) and x.name:
            out.add(x.name)
        elif isinstance(x, (ast.FunctionDef, ast.AsyncFunctionDef, ast.ClassDef)):
            out.add(x.name)
            a2 = getattr(x, 'args', None)
            if a2 is not None:
                for a in (list(a2.args) + list(a2.posonlyargs) + list(a2.kwonlyargs)):
                    out.add(a.arg)
                if a2.vararg:
                    out.add(a2.vararg.arg)
                if a2.kwarg:
                    out.add(a2.kwarg.arg)
        elif isinstance(x, ast.Lambda):
            a3 = x.args
            for a in (list(a3.args) + list(a3.posonlyargs) + list(a3.kwonlyargs)):
                out.add(a.arg)
        elif isinstance(x, (ast.Global, ast.Nonlocal)):
            out.update(x.names)
        elif isinstance(x, (ast.With, ast.AsyncWith)):
            for it in x.items:
                if it.optional_vars is not None:
                    for t in ast.walk(it.optional_vars):
                        if isinstance(t, ast.Name):
                            out.add(t.id)
    return out


def module_names(tree):
    out = set()
    for n in tree.body:
        if isinstance(n, (ast.FunctionDef, ast.AsyncFunctionDef, ast.ClassDef)):
            out.add(n.name)
        elif isinstance(n, ast.Assign):
            for t in n.targets:
                for x in ast.walk(t):
                    if isinstance(x, ast.Name):
                        out.add(x.id)
        elif isinstance(n, (ast.AnnAssign, ast.AugAssign)):
            for x in ast.walk(n.target):
                if isinstance(x, ast.Name):
                    out.add(x.id)
        elif isinstance(n, (ast.Import, ast.ImportFrom)):
            for al in n.names:
                out.add((al.asname or al.name).split('.')[0])
        elif isinstance(n, (ast.For, ast.While, ast.If, ast.Try, ast.With)):
            out |= _bound_in(n)
    return out


def check_module(path):
    src = open(path, encoding='utf-8', errors='replace').read()
    try:
        tree = ast.parse(src)
    except SyntaxError as exc:
        return [('<SYNTAX>', exc.lineno or 0, str(exc)[:60])]
    mod = module_names(tree)
    findings = []
    # map every function to its enclosing scopes so a method sees self, cls and the
    # class body, and a nested def sees the names its parent bound. Walking flat was
    # the reason a first pass reported 109 false positives on ordinary methods.
    enclosing = {}

    def _descend(n, chain):
        for ch in ast.iter_child_nodes(n):
            if isinstance(ch, (ast.FunctionDef, ast.AsyncFunctionDef)):
                enclosing[ch] = chain
                _descend(ch, chain | _bound_in(ch))
            elif isinstance(ch, ast.ClassDef):
                _descend(ch, chain | _bound_in(ch))
            else:
                _descend(ch, chain)

    _descend(tree, set())
    for node in ast.walk(tree):
        if not isinstance(node, (ast.FunctionDef, ast.AsyncFunctionDef)):
            continue
        bound = _bound_in(node) | enclosing.get(node, set()) | mod | BUILTINS
        for x in ast.walk(node):
            if isinstance(x, ast.Name) and isinstance(x.ctx, ast.Load) and x.id not in bound:
                findings.append((node.name, x.lineno, x.id))
    # module-level loads too
    top = set()
    for n in tree.body:
        if isinstance(n, (ast.FunctionDef, ast.AsyncFunctionDef, ast.ClassDef)):
            continue
        # a module-level comprehension binds its own target; resolve per-node rather
        # than against the module set alone, or every `[f(i) for i in xs]` at module
        # scope is reported as an undefined `i`.
        local = _bound_in(n)
        for x in ast.walk(n):
            if isinstance(x, ast.Name) and isinstance(x.ctx, ast.Load) and \
                    x.id not in (mod | BUILTINS | local):
                top.add((x.lineno, x.id))
    for ln, nm in sorted(top):
        findings.append(('<module>', ln, nm))
    return findings


def main():
    ap = argparse.ArgumentParser(description='Undefined-symbol gate.')
    ap.add_argument('--dir', default=os.path.dirname(os.path.abspath(__file__)))
    a = ap.parse_args()
    mods = []
    for sub in ('.', 'engine', 'scanners', 'orchestrator'):
        d = os.path.join(a.dir, sub)
        if not os.path.isdir(d):
            continue
        for fn in sorted(os.listdir(d)):
            if fn.endswith('.py'):
                mods.append((fn if sub == '.' else f'{sub}/{fn}', os.path.join(d, fn)))
    bad = 0
    examined, skipped = [], []
    for rel, path in mods:
        fs = check_module(path)
        if any(f[0] == '<SYNTAX>' for f in fs):
            skipped.append(f'{rel} (unparseable)')
        else:
            examined.append(rel)
        real = [f for f in fs if not any(f[2].startswith(w) for w in WORKER_GLOBALS)]
        if real:
            bad += len(real)
            for fnname, ln, nm in real:
                print(f'  UNDEFINED  {rel}:{ln}  in {fnname}()  -> {nm}')
    print(f'  COVERAGE: {len(examined)} of {len(mods)} modules EXAMINED, {len(skipped)} '
          f'UNEXAMINED' + (f' ({", ".join(skipped)})' if skipped else '')
          + ' | A DETECTOR THAT CANNOT SAY WHAT IT DID NOT EXAMINE IS NOT A DETECTOR.')
    if skipped:
        print(f'  *** {len(skipped)} MODULE(S) COULD NOT BE EXAMINED: {skipped}. AN UNEXAMINED '
              f'MODULE IS NOT A PASSING MODULE - a detector that reports a gap and exits 0 is '
              f'worse than one with no coverage check, because the gap now reads as audited. ***')
        return 1
    if bad:
        print(f'  *** {bad} UNDEFINED SYMBOL(S) - a NameError waiting for the call site to run. '
              f'The never-called sweep and the symbol diff are both BLIND to this shape. ***')
        return 1
    print('  UNDEFINED SYMBOLS: none found')
    return 0


if __name__ == '__main__':
    sys.exit(main())
