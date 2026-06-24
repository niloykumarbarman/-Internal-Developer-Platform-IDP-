variable "image_tag" {
  type    = string
  default = "latest"
}
variable "postgres_host" {
  type    = string
  default = "localhost"
}
variable "postgres_admin_user" {
  type    = string
  default = "postgres"
}
variable "postgres_admin_password" {
  type      = string
  sensitive = true
}
variable "app_db_password" {
  type      = string
  sensitive = true
}
variable "vault_token" {
  type      = string
  sensitive = true
  default   = "dev-root-token"
}
variable "github_token" {
  type      = string
  sensitive = true
  default   = ""
}
variable "github_org" {
  type    = string
  default = "enterprise-idp"
}
