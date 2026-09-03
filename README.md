# EmployeeService

Cloud-neutral EmployeeService microservice using Clean Architecture, ASP.NET Core, PostgreSQL, EF Core, Docker, Kubernetes, and Helm.

## Projects

- src/EmployeeService.Api
- src/EmployeeService.Application
- src/EmployeeService.Domain
- src/EmployeeService.Infrastructure
- tests/EmployeeService.UnitTests
- tests/EmployeeService.IntegrationTests
- tests/EmployeeService.ArchitectureTests

## Quick start

1. Copy `.env.example` to `.env` and provide local values.
2. Start local stack:

```bash
docker compose up --build
```

3. Open Swagger:

```text
http://localhost:8080/swagger
```

## Migrations

```bash
dotnet ef migrations add InitialCreate --project src/EmployeeService.Infrastructure --startup-project src/EmployeeService.Api --output-dir Persistence/Migrations
dotnet ef database update --project src/EmployeeService.Infrastructure --startup-project src/EmployeeService.Api
```

## Notes

- Secrets are injected through environment variables only.
- Kubernetes secrets are template placeholders and must be populated at deploy time.
- Existing root-level sample API files are intentionally preserved while EmployeeService becomes the active deployable microservice.
