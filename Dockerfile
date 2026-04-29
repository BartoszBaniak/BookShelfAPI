# Build stage — SDK + source + dependencies, shared by migrator and publish
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Tool manifest first — caches dotnet-ef tool restore independently
COPY .config/ .config/
RUN dotnet tool restore

# Copy csproj files first for cached restore
COPY src/BookShelfAPI/BookShelfAPI.csproj                 src/BookShelfAPI/
COPY src/BookShelfAPI.Application/BookShelfAPI.Application.csproj src/BookShelfAPI.Application/
COPY src/BookShelfAPI.Domain/BookShelfAPI.Domain.csproj   src/BookShelfAPI.Domain/
COPY src/BookShelfAPI.Infrastructure/BookShelfAPI.Infrastructure.csproj src/BookShelfAPI.Infrastructure/
RUN dotnet restore src/BookShelfAPI/BookShelfAPI.csproj

# Copy actual source
COPY src/ src/

# Migrator stage — applies EF Core migrations and exits
FROM build AS migrator
ENTRYPOINT ["dotnet", "ef", "database", "update", \
    "--project", "src/BookShelfAPI.Infrastructure", \
    "--startup-project", "src/BookShelfAPI"]

# Publish stage — produces deployable artifact
FROM build AS publish
RUN dotnet publish src/BookShelfAPI/BookShelfAPI.csproj \
    -c Release -o /app/publish --no-restore

# Runtime stage — slim aspnet image, just the published app
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "BookShelfAPI.dll"]
