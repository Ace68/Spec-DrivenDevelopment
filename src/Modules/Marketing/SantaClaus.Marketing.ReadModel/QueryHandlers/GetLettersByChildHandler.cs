using SantaClaus.Marketing.ReadModel.DTOs;
using SantaClaus.Marketing.ReadModel.Queries;

namespace SantaClaus.Marketing.ReadModel.QueryHandlers;

/// <summary>
/// Handles GetLettersByChild queries to retrieve all letters for a specific child.
/// Supports pagination.
/// </summary>
public sealed class GetLettersByChildHandler(IReadModelStore readModelStore)
{
    public async Task<IEnumerable<LetterListItemDto>> HandleAsync(GetLettersByChild query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var letters = readModelStore.GetLettersByChild(query.ChildId)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        return await Task.FromResult(letters);
    }
}
