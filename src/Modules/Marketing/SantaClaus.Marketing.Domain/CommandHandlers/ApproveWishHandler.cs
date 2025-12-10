using Muflone.Core;
using Muflone.Persistence;
using SantaClaus.Marketing.Domain.Entities;
using SantaClaus.Marketing.SharedKernel.Commands;
using SantaClaus.Shared.Exceptions;

namespace SantaClaus.Marketing.Domain.CommandHandlers;

public sealed class ApproveWishHandler
{
    private readonly IRepository _repository;

    public ApproveWishHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(ApproveWish command, CancellationToken cancellationToken = default)
    {
        var wish = await _repository.GetByIdAsync<Wish>(command.AggregateId, cancellationToken).ConfigureAwait(false);
        if (wish is null)
            throw new NotFoundException(nameof(Wish), command.AggregateId.Value);

        wish.Approve();
        await _repository.SaveAsync(wish, command.AggregateId, cancellationToken).ConfigureAwait(false);
    }
}
