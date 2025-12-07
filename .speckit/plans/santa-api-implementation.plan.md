# Santa Claus Work Management API - Implementation Plan

## Project Overview

**Project Name:** SantaClaus  
**Architecture:** Minimal API with Domain-Driven Design  
**CQRS/ES Library:** Muflone 8.5.0  
**Target Framework:** .NET 9.0  
**Modules:** Marketing, Production, Delivery

## Executive Summary

This plan outlines the complete implementation of the Santa Claus Work Management API using a modular DDD architecture with CQRS+ES pattern via Muflone. The implementation is divided into 6 phases, ensuring proper isolation between modules and validating architecture through comprehensive testing.

---

## Phase 0: Prerequisites & Environment Setup

**Duration:** 1 day  
**Goal:** Ensure development environment is ready

### Tasks

#### 0.1 Verify Development Environment

- [ ] .NET 9.0 SDK installed
- [ ] Visual Studio 2022 or JetBrains Rider with latest updates
- [ ] Git configured
- [ ] Docker Desktop installed (for RabbitMQ during development)

#### 0.2 Review Documentation

- [ ] Read `minimal-api-scaffolding.prompt.md`
- [ ] Review `santa-api.specification.md`
- [ ] Study `muflone-core.prompt.md`
- [ ] Understand `architecture-tests.prompt.md`
- [ ] Review `minimal-api-scaffolding.constitution.md`

#### 0.3 Decide on Transport Layer

**Options:**

1. **Muflone.Transport.InMemory** (Recommended for Phase 1)
   - No external dependencies
   - Fast development iteration
   - Easy debugging
   - Perfect for initial implementation

2. **Muflone.Transport.RabbitMQ** (Production-ready)
   - Requires RabbitMQ instance
   - Use Docker: `docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management`

3. **Muflone.Transport.Azure** (Cloud-native)
   - Requires Azure Service Bus
   - Higher infrastructure cost

**Decision:** Start with **InMemory** for Phase 1-4, migrate to **RabbitMQ** in Phase 5

---

## Phase 1: Project Scaffolding

**Duration:** 2-3 days  
**Goal:** Create complete project structure with all modules

### 1.1 Create Solution Structure

Follow `minimal-api-scaffolding.prompt.md` exactly:

```bash
dotnet new sln -n SantaClaus

# Create Rest API project
dotnet new web -n SantaClaus.Rest -o src/SantaClaus.Rest

# Create Infrastructure project
dotnet new classlib -n SantaClaus.Infrastructure -o src/SantaClaus.Infrastructure

# Create Shared project
dotnet new classlib -n SantaClaus.Shared -o src/SantaClaus.Shared

# For each module (Marketing, Production, Delivery):
dotnet new classlib -n SantaClaus.{Module}.Facade -o src/Modules/{Module}/SantaClaus.{Module}.Facade
dotnet new classlib -n SantaClaus.{Module}.Domain -o src/Modules/{Module}/SantaClaus.{Module}.Domain
dotnet new classlib -n SantaClaus.{Module}.SharedKernel -o src/Modules/{Module}/SantaClaus.{Module}.SharedKernel
dotnet new classlib -n SantaClaus.{Module}.ReadModel -o src/Modules/{Module}/SantaClaus.{Module}.ReadModel
dotnet new classlib -n SantaClaus.{Module}.Infrastructure -o src/Modules/{Module}/SantaClaus.{Module}.Infrastructure
```

### 1.2 Configure Project References

**Dependency Matrix:**

| Project | References |
|---------|-----------|
| Rest | All Facade projects |
| {Module}.Facade | {Module}.Domain, {Module}.ReadModel, {Module}.SharedKernel, Shared |
| {Module}.Domain | {Module}.SharedKernel, Shared |
| {Module}.ReadModel | {Module}.SharedKernel, Shared |
| {Module}.Infrastructure | {Module}.Domain, {Module}.SharedKernel, Infrastructure, Shared |

### 1.3 Install NuGet Packages

#### SantaClaus.Rest

```xml
<PackageReference Include="Serilog.AspNetCore" Version="8.*" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.*" />
<PackageReference Include="OpenTelemetry.Exporter.Console" Version="1.*" />
<PackageReference Include="OpenTelemetry.Extensions.Hosting" Version="1.*" />
<PackageReference Include="OpenTelemetry.Instrumentation.AspNetCore" Version="1.*" />
<PackageReference Include="OpenTelemetry.Instrumentation.Http" Version="1.*" />
```

#### Each {Module}.Facade

```xml
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="9.*" />
<PackageReference Include="Muflone" Version="8.5.0" />
```

