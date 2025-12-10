namespace SantaClaus.Marketing.ReadModel.Queries;

/// <summary>
/// Query to retrieve all wishes for a specific child.
/// </summary>
public sealed record GetWishesByChild
{
    public required Guid ChildId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
