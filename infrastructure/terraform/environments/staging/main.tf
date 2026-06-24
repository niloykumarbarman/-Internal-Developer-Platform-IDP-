terraform {
  required_version = ">= 1.6.0"
  backend "local" {
    path = "terraform.tfstate"
  }
}

module "enterprise_idp" {
  source = "../../"

  environment    = "staging"
  app_name       = "enterprise-idp"
  namespace      = "enterprise-idp-staging"
  replicas       = 2
  image_tag      = var.image_tag
  kube_context   = "staging-cluster"

  resource_limits = {
    cpu    = "1000m"
    memory = "1Gi"
  }
  resource_requests = {
    cpu    = "250m"
    memory = "256Mi"
  }

  postgres_host           = var.postgres_host
  postgres_admin_user     = var.postgres_admin_user
  postgres_admin_password = var.postgres_admin_password
  app_db_name             = "enterprise_idp_staging"
  app_db_user             = "idp_staging_user"
  app_db_password         = var.app_db_password

  vault_address = var.vault_address
  vault_token   = var.vault_token

  github_token = var.github_token
  github_org   = var.github_org

  grafana_admin_password = var.grafana_admin_password
}
