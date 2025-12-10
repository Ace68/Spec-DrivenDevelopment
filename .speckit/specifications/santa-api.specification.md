# Santa Claus Work Management API - Specification

## Overview

This specification defines a Minimal API for managing Santa Claus operations, organized around three main bounded contexts: Marketing, Production, and Delivery. The API follows Domain-Driven Design principles with modular architecture and complete isolation between bounded contexts.

## Project Information

- **Project Name**: SantaClaus
- **Architecture**: Minimal API with .NET 9
- **Pattern**: Domain-Driven Design with Bounded Contexts
- **HTTP Port**: 5000 (Development)

## Bounded Contexts / Modules

### 1. Marketing Module

Manages the collection, interpretation, and processing of children's requests, including external communications.

**Responsibilities:**

- Letter Management: Receive and store letters from children
- Children Registry: Maintain information about children (behavior, location, history)
- Wish Processing: Interpret requests and plan orders
- Notification & Communication: Send updates to children and families

### 2. Production Module

Handles the entire toy creation process: manufacturing and quality assurance.

**Responsibilities:**

- Toy Manufacturing: Coordinate workshop operations and toy production
- Quality Assurance: Verify toy quality before delivery approval

### 3. Delivery Module

Manages Christmas logistics: journey planning, reindeer fleet, and delivery execution.

**Responsibilities:**

- Logistics & Delivery: Plan routes and execute deliveries
- Reindeer Fleet Management: Track and manage reindeer health and assignments
- Mission Control: Monitor real-time delivery progress on Christmas Eve

## API Endpoints Specification

### Marketing Module

Base path: `/v1/marketing`

#### Letter Management

```http
POST /v1/marketing/letters
```

Create a new letter from a child.

**Request Body:**

```json
{
  "childId": "string (GUID)",
  "content": "string",
  "receivedDate": "datetime",
  "language": "string (ISO 639-1)"
}
```

**Response:** `201 Created`

```json
{
  "letterId": "string (GUID)",
  "childId": "string (GUID)",
  "status": "Received",
  "createdAt": "datetime"
}
```

---

```http
GET /v1/marketing/letters/{letterId}
```

Retrieve a specific letter by ID.

**Response:** `200 OK`

```json
{
  "letterId": "string (GUID)",
  "childId": "string (GUID)",
  "content": "string",
  "receivedDate": "datetime",
  "language": "string",
  "status": "Received|Processing|Processed",
  "processedAt": "datetime?"
}
```

---

```http
GET /v1/marketing/letters?childId={childId}&status={status}
```

List letters with optional filtering.

**Query Parameters:**

- `childId` (optional): Filter by child ID
- `status` (optional): Filter by status (Received, Processing, Processed)
- `page` (optional, default: 1): Page number
- `pageSize` (optional, default: 20): Items per page

**Response:** `200 OK`

```json
{
  "items": [
    {
      "letterId": "string (GUID)",
      "childId": "string (GUID)",
      "receivedDate": "datetime",
      "status": "string"
    }
  ],
  "totalCount": "integer",
  "page": "integer",
  "pageSize": "integer"
}
```

#### Children Registry

```http
POST /v1/marketing/children
```

Register a new child in the system.

**Request Body:**

```json
{
  "firstName": "string",
  "lastName": "string",
  "dateOfBirth": "date",
  "address": {
    "street": "string",
    "city": "string",
    "country": "string",
    "postalCode": "string",
    "coordinates": {
      "latitude": "decimal",
      "longitude": "decimal"
    }
  }
}
```

**Response:** `201 Created`

```json
{
  "childId": "string (GUID)",
  "firstName": "string",
  "lastName": "string",
  "registeredAt": "datetime"
}
```

---

```http
GET /v1/marketing/children/{childId}
```

Get child information by ID.

**Response:** `200 OK`

