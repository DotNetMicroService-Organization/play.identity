# Play.Identity
Play Economy Identity microservice.

## Create and publish package Contracts
```powershell
$version="1.0.4"
$owner="DotNetMicroService-Organization"
$ph_pat="[PAT HERE]"

dotnet pack src\Play.Identity.Contracts\ --configuration Release -p:PackageVersion=$version -p:RepositoryUrl=https://github.com/$owner/play.identity -o ..\packages

dotnet nuget push ..\packages\Play.Identity.Contracts.$version.nupkg --api-key $ph_pat --source "github"
```

## Build the docker image
```powershell
$env:GH_OWNER="DotNetMicroService-Organization"
$env:GH_PAT="[PAT HERE]"
$appname="dotnetplayeconomy"
docker build --secret id=GH_OWNER --secret id=GH_PAT -t "$appname.azurecr.io/play.identity:$version" .
```

## run the docker image
```powershell
$adminPass="[PASS HERE]"
$cosmosDbConnString="[CONN STRING HERE]"
$serviceBusConnString="[CONN STRING HERE]"
docker run -it --rm -p 5002:5002 --name identity -e MongoDbSettings__ConnectionString=$cosmosDbConnString -e ServiceBusSettings__ConnectionString=$serviceBusConnString -e ServiceSettings__MessageBroker="SERVICEBUS" -e IdentitySettings__AdminUserPassword=$adminPass play.identity:$version
```

## Publishing the docker image
```powershell
az acr login --name $appname
docker push "$appname.azurecr.io/play.identity:$version"
```

## Create the k8s namespace
```powershell
$namespace="identity"
kubectl create namespace $namespace
```

## Create the k8s secret
```powershell
kubectl create secret generic identity-secrets --from-literal=cosmosdb-connectionstring=$cosmosDbConnString --from-literal=servicebus-connectionstring=$serviceBusConnString --from-literal=admin-password=$adminPass -n $namespace
```

## Create the k8s pod
```powershell
kubectl apply -f kubernetes\identity.yaml -n $namespace
```