  FROM mcr.microsoft.com/dotnet/aspnet:8.0
  COPY bin/Release/net8.0/ app/
  WORKDIR /app
  ENTRYPOINT ["dotnet", "EventHub.BackgroundServices.dll"]