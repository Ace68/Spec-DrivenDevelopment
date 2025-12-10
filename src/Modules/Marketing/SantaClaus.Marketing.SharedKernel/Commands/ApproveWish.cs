using Muflone.Core;
using Muflone.Messages.Commands;

namespace SantaClaus.Marketing.SharedKernel.Commands;

public sealed class ApproveWish : Command
{
    public ApproveWish(IDomainId aggregateId) : base(aggregateId)
    {
    }
}
