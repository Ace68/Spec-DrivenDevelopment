using Microsoft.Extensions.DependencyInjection;
using Muflone.Persistence;

namespace SantaClaus.Infrastructure;

/// <summary>
/// Infrastructure module registration for dependency injection.
/// </summary>
public static class InfrastructureModule
{
    /// <summary>
    /// Adds infrastructure services to the DI container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Register Muflone infrastructure
        services.AddMufloneInfrastructure();
        
        return services;
    }
}
