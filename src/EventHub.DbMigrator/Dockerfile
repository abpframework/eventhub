  FROM mcr.microsoft.com/dotnet/aspnet:8.3.1-bullseye-slim
  COPY bin/Release/net8.0/ app/
  WORKDIR /app
  ENTRYPOINT ["dotnet", "EventHub.DbMigrator.dll"]