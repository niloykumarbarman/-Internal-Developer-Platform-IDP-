terraform {
  required_version = ">= 1.6.0"
  backend "local" {
    path = "terraform.tfstate"
  }
}

module "enterprise_idp" {
  source = "../../"

  environment    = "dev"
  app_name       = "enterprise-idp"
  namespace      = "enterprise-idp-dev"
  replicas       = 1
  image_tag      = var.image_tag
  kube_context   = "docker-desktop"

  resource_limits = {
    cpu    = "500m"
    memory = "512Mi"
  }
  resource_requests = {
    cpu    = "100m"
    memory = "128Mi"
  }

  postgres_host           = var.postgres_host
  postgres_admin_user     = var.postgres_admin_user
  postgres_admin_password = var.postgres_admin_password
  app_db_name             = "enterprise_idp_dev"
  app_db_user             = "idp_dev_user"
  app_db_password         = var.app_db_password

  vault_address = "http://localhost:8200"
  vault_token   = var.vault_token

  github_token = var.github_token
  github_org   = var.github_org

  grafana_admin_password = "admin123"
}
