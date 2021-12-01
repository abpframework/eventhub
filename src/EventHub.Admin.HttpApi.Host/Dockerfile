  FROM mcr.microsoft.com/dotnet/aspnet:6.0.0-bullseye-slim
  COPY bin/Release/net6.0/publish/ app/
  WORKDIR /app
  ENTRYPOINT ["dotnet", "EventHub.Admin.HttpApi.Host.dll"]