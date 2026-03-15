output "app_name" {
  description = "Name of the Web App"
  value       = azurerm_linux_web_app.main.name
}

output "app_url" {
  description = "Public URL of the Web App"
  value       = "https://${azurerm_linux_web_app.main.default_hostname}"
}

output "app_id" {
  description = "ID of the Web App"
  value       = azurerm_linux_web_app.main.id
}

output "service_plan_id" {
  description = "ID of the App Service Plan"
  value       = azurerm_service_plan.main.id
}
