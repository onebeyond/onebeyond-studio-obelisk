output "rg_name" {
  value = azurerm_resource_group.stage.name
}

output "static_ui_deployment_token" {
  value     = azurerm_static_site.ui.api_key
  sensitive = true
}

output "web_api_app_service_name" {
  value = azurerm_linux_web_app.web_api.name
}

output "web_api_url" {
  value = "https://${module.web_api_hostname.hostname}/"
}

output "web_api_sqldb_connection_string" {
  value     = azurerm_key_vault_secret.web_api_sqldb_connection_string.value
  sensitive = true
}

output "workers_app_name" {
  value = module.web_api_workers.app_name
}
