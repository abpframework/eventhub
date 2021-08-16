param ($version='latest')

kubectl delete job eh-st-dbmigrator
helm upgrade --install eh-st eventhub --namespace eventhub --create-namespace --set global.eventHubImageVersion=$version