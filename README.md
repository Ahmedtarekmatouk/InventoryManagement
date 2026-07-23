@'
# Inventory Management System

A full-stack inventory management application built with ASP.NET Core 8 and Angular, implementing CRUD operations for products and categories.

## Technology Stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core 8 Web API |
| Database | SQL Server LocalDB with Entity Framework Core 8 |
| Frontend | Angular 22 (standalone components, signals) |
| Styling | SCSS |
| Testing | xUnit, Moq, FluentAssertions |

## Getting Started

### Prerequisites

- .NET 8 SDK
- Node.js 20.19 or later
- SQL Server LocalDB (included with Visual Studio)

### Clone the repository

```bash
git clone https://github.com/Ahmedtarekmatouk/InventoryManagement.git
cd InventoryManagement
```

### Running the backend

```bash
dotnet run --project src/InventoryManagement.API
```

The database is created and seeded automatically on first run. Swagger is available at `http://localhost:5042/swagger`.

If the API starts on a different port, update `apiBaseUrl` in `client/src/environments/environment.development.ts` to match.

### Running the frontend

```bash
cd client
npm install
npm start
```

The application is available at `http://localhost:4200`.

### Running the tests

```bash
dotnet test
```

## Solution Structure

src/
├── InventoryManagement.Domain Entities, no external dependencies
├── InventoryManagement.Application Business logic, DTOs, interfaces, validation
├── InventoryManagement.Infrastructure EF Core, repositories, persistence
└── InventoryManagement.API Controllers, middleware, composition root
tests/
└── InventoryManagement.Tests Unit tests for application services
client/ Angular application


Dependencies point inward: the API depends on Application, Infrastructure implements interfaces defined in Application, and Domain depends on nothing.

## API Endpoints

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

**Clean Architecture over a single project.** Business rules live in the Application layer with no knowledge of EF Core or ASP.NET. Replacing SQL Server would require a new repository implementation and one dependency injection change, with no impact on business logic.

**Repository interfaces in Application, implementations in Infrastructure.** The Application layer owns the contract it needs; Infrastructure fulfills it. This keeps dependencies pointing inward and allows services to be unit tested without a database.

**Explicit repository methods over a generic repository.** Methods such as `ExistsWithNameAsync` state their intent. A generic repository taking expression trees would leak query construction into the service layer.

**Manual mapping over AutoMapper.** Mapping is handled by extension methods. Renaming a property produces a compile-time error rather than a silent runtime mismatch, and it avoids a dependency for roughly forty lines of code.

**Domain exceptions over status codes in services.** Services throw `NotFoundException` and `ConflictException`. A single middleware maps them to HTTP responses, so services remain usable outside a web context.

**Soft delete for products, hard delete for categories.** Products carry transactional history and are marked as deleted, filtered out globally by an EF Core query filter. Categories are reference data and cannot be deleted while products reference them.

**Signals for frontend state.** Angular 22 applications are zoneless by default, so component state that drives the view is held in signals.

**Global HTTP error interceptor.** Error notifications are handled once in an interceptor rather than in each component. Errors are re-thrown so components can still reset their own loading state.

**Custom SCSS over a component library.** Design tokens, mixins, and BEM-named component classes are defined in `client/src/styles`. Shared patterns such as tables and form cards are global; component stylesheets contain only what is unique to that component.

## Third-party Libraries

| Library | Reason |
|---|---|
| FluentValidation | Validation rules as separate testable classes, keeping DTOs free of attributes and complex rules readable |
| Moq | Substitutes repository interfaces so service tests run without a database |
| FluentAssertions | Assertion failures state the expected value, which shortens diagnosis |

No UI component library, object mapper, or logging framework was added; the built-in equivalents were sufficient for this scope.

## Testing

Unit tests cover the business rules of `ProductService` and `CategoryService`: not-found handling, duplicate name detection, category existence checks, soft delete behavior, and paging metadata. Repositories are mocked, so no database is required.
