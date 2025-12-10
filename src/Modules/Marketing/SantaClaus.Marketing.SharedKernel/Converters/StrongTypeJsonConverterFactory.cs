using System.Text.Json;
using System.Text.Json.Serialization;
using SantaClaus.Marketing.SharedKernel.CustomTypes;

namespace SantaClaus.Marketing.SharedKernel.Converters;

/// <summary>
/// Factory for creating JSON converters for strong types that can be deserialized from primitives.
/// </summary>
public sealed class StrongTypeJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(LetterId) ||
               typeToConvert == typeof(ChildId) ||
               typeToConvert == typeof(LetterContent) ||
               typeToConvert == typeof(LetterLanguage);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        if (typeToConvert == typeof(LetterId))
            return new StrongTypeConverter<LetterId, string>(
                (value) => new LetterId(value),
                (id) => id.Value);

        if (typeToConvert == typeof(ChildId))
            return new StrongTypeConverter<ChildId, string>(
                (value) => new ChildId(value),
                (id) => id.Value);

        if (typeToConvert == typeof(LetterContent))
            return new StrongTypeConverter<LetterContent, string>(
                (value) => new LetterContent(value),
                (content) => content.Value);

        if (typeToConvert == typeof(LetterLanguage))
            return new StrongTypeConverter<LetterLanguage, string>(
                (value) => new LetterLanguage(value),
                (lang) => lang.Value);

        throw new NotSupportedException($"Type {typeToConvert.Name} is not supported");
    }

    /// <summary>
    /// Generic converter for strong types that wrap a primitive value.
    /// </summary>
    private sealed class StrongTypeConverter<TStrong, TPrimitive> : JsonConverter<TStrong>
        where TStrong : notnull
    {
        private readonly Func<TPrimitive, TStrong> _factory;
        private readonly Func<TStrong, TPrimitive> _extractor;

        public StrongTypeConverter(Func<TPrimitive, TStrong> factory, Func<TStrong, TPrimitive> extractor)
        {
            _factory = factory;
            _extractor = extractor;
        }

        public override TStrong Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = JsonSerializer.Deserialize<TPrimitive>(ref reader, options);
            if (value == null)
                throw new JsonException($"Failed to deserialize {typeToConvert.Name}");
            return _factory(value);
        }

        public override void Write(Utf8JsonWriter writer, TStrong value, JsonSerializerOptions options)
        {
            var primitive = _extractor(value);
            JsonSerializer.Serialize(writer, primitive, options);
        }
    }
}
