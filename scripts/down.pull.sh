#!/usr/bin/env bash
# Usage: ./scripts/down.pull.sh [--volumes]
# --volumes: also removes named volumes (destroys database data)
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(dirname "$SCRIPT_DIR")"

COMPOSE_ARGS=("down")
for arg in "$@"; do
  [[ "$arg" == "--volumes" ]] && COMPOSE_ARGS+=("--volumes")
done

cd "$ROOT_DIR"
echo "Stopping RetailsEcosystem..."
docker compose -f docker-compose.pull.yml "${COMPOSE_ARGS[@]}"
echo "Done."
