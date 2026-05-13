#!/usr/bin/env bash
# Usage: ./scripts/start.pull.sh
# Pulls latest images from DockerHub then starts the full stack.
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(dirname "$SCRIPT_DIR")"
ENV_FILE="$ROOT_DIR/.env"

if [[ ! -f "$ENV_FILE" ]]; then
  echo "ERROR: .env not found."
  echo "  cp .env.example .env  — then fill in SA_PASSWORD, JWT_SECRET_KEY, DOCKERHUB_USERNAME"
  exit 1
fi

set -a
# shellcheck source=/dev/null
source "$ENV_FILE"
set +a

REQUIRED=(SA_PASSWORD JWT_SECRET_KEY DOCKERHUB_USERNAME)
MISSING=()
for var in "${REQUIRED[@]}"; do
  [[ -z "${!var:-}" ]] && MISSING+=("$var")
done

if [[ ${#MISSING[@]} -gt 0 ]]; then
  echo "ERROR: Missing required variables in .env: ${MISSING[*]}"
  exit 1
fi

cd "$ROOT_DIR"

echo "Pulling latest images from DockerHub..."
docker compose -f docker-compose.pull.yml pull

echo "Starting RetailsEcosystem..."
docker compose -f docker-compose.pull.yml up -d

echo "Waiting for API to become healthy..."
TIMEOUT=60
ELAPSED=0
until curl -sf http://localhost:8080/health > /dev/null 2>&1; do
  if [[ $ELAPSED -ge $TIMEOUT ]]; then
    echo "WARNING: API did not become healthy within ${TIMEOUT}s."
    echo "  Check logs: docker compose -f docker-compose.pull.yml logs api"
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
