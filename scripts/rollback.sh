#!/bin/bash
set -euo pipefail

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

log_info()    { echo -e "${BLUE}[INFO]${NC} $1"; }
log_success() { echo -e "${GREEN}[SUCCESS]${NC} $1"; }
log_warn()    { echo -e "${YELLOW}[WARN]${NC} $1"; }
log_error()   { echo -e "${RED}[ERROR]${NC} $1"; exit 1; }

ENVIRONMENT=${1:-dev}
REVISION=${2:-0}
NAMESPACE="enterprise-idp-${ENVIRONMENT}"
RELEASE_NAME="enterprise-idp"

log_warn "⚠️  Rolling back Enterprise IDP"
log_info "   Environment : ${ENVIRONMENT}"
log_info "   Revision    : ${REVISION:-previous}"
log_info "   Namespace   : ${NAMESPACE}"

# Show history
show_history() {
  log_info "Helm release history:"
  helm history ${RELEASE_NAME} --namespace ${NAMESPACE} --max 10
}

# Rollback
rollback() {
  show_history
  log_warn "Starting rollback in 5 seconds... (Ctrl+C to cancel)"
  sleep 5

  if [[ "${REVISION}" == "0" ]]; then
    helm rollback ${RELEASE_NAME} --namespace ${NAMESPACE} --wait --timeout 300s
  else
    helm rollback ${RELEASE_NAME} ${REVISION} --namespace ${NAMESPACE} --wait --timeout 300s
  fi

  log_success "Rollback complete"
}

# Verify
verify() {
  log_info "Verifying rollback..."
  kubectl rollout status deployment/${RELEASE_NAME}-backend \
    -n ${NAMESPACE} --timeout=120s
  kubectl get pods -n ${NAMESPACE} -l app=${RELEASE_NAME}
  log_success "Rollback verified"
}

main() {
  rollback
  verify
  log_success "✅ Rollback complete for ${ENVIRONMENT}"
}

main "$@"
