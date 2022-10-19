  FROM mcr.microsoft.com/dotnet/aspnet:7.0-bullseye-slim
  COPY bin/Release/net6.0/ app/
  WORKDIR /app
  ENTRYPOINT ["dotnet", "EventHub.BackgroundServices.dll"]