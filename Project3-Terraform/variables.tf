# =============================================================================
# Input Variables
# These are the values you can customise for each environment
# =============================================================================

variable "subscription_id" {
  description = "Your Azure Subscription ID"
  type        = string
  # Set this in terraform.tfvars — never hardcode it here
}

variable "project_name" {
  description = "Name of the project — used to name all resources"
  type        = string
  default     = "mediaapp"
}

variable "environment" {
  description = "Deployment environment (dev, staging, prod)"
  type        = string
  default     = "dev"

  validation {
    condition     = contains(["dev", "staging", "prod"], var.environment)
    error_message = "Environment must be dev, staging, or prod."
  }
}

variable "location" {
  description = "Azure region to deploy resources into"
  type        = string
  default     = "East US"
}

variable "app_service_sku" {
  description = "App Service pricing tier (F1 = Free)"
  type        = string
  default     = "F1"  # Free tier — no cost
}
