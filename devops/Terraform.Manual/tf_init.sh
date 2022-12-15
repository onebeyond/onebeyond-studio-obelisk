#!/bin/bash

terraform -chdir=../Terraform init -reconfigure -backend-config=config.tfbackend
terraform -chdir=../Terraform workspace select "$1" -no-color
