FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/BookShelfAPI/BookShelfAPI.csproj", "src/BookShelfAPI/"]
RUN dotnet restore "src/BookShelfAPI/BookShelfAPI.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "./src/BookShelfAPI/BookShelfAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./src/BookShelfAPI/BookShelfAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookShelfAPI.dll"]
