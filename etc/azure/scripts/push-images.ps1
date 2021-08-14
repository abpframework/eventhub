az acr login --name volocr

docker tag eventhub.dbmigrator:latest volocr.azurecr.io/eventhub.dbmigrator:latest
docker push volocr.azurecr.io/eventhub.dbmigrator:latest

docker tag eventhub.www:latest volocr.azurecr.io/eventhub.www:latest
docker push volocr.azurecr.io/eventhub.www:latest

docker tag eventhub.api:latest volocr.azurecr.io/eventhub.api:latest
docker push volocr.azurecr.io/eventhub.api:latest

docker tag eventhub.admin:latest volocr.azurecr.io/eventhub.admin:latest
docker push volocr.azurecr.io/eventhub.admin:latest

docker tag eventhub.admin-api:latest volocr.azurecr.io/eventhub.admin-api:latest
docker push volocr.azurecr.io/eventhub.admin-api:latest

docker tag eventhub.account:latest volocr.azurecr.io/eventhub.account:latest
docker push volocr.azurecr.io/eventhub.account:latest

docker tag eventhub.background-services:latest volocr.azurecr.io/eventhub.background-services:latest
docker push volocr.azurecr.io/eventhub.background-services:latest