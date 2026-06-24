# Enterprise IDP — Vault Policies

# Admin Policy — Full Access
path "secret/*" {
  capabilities = ["create", "read", "update", "delete", "list"]
}

path "sys/*" {
  capabilities = ["create", "read", "update", "delete", "list"]
}

path "auth/*" {
  capabilities = ["create", "read", "update", "delete", "list"]
}

---

# Platform Engineer Policy
path "secret/data/idp/*" {
  capabilities = ["create", "read", "update", "delete", "list"]
}

path "secret/data/services/*" {
  capabilities = ["create", "read", "update", "list"]
}

path "secret/metadata/*" {
  capabilities = ["read", "list"]
}

---

# Developer Policy — Read Only
path "secret/data/services/{{identity.entity.aliases.auth_kubernetes_cluster.metadata.service_account_namespace}}/*" {
  capabilities = ["read"]
}

path "secret/data/idp/dev/*" {
  capabilities = ["read"]
}
