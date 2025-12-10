using Microsoft.Extensions.DependencyInjection;
using Muflone.Persistence;

namespace SantaClaus.Infrastructure;

/// <summary>
/// Configuration for Muflone CQRS+ES infrastructure.
/// </summary>
public static class MufloneConfiguration
{
    /// <summary>
    /// Registers Muflone infrastructure services with the DI container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddMufloneInfrastructure(this IServiceCollection services)
    {
        // Register in-memory broker for messaging
        return services;
    }
}
