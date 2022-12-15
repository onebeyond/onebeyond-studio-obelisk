resource "azurerm_static_site" "ui" {
  name                = "${local.resource_prefix}-stapp"
  resource_group_name = azurerm_resource_group.stage.name
  location            = var.static_ui_location
  sku_size            = var.static_ui_sku_size
  tags                = local.default_tags
}

# There are 2 options to deploy custom domain:
# 1. Using TXT and CNAME records (implemented here). The below binding gets created in pending state
#    and waiting for a validation via the produced token and TXT record. After the biding is created
#    copy the token from Azure Portal and ask a person responsible for managing DNS records to create
#    both the TXT record with the token and the CNAME record for proper name resolution. The pros of this method
#    is that it does not require you to wait till all those records exist before you can complete the
#    deployment scripts.
# 2. Ask a person responsible for managing DNS records to create just a CNAME record, wait till it gets propagated
#    and use 'cname-delegation' for the validation_type. The deployment will succeed only when the CNAME
#    record propagation fully completes.
# Provided you are responsible for managing DNS records, you can automate it even further:
# 1. Via using Azure managed DNS records
# 2. Via a bunch of providers capable to deal with DNS records:
#    https://registry.terraform.io/providers/cloudflare/cloudflare/latest/docs
#    https://registry.terraform.io/providers/n3integration/godaddy/latest/docs
resource "azurerm_static_site_custom_domain" "ui" {
  count = (
    var.static_ui_custom_domain == null
    ? 0
    : 1
  )
  static_site_id  = azurerm_static_site.ui.id
  domain_name     = var.static_ui_custom_domain
  validation_type = "dns-txt-token"
}
