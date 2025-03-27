  FROM mcr.microsoft.com/dotnet/aspnet:9.0-jammy
  COPY bin/Release/net9.0/ app/
  WORKDIR /app
  EXPOSE 80
  ENV ASPNETCORE_URLS=http://+:80
  ENTRYPOINT ["dotnet", "EventHub.DbMigrator.dll"]