# Marcas Autos Backend

API REST desarrollada en C#/.NET para una prueba técnica de backend.

El proyecto implementa una gestión básica de marcas de autos utilizando ASP.NET Core, Entity Framework Core, PostgreSQL, Docker Compose, Swagger/OpenAPI, xUnit, Coverlet, DDD/Clean Architecture y MassTransit con transporte InMemory.

## Requisitos

Para ejecutar el proyecto se necesita tener instalado:

- .NET SDK
- Docker Desktop
- Docker Compose
- Entity Framework CLI

Instalar Entity Framework CLI:

```bash
dotnet tool install --global dotnet-ef
```

Si ya está instalado:

```bash
dotnet tool update --global dotnet-ef
```

Si el comando `dotnet ef` no funciona, agregar las herramientas de .NET al `PATH`:

```bash
export PATH="$PATH:$HOME/.dotnet/tools"
```

## Estructura del proyecto

```txt
marcas-autos-backend/
  MarcasAutosBackend.sln
  docker-compose.yml
  README.md

  src/
    MarcasAutos.Api/
      Controllers/
        MarcasAutosController.cs
      Program.cs
      appsettings.json
      appsettings.Development.json
      appsettings.Docker.json
      Dockerfile

    MarcasAutos.Application/
      Common/
        IMessageBus.cs
        IUnitOfWork.cs
      MarcasAutos/
        CreateMarcaAutoRequest.cs
        CreateMarcaAutoUseCase.cs
        GetMarcasAutosUseCase.cs
        MarcaAutoResponse.cs
        Events/
          MarcaAutoCreadaEvent.cs
      DependencyInjection.cs

    MarcasAutos.Domain/
      MarcasAutos/
        MarcaAuto.cs
        IMarcaAutoRepository.cs

    MarcasAutos.Infrastructure/
      Messaging/
        MarcaAutoCreadaConsumer.cs
        MassTransitMessageBus.cs
      Persistence/
        AppDbContext.cs
        UnitOfWork.cs
        Migrations/
      Repositories/
        MarcaAutoRepository.cs
      DependencyInjection.cs

  tests/
    MarcasAutos.Tests/
      MarcasAutosControllerTest.cs
```

## Arquitectura

El proyecto está organizado usando una estructura inspirada en DDD y Clean Architecture.

### Capas

- `MarcasAutos.Domain`: contiene el modelo de dominio, entidades, aggregate roots y contratos principales.
- `MarcasAutos.Application`: contiene casos de uso, DTOs, eventos y abstracciones de aplicación.
- `MarcasAutos.Infrastructure`: contiene Entity Framework, PostgreSQL, repositorios, Unit of Work y mensajería.
- `MarcasAutos.Api`: contiene controllers, Swagger, configuración HTTP y arranque de la aplicación.
- `MarcasAutos.Tests`: contiene pruebas unitarias con xUnit.

## DDD

El bounded context de esta prueba es la gestión de marcas de autos.

### Aggregate

- `MarcaAuto`

### Aggregate Root

- `MarcaAuto`

La entidad `MarcaAuto` encapsula reglas de negocio simples, como validar que el nombre sea requerido y evitar modificaciones directas mediante setters públicos.

Ejemplo de reglas incluidas:

- El nombre de la marca no puede estar vacío.
- El nombre se normaliza con `Trim()`.
- La modificación del nombre se realiza mediante un método explícito del dominio: `ActualizarNombre`.

## SOLID

El proyecto aplica principios SOLID de la siguiente manera:

- **Single Responsibility Principle**: los controllers solo manejan HTTP; los casos de uso coordinan acciones; los repositorios encapsulan persistencia; el dominio contiene reglas de negocio.
- **Open/Closed Principle**: se pueden agregar nuevos casos de uso sin modificar la estructura principal.
- **Liskov Substitution Principle**: las abstracciones permiten reemplazar implementaciones concretas por dobles de prueba.
- **Interface Segregation Principle**: las interfaces son pequeñas y enfocadas, por ejemplo `IMarcaAutoRepository`, `IUnitOfWork` e `IMessageBus`.
- **Dependency Inversion Principle**: la capa de aplicación depende de abstracciones, no directamente de Entity Framework ni MassTransit.

## Base de datos

La base de datos utilizada es PostgreSQL.

La tabla principal es:

```txt
MarcasAutos
```

El proyecto utiliza Entity Framework Core para:

- Configuración del `DbContext`.
- Migraciones.
- Seed de datos.
- Consultas hacia PostgreSQL.

## Seed de datos

La tabla `MarcasAutos` se carga inicialmente con tres marcas:

```txt
Toyota
Honda
Nissan
```

El seed se encuentra configurado en:

```txt
src/MarcasAutos.Infrastructure/Persistence/AppDbContext.cs
```

## Configuración por entorno

### Desarrollo local

Archivo:

```txt
src/MarcasAutos.Api/appsettings.Development.json
```

Connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=marcas_autos;Username=postgres;Password=postgres"
  }
}
```

### Docker

Archivo:

```txt
src/MarcasAutos.Api/appsettings.Docker.json
```

Connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db;Port=5432;Database=marcas_autos;Username=postgres;Password=postgres"
  }
}
```

La diferencia importante es:

- En desarrollo local se usa `Host=localhost`.
- Dentro de Docker se usa `Host=db`, porque `db` es el nombre del servicio PostgreSQL en `docker-compose.yml`.

## Ejecutar con Docker Compose

Desde la raíz del proyecto:

```bash
docker compose up --build
```

Esto levanta:

- `db`: servicio PostgreSQL.
- `api`: servicio ASP.NET Core.

La API queda disponible en:

```txt
http://localhost:8080
```

## Swagger

La documentación interactiva de la API está disponible en:

