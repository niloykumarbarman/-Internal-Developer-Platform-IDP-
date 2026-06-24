variable "image_tag" {
  type = string
}
variable "postgres_host" {
  type = string
}
variable "postgres_admin_user" {
  type    = string
  default = "postgres"
}
variable "postgres_admin_password" {
  type      = string
  sensitive = true
}
variable "postgres_ssl_mode" {
  type    = string
  default = "require"
}
variable "app_db_password" {
  type      = string
  sensitive = true
}
variable "vault_address" {
  type = string
}
variable "vault_token" {
  type      = string
  sensitive = true
}
variable "github_token" {
  type      = string
  sensitive = true
}
variable "github_org" {
  type = string
}
variable "grafana_admin_password" {
  type      = string
  sensitive = true
}
