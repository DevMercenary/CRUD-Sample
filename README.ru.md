<div align="center">

# CRUD-Sample

**ASP.NET Core образец production-формы — clean architecture, EF Core, валидация, ProblemDetails**

[![CI](https://github.com/DevMercenary/CRUD-Sample/actions/workflows/ci.yml/badge.svg)](https://github.com/DevMercenary/CRUD-Sample/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![EF Core](https://img.shields.io/badge/EF_Core-10-7c3aed)](https://learn.microsoft.com/ef/core/)

[English version](README.md)

</div>

---

Компактное ASP.NET Core Web API, показывающее как выглядит *настоящий* CRUD-сервис:
слоистая архитектура, EF Core поверх SQLite, FluentValidation, RFC 7807 ProblemDetails,
пагинация, OpenAPI, интеграционные тесты на `WebApplicationFactory`, Docker-образ.
Всё помещается в два production-проекта + проект тестов — читается за один присест.

## Что показано

- **Слоистая архитектура** (`Core/{Domain, Application, Infrastructure}` + `Api`). Api
  зависит только от Application абстракций; EF Core упоминается только в Infrastructure.
- **Domain primitives**: value object `Email` гарантирует валидность при создании.
- **Sealed entity c private setters и factory-методом** — невалидный `User` создать
  невозможно.
- **Use-case сервисы** (`IUserService` + `UserResult` discriminated union из records)
  вместо контроллеров, тянущих репозиторий напрямую.
- **FluentValidation** в model binding — авто-`400` + ValidationProblemDetails.
- **RFC 7807 ProblemDetails** на каждую ошибку (`404`, `409`, `400`).
- **EF Core 10 + SQLite**, `Email` через `HasConversion`.
- **Compile-time логирование** (`[LoggerMessage]` source generator).
- **Нативный OpenAPI** (`Microsoft.AspNetCore.OpenApi`) + UI через **Scalar**.
- **Health checks** включая DbContext liveness.
- **Интеграционные тесты на `WebApplicationFactory`** c EF in-memory.
- **Minimal API endpoint** рядом с контроллером (`/api/v1/users/count`) — сравнить
  эргономику обоих стилей.
- **Dockerfile** (multistage, .NET 10) + **docker-compose** c SQLite-volume.

## Структура проекта

```
CRUD-Sample/
├── src/
│   ├── CRUD-Sample.Api/                ASP.NET Core хост
│   │   ├── Controllers/UsersController.cs
│   │   ├── ValidationExceptionHandler.cs
│   │   ├── Program.cs                  composition root
│   │   └── appsettings*.json
│   └── CRUD-Sample.Core/
│       ├── Domain/                     User, Email value object, IUserRepository, PageRequest
│       ├── Application/                DTOs, validators, IUserService, UserService
│       └── Infrastructure/             AppDbContext, EfUserRepository, DI-extension
└── tests/
    └── CRUD-Sample.Tests/              WebApplicationFactory + xUnit + FluentAssertions
```

## API

| Метод    | Маршрут                        | Описание                                    | Ответы                   |
|----------|--------------------------------|---------------------------------------------|--------------------------|
| `GET`    | `/api/v1/users`                | Пагинированный список (`page`, `pageSize`, `sortBy`, `desc`) | `200`     |
| `GET`    | `/api/v1/users/{id}`           | Один пользователь                           | `200` / `404`            |
| `POST`   | `/api/v1/users`                | Создать пользователя                        | `201` / `400` / `409`    |
| `PUT`    | `/api/v1/users/{id}`           | Обновить пользователя                       | `200` / `400` / `404` / `409` |
| `DELETE` | `/api/v1/users/{id}`           | Удалить пользователя                        | `204` / `404`            |
| `GET`    | `/api/v1/users/count`          | Minimal-API счётчик                         | `200`                    |
| `GET`    | `/health`                      | Liveness + DbContext check                  | `200` / `503`            |
| `GET`    | `/openapi/v1.json`             | OpenAPI документ                            | `200`                    |
| `GET`    | `/scalar/v1`                   | Интерактивный API explorer (только dev)     | `200`                    |

### Схема User

```jsonc
{
  "id":         12,
  "firstName":  "Ada",
  "lastName":   "Lovelace",
  "email":      "ada@example.com",
  "createdAt":  "2026-05-17T07:14:48Z"
}
```

`email` обязателен, должен быть корректным e-mail, уникален. `firstName` и `lastName`
обязательны, максимум 100 символов.

## Быстрый старт

```bash
dotnet restore
dotnet run --project src/CRUD-Sample.Api
```

Откройте <http://localhost:5080/scalar/v1>.

### Docker

```bash
docker compose up --build
# API на http://localhost:8080
```

## Разработка

```bash
dotnet format --verify-no-changes        # проверка стиля
dotnet build --configuration Release     # сборка
dotnet test                              # 10 интеграционных тестов
```

## Технологический стек

| Слой             | Библиотека                                                                                |
|------------------|-------------------------------------------------------------------------------------------|
| Framework        | ASP.NET Core 10                                                                           |
| Хранилище        | EF Core 10 + SQLite                                                                       |
| Валидация        | [FluentValidation](https://docs.fluentvalidation.net/) + AspNetCore integration          |
| OpenAPI          | `Microsoft.AspNetCore.OpenApi` + [Scalar](https://github.com/scalar/scalar)              |
| Health checks    | `Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore`                      |
| Тесты            | xUnit + FluentAssertions + `Microsoft.AspNetCore.Mvc.Testing` (WebApplicationFactory)    |

## Лицензия

[MIT](LICENSE).