#### SantaClaus.Infrastructure

```xml
<PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="9.*" />
<PackageReference Include="Muflone" Version="8.5.0" />
<PackageReference Include="Muflone.Transport.InMemory" Version="8.5.0" />
```

#### Each {Module}.Domain

```xml
<PackageReference Include="Muflone" Version="8.5.0" />
```

#### Each {Module}.Infrastructure

```xml
<PackageReference Include="Muflone" Version="8.5.0" />
```

### 1.4 Implement Core Infrastructure

#### Program.cs

```csharp
using SantaClaus.Rest.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add modules
builder.Services.AddModules(builder.Configuration);

// Add OpenAPI module
builder.Services.AddOpenApiModule();

// Add OpenTelemetry module
builder.Services.AddOpenTelemetryModule(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapModules();
app.MapOpenApiModule();

app.Run();
```

#### IModule.cs, ModuleExtensions.cs, OpenApiModule.cs, OpenTelemetryModule.cs

Implement as per `minimal-api-scaffolding.prompt.md`

### 1.5 Create Module Files

For each module (Marketing, Production, Delivery):

- `{Module}Module.cs` in Rest/Modules/
- `I{Module}Facade.cs` in {Module}.Facade/
- `{Module}FacadeHelper.cs` in {Module}.Facade/
- `{Module}Endpoints.cs` in {Module}.Facade/
- `{Module}InfrastructureHelper.cs` in {Module}.Infrastructure/

### 1.6 Validation

- [ ] Solution compiles without warnings (`TreatWarningsAsErrors=true`)
- [ ] `dotnet run` starts successfully
- [ ] Swagger UI accessible at <http://localhost:5000/swagger>
- [ ] Each module shows at least one placeholder endpoint
- [ ] No `Class1.cs` files remain

**Deliverable:** Fully scaffolded solution with 3 modules, compiling and running

---

## Phase 2: Muflone Integration & Shared Infrastructure

**Duration:** 3-4 days  
**Goal:** Integrate Muflone and create shared CQRS/ES infrastructure

### 2.1 Create Shared Domain Base Classes

In `SantaClaus.Shared`:

#### Shared/Commands/Command.cs

```csharp
using Muflone.Messages.Commands;

namespace SantaClaus.Shared.Commands;

public abstract record Command : ICommand
{
    public Guid AggregateId { get; init; }
    public Guid MessageId { get; } = Guid.NewGuid();
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
}
```

#### Shared/Events/DomainEvent.cs

```csharp
using Muflone.Messages.Events;

namespace SantaClaus.Shared.Events;

public abstract record DomainEvent : IDomainEvent
{
    public Guid AggregateId { get; init; }
    public Guid MessageId { get; } = Guid.NewGuid();
    public Guid CorrelationId { get; init; }
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
    public int AggregateVersion { get; init; }
}
```

#### Shared/Events/IntegrationEvent.cs

```csharp
using Muflone.Messages.Events;

namespace SantaClaus.Shared.Events;

public abstract record IntegrationEvent : IIntegrationEvent
{
    public Guid MessageId { get; } = Guid.NewGuid();
    public Guid CorrelationId { get; init; }
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}
```

#### Shared/Aggregates/AggregateRoot.cs

```csharp
using Muflone;
using SantaClaus.Shared.Events;

namespace SantaClaus.Shared.Aggregates;

public abstract class AggregateRoot : MufloneAggregateRoot
{
    protected void RaiseEvent(DomainEvent @event)
    {
        ApplyEvent(@event);
        _uncommittedEvents.Add(@event);
    }

    protected abstract void ApplyEvent(DomainEvent @event);
}
```

### 2.2 Configure Muflone in Infrastructure

#### Infrastructure/MufloneConfiguration.cs

```csharp
using Microsoft.Extensions.DependencyInjection;
using Muflone.Persistence;
using Muflone.Transport.InMemory;

namespace SantaClaus.Infrastructure;

public static class MufloneConfiguration
{
    public static IServiceCollection AddMufloneInfrastructure(
        this IServiceCollection services)
    {
        // Register in-memory event store (Phase 1-4)
        services.AddSingleton<IEventStore, InMemoryEventStore>();
        
        // Register in-memory transport
        services.AddMufloneInMemoryTransport();
        
        // Register repository
        services.AddScoped(typeof(IRepository), typeof(Repository));
        
        return services;
    }
}
```

### 2.3 Update Rest Program.cs

Add Muflone infrastructure:

```csharp
// Add Muflone infrastructure
builder.Services.AddMufloneInfrastructure();
```

### 2.4 Create Result Pattern in Shared

