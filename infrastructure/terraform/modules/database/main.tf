resource "postgresql_database" "app" {
  name              = var.app_db_name
  owner             = postgresql_role.app.name
  encoding          = "UTF8"
  lc_collate        = "en_US.UTF-8"
  lc_ctype          = "en_US.UTF-8"
  connection_limit  = 100
  allow_connections = true
}

resource "postgresql_role" "app" {
  name     = var.app_db_user
  login    = true
  password = var.app_db_password
  encrypted_password = true
}

resource "postgresql_grant" "app" {
  database    = postgresql_database.app.name
  role        = postgresql_role.app.name
  object_type = "database"
  privileges  = ["CONNECT", "CREATE", "TEMPORARY"]
}

resource "postgresql_database" "keycloak" {
  name              = "${var.app_db_name}_keycloak"
  owner             = postgresql_role.app.name
  encoding          = "UTF8"
  connection_limit  = 50
  allow_connections = true
}

resource "postgresql_database" "vault" {
  name              = "${var.app_db_name}_vault"
  owner             = postgresql_role.app.name
  encoding          = "UTF8"
  connection_limit  = 50
  allow_connections = true
}
