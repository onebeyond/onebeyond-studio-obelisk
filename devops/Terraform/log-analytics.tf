resource "azurerm_log_analytics_workspace" "stage" {
  name                = "${local.resource_prefix}-log"
  resource_group_name = azurerm_resource_group.stage.name
  location            = azurerm_resource_group.stage.location
  sku                 = "PerGB2018"
  retention_in_days   = 30
  tags                = local.default_tags
}
