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

    public static implicit operator ChildId(Guid guid) => new(guid);
    public static implicit operator Guid(ChildId childId) => Guid.Parse(childId.Value);
}
