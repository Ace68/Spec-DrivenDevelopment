# Prompt: Scaffolding Minimal API .NET 9 con Domain-Driven Design

## Obiettivo
Generare la struttura completa di una Minimal API .NET 10 che rispetti i principi del Domain-Driven Design, con moduli completamente isolati e organizzati secondo un'architettura a layer ben definita.

---

## Parametri da Richiedere all'Utente

Prima di iniziare, chiedi all'utente:

1. **Nome del Progetto** (es. `SpiedoBresciano`, `GestioneRistorante`, etc.)
   - Questo sarà il prefisso di tutti i progetti: `{NomeProgetto}.Rest`, `{NomeProgetto}.{Modulo}.Domain`, etc.

2. **Elenco dei Moduli/Bounded Context** (es. `Macelleria, Trattoria` oppure `Ordini, Catalogo, Spedizioni`)
   - Ogni modulo rappresenta un Bounded Context isolato
   - Ogni modulo avrà la propria cartella fisica nel filesystem
   - Minimo 2 moduli, massimo consigliato 5 per la fase iniziale

3. **Porta HTTP** (opzionale, default: 5000 per Development)

---

## Prerequisiti e Riferimenti

1. **Consulta i seguenti file per comprendere i principi da applicare:**
   - `domain-driven-design.prompt.md`: principi DDD da rispettare
   - `glossario.prompt.md`: definizioni dei termini chiave
   - `improvements.prompt.md`: linee guida architetturali e matrice dipendenze

2. **Framework e Versioni:**
   - .NET 10 SDK
   - Minimal API pattern
   - OpenTelemetry per telemetria
   - Serilog per logging
   - Swagger/Scalar per documentazione OpenAPI

---

## Struttura Solution da Generare

### Solution Folders (Logici - non cartelle fisiche)

#### 90 Presentation
- **{NomeProgetto}.Rest**
  - Unico punto di ingresso API
  - Program.cs configurato con WebApplicationBuilder
  - Cartella `Modules/` contenente i file `{NomeModulo}Module.cs` per ogni modulo
  - Cartella `Infrastructure/` contenente:
    - `OpenApiModule.cs`: configurazione Swagger/OpenAPI
    - `OpenTelemetryModule.cs`: configurazione telemetria
    - `IModule.cs`: interfaccia base per i moduli
    - `ModuleExtensions.cs`: extension methods per registrare moduli

#### 80 Infrastructure
- **{NomeProgetto}.Infrastructure**
  - `InfrastructureHelper.cs`: metodi di registrazione dipendenze comuni
  - `EventStoreSettings.cs`: configurazione per event store (stub iniziale)
  - Non deve contenere logiche specifiche dei moduli

#### 50 Modules
Per ogni modulo specificato dall'utente, creare un **solution folder** con i seguenti progetti:

##### {NomeModulo} (solution folder)
- **{NomeProgetto}.{NomeModulo}.Facade**
  - Interfaccia `I{NomeModulo}Facade`
  - Classe `{NomeModulo}FacadeHelper` con metodi:
    - `AddServices(IServiceCollection services)`: registra dipendenze del modulo
    - `MapEndpoints(IEndpointRouteBuilder app)`: registra endpoint del modulo
  - Classe `{NomeModulo}Endpoints` con metodi statici per definire gli endpoint (inizialmente vuoti o con placeholder)
  - Dipendenze: Domain, SharedKernel, ReadModel dello stesso modulo

- **{NomeProgetto}.{NomeModulo}.Domain**
  - Cartella `Entities/` (inizialmente vuota)
  - Cartella `ValueObjects/` (inizialmente vuota)
  - Cartella `DomainEvents/` (inizialmente vuota)
  - Cartella `Repositories/` con solo interfacce (inizialmente vuote)
  - Cartella `Services/` per domain services (inizialmente vuota)
  - **NON aggiungere** CommandHandler o EventHandler astratti in questa fase

- **{NomeProgetto}.{NomeModulo}.SharedKernel**
  - Tipi custom, enumerazioni, interfacce condivise nel modulo
  - Inizialmente può contenere solo un file README.md che spiega lo scopo

- **{NomeProgetto}.{NomeModulo}.ReadModel**
  - Cartella `DTOs/` per data transfer objects (inizialmente vuota)
  - Cartella `Queries/` per query handlers (inizialmente vuota)
  - **NON aggiungere** classi handler astratte in questa fase

