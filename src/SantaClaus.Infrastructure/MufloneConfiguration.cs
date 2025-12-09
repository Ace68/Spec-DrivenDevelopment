using Microsoft.Extensions.DependencyInjection;

namespace SantaClaus.Infrastructure;

/// <summary>
/// Configuration for Muflone CQRS+ES infrastructure.
/// </summary>
public static class MufloneConfiguration
{
    /// <summary>
    /// Registers Muflone infrastructure services with the DI container.
    /// Configures in-memory event store and transport for development.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddMufloneInfrastructure(this IServiceCollection services)
    {
        // Placeholder for Muflone infrastructure configuration
        // Will be implemented when integrating with actual domain handlers
        
        return services;
    }
}