```json
{
  "childId": "string (GUID)",
  "firstName": "string",
  "lastName": "string",
  "dateOfBirth": "date",
  "address": {
    "street": "string",
    "city": "string",
    "country": "string",
    "postalCode": "string",
    "coordinates": {
      "latitude": "decimal",
      "longitude": "decimal"
    }
  },
  "behaviorScore": "integer (0-100)",
  "letterCount": "integer"
}
```

---

```http
PUT /v1/marketing/children/{childId}/behavior
```

Update child's behavior score.

**Request Body:**

```json
{
  "behaviorScore": "integer (0-100)",
  "notes": "string"
}
```

**Response:** `200 OK`

---

```http
GET /v1/marketing/children?country={country}&minBehavior={minBehavior}
```

List children with optional filtering.

**Query Parameters:**

- `country` (optional): Filter by country
- `minBehavior` (optional): Minimum behavior score
- `page` (optional, default: 1)
- `pageSize` (optional, default: 20)

**Response:** `200 OK`

#### Wish Processing

```http
POST /v1/marketing/wishes/process
```

Process a letter and extract wishes.

**Request Body:**

```json
{
  "letterId": "string (GUID)"
}
```

**Response:** `202 Accepted`

```json
{
  "wishProcessingId": "string (GUID)",
  "letterId": "string (GUID)",
  "status": "Processing",
  "startedAt": "datetime"
}
```

---

```http
GET /v1/marketing/wishes/{childId}
```

Get processed wishes for a child.

**Response:** `200 OK`

```json
{
  "childId": "string (GUID)",
  "wishes": [
    {
      "wishId": "string (GUID)",
      "description": "string",
      "category": "Toy|Book|Game|Other",
      "priority": "integer (1-5)",
      "status": "Pending|Approved|Rejected|Fulfilled"
    }
  ]
}
```

---

```http
PUT /v1/marketing/wishes/{wishId}/approve
```

Approve a wish for production.

**Request Body:**

```json
{
  "approvedBy": "string",
  "notes": "string"
}
```

**Response:** `200 OK`

#### Notifications

```http
POST /v1/marketing/notifications
```

Send a notification to a child.

**Request Body:**

```json
{
  "childId": "string (GUID)",
  "type": "LetterReceived|WishApproved|DeliveryScheduled|DeliveryComplete",
  "message": "string",
  "channel": "Email|Letter|App"
}
```

**Response:** `201 Created`

```json
{
  "notificationId": "string (GUID)",
  "sentAt": "datetime",
  "status": "Sent"
}
```

---

```http
GET /v1/marketing/notifications/{childId}
```

Get notification history for a child.

**Response:** `200 OK`

```json
{
  "notifications": [
    {
      "notificationId": "string (GUID)",
      "type": "string",
      "sentAt": "datetime",
      "status": "Sent|Failed"
    }
  ]
}
```

---

### Production Module

Base path: `/v1/production`

#### Toy Manufacturing

```http
POST /v1/production/work-orders
```

Create a new toy manufacturing work order.

**Request Body:**

```json
{
  "wishId": "string (GUID)",
  "toyDescription": "string",
  "category": "string",
  "quantity": "integer",
  "dueDate": "date",
  "priority": "Low|Normal|High|Urgent"
}
```

**Response:** `201 Created`

```json
{
  "workOrderId": "string (GUID)",
  "wishId": "string (GUID)",
  "status": "Pending",
  "createdAt": "datetime"
}
```

---

```http
GET /v1/production/work-orders/{workOrderId}
```

Get work order details.

**Response:** `200 OK`

```json
{
  "workOrderId": "string (GUID)",
  "wishId": "string (GUID)",
  "toyDescription": "string",
  "category": "string",
  "quantity": "integer",
  "status": "Pending|InProgress|Completed|Cancelled",
  "assignedElf": "string?",
  "startedAt": "datetime?",
  "completedAt": "datetime?",
  "dueDate": "date"
}
```

---

```http
PUT /v1/production/work-orders/{workOrderId}/start
```

Start production on a work order.

**Request Body:**

```json
{
  "assignedElf": "string"
}
```

**Response:** `200 OK`

