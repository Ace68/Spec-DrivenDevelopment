using Muflone.Core;
using Muflone.Messages.Commands;

namespace SantaClaus.Marketing.SharedKernel.Commands;

public sealed class ProcessLetter : Command
{
    public ProcessLetter(IDomainId aggregateId) : base(aggregateId)
    {
    }
}
