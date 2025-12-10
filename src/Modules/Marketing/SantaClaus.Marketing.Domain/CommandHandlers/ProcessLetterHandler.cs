using Microsoft.Extensions.Logging;
using Muflone.Persistence;
using SantaClaus.Marketing.Domain.Entities;
using SantaClaus.Marketing.SharedKernel.Commands;
using SantaClaus.Shared.Exceptions;

namespace SantaClaus.Marketing.Domain.CommandHandlers;

public class ProcessLetterHandler(IRepository repository, ILoggerFactory loggerFactory)
    : CommandHandlerBaseAsync<ProcessLetter>(repository, loggerFactory)
{
    public override async Task HandleAsync(ProcessLetter command, CancellationToken cancellationToken = default)
    {
        var letterId = Guid.Parse(command.AggregateId.Value);
        
        var letter = await Repository.GetByIdAsync<Letter>(command.AggregateId, cancellationToken);
        
        if (letter == null)
            throw new NotFoundException(nameof(Letter), letterId);

        letter.MarkAsProcessed();

        await Repository.SaveAsync(letter, Guid.NewGuid(), cancellationToken);
    }
}