---

```http
PUT /v1/production/work-orders/{workOrderId}/complete
```

Mark work order as completed.

**Response:** `200 OK`

---

```http
GET /v1/production/work-orders?status={status}&dueDate={dueDate}
```

List work orders with filtering.

**Query Parameters:**

- `status` (optional): Filter by status
- `dueDate` (optional): Filter by due date
- `category` (optional): Filter by toy category
- `page` (optional, default: 1)
- `pageSize` (optional, default: 20)

**Response:** `200 OK`

---

```http
GET /v1/production/statistics
```

Get production statistics.

**Response:** `200 OK`

```json
{
  "totalWorkOrders": "integer",
  "pendingCount": "integer",
  "inProgressCount": "integer",
  "completedCount": "integer",
  "completedToday": "integer",
  "averageCompletionTime": "decimal (hours)",
  "upcomingDeadlines": [
    {
      "dueDate": "date",
      "count": "integer"
    }
  ]
}
```

#### Quality Assurance

```http
POST /v1/production/quality-checks
```

Create a quality check for completed toys.

**Request Body:**

```json
{
  "workOrderId": "string (GUID)",
  "inspectorElf": "string"
}
```

**Response:** `201 Created`

```json
{
  "qualityCheckId": "string (GUID)",
  "workOrderId": "string (GUID)",
  "status": "Pending",
  "createdAt": "datetime"
}
```

---

```http
PUT /v1/production/quality-checks/{qualityCheckId}/complete
```

Complete a quality check inspection.

**Request Body:**

```json
{
  "passed": "boolean",
  "defects": [
    {
      "type": "string",
      "severity": "Minor|Major|Critical",
      "description": "string"
    }
  ],
  "notes": "string"
}
```

**Response:** `200 OK`

```json
{
  "qualityCheckId": "string (GUID)",
  "passed": "boolean",
  "completedAt": "datetime",
  "nextAction": "ApprovedForDelivery|ReturnToWorkshop|Scrap"
}
```

---

```http
GET /v1/production/quality-checks/{qualityCheckId}
```

Get quality check details.

**Response:** `200 OK`

```json
{
  "qualityCheckId": "string (GUID)",
  "workOrderId": "string (GUID)",
  "inspectorElf": "string",
  "status": "Pending|InProgress|Completed",
  "passed": "boolean?",
  "defects": [],
  "notes": "string",
  "completedAt": "datetime?"
}
```

---

```http
GET /v1/production/quality-checks?status={status}&passed={passed}
```

List quality checks with filtering.

**Query Parameters:**

- `status` (optional)
- `passed` (optional): true/false
- `page` (optional, default: 1)
- `pageSize` (optional, default: 20)

**Response:** `200 OK`

---

### Delivery Module

Base path: `/v1/delivery`

#### Logistics & Delivery

```http
POST /v1/delivery/deliveries
```

Create a delivery order.

**Request Body:**

```json
{
  "childId": "string (GUID)",
  "workOrderIds": ["string (GUID)"],
  "scheduledDate": "date",
  "address": {
    "street": "string",
    "city": "string",
    "country": "string",
    "postalCode": "string",
    "coordinates": {
      "latitude": "decimal",
      "longitude": "decimal"
    }
  }
}
```

**Response:** `201 Created`

```json
{
  "deliveryId": "string (GUID)",
  "childId": "string (GUID)",
  "status": "Scheduled",
  "scheduledDate": "date",
  "createdAt": "datetime"
}
```

---

```http
GET /v1/delivery/deliveries/{deliveryId}
```

Get delivery details.

**Response:** `200 OK`

```json
{
  "deliveryId": "string (GUID)",
  "childId": "string (GUID)",
  "workOrderIds": ["string (GUID)"],
  "status": "Scheduled|InTransit|Delivered|Failed",
  "scheduledDate": "date",
  "actualDeliveryTime": "datetime?",
  "routeId": "string (GUID)?",
  "address": {},
  "notes": "string"
}
```

---

