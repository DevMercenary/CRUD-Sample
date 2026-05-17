<div align="center">

# CRUD-Sample

**Компактный пример ASP.NET Core с чистым REST-дизайном**

[![CI](https://github.com/DevMercenary/CRUD-Sample/actions/workflows/ci.yml/badge.svg)](https://github.com/DevMercenary/CRUD-Sample/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)

[English version](README.md)

</div>

---

Небольшое, но грамотно структурированное Web API на ASP.NET Core c CRUD-операциями над
ресурсом `User`. Проект задумывался как образец идиоматичного ASP.NET Core — контроллеры
с DI, биндинг и валидация моделей, OpenAPI и корректные HTTP-коды ответов.

## Возможности

- **Атрибутно-маршрутизируемый `ApiController`** с документацией на уровне действий.
- **Репозиторий, инжектируемый через конструктор**, регистрируется через встроенный DI.
- **Валидация через data-annotations** (`[Required]`, `[EmailAddress]`) +
  автоматический `400 Bad Request` от `ModelState`.
- **OpenAPI** через Swashbuckle и Swagger UI в режиме разработки.
- **Корректные коды ответов**: `200 OK`, `201 Created` c `Location`, `204 No Content`,
  `404 Not Found`, `400 Bad Request`.

## Эндпоинты

| Метод    | Маршрут              | Описание                | Ответы             |
|----------|----------------------|-------------------------|--------------------|
| `GET`    | `/api/users`         | Список пользователей    | `200 OK`           |
| `GET`    | `/api/users/{id}`    | Пользователь по id      | `200 OK` / `404`   |
| `POST`   | `/api/users`         | Создать пользователя    | `201 Created`      |
| `PUT`    | `/api/users/{id}`    | Обновить пользователя   | `204 No Content`   |
| `DELETE` | `/api/users/{id}`    | Удалить пользователя    | `204 No Content`   |

### Схема User

```jsonc
{
  "id":        12,
  "firstName": "Ada",
  "lastName":  "Lovelace",
  "email":     "ada@example.com"
}
```

`email` обязателен и должен быть корректным e-mail; `firstName` и `lastName` обязательны.

## Быстрый старт

```bash
dotnet restore
dotnet run --project CRUD-Sample
```

Откройте <http://localhost:5000/swagger> для просмотра API.

## Структура проекта

```
CRUD-Sample/
├── Controllers/
│   └── UserController.cs    # ApiController, маршруты /api/users
├── Data/
│   └── UserRepository.cs    # Хранилище (in-memory)
├── Models/
│   └── User.cs              # Модель c аннотационной валидацией
├── Program.cs               # Composition root, Swagger
├── appsettings.json         # Конфигурация
└── CRUD-Sample.csproj
```

## Технологический стек

| Слой        | Библиотека                                                                          |
|-------------|-------------------------------------------------------------------------------------|
| Framework   | ASP.NET Core 10                                                                     |
| OpenAPI     | [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) |
| Хранилище   | In-memory (`List<User>`)                                                            |

## Разработка

```bash
dotnet format --verify-no-changes        # проверка стиля
dotnet build --configuration Release     # сборка
dotnet test                              # тесты
```

## Roadmap

Репозиторий планомерно расширяется до полноценного «production-shaped» образца:

- Разделение по слоям (`Api` / `Application` / `Domain` / `Infrastructure`).
- EF Core 9 + SQLite c миграциями.
- FluentValidation и RFC 7807 ProblemDetails для ошибок.
- Версионирование API, пагинация, фильтрация и сортировка.
- Интеграционные тесты на `WebApplicationFactory`.
- Dockerfile и `docker-compose.yml`.

## Лицензия

[MIT](LICENSE).
