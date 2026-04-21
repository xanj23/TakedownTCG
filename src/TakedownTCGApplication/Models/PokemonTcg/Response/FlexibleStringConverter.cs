using System.Text.Json;
using System.Text.Json.Serialization;

namespace TakedownTCGApplication.Models.PokemonTcg.Response;

public sealed class FlexibleStringConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.String => reader.GetString() ?? string.Empty,
            JsonTokenType.Number => reader.TryGetInt64(out long longValue)
                ? longValue.ToString()
                : reader.GetDecimal().ToString(System.Globalization.CultureInfo.InvariantCulture),
            JsonTokenType.True => bool.TrueString,
            JsonTokenType.False => bool.FalseString,
            JsonTokenType.Null => string.Empty,
            JsonTokenType.StartObject => ReadObjectValue(ref reader),
            JsonTokenType.StartArray => ReadArrayValue(ref reader),
            _ => throw new JsonException($"Cannot convert {reader.TokenType} to string.")
        };
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }

    private static string ReadObjectValue(ref Utf8JsonReader reader)
    {
        using JsonDocument document = JsonDocument.ParseValue(ref reader);
        JsonElement root = document.RootElement;

        foreach (string propertyName in new[] { "name", "title", "url", "small", "large", "value" })
        {
            if (root.TryGetProperty(propertyName, out JsonElement value))
            {
                string extracted = ReadElementValue(value);
                if (!string.IsNullOrWhiteSpace(extracted))
                {
                    return extracted;
                }
            }
        }

        return root.GetRawText();
    }

    private static string ReadArrayValue(ref Utf8JsonReader reader)
    {
        using JsonDocument document = JsonDocument.ParseValue(ref reader);

        return string.Join(
            ", ",
            document.RootElement
                .EnumerateArray()
                .Select(ReadElementValue)
                .Where(value => !string.IsNullOrWhiteSpace(value)));
    }

    private static string ReadElementValue(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString() ?? string.Empty,
            JsonValueKind.Number => element.TryGetInt64(out long longValue)
                ? longValue.ToString()
                : element.GetDecimal().ToString(System.Globalization.CultureInfo.InvariantCulture),
            JsonValueKind.True => bool.TrueString,
            JsonValueKind.False => bool.FalseString,
            JsonValueKind.Null => string.Empty,
            JsonValueKind.Object => ReadObjectElementValue(element),
            JsonValueKind.Array => string.Join(
                ", ",
                element.EnumerateArray()
                    .Select(ReadElementValue)
                    .Where(value => !string.IsNullOrWhiteSpace(value))),
            _ => string.Empty
        };
    }

    private static string ReadObjectElementValue(JsonElement element)
    {
        foreach (string propertyName in new[] { "name", "title", "url", "small", "large", "value" })
        {
            if (element.TryGetProperty(propertyName, out JsonElement value))
            {
                string extracted = ReadElementValue(value);
                if (!string.IsNullOrWhiteSpace(extracted))
                {
                    return extracted;
                }
            }
        }

        return element.GetRawText();
    }
}
