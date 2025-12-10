using Muflone.Core;

namespace SantaClaus.Marketing.SharedKernel.CustomTypes;

/// <summary>
/// Strong type for Letter aggregate identity.
/// </summary>
public sealed class LetterId(string value) : DomainId(value);