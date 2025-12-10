using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Muflone.Persistence;
using SantaClaus.Marketing.Infrastructure.ReadModels;
using SantaClaus.Marketing.ReadModel.DTOs;
using SantaClaus.Marketing.ReadModel.Queries;
using SantaClaus.Marketing.ReadModel.QueryHandlers;
using SantaClaus.Marketing.SharedKernel.Commands;
using SantaClaus.Marketing.SharedKernel.CustomTypes;

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

        // POST /v1/marketing/letters
        group.MapPost("/letters", CreateLetter)
            .WithName("CreateLetter")
            .WithSummary("Create a new letter")
            .Produces<LetterDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        // PUT /v1/marketing/letters/{letterId}/process
        group.MapPut("/letters/{letterId:guid}/process", ProcessLetter)
            .WithName("ProcessLetter")
            .WithSummary("Mark a letter as processed")
            .Produces<LetterDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

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

    private static async Task<IResult> CreateLetter(
        [FromBody] CreateLetterRequest request,
        [FromServices] IServiceBus serviceBus,
        [FromServices] IReadModelStore readModelStore,
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Content))
                return Results.BadRequest("Letter content is required");

            var letterId = Guid.NewGuid();
            var command = new CreateLetter(new LetterId(letterId),
                new ChildId(request.ChildId),
                new LetterContent(request.Content),
                request.ReceivedDate ?? DateTime.UtcNow,
                new LetterLanguage(request.Language ?? "EN"));

            await serviceBus.SendAsync(command, cancellationToken);

            return Results.Accepted($"/v1/marketing/letters/{letterId}", letterId);
        }
        catch (Exception ex)
        {
            return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> ProcessLetter(
        Guid letterId,
        [FromServices] IServiceBus serviceBus,
        [FromServices] IReadModelStore readModelStore,
        CancellationToken cancellationToken)
    {
        var letter = readModelStore.GetLetterById(letterId);
        if (letter is null)
            return Results.NotFound();

        if (letter.Status == "Processed")
            return Results.BadRequest("Letter is already processed");

        var command = new ProcessLetter(new LetterId(letterId));

        await serviceBus.SendAsync(command, cancellationToken);

        // Get the updated letter from read model
        var updatedLetter = readModelStore.GetLetterById(letterId);
        
        return updatedLetter is not null
            ? Results.Ok(updatedLetter)
            : Results.StatusCode(StatusCodes.Status500InternalServerError);
    }

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

    private static Task<IResult> GetChildren(
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

        return Task.FromResult(Results.Ok(new PaginatedResult<ChildListItemDto>(items, totalCount, page, pageSize)));
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

public record CreateLetterRequest(
    Guid ChildId,
    string Content,
    string? Language = null,
    DateTime? ReceivedDate = null
);

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
