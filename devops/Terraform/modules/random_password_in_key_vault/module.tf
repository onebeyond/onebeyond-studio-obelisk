resource "random_password" "sql_server_password" {
  length           = 16
  special          = true
  min_lower        = 1
  min_numeric      = 1
  min_upper        = 1
  min_special      = 1
  override_special = "!#$%*-_=+?@.[]()"

  lifecycle {
    ignore_changes = [
      length
    ]
  }
}

resource "azurerm_key_vault_secret" "sql_server_passwords" {
  key_vault_id = var.key_vault_secret.key_vault_id
  name         = var.key_vault_secret.key_vault_secret_name
  value        = random_password.sql_server_password.result
}
