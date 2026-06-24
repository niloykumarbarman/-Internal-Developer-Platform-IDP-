variable "environment" {
  type = string
}
variable "namespace" {
  type    = string
  default = "enterprise-idp"
}
variable "app_name" {
  type    = string
  default = "enterprise-idp"
}
variable "replicas" {
  type    = number
  default = 2
}
variable "image_tag" {
  type    = string
  default = "latest"
}
variable "resource_limits" {
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
  type = object({
    cpu    = string
    memory = string
  })
  default = {
    cpu    = "250m"
    memory = "256Mi"
  }
}
