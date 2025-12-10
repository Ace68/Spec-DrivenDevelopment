using SantaClaus.Marketing.ReadModel;
using SantaClaus.Marketing.ReadModel.DTOs;
using SantaClaus.Marketing.SharedKernel.Events;

namespace SantaClaus.Marketing.Infrastructure.EventHandlers;

/// <summary>
/// Projects WishApproved domain events to the Wish read model.
/// </summary>
public sealed class WishApprovedHandler(IReadModelStore readModelStore)
{
    public async Task HandleAsync(WishApproved @event, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@event);

        var existingWish = readModelStore.GetWishById(Guid.Parse(@event.AggregateId.Value));
        if (existingWish is null)
        {
            return;
        }

        var updatedWish = new WishItemDto(
            WishId: existingWish.WishId,
            Description: existingWish.Description,
            Category: existingWish.Category,
            Priority: existingWish.Priority,
            Status: "Approved");

        readModelStore.UpsertWish(Guid.Parse(@event.AggregateId.Value).ToString(), updatedWish);
        await Task.CompletedTask;
    }
}