#### Shared/Results/Result.cs

```csharp
namespace SantaClaus.Shared.Results;

public record Result
{
    public bool IsSuccess { get; init; }
    public string? Error { get; init; }
    
    public static Result Success() => new() { IsSuccess = true };
    public static Result Failure(string error) => new() { IsSuccess = false, Error = error };
}

public record Result<T> : Result
{
    public T? Value { get; init; }
    
    public static Result<T> Success(T value) => 
        new() { IsSuccess = true, Value = value };
    
    public new static Result<T> Failure(string error) => 
        new() { IsSuccess = false, Error = error };
}
```

### 2.5 Create Domain Exceptions in Shared

#### Shared/Exceptions/DomainException.cs

```csharp
namespace SantaClaus.Shared.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
    
    public DomainException(string message, Exception innerException) 
        : base(message, innerException) { }
}
```

### 2.6 Validation

- [ ] Muflone packages installed in all required projects
- [ ] Shared base classes compile
- [ ] Infrastructure configuration works
- [ ] Application still starts successfully

**Deliverable:** Muflone integrated with shared CQRS/ES infrastructure

---

## Phase 3: Marketing Module Implementation

**Duration:** 5-7 days  
**Goal:** Complete implementation of Marketing module with CQRS+ES

### 3.1 Define Domain Model

#### Domain/Entities/Letter.cs

```csharp
using SantaClaus.Shared.Aggregates;
using SantaClaus.Marketing.Domain.Events;

namespace SantaClaus.Marketing.Domain.Entities;

public class Letter : AggregateRoot
{
    public Guid ChildId { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public DateTime ReceivedDate { get; private set; }
    public string Language { get; private set; } = string.Empty;
    public LetterStatus Status { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

    // For reconstitution from events
    private Letter() { }

    public static Letter Create(Guid letterId, Guid childId, string content, 
        DateTime receivedDate, string language)
    {
        var letter = new Letter();
        letter.RaiseEvent(new LetterCreated
        {
            AggregateId = letterId,
            ChildId = childId,
            Content = content,
            ReceivedDate = receivedDate,
            Language = language
        });
        return letter;
    }

    public void MarkAsProcessed()
    {
        if (Status == LetterStatus.Processed)
            throw new InvalidOperationException("Letter already processed");

        RaiseEvent(new LetterProcessed
        {
            AggregateId = Id,
            ProcessedAt = DateTime.UtcNow
        });
    }

    protected override void ApplyEvent(DomainEvent @event)
    {
        switch (@event)
        {
            case LetterCreated e:
                Apply(e);
                break;
            case LetterProcessed e:
                Apply(e);
                break;
        }
    }

    private void Apply(LetterCreated e)
    {
        Id = e.AggregateId;
        ChildId = e.ChildId;
        Content = e.Content;
        ReceivedDate = e.ReceivedDate;
        Language = e.Language;
        Status = LetterStatus.Received;
    }

    private void Apply(LetterProcessed e)
    {
        Status = LetterStatus.Processed;
        ProcessedAt = e.ProcessedAt;
    }
}

public enum LetterStatus
{
    Received,
    Processing,
    Processed
}
```

#### Domain/Entities/Child.cs

Similar structure for Child aggregate

#### Domain/Entities/Wish.cs

Similar structure for Wish aggregate

### 3.2 Define Domain Events

#### Domain/Events/LetterCreated.cs

```csharp
using SantaClaus.Shared.Events;

namespace SantaClaus.Marketing.Domain.Events;

public record LetterCreated : DomainEvent
{
    public Guid ChildId { get; init; }
    public string Content { get; init; } = string.Empty;
    public DateTime ReceivedDate { get; init; }
    public string Language { get; init; } = string.Empty;
}
```

#### Domain/Events/LetterProcessed.cs

```csharp
using SantaClaus.Shared.Events;

namespace SantaClaus.Marketing.Domain.Events;

public record LetterProcessed : DomainEvent
{
    public DateTime ProcessedAt { get; init; }
}
```

Create similar events for:
- `ChildRegistered`
- `ChildBehaviorUpdated`
- `WishCreated`
- `WishApproved`
- `NotificationSent`

### 3.3 Define Commands

#### Domain/Commands/CreateLetter.cs

```csharp
using SantaClaus.Shared.Commands;

namespace SantaClaus.Marketing.Domain.Commands;

public record CreateLetter : Command
{
    public Guid ChildId { get; init; }
    public string Content { get; init; } = string.Empty;
    public DateTime ReceivedDate { get; init; }
    public string Language { get; init; } = string.Empty;
}
```

