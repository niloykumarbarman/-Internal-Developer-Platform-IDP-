#!/bin/bash
set -e

VAULT_ADDR=${VAULT_ADDR:-"http://vault:8200"}
VAULT_TOKEN=${VAULT_TOKEN:-"root"}
K8S_HOST=${K8S_HOST:-"https://kubernetes.default.svc"}

echo "🔐 Setting up HashiCorp Vault for Enterprise IDP..."

export VAULT_ADDR=$VAULT_ADDR
export VAULT_TOKEN=$VAULT_TOKEN

# Enable KV v2 secrets engine
echo "📦 Enabling KV v2 secrets engine..."
vault secrets enable -path=secret kv-v2 || echo "KV v2 already enabled"

# Enable Kubernetes auth method
echo "☸️  Enabling Kubernetes auth method..."
vault auth enable kubernetes || echo "Kubernetes auth already enabled"

# Configure Kubernetes auth
echo "⚙️  Configuring Kubernetes auth..."
vault write auth/kubernetes/config \
    kubernetes_host="$K8S_HOST" \
    kubernetes_ca_cert=@/var/run/secrets/kubernetes.io/serviceaccount/ca.crt \
    token_reviewer_jwt=@/var/run/secrets/kubernetes.io/serviceaccount/token

# Create policies
echo "📋 Creating Vault policies..."

# Admin policy
vault policy write idp-admin - << 'EOF'
path "secret/*" {
  capabilities = ["create", "read", "update", "delete", "list"]
}
path "sys/health" {
  capabilities = ["read"]
}
path "auth/*" {
  capabilities = ["create", "read", "update", "delete", "list"]
}
EOF

# Platform Engineer policy
vault policy write idp-platform-engineer - << 'EOF'
path "secret/data/idp/*" {
  capabilities = ["create", "read", "update", "delete", "list"]
}
path "secret/data/services/*" {
  capabilities = ["create", "read", "update", "list"]
}
path "secret/metadata/*" {
  capabilities = ["read", "list"]
}
EOF

# Developer policy
vault policy write idp-developer - << 'EOF'
path "secret/data/services/+/dev/*" {
  capabilities = ["read"]
}
path "secret/data/idp/dev/*" {
  capabilities = ["read"]
}
EOF

# Create Kubernetes auth roles
echo "🎭 Creating Kubernetes auth roles..."

vault write auth/kubernetes/role/idp-backend \
    bound_service_account_names=enterprise-idp-backend \
    bound_service_account_namespaces=enterprise-idp \
    policies=idp-admin \
    ttl=1h

vault write auth/kubernetes/role/idp-platform-engineer \
    bound_service_account_names=platform-engineer \
    bound_service_account_namespaces=enterprise-idp \
    policies=idp-platform-engineer \
    ttl=1h

vault write auth/kubernetes/role/idp-developer \
    bound_service_account_names=developer \
    bound_service_account_namespaces=enterprise-idp \
    policies=idp-developer \
    ttl=1h

# Seed initial secrets
echo "🌱 Seeding initial secrets..."

vault kv put secret/idp/database \
    host="postgresql:5432" \
    name="enterpriseidp" \
    username="idpuser" \
    password="$(openssl rand -base64 32)"

vault kv put secret/idp/redis \
    host="redis:6379" \
    password="$(openssl rand -base64 32)"

vault kv put secret/idp/jwt \
    secret="$(openssl rand -base64 64)" \
    issuer="EnterpriseIDP" \
    audience="EnterpriseIDP"

vault kv put secret/idp/github \
    client_id="REPLACE_WITH_GITHUB_CLIENT_ID" \
    client_secret="REPLACE_WITH_GITHUB_CLIENT_SECRET" \
    webhook_secret="$(openssl rand -base64 32)"

echo ""
echo "✅ Vault setup complete!"
echo "📊 Summary:"
echo "   - KV v2 secrets engine: enabled at secret/"
echo "   - Kubernetes auth: enabled and configured"
echo "   - Policies: idp-admin, idp-platform-engineer, idp-developer"
echo "   - K8s Roles: idp-backend, idp-platform-engineer, idp-developer"
echo "   - Initial secrets seeded"
