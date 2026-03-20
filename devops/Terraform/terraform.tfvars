project = "obelisk-backend"

resource_location = "uksouth"

# PLEASE CHANGE THESE - WE DON'T WANT TO ADMINISTER YOUR PROJECT
# Note that in 99% of cases, you will find that the correct form will be something like
# andrii.kaplanovskyi_one-beyond.com#EXT#@domain.onmicrosoft.com
key_vault_admins = [
  "andrii.kaplanovskyi@one-beyond.com",
  "nick.skliar-davies@one-beyond.com",
  "alexis.shirtliff@one-beyond.com"
]

sql_allowed_ips = {
  OpenVPNLondon = "20.68.179.247"
  Farnborough   = "167.98.118.42"
}

web_api_sku_size = "B1" # B1 is usually too low to even support a QA system. Consider B2 or B3. Production-tier should start at P0V3 (S-Tiers are deprecated for app services)
web_api_sqldb_sku_size = "Basic" # Basic is usually too low to even support a QA system. Consider at least S0 or S1.


# Depends on what is needed. 
# - "Y1" will create an elastic, but always off application - which "turns on", when triggered. This is very cheap - but has a cold start. 
#        Note - "Y1" may be deprecated in future - as such, there is included in here an az_flex_function module (for FC1 which still has a free tier but more control).
#        That has a slightly different setup - if you need help with this please ask
# - "EP1" and "EP2" are also elastic. These have a warm start with fast auto-scale.
# - "B1" (and any other SKU from the web apps) will result in an always on application with a hot start.
# All forms of job will work on any (provided the app has sufficient resource), but the performance of hot/cold starts varies significantly.
worker_sku_size = "B1" 

web_api_custom_domain = null

# Will likely be something like purple-octopus-[somerandomchars].azurestaticapps.net
spa_url = "UNKNOWN"

signalr_service_sku = "Free_F1"