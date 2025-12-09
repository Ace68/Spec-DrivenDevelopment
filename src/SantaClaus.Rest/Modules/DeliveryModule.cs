using SantaClaus.Delivery.Facade;
using SantaClaus.Rest.Infrastructure;

namespace SantaClaus.Rest.Modules;

public class DeliveryModule : IModule
{
    public void RegisterServices(IServiceCollection services)
    {
        DeliveryFacadeHelper.AddServices(services);
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        DeliveryFacadeHelper.MapEndpoints(endpoints);
    }
}
