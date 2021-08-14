kubectl apply -f .\corednsms.yaml
kubectl delete pod --namespace kube-system -l k8s-app=kube-dns