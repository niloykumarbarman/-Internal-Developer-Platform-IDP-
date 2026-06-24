# --- General ---
variable "environment" {
  description = "Deployment environment (dev, staging, production)"
  type        = string
  validation {
    condition     = contains(["dev", "staging", "production"], var.environment)
    error_message = "Environment must be dev, staging, or production."
  }
}

variable "app_name" {
  description = "Application name"
  type        = string
  default     = "enterprise-idp"
}

variable "namespace" {
  description = "Kubernetes namespace"
  type        = string
  default     = "enterprise-idp"
}

# --- Kubernetes ---
variable "kubeconfig_path" {
  description = "Path to kubeconfig file"
  type        = string
  default     = "~/.kube/config"
}

variable "kube_context" {
  description = "Kubernetes context to use"
  type        = string
  default     = "docker-desktop"
}

variable "replicas" {
  description = "Number of replicas"
  type        = number
  default     = 2
}

variable "image_tag" {
  description = "Docker image tag to deploy"
  type        = string
  default     = "latest"
}

variable "resource_limits" {
  description = "Kubernetes resource limits"
  type = object({
    cpu    = string
    memory = string
  })
  default = {
    cpu    = "500m"
    memory = "512Mi"
  }
}

variable "resource_requests" {
  description = "Kubernetes resource requests"
  type = object({
    cpu    = string
    memory = string
  })
  default = {
    cpu    = "250m"
    memory = "256Mi"
  }
}

# --- Database ---
variable "postgres_host" {
  description = "PostgreSQL host"
  type        = string
  default     = "localhost"
}

variable "postgres_port" {
  description = "PostgreSQL port"
  type        = number
  default     = 5432
}

variable "postgres_admin_user" {
  description = "PostgreSQL admin username"
  type        = string
  default     = "postgres"
}

variable "postgres_admin_password" {
  description = "PostgreSQL admin password"
  type        = string
  sensitive   = true
}

variable "postgres_ssl_mode" {
  description = "PostgreSQL SSL mode"
  type        = string
  default     = "disable"
}

variable "app_db_name" {
  description = "Application database name"
  type        = string
  default     = "enterprise_idp"
}

variable "app_db_user" {
  description = "Application database user"
  type        = string
  default     = "idp_user"
}

variable "app_db_password" {
  description = "Application database password"
  type        = string
  sensitive   = true
}

# --- Vault ---
variable "vault_address" {
  description = "HashiCorp Vault address"
  type        = string
  default     = "http://localhost:8200"
}

variable "vault_token" {
  description = "HashiCorp Vault root token"
  type        = string
  sensitive   = true
}

# --- GitHub ---
variable "github_token" {
  description = "GitHub Personal Access Token"
  type        = string
  sensitive   = true
}

variable "github_org" {
  description = "GitHub organization name"
  type        = string
  default     = "enterprise-idp"
}

# --- Monitoring ---
variable "grafana_admin_password" {
  description = "Grafana admin password"
  type        = string
  sensitive   = true
  default     = "admin123"
}
