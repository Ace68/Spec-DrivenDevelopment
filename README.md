# Santa Claus Work Management API

A **Domain-Driven Design (DDD)** REST API built with **.NET 10** and **Minimal API**, implementing **CQRS+ES** pattern using **Muflone 8.5.0**.

## 🎯 Project Overview

This project manages Santa Claus's workshop operations across three bounded contexts:

- **Marketing**: Letter management and child registration
- **Production**: Wish processing and toy manufacturing
- **Delivery**: Route planning and gift distribution

## 🏗️ Architecture

### Modular Monolith with Clean Architecture

Each module follows a strict layered architecture ensuring **domain isolation** and **bounded context separation**:

```text
src/
├── SantaClaus.Rest/              # API entry point (Minimal API)
├── SantaClaus.Infrastructure/     # Cross-cutting infrastructure (Muflone config)
├── SantaClaus.Shared/            # Shared types across all modules
│   ├── Commands/                 # Base Command record (ICommand)
│   ├── Events/                   # Base DomainEvent record (IDomainEvent)
│   ├── ValueObjects/             # DomainId wrapper
│   ├── Exceptions/               # Domain/Validation exceptions
│   └── Results/                  # Result pattern types
└── Modules/
    ├── Marketing/
    │   ├── SantaClaus.Marketing.Facade/          # Public API & endpoints
    │   ├── SantaClaus.Marketing.Domain/          # Aggregates & business logic
    │   ├── SantaClaus.Marketing.SharedKernel/    # Commands, Events, Value Objects
    │   ├── SantaClaus.Marketing.ReadModel/       # Query DTOs
    │   └── SantaClaus.Marketing.Infrastructure/  # Repositories & handlers
    ├── Production/
    │   └── (same structure)
    └── Delivery/
        └── (same structure)
```

### Key Design Principles

1. **Domain Isolation**: Each module's domain is completely isolated with zero cross-module dependencies
2. **CQRS+ES**: Commands and Events separated, using Muflone for event sourcing
3. **Facade Pattern**: Modules expose public interfaces through Facade layer
4. **Shared Kernel**: Commands/Events/Value Objects in SharedKernel, never in Domain
5. **Minimal API**: Endpoint registration via module helpers, clean and composable

## 🛠️ Technology Stack

- **.NET 10.0**: Latest LTS framework
- **C# 13**: Language features with nullable reference types enabled
- **Muflone 8.5.0**: CQRS+ES library for command/event handling
- **Minimal API**: Lightweight HTTP API with OpenAPI support
- **Dependency Injection**: Built-in ASP.NET Core DI

## 📦 Project Dependencies

### Cross-Module Dependencies (Allowed)

- `Shared` → Referenced by all modules
- `Infrastructure` → Referenced by module Infrastructure projects
- `Module.SharedKernel` → Referenced by all layers within the module

### Forbidden Dependencies

- ❌ Module Domain → Another Module (any layer)
- ❌ Module → Module (no cross-module references at any level)

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Any IDE: Visual Studio 2026, VS Code, or Rider

### Running the Application

```bash
# Clone the repository
git clone <repository-url>
cd Spec-DrivenDevelopment

# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run the API
dotnet run --project src/SantaClaus.Rest

# Navigate to Swagger UI
# Open browser at: http://localhost:5000/swagger
```

## 📋 Current Implementation Status

### ✅ Phase 1: Project Scaffolding (Complete)

- All module projects created
- Solution structure established
- NuGet packages installed

### ✅ Phase 2: Muflone Integration (Complete)

- Base `Command` record implementing `ICommand`
- Base `DomainEvent` record implementing `IDomainEvent`
- `DomainId` value object for `IDomainId` conversions
- Domain and Validation exceptions
- Result pattern implementation

### 🔄 Phase 3: Marketing Domain (In Progress)

- ✅ Letter aggregate with event sourcing
- ✅ `CreateLetter` and `ProcessLetter` commands
- ✅ `LetterCreated` and `LetterProcessed` events
- ✅ `LetterStatus` enum
- ⏳ Command/Event handlers
- ⏳ API endpoints

### ⏳ Phase 4-6: Production & Delivery Domains (Planned)

## 🧪 Testing Strategy

- Unit tests for domain logic (aggregates, value objects)
- Integration tests for handlers and repositories
- Architecture tests to enforce dependency rules (NetArchTest)

## 📖 Documentation

- [Implementation Plan](.speckit/plans/santa-api-implementation.plan.md)
- [Constitution](.speckit/constitution/minimal-api-scaffolding.constitution.md)
- [Task Breakdown](.speckit/tasks/santa-api-tasks.md)

## 🤝 Contributing

This is an experimental project for **spec-driven development with SpecKit**. All architecture, design, and implementation are guided through prompts and conversations.

## 📄 License

MIT License - See LICENSE file for details

## 🎄 About

This project demonstrates how to build a production-ready DDD application using modern .NET practices, CQRS+ES patterns, and clean architecture principles—all guided by specification-driven development.
