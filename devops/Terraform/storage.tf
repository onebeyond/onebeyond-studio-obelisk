resource "azurerm_storage_account" "stage" {
  name                             = "${replace(local.resource_prefix, "-", "")}storage"
  resource_group_name              = azurerm_resource_group.stage.name
  location                         = azurerm_resource_group.stage.location
  account_tier                     = "Standard"
  account_replication_type         = local.stage_code == "prod" ? "GRS" : "LRS"
  min_tls_version                  = "TLS1_2"
  allow_nested_items_to_be_public  = false
  cross_tenant_replication_enabled = false
  tags                             = local.default_tags
}

resource "azurerm_key_vault_secret" "storage_connection_string" {
  depends_on   = [azurerm_key_vault_access_policy.ci_service]
  key_vault_id = azurerm_key_vault.stage.id
  name         = local.key_vault_secret_names.storage_connection_string
  value        = azurerm_storage_account.stage.primary_connection_string
  tags         = local.default_tags
}
