using Muflone.Core;

namespace SantaClaus.Marketing.SharedKernel.CustomTypes;

public sealed class ChildId : DomainId
{
    public ChildId(Guid value) : base(value.ToString())
    {
    }

    public ChildId(string value) : base(value)
    {
    }
}
