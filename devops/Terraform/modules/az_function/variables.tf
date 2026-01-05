variable "resource_group" {
  type = object({
    name     = string
    location = string
  })
  description = "Resource group used for deploying the function."
}

variable "name_prefix" {
  type        = string
  description = "Name of the function which will be suffixed with '-func'."
}

variable "sku_name" {
  type        = string  
  description = "SKU name of the function."
}

variable "storage_account" {
  type = object({
    name       = string
    access_key = string
  })
  description = "Storage account for the function backend. It can't seenm to use managed identity for isolated runtime."
}

variable "key_vault_id" {
  type        = string
  description = "Key vault the function will be linked to."
}

variable "application_insights" {
  type = object({
    key               = string
    connection_string = string
  })
  description = "Application insights the function will be linked to."
}

variable "app_settings" {
  type        = map(string)
  default     = {}
  description = "App settings for the function."
}

variable "connection_strings" {
  type = map(object({
    type  = string
    value = string
  }))
  default     = {}
  description = "Connection strings for the function."
}

variable "tags" {
  type        = map(string)
  default     = {}
  description = "Tags for the function."
}
