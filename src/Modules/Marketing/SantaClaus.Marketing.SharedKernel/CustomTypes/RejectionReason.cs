namespace SantaClaus.Marketing.SharedKernel.CustomTypes;

public sealed record RejectionReason(string Value)
{
    public static implicit operator RejectionReason(string value) => new(value);
    public static implicit operator string(RejectionReason reason) => reason.Value;
}
