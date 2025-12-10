using Muflone.Core;
using Muflone.Persistence;
using Microsoft.Extensions.Logging;
using SantaClaus.Marketing.Domain.Entities;
using SantaClaus.Marketing.SharedKernel.Commands;
using SantaClaus.Shared.Exceptions;

namespace SantaClaus.Marketing.Domain.CommandHandlers;

public sealed class ApproveWishHandler(IRepository repository, ILoggerFactory loggerFactory)
    : CommandHandlerBaseAsync<ApproveWish>(repository, loggerFactory)
{
    public override async Task HandleAsync(ApproveWish command, CancellationToken cancellationToken = new())
    {
        var wish = await Repository.GetByIdAsync<Wish>(command.AggregateId, cancellationToken).ConfigureAwait(false);
        if (wish is null)
            throw new NotFoundException(nameof(Wish), command.AggregateId.Value);

        wish.Approve();
        await Repository.SaveAsync(wish, Guid.NewGuid(), cancellationToken).ConfigureAwait(false);
    }
}
