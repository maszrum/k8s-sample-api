> docker build -t sample-api:1 ../src/K8s.SampleApi/

> kind create cluster --config ./three-node-cluster.yaml

> kind load docker-image sample-api:1

> kubectl config current-context

Install Istio according to the [documentation](https://istio.io/latest/docs/setup/getting-started/).

> istioctl install -f ./istio-profile.yaml -y

> kubectl kustomize "github.com/kubernetes-sigs/gateway-api/config/crd?ref=v1.2.0" | kubectl apply -f -

> istioctl kube-inject -f .\sample-api.yaml | kubectl apply -f -

> kubectl port-forward service/sample-api-gateway-istio 8080:80