```txt
http://localhost:8080/swagger
```

Desde Swagger se pueden probar los endpoints disponibles.

## Endpoints

### Obtener todas las marcas

```txt
GET /api/MarcasAutos
```

Ejemplo:

```bash
curl http://localhost:8080/api/MarcasAutos
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

### Crear una marca

```txt
POST /api/MarcasAutos
```

Ejemplo:

```bash
curl -X POST http://localhost:8080/api/MarcasAutos \
  -H "Content-Type: application/json" \
  -d '{"nombre":"Mazda"}'
```

Respuesta esperada:

```json
{
  "id": 4,
  "nombre": "Mazda"
}
```

## Migraciones

Crear una nueva migración:

```bash
dotnet ef migrations add NombreDeLaMigracion \
  --project src/MarcasAutos.Infrastructure \
  --startup-project src/MarcasAutos.Api \
  --output-dir Persistence/Migrations
```

Aplicar migraciones manualmente:

```bash
ASPNETCORE_ENVIRONMENT=Development dotnet ef database update \
  --project src/MarcasAutos.Infrastructure \
  --startup-project src/MarcasAutos.Api
```

El proyecto también ejecuta migraciones automáticamente al iniciar la API, mediante:

```csharp
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}
```

Esto permite que `docker compose up --build` prepare la base de datos automáticamente.

## MassTransit

El proyecto incluye MassTransit usando transporte InMemory.

Se publica un evento cuando se crea una marca de auto:

```txt
MarcaAutoCreadaEvent
```

Consumer configurado:

```txt
MarcaAutoCreadaConsumer
```

Esto permite demostrar una base event-driven sin agregar RabbitMQ ni servicios externos adicionales.

## Ejecutar pruebas

Desde la raíz del proyecto:

```bash
dotnet test
```

Las pruebas utilizan xUnit y Entity Framework InMemory, por lo que no modifican la base de datos real de desarrollo.

## Cobertura mínima requerida: 90%

Para esta entrega se requiere validar **90% de cobertura**.

Ejecutar pruebas con cobertura mínima del 90%:

```bash
dotnet test \
  /p:CollectCoverage=true \
  /p:CoverletOutputFormat=cobertura \
  /p:ExcludeByFile="**/Program.cs%2c**/DependencyInjection.cs%2c**/Messaging/*.cs%2c**/Persistence/Migrations/*.cs%2c**/obj/**/*.cs" \
  /p:Threshold=90 \
  /p:ThresholdType=line \
  /p:ThresholdStat=total
```

Si la cobertura total de líneas está por debajo del 90%, el comando falla. Si el comando finaliza con `Build succeeded`, significa que la cobertura cumple o supera el mínimo requerido.

Se excluyen de la medición archivos que corresponden a configuración, wiring técnico o código generado:

- `Program.cs`
- `DependencyInjection.cs`
- `Messaging/*.cs`
- `Persistence/Migrations/*.cs`
- `obj/**/*.cs`

La lógica principal cubierta por pruebas incluye:

- Entidad de dominio `MarcaAuto`.
- Casos de uso `GetMarcasAutosUseCase` y `CreateMarcaAutoUseCase`.
- Repositorio `MarcaAutoRepository`.
- Unit of Work.
- Controller `MarcasAutosController`.
- Publicación de evento mediante una implementación fake de `IMessageBus`.

## Reporte visual de cobertura

Opcionalmente, se puede generar un reporte HTML para revisar la cobertura de forma visual.

Instalar la herramienta `reportgenerator`:

```bash
dotnet tool install --global dotnet-reportgenerator-globaltool
```

Si ya está instalada:

```bash
dotnet tool update --global dotnet-reportgenerator-globaltool
```

Generar el reporte HTML a partir del archivo `coverage.cobertura.xml`:

```bash
reportgenerator \
  -reports:"**/coverage.cobertura.xml" \
  -targetdir:"coverage-report" \
  -reporttypes:"Html;TextSummary"
```

Ver el resumen en consola:

```bash
cat coverage-report/Summary.txt
```

Abrir el reporte visual en macOS:

```bash
open coverage-report/index.html
```

El archivo `coverage-report/index.html` permite revisar la cobertura por proyecto, archivo y línea de código.

> Nota: `coverage-report/` es un artefacto generado localmente y no debería subirse al repositorio.

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

Detener contenedores:

```bash
docker compose down
```

Detener contenedores y eliminar volumen de base de datos:

```bash
docker compose down -v
```

> Nota: `docker compose down -v` elimina los datos guardados en PostgreSQL. Es útil para probar desde cero, pero debe usarse con cuidado.

## Validación final recomendada

Antes de entregar o subir cambios finales:

```bash
dotnet build
dotnet test
dotnet test \
  /p:CollectCoverage=true \
  /p:CoverletOutputFormat=cobertura \
  /p:ExcludeByFile="**/Program.cs%2c**/DependencyInjection.cs%2c**/Messaging/*.cs%2c**/Persistence/Migrations/*.cs%2c**/obj/**/*.cs" \
  /p:Threshold=90 \
  /p:ThresholdType=line \
  /p:ThresholdStat=total

reportgenerator \
  -reports:"**/coverage.cobertura.xml" \
  -targetdir:"coverage-report" \
  -reporttypes:"Html;TextSummary"

docker compose down -v
docker compose up --build
```

En otra terminal:

```bash
curl http://localhost:8080/api/MarcasAutos

curl -X POST http://localhost:8080/api/MarcasAutos \
  -H "Content-Type: application/json" \
  -d '{"nombre":"Mazda"}'
```

También validar Swagger en:

```txt
http://localhost:8080/swagger
```

Y, opcionalmente, abrir el reporte de cobertura:

```bash
open coverage-report/index.html
```
