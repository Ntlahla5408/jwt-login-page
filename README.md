# JWT Login and Role-Based Access API

A learning project demonstrating JWT authentication and role-based authorization with an ASP.NET Core Web API, SQL Server, BCrypt password hashing, and a React frontend.

## Technology

- ASP.NET Core Web API (.NET 8)
- C# and Entity Framework Core
- SQL Server
- JWT bearer authentication
- BCrypt password hashing
- React and React Bootstrap

## Repository structure

- `JWT/JWT/` — ASP.NET Core API and EF Core migrations.
- `jwtlogin/` — React client.

## Prerequisites

- .NET 8 SDK
- SQL Server instance
- Node.js 20 and npm

## Backend setup

1. Open a terminal in `JWT/JWT`.
2. Configure the database connection and JWT signing key using .NET user-secrets:

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DbConn" "YOUR_SQL_SERVER_CONNECTION_STRING"
dotnet user-secrets set "Jwt:Key" "REPLACE_WITH_A_RANDOM_SECRET_AT_LEAST_32_BYTES_LONG"
```

Generate a new signing key for your own environment. Never reuse the key that was previously committed to this repository.

3. Apply the Entity Framework migrations:

```bash
dotnet ef database update
```

The latest migration adds unique indexes for usernames and email addresses and limits the stored field lengths. If you already have user records, check for duplicate usernames/emails and values longer than the new limits before applying it; SQL Server cannot create the unique indexes while duplicates exist.

4. (Optional) Provision the first administrator. Set all three values as user-secrets before the first run:

```bash
dotnet user-secrets set "BootstrapAdmin:Username" "your-admin-username"
dotnet user-secrets set "BootstrapAdmin:Email" "admin@example.com"
dotnet user-secrets set "BootstrapAdmin:Password" "A-unique-password-at-least-12-characters"
```

The application creates this account only when the users table is empty. The password must be at least 12 characters and no more than 72 UTF-8 bytes (the BCrypt input limit). Remove the three bootstrap settings after the first successful startup. Subsequent accounts can be registered by an authenticated administrator.

5. Run the API:

```bash
dotnet run
```

Swagger is available in the Development environment.

## Frontend setup

Open a second terminal in `jwtlogin`:

```bash
npm ci
npm start
```

The current login component targets the local API at `https://localhost:7219`. If your API uses a different HTTPS port, update the frontend API base URL to match the backend launch profile.

## Authentication flow

- `POST /api/Auth/login` — validates credentials and returns a short-lived JWT.
- `POST /api/Auth/register` — creates a user account and requires an administrator JWT.
- The API validates token issuer, audience, lifetime and signing key.
- Passwords are stored as BCrypt hashes, never as plain text.
- Usernames and email addresses have database-level unique indexes.

## Security notes

- The signing key and database connection string are not stored in tracked application settings.
- Public self-registration is disabled; only an administrator can create accounts.
- The initial administrator is provisioned from local secrets and only when the database has no users.
- The React route guard improves navigation but is not an authorization boundary; protected API endpoints enforce authorization independently.
- This is an educational authentication example, not a production identity platform. Add rate limiting, account lockout, email verification, password reset, audit logging, and stronger operational controls before production use.

## Current limitations

The frontend currently contains a login screen and a basic dashboard. The API and UI should be tested together against a locally configured SQL Server instance before deployment.
