using Muflone.Core;
using Muflone.Messages.Commands;

namespace SantaClaus.Marketing.SharedKernel.Commands;

public sealed class RejectWish : Command
{
    public RejectWish(IDomainId aggregateId) : base(aggregateId)
    {
    }

    public string Reason { get; set; } = string.Empty;
}