Create commands for all write operations:
- `CreateLetter`
- `ProcessLetter`
- `RegisterChild`
- `UpdateChildBehavior`
- `CreateWish`
- `ApproveWish`
- `SendNotification`

### 3.4 Implement Command Handlers

#### Domain/CommandHandlers/CreateLetterHandler.cs

```csharp
using Muflone.Messages.Commands;
using Muflone.Persistence;
using SantaClaus.Marketing.Domain.Commands;
using SantaClaus.Marketing.Domain.Entities;

namespace SantaClaus.Marketing.Domain.CommandHandlers;

public class CreateLetterHandler : ICommandHandler<CreateLetter>
{
    private readonly IRepository _repository;

    public CreateLetterHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(CreateLetter command, CancellationToken cancellationToken)
    {
        var letter = Letter.Create(
            command.AggregateId,
            command.ChildId,
            command.Content,
            command.ReceivedDate,
            command.Language
        );

        await _repository.SaveAsync(letter, cancellationToken);
    }
}
```

Implement handlers for all commands

### 3.5 Define Read Models (DTOs)

#### ReadModel/DTOs/LetterDto.cs

```csharp
namespace SantaClaus.Marketing.ReadModel.DTOs;

public record LetterDto
{
    public Guid LetterId { get; init; }
    public Guid ChildId { get; init; }
    public string Content { get; init; } = string.Empty;
    public DateTime ReceivedDate { get; init; }
    public string Language { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime? ProcessedAt { get; init; }
}
```

Create DTOs for all read models

### 3.6 Implement Query Handlers

#### ReadModel/Queries/GetLetterById.cs

```csharp
namespace SantaClaus.Marketing.ReadModel.Queries;

public record GetLetterById
{
    public Guid LetterId { get; init; }
}
```

#### ReadModel/QueryHandlers/GetLetterByIdHandler.cs

```csharp
using SantaClaus.Marketing.ReadModel.DTOs;
using SantaClaus.Marketing.ReadModel.Queries;

namespace SantaClaus.Marketing.ReadModel.QueryHandlers;

public class GetLetterByIdHandler
{
    // Use read model repository or projection
    public async Task<LetterDto?> HandleAsync(GetLetterById query, 
        CancellationToken cancellationToken)
    {
        // Query from read model store
        // For Phase 3, can use in-memory projections
        throw new NotImplementedException();
    }
}
```

### 3.7 Implement Event Handlers (Projections)

#### Domain/EventHandlers/LetterCreatedHandler.cs

```csharp
using Muflone.Messages.Events;
using SantaClaus.Marketing.Domain.Events;

namespace SantaClaus.Marketing.Domain.EventHandlers;

public class LetterCreatedHandler : IDomainEventHandler<LetterCreated>
{
    // Project to read model
    public async Task HandleAsync(LetterCreated @event, CancellationToken cancellationToken)
    {
        // Update read model projection
        // For Phase 3, maintain in-memory dictionary
    }
}
```

### 3.8 Register Handlers in Facade

#### Facade/MarketingFacadeHelper.cs

```csharp
using Microsoft.Extensions.DependencyInjection;
using SantaClaus.Marketing.Domain.CommandHandlers;
using SantaClaus.Marketing.Domain.EventHandlers;

namespace SantaClaus.Marketing.Facade;

public static class MarketingFacadeHelper
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        // Register command handlers (Muflone 8.5.0+ auto-generation)
        services.AddCommandHandler<CreateLetterHandler>();
        services.AddCommandHandler<ProcessLetterHandler>();
        services.AddCommandHandler<RegisterChildHandler>();
        // ... all other command handlers

        // Register domain event handlers
        services.AddDomainEventHandler<LetterCreatedHandler>();
        services.AddDomainEventHandler<LetterProcessedHandler>();
        // ... all other event handlers

        // Register facade
        services.AddScoped<IMarketingFacade, MarketingFacade>();

        return services;
    }

    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
    {
        MarketingEndpoints.MapEndpoints(app);
        return app;
    }
}
```

### 3.9 Implement API Endpoints

#### Facade/MarketingEndpoints.cs

