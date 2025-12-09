using SantaClaus.Marketing.Facade;
using SantaClaus.Rest.Infrastructure;

namespace SantaClaus.Rest.Modules;

public class MarketingModule : IModule
{
    public void RegisterServices(IServiceCollection services)
    {
        MarketingFacadeHelper.AddServices(services);
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        MarketingFacadeHelper.MapEndpoints(endpoints);
    }
}
