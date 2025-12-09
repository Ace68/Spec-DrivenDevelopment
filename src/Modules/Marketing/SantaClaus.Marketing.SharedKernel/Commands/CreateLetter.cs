using SantaClaus.Shared.Commands;

namespace SantaClaus.Marketing.SharedKernel.Commands;

public sealed record CreateLetter : Command
{
    public Guid ChildId { get; init; }
    public string Content { get; init; } = string.Empty;
    public DateTime ReceivedDate { get; init; }
    public string Language { get; init; } = string.Empty;
}
