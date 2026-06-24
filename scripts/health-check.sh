#!/bin/bash
set -euo pipefail

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

log_info()    { echo -e "${BLUE}[INFO]${NC} $1"; }
log_success() { echo -e "${GREEN}[✓]${NC} $1"; }
log_warn()    { echo -e "${YELLOW}[⚠]${NC} $1"; }
log_error()   { echo -e "${RED}[✗]${NC} $1"; }

ENVIRONMENT=${1:-dev}
NAMESPACE="enterprise-idp-${ENVIRONMENT}"
PASS=0
FAIL=0

check() {
  local name=$1
  local cmd=$2
  if eval "${cmd}" >/dev/null 2>&1; then
    log_success "${name}"
    ((PASS++))
  else
    log_error "${name}"
    ((FAIL++))
  fi
}

echo ""
echo "🏥 Enterprise IDP Health Check — ${ENVIRONMENT}"
echo "================================================"

# Kubernetes
echo ""
log_info "📦 Kubernetes:"
check "Cluster reachable"        "kubectl cluster-info"
check "Namespace exists"         "kubectl get namespace ${NAMESPACE}"
check "Backend deployment ready" "kubectl rollout status deployment/enterprise-idp-backend -n ${NAMESPACE} --timeout=30s"
check "Frontend deployment ready" "kubectl rollout status deployment/enterprise-idp-frontend -n ${NAMESPACE} --timeout=30s"
check "PostgreSQL StatefulSet ready" "kubectl rollout status statefulset/enterprise-idp-postgresql -n ${NAMESPACE} --timeout=30s"

# Pods
echo ""
log_info "🐳 Pods:"
check "All pods running" "[ \$(kubectl get pods -n ${NAMESPACE} --field-selector=status.phase!=Running --no-headers 2>/dev/null | wc -l) -eq 0 ]"

# Services
echo ""
log_info "🌐 Services:"
check "Backend service exists"  "kubectl get service enterprise-idp-backend -n ${NAMESPACE}"
check "Frontend service exists" "kubectl get service enterprise-idp-frontend -n ${NAMESPACE}"
check "PostgreSQL service exists" "kubectl get service enterprise-idp-postgresql -n ${NAMESPACE}"
check "Redis service exists"    "kubectl get service enterprise-idp-redis -n ${NAMESPACE}"

# Monitoring
echo ""
log_info "📊 Monitoring:"
check "Prometheus running" "kubectl get pods -n monitoring -l app=prometheus --field-selector=status.phase=Running --no-headers | grep -q ."
check "Grafana running"    "kubectl get pods -n monitoring -l app.kubernetes.io/name=grafana --field-selector=status.phase=Running --no-headers | grep -q ."
check "Loki running"       "kubectl get pods -n monitoring -l app=loki --field-selector=status.phase=Running --no-headers | grep -q ."

# Vault
echo ""
log_info "🔐 Vault:"
check "Vault running" "kubectl get pods -n vault -l app.kubernetes.io/name=vault --field-selector=status.phase=Running --no-headers | grep -q ."

# Summary
echo ""
echo "================================================"
echo -e "Results: ${GREEN}${PASS} passed${NC} | ${RED}${FAIL} failed${NC}"
echo ""

if [[ ${FAIL} -gt 0 ]]; then
  log_warn "Some checks failed. Run: kubectl get pods -n ${NAMESPACE} for details"
  exit 1
else
  log_success "All checks passed! 🎉"
  exit 0
fi
