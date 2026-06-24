resource "kubernetes_namespace" "app" {
  metadata {
    name = var.namespace
    labels = {
      app         = var.app_name
      environment = var.environment
      managed-by  = "terraform"
    }
  }
}

resource "kubernetes_resource_quota" "app" {
  metadata {
    name      = "${var.app_name}-quota"
    namespace = kubernetes_namespace.app.metadata[0].name
  }
  spec {
    hard = {
      "requests.cpu"    = "4"
      "requests.memory" = "4Gi"
      "limits.cpu"      = "8"
      "limits.memory"   = "8Gi"
      "pods"            = "20"
    }
  }
}

resource "kubernetes_deployment" "app" {
  metadata {
    name      = var.app_name
    namespace = kubernetes_namespace.app.metadata[0].name
    labels = {
      app         = var.app_name
      environment = var.environment
      version     = var.image_tag
    }
  }
  spec {
    replicas = var.replicas
    selector {
      match_labels = {
        app = var.app_name
      }
    }
    template {
      metadata {
        labels = {
          app         = var.app_name
          environment = var.environment
          version     = var.image_tag
        }
        annotations = {
          "prometheus.io/scrape" = "true"
          "prometheus.io/port"   = "8080"
          "prometheus.io/path"   = "/metrics"
        }
      }
      spec {
        container {
          name  = var.app_name
          image = "ghcr.io/enterprise-idp/backend:${var.image_tag}"
          port {
            container_port = 8080
          }
          resources {
            limits = {
              cpu    = var.resource_limits.cpu
              memory = var.resource_limits.memory
            }
            requests = {
              cpu    = var.resource_requests.cpu
              memory = var.resource_requests.memory
            }
          }
          liveness_probe {
            http_get {
              path = "/health"
              port = 8080
            }
            initial_delay_seconds = 30
            period_seconds        = 10
          }
          readiness_probe {
            http_get {
              path = "/health/ready"
              port = 8080
            }
            initial_delay_seconds = 15
            period_seconds        = 5
          }
        }
      }
    }
  }
}

resource "kubernetes_service" "app" {
  metadata {
    name      = var.app_name
    namespace = kubernetes_namespace.app.metadata[0].name
  }
  spec {
    selector = {
      app = var.app_name
    }
    port {
      port        = 80
      target_port = 8080
    }
    type = "ClusterIP"
  }
}

resource "kubernetes_horizontal_pod_autoscaler_v2" "app" {
  metadata {
    name      = "${var.app_name}-hpa"
    namespace = kubernetes_namespace.app.metadata[0].name
  }
  spec {
    scale_target_ref {
      api_version = "apps/v1"
      kind        = "Deployment"
      name        = kubernetes_deployment.app.metadata[0].name
    }
    min_replicas = var.replicas
    max_replicas = var.replicas * 3
    metric {
      type = "Resource"
      resource {
        name = "cpu"
        target {
          type                = "Utilization"
          average_utilization = 70
        }
      }
    }
    metric {
      type = "Resource"
      resource {
        name = "memory"
        target {
          type                = "Utilization"
          average_utilization = 80
        }
      }
    }
  }
}