```http
PUT /v1/delivery/deliveries/{deliveryId}/status
```

Update delivery status.

**Request Body:**

```json
{
  "status": "InTransit|Delivered|Failed",
  "notes": "string",
  "timestamp": "datetime"
}
```

**Response:** `200 OK`

---

```http
GET /v1/delivery/deliveries?status={status}&country={country}&date={date}
```

List deliveries with filtering.

**Query Parameters:**

- `status` (optional)
- `country` (optional)
- `date` (optional): Filter by scheduled date
- `page` (optional, default: 1)
- `pageSize` (optional, default: 20)

**Response:** `200 OK`

---

```http
POST /v1/delivery/routes/optimize
```

Generate optimized delivery routes for a given date.

**Request Body:**

```json
{
  "deliveryDate": "date",
  "region": "string?"
}
```

**Response:** `200 OK`

```json
{
  "routes": [
    {
      "routeId": "string (GUID)",
      "region": "string",
      "deliveryIds": ["string (GUID)"],
      "estimatedDuration": "decimal (hours)",
      "totalDistance": "decimal (km)"
    }
  ]
}
```

---

```http
GET /v1/delivery/routes/{routeId}
```

Get route details with ordered stops.

**Response:** `200 OK`

```json
{
  "routeId": "string (GUID)",
  "region": "string",
  "stops": [
    {
      "sequence": "integer",
      "deliveryId": "string (GUID)",
      "address": {},
      "estimatedArrival": "datetime"
    }
  ],
  "assignedReindeerTeam": "string (GUID)?",
  "status": "Planned|Active|Completed"
}
```

#### Reindeer Fleet Management

```http
POST /v1/delivery/reindeer
```

Register a new reindeer.

**Request Body:**

```json
{
  "name": "string",
  "specialAbility": "string?",
  "maxLoadCapacity": "integer (kg)"
}
```

**Response:** `201 Created`

```json
{
  "reindeerId": "string (GUID)",
  "name": "string",
  "registeredAt": "datetime"
}
```

---

```http
GET /v1/delivery/reindeer/{reindeerId}
```

Get reindeer details.

**Response:** `200 OK`

```json
{
  "reindeerId": "string (GUID)",
  "name": "string",
  "specialAbility": "string?",
  "maxLoadCapacity": "integer",
  "healthStatus": "Healthy|Resting|Injured|Unavailable",
  "currentAssignment": "string (GUID)?",
  "totalDeliveriesCompleted": "integer"
}
```

---

```http
PUT /v1/delivery/reindeer/{reindeerId}/health
```

Update reindeer health status.

**Request Body:**

```json
{
  "healthStatus": "Healthy|Resting|Injured|Unavailable",
  "notes": "string",
  "assessedBy": "string (veterinarian)"
}
```

**Response:** `200 OK`

---

```http
GET /v1/delivery/reindeer?status={status}&available={available}
```

List reindeer with filtering.

**Query Parameters:**

- `status` (optional): Filter by health status
- `available` (optional): true for unassigned reindeer
- `page` (optional, default: 1)
- `pageSize` (optional, default: 20)

**Response:** `200 OK`

---

```http
POST /v1/delivery/reindeer-teams
```

Create a reindeer team for a route.

**Request Body:**

```json
{
  "routeId": "string (GUID)",
  "reindeerIds": ["string (GUID)"],
  "leadReindeer": "string (GUID)"
}
```

**Response:** `201 Created`

```json
{
  "teamId": "string (GUID)",
  "routeId": "string (GUID)",
  "createdAt": "datetime"
}
```

---

```http
GET /v1/delivery/reindeer-teams/{teamId}
```

Get team composition and status.

**Response:** `200 OK`

```json
{
  "teamId": "string (GUID)",
  "routeId": "string (GUID)",
  "reindeer": [
    {
      "reindeerId": "string (GUID)",
      "name": "string",
      "role": "Lead|Support"
    }
  ],
  "status": "Assigned|Active|Completed|Disbanded"
}
```

