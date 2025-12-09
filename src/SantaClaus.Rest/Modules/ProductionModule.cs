using SantaClaus.Production.Facade;
using SantaClaus.Rest.Infrastructure;

namespace SantaClaus.Rest.Modules;

public class ProductionModule : IModule
{
    public void RegisterServices(IServiceCollection services)
    {
        ProductionFacadeHelper.AddServices(services);
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        ProductionFacadeHelper.MapEndpoints(endpoints);
    }
}
