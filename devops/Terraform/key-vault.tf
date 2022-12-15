resource "azurerm_key_vault" "stage" {
  name                = "${local.resource_prefix}-kv"
  resource_group_name = azurerm_resource_group.stage.name
  location            = azurerm_resource_group.stage.location
  tenant_id           = data.azurerm_client_config.current.tenant_id
  sku_name            = "standard"
  tags                = local.default_tags
}

resource "azurerm_key_vault_access_policy" "stage_admins" {
  count        = length(data.azuread_users.key_vault_admins.object_ids)
  key_vault_id = azurerm_key_vault.stage.id
  tenant_id    = data.azurerm_client_config.current.tenant_id
  object_id    = data.azuread_users.key_vault_admins.object_ids[count.index]
  secret_permissions = [
    "Get",
    "List",
    "Set",
    "Delete",
    "Purge"
  ]
}

resource "azurerm_key_vault_access_policy" "ci_service" {
  key_vault_id = azurerm_key_vault.stage.id
  tenant_id    = data.azurerm_client_config.current.tenant_id
  object_id    = data.azurerm_client_config.current.object_id
  secret_permissions = [
    "Get",
    "List",
    "Set",
    "Delete",
    "Purge" # This is required when a secret has to be removed
  ]
}

data "azuread_users" "key_vault_admins" {
  user_principal_names = var.key_vault_admins
}
