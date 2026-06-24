terraform {
  required_version = ">= 1.6.0"

  required_providers {
    kubernetes = {
      source  = "hashicorp/kubernetes"
      version = "~> 2.23"
    }
    helm = {
      source  = "hashicorp/helm"
      version = "~> 2.11"
    }
    postgresql = {
      source  = "cyrilgdn/postgresql"
      version = "~> 1.21"
    }
    vault = {
      source  = "hashicorp/vault"
      version = "~> 3.20"
    }
    github = {
      source  = "integrations/github"
      version = "~> 5.0"
    }
  }

  backend "kubernetes" {
    secret_suffix    = "terraform-state"
    namespace        = "enterprise-idp"
    load_config_file = true
  }
}

provider "kubernetes" {
  config_path    = var.kubeconfig_path
  config_context = var.kube_context
}

provider "helm" {
  kubernetes {
    config_path    = var.kubeconfig_path
    config_context = var.kube_context
  }
}

provider "postgresql" {
  host     = var.postgres_host
  port     = var.postgres_port
  username = var.postgres_admin_user
  password = var.postgres_admin_password
  sslmode  = var.postgres_ssl_mode
}

provider "vault" {
  address = var.vault_address
  token   = var.vault_token
}

provider "github" {
  token = var.github_token
  owner = var.github_org
}

# --- Modules ---

module "kubernetes" {
  source      = "./modules/kubernetes"
  environment = var.environment
  namespace   = var.namespace
  app_name    = var.app_name
  replicas    = var.replicas
  image_tag   = var.image_tag
  resource_limits   = var.resource_limits
  resource_requests = var.resource_requests
}

module "database" {
  source              = "./modules/database"
  environment         = var.environment
  postgres_host       = var.postgres_host
  postgres_admin_user = var.postgres_admin_user
  postgres_admin_password = var.postgres_admin_password
  app_db_name         = var.app_db_name
  app_db_user         = var.app_db_user
  app_db_password     = var.app_db_password
}

module "monitoring" {
  source      = "./modules/monitoring"
  environment = var.environment
  namespace   = var.namespace
  grafana_admin_password = var.grafana_admin_password
}

module "vault" {
  source        = "./modules/vault"
  environment   = var.environment
  vault_address = var.vault_address
  vault_token   = var.vault_token
  namespace     = var.namespace
}
