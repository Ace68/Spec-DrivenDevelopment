# Constitution: Minimal API Scaffolding with Domain-Driven Design

## Core Principles

### 1. Modular Architecture with Bounded Contexts

- Each module represents an isolated Bounded Context
- Modules are physically separated in the filesystem
- Zero cross-module dependencies at domain level
- Modules communicate only through well-defined Facade interfaces

### 2. Layered Architecture per Module

Each module MUST follow this exact structure:

- **Facade**: Public interface and endpoint definitions
- **Domain**: Business logic, entities, value objects, domain events
- **SharedKernel**: Module-specific shared types and enumerations
- **ReadModel**: DTOs and query handlers for read operations
- **Infrastructure**: Repository implementations and external integrations

### 3. Solution Organization

Solution folders (logical) MUST be organized by architectural layers:

- **90 Presentation**: API entry point (Rest project)
- **80 Infrastructure**: Cross-cutting infrastructure concerns
- **50 Modules**: All bounded context modules
- **30 Shared**: Types shared across ALL modules

### 4. Project Dependencies Rules

Strict dependency matrix MUST be enforced:

- **Rest** → only Facade projects
- **Facade** → Domain, ReadModel, SharedKernel of same module + Shared
- **Domain** → SharedKernel of same module + Shared
- **ReadModel** → SharedKernel of same module + Shared
- **Infrastructure** → Domain, SharedKernel of same module + Infrastructure + Shared
- **NO** cross-module references at Domain/ReadModel/Infrastructure level

### 5. Minimal API Pattern

- Use WebApplicationBuilder and WebApplication
- No controllers, only endpoint mappings
- Group endpoints by module using `MapGroup`
- Each module exposes endpoints through static Endpoints class

### 6. Module Registration Pattern

- Each module implements `IModule` interface
- Registration through `ModuleExtensions.AddModules()`
- Endpoint mapping through `ModuleExtensions.MapModules()`
- Module-specific configuration in `{Module}FacadeHelper`

### 7. Code Quality Standards

- `TreatWarningsAsErrors=true` in all .csproj files
- `Nullable=enable` for null safety
- `ImplicitUsings=enable` for cleaner code
- Target .NET 10.0 or higher
- All code and comments in English
- No `Class1.cs` default files

### 8. Configuration and Observability

- Structured logging with Serilog
- OpenTelemetry support (disabled by default)
- Swagger/OpenAPI documentation for all endpoints
- Configuration through appsettings.json
- Environment-specific settings in launchSettings.json

### 9. Incremental Scaffolding

Initial scaffolding MUST include:

- Complete project structure
- Working Program.cs
- Module infrastructure (IModule, extensions)
- OpenAPI and telemetry modules
- At least one placeholder endpoint per module
- Complete README.md

Initial scaffolding MUST NOT include:

- Concrete domain entities
- Repository implementations
- Command/Event handlers
- Database configuration
- Authentication/Authorization

### 10. Endpoint Conventions

- Path pattern: `/v1/{module-lowercase}`
- OpenAPI tags: module name in PascalCase
- HTTP methods follow REST conventions
- Status codes explicitly defined with `.Produces<T>()`
- Endpoint names follow pattern: `{Verb}{Module}{Entity}`

### 11. Naming Conventions

- Namespace: `{ProjectName}.{Module}.{Layer}`
- Project: `{ProjectName}.{Module}.{Layer}`
- Files: PascalCase matching class names
- Endpoint groups: lowercase module names
- Configuration sections: PascalCase

### 12. Dependency Injection

- Register services in `{Module}FacadeHelper.AddServices()`
- Use appropriate lifetime: Scoped for stateful, Singleton for stateless
- Resolve dependencies through constructor injection
- No service locator pattern

### 13. API Versioning

- Version prefix in URL: `/v1/`
- Major version changes require new endpoint groups
- Maintain backward compatibility within same major version
- Document breaking changes in README

### 14. Error Handling (Future)

- Domain exceptions in Shared project
- Result pattern for operation outcomes
- Consistent error response format
- HTTP status codes aligned with RFC 7231

### 15. Testing Strategy (Future Consideration)

- Unit tests for Domain logic
- Integration tests for Facade/Endpoints
- Separate test projects per module
- Follow same namespace conventions

