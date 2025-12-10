using Microsoft.Extensions.DependencyInjection;
using Muflone.Persistence;
using Muflone.Transport.InMemory;
using SantaClaus.Infrastructure.Repositories;

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
        services.AddMufloneTransportInMemory();
        services.AddSingleton<IRepository, InMemoryRepository>();
        return services;
    }
}