- **{NomeProgetto}.{NomeModulo}.Infrastructure**
  - File `{NomeModulo}InfrastructureHelper.cs` con metodo per registrare repository
  - Cartella `Repositories/` per implementazioni (inizialmente vuota)
  - Dipendenze verso Domain, SharedKernel, ReadModel dello stesso modulo

#### 30 Shared
- **{NomeProgetto}.Shared**
  - Tipi, interfacce, utilities condivise tra TUTTI i moduli
  - Esempi: `Result<T>`, `DomainException`, extension methods comuni
  - Inizialmente può contenere classi base per gestione errori

---

## Implementazione del Progetto Rest

### Program.cs
```csharp
using {NomeProgetto}.Rest.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
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

// Add OpenTelemetry module (if enabled)
builder.Services.AddOpenTelemetryModule(builder.Configuration);

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Map modules endpoints
app.MapModules();

// Map OpenAPI
app.MapOpenApiModule();

app.Run();
```

### IModule.cs
```csharp
namespace {NomeProgetto}.Rest.Infrastructure;

public interface IModule
{
    void RegisterServices(IServiceCollection services, IConfiguration configuration);
    void MapEndpoints(IEndpointRouteBuilder app);
}
```

### ModuleExtensions.cs
```csharp
namespace {NomeProgetto}.Rest.Infrastructure;

public static class ModuleExtensions
{
    private static readonly List<IModule> _modules = new();

    public static IServiceCollection AddModules(this IServiceCollection services, IConfiguration configuration)
    {
        // Register all modules
        var modules = GetModules();
        
        foreach (var module in modules)
        {
            module.RegisterServices(services, configuration);
            _modules.Add(module);
        }

        return services;
    }

    public static IEndpointRouteBuilder MapModules(this IEndpointRouteBuilder app)
    {
        foreach (var module in _modules)
        {
            module.MapEndpoints(app);
        }

        return app;
    }

    private static IEnumerable<IModule> GetModules()
    {
        // Return instances of all module classes
        // Per ogni modulo: yield return new {NomeModulo}Module();
        yield break; // Placeholder
    }
}
```

### {NomeModulo}Module.cs (uno per ogni modulo)
```csharp
using {NomeProgetto}.{NomeModulo}.Facade;
using {NomeProgetto}.Rest.Infrastructure;

namespace {NomeProgetto}.Rest.Modules;

public class {NomeModulo}Module : IModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        {NomeModulo}FacadeHelper.AddServices(services);
    }

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        {NomeModulo}FacadeHelper.MapEndpoints(app);
    }
}
```

### OpenApiModule.cs
```csharp
using Microsoft.OpenApi.Models;

namespace {NomeProgetto}.Rest.Infrastructure;

public static class OpenApiModule
{
    public static IServiceCollection AddOpenApiModule(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "{NomeProgetto} API",
                Version = "v1",
                Description = "Minimal API with Domain-Driven Design"
            });
        });

        return services;
    }

    public static IEndpointRouteBuilder MapOpenApiModule(this IEndpointRouteBuilder app)
    {
        app.MapSwagger();
        
        if (app is WebApplication webApp && webApp.Environment.IsDevelopment())
        {
            webApp.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "{NomeProgetto} API V1");
            });
        }

        return app;
    }
}
```

### OpenTelemetryModule.cs
```csharp
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace {NomeProgetto}.Rest.Infrastructure;

public static class OpenTelemetryModule
{
    public static IServiceCollection AddOpenTelemetryModule(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var isEnabled = configuration.GetValue<bool>("OpenTelemetry:IsEnabled");
        
        if (!isEnabled)
            return services;

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(
                    serviceName: "{NomeProgetto}Api",
                    serviceVersion: typeof(Program).Assembly.GetName().Version?.ToString() ?? "1.0.0"))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation());

        return services;
    }
}
```

---

## Implementazione Progetti Facade

### I{NomeModulo}Facade.cs (interfaccia)
```csharp
namespace {NomeProgetto}.{NomeModulo}.Facade;

public interface I{NomeModulo}Facade
{
    // Metodi pubblici esposti dal modulo verso l'esterno
    // Inizialmente vuota
}
```

