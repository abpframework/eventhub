FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/EventHub.Web/EventHub.Web.csproj", "src/EventHub.Web/"]
RUN dotnet restore "src/EventHub.Web/EventHub.Web.csproj"
COPY . .
WORKDIR "/src/src/EventHub.Web"
RUN dotnet build "EventHub.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "EventHub.Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "EventHub.Web.dll"]