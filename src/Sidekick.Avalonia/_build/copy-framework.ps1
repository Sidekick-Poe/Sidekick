param(
    [string]$NuGetRoot,
    [string]$OutDir
)

$base = Join-Path $NuGetRoot "microsoft.aspnetcore.app.internal.assets"

# Find version folders
$versions = Get-ChildItem $base -Directory -Filter "10.*" | Sort-Object Name -Descending

if ($versions.Count -eq 0) {
    Write-Host "No version folders found under $base"
    exit 1
}

$latest = $versions[0].FullName
Write-Host "Using Blazor framework from: $latest"

$src = Join-Path $latest "_framework"
$dst = Join-Path $OutDir "wwwroot\_framework"

New-Item -ItemType Directory -Force -Path $dst | Out-Null
Copy-Item $src\* $dst -Recurse -Force
