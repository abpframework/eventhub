param ($version='latest')

az acr login --name volosoft

docker tag eventhub.dbmigrator:$version volosoft.azurecr.io/eventhub.dbmigrator:$version
docker push volosoft.azurecr.io/eventhub.dbmigrator:$version
docker tag volosoft.azurecr.io/eventhub.dbmigrator:$version volosoft.azurecr.io/eventhub.dbmigrator:latest

docker tag eventhub.www:$version volosoft.azurecr.io/eventhub.www:$version
docker push volosoft.azurecr.io/eventhub.www:$version
docker tag volosoft.azurecr.io/eventhub.www:$version volosoft.azurecr.io/eventhub.www:latest

docker tag eventhub.api:$version volosoft.azurecr.io/eventhub.api:$version
docker push volosoft.azurecr.io/eventhub.api:$version
docker tag volosoft.azurecr.io/eventhub.api:$version volosoft.azurecr.io/eventhub.api:latest

docker tag eventhub.admin:$version volosoft.azurecr.io/eventhub.admin:$version
docker push volosoft.azurecr.io/eventhub.admin:$version
docker tag volosoft.azurecr.io/eventhub.admin:$version volosoft.azurecr.io/eventhub.admin:latest

docker tag eventhub.admin-api:$version volosoft.azurecr.io/eventhub.admin-api:$version
docker push volosoft.azurecr.io/eventhub.admin-api:$version
docker tag volosoft.azurecr.io/eventhub.admin-api:$version volosoft.azurecr.io/eventhub.admin-api:latest

docker tag eventhub.account:$version volosoft.azurecr.io/eventhub.account:$version
docker push volosoft.azurecr.io/eventhub.account:$version
docker tag volosoft.azurecr.io/eventhub.account:$version volosoft.azurecr.io/eventhub.account:latest

docker tag eventhub.background-services:$version volosoft.azurecr.io/eventhub.background-services:$version
docker push volosoft.azurecr.io/eventhub.background-services:$version
docker tag volosoft.azurecr.io/eventhub.background-services:$version volosoft.azurecr.io/eventhub.background-services:latest
