using Muflone.Core;

namespace SantaClaus.Marketing.SharedKernel.CustomTypes;

public sealed class ChildId(string value) : DomainId(value);
