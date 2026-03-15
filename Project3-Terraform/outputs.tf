# =============================================================================
# Outputs
# Values printed after terraform apply — useful for connecting other services
# =============================================================================

output "resource_group_name" {
  description = "Name of the created resource group"
  value       = azurerm_resource_group.main.name
}

output "resource_group_location" {
  description = "Location of the resource group"
  value       = azurerm_resource_group.main.location
}

output "app_service_url" {
  description = "Public URL of the deployed web app"
  value       = module.appservice.app_url
}

output "app_service_name" {
  description = "Name of the Azure Web App"
  value       = module.appservice.app_name
}

output "virtual_network_name" {
  description = "Name of the Virtual Network"
  value       = module.network.vnet_name
}

output "subnet_id" {
  description = "ID of the subnet"
  value       = module.network.subnet_id
}
