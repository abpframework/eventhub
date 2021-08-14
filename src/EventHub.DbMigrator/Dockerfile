  FROM mcr.microsoft.com/dotnet/aspnet:5.0
  COPY bin/Release/net5.0/ app/
  WORKDIR /app
  ENTRYPOINT ["dotnet", "EventHub.DbMigrator.dll"]