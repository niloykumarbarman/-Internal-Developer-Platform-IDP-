output "kv_path" {
  value = vault_mount.kv.path
}
output "vault_url" {
  value = "http://vault.${var.environment}.enterprise-idp.local"
}
output "idp_policy_name" {
  value = vault_policy.idp_policy.name
}
