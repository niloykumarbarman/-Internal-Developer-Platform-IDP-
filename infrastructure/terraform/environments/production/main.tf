terraform {
  required_version = ">= 1.6.0"
  backend "kubernetes" {
    secret_suffix    = "terraform-state-prod"
    namespace        = "enterprise-idp"
    load_config_file = true
  }
}

module "enterprise_idp" {
  source = "../../"

  environment    = "production"
  app_name       = "enterprise-idp"
  namespace      = "enterprise-idp-production"
  replicas       = 3
  image_tag      = var.image_tag
  kube_context   = "production-cluster"

  resource_limits = {
    cpu    = "2000m"
    memory = "2Gi"
  }
  resource_requests = {
    cpu    = "500m"
    memory = "512Mi"
  }

  postgres_host           = var.postgres_host
  postgres_admin_user     = var.postgres_admin_user
  postgres_admin_password = var.postgres_admin_password
  app_db_name             = "enterprise_idp_production"
  app_db_user             = "idp_prod_user"
  app_db_password         = var.app_db_password
  postgres_ssl_mode       = "require"

  vault_address = var.vault_address
  vault_token   = var.vault_token

  github_token = var.github_token
  github_org   = var.github_org

  grafana_admin_password = var.grafana_admin_password
}
