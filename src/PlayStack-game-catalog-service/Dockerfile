# Base para runtime do app
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia apenas o .csproj para cache eficiente de restore
COPY PlayStack-game-catalog-service.csproj ./
RUN dotnet restore PlayStack-game-catalog-service.csproj

# Copia todo o c�digo ap�s restore
COPY . .

RUN dotnet build PlayStack-game-catalog-service.csproj -c $BUILD_CONFIGURATION -o /app/build

# Etapa de publica��o
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish PlayStack-game-catalog-service.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Imagem final, pronta para produ��o
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PlayStack-game-catalog-service.dll"]