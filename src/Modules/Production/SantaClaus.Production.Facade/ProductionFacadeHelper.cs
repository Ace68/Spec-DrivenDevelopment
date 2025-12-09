using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;

namespace SantaClaus.Production.Facade;

public static class ProductionFacadeHelper
{
    public static IServiceCollection AddServices(IServiceCollection services)
    {
        services.AddScoped<IProductionFacade, ProductionFacade>();
        return services;
    }

    public static IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        ProductionEndpoints.MapEndpoints(endpoints);
        return endpoints;
    }
}
