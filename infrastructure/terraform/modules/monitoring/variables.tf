variable "environment" {
  type = string
}
variable "namespace" {
  type    = string
  default = "enterprise-idp"
}
variable "grafana_admin_password" {
  type      = string
  sensitive = true
  default   = "admin123"
}
