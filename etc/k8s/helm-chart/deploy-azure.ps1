param ($version='latest')

helm upgrade --install eh-az eventhub -f .\eventhub\values.azure.yaml --namespace eventhub --create-namespace --set global.eventHubImageVersion=$version