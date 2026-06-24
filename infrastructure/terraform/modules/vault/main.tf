resource "helm_release" "vault" {
  name             = "vault"
  repository       = "https://helm.releases.hashicorp.com"
  chart            = "vault"
  namespace        = "vault"
  create_namespace = true
  version          = "0.27.0"

  set {
    name  = "server.dev.enabled"
    value = var.environment == "dev" ? "true" : "false"
  }
  set {
    name  = "server.ha.enabled"
    value = var.environment == "production" ? "true" : "false"
  }
  set {
    name  = "server.ha.replicas"
    value = var.environment == "production" ? "3" : "1"
  }
  set {
    name  = "ui.enabled"
    value = "true"
  }
  set {
    name  = "ui.serviceType"
    value = "ClusterIP"
  }
  set {
    name  = "injector.enabled"
    value = "true"
  }
  set {
    name  = "server.ingress.enabled"
    value = "true"
  }
  set {
    name  = "server.ingress.hosts[0].host"
    value = "vault.${var.environment}.enterprise-idp.local"
  }
}

resource "vault_mount" "kv" {
  path        = "secret"
  type        = "kv"
  options     = { version = "2" }
  description = "KV Version 2 secret engine for Enterprise IDP"

  depends_on = [helm_release.vault]
}

resource "vault_mount" "database" {
  path        = "database"
  type        = "database"
  description = "Database secret engine for Enterprise IDP"

  depends_on = [helm_release.vault]
}

resource "vault_kv_secret_v2" "app_secrets" {
  mount = vault_mount.kv.path
  name  = "${var.environment}/enterprise-idp"

  data_json = jsonencode({
    JWT_SECRET         = var.jwt_secret
    DB_PASSWORD        = var.db_password
    GITHUB_TOKEN       = var.github_token
    REDIS_PASSWORD     = var.redis_password
    SMTP_PASSWORD      = var.smtp_password
  })

  depends_on = [vault_mount.kv]
}

resource "vault_policy" "idp_policy" {
  name = "enterprise-idp-${var.environment}"

  policy = <<EOT
path "secret/data/${var.environment}/enterprise-idp" {
  capabilities = ["read", "list"]
}
path "secret/metadata/${var.environment}/enterprise-idp" {
  capabilities = ["read", "list"]
}
path "database/creds/enterprise-idp-${var.environment}" {
  capabilities = ["read"]
}
path "auth/token/renew-self" {
  capabilities = ["update"]
}
EOT

  depends_on = [helm_release.vault]
}

resource "vault_auth_backend" "kubernetes" {
  type = "kubernetes"
  path = "kubernetes"

  depends_on = [helm_release.vault]
}

resource "vault_kubernetes_auth_backend_role" "idp" {
  backend                          = vault_auth_backend.kubernetes.path
  role_name                        = "enterprise-idp"
  bound_service_account_names      = ["enterprise-idp"]
  bound_service_account_namespaces = [var.namespace]
  token_ttl                        = 3600
  token_policies                   = [vault_policy.idp_policy.name]

  depends_on = [vault_auth_backend.kubernetes]
}
