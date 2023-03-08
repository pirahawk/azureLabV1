# Docker 

## Docker Build
```
docker build -t 'daprsampleapi:latest' .

1) Build SampleWebApi solution

docker build -t daprsampleapi:latest -f .\azlabv1-sln\AzureLabV1.Dapr.SampleWebApi\dockerfile .\azlabv1-sln\AzureLabV1.Dapr.SampleWebApi

2) Build ClientApi solution

docker build -t daprsampleclientapi:latest -f .\azlabv1-sln\AzureLabV1.Dapr.SampleWebApi.ClientApi\dockerfile .\azlabv1-sln\AzureLabV1.Dapr.SampleWebApi.ClientApi

```

## Docker Run
```
docker run --rm -d -p 7285:443 -p 5285:80 daprsampleapi:latest -n daprsampleapi


// See https://learn.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-7.0

docker run -it -p 7285:443 -p 5285:80 -e ASPNETCORE_URLS="https://+;http://+" -e ASPNETCORE_Kestrel__Certificates__Default__Password="MyPassword123" -e ASPNETCORE_Kestrel__Certificates__Default__Path=/app/https/aspnetapp.pfx daprsampleapi:latest  -n daprsampleapi

docker run --rm -d -p 7285:443 -p 5285:80 -e ASPNETCORE_URLS="https://+;http://+" -e ASPNETCORE_Kestrel__Certificates__Default__Password="MyPassword123" -e ASPNETCORE_Kestrel__Certificates__Default__Path=/app/https/aspnetapp.pfx daprsampleapi:latest  -n daprsampleapi

// NOT USED:
docker run -dp 5002:80 --env WEBSITE_HOSTNAME="localhost:5002" --env AzureWebJobsStorage="<CONN STRING HERE>" --env AzServiceBusConnection="<CONN STRING HERE>" --env AzCosmosDbConnection="<CONN STRING HERE>;" daprsampleapi:latest


http://localhost:5285/swagger/index.html

https://localhost:7285/swagger/index.html


```


# Kubernetes

For notes on pulling local docker images see https://medium.com/swlh/how-to-run-locally-built-docker-images-in-kubernetes-b28fbc32cc1d  Although note that this did not work for me. In the end I just needed to set `imagePullPolicy: IfNotPresent` in the yaml definitions


# Dapr

https://docs.dapr.io/operations/hosting/kubernetes/

install using https://docs.dapr.io/operations/hosting/kubernetes/kubernetes-deploy/ 

Dapr install with helm  https://docs.dapr.io/getting-started/install-dapr-kubernetes/#install-with-helm-advanced

## Running locally

To run locally using the >dapr cli tool for example:


```
// https://docs.dapr.io/reference/cli/dapr-run/

// NOTE:Remember to match the "--app-port" to whatever is configured in your C# project

// Note: You need to makesure the dapr http port for the actors is static and always constant as for Actor proxies to work correctly, they need to be aimed at the 
// Dapr sidecar [http://hostname:port] of the App that hosts the Actors, NOT the Actor [http://hostname:port]!!
// this is why the `--dapr-http-port` flag is essential.



dapr run --app-id csharp-subscriber --dapr-http-port 64441 --app-port 5285 -- dotnet run --project .\azlabv1-sln\AzureLabV1.Dapr.SampleWebApi\AzureLabV1.Dapr.SampleWebApi.csproj


dapr run --app-id csharp-subscriber --dapr-http-port 64441 --app-port 5285 --components-path ".\az-components" -- dotnet run --project .\azlabv1-sln\AzureLabV1.Dapr.SampleWebApi\AzureLabV1.Dapr.SampleWebApi.csproj


//Note: Update the `launchsettings.json` for the `AzureLabV1.Dapr.SampleWebApi.ClientApi.csproj` to add the environment variables to match the SampleWebApi run above



dapr run --app-id csharp-client --dapr-http-port 65295 --app-port 5286 -- dotnet run --project .\azlabv1-sln\AzureLabV1.Dapr.SampleWebApi.ClientApi\AzureLabV1.Dapr.SampleWebApi.ClientApi.csproj

dapr run --app-id csharp-client --dapr-http-port 65295 --app-port 5286 --components-path ".\az-components" -- dotnet run --project .\azlabv1-sln\AzureLabV1.Dapr.SampleWebApi.ClientApi\AzureLabV1.Dapr.SampleWebApi.ClientApi.csproj



// https://docs.dapr.io/reference/cli/dapr-publish/
dapr publish --publish-app-id csharp-subscriber --pubsub orderpubsub --topic orders  --data '{"orderId":"888"}'
dapr publish --publish-app-id csharp-subscriber --pubsub orderpubsub --topic orders  --data '{"order":{"orderId":"888"}}'

```

