  FROM mcr.microsoft.com/dotnet/aspnet:8.0
  COPY bin/Release/net9.0/publish/ app/
  WORKDIR /app
  EXPOSE 80
  ENV ASPNETCORE_URLS=http://+:80
  ENTRYPOINT ["dotnet", "EventHub.Admin.HttpApi.Host.dll"]