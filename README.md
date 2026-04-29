# BookShelf API

Personal book library management API built with ASP.NET Core 10 and EF Core (SQLite).

## Quick start

**Docker (one command):**

```bash
docker compose up --build
```

→ <http://localhost:8080/swagger>

**Local (three commands):**

```bash
dotnet tool restore
dotnet ef database update --project src/BookShelfAPI.Infrastructure --startup-project src/BookShelfAPI
dotnet run --project src/BookShelfAPI
```

→ <http://localhost:5000/swagger>

The database is seeded with eight sample books on first start (idempotent).

## Endpoints

| Method | Path |
|--------|------|
| POST   | `/api/books` |
| GET    | `/api/books` (supports `?page=`, `?pageSize=`, `?status=`) |
| GET    | `/api/books/{id}` |
| PUT    | `/api/books/{id}` |
| DELETE | `/api/books/{id}` |
| PATCH  | `/api/books/{id}/status` |
| PATCH  | `/api/books/{id}/rating` |
| GET    | `/api/books/statistics` |

Full schemas, request/response shapes and status codes documented under `/swagger`. Errors follow [RFC 7807 ProblemDetails](https://datatracker.ietf.org/doc/html/rfc7807).

## Architecture

**Onion Architecture** — dependencies point inward only.

```
Domain  ←  Application  ←  Infrastructure / API
```

- All ports (repository, domain services, read-side projections) live in **Domain**.
- **Application** holds use cases as command/query handlers (`ICommandHandler`, `IQueryHandler`) and DTOs; it never references Infrastructure or API.
- **Infrastructure** is EF Core implementations of Domain ports.
- **API** is the composition root — controllers, middleware, request/response contracts. Does not reference `Domain.Entities`; entity-to-DTO mapping happens in Application.

### Key decisions

- **Manual migrations only** — `dotnet ef database update` is the sole entry point; the app never auto-migrates on startup. The Docker `migrator` service runs the same command before the API starts.
- **HTTP code mapping** — `201`/`204`/`400`/`404`/`409`/`422` mapped centrally by `DomainExceptionHandler` (an `IExceptionHandler` middleware), with `[ApiController]` covering 400 for malformed input.
- **Statistics as read-side projection** — separate `IBookStatisticsReader` port keeps reporting independent of the write model.
- **Sample seeding** — idempotent, gated by `SeedData` config flag (default `true`; integration tests set `false`).

## Libraries

| Library | Purpose | Reason |
|---------|---------|--------|
| ASP.NET Core 10 | Web host, MVC controllers | Built-in `[ApiController]` model validation, DI, OpenAPI integration |
| Entity Framework Core 10 (SQLite) | Persistence | Embedded — no external service, fits the "≤3-step setup" constraint |
| Swashbuckle.AspNetCore | OpenAPI + Swagger UI | One package; auto-generates `/swagger` from controller signatures |
| xUnit | Testing | De facto .NET standard, runs with `dotnet test` |
| Microsoft.AspNetCore.Mvc.Testing | Integration tests | Provides `WebApplicationFactory<Program>` for in-process HTTP-level tests |

Intentionally avoided: **Moq/NSubstitute** (one-method port → 5-line manual fake is clearer), **FluentAssertions** (v8 commercial license; built-in `Assert` is sufficient), **MediatR** (hand-rolled handler interfaces are four lines, no dependency needed for nine use cases).

## Tests

```bash
dotnet test
```

- `tests/BookShelfAPI.Domain.Tests/` — 5 unit tests covering business rules on `Book`.
- `tests/BookShelfAPI.IntegrationTests/` — 3 HTTP-level tests via `WebApplicationFactory<Program>` + SQLite in-memory.

## Assumptions

- **PUT scope** — updates catalog metadata only (title, author, isbn, year, description); reading state is exclusively managed via the dedicated `PATCH` endpoints.
- **List defaults** — alphabetical by title, `page=1`, `pageSize=20`, `pageSize` capped at 100.
- **Statistics — average rating** — computed only over Finished books with a rating set; `null` when none.
- **Statistics — current year & tiebreaks** — current year is UTC; top-authors tied on count are ordered alphabetically.
