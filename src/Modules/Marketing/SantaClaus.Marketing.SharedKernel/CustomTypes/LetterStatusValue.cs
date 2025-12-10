namespace SantaClaus.Marketing.SharedKernel.CustomTypes;

/// <summary>
/// Strong type for letter processing status.
/// </summary>
public sealed record LetterStatusValue(string Value)
{
    public static implicit operator string(LetterStatusValue? status) => status?.Value ?? string.Empty;
    public static implicit operator LetterStatusValue(string value) => new(value);
}
