# Run Sample API in Kind cluster

Execute all commands from the ``k8s-kind`` directory.

Build the Docker image and assign a tag:

> docker build -t sample-api:1 ../src/K8s.SampleApi/

Create a Kind cluster with three nodes, using the configuration specified in the ``three-node-cluster.yaml`` file:

> kind create cluster --config ./three-node-cluster.yaml

Load the newly created Docker image into the Kind cluster nodes:

> kind load docker-image sample-api:1

Ensure that the currently selected Kubernetes context is ``kind-kind``:

> kubectl config current-context

Deploy Redis to the cluster:

> kubectl deploy -f ./redis.yaml

Follow the Istio installation [documentation](https://istio.io/latest/docs/setup/getting-started/) and install Istio on the Kind cluster using the profile specified in ``istio-profile.yaml``:

> istioctl install -f ./istio-profile.yaml -y

Install the Gateway API CRDs (Custom Resource Definition):

> kubectl kustomize "github.com/kubernetes-sigs/gateway-api/config/crd?ref=v1.2.0" | kubectl apply -f -

Forward the port from the cluster to a local host port:

> kubectl port-forward service/sample-api-gateway-istio 8080:80

Navigate in the browser to the ``http://localhost:8080/counter`` address.
