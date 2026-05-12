#Requires -Version 5.1
# Usage: .\scripts\start.ps1 [-NoBuild] [-NoDetach]
[CmdletBinding()]
param(
    [switch]$NoBuild,
    [switch]$NoDetach
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$RootDir = Split-Path -Parent $PSScriptRoot
$EnvFile = Join-Path $RootDir ".env"

# --- Load .env ---
if (-not (Test-Path $EnvFile)) {
    Write-Error ".env not found. Run: Copy-Item .env.example .env  then fill in SA_PASSWORD and JWT_SECRET_KEY"
}

Get-Content $EnvFile | ForEach-Object {
    if ($_ -match '^\s*(#|$)') { return }
    $parts = $_ -split '=', 2
    if ($parts.Length -eq 2) {
        $name  = $parts[0].Trim()
        $value = $parts[1].Trim()
        [System.Environment]::SetEnvironmentVariable($name, $value, "Process")
    }
}

# --- Validate required vars ---
$required = @("SA_PASSWORD", "JWT_SECRET_KEY")
$missing  = $required | Where-Object { -not [System.Environment]::GetEnvironmentVariable($_) }

if ($missing) {
    Write-Error "Missing required variables in .env: $($missing -join ', ')"
}

# --- Build and start ---
Set-Location $RootDir

$composeArgs = @("up")
if (-not $NoBuild)  { $composeArgs += "--build" }
if (-not $NoDetach) { $composeArgs += "-d" }

Write-Host "Starting RetailsEcosystem..." -ForegroundColor Cyan
docker compose @composeArgs
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

# --- Health check ---
if (-not $NoDetach) {
    Write-Host "Waiting for API to become healthy..." -ForegroundColor Yellow
    $timeout = 60
    $elapsed = 0
    $healthy = $false

    while ($elapsed -lt $timeout) {
        Start-Sleep -Seconds 5
        $elapsed += 5
        try {
            $r = Invoke-WebRequest -Uri "http://localhost:8080/health" -UseBasicParsing -TimeoutSec 3 -ErrorAction Stop
            if ($r.StatusCode -eq 200) { $healthy = $true; break }
        } catch { }
        Write-Host "  Still waiting... ($elapsed/${timeout}s)"
    }

    if (-not $healthy) {
        Write-Warning "API did not become healthy within ${timeout}s. Check: docker compose logs api"
    } else {
        Write-Host ""
        Write-Host "RetailsEcosystem is up:" -ForegroundColor Green
        Write-Host "  API:   http://localhost:8080"
        Write-Host "  Web:   http://localhost:8081"
        Write-Host "  Admin: http://localhost:3000"
    }
}
