#Requires -Version 5.1
# Usage: .\scripts\down.ps1 [-Volumes]
# -Volumes: also removes named volumes (destroys database data)
[CmdletBinding()]
param([switch]$Volumes)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$RootDir = Split-Path -Parent $PSScriptRoot
Set-Location $RootDir

$composeArgs = @("down")
if ($Volumes) { $composeArgs += "--volumes" }

Write-Host "Stopping RetailsEcosystem..." -ForegroundColor Yellow
docker compose @composeArgs
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
Write-Host "Done." -ForegroundColor Green
