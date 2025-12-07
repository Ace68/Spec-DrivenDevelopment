# Prompt: Test Architetturali con NetArchTest per Validare DDD

## Obiettivo
Creare test architetturali utilizzando NetArchTest per verificare che la struttura della Minimal API rispetti i principi del Domain-Driven Design e le regole di isolamento dei moduli.

---

## Prerequisiti

Questo prompt va eseguito **DOPO** aver completato lo scaffolding della soluzione (vedi `minimal-api-scaffolding.prompt.md`).

Prima di procedere, verifica che:
- La solution sia stata creata e compili senza errori
- Tutti i moduli siano presenti con i loro progetti
- Le dipendenze tra progetti siano state configurate

---

## Parametri da Richiedere

1. **Nome del Progetto** (es. `SpiedoBresciano`)
2. **Elenco dei Moduli** (es. `Macelleria, Trattoria`)

---

## Struttura Test da Creare

Per ogni modulo, creare un progetto di test:
- **{NomeProgetto}.{NomeModulo}.Tests**
  - Deve essere un progetto xUnit o NUnit
  - Deve referenziare il package `NetArchTest.Rules`
  - Deve avere riferimenti a TUTTI i progetti del modulo e al progetto Rest

---

## Implementazione Test Project

### File .csproj per {NomeModulo}.Tests

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.*" />
    <PackageReference Include="xunit" Version="2.*" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.*">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="NetArchTest.Rules" Version="1.*" />
  </ItemGroup>

  <ItemGroup>
    <!-- Riferimenti ai progetti del modulo -->
    <ProjectReference Include="..\{NomeProgetto}.{NomeModulo}.Domain\{NomeProgetto}.{NomeModulo}.Domain.csproj" />
    <ProjectReference Include="..\{NomeProgetto}.{NomeModulo}.Facade\{NomeProgetto}.{NomeModulo}.Facade.csproj" />
    <ProjectReference Include="..\{NomeProgetto}.{NomeModulo}.Infrastructure\{NomeProgetto}.{NomeModulo}.Infrastructure.csproj" />
    <ProjectReference Include="..\{NomeProgetto}.{NomeModulo}.ReadModel\{NomeProgetto}.{NomeModulo}.ReadModel.csproj" />
    <ProjectReference Include="..\{NomeProgetto}.{NomeModulo}.SharedKernel\{NomeProgetto}.{NomeModulo}.SharedKernel.csproj" />
    <!-- Riferimento al progetto Rest per verificare le dipendenze -->
    <ProjectReference Include="..\..\{NomeProgetto}.Rest\{NomeProgetto}.Rest.csproj" />
  </ItemGroup>

</Project>
```

---

## Test Architetturali da Implementare

### 1. ArchitectureTests.cs

Questo file conterrà tutti i test architetturali per il modulo.

```csharp
using NetArchTest.Rules;
using Xunit;

namespace {NomeProgetto}.{NomeModulo}.Tests;

public class ArchitectureTests
{
    private const string DomainNamespace = "{NomeProgetto}.{NomeModulo}.Domain";
    private const string InfrastructureNamespace = "{NomeProgetto}.{NomeModulo}.Infrastructure";
    private const string FacadeNamespace = "{NomeProgetto}.{NomeModulo}.Facade";
    private const string ReadModelNamespace = "{NomeProgetto}.{NomeModulo}.ReadModel";
    private const string SharedKernelNamespace = "{NomeProgetto}.{NomeModulo}.SharedKernel";
    private const string SharedNamespace = "{NomeProgetto}.Shared";

