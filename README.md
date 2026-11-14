# JWT Authentication Demo API

A learning project demonstrating a basic authentication API built with ASP.NET Core and JSON Web Tokens (JWT). The project focuses on the server-side authentication flow and role information returned for registered users.

## Implemented API endpoints

The `AuthController` defines:

- `POST /api/auth/register` — registers a user with a username, password, confirmation password and role.
- `POST /api/auth/login` — checks the supplied username and password and returns a JWT with the username and role when the credentials match.

The roles represented in the registration flow are `Admin`, `Doctor` and `Assistant`. Passwords are hashed using BCrypt before being stored.

## Technology stack

- C#
- ASP.NET Core Web API (.NET 8)
- Entity Framework Core
- Microsoft SQL Server
- JWT bearer token packages
- BCrypt.Net-Next
- Swagger / OpenAPI

## Project structure

The solution is located in the `JWT/` directory:

- `JWT.sln` — Visual Studio solution
- `JWT/` — ASP.NET Core API project
- `Controllers/AuthController.cs` — registration and login endpoints
- `Data/ApplicationDbContext.cs` — Entity Framework Core database context
- `Entities/` — user and request DTO classes
- `Migrations/` — EF Core migration history

## Configuration

The API expects a SQL Server connection string named `DbConn` and JWT settings for the issuer, audience and signing key.

For local development, configure these values with .NET User Secrets or environment variables rather than relying on machine-specific settings:

```bash
dotnet user-secrets set "ConnectionStrings:DbConn" "YOUR_LOCAL_SQL_SERVER_CONNECTION_STRING"
dotnet user-secrets set "Jwt:Issuer" "YOUR_ISSUER"
dotnet user-secrets set "Jwt:Audience" "YOUR_AUDIENCE"
dotnet user-secrets set "Jwt:Key" "YOUR_DEVELOPMENT_SIGNING_KEY"
```

Do not use example or development credentials and signing keys for a deployed service.

## Run locally

From the API project directory:

```bash
cd JWT/JWT
dotnet restore
dotnet run
```

Swagger is configured for the development environment. Use the URL printed by the local ASP.NET Core process.

## Project scope

This repository is an educational authentication demo, not a production-ready identity platform. Review the current implementation and deployment configuration before using it beyond local learning and experimentation.

## Learning focus

The project provides a small example of:

- Defining registration and login request models
- Persisting user records with Entity Framework Core
- Hashing and verifying passwords with BCrypt
- Creating JWTs containing identity and role claims
- Describing an API with Swagger / OpenAPI

