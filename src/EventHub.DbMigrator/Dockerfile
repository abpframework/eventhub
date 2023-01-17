  FROM mcr.microsoft.com/dotnet/aspnet:7.0.0-bullseye-slim
  COPY bin/Release/net7.0/ app/
  WORKDIR /app
  ENTRYPOINT ["dotnet", "EventHub.DbMigrator.dll"]