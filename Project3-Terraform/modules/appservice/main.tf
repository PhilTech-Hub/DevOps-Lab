# =============================================================================
# App Service Module
# Creates an Azure App Service Plan + Web App to host the ASP.NET API
# =============================================================================

# ── App Service Plan ──────────────────────────────────────────────────────────
# The "server" that runs your web app — F1 is the free tier
resource "azurerm_service_plan" "main" {
  name                = "${var.project_name}-${var.environment}-plan"
  resource_group_name = var.resource_group_name
  location            = var.location
  os_type             = "Linux"
  sku_name            = var.sku_name

  tags = {
    Project     = var.project_name
    Environment = var.environment
    ManagedBy   = "Terraform"
  }
}

# ── Web App ───────────────────────────────────────────────────────────────────
# The actual web application — runs the ASP.NET Docker container
resource "azurerm_linux_web_app" "main" {
  name                = "${var.project_name}-${var.environment}-app"
  resource_group_name = var.resource_group_name
  location            = var.location
  service_plan_id     = azurerm_service_plan.main.id

  site_config {
    always_on = false # Must be false for free tier

    application_stack {
      dotnet_version = "8.0"
    }
  }

  app_settings = {
    "ASPNETCORE_ENVIRONMENT"              = var.environment == "prod" ? "Production" : "Development"
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE" = "false"
  }

  tags = {
    Project     = var.project_name
    Environment = var.environment
    ManagedBy   = "Terraform"
  }
}
