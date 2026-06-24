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
IMAGE_TAG=${2:-latest}
NAMESPACE="enterprise-idp-${ENVIRONMENT}"
RELEASE_NAME="enterprise-idp"

log_info "🚀 Deploying Enterprise IDP"
log_info "   Environment : ${ENVIRONMENT}"
log_info "   Image Tag   : ${IMAGE_TAG}"
log_info "   Namespace   : ${NAMESPACE}"

# Validate environment
validate() {
  [[ "${ENVIRONMENT}" =~ ^(dev|staging|production)$ ]] || \
    log_error "Invalid environment: ${ENVIRONMENT}. Use dev|staging|production"
  command -v kubectl >/dev/null 2>&1 || log_error "kubectl not found"
  command -v helm    >/dev/null 2>&1 || log_error "helm not found"
}

# Pre-deploy checks
pre_deploy() {
  log_info "Running pre-deploy checks..."
  kubectl cluster-info >/dev/null 2>&1 || log_error "Cannot connect to Kubernetes cluster"
  helm lint helm/enterprise-idp \
    --values helm/enterprise-idp/values.yaml \
    --values helm/enterprise-idp/values-${ENVIRONMENT}.yaml || \
    log_error "Helm lint failed"
  log_success "Pre-deploy checks passed"
}

# Deploy
deploy() {
  log_info "Deploying via Helm..."
  helm upgrade --install ${RELEASE_NAME} helm/enterprise-idp \
    --namespace ${NAMESPACE} \
    --create-namespace \
    --values helm/enterprise-idp/values.yaml \
    --values helm/enterprise-idp/values-${ENVIRONMENT}.yaml \
    --set image.tag=${IMAGE_TAG} \
    --set environment=${ENVIRONMENT} \
    --atomic \
    --cleanup-on-fail \
    --timeout 300s \
    --history-max 10

  log_success "Helm deployment complete"
}

# Post-deploy checks
post_deploy() {
  log_info "Running post-deploy checks..."
  kubectl rollout status deployment/${RELEASE_NAME}-backend \
    -n ${NAMESPACE} --timeout=120s
  kubectl rollout status deployment/${RELEASE_NAME}-frontend \
    -n ${NAMESPACE} --timeout=120s

  READY=$(kubectl get pods -n ${NAMESPACE} \
    -l app=${RELEASE_NAME} \
    --field-selector=status.phase=Running \
    --no-headers | wc -l)

  log_success "Post-deploy: ${READY} pods running"
}

# Tag git commit
tag_release() {
  if [[ "${ENVIRONMENT}" == "production" ]]; then
    log_info "Tagging release..."
    git tag -a "release-${IMAGE_TAG}" -m "Production release ${IMAGE_TAG}" 2>/dev/null || \
      log_warn "Git tag already exists"
    log_success "Release tagged: release-${IMAGE_TAG}"
  fi
}

main() {
  validate
  pre_deploy
  deploy
  post_deploy
  tag_release
  log_success "✅ Deployment complete! Tag: ${IMAGE_TAG} → ${ENVIRONMENT}"
}

main "$@"
