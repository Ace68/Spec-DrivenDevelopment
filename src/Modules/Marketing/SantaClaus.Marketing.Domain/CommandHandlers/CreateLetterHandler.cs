using Muflone.Persistence;
using SantaClaus.Marketing.Domain.Entities;
using SantaClaus.Marketing.SharedKernel.Commands;

namespace SantaClaus.Marketing.Domain.CommandHandlers;

public class CreateLetterHandler
{
    private readonly IRepository _repository;

    public CreateLetterHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(CreateLetter command, CancellationToken cancellationToken = default)
    {
        var letterId = Guid.Parse(command.AggregateId.Value);
        
        var letter = Letter.Create(
            letterId,
            command.ChildId,
            command.Content,
            command.ReceivedDate,
            command.Language
        );

        await _repository.SaveAsync(letter, letterId, cancellationToken);
    }
}