### {NomeModulo}FacadeHelper.cs
```csharp
using Microsoft.Extensions.DependencyInjection;

namespace {NomeProgetto}.{NomeModulo}.Facade;

public static class {NomeModulo}FacadeHelper
{
    public static IServiceCollection AddServices(IServiceCollection services)
    {
        // Registra le dipendenze del modulo
        services.AddScoped<I{NomeModulo}Facade, {NomeModulo}Facade>();
        
        // Chiama helper di Infrastructure per registrare repository
        // {NomeModulo}InfrastructureHelper.AddRepositories(services);
        
        return services;
    }

    public static IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder app)
    {
        {NomeModulo}Endpoints.MapEndpoints(app);
        return app;
    }
}
```

### {NomeModulo}Endpoints.cs
```csharp
namespace {NomeProgetto}.{NomeModulo}.Facade;

public static class {NomeModulo}Endpoints
{
    public static IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/v1/{nomeModuloLowercase}")
            .WithTags("{NomeModulo}")
            .WithOpenApi();

        // Endpoint placeholder
        group.MapGet("/", () => Results.Ok(new { module = "{NomeModulo}", status = "active" }))
            .WithName("Get{NomeModulo}Status")
            .Produces<object>(StatusCodes.Status200OK);

        return app;
    }
}
```

---

## Configurazione appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "OpenTelemetry": {
    "IsEnabled": false
  },
  "ConnectionStrings": {
    "DefaultConnection": ""
  }
}
```

---

## File .csproj per Rest Project

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Serilog.AspNetCore" Version="8.*" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.*" />
    <PackageReference Include="OpenTelemetry.Exporter.Console" Version="1.*" />
    <PackageReference Include="OpenTelemetry.Extensions.Hosting" Version="1.*" />
    <PackageReference Include="OpenTelemetry.Instrumentation.AspNetCore" Version="1.*" />
    <PackageReference Include="OpenTelemetry.Instrumentation.Http" Version="1.*" />
  </ItemGroup>

  <ItemGroup>
    <!-- Riferimenti ai progetti Facade di ogni modulo -->
  </ItemGroup>

</Project>
```

---

## File .csproj per altri progetti

### Domain, ReadModel, SharedKernel
```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  </PropertyGroup>

</Project>
```

### Infrastructure
```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="9.*" />
  </ItemGroup>

</Project>
```

### Facade
```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="9.*" />
  </ItemGroup>

</Project>
```

---

## Dipendenze tra Progetti

### {NomeProgetto}.Rest
- Riferimenti: tutti i progetti `{NomeProgetto}.{NomeModulo}.Facade`

### {NomeProgetto}.{NomeModulo}.Facade
- Riferimenti: `{NomeProgetto}.{NomeModulo}.Domain`, `{NomeProgetto}.{NomeModulo}.ReadModel`, `{NomeProgetto}.{NomeModulo}.SharedKernel`, `{NomeProgetto}.Shared`

### {NomeProgetto}.{NomeModulo}.Domain
- Riferimenti: `{NomeProgetto}.{NomeModulo}.SharedKernel`, `{NomeProgetto}.Shared`

### {NomeProgetto}.{NomeModulo}.ReadModel
- Riferimenti: `{NomeProgetto}.{NomeModulo}.SharedKernel`, `{NomeProgetto}.Shared`

### {NomeProgetto}.{NomeModulo}.Infrastructure
- Riferimenti: `{NomeProgetto}.{NomeModulo}.Domain`, `{NomeProgetto}.{NomeModulo}.SharedKernel`, `{NomeProgetto}.Infrastructure`, `{NomeProgetto}.Shared`

---

## Regole Importanti

### NON Includere in Questa Fase
- Entità specifiche nei progetti Domain
- Repository concreti nei progetti Infrastructure
- CommandHandler o EventHandler (astratti o concreti)
- Query Handler concreti
- Configurazioni database o Entity Framework
- Implementazioni di autenticazione/autorizzazione

### Includere
- Struttura completa dei progetti e cartelle
- File di configurazione (appsettings.json)
- Implementazione base di IModule e ModuleExtensions
- OpenApiModule e OpenTelemetryModule funzionanti
- Program.cs configurato e funzionante
- Interfacce Facade per ogni modulo
- FacadeHelper e Endpoints per ogni modulo
- Almeno un endpoint placeholder per modulo (`GET /v1/{modulo}/`)

