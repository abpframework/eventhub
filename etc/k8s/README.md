### How to run?

* Add entries to the hosts file:

````
127.0.0.1 eh-st-account
127.0.0.1 eh-st-www
127.0.0.1 eh-st-api
127.0.0.1 eh-st-admin
127.0.0.1 eh-st-admin-api
````

* Run `build-images.ps1` in the `scripts` directory.
* Run `helm install eh-st eventhub` in the `helm-chart` directory.

* Browse https://eh-st-www