## Running on Kube
Example

```
dapr init -k -n dapr-control-plane
```

* Port forward the dapr dashboard like so

```
kubectl port-forward service/dapr-dashboard -n dapr-control-plane 5001:8080

http://localhost:5001/overview
```

```
// Note that the --app-port is the "HTTP" port that the dotnet api is configured to listen on which for our porject happens to be ":5285"
dapr run --app-id order-processor --components-path .\components\ --app-port 5285 -- dotnet run --project .\azlabv1-sln\AzureLabV1.Dapr.SampleWebApi\AzureLabV1.Dapr.SampleWebApi.csproj

dapr run --app-id checkout-sdk --components-path .\components\ -- dotnet run --project .\azlabv1-sln\AzureLabV1.Dapr.SampleClient\AzureLabV1.Dapr.SampleClient.csproj
```

* FOR A FULL LIST OF DAPR ANNOTATIONS AND FLAGS SEE
https://docs.dapr.io/reference/arguments-annotations-overview/

# Helm

```
helm upgrade --debug --dry-run dapr-sample-system ./helm/dapr-sample-application -i -n daprsamplesystem --create-namespace --wait --reset-values

helm upgrade dapr-sample-system ./helm/dapr-sample-application -i -n daprsampleapplication --create-namespace --wait --reset-values


 kubectl port-forward  service/mysampleapi -n daprsampleapplication 5001:8080
```


# First Try - Getting Dapr working in Default namespace

Had some issues with getting everything working in my private namespace, got dapper working by just deploying to default namespace. Did the following:
```
https://learn.microsoft.com/en-us/dotnet/architecture/dapr-for-net-developers/publish-subscribe

1) Install redis from Helm:
helm install redis bitnami/redis --set image.tag=6.2
kubectl get secret --namespace default redis -o jsonpath="{.data.redis-password}"

2) deploy Helm chart for my app to default namespace
helm upgrade dapr-sample-system ./helm/dapr-sample-application -i  --wait --reset-values

3) If needed to update or port-forward, commands here:
helm uninstall dapr-sample-system

kubectl port-forward  service/mysampleapiclient  5002:8080
kubectl port-forward  service/mysampleapi  5001:8080

also need to try:
ASPNETCORE_URLS="https://+:443;http://+:3000"
```


# Second Try - Getting Dapr working in custom namespace

```
 helm install redis bitnami/redis --set image.tag=6.2 -n daprapp --create-namespace

 Output:
 ** Please be patient while the chart is being deployed **

Redis&reg; can be accessed on the following DNS names from within your cluster:

    redis-master.daprapp.svc.cluster.local for read/write operations (port 6379)
    redis-replicas.daprapp.svc.cluster.local for read-only operations (port 6379)



To get your password run:

    export REDIS_PASSWORD=$(kubectl get secret --namespace daprapp redis -o jsonpath="{.data.redis-password}" | base64 -d)

To connect to your Redis&reg; server:

1. Run a Redis&reg; pod that you can use as a client:

   kubectl run --namespace daprapp redis-client --restart='Never'  --env REDIS_PASSWORD=$REDIS_PASSWORD  --image docker.io/bitnami/redis:6.2 --command -- sleep infinity

   Use the following command to attach to the pod:

   kubectl exec --tty -i redis-client \
   --namespace daprapp -- bash

2. Connect using the Redis&reg; CLI:
   REDISCLI_AUTH="$REDIS_PASSWORD" redis-cli -h redis-master
   REDISCLI_AUTH="$REDIS_PASSWORD" redis-cli -h redis-replicas

To connect to your database from outside the cluster execute the following commands:

    kubectl port-forward --namespace daprapp svc/redis-master 6379:6379 &
    REDISCLI_AUTH="$REDIS_PASSWORD" redis-cli -h 127.0.0.1 -p 6379
WARNING: Rolling tag detected (bitnami/redis:6.2), please note that it is strongly recommended to avoid using rolling tags in a production environment.
+info https://docs.bitnami.com/containers/how-to/understand-rolling-tags-containers/
```


Then installing rest
```
helm upgrade dapr-sample-system ./helm/dapr-sample-application -i -n daprapp --create-namespace --wait --reset-values



kubectl port-forward  service/mysampleapiclient -n daprapp  5002:8080
```

# Dapr Actors
https://docs.dapr.io/developing-applications/sdks/dotnet/dotnet-actors/dotnet-actors-howto/

https://docs.dapr.io/developing-applications/building-blocks/actors/howto-actors/

https://docs.dapr.io/reference/api/actors_api/

https://docs.dapr.io/developing-applications/sdks/dotnet/dotnet-actors/

https://github.com/dapr/dotnet-sdk/tree/master/examples/Actor