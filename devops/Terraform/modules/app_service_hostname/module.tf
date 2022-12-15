resource "azurerm_app_service_custom_hostname_binding" "hostname_binding" {
  count = (
    var.app_service_custom_hostname == null
    ? 0
    : 1
  )
  hostname            = var.app_service_custom_hostname
  app_service_name    = var.app_service_name
  resource_group_name = var.app_service_resource_group_name
}

resource "azurerm_app_service_managed_certificate" "managed_certificate" {
  count = (
    length(azurerm_app_service_custom_hostname_binding.hostname_binding) == 0
    ? 0
    : 1
  )
  custom_hostname_binding_id = azurerm_app_service_custom_hostname_binding.hostname_binding[0].id
}

resource "azurerm_app_service_certificate_binding" "certificate_binding" {
  count = (
    length(azurerm_app_service_managed_certificate.managed_certificate) == 0
    ? 0
    : 1
  )
  hostname_binding_id = azurerm_app_service_custom_hostname_binding.hostname_binding[0].id
  certificate_id      = azurerm_app_service_managed_certificate.managed_certificate[0].id
  ssl_state           = "SniEnabled"
}
