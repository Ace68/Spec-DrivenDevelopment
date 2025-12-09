using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace SantaClaus.Production.Facade;

public static class ProductionEndpoints
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/v1/production")
            .WithTags("Production");

        group.MapGet("/", () => Results.Ok(new { module = "Production", status = "Ready" }))
            .WithName("GetProductionStatus")
            .Produces<object>(StatusCodes.Status200OK);
    }
}
