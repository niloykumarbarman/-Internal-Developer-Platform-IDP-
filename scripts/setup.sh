#!/bin/bash
set -euo pipefail

# Colors
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
NAMESPACE="enterprise-idp-${ENVIRONMENT}"

log_info "🚀 Setting up Enterprise IDP — Environment: ${ENVIRONMENT}"

# Check prerequisites
check_prerequisites() {
  log_info "Checking prerequisites..."
  command -v kubectl  >/dev/null 2>&1 || log_error "kubectl not found"
  command -v helm     >/dev/null 2>&1 || log_error "helm not found"
  command -v docker   >/dev/null 2>&1 || log_error "docker not found"
  command -v terraform >/dev/null 2>&1 || log_warn "terraform not found — skipping IaC"
  log_success "Prerequisites OK"
}

# Create namespaces
setup_namespaces() {
  log_info "Creating namespaces..."
  kubectl apply -f infrastructure/kubernetes/namespaces.yaml
  log_success "Namespaces created"
}

# Setup RBAC
setup_rbac() {
  log_info "Setting up RBAC..."
  kubectl apply -f infrastructure/kubernetes/rbac.yaml
  log_success "RBAC configured"
}

# Setup storage
setup_storage() {
  log_info "Setting up storage..."
  kubectl apply -f infrastructure/kubernetes/storage.yaml
  log_success "Storage configured"
}

# Install ArgoCD
setup_argocd() {
  log_info "Installing ArgoCD..."
  kubectl create namespace argocd --dry-run=client -o yaml | kubectl apply -f -
  kubectl apply -n argocd -f https://raw.githubusercontent.com/argoproj/argo-cd/stable/manifests/install.yaml
  log_info "Waiting for ArgoCD to be ready..."
  kubectl wait --for=condition=available --timeout=300s deployment/argocd-server -n argocd
  kubectl apply -f infrastructure/argocd/project.yaml
  kubectl apply -f infrastructure/argocd/app-of-apps.yaml
  log_success "ArgoCD installed"
}

# Install monitoring stack
setup_monitoring() {
  log_info "Installing monitoring stack..."
  helm repo add prometheus-community https://prometheus-community.github.io/helm-charts
  helm repo add grafana https://grafana.github.io/helm-charts
  helm repo update
  helm upgrade --install prometheus-stack prometheus-community/kube-prometheus-stack \
    --namespace monitoring \
    --create-namespace \
    --set grafana.adminPassword=admin123 \
    --wait --timeout 300s
  helm upgrade --install loki-stack grafana/loki-stack \
    --namespace monitoring \
    --set loki.enabled=true \
    --set promtail.enabled=true \
    --wait --timeout 300s
  log_success "Monitoring stack installed"
}

# Install Vault
setup_vault() {
  log_info "Installing HashiCorp Vault..."
  helm repo add hashicorp https://helm.releases.hashicorp.com
  helm repo update
  helm upgrade --install vault hashicorp/vault \
    --namespace vault \
    --create-namespace \
    --set server.dev.enabled=true \
    --set ui.enabled=true \
    --wait --timeout 300s
  log_success "Vault installed"
}

# Deploy application
deploy_app() {
  log_info "Deploying Enterprise IDP application..."
  helm upgrade --install enterprise-idp helm/enterprise-idp \
    --namespace ${NAMESPACE} \
    --create-namespace \
    --values helm/enterprise-idp/values.yaml \
    --values helm/enterprise-idp/values-${ENVIRONMENT}.yaml \
    --wait --timeout 300s
  log_success "Application deployed"
}

# Main
main() {
  check_prerequisites
  setup_namespaces
  setup_rbac
  setup_storage

  case "${ENVIRONMENT}" in
    dev)
      setup_monitoring
      setup_vault
      deploy_app
      ;;
    staging|production)
      setup_argocd
      setup_monitoring
      setup_vault
      deploy_app
      ;;
    *)
      log_error "Unknown environment: ${ENVIRONMENT}"
      ;;
  esac

  log_success "✅ Enterprise IDP setup complete for ${ENVIRONMENT}!"
  echo ""
  echo "📊 Access URLs:"
  echo "  App:        http://app.enterprise-idp.local"
  echo "  API:        http://api.enterprise-idp.local"
  echo "  Grafana:    http://localhost:3000 (admin/admin123)"
  echo "  ArgoCD:     http://localhost:8080"
  echo "  Vault:      http://localhost:8200 (token: dev-root-token)"
}

main "$@"
