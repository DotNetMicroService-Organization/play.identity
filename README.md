# Play.Identity
Play Economy Identity microservice.

## Create and publish package Contracts
```powershell
$version="1.0.2"
$owner="DotNetMicroService-Organization"
$ph_pat="[PAT HERE]"

dotnet pack src\Play.Identity.Contracts\ --configuration Release -p:PackageVersion=$version -p:RepositoryUrl=https://github.com/$owner/play.identity -o ..\packages

dotnet nuget push ..\packages\Play.Identity.Contracts.$version.nupkg --api-key $ph_pat --source "github"
```