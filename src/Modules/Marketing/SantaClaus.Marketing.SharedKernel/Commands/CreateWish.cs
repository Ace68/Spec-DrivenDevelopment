using SantaClaus.Shared.Commands;
using SantaClaus.Marketing.SharedKernel.CustomTypes;

namespace SantaClaus.Marketing.SharedKernel.Commands;

public sealed record CreateWish : Command
{
    public required ChildId ChildId { get; init; }
    public required ToyDescription ToyDescription { get; init; }
    public required WishPriority Priority { get; init; }
}
