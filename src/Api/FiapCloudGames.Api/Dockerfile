FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore src/Api/FiapCloudGames.Api/FiapCloudGames.Api.csproj
RUN dotnet publish src/Api/FiapCloudGames.Api/FiapCloudGames.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app .
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "FiapCloudGames.Api.dll"]
