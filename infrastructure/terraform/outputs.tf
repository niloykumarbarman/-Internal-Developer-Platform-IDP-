output "namespace" {
  description = "Kubernetes namespace"
  value       = module.kubernetes.namespace
}

output "deployment_name" {
  description = "Kubernetes deployment name"
  value       = module.kubernetes.deployment_name
}

output "service_url" {
  description = "Application service URL"
  value       = module.kubernetes.service_url
}

output "database_name" {
  description = "PostgreSQL database name"
  value       = module.database.database_name
}

output "database_user" {
  description = "PostgreSQL database user"
  value       = module.database.database_user
  sensitive   = true
}

output "vault_kv_path" {
  description = "Vault KV secrets path"
  value       = module.vault.kv_path
}

output "grafana_url" {
  description = "Grafana dashboard URL"
  value       = module.monitoring.grafana_url
}

output "prometheus_url" {
  description = "Prometheus URL"
  value       = module.monitoring.prometheus_url
}
