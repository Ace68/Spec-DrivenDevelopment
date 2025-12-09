namespace SantaClaus.Rest.Infrastructure;

public static class ModuleExtensions
{
    public static IServiceCollection AddModules(this IServiceCollection services)
    {
        var modules = GetModules();
        foreach (var module in modules)
        {
            module.RegisterServices(services);
        }
        return services;
    }

    public static IEndpointRouteBuilder MapModules(this IEndpointRouteBuilder endpoints)
    {
        var modules = GetModules();
        foreach (var module in modules)
        {
            module.MapEndpoints(endpoints);
        }
        return endpoints;
    }

    private static IEnumerable<IModule> GetModules()
    {
        return
        [
            new Modules.MarketingModule(),
            new Modules.ProductionModule(),
            new Modules.DeliveryModule()
        ];
    }
}
