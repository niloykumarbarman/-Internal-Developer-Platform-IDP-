output "grafana_url" {
  value = "http://grafana.${var.environment}.enterprise-idp.local"
}
output "prometheus_url" {
  value = "http://prometheus.${var.environment}.enterprise-idp.local"
}
output "jaeger_url" {
  value = "http://jaeger.${var.environment}.enterprise-idp.local"
}
output "loki_url" {
  value = "http://loki.monitoring.svc.cluster.local:3100"
}
