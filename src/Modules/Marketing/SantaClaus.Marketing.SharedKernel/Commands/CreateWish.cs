using Muflone.Core;
using SantaClaus.Shared.Commands;

namespace SantaClaus.Marketing.SharedKernel.Commands;

public sealed record CreateWish : Command
{
    public required Guid ChildId { get; init; }
    public required string ToyDescription { get; init; }
    public required int Priority { get; init; }
}
