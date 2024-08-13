# Imagen oficial de Microsoft para .NET 6 SDK como imagen base
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

# directorio de trabajo en el contenedor
WORKDIR /app

# .csproj y restaura las dependencias
COPY *.csproj ./
RUN dotnet restore

# Copia el resto de los archivos del proyecto
COPY . ./

# Compila la aplicación en modo Release
RUN dotnet publish -c Release -o out

# Genera la imagen final utilizando la imagen de runtime de .NET 6
FROM mcr.microsoft.com/dotnet/aspnet:6.0

# Directorio de trabajo en el contenedor
WORKDIR /app

# Archivos binarios desde la imagen de build
COPY --from=build-env /app/out .

# Punto de entrada de la aplicación
ENTRYPOINT ["dotnet", "data-agent-service.dll"]
