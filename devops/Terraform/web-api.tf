resource "azurerm_service_plan" "web_api" {
  name                = "${local.resource_prefix}-plan"
  resource_group_name = azurerm_resource_group.stage.name
  location            = azurerm_resource_group.stage.location
  os_type             = "Linux"
  sku_name            = var.web_api_sku_size
  tags                = local.default_tags
}

resource "azurerm_linux_web_app" "web_api" {
  name                    = "${local.resource_prefix}-app"
  resource_group_name     = azurerm_resource_group.stage.name
  location                = azurerm_resource_group.stage.location
  service_plan_id         = azurerm_service_plan.web_api.id
  https_only              = true
  client_affinity_enabled = false
  tags                    = local.default_tags
  app_settings = {
    APPINSIGHTS_INSTRUMENTATIONKEY                    = azurerm_application_insights.web_api.instrumentation_key
    APPLICATIONINSIGHTS_CONNECTION_STRING             = azurerm_application_insights.web_api.connection_string
    ApplicationInsightsAgent_EXTENSION_VERSION        = "~2"
    InstrumentationEngine_EXTENSION_VERSION           = "~1"
    XDT_MicrosoftApplicationInsights_BaseExtensions   = "~1"
    XDT_MicrosoftApplicationInsights_Mode             = "recommended"
    "DomainEvents__Queue__ConnectionString"           = "@Microsoft.KeyVault(VaultName=${azurerm_key_vault.stage.name};SecretName=${azurerm_key_vault_secret.storage_connection_string.name})"
    "FileStorage__AzureBlobStorage__ConnectionString" = "@Microsoft.KeyVault(VaultName=${azurerm_key_vault.stage.name};SecretName=${azurerm_key_vault_secret.storage_connection_string.name})"
    "Identities__Seeding__AdminPassword"              = "@Microsoft.KeyVault(VaultName=${azurerm_key_vault.stage.name};SecretName=${module.web_api_admin_password.secret_name})"
  }
  connection_string {
    name  = "ApplicationConnectionString"
    type  = "SQLAzure"
    value = "@Microsoft.KeyVault(VaultName=${azurerm_key_vault.stage.name};SecretName=${azurerm_key_vault_secret.web_api_sqldb_connection_string.name})"
  }
  site_config {
    always_on             = true
    ftps_state            = "Disabled"
    http2_enabled         = true
    managed_pipeline_mode = "Integrated"
    minimum_tls_version   = "1.2"
    use_32_bit_worker     = false
    websockets_enabled    = true
    health_check_path     = "/health/ready"
    application_stack {
      dotnet_version = "7.0"
    }
  }
  logs {
    detailed_error_messages = true
    application_logs {
      file_system_level = "Warning"
    }
    http_logs {
      file_system {
        retention_in_days = 7
        retention_in_mb   = 35
      }
    }
  }
  identity {
    type = "SystemAssigned"
  }
  lifecycle {
    ignore_changes = [
      app_settings["ASPNETCORE_ENVIRONMENT"],
      tags["hidden-link: /app-insights-conn-string"]
    ]
  }
}

module "web_api_hostname" {
  source = "./modules/app_service_hostname"

  app_service_name                = azurerm_linux_web_app.web_api.name
  app_service_resource_group_name = azurerm_linux_web_app.web_api.resource_group_name
  app_service_custom_hostname     = var.web_api_custom_domain
  app_service_default_hostname    = azurerm_linux_web_app.web_api.default_hostname
}

resource "azurerm_key_vault_access_policy" "web_api" {
  key_vault_id = azurerm_key_vault.stage.id
  tenant_id    = azurerm_linux_web_app.web_api.identity[0].tenant_id
  object_id    = azurerm_linux_web_app.web_api.identity[0].principal_id
  secret_permissions = [
    "Get"
  ]
}

resource "azurerm_application_insights" "web_api" {
  name                = "${local.resource_prefix}-api-appi"
  resource_group_name = azurerm_resource_group.stage.name
  location            = azurerm_resource_group.stage.location
  workspace_id        = azurerm_log_analytics_workspace.stage.id
  application_type    = "web"
  sampling_percentage = 0 # By default it is 100%, but it was 0% in Azure. Needs investigation
  tags                = local.default_tags
}

