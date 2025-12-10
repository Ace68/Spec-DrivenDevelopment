using Microsoft.Extensions.Logging;
using Muflone.Persistence;
using SantaClaus.Marketing.Domain.Entities;
using SantaClaus.Marketing.SharedKernel.Commands;

namespace SantaClaus.Marketing.Domain.CommandHandlers;

public class CreateLetterHandler(IRepository repository, ILoggerFactory loggerFactory)
    : CommandHandlerBaseAsync<CreateLetter>(repository, loggerFactory)
{
    public override async Task HandleAsync(CreateLetter command, CancellationToken cancellationToken = default)
    {
        var letterId = Guid.Parse(command.AggregateId.Value);
        
        var letter = Letter.Create(
            letterId,
            command.ChildId,
            command.Content,
            command.ReceivedDate,
            command.Language
        );

        await Repository.SaveAsync(letter, Guid.NewGuid(), cancellationToken);
    }
}
