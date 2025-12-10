namespace SantaClaus.Marketing.ReadModel.Queries;

/// <summary>
/// Query to retrieve all letters for a specific child.
/// </summary>
public sealed record GetLettersByChild
{
    public required string ChildId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
