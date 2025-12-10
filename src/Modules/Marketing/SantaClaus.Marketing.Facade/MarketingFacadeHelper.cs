using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;
using SantaClaus.Marketing.Infrastructure.ReadModels;
using SantaClaus.Marketing.ReadModel.QueryHandlers;

namespace SantaClaus.Marketing.Facade;

public static class MarketingFacadeHelper
{
    public static IServiceCollection AddServices(IServiceCollection services)
    {
        services.AddSingleton<IReadModelStore, InMemoryReadModelStore>();
        services.AddScoped<IMarketingFacade, MarketingFacade>();
        
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
