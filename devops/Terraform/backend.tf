terraform {
  backend "azurerm" {
    resource_group_name  = "dcslgs-wt-infra-rg"
    storage_account_name = "dcslgswtinfrastorage"
    container_name       = "terraform"
    key                  = "terraform.tfstate"
  }
}
