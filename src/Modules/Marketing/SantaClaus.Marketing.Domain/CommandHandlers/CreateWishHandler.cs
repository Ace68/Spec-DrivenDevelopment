using Microsoft.Extensions.Logging;
using Muflone.Persistence;
using SantaClaus.Marketing.Domain.Entities;
using SantaClaus.Marketing.SharedKernel.Commands;

namespace SantaClaus.Marketing.Domain.CommandHandlers;

public sealed class CreateWishHandler(IRepository repository, ILoggerFactory loggerFactory)
    : CommandHandlerBaseAsync<CreateWish>(repository, loggerFactory)
{
    public override async Task HandleAsync(CreateWish command, CancellationToken cancellationToken = new())
    {
        var wish = Wish.Create(Guid.Parse(command.AggregateId.Value), command.ChildId, command.ToyDescription, command.Priority);
        await Repository.SaveAsync(wish, Guid.NewGuid(), cancellationToken).ConfigureAwait(false);
    }
}
