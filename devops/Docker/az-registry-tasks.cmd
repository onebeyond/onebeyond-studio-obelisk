az acr task create --name PurgeOldImages --cmd "acr purge --filter 'web-api:.*' --filter 'workers:.*' --ago 7d --keep 2 --untagged" --registry dcslgswtcr --context /dev/null
