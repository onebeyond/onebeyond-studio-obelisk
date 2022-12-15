#!/bin/bash

currentWs=$(terraform -chdir=../Terraform workspace show)
terraform -chdir=../Terraform plan -var-file=terraform.tfvars -var-file=terraform.$currentWs.tfvars -out=terraform.tfplan
