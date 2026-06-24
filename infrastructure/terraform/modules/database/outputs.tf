output "database_name" {
  value = postgresql_database.app.name
}
output "database_user" {
  value     = postgresql_role.app.name
  sensitive = true
}
output "keycloak_db_name" {
  value = postgresql_database.keycloak.name
}
output "vault_db_name" {
  value = postgresql_database.vault.name
}
