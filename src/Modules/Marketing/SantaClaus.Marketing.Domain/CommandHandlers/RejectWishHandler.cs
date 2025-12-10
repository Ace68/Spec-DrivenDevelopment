using Muflone.Persistence;
using SantaClaus.Marketing.Domain.Entities;
using SantaClaus.Marketing.SharedKernel.Commands;
using SantaClaus.Shared.Exceptions;

namespace SantaClaus.Marketing.Domain.CommandHandlers;

public sealed class RejectWishHandler
{
    private readonly IRepository _repository;

    public RejectWishHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(RejectWish command, CancellationToken cancellationToken = default)
    {
        var wish = await _repository.GetByIdAsync<Wish>(command.AggregateId, cancellationToken).ConfigureAwait(false);
        if (wish is null)
            throw new NotFoundException(nameof(Wish), command.AggregateId.Value);

        wish.Reject(command.Reason);
        await _repository.SaveAsync(wish, command.AggregateId, cancellationToken).ConfigureAwait(false);
    }
}
