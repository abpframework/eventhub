$currentFolder = (Get-Item -Path "./" -Verbose).FullName
$slnFolder = Join-Path $currentFolder "../../../"
$dbmigratorFolder = Join-Path $slnFolder "src/EventHub.DbMigrator"
$webFolder = Join-Path $slnFolder "src/EventHub.Web"
$apiFolder = Join-Path $slnFolder "src/EventHub.HttpApi.Host"
$adminFolder = Join-Path $slnFolder "src/EventHub.Admin.Web"
$adminApiFolder = Join-Path $slnFolder "src/EventHub.Admin.HttpApi.Host"
$identityServerFolder = Join-Path $slnFolder "src/EventHub.IdentityServer"
$backgroundServicesFolder = Join-Path $slnFolder "src/EventHub.BackgroundServices"

### DB MIGRATOR

Write-Host "*** BUILDING DB MIGRATOR ****************" -ForegroundColor Green
Set-Location $dbmigratorFolder
dotnet publish -c Release
docker build -t eventhub.dbmigrator .

### WEB (WWW)

Write-Host "*** BUILDING WEB (WWW) ****************" -ForegroundColor Green
Set-Location $webFolder
dotnet publish -c Release
docker build -t eventhub.www .

### API

Write-Host "*** BUILDING API ****************" -ForegroundColor Green
Set-Location $apiFolder
dotnet publish -c Release
docker build -t eventhub.api .

### ADMIN (BLAZOR)

Write-Host "*** BUILDING ADMIN (BLAZOR) ****************" -ForegroundColor Green
Set-Location $adminFolder
dotnet publish -c Release
docker build -t eventhub.admin .

### ADMIN API

Write-Host "*** BUILDING ADMIN API ****************" -ForegroundColor Green
Set-Location $adminApiFolder
dotnet publish -c Release
docker build -t eventhub.admin-api .

### IDENTITY SERVER (ACCOUNT)

Write-Host "*** BUILDING IDENTITY SERVER (ACCOUNT) ****************" -ForegroundColor Green
Set-Location $identityServerFolder
dotnet publish -c Release
docker build -t eventhub.account .

### BACKGROUND SERVICES

Write-Host "*** BUILDING BACKGROUND SERVICES ****************" -ForegroundColor Green
Set-Location $backgroundServicesFolder
dotnet publish -c Release
docker build -t eventhub.background-services .

### ALL COMPLETED

Write-Host "ALL COMPLETED" -ForegroundColor Green
Set-Location $currentFolder