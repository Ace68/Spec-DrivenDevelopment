using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace SantaClaus.Marketing.Facade;

public static class MarketingEndpoints
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/v1/marketing")
            .WithTags("Marketing");

        group.MapGet("/", () => Results.Ok(new { module = "Marketing", status = "Ready" }))
            .WithName("GetMarketingStatus")
            .Produces<object>(StatusCodes.Status200OK);
    }
}
