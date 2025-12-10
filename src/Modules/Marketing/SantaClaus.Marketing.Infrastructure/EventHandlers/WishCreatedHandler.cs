using SantaClaus.Marketing.ReadModel;
using SantaClaus.Marketing.ReadModel.DTOs;
using SantaClaus.Marketing.SharedKernel.Events;

namespace SantaClaus.Marketing.Infrastructure.EventHandlers;

/// <summary>
/// Projects WishCreated domain events to the Wish read model.
/// </summary>
public sealed class WishCreatedHandler(IReadModelStore readModelStore)
{
    public async Task HandleAsync(WishCreated @event, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@event);

        var wishDto = new WishItemDto(
            WishId: Guid.Parse(@event.AggregateId.Value),
            Description: @event.ToyDescription,
            Category: "General",
            Priority: @event.Priority,
            Status: "Pending");

        readModelStore.UpsertWish((Guid)@event.ChildId, wishDto);
        await Task.CompletedTask;
    }
}