#### Mission Control

```http
GET /v1/delivery/mission-control/dashboard
```

Get real-time mission control dashboard (Christmas Eve).

**Response:** `200 OK`

```json
{
  "date": "date",
  "overallStatus": "Scheduled|InProgress|Completed",
  "statistics": {
    "totalDeliveries": "integer",
    "completedDeliveries": "integer",
    "inTransitDeliveries": "integer",
    "failedDeliveries": "integer",
    "percentComplete": "decimal"
  },
  "activeRoutes": [
    {
      "routeId": "string (GUID)",
      "region": "string",
      "teamId": "string (GUID)",
      "currentStop": "integer",
      "totalStops": "integer",
      "estimatedCompletion": "datetime"
    }
  ]
}
```

---

```http
GET /v1/delivery/mission-control/tracking/{routeId}
```

Track real-time position and progress of a route.

**Response:** `200 OK`

```json
{
  "routeId": "string (GUID)",
  "teamId": "string (GUID)",
  "currentPosition": {
    "latitude": "decimal",
    "longitude": "decimal",
    "altitude": "decimal (meters)"
  },
  "currentStop": {
    "deliveryId": "string (GUID)",
    "address": {},
    "arrivedAt": "datetime?",
    "status": "Approaching|Delivering|Completed"
  },
  "nextStop": {
    "deliveryId": "string (GUID)",
    "estimatedArrival": "datetime"
  },
  "lastUpdate": "datetime"
}
```

---

```http
GET /v1/delivery/mission-control/alerts
```

Get active alerts and issues.

**Response:** `200 OK`

```json
{
  "alerts": [
    {
      "alertId": "string (GUID)",
      "severity": "Info|Warning|Critical",
      "type": "WeatherDelay|ReindeerHealth|RouteDeviation|Other",
      "message": "string",
      "routeId": "string (GUID)?",
      "createdAt": "datetime",
      "resolved": "boolean"
    }
  ]
}
```

---

```http
POST /v1/delivery/mission-control/alerts/{alertId}/resolve
```

Mark an alert as resolved.

**Request Body:**

```json
{
  "resolution": "string",
  "resolvedBy": "string"
}
```

**Response:** `200 OK`

---

## Cross-Module Integration Points

### Marketing → Production

When a wish is approved in Marketing:

1. Marketing publishes `WishApproved` domain event
2. Production consumes event and creates work order

### Production → Delivery

When a toy passes quality assurance:

1. Production publishes `ToyReadyForDelivery` domain event
2. Delivery can create delivery order

### Delivery → Marketing

When a delivery is completed:

1. Delivery publishes `DeliveryCompleted` domain event
2. Marketing can send notification to child

## Common Data Types

### Address

```json
{
  "street": "string",
  "city": "string",
  "country": "string",
  "postalCode": "string",
  "coordinates": {
    "latitude": "decimal",
    "longitude": "decimal"
  }
}
```

### Coordinates

```json
{
  "latitude": "decimal",
  "longitude": "decimal"
}
```

### Pagination Response

```json
{
  "items": [],
  "totalCount": "integer",
  "page": "integer",
  "pageSize": "integer",
  "totalPages": "integer"
}
```

## Error Responses

All endpoints return consistent error responses:

### 400 Bad Request

```json
{
  "type": "ValidationError",
  "title": "One or more validation errors occurred",
  "status": 400,
  "errors": {
    "fieldName": ["error message"]
  }
}
```

### 404 Not Found

```json
{
  "type": "NotFound",
  "title": "Resource not found",
  "status": 404,
  "detail": "The requested resource was not found"
}
```

### 409 Conflict

```json
{
  "type": "Conflict",
  "title": "Operation conflict",
  "status": 409,
  "detail": "The operation conflicts with the current state"
}
```

### 500 Internal Server Error

```json
{
  "type": "InternalError",
  "title": "An error occurred",
  "status": 500,
  "detail": "An unexpected error occurred"
}
```

## Authentication & Authorization

