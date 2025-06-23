# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia os arquivos .csproj das dependências
COPY Catalog.Api/Catalog.Api.csproj Catalog.Api/
COPY Catalog.Application/Catalog.Application.csproj Catalog.Application/
COPY Catalog.Domain/Catalog.Domain.csproj Catalog.Domain/
COPY Catalog.Infrastructure/Catalog.Infrastructure.csproj Catalog.Infrastructure/

# Restaura dependências
RUN dotnet restore Catalog.Api/Catalog.Api.csproj

# Copia o restante do código
COPY . .

# Build do projeto
WORKDIR /src/Catalog.Api
RUN dotnet build Catalog.Api.csproj -c Release -o /app/build

# Etapa 2: Publish
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/build .

ENTRYPOINT ["dotnet", "Catalog.Api.dll"]