    [Fact]
    public void Domain_Should_Not_Depend_On_Infrastructure()
    {
        // Arrange
        var domain = Types.InAssembly(typeof({NomeProgetto}.{NomeModulo}.Domain.AssemblyMarker).Assembly);

        // Act
        var result = domain
            .ShouldNot()
            .HaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, 
            $"Domain layer should not depend on Infrastructure. Violations: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Domain_Should_Not_Depend_On_Facade()
    {
        // Arrange
        var domain = Types.InAssembly(typeof({NomeProgetto}.{NomeModulo}.Domain.AssemblyMarker).Assembly);

        // Act
        var result = domain
            .ShouldNot()
            .HaveDependencyOn(FacadeNamespace)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"Domain layer should not depend on Facade. Violations: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Domain_Should_Only_Depend_On_SharedKernel_And_Shared()
    {
        // Arrange
        var domain = Types.InAssembly(typeof({NomeProgetto}.{NomeModulo}.Domain.AssemblyMarker).Assembly);

        // Act
        var result = domain
            .Should()
            .OnlyHaveDependenciesOn(DomainNamespace, SharedKernelNamespace, SharedNamespace, "System", "Microsoft")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"Domain should only depend on its own namespace, SharedKernel, Shared and system libraries. Violations: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Infrastructure_Should_Not_Depend_On_Facade()
    {
        // Arrange
        var infrastructure = Types.InAssembly(typeof({NomeProgetto}.{NomeModulo}.Infrastructure.AssemblyMarker).Assembly);

        // Act
        var result = infrastructure
            .ShouldNot()
            .HaveDependencyOn(FacadeNamespace)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"Infrastructure should not depend on Facade. Violations: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void ReadModel_Should_Not_Depend_On_Domain()
    {
        // Arrange
        var readModel = Types.InAssembly(typeof({NomeProgetto}.{NomeModulo}.ReadModel.AssemblyMarker).Assembly);

        // Act
        var result = readModel
            .ShouldNot()
            .HaveDependencyOn(DomainNamespace)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"ReadModel should not depend on Domain (CQRS separation). Violations: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void ReadModel_Should_Not_Depend_On_Infrastructure()
    {
        // Arrange
        var readModel = Types.InAssembly(typeof({NomeProgetto}.{NomeModulo}.ReadModel.AssemblyMarker).Assembly);

        // Act
        var result = readModel
            .ShouldNot()
            .HaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"ReadModel should not depend on Infrastructure. Violations: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Facade_Should_Not_Depend_On_Infrastructure()
    {
        // Arrange
        var facade = Types.InAssembly(typeof({NomeProgetto}.{NomeModulo}.Facade.AssemblyMarker).Assembly);

        // Act
        var result = facade
            .ShouldNot()
            .HaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"Facade should not directly depend on Infrastructure. Violations: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void SharedKernel_Should_Not_Depend_On_Domain()
    {
        // Arrange
        var sharedKernel = Types.InAssembly(typeof({NomeProgetto}.{NomeModulo}.SharedKernel.AssemblyMarker).Assembly);

        // Act
        var result = sharedKernel
            .ShouldNot()
            .HaveDependencyOn(DomainNamespace)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"SharedKernel should not depend on Domain. Violations: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void SharedKernel_Should_Not_Depend_On_Infrastructure()
    {
        // Arrange
        var sharedKernel = Types.InAssembly(typeof({NomeProgetto}.{NomeModulo}.SharedKernel.AssemblyMarker).Assembly);

        // Act
        var result = sharedKernel
            .ShouldNot()
            .HaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"SharedKernel should not depend on Infrastructure. Violations: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void SharedKernel_Should_Not_Depend_On_Facade()
    {
        // Arrange
        var sharedKernel = Types.InAssembly(typeof({NomeProgetto}.{NomeModulo}.SharedKernel.AssemblyMarker).Assembly);

        // Act
        var result = sharedKernel
            .ShouldNot()
            .HaveDependencyOn(FacadeNamespace)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"SharedKernel should not depend on Facade. Violations: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void SharedKernel_Should_Not_Depend_On_ReadModel()
    {
        // Arrange
        var sharedKernel = Types.InAssembly(typeof({NomeProgetto}.{NomeModulo}.SharedKernel.AssemblyMarker).Assembly);

        // Act
        var result = sharedKernel
            .ShouldNot()
            .HaveDependencyOn(ReadModelNamespace)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"SharedKernel should not depend on ReadModel. Violations: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }
}
```

---

### 2. ModuleIsolationTests.cs

Test per verificare l'isolamento tra moduli diversi.

```csharp
using NetArchTest.Rules;
using Xunit;

namespace {NomeProgetto}.{NomeModulo}.Tests;

public class ModuleIsolationTests
{
    private const string CurrentModuleNamespace = "{NomeProgetto}.{NomeModulo}";
    
    // Lista di tutti gli altri moduli da verificare
    private static readonly string[] OtherModules = new[]
    {
        // Inserire qui i namespace degli altri moduli
        // Es: "{NomeProgetto}.AltroModulo1", "{NomeProgetto}.AltroModulo2"
    };

    [Fact]
    public void Module_Should_Not_Reference_Other_Modules()
    {
        if (OtherModules.Length == 0)
        {
            // Se non ci sono altri moduli, il test passa automaticamente
            return;
        }

        // Arrange - Raccoglie tutti i tipi del modulo corrente
        var currentModuleTypes = Types.InNamespace(CurrentModuleNamespace);

        // Act & Assert - Verifica che nessun tipo del modulo dipenda da altri moduli
        foreach (var otherModule in OtherModules)
        {
            var result = currentModuleTypes
                .ShouldNot()
                .HaveDependencyOn(otherModule)
                .GetResult();

            Assert.True(result.IsSuccessful,
                $"Module {CurrentModuleNamespace} should not depend on {otherModule}. Violations: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
        }
    }

    [Fact]
    public void Domain_Should_Not_Reference_Any_Other_Module()
    {
        if (OtherModules.Length == 0)
        {
            return;
        }

        // Arrange
        var domainTypes = Types.InNamespace($"{CurrentModuleNamespace}.Domain");

        // Act & Assert
        foreach (var otherModule in OtherModules)
        {
            var result = domainTypes
                .ShouldNot()
                .HaveDependencyOn(otherModule)
                .GetResult();

            Assert.True(result.IsSuccessful,
                $"Domain of {CurrentModuleNamespace} should not depend on {otherModule}. Violations: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
        }
    }

    [Fact]
    public void Infrastructure_Should_Not_Reference_Any_Other_Module()
    {
        if (OtherModules.Length == 0)
        {
            return;
        }

        // Arrange
        var infrastructureTypes = Types.InNamespace($"{CurrentModuleNamespace}.Infrastructure");

        // Act & Assert
        foreach (var otherModule in OtherModules)
        {
            var result = infrastructureTypes
                .ShouldNot()
                .HaveDependencyOn(otherModule)
                .GetResult();

            Assert.True(result.IsSuccessful,
                $"Infrastructure of {CurrentModuleNamespace} should not depend on {otherModule}. Violations: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
        }
    }

    [Fact]
    public void Facade_Should_Not_Reference_Any_Other_Module()
    {
        if (OtherModules.Length == 0)
        {
            return;
        }

        // Arrange
        var facadeTypes = Types.InNamespace($"{CurrentModuleNamespace}.Facade");

        // Act & Assert
        foreach (var otherModule in OtherModules)
        {
            var result = facadeTypes
                .ShouldNot()
                .HaveDependencyOn(otherModule)
                .GetResult();

            Assert.True(result.IsSuccessful,
                $"Facade of {CurrentModuleNamespace} should not depend on {otherModule}. Violations: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
        }
    }
}
```

---

### 3. RestProjectTests.cs

Test per verificare che il progetto Rest abbia solo le dipendenze autorizzate.

```csharp
using NetArchTest.Rules;
using Xunit;

namespace {NomeProgetto}.{NomeModulo}.Tests;

public class RestProjectTests
{
    private const string RestNamespace = "{NomeProgetto}.Rest";
    private const string CurrentModuleNamespace = "{NomeProgetto}.{NomeModulo}";

    [Fact]
    public void Rest_Should_Only_Depend_On_Facade_Projects()
    {
        // Arrange
        var restTypes = Types.InNamespace(RestNamespace);

        // Act - Verifica che Rest non dipenda da Domain, Infrastructure o ReadModel del modulo
        var domainResult = restTypes
            .ShouldNot()
            .HaveDependencyOn($"{CurrentModuleNamespace}.Domain")
            .GetResult();

        var infrastructureResult = restTypes
            .ShouldNot()
            .HaveDependencyOn($"{CurrentModuleNamespace}.Infrastructure")
            .GetResult();

        var readModelResult = restTypes
            .ShouldNot()
            .HaveDependencyOn($"{CurrentModuleNamespace}.ReadModel")
            .GetResult();

        // Assert
        Assert.True(domainResult.IsSuccessful,
            $"Rest should not directly depend on Domain. Violations: {string.Join(", ", domainResult.FailingTypeNames ?? Array.Empty<string>())}");

        Assert.True(infrastructureResult.IsSuccessful,
            $"Rest should not directly depend on Infrastructure. Violations: {string.Join(", ", infrastructureResult.FailingTypeNames ?? Array.Empty<string>())}");

        Assert.True(readModelResult.IsSuccessful,
            $"Rest should not directly depend on ReadModel. Violations: {string.Join(", ", readModelResult.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Rest_Can_Depend_On_Facade()
    {
        // Arrange
        var restTypes = Types.InNamespace(RestNamespace);

        // Act - Verifica che Rest possa dipendere da Facade (questo test dovrebbe passare)
        var result = restTypes
            .That()
            .HaveDependencyOn($"{CurrentModuleNamespace}.Facade")
            .GetTypes();

        // Assert - Non stiamo facendo un assert negativo, solo verificando che l'assembly possa essere caricato
        Assert.NotNull(result);
    }
}
```

---

## Assembly Marker Classes

Per permettere ai test di referenziare gli assembly, ogni progetto deve contenere una classe marker:

### AssemblyMarker.cs (da aggiungere in ogni progetto del modulo)

```csharp
namespace {NomeProgetto}.{NomeModulo}.{LayerName};

/// <summary>
/// Marker class for assembly reference in architecture tests.
/// </summary>
public class AssemblyMarker
{
}
```

Creare questa classe in:
- `{NomeProgetto}.{NomeModulo}.Domain`
- `{NomeProgetto}.{NomeModulo}.Facade`
- `{NomeProgetto}.{NomeModulo}.Infrastructure`
- `{NomeProgetto}.{NomeModulo}.ReadModel`
- `{NomeProgetto}.{NomeModulo}.SharedKernel`

---

## Configurazione degli Altri Moduli in ModuleIsolationTests

Nel file `ModuleIsolationTests.cs`, aggiorna l'array `OtherModules` con i namespace degli altri moduli presenti nella solution.

Esempio per una solution con moduli `Macelleria` e `Trattoria`:

**In {NomeProgetto}.Macelleria.Tests/ModuleIsolationTests.cs:**
```csharp
private static readonly string[] OtherModules = new[]
{
    "{NomeProgetto}.Trattoria"
};
```

**In {NomeProgetto}.Trattoria.Tests/ModuleIsolationTests.cs:**
```csharp
private static readonly string[] OtherModules = new[]
{
    "{NomeProgetto}.Macelleria"
};
```

---

## Esecuzione dei Test

Per eseguire tutti i test architetturali:

```bash
# Dalla root della solution
dotnet test

# Per eseguire solo i test di un modulo specifico
dotnet test src/{NomeModulo}/{NomeProgetto}.{NomeModulo}.Tests

# Per vedere output dettagliato
dotnet test --logger "console;verbosity=detailed"
```

---

## Interpretazione dei Risultati

### Test Passati (✓)
- L'architettura rispetta i principi DDD
- I moduli sono correttamente isolati
- Le dipendenze sono conformi alle regole definite

### Test Falliti (✗)
- Il messaggio di errore elenca i tipi che violano la regola
- Esempio: `"Violations: MyNamespace.MyClass, MyNamespace.AnotherClass"`
- Ogni violazione indica una dipendenza non autorizzata che deve essere corretta

---

## Regole Architetturali Verificate

### Separazione dei Layer (per modulo)
1. ✓ Domain non dipende da Infrastructure
2. ✓ Domain non dipende da Facade
3. ✓ Domain dipende solo da SharedKernel, Shared e System libraries
4. ✓ Infrastructure non dipende da Facade
5. ✓ ReadModel non dipende da Domain (CQRS)
6. ✓ ReadModel non dipende da Infrastructure
7. ✓ Facade non dipende direttamente da Infrastructure
8. ✓ SharedKernel non dipende da Domain, Infrastructure, Facade o ReadModel

### Isolamento dei Moduli
9. ✓ Nessun progetto di un modulo dipende da progetti di altri moduli
10. ✓ Domain di un modulo non referenzia altri moduli
11. ✓ Infrastructure di un modulo non referenzia altri moduli
12. ✓ Facade di un modulo non referenzia altri moduli

### Progetto Rest
13. ✓ Rest non dipende direttamente da Domain di nessun modulo
14. ✓ Rest non dipende direttamente da Infrastructure di nessun modulo
15. ✓ Rest non dipende direttamente da ReadModel di nessun modulo
16. ✓ Rest dipende solo dai progetti Facade dei moduli

---

## Checklist di Completamento

- [ ] Creato progetto Test per ogni modulo
- [ ] Aggiunta classe `AssemblyMarker` in ogni progetto del modulo
- [ ] Implementato `ArchitectureTests.cs` per ogni modulo
- [ ] Implementato `ModuleIsolationTests.cs` per ogni modulo
- [ ] Implementato `RestProjectTests.cs` per ogni modulo (può essere condiviso)
- [ ] Aggiornato array `OtherModules` in ogni `ModuleIsolationTests.cs`
- [ ] Package `NetArchTest.Rules` installato in tutti i progetti test
- [ ] Tutti i test compilano senza errori
- [ ] `dotnet test` eseguito con successo
- [ ] **TUTTI i test architetturali passano**

---

## Criteri di Done

La fase di test architetturali è DONE quando:
1. Tutti i progetti test sono stati creati
2. Tutti i test compilano senza errori
3. `dotnet test` viene eseguito con successo
4. **Tutti i test architetturali passano** (100% success rate)
5. Nessuna violazione delle regole DDD rilevata
6. L'isolamento dei moduli è completamente verificato

---

## Cosa Fare se i Test Falliscono

Se i test architetturali falliscono:

1. **Identifica la violazione**: leggi il messaggio di errore che elenca le classi problematiche
2. **Analizza la dipendenza**: capire perché quella dipendenza esiste
3. **Correggi l'architettura**:
   - Rimuovi il riferimento al progetto non autorizzato
   - Sposta la classe nel layer corretto
   - Usa un'interfaccia nel layer appropriato
   - Usa Dependency Injection per invertire la dipendenza
4. **Riesegui i test** fino a quando tutti passano

**NON modificare i test per farli passare** - i test rappresentano le regole architetturali che DEVONO essere rispettate.

---

## Note Finali

- Questi test sono una salvaguardia contro il degrado architetturale
- Devono essere eseguiti in CI/CD per ogni commit
- Se un test fallisce, è un segnale che l'architettura sta degenerando
- Mantenere i test aggiornati quando si aggiungono nuovi moduli
- I test architetturali sono documentazione eseguibile dell'architettura

---

## Integrazione con CI/CD

Esempio di step per Azure DevOps / GitHub Actions:

```yaml
- name: Run Architecture Tests
  run: dotnet test --filter "FullyQualifiedName~ArchitectureTests|ModuleIsolationTests|RestProjectTests" --logger "trx;LogFileName=architecture-tests.trx"
  
- name: Fail if Architecture Tests Failed
  if: failure()
  run: exit 1
```

---

## Estensioni Future

Quando la solution cresce, considera di aggiungere test per:
- Naming conventions (es. Repository deve finire con "Repository")
- Classi in Domain devono essere sealed o abstract
- Interfacce di repository solo in Domain
- Domain Events devono implementare INotification
- Command/Query handler devono seguire pattern specifici
