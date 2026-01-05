project = "obelisk-backend"

resource_location = "uksouth"

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
# - "EP1" and "EP2" are also elastic. These have a warm start with fast auto-scale.
# - "B1" (and any other SKU from the web apps) will result in an always on application with a hot start.
# All forms of job will work on any (provided the app has sufficient resource), but the performance of hot/cold starts varies significantly.
worker_sku_size = "B1" 

web_api_custom_domain = null
