# ElectroPi Task Manager API

<div align="center">

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?style=for-the-badge&logo=csharp)
![SQL Server](https://img.shields.io/badge/SQL_Server-2022-CC2927?style=for-the-badge&logo=microsoftsqlserver)
![Redis](https://img.shields.io/badge/Redis-7-DC382D?style=for-the-badge&logo=redis)
![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?style=for-the-badge&logo=docker)

A production-grade **Project & Task Management REST API** built with Clean Architecture, CQRS, MediatR, and JWT Authentication.

Built for the **ElectroPi** Backend .NET Developer Technical Assessment.

</div>

---

## Table of Contents

- [Architecture Overview](#architecture-overview)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Run with Docker](#run-with-docker-recommended)
  - [Run Locally](#run-locally)
- [Authentication](#authentication)
- [API Reference](#api-reference)
- [Running Tests](#running-tests)
- [Design Decisions](#design-decisions)
- [Bonus Features Implemented](#bonus-features-implemented)

---

## Architecture Overview

The solution follows **Clean Architecture** — dependencies point strictly inward. No outer layer can corrupt the domain model.

```
┌─────────────────────────────────────────────────┐
│                    API Layer                    │
│         Controllers · Middleware · Swagger      │
├─────────────────────────────────────────────────┤
│              Infrastructure Layer               │
│    EF Core · SQL Server · Redis · JWT · Identity│
├─────────────────────────────────────────────────┤
│              Application Layer                  │
│    CQRS · MediatR · FluentValidation · Mapster  │
├─────────────────────────────────────────────────┤
│                 Domain Layer                    │
│   Entities · Enums · Errors · Interfaces        │
│            ← Zero external dependencies →       │
└─────────────────────────────────────────────────┘
         Dependencies point INWARD only
```

### Request Flow

```
HTTP Request
    ↓
GlobalExceptionMiddleware   → catches all unhandled exceptions
    ↓
CorrelationIdMiddleware     → stamps X-Correlation-Id on every request
    ↓
JwtBearer Authentication    → validates Bearer token
    ↓
Controller                  → builds MediatR Command/Query
    ↓
MediatR Pipeline
  ├── LoggingBehavior        → logs request name + duration
  ├── ValidationBehavior     → runs FluentValidation
  └── CachingBehavior        → Redis cache check (queries only)
    ↓
Command / Query Handler     → business logic + UnitOfWork
    ↓
ApiResponseFilter           → wraps result in ApiResponse<T>
    ↓
HTTP Response (JSON)
```

---

## Technology Stack

| Concern              | Technology                          |
|----------------------|-------------------------------------|
| Runtime              | .NET 9                              |
| Web Framework        | ASP.NET Core Web API                |
| ORM                  | Entity Framework Core 9             |
| Database             | SQL Server 2022                     |
| Authentication       | JWT Bearer (HMAC-SHA256)            |
| Caching              | Redis 7 (StackExchange.Redis)       |
| Mediator             | MediatR 12                          |
| Validation           | FluentValidation 11                 |
| Object Mapping       | Mapster 10                          |
| API Documentation    | Swagger / OpenAPI (Swashbuckle)     |
| API Versioning       | Asp.Versioning.Mvc 8                |
| Containerisation     | Docker + Docker Compose             |
| Testing              | xUnit + Moq + FluentAssertions      |

---

## Project Structure

```
ElectroPi.TaskManager.Solution/
├── src/
│   ├── ElectroPi.TaskManager.Domain/           # Entities, enums, interfaces — zero dependencies
│   ├── ElectroPi.TaskManager.Application/      # CQRS handlers, validators, DTOs
│   ├── ElectroPi.TaskManager.Infrastructure/   # EF Core, Redis, JWT, Identity
│   └── ElectroPi.TaskManager.API/              # Controllers, middleware, Program.cs
├── tests/
│   ├── ElectroPi.TaskManager.Domain.Tests/     # Entity behaviour tests
│   └── ElectroPi.TaskManager.Application.Tests/# Handler + validator tests
├── Dockerfile
├── docker-compose.yml
├── docker-compose.override.yml
└── ElectroPi.TaskManager.Solution.sln
```

---

## Getting Started

### Prerequisites

| Tool           | Minimum Version | Download                               |
|----------------|-----------------|----------------------------------------|
| .NET SDK       | 9.0             | https://dotnet.microsoft.com/download  |
| Docker Desktop | 4.x             | https://www.docker.com/products/docker-desktop |

> SQL Server and Redis do **not** need to be installed locally — Docker provisions both automatically.

---

### Run with Docker (Recommended)

```bash
# 1. Clone the repository
git clone https://github.com/mariammahboub/ElectroPi.TaskManager.git
cd ElectroPi.TaskManager

# 2. Start all services (API + SQL Server + Redis)
docker compose up --build

# 3. Open Swagger UI
# http://localhost:5000/swagger
```

> On first run, EF Core migrations and role seeding run automatically. No manual database setup required.

---

### Run Locally

```bash
# 1. Start infrastructure only
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=ElectroPi@Strong123" \
  -p 1433:1433 --name electropi-sqlserver \
  -d mcr.microsoft.com/mssql/server:2022-latest

docker run -p 6379:6379 --name electropi-redis -d redis:7-alpine

# 2. Run the API
dotnet run --project ElectroPi.TaskManager.API

# 3. Open Swagger UI
# http://localhost:5084/swagger
```

**Next time you start your machine:**
```bash
docker start electropi-sqlserver
docker start electropi-redis
dotnet run --project ElectroPi.TaskManager.API
```

---

## Authentication

All endpoints except `POST /api/v1/auth/register` and `POST /api/v1/auth/login` require a **Bearer token**.

```
Authorization: Bearer <your_jwt_token>
```

**Token lifetime:** 60 minutes (configurable via `Jwt:ExpiryMinutes`).

**Workflow:**
1. `POST /api/v1/auth/register` → receive token
2. Copy the `token` value from the response
3. Add `Authorization: Bearer <token>` to all subsequent requests

---

## API Reference

### Base URL
```
http://localhost:5084/api/v1
```

### Standard Response Envelope

Every response — success or failure — uses this envelope:

```json
{
  "success": true,
  "message": "Request completed successfully.",
  "data": {},
  "errors": null,
  "statusCode": 200,
  "traceId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

---

### Auth Endpoints

| Method | Endpoint               | Description          | Auth |
|--------|------------------------|----------------------|------|
| POST   | `/auth/register`       | Register new user    | ❌   |
| POST   | `/auth/login`          | Login + get token    | ❌   |

**Register Request:**
```json
{
  "fullName": "Jane Doe",
  "email": "jane@electropi.com",
  "password": "StrongPass@1",
  "confirmPassword": "StrongPass@1"
}
```

---

### Projects Endpoints

| Method | Endpoint              | Description               | Auth |
|--------|-----------------------|---------------------------|------|
| GET    | `/projects`           | Get all my projects       | ✅   |
| POST   | `/projects`           | Create a project          | ✅   |
| GET    | `/projects/{id}`      | Get project by ID         | ✅   |
| PUT    | `/projects/{id}`      | Update project            | ✅   |
| DELETE | `/projects/{id}`      | Delete project + tasks    | ✅   |

**Create Project Request:**
```json
{
  "name": "ElectroPi Mobile App",
  "description": "Cross-platform mobile application"
}
```

---

### Tasks Endpoints

All task endpoints are nested under their parent project.

| Method | Endpoint                                     | Description          | Auth |
|--------|----------------------------------------------|----------------------|------|
| GET    | `/projects/{id}/tasks`                       | Get all tasks        | ✅   |
| POST   | `/projects/{id}/tasks`                       | Create a task        | ✅   |
| PATCH  | `/projects/{id}/tasks/{taskId}/status`       | Update task status   | ✅   |
| DELETE | `/projects/{id}/tasks/{taskId}`              | Delete a task        | ✅   |

**Create Task Request:**
```json
{
  "title": "Implement login screen",
  "description": "Build UI and wire up the API",
  "priority": 3,
  "dueDate": "2026-12-31T00:00:00Z"
}
```

Priority values: `1` = Low · `2` = Medium · `3` = High · `4` = Critical

**Update Task Status Request:**
```json
{
  "newStatus": 2
}
```

Status values: `1` = Todo · `2` = InProgress · `3` = Done

> ⚠️ Status transitions are **forward-only**: Todo → InProgress → Done. Backward transitions return HTTP 400.

---

### Health Check

```
GET /health
```

```json
{
  "status": "Healthy",
  "checks": [
    { "name": "sql-server", "status": "Healthy", "duration": "12ms" },
    { "name": "redis",      "status": "Healthy", "duration": "3ms"  }
  ]
}
```

---

## Running Tests

```bash
dotnet test
```

**Expected output:**
```
128 Tests (128 Passed, 0 Failed, 0 Skipped)
```

Test coverage includes:
- **Domain Tests** — Entity behaviour, state machine validation, domain errors
- **Application Tests** — Command/Query handlers (Moq), FluentValidation rules

---

## Design Decisions

### 1. `ProjectTask` not `Task`
Named `ProjectTask` to avoid collision with `System.Threading.Tasks.Task`.

### 2. Tasks created through `Project.AddTask()`
Tasks cannot be instantiated directly — they can only exist inside a project. The `internal` constructor on `ProjectTask` enforces this aggregate boundary at compile time.

### 3. Status transitions are forward-only
`Todo → InProgress → Done`. The domain entity enforces this rule. A `DomainError` is thrown on invalid transitions and translated to HTTP 400 by the Application layer.

### 4. Redis with graceful fallback
Cache failures log a warning but never crash a request. The system degrades to always hitting the database rather than returning errors.

### 5. Commands eliminated redundant Request DTOs
Commands serve as both the MediatR message and the HTTP request body. The controller enriches them with JWT claims (`OwnerId`, `RequestingUserId`) that the body must never carry.

### 6. `UnitOfWork` as the single commit point
No `SaveChanges` is ever called inside a repository. Every command handler calls `_unitOfWork.SaveChangesAsync()` exactly once after all mutations.

---

## Bonus Features Implemented

| Feature                   | Details                                                   |
|---------------------------|-----------------------------------------------------------|
| ✅ CQRS + MediatR         | Separate Commands/Queries, 3-stage pipeline               |
| ✅ Docker                 | Multi-stage Dockerfile + docker-compose with health checks|
| ✅ Unit Tests             | 128 tests — domain entity + application handler coverage  |
| ✅ Redis Caching          | `CacheService` + `CachingBehavior` with graceful fallback |
| ✅ Generic Response Wrapper | `ApiResponse<T>` on every endpoint via `ApiResponseFilter`|
| ✅ Role-based Authorization | Admin / Member roles seeded via ASP.NET Core Identity   |
| ✅ API Versioning         | URL segment `/api/v1/` + header + query string            |

---

## Database Migration Files

Migration files are located at:
```
ElectroPi.TaskManager.Infrastructure/
└── Migrations/
    ├── 20260519192258_InitialCreate.cs
    └── 20260519193210_FixTaskStatusDefault.cs
```

To apply migrations manually:
```bash
dotnet ef database update \
  --project ElectroPi.TaskManager.Infrastructure \
  --startup-project ElectroPi.TaskManager.API
```

---

<div align="center">

Built with by **Mariam Mahboub** Backend .NET Developer

</div>
