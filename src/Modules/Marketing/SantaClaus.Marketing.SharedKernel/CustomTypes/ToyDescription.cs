namespace SantaClaus.Marketing.SharedKernel.CustomTypes;

public sealed record ToyDescription(string Value)
{
    public static implicit operator ToyDescription(string value) => new(value);
    public static implicit operator string(ToyDescription toyDescription) => toyDescription.Value;
}