## Definition of Done for Scaffolding

The scaffolding is complete when ALL of these conditions are met:

1. ✅ Solution compiles in Debug and Release without warnings
2. ✅ `dotnet run` starts application successfully
3. ✅ Swagger UI accessible at <http://localhost:5000/swagger>
4. ✅ Each module has at least one endpoint visible at `/v1/{module}/`
5. ✅ Placeholder endpoints respond with HTTP 200
6. ✅ README.md contains complete local run instructions
7. ✅ No `Class1.cs` files present
8. ✅ All inter-project dependencies match the dependency matrix
9. ✅ OpenTelemetry configured but disabled by default
10. ✅ All .csproj files have `TreatWarningsAsErrors=true`

## Anti-Patterns to Avoid

### ❌ Cross-Module Domain Dependencies

Never reference another module's Domain, ReadModel, or Infrastructure projects.

### ❌ Premature Optimization

Don't add caching, performance optimizations, or complex patterns in scaffolding phase.

### ❌ Leaky Abstractions

Domain entities should not leak into Facade/Endpoints. Use DTOs in ReadModel.

### ❌ Anemic Domain Model

Domain should contain business logic, not just data structures.

### ❌ God Modules

Keep modules focused on a single Bounded Context. Split if module grows too large.

### ❌ Implicit Dependencies

All dependencies must be explicit through constructor injection, never through static access.

### ❌ Configuration Magic Strings

Use strongly-typed configuration classes, not scattered string keys.

### ❌ Mixing Concerns

Keep endpoint definition (Facade) separate from business logic (Domain) and data access (Infrastructure).

## Required User Inputs

Before starting scaffolding, MUST collect:

1. **Project Name**: Used as namespace root and project prefix
2. **Module List**: At least 2 modules, each representing a Bounded Context
3. **HTTP Port**: Optional, default 5000 for Development

## File Structure Example

```text
{ProjectName}.sln
├── src/
│   ├── {ProjectName}.Rest/
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   ├── Properties/launchSettings.json
│   │   ├── Infrastructure/
│   │   │   ├── IModule.cs
│   │   │   ├── ModuleExtensions.cs
│   │   │   ├── OpenApiModule.cs
│   │   │   └── OpenTelemetryModule.cs
│   │   └── Modules/
│   │       ├── {Module1}Module.cs
│   │       └── {Module2}Module.cs
│   ├── {ProjectName}.Infrastructure/
│   │   ├── InfrastructureHelper.cs
│   │   └── EventStoreSettings.cs
│   ├── {ProjectName}.Shared/
│   │   └── (common types)
│   └── Modules/
│       ├── {Module1}/
│       │   ├── {ProjectName}.{Module1}.Facade/
│       │   ├── {ProjectName}.{Module1}.Domain/
│       │   ├── {ProjectName}.{Module1}.SharedKernel/
│       │   ├── {ProjectName}.{Module1}.ReadModel/
│       │   └── {ProjectName}.{Module1}.Infrastructure/
│       └── {Module2}/
│           └── (same structure)
└── README.md
```

## Package References by Project Type

### Rest Project

- Serilog.AspNetCore (8.*)
- Swashbuckle.AspNetCore (6.*)
- OpenTelemetry.Exporter.Console (1.*)
- OpenTelemetry.Extensions.Hosting (1.*)
- OpenTelemetry.Instrumentation.AspNetCore (1.*)
- OpenTelemetry.Instrumentation.Http (1.*)

### Facade Project

- Microsoft.AspNetCore.OpenApi (9.*)

### Infrastructure Project

- Microsoft.Extensions.DependencyInjection.Abstractions (9.*)

### Other Projects

- No external packages in initial scaffolding

## Evolution Path

After scaffolding is complete, follow this order:

1. Define domain entities and value objects per module
2. Implement repository interfaces in Domain
3. Implement repositories in Infrastructure
4. Add domain services if needed
5. Create DTOs in ReadModel
6. Implement query handlers
7. Define endpoints in Facade
8. Add command handling
9. Implement event handling
10. Add persistence layer
11. Configure authentication/authorization

Each step should be completed per module before moving to the next step, maintaining modularity throughout evolution.
