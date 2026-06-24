variable "environment" {
  type = string
}
variable "vault_address" {
  type = string
}
variable "vault_token" {
  type      = string
  sensitive = true
}
variable "namespace" {
  type    = string
  default = "enterprise-idp"
}
variable "jwt_secret" {
  type      = string
  sensitive = true
  default   = "change-me-in-production"
}
variable "db_password" {
  type      = string
  sensitive = true
  default   = "change-me-in-production"
}
variable "github_token" {
  type      = string
  sensitive = true
  default   = ""
}
variable "redis_password" {
  type      = string
  sensitive = true
  default   = "change-me-in-production"
}
variable "smtp_password" {
  type      = string
  sensitive = true
  default   = ""
}