**Phase 1 (Initial):** No authentication required

**Phase 2 (Future):**

- API Key authentication for external systems
- Role-based access control (RBAC)
- Roles: SantaAdmin, ElfWorker, DeliveryCoordinator, ReadOnly

## API Versioning

- Current version: v1
- Version prefix in URL: `/v1/`
- Breaking changes require new version (`/v2/`)

## Rate Limiting

**Phase 1:** Not implemented

**Phase 2 (Future):**

- 1000 requests per hour per IP
- 10000 requests per hour for authenticated users
- Headers: `X-RateLimit-Limit`, `X-RateLimit-Remaining`, `X-RateLimit-Reset`

## Performance Requirements

- Response time: < 200ms for GET requests (p95)
- Response time: < 500ms for POST/PUT requests (p95)
- Availability: 99.9% during November-December
- Availability: 99.99% on December 24-25

## Documentation

- OpenAPI/Swagger documentation at `/swagger`
- Each endpoint includes:
  - Description
  - Request/Response examples
  - Status codes
  - Authentication requirements (when implemented)

## Muflone Framework - Strong Typing Pattern

The project uses **Muflone 8.5.0**, a CQRS+ES (Command Query Responsibility Segregation + Event Sourcing) framework that enforces strong typing throughout the application. All commands must derive from the base `Muflone.Messages.Commands.Command` class.

### Strong Type Pattern

Rather than using primitive types (strings, integers) in commands and events, the architecture requires custom types that provide semantic meaning and type safety:

#### Domain IDs (DomainId-derived classes)

For aggregate identifiers, create sealed classes that derive from `Muflone.Core.DomainId`:

**ChildId example:**

```csharp
using Muflone.Core;

namespace SantaClaus.Marketing.SharedKernel.CustomTypes;

public sealed class ChildId : DomainId
{
    public ChildId(Guid value) : base(value.ToString()) { }
    public ChildId(string value) : base(value) { }

    public static implicit operator ChildId(Guid guid) => new(guid);
    public static implicit operator Guid(ChildId childId) => Guid.Parse(childId.Value);
}
```

**LetterId example (TASK-303 - Letter aggregate):**

```csharp
using Muflone.Core;

namespace SantaClaus.Marketing.SharedKernel.CustomTypes;

public sealed class LetterId : DomainId
{
    public LetterId(Guid value) : base(value.ToString()) { }
    public LetterId(string value) : base(value) { }

    public static implicit operator Guid(LetterId? letterId) => 
        letterId != null ? Guid.Parse(letterId.Value) : Guid.Empty;
    public static implicit operator LetterId(Guid value) => new(value);
    public static implicit operator string(LetterId? letterId) => 
        letterId?.Value ?? string.Empty;
}
```

#### Value Types (Record-based)

For non-identifier types, create sealed records with implicit operators:

**ToyDescription and WishPriority examples (TASK-305 - Wish aggregate):**

```csharp
namespace SantaClaus.Marketing.SharedKernel.CustomTypes;

public sealed record ToyDescription(string Value)
{
    public static implicit operator ToyDescription(string value) => new(value);
    public static implicit operator string(ToyDescription toyDescription) => toyDescription.Value;
}

public sealed record WishPriority(int Value)
{
    public static implicit operator WishPriority(int value) => new(value);
    public static implicit operator int(WishPriority priority) => priority.Value;
}
```

**LetterContent and LetterLanguage examples (TASK-303 - Letter aggregate):**

```csharp
namespace SantaClaus.Marketing.SharedKernel.CustomTypes;

public sealed record LetterContent(string Value)
{
    public static implicit operator string(LetterContent? content) => content?.Value ?? string.Empty;
    public static implicit operator LetterContent(string value) => new(value);
}

public sealed record LetterLanguage(string Value)
{
    public static implicit operator string(LetterLanguage? language) => language?.Value ?? string.Empty;
    public static implicit operator LetterLanguage(string value) => new(value);
}
```

### Commands with Strong Types

