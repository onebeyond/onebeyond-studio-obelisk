variable "project" {
  type        = string
  description = "Project code which is used for naming the stage resources. For example, \"dcslgs-wt\"."
  validation {
    condition     = length(var.project) > 0
    error_message = "Project code should be a non-empty string like \"dcslgs-wt\"."
  }
}

variable "resource_location" {
  type        = string
  description = "Moniker of location which is used for placing the stage resources in. For exaple, \"uksouth\", \"canadacentral\"."
  validation {
    condition     = length(var.resource_location) > 0 # Ideally should be checked against a list of possible locations via data source
    error_message = "Stage location moniker should be a non-empty string like \"uksouth\", \"canadacentral\"."
  }
}

variable "key_vault_admins" {
  type        = list(string)
  description = "List of UPNs to be added as Key Vault admins."
  default     = []
}

variable "sql_allowed_ips" {
  type        = map(string)
  description = "List of IPs to be whitelisted for accessing SQL server."
  default = {
  }
}

variable "web_api_sku_size" {
  type        = string
  description = "SKU for Web API app service plan."
}

# Prior setting this variable to a non-null value, ask a person responsible for DNS records
# management to create correspondong TXT and CNAME records and wait till their
# propagation otherwise deployment fails due to record validation issues.
# The TXT record should contain:
#   Name: asuid.<custom-domain-name-you-want-to-be-assigned>
#   Content: <custom-domain-verification-id>
# The CNAME record should contain:
#   Name: <custom-domain-name-you-want-to-be-assigned>
#   Content: <Azure-generated-name-of-the-app-service>
# An example of such records:
#   CNAME qa.nemo-pro-app[.one-beyond.com]         dcslgs-wt-qa-app.azurewebsites.net
#   TXT   asuid.qa.nemo-pro-app[.one-beyond.com]   DB50885....
# You can obtain <custom-domain-verification-id> via Azure Portal by navigating to
# the app service blade's 'Custom domains' section and finding the 'Custom Domain Verification ID'
# property there. It is sensitive information.
variable "web_api_custom_domain" {
  type        = string
  description = "Custom domain name for Web API. Null value results in Web API default hostname."
  nullable    = true
}

variable "web_api_sqldb_max_size_gb" {
  type        = number
  description = "Max SQL DB size in GB used by Web API. Defaults to 1GB."
  default     = 1
}

variable "web_api_sqldb_sku_size" {
  type        = string
  description = "SKU for Web API SQL DB."  
}

variable "worker_sku_size" {
  type        = string
  description = "SKU for Worker for background jobs."
}
