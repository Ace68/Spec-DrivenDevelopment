using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;

namespace SantaClaus.Delivery.Facade;

public static class DeliveryFacadeHelper
{
    public static IServiceCollection AddServices(IServiceCollection services)
    {
        services.AddScoped<IDeliveryFacade, DeliveryFacade>();
        return services;
    }

    public static IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        DeliveryEndpoints.MapEndpoints(endpoints);
        return endpoints;
    }
}
