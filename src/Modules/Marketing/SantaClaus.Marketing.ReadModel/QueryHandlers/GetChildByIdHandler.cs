using SantaClaus.Marketing.ReadModel.DTOs;
using SantaClaus.Marketing.ReadModel.Queries;
using SantaClaus.Marketing.ReadModel;

namespace SantaClaus.Marketing.ReadModel.QueryHandlers;

/// <summary>
/// Handles GetChildById queries to retrieve a single child from the read model.
/// </summary>
public sealed class GetChildByIdHandler(IReadModelStore readModelStore)
{
    public async Task<ChildDto?> HandleAsync(GetChildById query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var child = readModelStore.GetChildById(query.ChildId);
        return await Task.FromResult(child);
    }
}