```csharp
using Microsoft.AspNetCore.Mvc;
using Muflone.Messages.Commands;
using SantaClaus.Marketing.Domain.Commands;
using SantaClaus.Marketing.ReadModel.Queries;
using SantaClaus.Marketing.ReadModel.QueryHandlers;

namespace SantaClaus.Marketing.Facade;

public static class MarketingEndpoints
{
    public static IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/v1/marketing")
            .WithTags("Marketing")
            .WithOpenApi();

        // POST /v1/marketing/letters
        group.MapPost("/letters", async (
            [FromBody] CreateLetterRequest request,
            [FromServices] ICommandSender commandSender,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateLetter
            {
                AggregateId = Guid.NewGuid(),
                ChildId = request.ChildId,
                Content = request.Content,
                ReceivedDate = request.ReceivedDate,
                Language = request.Language
            };

            await commandSender.SendAsync(command, cancellationToken);

            return Results.Created($"/v1/marketing/letters/{command.AggregateId}", 
                new { LetterId = command.AggregateId });
        })
        .WithName("CreateLetter")
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        // GET /v1/marketing/letters/{letterId}
        group.MapGet("/letters/{letterId:guid}", async (
            Guid letterId,
            [FromServices] GetLetterByIdHandler handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetLetterById { LetterId = letterId };
            var result = await handler.HandleAsync(query, cancellationToken);

            return result is not null 
                ? Results.Ok(result) 
                : Results.NotFound();
        })
        .WithName("GetLetterById")
        .Produces<LetterDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // Implement all other endpoints from specification

        return app;
    }
}

public record CreateLetterRequest(
    Guid ChildId,
    string Content,
    DateTime ReceivedDate,
    string Language
);
```

### 3.10 Validation

- [ ] All Marketing domain entities implemented
- [ ] All commands and events defined
- [ ] All command handlers implemented
- [ ] All event handlers (projections) implemented
- [ ] All query handlers implemented
- [ ] All endpoints implemented as per specification
- [ ] Marketing module compiles without warnings
- [ ] Endpoints testable via Swagger
- [ ] Events are stored in event store
- [ ] Read models are projected correctly

**Deliverable:** Fully functional Marketing module with CQRS+ES

---

## Phase 4: Production & Delivery Modules Implementation

**Duration:** 10-14 days  
**Goal:** Implement remaining modules following Marketing pattern

### 4.1 Production Module

Follow the same pattern as Marketing:

#### Domain Entities

- `WorkOrder` aggregate
- `QualityCheck` aggregate

#### Commands

- `CreateWorkOrder`
- `StartWorkOrder`
- `CompleteWorkOrder`
- `CreateQualityCheck`
- `CompleteQualityCheck`

#### Events

- `WorkOrderCreated`
- `WorkOrderStarted`
- `WorkOrderCompleted`
- `QualityCheckCreated`
- `QualityCheckCompleted`
- `ToyReadyForDelivery` (Integration Event)

#### Endpoints

Implement all Production endpoints from specification

### 4.2 Delivery Module

#### Domain Entities

- `Delivery` aggregate
- `Route` aggregate
- `Reindeer` aggregate
- `ReindeerTeam` aggregate

#### Commands

- `CreateDelivery`
- `UpdateDeliveryStatus`
- `RegisterReindeer`
- `UpdateReindeerHealth`
- `CreateRoute`
- `CreateReindeerTeam`

#### Events

- `DeliveryCreated`
- `DeliveryStatusUpdated`
- `ReindeerRegistered`
- `ReindeerHealthUpdated`
- `RouteCreated`
- `ReindeerTeamCreated`
- `DeliveryCompleted` (Integration Event)

#### Endpoints

Implement all Delivery endpoints from specification

### 4.3 Validation

- [ ] Production module fully implemented
- [ ] Delivery module fully implemented
- [ ] All endpoints from specification implemented
- [ ] All modules compile without warnings
- [ ] All endpoints testable via Swagger

**Deliverable:** Complete API with all three modules functional

---

## Phase 5: Cross-Module Integration with Integration Events

**Duration:** 3-5 days  
**Goal:** Enable communication between modules via integration events

### 5.1 Define Integration Events

#### Marketing/Domain/IntegrationEvents/WishApproved.cs

```csharp
using SantaClaus.Shared.Events;

namespace SantaClaus.Marketing.Domain.IntegrationEvents;

public record WishApproved : IntegrationEvent
{
    public Guid WishId { get; init; }
    public Guid ChildId { get; init; }
    public string ToyDescription { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public int Priority { get; init; }
}
```

#### Production/Domain/IntegrationEvents/ToyReadyForDelivery.cs

```csharp
using SantaClaus.Shared.Events;

namespace SantaClaus.Production.Domain.IntegrationEvents;

public record ToyReadyForDelivery : IntegrationEvent
{
    public Guid WorkOrderId { get; init; }
    public Guid WishId { get; init; }
    public Guid ChildId { get; init; }
    public string ToyDescription { get; init; } = string.Empty;
}
```

#### Delivery/Domain/IntegrationEvents/DeliveryCompleted.cs

