terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "3.59.0"
    }
    azapi = {
        source  = "Azure/azapi"
    }
  }
  required_version = ">= 1.1.0"
}

provider "azurerm" {
  
  subscription_id = ""
  client_id       = ""
  client_secret   = ""
  tenant_id       = ""
  features {
  }
  skip_provider_registration = true
}

provider "azapi" {
  alias           = "tlsfix"
  subscription_id = ""
  client_id       = ""
  client_secret   = ""
  tenant_id       = ""
  
  skip_provider_registration = true
}