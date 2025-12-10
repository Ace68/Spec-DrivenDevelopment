using Muflone.Core;
using Muflone.Messages.Commands;
using SantaClaus.Marketing.SharedKernel.CustomTypes;

namespace SantaClaus.Marketing.SharedKernel.Commands;

public sealed class CreateWish(IDomainId aggregateId) : Command(aggregateId)
{
    public Guid ChildId { get; set; }
    public ToyDescription ToyDescription { get; set; } = new(string.Empty);
    public int Priority { get; set; }
}
