> docker build -t sample-api:1 ../src/K8s.SampleApi/

> kind create cluster --config .\three-node-cluster.yaml

> kind load docker-image sample-api:1
