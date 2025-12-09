using Muflone.Core;

namespace SantaClaus.Shared.ValueObjects;

public sealed class DomainId : IDomainId
{
    public string Value { get; }

    public DomainId(Guid value)
    {
        Value = value.ToString();
    }

    public DomainId(string value)
    {
        Value = value;
    }

    public static implicit operator DomainId(Guid guid) => new(guid);
    public static implicit operator Guid(DomainId domainId) => Guid.Parse(domainId.Value);
}
