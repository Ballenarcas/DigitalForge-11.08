# Votify - Dockerfile para Azure Container Apps / App Service
# Build: docker build -t votify .
# Run:  docker run -p 8080:8080 --env-file .env votify

# ==========================================
# ETAPA 1: Build del Cliente Blazor WASM
# ==========================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-client
WORKDIR /src

# Votify.Client depende de Votify.Application y Votify.Domain, copiamos sus .csproj
COPY Votify.Client/Votify.Client.csproj Votify.Client/
COPY Votify.Application/Votify.Application.csproj Votify.Application/
COPY Votify.Domain/Votify.Domain.csproj Votify.Domain/

# Restore del cliente (restaurará automáticamente sus dependencias)
RUN dotnet restore Votify.Client/Votify.Client.csproj

# Copiar el código fuente completo de los proyectos que necesita el cliente
COPY Votify.Client/ Votify.Client/
COPY Votify.Application/ Votify.Application/
COPY Votify.Domain/ Votify.Domain/

RUN dotnet publish Votify.Client/Votify.Client.csproj \
    -c Release \
    -o /app/client-publish \
    --no-restore

# ==========================================
# ETAPA 2: Build de la API + copiar cliente
# ==========================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-api
WORKDIR /src

# Copiar archivos de proyecto para cachear restore
COPY Votify.sln global.json ./
COPY Votify.Domain/Votify.Domain.csproj Votify.Domain/
COPY Votify.Application/Votify.Application.csproj Votify.Application/
COPY Votify.Infrastructure/Votify.Infrastructure.csproj Votify.Infrastructure/
COPY Votify.API/Votify.API.csproj Votify.API/

RUN dotnet restore Votify.API/Votify.API.csproj

# Copiar todo el código fuente
COPY Votify.Domain/ Votify.Domain/
COPY Votify.Application/ Votify.Application/
COPY Votify.Infrastructure/ Votify.Infrastructure/
COPY Votify.API/ Votify.API/

# Copiar el cliente Blazor compilado a wwwroot de la API
# Asi la API sirve el SPA estatico en la raiz
COPY --from=build-client /app/client-publish/wwwroot Votify.API/wwwroot/

# Publicar la API en modo Release
RUN dotnet publish Votify.API/Votify.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ==========================================
# ETAPA 3: Runtime final (imagen ligera)
# ==========================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime
WORKDIR /app

# Variables de entorno para ASP.NET Core en contenedores
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_USE_POLLING_FILE_WATCHER=false

# Puerto que expone el contenedor (Azure Container Apps espera 8080 por defecto)
EXPOSE 8080

# Copiar la aplicacion publicada
COPY --from=build-api /app/publish .

# Health check para Azure
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:8080/healthz || exit 1

ENTRYPOINT ["dotnet", "Votify.API.dll"]