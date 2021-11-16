param ($version='latest')

kubectl delete job eh-az-dbmigrator
helm upgrade --install eh-az eventhub -f .\eventhub\values.azure.yaml --namespace eventhub --create-namespace --set global.eventHubImageVersion=$version