using Muflone.Persistence;
using Microsoft.Extensions.Logging;
using SantaClaus.Marketing.Domain.Entities;
using SantaClaus.Marketing.SharedKernel.Commands;
using SantaClaus.Shared.Exceptions;

namespace SantaClaus.Marketing.Domain.CommandHandlers;

public sealed class RejectWishHandler(IRepository repository, ILoggerFactory loggerFactory)
    : CommandHandlerBaseAsync<RejectWish>(repository, loggerFactory)
{
    public override async Task HandleAsync(RejectWish command, CancellationToken cancellationToken = new())
    {
        var wish = await Repository.GetByIdAsync<Wish>(command.AggregateId, cancellationToken).ConfigureAwait(false);
        if (wish is null)
            throw new NotFoundException(nameof(Wish), command.AggregateId.Value);

        wish.Reject(command.Reason);
        await Repository.SaveAsync(wish, Guid.NewGuid(), cancellationToken).ConfigureAwait(false);
    }
}
