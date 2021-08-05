$currentFolder = (Get-Item -Path "./" -Verbose).FullName
$slnFolder = Join-Path $currentFolder "../../../"
$dbmigratorFolder = Join-Path $slnFolder "src/EventHub.DbMigrator"
$webFolder = Join-Path $slnFolder "src/EventHub.Web"
$apiFolder = Join-Path $slnFolder "src/EventHub.HttpApi.Host"
$identityServerFolder = Join-Path $slnFolder "src/EventHub.IdentityServer"

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

### IDENTITY SERVER (ACCOUNT)

Write-Host "*** BUILDING IDENTITY SERVER (ACCOUNT) ****************" -ForegroundColor Green
Set-Location $identityServerFolder
dotnet publish -c Release
docker build -t eventhub.account .

Write-Host "ALL COMPLETED" -ForegroundColor Green
Set-Location $currentFolder