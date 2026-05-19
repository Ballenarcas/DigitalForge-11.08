# Stage 1: Base image for the final runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Stage 2: Build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy all .csproj files and the .sln file
COPY ["Votify.sln", "./"]
COPY ["Votify.API/Votify.API.csproj", "Votify.API/"]
COPY ["Votify.Application/Votify.Application.csproj", "Votify.Application/"]
COPY ["Votify.Client/Votify.Client.csproj", "Votify.Client/"]
COPY ["Votify.Domain/Votify.Domain.csproj", "Votify.Domain/"]
COPY ["Votify.Infrastructure/Votify.Infrastructure.csproj", "Votify.Infrastructure/"]
COPY ["Votify.Tests/Votify.Tests.csproj", "Votify.Tests/"]
# You can add the acceptance tests project here if it exists
# COPY ["Votify.AcceptanceTests/Votify.AcceptanceTests.csproj", "Votify.AcceptanceTests/"]

# Restore dependencies for the entire solution
RUN dotnet restore "Votify.sln"

# Copy the rest of the source code
COPY . .

# Build the API project specifically
WORKDIR "/src/Votify.API"
RUN dotnet build "Votify.API.csproj" -c Release -o /app/build

# Stage 3: Publish the application
FROM build AS publish
RUN dotnet publish "Votify.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 4: Final image with the runtime and published app
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Votify.API.dll"]
