namespace SantaClaus.Rest.Infrastructure;

public static class OpenApiModule
{
    public static IServiceCollection AddOpenApiModule(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new()
            {
                Title = "Santa Claus Work Management API",
                Version = "v1",
                Description = "API for managing Santa's workshop operations including marketing, production, and delivery"
            });
        });
        return services;
    }

    public static IApplicationBuilder MapOpenApiModule(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Santa Claus API v1");
            options.RoutePrefix = "swagger";
        });
        return app;
    }
}
