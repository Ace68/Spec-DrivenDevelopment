namespace SantaClaus.Marketing.ReadModel.Queries;

/// <summary>
/// Query to retrieve a child by its identifier.
/// </summary>
public sealed record GetChildById
{
    public required Guid ChildId { get; init; }
}
