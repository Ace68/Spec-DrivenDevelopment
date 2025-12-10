using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;
using SantaClaus.Marketing.Domain;
using SantaClaus.Marketing.Infrastructure.ReadModels;
using SantaClaus.Marketing.ReadModel.QueryHandlers;
using IReadModelStoreReadModel = SantaClaus.Marketing.ReadModel.IReadModelStore;

namespace SantaClaus.Marketing.Facade;

public static class MarketingFacadeHelper
{
    public static IServiceCollection AddServices(IServiceCollection services)
    {
        var readModelStore = new InMemoryReadModelStore();
        services.AddSingleton<IReadModelStore>(readModelStore);
        services.AddSingleton<IReadModelStoreReadModel>(readModelStore);
        services.AddScoped<IMarketingFacade, MarketingFacade>();
        
        // Register CommandHandlers
        services.AddMarketingDomain();
        
        // Register query handlers
        services.AddScoped<GetLetterByIdHandler>();
        services.AddScoped<GetLettersByChildHandler>();
        services.AddScoped<GetChildByIdHandler>();
        services.AddScoped<GetWishesByChildHandler>();
        
        return services;
    }

    public static IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        MarketingEndpoints.MapEndpoints(endpoints);
        return endpoints;
    }
}
