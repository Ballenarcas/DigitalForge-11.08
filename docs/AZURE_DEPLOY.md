# Guia de Despliegue en Azure - Votify

## Opcion recomendada: Azure Container Apps (ACA)

Azure Container Apps es la opcion mas simple y economica para aplicaciones .NET en contenedores.

---

## Paso 1: Instalar Azure CLI

Descarga e instala desde: https://aka.ms/installazurecliwindows

```powershell
az --version
az login
```

---

## Paso 2: Crear recursos en Azure

### 2.1 Crear Resource Group
```bash
az group create --name rg-votify --location westeurope
```

### 2.2 Crear Azure Container Registry (ACR)
```bash
az acr create --resource-group rg-votify --name acrvotify --sku Basic --admin-enabled true
```

### 2.3 Crear Azure Container Apps Environment
```bash
az containerapp env create --name cae-votify --resource-group rg-votify --location westeurope
```

---

## Paso 3: Construir y subir la imagen Docker

### 3.1 Iniciar sesion en ACR
```bash
az acr login --name acrvotify
```

### 3.2 Construir la imagen
```bash
cd C:\Users\angel\source\repos\Ballenarcas\Proyecto-DDS-PSW

docker build -t acrvotify.azurecr.io/votify:latest .
docker push acrvotify.azurecr.io/votify:latest
```

---

## Paso 4: Desplegar en Azure Container Apps

### Opcion A: Por CLI

```bash
az containerapp create \
  --name app-votify \
  --resource-group rg-votify \
  --environment cae-votify \
  --image acrvotify.azurecr.io/votify:latest \
  --target-port 8080 \
  --ingress external \
  --query properties.configuration.ingress.fqdn
```

### Configurar variables de entorno
```bash
az containerapp env vars set \
  --name app-votify \
  --resource-group rg-votify \
  --env-vars \
    DB_HOST=aws-1-eu-west-1.pooler.supabase.com \
    DB_PORT=5432 \
    DB_NAME=postgres \
    DB_USER=tu-usuario \
    DB_PASSWORD=tu-password \
    Jwt__Key=TuClaveSecretaLarga \
    SUPABASE_URL=https://tu-proyecto.supabase.co \
    SUPABASE_SERVICE_KEY=tu-service-key \
    AI_SUMMARIZER_ENABLED=true \
    AI_SUMMARIZER_BASE_URL=https://generativelanguage.googleapis.com/ \
    AI_SUMMARIZER_API_KEY=tu-api-key \
    AI_SUMMARIZER_MODEL=gemini-2.5-flash
```

### Opcion B: Por Azure Portal

1. Ve a https://portal.azure.com
2. Busca "Container Apps"
3. Clic en "Create"
4. Rellena:
   - **Subscription:** Tu suscripcion
   - **Resource Group:** rg-votify
   - **Container app name:** app-votify
   - **Region:** West Europe
5. En "Container":
   - **Image source:** Azure Container Registry
   - **Registry:** acrvotify
   - **Image:** votify
   - **Tag:** latest
6. En "Ingress":
   - **Enabled:** Yes
   - **Target port:** 8080
7. En "Environment variables": Añade todas las variables del .env
8. Revisa y crea

---

## Paso 5: Verificar el despliegue

### URL de la aplicacion
```bash
az containerapp show --name app-votify --resource-group rg-votify --query properties.configuration.ingress.fqdn --output tsv
```

### Endpoints a probar
```bash
# Health check
curl https://TU-URL.azurecontainerapps.io/healthz

# API
curl https://TU-URL.azurecontainerapps.io/api/eventos

# Frontend: abre la URL base en navegador
```

---

## Paso 6: Actualizar el despliegue

```bash
docker build -t acrvotify.azurecr.io/votify:latest .
docker push acrvotify.azurecr.io/votify:latest

az containerapp update --name app-votify --resource-group rg-votify --image acrvotify.azurecr.io/votify:latest
```

---

## Alternativa: Azure App Service

```bash
# Crear App Service Plan (Linux)
az appservice plan create --name asp-votify --resource-group rg-votify --sku B1 --is-linux

# Crear Web App con Docker
az webapp create --name app-votify --resource-group rg-votify --plan asp-votify --deployment-container-image-name acrvotify.azurecr.io/votify:latest

# Configurar variables
az webapp config appsettings set --name app-votify --resource-group rg-votify --settings DB_HOST=... DB_NAME=...
```

URL: `https://app-votify.azurewebsites.net`

---

## Comparativa

| Caracteristica | Container Apps | App Service |
|---|---|---|
| Precio gratis | 2M requests/mes | 1GB RAM, 1h CPU/dia |
| Escalado | Auto (incluso a 0) | Manual o auto limitado |
| SSL/HTTPS | Gratis automatico | Gratis automatico |
| Mejor para | Microservicios, APIs | Apps web tradicionales |

---

## Troubleshooting

### Error: "Cannot connect to database"
- Verifica que Supabase permite conexiones desde cualquier IP
- Ve a Supabase Dashboard: Database > Settings > Network > Allow all connections

### Error: "Container failed to start"
- Revisa los logs: `az containerapp logs show --name app-votify --resource-group rg-votify`
- Verifica que todas las variables de entorno estan configuradas

### Error: "502 Bad Gateway"
- El puerto del contenedor no coincide con el puerto expuesto
- Verifica que el Dockerfile usa EXPOSE 8080 y ASPNETCORE_URLS=http://+:8080