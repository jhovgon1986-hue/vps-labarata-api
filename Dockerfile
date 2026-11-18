# 1. Etapa de construcción (SDK 10.0)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

# Copiar todo y restaurar dependencias
COPY . .
RUN dotnet restore

# Compilar en modo Release
RUN dotnet publish -c Release -o /app/publish

# 2. Etapa de Producción (Solo el Runtime 10.0, más ligero)
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

# Exponer el puerto estándar de ASP.NET Core
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Ejecutar la API
ENTRYPOINT ["dotnet", "VPS_LaBarata.API.dll"]