```csharp
using SantaClaus.Shared.Events;

namespace SantaClaus.Delivery.Domain.IntegrationEvents;

public record DeliveryCompleted : IntegrationEvent
{
    public Guid DeliveryId { get; init; }
    public Guid ChildId { get; init; }
    public DateTime DeliveredAt { get; init; }
}
```

### 5.2 Implement Integration Event Handlers

#### Production/Domain/IntegrationEventHandlers/WishApprovedHandler.cs

```csharp
using Muflone.Messages.Events;
using Muflone.Messages.Commands;
using SantaClaus.Marketing.Domain.IntegrationEvents;
using SantaClaus.Production.Domain.Commands;

namespace SantaClaus.Production.Domain.IntegrationEventHandlers;

public class WishApprovedHandler : IIntegrationEventHandler<WishApproved>
{
    private readonly ICommandSender _commandSender;

    public WishApprovedHandler(ICommandSender commandSender)
    {
        _commandSender = commandSender;
    }

    public async Task HandleAsync(WishApproved @event, CancellationToken cancellationToken)
    {
        // Create work order in Production module
        var command = new CreateWorkOrder
        {
            AggregateId = Guid.NewGuid(),
            WishId = @event.WishId,
            ToyDescription = @event.ToyDescription,
            Category = @event.Category,
            Priority = MapPriority(@event.Priority)
        };

        await _commandSender.SendAsync(command, cancellationToken);
    }

    private static string MapPriority(int priority) => priority switch
    {
        >= 4 => "Urgent",
        3 => "High",
        2 => "Normal",
        _ => "Low"
    };
}
```

#### Delivery/Domain/IntegrationEventHandlers/ToyReadyForDeliveryHandler.cs

Similar pattern for creating deliveries

#### Marketing/Domain/IntegrationEventHandlers/DeliveryCompletedHandler.cs

Similar pattern for sending notifications

### 5.3 Register Integration Event Handlers

In each module's `FacadeHelper`:

```csharp
// Register integration event handlers
services.AddIntegrationEventHandler<WishApprovedHandler>();
services.AddIntegrationEventHandler<ToyReadyForDeliveryHandler>();
services.AddIntegrationEventHandler<DeliveryCompletedHandler>();
```

### 5.4 Publish Integration Events

Modify domain event handlers to publish integration events:

```csharp
public class WishApprovedHandler : IDomainEventHandler<WishApprovedEvent>
{
    private readonly IEventBus _eventBus;

    public async Task HandleAsync(WishApprovedEvent @event, CancellationToken cancellationToken)
    {
        // Update read model projection
        // ...

        // Publish integration event
        var integrationEvent = new WishApproved
        {
            WishId = @event.AggregateId,
            ChildId = @event.ChildId,
            ToyDescription = @event.Description,
            Category = @event.Category,
            Priority = @event.Priority,
            CorrelationId = @event.CorrelationId
        };

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
```

### 5.5 Validation

- [ ] Integration events defined for all cross-module interactions
- [ ] Integration event handlers implemented
- [ ] Handlers registered with Muflone
- [ ] End-to-end flow works: Marketing → Production → Delivery → Marketing
- [ ] No direct dependencies between module projects
- [ ] Events are properly published and consumed

**Deliverable:** Fully integrated system with event-driven communication

---

## Phase 6: Architectural Tests Implementation

**Duration:** 3-4 days  
**Goal:** Ensure module isolation through comprehensive architectural tests

### 6.1 Create Test Projects

For each module:

```bash
dotnet new xunit -n SantaClaus.{Module}.Tests -o tests/SantaClaus.{Module}.Tests
dotnet add tests/SantaClaus.{Module}.Tests package NetArchTest.Rules
```

Add project references as per `architecture-tests.prompt.md`

### 6.2 Add Assembly Marker Classes

In each layer project, add `AssemblyMarker.cs`:

```csharp
namespace SantaClaus.{Module}.{Layer};

/// <summary>
/// Marker class for assembly reference in architecture tests.
/// </summary>
public class AssemblyMarker
{
}
```

### 6.3 Implement ArchitectureTests.cs

For each module, create comprehensive layer dependency tests:

```csharp
using NetArchTest.Rules;
using Xunit;

namespace SantaClaus.Marketing.Tests;

public class ArchitectureTests
{
    private const string DomainNamespace = "SantaClaus.Marketing.Domain";
    private const string InfrastructureNamespace = "SantaClaus.Marketing.Infrastructure";
    private const string FacadeNamespace = "SantaClaus.Marketing.Facade";
    private const string ReadModelNamespace = "SantaClaus.Marketing.ReadModel";
    private const string SharedKernelNamespace = "SantaClaus.Marketing.SharedKernel";

    [Fact]
    public void Domain_Should_Not_Depend_On_Infrastructure()
    {
        var domain = Types.InAssembly(typeof(SantaClaus.Marketing.Domain.AssemblyMarker).Assembly);

        var result = domain
            .ShouldNot()
            .HaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, 
            $"Domain should not depend on Infrastructure. Violations: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    // Implement all 10 tests from architecture-tests.prompt.md
}
```

