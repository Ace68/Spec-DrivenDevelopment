using SantaClaus.Marketing.SharedKernel.CustomTypes;
using SantaClaus.Shared.Commands;

namespace SantaClaus.Marketing.SharedKernel.Commands;

public sealed record CreateLetter : Command
{
    public Guid ChildId { get; init; }
    public LetterContent Content { get; init; } = new(string.Empty);
    public DateTime ReceivedDate { get; init; }
    public LetterLanguage Language { get; init; } = new(string.Empty);
}
