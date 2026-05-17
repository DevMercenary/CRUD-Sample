<div align="center">

# CRUD-Sample

**A production-shaped ASP.NET Core sample — clean architecture, EF Core, validation, ProblemDetails**

[![CI](https://github.com/DevMercenary/CRUD-Sample/actions/workflows/ci.yml/badge.svg)](https://github.com/DevMercenary/CRUD-Sample/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![EF Core](https://img.shields.io/badge/EF_Core-10-7c3aed)](https://learn.microsoft.com/ef/core/)

[Русская версия](README.ru.md)

</div>

---

A compact ASP.NET Core Web API that demonstrates how a *real* CRUD service is shaped:
layered architecture, EF Core on SQLite, FluentValidation, RFC 7807 ProblemDetails,
pagination, OpenAPI, integration tests via `WebApplicationFactory`, and a Docker image.
Everything fits in two production projects plus a test project — small enough to read
top-to-bottom in one sitting.

## What is showcased

- **Layered architecture** (`Core/{Domain, Application, Infrastructure}` + `Api`). The
  Api layer depends only on Application abstractions; Infrastructure is the only place
  that mentions EF Core.
- **Domain primitives**: `Email` value object enforces validity at construction.
- **Sealed entity with private setters and a factory method** — invalid `User`s can't
  exist.
- **Use-case-style services** (`IUserService` + `UserResult` discriminated union with
  records) instead of repository-leaking controllers.
- **FluentValidation** plugged into model binding for `400` + ValidationProblemDetails.
- **RFC 7807 ProblemDetails** for every error response (`404`, `409`, `400`).
- **EF Core 10 + SQLite**, with `Email` mapped via `HasConversion`.
- **Compile-time logging** (`[LoggerMessage]` source generator).
- **Native OpenAPI** (`Microsoft.AspNetCore.OpenApi`) served through **Scalar** UI.
- **Health checks** including DbContext liveness.
- **WebApplicationFactory** integration tests with EF in-memory database.
- **Minimal API** alongside the controller (`/api/v1/users/count`) for a side-by-side
  ergonomics comparison.
- **Dockerfile** (multistage, .NET 10 base) + **docker-compose** with a SQLite volume.

## Project layout

```
CRUD-Sample/
├── src/
│   ├── CRUD-Sample.Api/                ASP.NET Core host
│   │   ├── Controllers/UsersController.cs
│   │   ├── ValidationExceptionHandler.cs
│   │   ├── Program.cs                  composition root
│   │   └── appsettings*.json
│   └── CRUD-Sample.Core/
│       ├── Domain/                     User, Email value object, IUserRepository, PageRequest
│       ├── Application/                DTOs, validators, IUserService, UserService
│       └── Infrastructure/             AppDbContext, EfUserRepository, DI extension
└── tests/
    └── CRUD-Sample.Tests/              WebApplicationFactory + xUnit + FluentAssertions
```

## API

| Method   | Route                          | Description                                | Responses                |
|----------|--------------------------------|--------------------------------------------|--------------------------|
| `GET`    | `/api/v1/users`                | Paginated list (`page`, `pageSize`, `sortBy`, `desc`) | `200`            |
| `GET`    | `/api/v1/users/{id}`           | Get a single user                          | `200` / `404`            |
| `POST`   | `/api/v1/users`                | Create a user                              | `201` / `400` / `409`    |
| `PUT`    | `/api/v1/users/{id}`           | Update a user                              | `200` / `400` / `404` / `409` |
| `DELETE` | `/api/v1/users/{id}`           | Delete a user                              | `204` / `404`            |
| `GET`    | `/api/v1/users/count`          | Minimal-API counterpart — just the count   | `200`                    |
| `GET`    | `/health`                      | Liveness + DbContext check                 | `200` / `503`            |
| `GET`    | `/openapi/v1.json`             | OpenAPI document                           | `200`                    |
| `GET`    | `/scalar/v1`                   | Interactive API explorer (dev only)        | `200`                    |

### User schema

```jsonc
{
  "id":         12,
  "firstName":  "Ada",
  "lastName":   "Lovelace",
  "email":      "ada@example.com",
  "createdAt":  "2026-05-17T07:14:48Z"
}
```

`email` is required, must be a valid e-mail address, and is unique. `firstName` and
`lastName` are required, max 100 characters each.

## Quick start

```bash
dotnet restore
dotnet run --project src/CRUD-Sample.Api
```

Open <http://localhost:5080/scalar/v1> to explore the API.

### Docker

```bash
docker compose up --build
# API exposed on http://localhost:8080
```

## Development

```bash
dotnet format --verify-no-changes        # check style
dotnet build --configuration Release     # build
dotnet test                              # 10 integration tests
```

## Tech stack

| Layer            | Library                                                                                  |
|------------------|------------------------------------------------------------------------------------------|
| Framework        | ASP.NET Core 10                                                                          |
| Persistence      | EF Core 10 + SQLite                                                                      |
| Validation       | [FluentValidation](https://docs.fluentvalidation.net/) + AspNetCore integration         |
| OpenAPI          | `Microsoft.AspNetCore.OpenApi` + [Scalar](https://github.com/scalar/scalar)             |
| Health checks    | `Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore`                     |
| Tests            | xUnit + FluentAssertions + `Microsoft.AspNetCore.Mvc.Testing` (WebApplicationFactory)   |

## License

[MIT](LICENSE).
