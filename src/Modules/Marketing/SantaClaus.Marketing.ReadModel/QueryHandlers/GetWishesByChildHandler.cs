using SantaClaus.Marketing.ReadModel.DTOs;
using SantaClaus.Marketing.ReadModel.Queries;
using SantaClaus.Marketing.ReadModel;

namespace SantaClaus.Marketing.ReadModel.QueryHandlers;

/// <summary>
/// Handles GetWishesByChild queries to retrieve all wishes for a specific child.
/// Supports pagination.
/// </summary>
public sealed class GetWishesByChildHandler(IReadModelStore readModelStore)
{
    public async Task<IEnumerable<WishItemDto>> HandleAsync(GetWishesByChild query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var wishes = readModelStore.GetWishesByChild(query.ChildId)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        return await Task.FromResult(wishes);
    }
}
