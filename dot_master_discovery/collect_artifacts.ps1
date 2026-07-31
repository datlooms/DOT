# Collects every DOT analysis artifact into one flat folder, then splits
# anything over 28 MB into part_N files with the header repeated.

$src      = "C:\Users\d\Documents\GitHub\DOT_deploy\dot_master_discovery"
$dst      = "C:\Users\d\Documents\GitHub\deploy_output"
$limitMB  = 28

New-Item -ItemType Directory -Force -Path $dst | Out-Null

$searchRoots = @(
    (Join-Path $src "discovery\full"),
    (Join-Path $src "data")
)

$copied  = 0
$skipped = 0

foreach ($root in $searchRoots) {
    if (-not (Test-Path $root)) { Write-Host "MISSING ROOT: $root" -ForegroundColor Yellow; continue }

    Get-ChildItem -Path $root -Recurse -File | ForEach-Object {
        $n = $_.Name

        # --- EXCLUSIONS ---
        if ($n -match '_c\d{4}\.(csv|pkl|done|cand)$') { $skipped++; return }
        if ($n -match '\.done$')                       { $skipped++; return }
        if ($n -match '\.cand$')                       { $skipped++; return }
        if ($n -match '\.provenance$')                 { $skipped++; return }
        if ($n -match '^_frame_.*\.csv$')              { $skipped++; return }
        if ($n -match '^_s3_frame.*\.csv$')            { $skipped++; return }
        if ($_.FullName -match '\\_f13_shards\\')      { $skipped++; return }
        if ($_.FullName -match '\\\.markers\\')        { $skipped++; return }
        if ($n -match '^shard_\d+\.csv$')              { $skipped++; return }
        if ($_.Extension -notin @('.csv','.md','.txt','.jsonl')) { $skipped++; return }

        $target = Join-Path $dst $n
        if (Test-Path $target) {
            $parent = Split-Path $_.DirectoryName -Leaf
            $target = Join-Path $dst ("{0}__{1}" -f $parent, $n)
        }
        Copy-Item $_.FullName -Destination $target -Force
        $copied++
    }
}

Write-Host ""
Write-Host "COPIED : $copied files" -ForegroundColor Green
Write-Host "SKIPPED: $skipped (chunks, markers, provenance, shards, temporaries)"
Write-Host ""

# ---------------------------------------------------------------------------
# SPLIT anything over the limit. CSV keeps its header on every part.
# ---------------------------------------------------------------------------
$limitBytes = $limitMB * 1MB
$big = Get-ChildItem -Path $dst -File | Where-Object { $_.Length -gt $limitBytes } |
       Sort-Object Length -Descending

if (-not $big) {
    Write-Host "No file exceeds $limitMB MB - nothing to split." -ForegroundColor Green
} else {
    foreach ($f in $big) {
        $base = [IO.Path]::GetFileNameWithoutExtension($f.Name)
        $ext  = $f.Extension
        $parts = [Math]::Ceiling($f.Length / $limitBytes)

        Write-Host ("SPLITTING {0}  ({1:N1} MB -> {2} parts)" -f $f.Name, ($f.Length/1MB), $parts) -ForegroundColor Cyan

        $reader = [IO.StreamReader]::new($f.FullName)
        try {
            $header = $null
            if ($ext -eq '.csv') { $header = $reader.ReadLine() }

            $idx     = 1
            $writer  = $null
            $written = 0

            while (-not $reader.EndOfStream) {
                if ($null -eq $writer) {
                    $outPath = Join-Path $dst ("{0}_part_{1}{2}" -f $base, $idx, $ext)
                    $writer  = [IO.StreamWriter]::new($outPath, $false, [Text.Encoding]::UTF8)
                    $written = 0
                    if ($header) { $writer.WriteLine($header); $written += $header.Length + 2 }
                }

                $line = $reader.ReadLine()
                $writer.WriteLine($line)
                $written += $line.Length + 2

                if ($written -ge $limitBytes) {
                    $writer.Close(); $writer = $null; $idx++
                }
            }
            if ($writer) { $writer.Close() }
        }
        finally { $reader.Close() }

        Remove-Item $f.FullName -Force
        Write-Host ("  -> {0}_part_1{1} .. {0}_part_{2}{1}   (original removed)" -f $base, $ext, ($idx)) -ForegroundColor DarkGray
    }
}

# ---------------------------------------------------------------------------
Write-Host ""
$all = Get-ChildItem -Path $dst -File
Write-Host ("FINAL: {0} files, {1:N1} MB total" -f $all.Count, (($all | Measure-Object Length -Sum).Sum / 1MB)) -ForegroundColor Green
$over = $all | Where-Object { $_.Length -gt $limitBytes }
if ($over) {
    Write-Host "STILL OVER LIMIT:" -ForegroundColor Red
    $over | ForEach-Object { "  {0,-52} {1,8:N1} MB" -f $_.Name, ($_.Length/1MB) }
} else {
    Write-Host "Every file is under $limitMB MB." -ForegroundColor Green
}
