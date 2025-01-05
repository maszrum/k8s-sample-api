# Sample API

The following repository contains simple API written in C# .NET with endpoints ``/counter`` and ``/health``.

When running with Docker compose it uses:
- Redis as a cache database and session storage,
- Consul for service discovery,
- HAProxy for load balancing.

When running with Kind (local Kubernetes cluster) is uses:
- Redis as with Docker compose,
- Istio as a Gateway API.

The solution is intentionally over-engineered, to learn how to use Kubernetes locally (Kind).

## Run locally (single instance)

`` dotnet run --project .\src\K8s.SampleApi\K8s.SampleApi.csproj``

## Docker compose

From the ``compose`` directory run ``docker compose up``.

## Kubernetes (Kind)

From the ``k8s-kind`` directory follow the instructions from ``README.md`` file.
