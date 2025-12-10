using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using SantaClaus.Marketing.Infrastructure.ReadModels;
using SantaClaus.Marketing.ReadModel.DTOs;
using SantaClaus.Marketing.ReadModel.Queries;
using SantaClaus.Marketing.ReadModel.QueryHandlers;

namespace SantaClaus.Marketing.Facade;

public static class MarketingEndpoints
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/v1/marketing")
            .WithTags("Marketing");

        // ==================== LETTERS ====================

        // GET /v1/marketing/letters/{letterId}
        group.MapGet("/letters/{letterId:guid}", GetLetterByIdHandler)
            .WithName("GetLetterById")
            .WithSummary("Get a specific letter by ID")
            .Produces<LetterDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        // GET /v1/marketing/letters
        group.MapGet("/letters", GetLetters)
            .WithName("GetLetters")
            .WithSummary("List letters with optional filtering")
            .Produces<PaginatedResult<LetterListItemDto>>(StatusCodes.Status200OK);

        // ==================== CHILDREN ====================

        // GET /v1/marketing/children/{childId}
        group.MapGet("/children/{childId:guid}", GetChildById)
            .WithName("GetChildById")
            .WithSummary("Get child information by ID")
            .Produces<ChildDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        // GET /v1/marketing/children
        group.MapGet("/children", GetChildren)
            .WithName("GetChildren")
            .WithSummary("List children with optional filtering")
            .Produces<PaginatedResult<ChildListItemDto>>(StatusCodes.Status200OK);

        // ==================== WISHES ====================

        // GET /v1/marketing/wishes/{childId}
        group.MapGet("/wishes/{childId:guid}", GetWishesByChild)
            .WithName("GetWishesByChild")
            .WithSummary("Get processed wishes for a child")
            .Produces<PaginatedResult<WishItemDto>>(StatusCodes.Status200OK);
    }

    // ==================== LETTER HANDLERS ====================

    private static async Task<IResult> GetLetterByIdHandler(
        Guid letterId,
        [FromServices] GetLetterByIdHandler handler,
        CancellationToken cancellationToken)
    {
        var query = new GetLetterById { LetterId = letterId };
        var result = await handler.HandleAsync(query, cancellationToken);

        return result is not null
            ? Results.Ok(result)
            : Results.NotFound();
    }

    private static async Task<IResult> GetLetters(
        [FromQuery] Guid? childId,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromServices] GetLettersByChildHandler handler = null!,
        CancellationToken cancellationToken = default)
    {
        var query = new GetLettersByChild { ChildId = childId ?? Guid.Empty, PageNumber = page, PageSize = pageSize };
        var letters = await handler.HandleAsync(query, cancellationToken);
        var letterList = letters.ToList();

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<LetterStatus>(status, true, out var letterStatus))
        {
            letterList = letterList.Where(l => l.Status == letterStatus.ToString()).ToList();
        }

        var totalCount = letterList.Count;

        return Results.Ok(new PaginatedResult<LetterListItemDto>(letterList, totalCount, page, pageSize));
    }

    // ==================== CHILDREN HANDLERS ====================

    private static async Task<IResult> GetChildById(
        Guid childId,
        [FromServices] GetChildByIdHandler handler,
        CancellationToken cancellationToken)
    {
        var query = new GetChildById { ChildId = childId };
        var result = await handler.HandleAsync(query, cancellationToken);

        return result is not null
            ? Results.Ok(result)
            : Results.NotFound();
    }

    private static async Task<IResult> GetChildren(
        [FromQuery] string? country,
        [FromQuery] int? minBehavior,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromServices] IReadModelStore readModelStore = null!,
        CancellationToken cancellationToken = default)
    {
        // Read directly from store for now (implement handler later if needed)
        var children = readModelStore.GetChildren()
            .AsEnumerable();

        if (!string.IsNullOrEmpty(country))
            children = children.Where(c => c.Country == country);

        if (minBehavior.HasValue)
            children = children.Where(c => c.BehaviorScore >= minBehavior);

        var totalCount = children.Count();
        var items = children
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Cast<ChildListItemDto>()
            .ToList();

        return Results.Ok(new PaginatedResult<ChildListItemDto>(items, totalCount, page, pageSize));
    }

    // ==================== WISHES HANDLERS ====================

    private static async Task<IResult> GetWishesByChild(
        Guid childId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromServices] GetWishesByChildHandler handler = null!,
        CancellationToken cancellationToken = default)
    {
        var query = new GetWishesByChild { ChildId = childId, PageNumber = page, PageSize = pageSize };
        var wishes = await handler.HandleAsync(query, cancellationToken);

        var wishList = wishes?.ToList() ?? new List<WishItemDto>();
        var totalCount = wishList.Count;

        return Results.Ok(new PaginatedResult<WishItemDto>(wishList, totalCount, page, pageSize));
    }
}

// ==================== REQUEST/RESPONSE MODELS ====================

public record PaginatedResult<T>(
    List<T> Items,
    int TotalCount,
    int Page,
    int PageSize
);

// Enum for letter status
public enum LetterStatus
{
    Received,
    Processing,
    Processed
}
