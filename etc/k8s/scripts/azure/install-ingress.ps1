$REGISTRY_NAME="volocr.azurecr.io"
$CONTROLLER_REGISTRY="k8s.gcr.io"
$CONTROLLER_IMAGE="ingress-nginx/controller"
$CONTROLLER_TAG="v0.48.1"
$PATCH_REGISTRY="docker.io"
$PATCH_IMAGE="jettech/kube-webhook-certgen"
$PATCH_TAG="v1.5.1"
$DEFAULTBACKEND_REGISTRY="k8s.gcr.io"
$DEFAULTBACKEND_IMAGE="defaultbackend-amd64"
$DEFAULTBACKEND_TAG="1.5"

az acr import --name $REGISTRY_NAME --source ${CONTROLLER_REGISTRY}/${CONTROLLER_IMAGE}:${CONTROLLER_TAG} --image ${CONTROLLER_IMAGE}:${CONTROLLER_TAG}
az acr import --name $REGISTRY_NAME --source ${PATCH_REGISTRY}/${PATCH_IMAGE}:${PATCH_TAG} --image ${PATCH_IMAGE}:${PATCH_TAG}
az acr import --name $REGISTRY_NAME --source ${DEFAULTBACKEND_REGISTRY}/${DEFAULTBACKEND_IMAGE}:${DEFAULTBACKEND_TAG} --image ${DEFAULTBACKEND_IMAGE}:${DEFAULTBACKEND_TAG}

# Create a namespace for your ingress resources
kubectl create namespace ingress-basic

# Add the ingress-nginx repository
helm repo add ingress-nginx https://kubernetes.github.io/ingress-nginx

# Set variable for ACR location to use for pulling images
$ACR_URL="volocr.azurecr.io"

# Use Helm to deploy an NGINX ingress controller
helm install nginx-ingress ingress-nginx/ingress-nginx `
    --namespace ingress-basic `
    --set controller.replicaCount=1 `
    --set controller.nodeSelector."kubernetes\.io/os"=linux `
    --set controller.image.registry=$ACR_URL `
    --set controller.image.image=$CONTROLLER_IMAGE `
    --set controller.image.tag=$CONTROLLER_TAG `
    --set controller.image.digest="" `
    --set controller.admissionWebhooks.patch.nodeSelector."kubernetes\.io/os"=linux `
    --set controller.admissionWebhooks.patch.image.registry=$ACR_URL `
    --set controller.admissionWebhooks.patch.image.image=$PATCH_IMAGE `
    --set controller.admissionWebhooks.patch.image.tag=$PATCH_TAG `
    --set defaultBackend.nodeSelector."kubernetes\.io/os"=linux `
    --set defaultBackend.image.registry=$ACR_URL `
    --set defaultBackend.image.image=$DEFAULTBACKEND_IMAGE `
    --set defaultBackend.image.tag=$DEFAULTBACKEND_TAG