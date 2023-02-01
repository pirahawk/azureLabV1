# Docker 

```
docker build -t 'daprsampleapi:latest' .  

docker build -t daprsampleapi:latest -f .\azlabv1-sln\AzureLabV1.Dapr.SampleWebApi\dockerfile .\azlabv1-sln\AzureLabV1.Dapr.SampleWebApi


docker run --rm -d -p 7285:443 -p 5285:80 daprsampleapi:latest -n daprsampleapi


// See https://learn.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-7.0

docker run -it -p 7285:443 -p 5285:80 -e ASPNETCORE_URLS="https://+;http://+" -e ASPNETCORE_Kestrel__Certificates__Default__Password="MyPassword123" -e ASPNETCORE_Kestrel__Certificates__Default__Path=/app/https/aspnetapp.pfx daprsampleapi:latest  -n daprsampleapi

docker run --rm -d -p 7285:443 -p 5285:80 -e ASPNETCORE_URLS="https://+;http://+" -e ASPNETCORE_Kestrel__Certificates__Default__Password="MyPassword123" -e ASPNETCORE_Kestrel__Certificates__Default__Path=/app/https/aspnetapp.pfx daprsampleapi:latest  -n daprsampleapi

// NOT USED:
docker run -dp 5002:80 --env WEBSITE_HOSTNAME="localhost:5002" --env AzureWebJobsStorage="<CONN STRING HERE>" --env AzServiceBusConnection="<CONN STRING HERE>" --env AzCosmosDbConnection="<CONN STRING HERE>;" daprsampleapi:latest


http://localhost:5285/swagger/index.html

https://localhost:7285/swagger/index.html


```