### 6.4 Implement ModuleIsolationTests.cs

```csharp
using NetArchTest.Rules;
using Xunit;

namespace SantaClaus.Marketing.Tests;

public class ModuleIsolationTests
{
    private const string CurrentModuleNamespace = "SantaClaus.Marketing";
    
    private static readonly string[] OtherModules = new[]
    {
        "SantaClaus.Production",
        "SantaClaus.Delivery"
    };

    [Fact]
    public void Domain_Should_Not_Reference_Any_Other_Module()
    {
        var domainTypes = Types.InNamespace($"{CurrentModuleNamespace}.Domain");

        foreach (var otherModule in OtherModules)
        {
            var result = domainTypes
                .ShouldNot()
                .HaveDependencyOn(otherModule)
                .GetResult();

            Assert.True(result.IsSuccessful,
                $"Domain should not reference {otherModule}. Violations: {string.Join(", ", result.FailingTypeNames ?? [])}");
        }
    }

    // Implement all isolation tests
}
```

### 6.5 Implement RestProjectTests.cs

```csharp
using NetArchTest.Rules;
using Xunit;

namespace SantaClaus.Marketing.Tests;

public class RestProjectTests
{
    private const string RestNamespace = "SantaClaus.Rest";
    private const string CurrentModuleNamespace = "SantaClaus.Marketing";

    [Fact]
    public void Rest_Should_Only_Depend_On_Facade_Projects()
    {
        var restTypes = Types.InNamespace(RestNamespace);

        var domainResult = restTypes
            .ShouldNot()
            .HaveDependencyOn($"{CurrentModuleNamespace}.Domain")
            .GetResult();

        Assert.True(domainResult.IsSuccessful,
            $"Rest should not depend on Domain. Violations: {string.Join(", ", domainResult.FailingTypeNames ?? [])}");

        // Test Infrastructure and ReadModel dependencies
    }
}
```

### 6.6 Run and Validate Tests

```bash
# Run all tests
dotnet test

# Run architecture tests only
dotnet test --filter "FullyQualifiedName~ArchitectureTests|ModuleIsolationTests|RestProjectTests"

# Generate test report
dotnet test --logger "trx;LogFileName=architecture-tests.trx"
```

### 6.7 Fix Any Violations

If tests fail:

1. Identify the violation from error message
2. Remove unauthorized project reference
3. Refactor to use proper abstraction
4. Re-run tests until all pass

### 6.8 Validation

- [ ] Test projects created for all modules
- [ ] All assembly markers in place
- [ ] ArchitectureTests implemented (10 tests per module)
- [ ] ModuleIsolationTests implemented (4 tests per module)
- [ ] RestProjectTests implemented (2 tests per module)
- [ ] All tests compile
- [ ] **ALL tests pass (100% success rate)**
- [ ] No architectural violations detected

**Deliverable:** Comprehensive architectural test suite with 100% pass rate

---

## Phase 7: Documentation & Polish

**Duration:** 2-3 days  
**Goal:** Complete documentation and prepare for deployment

### 7.1 Update README.md

Create comprehensive README with:

- Project overview
- Architecture diagram
- Module descriptions
- Setup instructions
- Run instructions
- API documentation links
- Testing instructions
- Deployment guide

### 7.2 Add API Documentation

Enhance Swagger documentation:

- Add XML comments to all endpoints
- Add example requests/responses
- Document error codes
- Add authentication notes (for future)

### 7.3 Create Architecture Decision Records (ADRs)

Document key decisions:

- ADR-001: Why Muflone for CQRS+ES
- ADR-002: Module isolation strategy
- ADR-003: Integration event patterns
- ADR-004: InMemory vs RabbitMQ transport

### 7.4 Performance Testing

- Load test critical endpoints
- Measure event processing latency
- Identify bottlenecks
- Document performance characteristics

### 7.5 Security Audit

- Review input validation
- Check for injection vulnerabilities
- Validate error messages don't leak sensitive info
- Plan authentication/authorization strategy

**Deliverable:** Production-ready application with complete documentation

---

## Success Criteria

### Functional Requirements

