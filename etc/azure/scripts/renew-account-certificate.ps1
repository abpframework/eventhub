$currentFolder = $PSScriptRoot
Set-Location $currentFolder

kubectl delete -f .\corednsms.yaml --ignore-not-found=true
kubectl delete pod --namespace kube-system -l k8s-app=kube-dns --ignore-not-found=true
kubectl rollout status deployment --namespace kube-system coredns
kubectl delete certificate eh-az-account-tls

sleep 60

kubectl apply -f .\corednsms.yaml
kubectl delete pod --namespace kube-system -l k8s-app=kube-dns