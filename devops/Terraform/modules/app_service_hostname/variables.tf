variable "app_service_resource_group_name" {
  type        = string
  description = "App Service resource group name"
}

variable "app_service_name" {
  type        = string
  description = "App Service name"
}

variable "app_service_custom_hostname" {
  type        = string
  description = "App Service custom hostname. Null value results in App Service default hostname"
  nullable    = true
}

variable "app_service_default_hostname" {
  type        = string
  description = "App Service default hostname. Used when custom hostname is not specified"
}