### Namespace e Naming
- Namespace radice: `{NomeProgetto}.[Modulo].[Strato]`
- Esempio: `SpiedoBresciano.Macelleria.Domain`
- Path endpoint: `/v1/{nomemodulo-lowercase}` (es. `/v1/macelleria`)
- Tag OpenAPI: usare il nome del modulo in PascalCase

### File Properties/launchSettings.json
```json
{
  "profiles": {
    "{NomeProgetto}.Rest": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "http://localhost:5000",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

---

## README.md per il Progetto

Genera un file README.md nella root della solution con:
- Nome e descrizione del progetto
- Architettura utilizzata (Minimal API, DDD, moduli isolati)
- Elenco dei moduli/bounded context
- Istruzioni per build e run locali
- URL per accedere a Swagger
- Comandi dotnet CLI essenziali

Esempio:
```markdown
# {NomeProgetto}

Minimal API .NET 9 con Domain-Driven Design

## Moduli
- {Modulo1}: Descrizione breve
- {Modulo2}: Descrizione breve

## Run Locale
```bash
cd src/{NomeProgetto}.Rest
dotnet restore
dotnet run
```

Swagger UI: http://localhost:5000/swagger
```

---

## Checklist di Completamento

Prima di considerare il lavoro completato, verifica:

- [ ] Solution creata con tutti i solution folder logici corretti
- [ ] Tutti i progetti creati secondo la struttura definita
- [ ] File `Class1.cs` di default rimossi da tutti i progetti
- [ ] Dipendenze tra progetti configurate correttamente
- [ ] Program.cs implementato e completo
- [ ] IModule, ModuleExtensions, OpenApiModule, OpenTelemetryModule implementati
- [ ] Un file {NomeModulo}Module.cs per ogni modulo in Rest/Modules/
- [ ] Interfaccia I{NomeModulo}Facade per ogni modulo
- [ ] {NomeModulo}FacadeHelper implementato per ogni modulo
- [ ] {NomeModulo}Endpoints con almeno un endpoint placeholder per ogni modulo
- [ ] appsettings.json configurato
- [ ] launchSettings.json configurato
- [ ] README.md creato con istruzioni chiare
- [ ] Solution compila senza errori e senza warning (TreatWarningsAsErrors=true)
- [ ] `dotnet run` avvia l'applicazione senza errori
- [ ] Swagger UI accessibile e mostra tutti i moduli con i loro endpoint
- [ ] Endpoint placeholder rispondono correttamente (HTTP 200)

---

## Criteri di Done

La fase di scaffolding è DONE quando:
1. La solution compila in Debug e Release senza warning
2. `dotnet run` avvia l'applicazione con successo
3. Swagger UI è accessibile su http://localhost:5000/swagger
4. Ogni modulo ha almeno un endpoint visibile su `/v1/{modulo}/`
5. Gli endpoint placeholder rispondono con HTTP 200
6. README.md contiene istruzioni complete per run locale
7. Nessun file `Class1.cs` presente nei progetti
8. Tutte le dipendenze tra progetti sono corrette secondo la matrice
9. OpenTelemetry è configurato ma disabilitato di default

---

## Note Finali

- Mantieni il codice semplice e pulito
- Usa commenti solo quando necessario per spiegare scelte architetturali
- Segui le convenzioni C# standard
- Tutti i nomi di classi, metodi, proprietà in inglese
- I commenti possono essere in inglese
- Non creare complessità prematura
- La struttura deve essere pronta per accogliere le implementazioni future dei bounded context

---

## Esempio di Output Atteso

Dopo aver eseguito questo prompt, l'utente dovrebbe avere:
- Una solution .NET 9 completamente funzionante
- Struttura modulare pronta per l'implementazione di logiche di dominio
- API REST documentata con Swagger
- Logging configurato con Serilog
- Telemetria configurabile con OpenTelemetry
- Zero dipendenze cross-modulo non autorizzate
- Base solida per procedere con l'implementazione dei bounded context

**Il progetto NON contiene ancora:**
- Logiche di business specifiche
- Entità di dominio concrete
- Repository implementati
- Database o persistence layer configurato
- Autenticazione/Autorizzazione

Questi elementi saranno aggiunti in fasi successive seguendo altri prompt specifici.
