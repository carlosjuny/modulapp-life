# ModulApp Life

[![Backend CI](https://github.com/carlosjuny/modulapp-life/actions/workflows/backend.yml/badge.svg?branch=main)](https://github.com/carlosjuny/modulapp-life/actions/workflows/backend.yml)
[![Mobile CI](https://github.com/carlosjuny/modulapp-life/actions/workflows/mobile.yml/badge.svg?branch=main)](https://github.com/carlosjuny/modulapp-life/actions/workflows/mobile.yml)

Monorepo de ModulApp Life: backend .NET 10 (Clean Architecture) y frontend React Native con Expo, sobre SQL Server.

## Requisitos

| Herramienta | Versión | Para qué |
|---|---|---|
| Git | 2.40+ | Control de versiones |
| .NET SDK | 10 | `life-backend` |
| SQL Server | 2019+ (Developer, Express o LocalDB) | Base de datos |
| Node.js | 22 LTS | `life-frontend` |
| npm | 10+ | Dependencias del frontend |
| Expo Go o emulador Android/iOS | última | Probar la app móvil |
| EF Core CLI | `dotnet tool install --global dotnet-ef` | Migraciones |

## Estructura del monorepo

```
modulapp-life/
├── life-backend/            # .NET 10, Clean Architecture
│   ├── life-backend.slnx
│   ├── API/                 # Endpoints, Program.cs
│   ├── Application/         # Casos de uso
│   ├── Domain/              # Entidades y reglas (no referencia a nadie)
│   ├── Infrastructure/      # Servicios externos
│   └── Persistence/         # EF Core + SQL Server
├── life-frontend/           # React Native + TypeScript (Expo), también compila a web
├── docs/
│   └── arquitectura.md      # Diagrama y capas
├── .editorconfig
├── .gitignore               # Reglas comunes; cada proyecto tiene el suyo
└── README.md
```

Regla de dependencias del backend: `Domain` no referencia a nadie; `Application` → `Domain`;
`Infrastructure` y `Persistence` → `Application` y `Domain`; `API` → `Application`, `Infrastructure` y `Persistence`.
Más detalle en [docs/arquitectura.md](docs/arquitectura.md).

## Cómo correr el proyecto

Clonar y entrar a la rama de trabajo:

```bash
git clone git@github.com:carlosjuny/modulapp-life.git
cd modulapp-life
git checkout dev-backend    # o dev-frontend, según lo que vayas a tocar
```

### Atajo: todo con Docker Compose

Con Docker instalado, un solo comando levanta SQL Server y la API (aplica las migraciones al arrancar):

```bash
cp .env.example .env        # definir MSSQL_SA_PASSWORD
docker compose up --build
```

La API queda en `http://localhost:5187` (`/health`, `/swagger`). El frontend se corre aparte (sección 3) con
`life-frontend/.env.example` copiado a `.env.local`.

### Variables por ambiente

| Variable | dev | staging | producción |
|---|---|---|---|
| `ASPNETCORE_ENVIRONMENT` | `Development` | `Staging` | `Production` |
| `ConnectionStrings__DefaultConnection` | `.env` / user-secrets | secreto del entorno | secreto del entorno |
| `Cors__AllowedOrigins__N` | localhost (Expo) | dominio de staging | dominio de producción |
| `Database__MigrateOnStartup` | `true` | opcional | ignorada (las migraciones se aplican en el despliegue) |
| `Database__SeedOnStartup` | opcional | ignorada | ignorada |
| `Sentry__Dsn` | vacío (desactivado) | secreto del entorno | secreto del entorno |
| `Jwt__SigningKey` (mínimo 32 caracteres) | user-secrets / `.env` | secreto del entorno | secreto del entorno |
| `Jwt__Issuer` / `Jwt__Audience` / `Jwt__AccessTokenMinutes` / `Jwt__RefreshTokenDays` (30 por defecto) | valores por defecto de `appsettings.json` | idem | idem |
| `Observability__MetricsEnabled` | `true` | `true` | `true` (restringir `/metrics` a red interna) |
| `EXPO_PUBLIC_API_URL` / `EXPO_PUBLIC_APP_ENV` | local | URL de staging / `staging` | URL de producción / `production` |

Los secretos van siempre por variables de entorno (o user-secrets en local); `.env*` está ignorado por git y solo se
versionan los `.env.example`.

La API exige `Jwt:SigningKey` (mínimo 32 caracteres) y no arranca sin ella; `dotnet ef` también la necesita porque
carga `Program.cs`. En local se guarda con user-secrets (nunca se versiona):

```bash
cd life-backend/API
dotnet user-secrets set "Jwt:SigningKey" "<clave de al menos 32 caracteres>"
```

Con Docker Compose se define `JWT_SIGNING_KEY` en el `.env`. Para generar una clave aleatoria:
`openssl rand -base64 48`.

### 1. Base de datos

El backend ya referencia EF Core y SQL Server, pero **todavía no define `DbContext`, cadena de conexión ni
migraciones**. Cuando existan:

1. Tener una instancia de SQL Server disponible.
2. Guardar la cadena de conexión con user-secrets. **Nunca se versiona**:

   ```bash
   cd life-backend/API
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<cadena de conexión>"
   ```

   Alternativa: `appsettings.Development.local.json` (ignorado por git).
3. Cada integrante aplica las migraciones sobre su propia base local:

   ```bash
   dotnet ef database update --project Persistence --startup-project API
   ```

   (desde `life-backend/`)

### 2. Backend

```bash
cd life-backend
dotnet restore
dotnet build
dotnet run --project API
```

Los puertos están en `life-backend/API/Properties/launchSettings.json`. En desarrollo la API expone OpenAPI y
`life-backend/API/API.http` trae peticiones de ejemplo.

### 3. Frontend / mobile

```bash
cd life-frontend
npm install
npm start           # servidor de Expo (Expo Go o emulador)
npm run android     # abre en emulador Android
npm run ios         # abre en simulador iOS (solo macOS)
npm run web         # versión web
```

Antes de tocar APIs de Expo o React Native, leer [life-frontend/AGENTS.md](life-frontend/AGENTS.md).

### Verificación antes de abrir un pull request

| Proyecto | Comandos |
|---|---|
| Backend | `dotnet build` y `dotnet test` (desde `life-backend/`) |
| Frontend | `npx tsc --noEmit` y `npx expo lint` (desde `life-frontend/`) |

El backend tiene el proyecto de pruebas unitarias `life-backend/Application.Tests` (xUnit + NSubstitute, sin base de datos
real). Se ejecutan con `dotnet test life-backend.slnx` desde `life-backend/`. Convención: una carpeta por módulo de
`Application` (por ejemplo `EventsLog/`), una clase `<Clase>Tests` y pruebas con nombre
`Metodo_Condicion_ResultadoEsperado`, en el formato de 3 partes (preparación, qué se prueba, la prueba) separadas por una
línea en blanco. El frontend todavía no tiene tests automáticos.

Estas verificaciones también corren en GitHub Actions (`.github/workflows/backend.yml` y `mobile.yml`) en cada pull
request hacia `review` y `main`, y en push a `dev-backend` / `dev-frontend`. El backend ejecuta `dotnet restore`,
`build` y `test`; el mobile ejecuta `npm ci`, `tsc --noEmit` y, cuando existan los scripts, `lint` y `test`.

## Estrategia de ramas

| Rama | Propósito | Reglas |
|---|---|---|
| `main` | Código estable | Protegida. Solo recibe merges desde `review` |
| `review` | Revisión / QA | Recibe merges desde `dev-frontend` y `dev-backend` vía pull request |
| `dev-frontend` | Desarrollo del frontend/mobile | Trabajo diario del frontend |
| `dev-backend` | Desarrollo del backend | Trabajo diario del backend |
| `release` | Publicación de versiones | Se crea desde `main`; los tags de versión se publican desde ella. El esquema de versionado y si habrá una rama por año (`release/<año>`) se define más adelante |

Flujo:

```
dev-frontend ─┐
              ├─► review ─► main ─► release
dev-backend  ─┘
```

Nunca se trabaja ni se hace push directo a `main` ni a `review`: todo entra por pull request.

## Convención de commits

Cada commit referencia el ticket de Jira (proyecto `LIF`) y no mezcla tickets distintos:

```
LIF-<número> : <título del ticket> : <qué se hizo>
```

Ejemplo del historial:
`LIF-2 : Backend: solución .NET con Clean Architecture : Arquitectura backend`.

## Archivos de configuración por proyecto

| Archivo | Ubicación | Qué cubre |
|---|---|---|
| `.gitignore` | Raíz | SO, IDEs, `.env*`, secretos, logs |
| `.gitignore` | `life-backend/` | Salidas de .NET (`bin/`, `obj/`), user-secrets y configuración local |
| `.gitignore` | `life-frontend/` | Node, Expo, carpetas nativas generadas |
| `.editorconfig` | Raíz | Estilo de C# y TypeScript para todo el monorepo |

## Documentación

- [docs/arquitectura.md](docs/arquitectura.md): diagrama y capas del sistema.
- Notion → Documentación de Producto → 3.6 Entornos, calidad y despliegue.
