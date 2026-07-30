# EmployeeApi

A simple ASP.NET Core Web API project for managing employee data.

## Overview

- Framework: .NET 10
- API Controller: `Controllers/EmployeeController.cs`
- Swagger supported via `Swashbuckle.AspNetCore`

## Running locally

```bash
dotnet run
```

Then open Swagger UI at `https://localhost:<port>/swagger`.

## API Endpoints

- `GET /api/Employee` - returns a sample list of employees

## Notes

- The project includes a `catalog-info.yaml` for Backstage metadata.
