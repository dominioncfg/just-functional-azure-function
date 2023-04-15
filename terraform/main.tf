##################################################################################
# Terrraform Config
##################################################################################
terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 2.65"
    }
  }
  required_version = ">= 0.14.9"
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
  default = "justfunction"
}

variable "environment_name" {
  type    = string
  default = "Production"
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
