# =============================================================================
# Network Module
# Creates a Virtual Network and Subnet for the application
# =============================================================================

# ── Virtual Network ───────────────────────────────────────────────────────────
# A private network inside Azure — isolates your resources
resource "azurerm_virtual_network" "main" {
  name                = "${var.project_name}-${var.environment}-vnet"
  resource_group_name = var.resource_group_name
  location            = var.location
  address_space       = ["10.0.0.0/16"] # IP range for the network

  tags = {
    Project     = var.project_name
    Environment = var.environment
    ManagedBy   = "Terraform"
  }
}

# ── Subnet ────────────────────────────────────────────────────────────────────
# A subdivision of the virtual network for the web app
resource "azurerm_subnet" "app" {
  name                 = "${var.project_name}-${var.environment}-subnet"
  resource_group_name  = var.resource_group_name
  virtual_network_name = azurerm_virtual_network.main.name
  address_prefixes     = ["10.0.1.0/24"]

  delegation {
    name = "app-service-delegation"
    service_delegation {
      name    = "Microsoft.Web/serverFarms"
      actions = ["Microsoft.Network/virtualNetworks/subnets/action"]
    }
  }
}