- [ ] All 48 endpoints from specification implemented
- [ ] All CRUD operations work correctly
- [ ] Cross-module integration via events works
- [ ] Event sourcing: all aggregates reconstitutable from events
- [ ] Read models accurately reflect current state

### Technical Requirements

- [ ] Zero compilation warnings (`TreatWarningsAsErrors=true`)
- [ ] All architectural tests pass (100%)
- [ ] No cross-module project dependencies
- [ ] Muflone properly integrated
- [ ] Swagger documentation complete and accurate
- [ ] Logging and telemetry working

### Code Quality

- [ ] Code follows DDD principles
- [ ] Clear separation of concerns
- [ ] Consistent naming conventions
- [ ] Adequate inline comments for complex logic
- [ ] No code smells or anti-patterns

---

## Risk Management

### High Priority Risks

| Risk | Impact | Mitigation |
|------|--------|-----------|
| Muflone learning curve | Delays | Study docs thoroughly in Phase 0, start simple in Phase 2 |
| Event sourcing complexity | Architecture issues | Keep aggregates small, use clear event naming |
| Module isolation violations | Failed arch tests | Review dependencies frequently, run tests often |
| Performance issues | Slow API | Profile early, use async/await properly, consider caching |

### Medium Priority Risks

| Risk | Impact | Mitigation |
|------|--------|-----------|
| Incomplete specification | Missing features | Clarify requirements early, iterate with stakeholders |
| Testing gaps | Bugs in production | Write tests alongside implementation |
| Event versioning | Breaking changes | Plan event schema evolution from start |

---

## Timeline Summary

| Phase | Duration | Cumulative |
|-------|----------|-----------|
| Phase 0: Prerequisites | 1 day | 1 day |
| Phase 1: Scaffolding | 2-3 days | 4 days |
| Phase 2: Muflone Integration | 3-4 days | 8 days |
| Phase 3: Marketing Module | 5-7 days | 15 days |
| Phase 4: Production & Delivery | 10-14 days | 29 days |
| Phase 5: Cross-Module Integration | 3-5 days | 34 days |
| Phase 6: Architectural Tests | 3-4 days | 38 days |
| Phase 7: Documentation & Polish | 2-3 days | 41 days |

**Total Estimated Duration:** 6-8 weeks (41 working days)

---

## Team Structure Recommendation

**Minimum Team:**

- 1 Senior .NET Developer (DDD + CQRS+ES experience)
- 1 Mid-level .NET Developer
- 0.5 Tech Lead (reviews + architecture guidance)

**Optimal Team:**

- 1 Tech Lead / Architect
- 2 Senior .NET Developers (one per module group)
- 1 Mid-level .NET Developer
- 0.5 QA Engineer (testing focus)

---

## Deployment Strategy

### Phase 1-4: Development

- Local development with InMemory transport
- SQLite for event store (Phase 3+)
- Docker for dependencies (future RabbitMQ)

### Phase 5: Staging

- Migrate to RabbitMQ transport
- PostgreSQL or SQL Server for event store
- Azure App Service or Kubernetes
- Separate instance per environment

### Phase 6: Production

- Azure Service Bus or RabbitMQ cluster
- Dedicated event store database with backups
- Kubernetes with multiple replicas
- Monitoring and alerting (Application Insights, Prometheus)
- API Gateway (Azure API Management or Kong)

---

## Next Steps

1. **Review this plan** with the team and stakeholders
2. **Adjust timelines** based on team capacity
3. **Set up development environment** (Phase 0)
4. **Create project repository** and configure CI/CD
5. **Begin Phase 1** scaffolding
6. **Schedule daily standups** and weekly reviews

---

## Appendix A: Useful Commands

### Build and Run

```bash
# Restore and build
dotnet restore
dotnet build

# Run API
cd src/SantaClaus.Rest
dotnet run

# Run with hot reload
dotnet watch run
```

### Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test category
dotnet test --filter "Category=Architecture"
```

### Code Quality

```bash
# Format code
dotnet format

# Analyze code
dotnet build -warnaserror

# Check for outdated packages
dotnet list package --outdated
```

---

## Appendix B: Reference Links

- **Muflone GitHub:** <https://github.com/CQRS-Muflone/Muflone>
- **Muflone NuGet:** <https://www.nuget.org/packages/Muflone>
- **NetArchTest:** <https://github.com/BenMorris/NetArchTest>
- **DDD Reference:** Eric Evans - Domain-Driven Design
- **CQRS Pattern:** Martin Fowler - CQRS
- **Event Sourcing:** Martin Fowler - Event Sourcing

---

**Document Version:** 1.0  
**Created:** December 7, 2025  
**Status:** Ready for Implementation
