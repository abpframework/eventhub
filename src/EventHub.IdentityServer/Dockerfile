  FROM mcr.microsoft.com/dotnet/aspnet:7.0.0-bullseye-slim
  COPY bin/Release/net7.0/publish/ app/
  WORKDIR /app
  ENTRYPOINT ["dotnet", "EventHub.IdentityServer.dll"]