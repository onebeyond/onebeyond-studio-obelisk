resource "azurerm_service_plan" "function_plan" {
  name                = "${var.name_prefix}-plan"
  resource_group_name = var.resource_group.name
  location            = var.resource_group.location
  os_type             = "Linux"
  sku_name            = "FC1"
  tags                = var.tags
}
# Explicitly sets a separate storage account for handling the jobs. This
# is not strictly a requirement, but it keeps it separate and seems to work
# better. Azure Storage is pay-as-you-go so more accounts is just segregation
resource "azurerm_storage_account" "store" {
  name = "${var.stage_code}wrkjobsstrg"
  resource_group_name = azurerm_resource_group.env.name
  location = azurerm_resource_group.env.location
  account_tier = "Standard"
  account_replication_type = "LRS"
  min_tls_version = "TLS1_2"
  allow_nested_items_to_be_public = true
  cross_tenant_replication_enabled = false
  tags = var.tags
}

resource "azurerm_storage_container" "store" {
  name                  = "worker-container"
  storage_account_id    = azurerm_storage_account.store.id
  container_access_type = "private"
}

resource "azurerm_function_app_flex_consumption" "function" {
  runtime_name = "dotnet-isolated"
  runtime_version = "10.0"
  storage_access_key = azurerm_storage_account.store.primary_access_key
  storage_container_type = "blobContainer"
  storage_container_endpoint = "${azurerm_storage_account.store.primary_blob_endpoint}${azurerm_storage_container.store.name}"
  storage_authentication_type = "StorageAccountConnectionString"
  name                        = "${var.name_prefix}-func"
  resource_group_name         = azurerm_service_plan.function_plan.resource_group_name
  location                    = azurerm_service_plan.function_plan.location
  service_plan_id             = azurerm_service_plan.function_plan.id
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
    http2_enabled                          = true
    managed_pipeline_mode                  = "Integrated"
    minimum_tls_version                    = "1.2"
    use_32_bit_worker                      = false
    websockets_enabled                     = true
    application_insights_key               = var.application_insights.key
    application_insights_connection_string = var.application_insights.connection_string    
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
  tenant_id    = azurerm_function_app_flex_consumption.function.identity[0].tenant_id
  object_id    = azurerm_function_app_flex_consumption.function.identity[0].principal_id
  secret_permissions = [
    "Get"
  ]
}
