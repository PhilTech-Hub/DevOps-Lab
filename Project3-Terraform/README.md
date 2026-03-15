# 🏗️ Project 3 — Infrastructure as Code with Terraform

> Automated Azure infrastructure provisioning using Terraform.
> Part of the DevOps-Lab portfolio.

---

## 🏗️ What this provisions

Running `terraform apply` automatically creates all of this in Azure:

```
Azure Subscription
└── Resource Group (mediaapp-dev-rg)
    ├── Virtual Network (mediaapp-dev-vnet)
    │   └── Subnet (mediaapp-dev-subnet)
    └── App Service Plan (mediaapp-dev-plan)  ← Free tier
        └── Web App (mediaapp-dev-app)        ← Hosts the API
```

---

## 🗂️ Project structure

```
Project3-Terraform/
├── .github/workflows/
│   └── project3-terraform.yml  ← CI/CD: validate + plan + apply
├── modules/
│   ├── network/
│   │   ├── main.tf             ← Virtual Network + Subnet
│   │   ├── variables.tf
│   │   └── outputs.tf
│   └── appservice/
│       ├── main.tf             ← App Service Plan + Web App
│       ├── variables.tf
│       └── outputs.tf
├── main.tf                     ← Root config, calls modules
├── variables.tf                ← All input variables
├── outputs.tf                  ← Values printed after apply
├── terraform.tfvars            ← Your actual values (gitignored)
├── terraform.tfvars.example    ← Safe example to commit
└── .gitignore                  ← Excludes state files + tfvars
```

---

## 🛠️ Run locally

### Prerequisites
- [Terraform](https://developer.hashicorp.com/terraform/install)
- [Azure CLI](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli)
- Azure account (`az login`)

### Step 1 — Copy and fill in your variables
```bash
cp terraform.tfvars.example terraform.tfvars
# Edit terraform.tfvars with your subscription ID
```

### Step 2 — Initialise Terraform
```bash
terraform init
```

### Step 3 — Preview what will be created
```bash
terraform plan
```

### Step 4 — Create the infrastructure
```bash
terraform apply
```
Type `yes` when prompted.

### Step 5 — See your outputs
```bash
terraform output
```
You'll see your app URL, resource group name, etc.

### Step 6 — Destroy infrastructure when done (saves money)
```bash
terraform destroy
```

---

## 📈 What this demonstrates on your CV

| Skill | Evidence |
|---|---|
| Infrastructure as Code | Terraform provisions real Azure resources |
| Modular IaC design | Reusable network + appservice modules |
| Version controlled infrastructure | All config in Git |
| CI/CD for infrastructure | GitHub Actions validates on every push |
| Cloud networking | Virtual Network + Subnet configuration |
| Security | tfvars gitignored, state files excluded |
