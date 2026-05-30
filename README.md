# Marcas Autos Backend

API REST desarrollada en C#/.NET para la prueba técnica de backend.  
El proyecto utiliza Entity Framework Core, PostgreSQL, Docker Compose y pruebas unitarias con xUnit.

## Requisitos

Antes de ejecutar el proyecto, asegúrate de tener instalado:

- .NET SDK
- Docker Desktop
- Docker Compose
- Entity Framework CLI

Para instalar o actualizar la herramienta de Entity Framework:

```bash
dotnet tool install --global dotnet-ef
```

Si ya está instalada:

```bash
dotnet tool update --global dotnet-ef
```

Si el comando `dotnet ef` no funciona, agrega las herramientas de .NET al `PATH`:

```bash
export PATH="$PATH:$HOME/.dotnet/tools"
```

## Estructura general del proyecto

```txt
marcas-autos-backend/
  MarcasAutosBackend.sln
  docker-compose.yml

  MarcasAutos.Api/
    Controllers/
      MarcasAutosController.cs
    Data/
      AppDbContext.cs
    Models/
      MarcaAuto.cs
    Migrations/
    Program.cs
    appsettings.json
    appsettings.Development.json
    appsettings.Docker.json
    Dockerfile

  MarcasAutos.Tests/
    MarcasAutosControllerTest.cs
```

## Variables de configuración

El proyecto utiliza diferentes archivos de configuración según el entorno:

### Desarrollo local

`MarcasAutos.Api/appsettings.Development.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=marcas_autos;Username=postgres;Password=postgres"
  }
}
```

### Docker

`MarcasAutos.Api/appsettings.Docker.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db;Port=5432;Database=marcas_autos;Username=postgres;Password=postgres"
  }
}
```

La diferencia importante es:

- En desarrollo local se usa `Host=localhost`.
- Dentro de Docker se usa `Host=db`, porque `db` es el nombre del servicio de PostgreSQL en Docker Compose.

## Ejecutar el proyecto con Docker Compose

Desde la raíz del proyecto:

```bash
docker compose up --build
```

Esto levanta dos servicios:

- `db`: contenedor de PostgreSQL.
- `api`: contenedor de la API REST.

La API queda disponible en:

```txt
http://localhost:8080
```

Endpoint principal:

```txt
GET http://localhost:8080/api/MarcasAutos
```

Respuesta esperada:

```json
[
  {
    "id": 1,
    "nombre": "Toyota"
  },
  {
    "id": 2,
    "nombre": "Honda"
  },
  {
    "id": 3,
    "nombre": "Nissan"
  }
]
```

## Ejecutar solamente la base de datos con Docker

Si deseas correr la API localmente con `dotnet run`, puedes levantar solamente PostgreSQL:

```bash
docker compose up -d db
```

Luego ejecuta la API:

```bash
dotnet run --project MarcasAutos.Api
```

## Migraciones de Entity Framework

Para crear una nueva migración:

```bash
dotnet ef migrations add NombreDeLaMigracion \
  --project MarcasAutos.Api \
  --startup-project MarcasAutos.Api
```

Para aplicar las migraciones manualmente:

```bash
dotnet ef database update \
  --project MarcasAutos.Api \
  --startup-project MarcasAutos.Api
```

El proyecto también puede ejecutar migraciones automáticamente al iniciar la API si `Program.cs` incluye:

```csharp
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}
```

Esto permite que, al ejecutar `docker compose up --build`, la base de datos quede preparada automáticamente.

## Seed de datos

La tabla `MarcasAutos` se carga con datos iniciales mediante Entity Framework en `AppDbContext`.

Ejemplo de datos iniciales:

```txt
Toyota
Honda
Nissan
```

## Ejecutar pruebas unitarias

Desde la raíz del proyecto:

```bash
dotnet test
```

Las pruebas utilizan una base de datos en memoria con Entity Framework, por lo que no modifican la base de datos de desarrollo.

## Ejecutar pruebas con cobertura

```bash
dotnet test --collect:"XPlat Code Coverage"
```

El resultado de cobertura se genera dentro de la carpeta `TestResults`.

## Comandos útiles

Ver contenedores activos:

```bash
docker compose ps
```

Ver logs de la API:

```bash
docker compose logs api
```

Ver logs de PostgreSQL:

```bash
docker compose logs db
```

Detener los contenedores:

```bash
docker compose down
```

Detener contenedores y eliminar el volumen de datos:

```bash
docker compose down -v
```

> Nota: `docker compose down -v` elimina los datos de PostgreSQL guardados en el volumen. Úsalo solamente si deseas reiniciar la base de datos desde cero.

## Notas técnicas

- La API utiliza ASP.NET Core.
- Entity Framework Core se utiliza como ORM.
- PostgreSQL se ejecuta en Docker.
- Las pruebas unitarias están implementadas con xUnit.
- Para evitar modificar la base de datos real durante las pruebas, se utiliza `UseInMemoryDatabase`.
- En Docker, la API se conecta a PostgreSQL usando el nombre del servicio `db`.
