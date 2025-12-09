using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace SantaClaus.Rest.Infrastructure;

public static class OpenTelemetryModule
{
    public static IServiceCollection AddOpenTelemetryModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var enabled = configuration.GetValue<bool>("OpenTelemetry:Enabled");
        if (!enabled)
        {
            return services;
        }

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService("SantaClaus.Rest"))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddConsoleExporter());

        return services;
    }
}
