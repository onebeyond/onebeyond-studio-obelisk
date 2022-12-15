output "result" {
  value = azurerm_key_vault_secret.sql_server_passwords.value
}

output "secret_name" {
  value = azurerm_key_vault_secret.sql_server_passwords.name
}
