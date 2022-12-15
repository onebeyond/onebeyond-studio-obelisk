resource "azurerm_mssql_server" "stage" {
  name                          = "${local.resource_prefix}-sql"
  resource_group_name           = azurerm_resource_group.stage.name
  location                      = azurerm_resource_group.stage.location
  version                       = "12.0"
  administrator_login           = "${local.project}-admin"
  administrator_login_password  = module.sql_admin_password.result
  minimum_tls_version           = "1.2"
  tags                          = local.default_tags
  public_network_access_enabled = true

  lifecycle {
    prevent_destroy = false
  }
}

module "sql_admin_password" {
  depends_on = [azurerm_key_vault_access_policy.ci_service]
  source     = "./modules/random_password_in_key_vault"

  key_vault_secret = {
    key_vault_id          = azurerm_key_vault.stage.id
    key_vault_secret_name = local.key_vault_secret_names.sql_admin_password
  }
}

resource "azurerm_mssql_firewall_rule" "azure_resources" {
  name             = "Azure Resources"
  server_id        = azurerm_mssql_server.stage.id
  start_ip_address = "0.0.0.0" # Documented here https://docs.microsoft.com/en-us/rest/api/sql/2021-02-01-preview/firewall-rules/create-or-update
  end_ip_address   = "0.0.0.0"
}

resource "azurerm_mssql_firewall_rule" "allowed_ips" {
  count            = length(var.sql_allowed_ips)
  name             = "DCSL.${keys(var.sql_allowed_ips)[count.index]}"
  server_id        = azurerm_mssql_server.stage.id
  start_ip_address = values(var.sql_allowed_ips)[count.index]
  end_ip_address   = values(var.sql_allowed_ips)[count.index]
}
