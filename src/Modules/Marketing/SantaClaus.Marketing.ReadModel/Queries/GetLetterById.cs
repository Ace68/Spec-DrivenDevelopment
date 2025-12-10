namespace SantaClaus.Marketing.ReadModel.Queries;

/// <summary>
/// Query to retrieve a letter by its identifier.
/// </summary>
public sealed record GetLetterById
{
    public required Guid LetterId { get; init; }
}
