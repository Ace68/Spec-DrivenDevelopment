namespace SantaClaus.Marketing.SharedKernel.CustomTypes;

public sealed record WishPriority(int Value)
{
    public static implicit operator WishPriority(int value) => new(value);
    public static implicit operator int(WishPriority priority) => priority.Value;
}
