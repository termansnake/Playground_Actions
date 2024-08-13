data "azurerm_resource_group" "example" {
  name     = "DevOps-Resources_01"
}

resource "azurerm_cosmosdb_account" "example" {
  name                = "cosmos-metrics-terraform-01"
  location            = data.azurerm_resource_group.example.location
  resource_group_name = data.azurerm_resource_group.example.name
  offer_type          = "Standard"
  kind                = "GlobalDocumentDB" # API: Núcleo (SQL)

  enable_automatic_failover = true
  capabilities {
    name = "EnableServerless"
  }

  consistency_policy {
    consistency_level       = "Session"
    max_interval_in_seconds = 10
    max_staleness_prefix    = 200
  }

  geo_location {
    location          = data.azurerm_resource_group.example.location
    failover_priority = 0
  }

  # Método de conectividad: Todas las redes
  public_network_access_enabled = true
}

resource "azurerm_cosmosdb_sql_database" "example" {
  name                = "KPIS_BD"
  resource_group_name = data.azurerm_resource_group.example.name
  account_name        = azurerm_cosmosdb_account.example.name
}

resource "azurerm_cosmosdb_sql_container" "example" {
  name                = "devops.oxxo.cfd"
  resource_group_name = data.azurerm_resource_group.example.name
  account_name        = azurerm_cosmosdb_account.example.name
  database_name       = azurerm_cosmosdb_sql_database.example.name

  partition_key_path   = "/partitionKey"
  unique_key {
    paths = ["/Id"]
  }
}

resource "azapi_update_resource" "azurerm_cosmosdb_account_update_tls_to_1_2" {
  type        = "Microsoft.DocumentDB/databaseAccounts@2023-03-15"
  resource_id = azurerm_cosmosdb_account.example.id
  body = jsonencode({
    properties = {
      minimalTlsVersion = "Tls12"
    }
  })
  provider = azapi.tlsfix
  depends_on = [
    azurerm_cosmosdb_sql_container.example
  ]
}

resource "local_file" "output_json" {
  filename = "output.json"
  content  = jsonencode({
    cosmosdb_uri                   = azurerm_cosmosdb_account.example.endpoint,
    cosmosdb_primary_key           = azurerm_cosmosdb_account.example.primary_key,
    cosmosdb_primary_connection_string = azurerm_cosmosdb_account.example.connection_strings[0],
  })
}