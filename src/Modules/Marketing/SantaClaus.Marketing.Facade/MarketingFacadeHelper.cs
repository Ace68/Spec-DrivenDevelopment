using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;
using SantaClaus.Marketing.Infrastructure.ReadModels;

namespace SantaClaus.Marketing.Facade;

public static class MarketingFacadeHelper
{
    public static IServiceCollection AddServices(IServiceCollection services)
    {
        services.AddSingleton<IReadModelStore, InMemoryReadModelStore>();
        services.AddScoped<IMarketingFacade, MarketingFacade>();
        return services;
    }

    public static IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        MarketingEndpoints.MapEndpoints(endpoints);
        return endpoints;
    }
}