resource "azurerm_mssql_database" "web_api" {
  name                 = "${local.resource_prefix}-api-sqldb"
  server_id            = azurerm_mssql_server.stage.id
  collation            = "SQL_Latin1_General_CP1_CI_AS"
  max_size_gb          = var.web_api_sqldb_max_size_gb
  sku_name             = var.web_api_sqldb_sku_size
  storage_account_type = local.stage_code == "prod" ? "Geo" : "Local"
  tags                 = local.default_tags

  lifecycle {
    prevent_destroy = false
  }
}

module "web_api_sqldb_user_password" {
  depends_on = [azurerm_key_vault_access_policy.ci_service]
  source     = "./modules/random_password_in_key_vault"

  key_vault_secret = {
    key_vault_id          = azurerm_key_vault.stage.id
    key_vault_secret_name = local.key_vault_secret_names.web_api_sqldb_user_password
  }
}

resource "mssql_login" "web_api" {
  server {
    host = azurerm_mssql_server.stage.fully_qualified_domain_name
    login {
      username = azurerm_mssql_server.stage.administrator_login
      password = azurerm_mssql_server.stage.administrator_login_password
    }
  }
  login_name = "web-api-user"
  password   = module.web_api_sqldb_user_password.result
}

resource "mssql_user" "web_api" {
  server {
    host = azurerm_mssql_server.stage.fully_qualified_domain_name
    login {
      username = azurerm_mssql_server.stage.administrator_login
      password = azurerm_mssql_server.stage.administrator_login_password
    }
  }
  database   = azurerm_mssql_database.web_api.name
  username   = "web-api-user"
  login_name = mssql_login.web_api.login_name
  roles      = ["db_owner"]
}

resource "azurerm_key_vault_secret" "web_api_sqldb_connection_string" {
  depends_on   = [azurerm_key_vault_access_policy.ci_service]
  key_vault_id = azurerm_key_vault.stage.id
  name         = local.key_vault_secret_names.web_api_sqldb_connection_string
  value        = "Server=tcp:${azurerm_mssql_server.stage.fully_qualified_domain_name},1433;Initial Catalog=${azurerm_mssql_database.web_api.name};Persist Security Info=False;User ID=${mssql_user.web_api.username};Password=${mssql_login.web_api.password};MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  tags         = local.default_tags
}

module "web_api_admin_password" {
  depends_on = [azurerm_key_vault_access_policy.ci_service]
  source     = "./modules/random_password_in_key_vault"

  key_vault_secret = {
    key_vault_id          = azurerm_key_vault.stage.id
    key_vault_secret_name = local.key_vault_secret_names.web_api_admin_password
  }
}

module "web_api_workers" {
  source = "./modules/az_function"

  resource_group = {
    name     = azurerm_resource_group.stage.name
    location = azurerm_resource_group.stage.location
  }
  name_prefix = "${local.resource_prefix}-workers"
  storage_account = {
    name       = azurerm_storage_account.stage.name # Isolated function can't seem to use managed identity for its backend
    access_key = azurerm_storage_account.stage.primary_access_key
  }
  key_vault_id = azurerm_key_vault.stage.id
  application_insights = {
    key               = azurerm_application_insights.web_api.instrumentation_key
    connection_string = azurerm_application_insights.web_api.connection_string
  }
  app_settings = {
    "DomainEvents_Queue_ConnectionString" = "@Microsoft.KeyVault(VaultName=${azurerm_key_vault.stage.name};SecretName=${azurerm_key_vault_secret.storage_connection_string.name})"
    "DomainEvents_Queue_QueueName"        = "obelisk-domain-events"
  }
  connection_strings = {
    "ApplicationConnectionString" = {
      type  = "SQLAzure"
      value = "@Microsoft.KeyVault(VaultName=${azurerm_key_vault.stage.name};SecretName=${azurerm_key_vault_secret.web_api_sqldb_connection_string.name})"
    }
  }
  tags = local.default_tags
}
