  FROM mcr.microsoft.com/dotnet/aspnet:5.0
  COPY bin/Release/net5.0/publish/ app/
  RUN apt-get update && apt-get install -y curl
  WORKDIR /app
  ENTRYPOINT ["dotnet", "EventHub.Web.dll"]