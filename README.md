# Wonga Platform

`Wonga Platform` is a small authentication and identity management system designed to demonstrate a clean, service-oriented backend architecture.

The platform allows users to register, authenticate, and securely retrieve their profile information through a single gateway API. It models how modern systems separate authentication, user data, and public API access into independent services that can evolve and scale independently.

This repository contains the backend implementation built as a .NET 10 monorepo, composed of a gateway service, domain services, and a shared security library, with PostgreSQL used for persistence.

The goal of the platform is to illustrate:

`Clear service boundaries`

`Gateway-based API access`

`Secure authentication workflows`

`A maintainable monorepo structure for multi-service systems`

Together, these components form a simple but realistic foundation for an identity-driven platform that could be extended with additional services such as authorization, auditing, or account management.

## Services

- `Wonga.Gateway.Api`
- `Wonga.Services.Identity.Api`
- `Wonga.Services.UserProfile.Api`

Shared support:

- `Wonga.Shared.Security`

## Frontend

The React frontend lives in the separate `../Wonga-Front-End` workspace and talks to the backend through the gateway.

## Repository Structure

```text
src/
  Wonga.Gateway.Api/
  Wonga.Services.Identity.Api/
  Wonga.Services.UserProfile.Api/
  Wonga.Shared.Security/
tests/
```

## Basic Commands

Build the solution:

```powershell
./build.ps1
```

Run the gateway:

```powershell
 dotnet run --project .\src\Wonga.Gateway.Api\Wonga.Gateway.Api.csproj
```

Run the identity service:

```powershell
 dotnet run --project .\src\Wonga.Services.Identity.Api\Wonga.Services.Identity.Api.csproj
```

Run the user profile service:

```powershell
 dotnet run --project .\src\Wonga.Services.UserProfile.Api\Wonga.Services.UserProfile.Api.csproj
```

Run the full backend test suite:

```powershell
dotnet test .\Wonga.Platform.slnx -c Debug
```

Run the backend stack in Docker:

```powershell
docker compose up --build
```

Backend endpoints:

- `POST /identity/register`
- `POST /identity/login`
- `GET /users/me`
- `GET /health`
