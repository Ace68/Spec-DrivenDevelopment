using SantaClaus.Marketing.ReadModel;
using SantaClaus.Marketing.ReadModel.DTOs;
using SantaClaus.Marketing.SharedKernel.Events;

namespace SantaClaus.Marketing.Infrastructure.EventHandlers;

/// <summary>
/// Projects ChildRegistered domain events to the Child read model.
/// </summary>
public sealed class ChildRegisteredHandler(IReadModelStore readModelStore)
{
    public async Task HandleAsync(ChildRegistered @event, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@event);

        var coordinatesDto = new CoordinatesDto(
            Latitude: (decimal)@event.Latitude,
            Longitude: (decimal)@event.Longitude);

        var addressDto = new AddressDetailDto(
            Street: @event.Street,
            City: @event.City,
            PostalCode: @event.PostalCode,
            Country: @event.Country,
            Coordinates: coordinatesDto);

        var childDto = new ChildDto(
            ChildId: Guid.Parse(@event.AggregateId.Value),
            FirstName: @event.FirstName,
            LastName: @event.LastName,
            DateOfBirth: @event.DateOfBirth,
            Address: addressDto,
            BehaviorScore: 100,
            LetterCount: 0,
            RegisteredAt: DateTime.UtcNow);

        readModelStore.UpsertChild(childDto);
        await Task.CompletedTask;
    }
}
