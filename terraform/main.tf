provider "azurerm" {  
  version = "~>2.0"
  features {}
}
 
data "azurerm_client_config" "current" {}

variable "default_location" {
  type    = string
  default = "francecentral"
}
 
#Create Resource Group
resource "azurerm_resource_group" "tamops" {
  name     = "rg-${var.application_name}"
  location = var.francecentral
}


