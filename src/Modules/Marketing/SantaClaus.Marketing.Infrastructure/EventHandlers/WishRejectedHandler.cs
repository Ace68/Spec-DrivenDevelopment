using SantaClaus.Marketing.ReadModel;
using SantaClaus.Marketing.ReadModel.DTOs;
using SantaClaus.Marketing.SharedKernel.Events;

namespace SantaClaus.Marketing.Infrastructure.EventHandlers;

/// <summary>
/// Projects WishRejected domain events to the Wish read model.
/// </summary>
public sealed class WishRejectedHandler(IReadModelStore readModelStore)
{
    public async Task HandleAsync(WishRejected @event, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@event);

        var existingWish = readModelStore.GetWishById(@event.AggregateId.Value);
        if (existingWish is null)
        {
            return;
        }

        var updatedWish = new WishItemDto(
            WishId: existingWish.WishId,
            Description: existingWish.Description,
            Category: existingWish.Category,
            Priority: existingWish.Priority,
            Status: "Rejected");

        readModelStore.UpsertWish(@event.AggregateId.Value, updatedWish);
        await Task.CompletedTask;
    }
}
