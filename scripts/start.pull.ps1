#Requires -Version 5.1
# Usage: .\scripts\start.pull.ps1
# Pulls latest images from DockerHub then starts the full stack.
Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$RootDir = Split-Path -Parent $PSScriptRoot
$EnvFile = Join-Path $RootDir ".env"

if (-not (Test-Path $EnvFile)) {
    Write-Error ".env not found. Run: Copy-Item .env.example .env  then fill in SA_PASSWORD, JWT_SECRET_KEY, DOCKERHUB_USERNAME"
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

$required = @("SA_PASSWORD", "JWT_SECRET_KEY", "DOCKERHUB_USERNAME")
$missing  = $required | Where-Object { -not [System.Environment]::GetEnvironmentVariable($_) }

if ($missing) {
    Write-Error "Missing required variables in .env: $($missing -join ', ')"
}

Set-Location $RootDir

Write-Host "Pulling latest images from DockerHub..." -ForegroundColor Cyan
docker compose -f docker-compose.pull.yml pull
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "Starting RetailsEcosystem..." -ForegroundColor Cyan
docker compose -f docker-compose.pull.yml up -d
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

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
    Write-Warning "API did not become healthy within ${timeout}s. Check: docker compose -f docker-compose.pull.yml logs api"
} else {
    Write-Host ""
    Write-Host "RetailsEcosystem is up:" -ForegroundColor Green
    Write-Host "  API:   http://localhost:8080"
    Write-Host "  Web:   http://localhost:8081"
    Write-Host "  Admin: http://localhost:3000"
}
