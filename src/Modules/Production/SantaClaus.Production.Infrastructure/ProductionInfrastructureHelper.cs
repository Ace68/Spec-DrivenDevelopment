using Microsoft.Extensions.DependencyInjection;

namespace SantaClaus.Production.Infrastructure;

public static class ProductionInfrastructureHelper
{
    public static IServiceCollection AddRepositories(IServiceCollection services)
    {
        return services;
    }
}
