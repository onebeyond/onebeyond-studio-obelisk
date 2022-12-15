variable "key_vault_secret" {
  type = object({
    key_vault_id          = string
    key_vault_secret_name = string
  })
  description = "Key Vault secret to put the password in."
}
