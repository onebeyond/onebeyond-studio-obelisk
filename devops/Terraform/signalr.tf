resource "azurerm_signalr_service" "signalr" {
    name = "${local.resource_prefix}-signalr"
    location = azurerm_resource_group.stage.location
    resource_group_name = azurerm_resource_group.stage.name
    service_mode = "Serverless"
    sku {
        name = var.signalr_service_sku
        capacity = 1
    }
    cors {
      allowed_origins = var.allow_dev ? concat([var.spa_url], var.dev_urls) : [var.spa_url]
    }
    tags = local.default_tags
}