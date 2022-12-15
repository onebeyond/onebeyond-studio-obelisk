resource "azurerm_resource_group" "stage" {
  name     = "${local.resource_prefix}-rg"
  location = var.resource_location
  tags     = local.default_tags
}
