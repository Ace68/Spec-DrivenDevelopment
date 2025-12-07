## 1. Overview

**Muflone** is a **CQRS (Command Query Responsibility Segregation)** and **Event Store** library for .NET, built upon the foundation laid by Jonathan Oliver’s *CommonDomain* as part of *NEventStore* ([GitHub][1]).

* **License:** MIT ([GitHub][1])
* **Latest Stable Version (as of July 31, 2025):** 8.5.0 ([NuGet][2])
* **Target Framework Compatibility:** .NET 8.0 and above (including net8.0-android, net9.0, net10.0, etc.) ([NuGet][2])

---

## 2. Installation

### NuGet (recommended)

To install the latest stable version:

```bash
dotnet add package Muflone --version 8.5.0
```

Or via Package Manager Console:

```powershell
Install-Package Muflone -Version 8.5.0
```

You can also reference it in your `.csproj` via:

```xml
<PackageReference Include="Muflone" Version="8.5.0" />
```

([NuGet][2])

---

## 3. Quick Start & Usage Example

### Before Version 8.3

Traditionally, developers needed to manually register consumers that wrapped command and event handlers:

```csharp
builder.Services.AddMufloneRabbitMQConsumers(new List<IConsumer>
{
    new CreateCartConsumer(repository, connectionFactory, loggerFactory),
    new CartCreatedConsumer(repository, connectionFactory, loggerFactory),
    new ProductCreatedConsumer(repository, connectionFactory, loggerFactory),
});
```

([GitHub][3])

### Since Version 8.3

Muflone now supports **automatic consumer generation**, significantly reducing boilerplate and potential misconfigurations. You just register handlers directly:

```csharp
builder.Services.AddCommandHandler<CreateCartHandler>();
builder.Services.AddDomainEventHandler<CartCreatedHandler>();
builder.Services.AddIntegrationEventHandler<ProductCreatedHandler>();
```

For generic handlers not falling under command or event categories:

```csharp
builder.Services.AddGenericHandler<MyGenericHandler>();
```

Note: The older method of explicit consumer registration remains available for advanced use cases ([GitHub][3], [NuGet][2]).

---

## 4. Features & Capabilities

* **CQRS & Event Store Architecture:** Enables separation of command and query logic with reliable event sourcing.
* **Message Transport Support:** Through packages like `Muflone.Transport.RabbitMQ`, `Muflone.Transport.Azure`, and `Muflone.Transport.InMemory`, tailored for various messaging scenarios ([GitHub][4]).
* **Saga Support:** For long-running and distributed transactions using `Muflone.Saga` ([GitHub][4]).
* **Testing Support:** `Muflone.SpecificationTests` facilitates behavior-driven and event-driven testing strategies ([GitHub][4]).
* **Transport Flexibility:** Integrates with popular patterns like Azure transports and in-memory queues ([GitHub][4]).

---

## 5. Architecture & Design Principles

* **Modularity:** Core CQRS and Event Store features are decoupled from transport mechanisms.
* **Automatic Consumer Generation:** Reduces manual configuration, improving maintainability and reducing human error.
* **Extensibility:** Developers can mix auto-generated handlers with explicitly registered consumers as needed.

---

## 6. Extending and Customizing

* **Advanced Consumer Logic:** You can still register custom or legacy consumers manually if auto-generation is not suitable for specific scenarios.
* **Transport Extensions:** Choose appropriate transport modules based on project needs—support includes RabbitMQ, Azure services, etc.
* **Saga Orchestration:** Implement complex workflows using the Saga extension.
* **Test Scenarios:** Use `SpecificationTests` to validate event sequences or complex behavior.

---

## 7. Resources & Further Reading

* **GitHub Repository:** For source code, change history, issues, and discussions ([GitHub][1])
* **NuGet Package Page:** For version history, dependencies, and download stats ([NuGet][2], [NuGet Must Haves][5])
* **Announcements:** Developers (Alessandro Colla and Alberto Acerbis) have promoted Muflone as a reliable library for CQRS/ES in .NET ([X (formerly Twitter)][6], [X (formerly Twitter)][7])

---

## 8. Suggested Document Structure (TOC)

1. **Introduction**

   * What is Muflone?
   * When and why to use it (CQRS/Event Sourcing context)

2. **Getting Started**

   * Prerequisites
   * Installation (NuGet CLI, PackageReference)

3. **Core Concepts & Architecture**

   * Overview of CQRS and Event Store
   * Handler registration model (before vs. after v8.3)
   * Overview of transports, saga workflows, testing support

4. **Quick Start Example**

   * Setup commands and event handlers
   * Example transport and saga usage

5. **Customization & Advanced Topics**

   * Manual consumer registration
   * Extending or replacing transport modules
   * Advanced saga scenarios
   * Testing with `SpecificationTests`

6. **Migration Guide**

   * Upgrade from pre-8.3 versions
   * Moving to automatic handler registration

7. **Reference**

   * API documentation
   * Extension points
   * Version history

8. **Appendix**

   * Contribution guidelines
   * Common troubleshooting tips
   * Contact & community links

---

Let me know if you'd like me to flesh out any of those sections—like adding code snippets, fleshing out API docs, or drafting migration guidelines. I’d be glad to dive deeper!

[1]: https://github.com/CQRS-Muflone/Muflone?utm_source=chatgpt.com "CQRS-Muflone/Muflone: A CQRS and event store library - GitHub"
[2]: https://www.nuget.org/packages/Muflone?utm_source=chatgpt.com "Muflone 8.5.0 - NuGet"
[3]: https://github.com/CQRS-Muflone/Muflone "GitHub - CQRS-Muflone/Muflone: A CQRS and event store library"
[4]: https://github.com/CQRS-Muflone?utm_source=chatgpt.com "CQRS-Muflone - GitHub"
[5]: https://nugetmusthaves.com/Package/Muflone?utm_source=chatgpt.com "Muflone - NuGet Must Haves Package"
[6]: https://twitter.com/aacerbis/status/1687776306635620352?utm_source=chatgpt.com "Alberto Acerbis on X: \"Are you looking for a library to manage the ..."
[7]: https://x.com/collaalessandro?utm_source=chatgpt.com "Alessandro Colla (@collaalessandro) / X"