All commands must use custom types instead of primitives:

**Incorrect:**

```csharp
public sealed record CreateWish : Command
{
    public required Guid ChildId { get; init; }
    public required string ToyDescription { get; init; }
    public required int Priority { get; init; }
}
```

**Correct:**

```csharp
public sealed record CreateWish : Command
{
    public required ChildId ChildId { get; init; }
    public required ToyDescription ToyDescription { get; init; }
    public required WishPriority Priority { get; init; }
}
```

### Benefits

- **Type Safety**: Compiler prevents mixing incompatible types
- **Domain-Driven**: Types reflect domain concepts
- **Validation**: Custom types encapsulate validation logic
- **Implicit Conversion**: Seamless interop with primitives via operators
- **Muflone Compatibility**: Required for proper serialization and handler registration

### Command Handler Pattern

Command handlers must inherit from `CommandHandlerBaseAsync<TCommand>` and implement the CQRS pattern:

```csharp
public sealed class CreateWishHandler(IRepository repository, ILoggerFactory loggerFactory)
    : CommandHandlerBaseAsync<CreateWish>(repository, loggerFactory)
{
    public override async Task HandleAsync(CreateWish command, CancellationToken cancellationToken = new())
    {
        var wish = Wish.Create(
            Guid.Parse(command.AggregateId.Value),
            command.ChildId,
            command.ToyDescription,
            command.Priority
        );
        await repository.SaveAsync(wish, Guid.NewGuid(), cancellationToken).ConfigureAwait(false);
    }
}
```

### Folder Structure for Custom Types

Each module's SharedKernel must include a `CustomTypes` folder:

```
SantaClaus.Marketing.SharedKernel/
├── Commands/
│   ├── CreateWish.cs
│   ├── ApproveWish.cs
│   ├── RejectWish.cs
│   ├── CreateLetter.cs
│   └── ProcessLetter.cs
├── CustomTypes/
│   ├── ChildId.cs (TASK-305: Wish aggregate ID)
│   ├── ToyDescription.cs (TASK-305)
│   ├── WishPriority.cs (TASK-305)
│   ├── RejectionReason.cs (TASK-305)
│   ├── LetterId.cs (TASK-303: Letter aggregate ID)
│   ├── LetterContent.cs (TASK-303)
│   ├── LetterLanguage.cs (TASK-303)
│   └── LetterStatusValue.cs (TASK-303)
└── Events/
    ├── WishCreated.cs
    ├── WishApproved.cs
    ├── WishRejected.cs
    ├── LetterCreated.cs
    └── LetterProcessed.cs
```

## Implementation Notes

### Module Structure

Each module follows the standard structure:

- `SantaClaus.{Module}.Facade`: Public API endpoints
- `SantaClaus.{Module}.Domain`: Business logic and entities
- `SantaClaus.{Module}.ReadModel`: DTOs and query models
- `SantaClaus.{Module}.Infrastructure`: Data access and external services
- `SantaClaus.{Module}.SharedKernel`: Module-specific shared types

### Module Names

- Marketing
- Production
- Delivery

### Endpoint Tags for OpenAPI

- Marketing → Tag: "Marketing - Letters", "Marketing - Children", "Marketing - Wishes", "Marketing - Notifications"
- Production → Tag: "Production - Manufacturing", "Production - Quality"
- Delivery → Tag: "Delivery - Logistics", "Delivery - Reindeer", "Delivery - Mission Control"

## Next Steps

1. Set up project scaffolding using minimal-api-scaffolding.prompt.md
2. Define domain entities for each module
3. Implement repository interfaces
4. Create endpoint implementations
5. Add domain event handling for cross-module integration
6. Implement data persistence
7. Add comprehensive testing

## Success Criteria

- All endpoints documented and discoverable via Swagger
- Each module independently deployable
- Zero cross-module dependencies at domain level
- All responses follow consistent error format
- API compiles and runs without warnings
- Placeholder endpoints respond with HTTP 200
- Clear separation of concerns between modules
