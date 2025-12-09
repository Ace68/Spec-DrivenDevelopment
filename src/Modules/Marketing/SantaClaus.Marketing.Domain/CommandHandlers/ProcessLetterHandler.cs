using Muflone.Persistence;
using SantaClaus.Marketing.Domain.Entities;
using SantaClaus.Marketing.SharedKernel.Commands;
using SantaClaus.Shared.Exceptions;

namespace SantaClaus.Marketing.Domain.CommandHandlers;

public class ProcessLetterHandler
{
    private readonly IRepository _repository;

    public ProcessLetterHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(ProcessLetter command, CancellationToken cancellationToken = default)
    {
        var letterId = Guid.Parse(command.AggregateId.Value);
        
        var letter = await _repository.GetByIdAsync<Letter>(command.AggregateId, cancellationToken);
        
        if (letter == null)
            throw new NotFoundException(nameof(Letter), letterId);

        letter.MarkAsProcessed();

        await _repository.SaveAsync(letter, letterId, cancellationToken);
    }
}
