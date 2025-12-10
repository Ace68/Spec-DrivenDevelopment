namespace SantaClaus.Marketing.SharedKernel.CustomTypes;

/// <summary>
/// Strong type for letter language code (e.g., "en", "it", "de").
/// </summary>
public sealed record LetterLanguage(string Value)
{
    public static implicit operator string(LetterLanguage? language) => language?.Value ?? string.Empty;
    public static implicit operator LetterLanguage(string value) => new(value);
}
