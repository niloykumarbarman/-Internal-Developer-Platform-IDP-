variable "environment" {
  type = string
}
variable "postgres_host" {
  type = string
}
variable "postgres_admin_user" {
  type = string
}
variable "postgres_admin_password" {
  type      = string
  sensitive = true
}
variable "app_db_name" {
  type    = string
  default = "enterprise_idp"
}
variable "app_db_user" {
  type    = string
  default = "idp_user"
}
variable "app_db_password" {
  type      = string
  sensitive = true
}
