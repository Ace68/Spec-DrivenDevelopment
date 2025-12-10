using SantaClaus.Marketing.ReadModel;
using SantaClaus.Marketing.ReadModel.DTOs;
using SantaClaus.Marketing.SharedKernel.Events;

namespace SantaClaus.Marketing.Infrastructure.EventHandlers;

/// <summary>
/// Projects LetterCreated domain events to the Letter read model.
/// </summary>
public sealed class LetterCreatedHandler(IReadModelStore readModelStore)
{
    public async Task HandleAsync(LetterCreated @event, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@event);

        var letterDto = new LetterDto(
            LetterId: @event.AggregateId.Value,
            ChildId: @event.ChildId.Value,
            Content: @event.Content.Value,
            ReceivedDate: DateTime.UtcNow,
            Language: @event.Language.Value,
            Status: "Pending",
            ProcessedAt: null);

        readModelStore.UpsertLetter(letterDto);
        await Task.CompletedTask;
    }
}
