output "vnet_name" {
  description = "Name of the Virtual Network"
  value       = azurerm_virtual_network.main.name
}

output "vnet_id" {
  description = "ID of the Virtual Network"
  value       = azurerm_virtual_network.main.id
}

output "subnet_id" {
  description = "ID of the app subnet"
  value       = azurerm_subnet.app.id
}

output "subnet_name" {
  description = "Name of the app subnet"
  value       = azurerm_subnet.app.name
}
