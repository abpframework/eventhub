 ### Pre-requirements

* Docker Desktop with Kubernetes enabled
* Install [NGINX ingress](https://kubernetes.github.io/ingress-nginx/deploy/) for k8s
* Install [Helm](https://helm.sh/docs/intro/install/) for running helm charts

### How to run?

* Add entries to the hosts file (in Windows: `C:\Windows\System32\drivers\etc\hosts`, in MacOs: `/etc/hosts`):

````
127.0.0.1 eh-st-account
127.0.0.1 eh-st-www
127.0.0.1 eh-st-api
127.0.0.1 eh-st-admin
127.0.0.1 eh-st-admin-api
````

* Run `build-images.ps1` in the `scripts` directory.
* Run `minikube-load-images.ps1` in the `scripts` directory(only for `minikube`). 
* Run `kubectl config set-context --current --namespace=eventhub`
* Run `deploy-staging.ps1` in the `helm-chart` directory. It is deployed with the `eventhub` namespace.
* *You may wait ~30 seconds on first run for preparing the database*.
* Browse https://eh-st-www and https://eh-st-admin
* Username: `admin`, password: `1q2w3E*`.

### How to stop?
* Run `helm uninstall eh-st` command.