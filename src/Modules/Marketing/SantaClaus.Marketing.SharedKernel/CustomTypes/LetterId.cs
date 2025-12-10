using Muflone.Core;

namespace SantaClaus.Marketing.SharedKernel.CustomTypes;

/// <summary>
/// Strong type for Letter aggregate identity.
/// </summary>
public sealed class LetterId : DomainId
{
    public LetterId(Guid value) : base(value.ToString())
    {
    }

    public LetterId(string value) : base(value)
    {
    }

    public static implicit operator Guid(LetterId? letterId) => letterId != null ? Guid.Parse(letterId.Value) : Guid.Empty;
    public static implicit operator LetterId(Guid value) => new(value);
    public static implicit operator string(LetterId? letterId) => letterId?.Value ?? string.Empty;
}
