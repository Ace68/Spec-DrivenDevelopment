using Muflone.Core;
using Muflone.Messages.Commands;
using SantaClaus.Marketing.SharedKernel.CustomTypes;

namespace SantaClaus.Marketing.SharedKernel.Commands;

public sealed class CreateWish(IDomainId aggregateId,
    ChildId childId,
    ToyDescription toyDescription,
    int priority) : Command(aggregateId)
{
    public ChildId ChildId { get; private set; } = childId;
    public ToyDescription ToyDescription { get; private set; } = toyDescription;
    public int Priority { get; private set; } = priority;
}
