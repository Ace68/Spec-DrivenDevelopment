using Muflone.Persistence;
using SantaClaus.Marketing.Domain.Entities;
using SantaClaus.Marketing.SharedKernel.Commands;

namespace SantaClaus.Marketing.Domain.CommandHandlers;

public sealed class CreateWishHandler
{
    private readonly IRepository _repository;

    public CreateWishHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(CreateWish command, CancellationToken cancellationToken = default)
    {
        var wish = Wish.Create(Guid.Parse(command.AggregateId.Value), command.ChildId, command.ToyDescription, command.Priority);
        await _repository.SaveAsync(wish, command.AggregateId, cancellationToken).ConfigureAwait(false);
    }
}
