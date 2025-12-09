using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace SantaClaus.Delivery.Facade;

public static class DeliveryEndpoints
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/v1/delivery")
            .WithTags("Delivery");

        group.MapGet("/", () => Results.Ok(new { module = "Delivery", status = "Ready" }))
            .WithName("GetDeliveryStatus")
            .Produces<object>(StatusCodes.Status200OK);
    }
}
