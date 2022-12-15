output "hostname" {
  value = (
    var.app_service_custom_hostname == null
    ? var.app_service_default_hostname
    : var.app_service_custom_hostname
  )
}
