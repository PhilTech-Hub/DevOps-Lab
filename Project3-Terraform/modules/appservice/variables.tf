variable "resource_group_name" {
  description = "Name of the resource group"
  type        = string
}

variable "location" {
  description = "Azure region"
  type        = string
}

variable "project_name" {
  description = "Project name"
  type        = string
}

variable "environment" {
  description = "Environment name"
  type        = string
}

variable "subnet_id" {
  description = "ID of the subnet to connect the app to"
  type        = string
}

variable "sku_name" {
  description = "App Service SKU (F1 = Free)"
  type        = string
  default     = "F1"
}
