using System.Text.Json;
using System.Text.Json.Serialization;
using Serilog;
using SantaClaus.Infrastructure;
using SantaClaus.Marketing.SharedKernel.Converters;
using SantaClaus.Rest.Infrastructure;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/santa-api-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Starting Santa Claus Work Management API");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    // Configure JSON serialization for strong types
    builder.Services.Configure<JsonSerializerOptions>(options =>
    {
        options.PropertyNameCaseInsensitive = true;
        options.Converters.Add(new StrongTypeJsonConverterFactory());
    });

    // builder.Services.AddInMemoryBroker();
    builder.Services.AddInfrastructure();
    builder.Services.AddOpenApiModule();
    builder.Services.AddOpenTelemetryModule(builder.Configuration);
    builder.Services.AddModules();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.MapOpenApiModule();
    app.MapModules();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
