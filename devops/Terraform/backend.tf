terraform {
  backend "azurerm" {
    resource_group_name  = "obelisk-infra-rg"
    storage_account_name = "obeliskinfrastorage"
    container_name       = "obelisk-backend"
    key                  = "obelisk-backend.tfstate"
  }
}
