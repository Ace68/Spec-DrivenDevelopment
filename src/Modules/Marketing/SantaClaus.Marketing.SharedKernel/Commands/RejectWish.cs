using SantaClaus.Shared.Commands;

namespace SantaClaus.Marketing.SharedKernel.Commands;

public sealed record RejectWish : Command
{
    public string Reason { get; init; } = string.Empty;
}
