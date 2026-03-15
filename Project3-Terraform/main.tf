# =============================================================================
# Project 3 — Infrastructure as Code with Terraform
# Provisions Azure infrastructure for the Media Content Management System
# =============================================================================

terraform {
  required_version = ">= 1.0"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
  }
}

# ── Azure Provider ────────────────────────────────────────────────────────────
provider "azurerm" {
  features {}
  subscription_id            = var.subscription_id
  skip_provider_registration = true
}

# ── Resource Group ────────────────────────────────────────────────────────────
# A resource group is like a folder that holds all your Azure resources
resource "azurerm_resource_group" "main" {
  name     = "${var.project_name}-rg"
  location = var.location

  tags = {
    Project     = var.project_name
    Environment = var.environment
    ManagedBy   = "Terraform"
  }
}

# ── Network Module ────────────────────────────────────────────────────────────
# Creates a Virtual Network and Subnet
module "network" {
  source = "./modules/network"

  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  project_name        = var.project_name
  environment         = var.environment
}

# ── App Service Module ────────────────────────────────────────────────────────
# Creates an App Service Plan + Web App to host the ASP.NET API
module "appservice" {
  source = "./modules/appservice"

  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  project_name        = var.project_name
  environment         = var.environment
  subnet_id           = module.network.subnet_id
}
