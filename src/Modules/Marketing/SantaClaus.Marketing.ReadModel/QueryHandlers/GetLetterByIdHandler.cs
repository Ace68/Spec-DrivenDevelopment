using SantaClaus.Marketing.ReadModel.DTOs;
using SantaClaus.Marketing.ReadModel.Queries;
using SantaClaus.Marketing.ReadModel;

namespace SantaClaus.Marketing.ReadModel.QueryHandlers;

/// <summary>
/// Handles GetLetterById queries to retrieve a single letter from the read model.
/// </summary>
public sealed class GetLetterByIdHandler(IReadModelStore readModelStore)
{
    public async Task<LetterDto?> HandleAsync(GetLetterById query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var letter = readModelStore.GetLetterById(query.LetterId);
        return await Task.FromResult(letter);
    }
}
