resource "azurerm_service_plan" "function_plan" {
  name                = "${var.name_prefix}-plan"
  resource_group_name = var.resource_group.name
  location            = var.resource_group.location
  os_type             = "Linux"
  sku_name            = var.sku_name
  tags                = var.tags
}

resource "azurerm_linux_function_app" "function" {
  name                        = "${var.name_prefix}-func"
  resource_group_name         = azurerm_service_plan.function_plan.resource_group_name
  location                    = azurerm_service_plan.function_plan.location
  service_plan_id             = azurerm_service_plan.function_plan.id
  functions_extension_version = "~4"
  https_only                  = true
  storage_account_name        = var.storage_account.name
  storage_account_access_key  = var.storage_account.access_key
  app_settings                = var.app_settings # It can be merged with some predefined ones if needed
  tags                        = var.tags

  dynamic "connection_string" {
    for_each = var.connection_strings
    iterator = connection_string
    content {
      name  = connection_string.key
      type  = connection_string.value.type
      value = connection_string.value.value
    }
  }

  site_config {
    always_on                              = true # required to be true for non-HTTP triggers to always work
    ftps_state                             = "Disabled"
    http2_enabled                          = true
    managed_pipeline_mode                  = "Integrated"
    minimum_tls_version                    = "1.2"
    use_32_bit_worker                      = false
    websockets_enabled                     = true
    application_insights_key               = var.application_insights.key
    application_insights_connection_string = var.application_insights.connection_string
    application_stack {
      dotnet_version              = "7.0"
      use_dotnet_isolated_runtime = true
    }
  }

  identity {
    type = "SystemAssigned"
  }

  lifecycle {
    ignore_changes = [
      app_settings["AZURE_FUNCTIONS_ENVIRONMENT"],
      app_settings["WEBSITE_ENABLE_SYNC_UPDATE_SITE"],
      tags["hidden-link: /app-insights-instrumentation-key"],
      tags["hidden-link: /app-insights-resource-id"]
    ]
  }
}

resource "azurerm_key_vault_access_policy" "function" {
  key_vault_id = var.key_vault_id
  tenant_id    = azurerm_linux_function_app.function.identity[0].tenant_id
  object_id    = azurerm_linux_function_app.function.identity[0].principal_id
  secret_permissions = [
    "Get"
  ]
}
