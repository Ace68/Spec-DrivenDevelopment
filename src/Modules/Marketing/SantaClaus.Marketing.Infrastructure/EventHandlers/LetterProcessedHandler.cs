using SantaClaus.Marketing.Infrastructure.ReadModels;
using SantaClaus.Marketing.ReadModel.DTOs;
using SantaClaus.Marketing.SharedKernel.Events;

namespace SantaClaus.Marketing.Infrastructure.EventHandlers;

/// <summary>
/// Projects LetterProcessed domain events to the Letter read model.
/// </summary>
public sealed class LetterProcessedHandler(IReadModelStore readModelStore)
{
    public async Task HandleAsync(LetterProcessed @event, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@event);

        var existingLetter = readModelStore.GetLetterById(Guid.Parse(@event.AggregateId.Value));
        if (existingLetter is null)
        {
            return;
        }

        var updatedLetter = new LetterDto(
            LetterId: existingLetter.LetterId,
            ChildId: existingLetter.ChildId,
            Content: existingLetter.Content,
            ReceivedDate: existingLetter.ReceivedDate,
            Language: existingLetter.Language,
            Status: "Processed",
            ProcessedAt: DateTime.UtcNow);

        readModelStore.UpsertLetter(updatedLetter);
        await Task.CompletedTask;
    }
}
