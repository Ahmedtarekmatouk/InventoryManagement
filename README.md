# Inventory Management System

A full-stack inventory management application built with ASP.NET Core 8 and Angular, implementing CRUD operations for products and categories with single sign-on through Microsoft Entra ID.

## Technology Stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core 8 Web API |
| Database | SQL Server LocalDB with Entity Framework Core 8 |
| Frontend | Angular 22 (standalone components, signals) |
| Styling | SCSS |
| Authentication | Microsoft Entra ID (OpenID Connect) |
| Testing | xUnit, Moq, FluentAssertions |

## Getting Started

### Prerequisites

- .NET 8 SDK
- Node.js 20.19 or later
- SQL Server LocalDB (included with Visual Studio)

### Backend

```bash
git clone https://github.com/Ahmedtarekmatouk/InventoryManagement.git
cd InventoryManagement
dotnet run --project src/InventoryManagement.API
```

The database is created and seeded automatically on first run. Swagger is available at `http://localhost:5042/swagger`. If the API starts on a different port, update `apiBaseUrl` in `client/src/environments/environment.development.ts`.

### Frontend

```bash
cd client
npm install
npm start
```

The application runs at `http://localhost:4200`. Test account credentials are provided in the submission email.

### Tests

```bash
dotnet test
```

## Solution Structure

```
src/
├── InventoryManagement.Domain          Entities, no external dependencies
├── InventoryManagement.Application     Business logic, DTOs, interfaces, validation
├── InventoryManagement.Infrastructure  EF Core, repositories, persistence
└── InventoryManagement.API             Controllers, middleware, composition root
tests/
└── InventoryManagement.Tests           Unit tests for application services
client/                                 Angular application
```

Dependencies point inward: the API depends on Application, Infrastructure implements interfaces defined in Application, and Domain depends on nothing.

## API Endpoints

All endpoints require a valid bearer token.

| Method | Route | Description |
|---|---|---|
| GET | `/api/products` | Paged list with search, category filter and sorting |
| GET | `/api/products/{id}` | Single product |
| POST | `/api/products` | Create product |
| PUT | `/api/products/{id}` | Update product |
| DELETE | `/api/products/{id}` | Soft delete product |
| GET | `/api/categories` | All categories |
| GET | `/api/categories/{id}` | Single category |
| POST | `/api/categories` | Create category |
| PUT | `/api/categories/{id}` | Update category |
| DELETE | `/api/categories/{id}` | Delete category |

## Architecture Decisions

**Clean architecture.** Business rules live in the Application layer with no knowledge of EF Core or ASP.NET. Repository interfaces are defined in Application and implemented in Infrastructure, so services can be unit tested without a database and the data store can be replaced without touching business logic.

**Services throw, middleware translates.** Services raise `NotFoundException` and `ConflictException` rather than returning status codes. A single middleware maps them to HTTP responses and returns a generic message for unexpected failures while logging the detail. Controllers stay at two or three lines each.

**Rules live where they cannot be bypassed.** Soft-deleted products are filtered by an EF Core query filter rather than a condition repeated in each query; page size is clamped inside the query parameter object rather than in the controller; audit timestamps are set in `SaveChangesAsync`.

**Cancellation tokens throughout the async chain.** A cancelled request propagates from the controller to the database, releasing the thread and connection instead of computing a result nobody will receive.

**Signals for frontend state.** Component state is managed with Angular Signals. HTTP errors are handled centrally through an interceptor and re-thrown so components only reset their own loading state.

**Custom SCSS over a component library.** Design tokens, mixins and BEM-named classes are defined in `client/src/styles`. Shared patterns such as tables and form cards are global; component stylesheets contain only what is unique to that component.

## Single Sign-On

SSO is implemented with Microsoft Entra ID using OpenID Connect with the authorization code flow and PKCE. ADFS was recommended but requires a Windows Server domain that was not available; Entra ID implements the same protocol, so moving to ADFS would change the authority URL and token validation parameters, not the application code.

Two registrations are used: the API as the protected resource exposing an `access_as_user` scope, and the Angular client as a public client requesting it. No client secret exists, since a browser cannot hold one securely.

The API validates tokens with the standard JWT bearer handler. The client acquires tokens silently and attaches them in an HTTP interceptor, so no component handles a token directly. Route guards protect the application shell, but the API enforces authorization independently — the guards are a user-experience concern, not the security boundary.

## Third-party Libraries

| Library | Reason |
|---|---|
| FluentValidation | Validation rules as separate testable classes, keeping DTOs free of attributes |
| Microsoft.Identity.Web | Configures the JWT bearer handler with the Entra authority, audience and signing key rotation |
| Moq | Substitutes repository interfaces so service tests run without a database |
| FluentAssertions | Assertion failures state the expected value, which shortens diagnosis |
| @azure/msal-browser | OIDC authorization code flow with PKCE in the browser. The Angular wrapper does not yet support Angular 22, so the service, guards and interceptor are written against the core library |

No UI component library, object mapper or logging framework was added; the built-in equivalents were sufficient for this scope.

## Testing

Unit tests cover the business rules of `ProductService` and `CategoryService`: not-found handling, duplicate name detection, category existence checks, soft delete behavior and paging metadata. Repositories are mocked, so the suite runs without a database.
