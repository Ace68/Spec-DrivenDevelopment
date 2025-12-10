namespace SantaClaus.Marketing.SharedKernel.CustomTypes;

/// <summary>
/// Strong type for letter content.
/// </summary>
public sealed record LetterContent(string Value)
{
    public static implicit operator string(LetterContent? content) => content?.Value ?? string.Empty;
    public static implicit operator LetterContent(string value) => new(value);
}
