FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files for restore
COPY Votify.sln global.json ./
COPY Votify.API/Votify.API.csproj Votify.API/
COPY Votify.Application/Votify.Application.csproj Votify.Application/
COPY Votify.Domain/Votify.Domain.csproj Votify.Domain/
COPY Votify.Infrastructure/Votify.Infrastructure.csproj Votify.Infrastructure/
COPY Votify.Client/Votify.Client.csproj Votify.Client/
COPY Votify.Tests/Votify.Tests.csproj Votify.Tests/

RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish Votify.API/Votify.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT:-8080}
ENTRYPOINT ["dotnet", "Votify.API.dll"]
