$currentWs = terraform -chdir='../Terraform' workspace show

terraform -chdir='../Terraform' import -var-file="terraform.tfvars" -var-file="terraform.$currentWs.tfvars" $args[0] $args[1]
