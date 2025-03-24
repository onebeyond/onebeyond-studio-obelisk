locals {
  project         = lower(var.project)
  stage_code      = lower(terraform.workspace)
  resource_prefix = "${local.project}-${local.stage_code}"
  key_vault_secret_names = {
    sql_admin_password              = "SqlAdminPassword"
    storage_connection_string       = "StorageConnectionString"
    web_api_sqldb_user_password     = "WebApiSqlDbUserPassword"
    web_api_sqldb_connection_string = "WebApiSqlDbConnectionString"
    web_api_admin_password          = "WebApiAdminPassword"
    jwt_secret                      = "JwtSecret"
  }
  default_tags = {
    stage      = local.stage_code
    managed-by = "terraform"
  }
}
