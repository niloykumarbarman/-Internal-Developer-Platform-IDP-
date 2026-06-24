resource "helm_release" "prometheus_stack" {
  name             = "prometheus-stack"
  repository       = "https://prometheus-community.github.io/helm-charts"
  chart            = "kube-prometheus-stack"
  namespace        = "monitoring"
  create_namespace = true
  version          = "55.5.0"

  set {
    name  = "grafana.adminPassword"
    value = var.grafana_admin_password
  }
  set {
    name  = "grafana.enabled"
    value = "true"
  }
  set {
    name  = "prometheus.enabled"
    value = "true"
  }
  set {
    name  = "alertmanager.enabled"
    value = "true"
  }
  set {
    name  = "grafana.ingress.enabled"
    value = "true"
  }
  set {
    name  = "grafana.ingress.hosts[0]"
    value = "grafana.${var.environment}.enterprise-idp.local"
  }
  set {
    name  = "prometheus.prometheusSpec.retention"
    value = "30d"
  }
  set {
    name  = "prometheus.prometheusSpec.storageSpec.volumeClaimTemplate.spec.resources.requests.storage"
    value = "20Gi"
  }
}

resource "helm_release" "loki_stack" {
  name             = "loki-stack"
  repository       = "https://grafana.github.io/helm-charts"
  chart            = "loki-stack"
  namespace        = "monitoring"
  create_namespace = true
  version          = "2.9.11"

  set {
    name  = "loki.enabled"
    value = "true"
  }
  set {
    name  = "promtail.enabled"
    value = "true"
  }
  set {
    name  = "loki.persistence.enabled"
    value = "true"
  }
  set {
    name  = "loki.persistence.size"
    value = "10Gi"
  }
}

resource "helm_release" "otel_collector" {
  name             = "otel-collector"
  repository       = "https://open-telemetry.github.io/opentelemetry-helm-charts"
  chart            = "opentelemetry-collector"
  namespace        = "monitoring"
  create_namespace = true
  version          = "0.73.1"

  set {
    name  = "mode"
    value = "deployment"
  }
}

resource "helm_release" "jaeger" {
  name             = "jaeger"
  repository       = "https://jaegertracing.github.io/helm-charts"
  chart            = "jaeger"
  namespace        = "monitoring"
  create_namespace = true
  version          = "0.71.14"

  set {
    name  = "provisionDataStore.cassandra"
    value = "false"
  }
  set {
    name  = "allInOne.enabled"
    value = "true"
  }
  set {
    name  = "storage.type"
    value = "memory"
  }
  set {
    name  = "query.ingress.enabled"
    value = "true"
  }
  set {
    name  = "query.ingress.hosts[0]"
    value = "jaeger.${var.environment}.enterprise-idp.local"
  }
}
