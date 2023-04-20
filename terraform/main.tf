##################################################################################
# Terrraform Config
##################################################################################
terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.52.0"
    }
  }
  required_version = ">= 1.4.5"
}

##################################################################################
# PROVIDERS
##################################################################################

provider "azurerm" {
  features {}
}


data "azurerm_client_config" "current" {}


##################################################################################
# Variables
##################################################################################

variable "default_location" {
  type    = string
  default = "francecentral"
}

variable "application_name" {
  type    = string
  default = "justfunctional"
}

variable "environment_name" {
  type    = string
  default = "Production"
}

variable "dotnet_version" {
  type    = string
  default = "6.0"
}

variable "functions_cors" {
  type    = list
  default = ["https://portal.azure.com"]
}

##################################################################################
# Locals
##################################################################################

locals {
  environments = {
    Development = "dev"
    Production  = "prod"
  }

  rnd_number = "001"
}


locals {
  environment_prefix = lookup(local.environments, var.environment_name, local.environments["Development"])
}

##################################################################################
# RESOURCES
##################################################################################

#Create Resource Group
resource "azurerm_resource_group" "rgApp" {
  name     = "rg-${var.application_name}-${local.environment_prefix}-${local.rnd_number}"
  location = var.default_location
}

resource "azurerm_storage_account" "functionsStorage" {
  name                     = "st${var.application_name}${local.environment_prefix}${local.rnd_number}"
  resource_group_name      = azurerm_resource_group.rgApp.name
  location                 = azurerm_resource_group.rgApp.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_service_plan" "functionsPlan" {
  name                = "plan-${var.application_name}-${local.environment_prefix}-${local.rnd_number}"
  resource_group_name = azurerm_resource_group.rgApp.name
  location            = azurerm_resource_group.rgApp.location
  os_type             = "Linux"
  sku_name            = "Y1"
}

resource "azurerm_linux_function_app" "functionApp" {
  name                = "fap-${var.application_name}-evaluator-${local.environment_prefix}"
  resource_group_name = azurerm_resource_group.rgApp.name
  location            = azurerm_resource_group.rgApp.location

  storage_account_name       = azurerm_storage_account.functionsStorage.name
  storage_account_access_key = azurerm_storage_account.functionsStorage.primary_access_key
  service_plan_id            = azurerm_service_plan.functionsPlan.id

  site_config {
    application_stack {
      dotnet_version = var.dotnet_version
    }
    cors {
      allowed_origins = var.functions_cors
    }
  }
}

output "resource_group_name" {
  value       = azurerm_resource_group.rgApp.name
  description = "The Name of the Resource Group"
  sensitive   = false
}


output "service_plan_name" {
  value       = azurerm_service_plan.functionsPlan.name
  description = "The Name of the Azure Function's Service Plan"
  sensitive   = false
}

output "function_app_name" {
  value       = azurerm_linux_function_app.functionApp.name
  description = "The Name of the Azure Function's App"
  sensitive   = false
}

