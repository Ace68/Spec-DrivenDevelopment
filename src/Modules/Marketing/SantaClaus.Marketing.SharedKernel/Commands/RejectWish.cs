using SantaClaus.Shared.Commands;
using SantaClaus.Marketing.SharedKernel.CustomTypes;

namespace SantaClaus.Marketing.SharedKernel.Commands;

public sealed record RejectWish : Command
{
    public RejectionReason Reason { get; init; } = new(string.Empty);
}
