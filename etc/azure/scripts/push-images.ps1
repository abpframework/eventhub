param ($version='latest')

az acr login --name volocr

docker tag eventhub.dbmigrator:$version volocr.azurecr.io/eventhub.dbmigrator:$version
docker push volocr.azurecr.io/eventhub.dbmigrator:$version
docker tag volocr.azurecr.io/eventhub.dbmigrator:$version volocr.azurecr.io/eventhub.dbmigrator:latest

docker tag eventhub.www:$version volocr.azurecr.io/eventhub.www:$version
docker push volocr.azurecr.io/eventhub.www:$version
docker tag volocr.azurecr.io/eventhub.www:$version volocr.azurecr.io/eventhub.www:latest

docker tag eventhub.api:$version volocr.azurecr.io/eventhub.api:$version
docker push volocr.azurecr.io/eventhub.api:$version
docker tag volocr.azurecr.io/eventhub.api:$version volocr.azurecr.io/eventhub.api:latest

docker tag eventhub.admin:$version volocr.azurecr.io/eventhub.admin:$version
docker push volocr.azurecr.io/eventhub.admin:$version
docker tag volocr.azurecr.io/eventhub.admin:$version volocr.azurecr.io/eventhub.admin:latest

docker tag eventhub.admin-api:$version volocr.azurecr.io/eventhub.admin-api:$version
docker push volocr.azurecr.io/eventhub.admin-api:$version
docker tag volocr.azurecr.io/eventhub.admin-api:$version volocr.azurecr.io/eventhub.admin-api:latest

docker tag eventhub.account:$version volocr.azurecr.io/eventhub.account:$version
docker push volocr.azurecr.io/eventhub.account:$version
docker tag volocr.azurecr.io/eventhub.account:$version volocr.azurecr.io/eventhub.account:latest

docker tag eventhub.background-services:$version volocr.azurecr.io/eventhub.background-services:$version
docker push volocr.azurecr.io/eventhub.background-services:$version
docker tag volocr.azurecr.io/eventhub.background-services:$version volocr.azurecr.io/eventhub.background-services:latest
