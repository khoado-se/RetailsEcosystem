#!/usr/bin/env bash
# Usage: ./scripts/start.sh [--build] [--detach]
# Defaults: build=true, detach=true
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(dirname "$SCRIPT_DIR")"
ENV_FILE="$ROOT_DIR/.env"

# ── Parse flags ────────────────────────────────────────────────────────────
BUILD=true
DETACH=true
for arg in "$@"; do
  case "$arg" in
    --no-build)  BUILD=false ;;
    --no-detach) DETACH=false ;;
  esac
done

# ── Load .env ──────────────────────────────────────────────────────────────
if [[ ! -f "$ENV_FILE" ]]; then
  echo "ERROR: .env not found."
  echo "  cp .env.example .env  — then fill in SA_PASSWORD and JWT_SECRET_KEY"
  exit 1
fi

set -a
# shellcheck source=/dev/null
source "$ENV_FILE"
set +a

# ── Validate required vars ─────────────────────────────────────────────────
REQUIRED=(SA_PASSWORD JWT_SECRET_KEY)
MISSING=()
for var in "${REQUIRED[@]}"; do
  [[ -z "${!var:-}" ]] && MISSING+=("$var")
done

if [[ ${#MISSING[@]} -gt 0 ]]; then
  echo "ERROR: Missing required variables in .env: ${MISSING[*]}"
  exit 1
fi

# ── Build and start ────────────────────────────────────────────────────────
cd "$ROOT_DIR"

COMPOSE_ARGS=("up")
$BUILD   && COMPOSE_ARGS+=("--build")
$DETACH  && COMPOSE_ARGS+=("-d")

echo "Starting RetailsEcosystem..."
docker compose "${COMPOSE_ARGS[@]}"

# ── Health check ───────────────────────────────────────────────────────────
if $DETACH; then
  echo "Waiting for API to become healthy..."
  TIMEOUT=60
  ELAPSED=0
  until curl -sf http://localhost:8080/health > /dev/null 2>&1; do
    if [[ $ELAPSED -ge $TIMEOUT ]]; then
      echo "WARNING: API did not become healthy within ${TIMEOUT}s."
      echo "  Check logs: docker compose logs api"
      exit 0
    fi
    sleep 5
    ELAPSED=$((ELAPSED + 5))
    echo "  Still waiting... (${ELAPSED}/${TIMEOUT}s)"
  done

  echo ""
  echo "RetailsEcosystem is up:"
  echo "  API:   http://localhost:8080"
  echo "  Web:   http://localhost:8081"
  echo "  Admin: http://localhost:3000"
fi
