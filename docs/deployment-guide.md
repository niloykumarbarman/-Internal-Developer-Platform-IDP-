# Enterprise IDP — Deployment Guide

## Prerequisites

### Required Tools
```bash
# Check versions
kubectl version --client    # >= 1.28
helm version               # >= 3.13
docker --version           # >= 24.0
terraform --version        # >= 1.6
git --version              # >= 2.40
```

### Install Tools (Windows)
```bash
# Chocolatey
choco install kubernetes-cli helm docker-desktop terraform git -y

# Or Winget
winget install Kubernetes.kubectl Helm.Helm Docker.DockerDesktop Hashicorp.Terraform Git.Git
```

---

## Local Development Setup

### Step 1 — Clone & Configure
```bash
git clone https://github.com/YOUR_ORG/enterprise-idp.git
cd enterprise-idp
cp infrastructure/terraform/environments/dev/terraform.tfvars.example \
   infrastructure/terraform/environments/dev/terraform.tfvars
```

### Step 2 — Start Local Services
```bash
# Start all services with Docker Compose
docker-compose up -d

# Verify services
docker-compose ps
```

### Step 3 — Apply Kubernetes Manifests
```bash
# Enable Kubernetes in Docker Desktop first
kubectl apply -f infrastructure/kubernetes/namespaces.yaml
kubectl apply -f infrastructure/kubernetes/rbac.yaml
kubectl apply -f infrastructure/kubernetes/storage.yaml
```

### Step 4 — Install Monitoring Stack
```bash
helm repo add prometheus-community https://prometheus-community.github.io/helm-charts
helm repo add grafana https://grafana.github.io/helm-charts
helm repo update

helm upgrade --install prometheus-stack prometheus-community/kube-prometheus-stack \
  --namespace monitoring \
  --create-namespace \
  --set grafana.adminPassword=admin123 \
  --wait

helm upgrade --install loki-stack grafana/loki-stack \
  --namespace monitoring \
  --set loki.enabled=true \
  --set promtail.enabled=true \
  --wait
```

### Step 5 — Install Vault
```bash
helm repo add hashicorp https://helm.releases.hashicorp.com
helm repo update

helm upgrade --install vault hashicorp/vault \
  --namespace vault \
  --create-namespace \
  --set server.dev.enabled=true \
  --set server.dev.devRootToken=dev-root-token \
  --set ui.enabled=true \
  --wait
```

### Step 6 — Install ArgoCD
```bash
kubectl create namespace argocd --dry-run=client -o yaml | kubectl apply -f -
kubectl apply -n argocd \
  -f https://raw.githubusercontent.com/argoproj/argo-cd/stable/manifests/install.yaml

# Wait for ArgoCD
kubectl wait --for=condition=available \
  --timeout=300s deployment/argocd-server -n argocd

# Apply IDP ArgoCD config
kubectl apply -f infrastructure/argocd/project.yaml
kubectl apply -f infrastructure/argocd/app-of-apps.yaml

# Get ArgoCD admin password
kubectl -n argocd get secret argocd-initial-admin-secret \
  -o jsonpath="{.data.password}" | base64 -d
```

### Step 7 — Deploy Application
```bash
# Using setup script
bash scripts/setup.sh dev

# Or manually with Helm
helm upgrade --install enterprise-idp helm/enterprise-idp \
  --namespace enterprise-idp-dev \
  --create-namespace \
  --values helm/enterprise-idp/values.yaml \
  --values helm/enterprise-idp/values-dev.yaml \
  --wait
```

### Step 8 — Verify
```bash
bash scripts/health-check.sh dev
```

---

## Port Forwarding (Local Access)

```bash
# Application
kubectl port-forward svc/enterprise-idp-frontend 3001:80 -n enterprise-idp-dev &
kubectl port-forward svc/enterprise-idp-backend 8080:80 -n enterprise-idp-dev &

# Monitoring
kubectl port-forward svc/prometheus-stack-grafana 3000:80 -n monitoring &
kubectl port-forward svc/prometheus-stack-kube-prom-prometheus 9090:9090 -n monitoring &
kubectl port-forward svc/loki-stack 3100:3100 -n monitoring &

# Vault
kubectl port-forward svc/vault 8200:8200 -n vault &

# ArgoCD
kubectl port-forward svc/argocd-server 8081:443 -n argocd &
```

### Access URLs
| Service | URL | Credentials |
|---------|-----|-------------|
| Frontend | http://localhost:3001 | - |
| Backend API | http://localhost:8080/swagger | - |
| Grafana | http://localhost:3000 | admin / admin123 |
| Prometheus | http://localhost:9090 | - |
| Vault | http://localhost:8200 | token: dev-root-token |
| ArgoCD | https://localhost:8081 | admin / (see above) |

---

## Staging Deployment

```bash
# Set staging kubeconfig
export KUBECONFIG=~/.kube/staging-config

# Deploy
bash scripts/deploy.sh staging v1.0.0

# Verify
bash scripts/health-check.sh staging
```

---

## Production Deployment

### Pre-Production Checklist
[ ] All tests passing in CI

[ ] Security scan clean

[ ] Staging deployment verified

[ ] Vault secrets configured

[ ] TLS certificates ready

[ ] Resource quotas reviewed

[ ] Backup configured

[ ] Runbook updated

[ ] On-call engineer notified
### Deploy to Production
```bash
# Via GitHub Actions (recommended)
gh workflow run cd-pipeline.yml \
  -f environment=production \
  -f image_tag=v1.0.0

# Or manually
export KUBECONFIG=~/.kube/production-config
bash scripts/deploy.sh production v1.0.0
```

### Rollback
```bash
# Rollback to previous release
bash scripts/rollback.sh production

# Rollback to specific revision
bash scripts/rollback.sh production 3
```

---

## Terraform Infrastructure

```bash
cd infrastructure/terraform/environments/dev

# Initialize
terraform init

# Plan
terraform plan -var-file=terraform.tfvars

# Apply
terraform apply -var-file=terraform.tfvars -auto-approve

# Destroy (dev only)
terraform destroy -var-file=terraform.tfvars
```

---

## Troubleshooting

### Pods not starting
```bash
kubectl describe pod <pod-name> -n enterprise-idp-dev
kubectl logs <pod-name> -n enterprise-idp-dev --previous
```

### Database connection issues
```bash
kubectl exec -it <backend-pod> -n enterprise-idp-dev -- \
  curl http://enterprise-idp-postgresql:5432
```

### ArgoCD sync failed
```bash
kubectl get application -n argocd
kubectl describe application enterprise-idp -n argocd
argocd app sync enterprise-idp --force
```

### Vault secrets not injected
```bash
kubectl logs <pod-name> -c vault-agent-init -n enterprise-idp-dev
kubectl describe pod <pod-name> -n enterprise-idp-dev | grep vault
```
