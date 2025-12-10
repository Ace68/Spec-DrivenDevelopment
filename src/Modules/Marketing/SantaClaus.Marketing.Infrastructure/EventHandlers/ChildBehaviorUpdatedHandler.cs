using SantaClaus.Marketing.ReadModel;
using SantaClaus.Marketing.ReadModel.DTOs;
using SantaClaus.Marketing.SharedKernel.Events;

namespace SantaClaus.Marketing.Infrastructure.EventHandlers;

/// <summary>
/// Projects ChildBehaviorUpdated domain events to the Child read model.
/// </summary>
public sealed class ChildBehaviorUpdatedHandler(IReadModelStore readModelStore)
{
    public async Task HandleAsync(ChildBehaviorUpdated @event, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@event);

        var existingChild = readModelStore.GetChildById(@event.AggregateId.Value);
        if (existingChild is null)
        {
            return;
        }

        var updatedChild = new ChildDto(
            ChildId: existingChild.ChildId,
            FirstName: existingChild.FirstName,
            LastName: existingChild.LastName,
            DateOfBirth: existingChild.DateOfBirth,
            BehaviorScore: @event.NewBehaviorScore,
            Address: existingChild.Address,
            LetterCount: existingChild.LetterCount,
            RegisteredAt: existingChild.RegisteredAt);

        readModelStore.UpsertChild(updatedChild);
        await Task.CompletedTask;
    }
}
