using System.Text.Json;
using System.Text.Json.Serialization;

namespace TakedownTCGApplication.Models.PokemonTcg.Response;

public sealed class PokemonGradedPriceListConverter : JsonConverter<List<PokemonGradedPrice>>
{
    public override List<PokemonGradedPrice> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using JsonDocument document = JsonDocument.ParseValue(ref reader);
        List<PokemonGradedPrice> prices = new();

        ReadElement(document.RootElement, prices, companyHint: string.Empty, gradeHint: string.Empty);

        return prices
            .Where(price => price.Price.HasValue)
            .ToList();
    }

    public override void Write(Utf8JsonWriter writer, List<PokemonGradedPrice> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }

    private static void ReadElement(JsonElement element, List<PokemonGradedPrice> prices, string companyHint, string gradeHint)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                ReadObject(element, prices, companyHint, gradeHint);
                break;
            case JsonValueKind.Array:
                foreach (JsonElement item in element.EnumerateArray())
                {
                    ReadElement(item, prices, companyHint, gradeHint);
                }
                break;
            case JsonValueKind.Number:
                AddPrice(prices, companyHint, gradeHint, ReadDecimal(element));
                break;
        }
    }

    private static void ReadObject(JsonElement element, List<PokemonGradedPrice> prices, string companyHint, string gradeHint)
    {
        decimal? directPrice = ExtractPrice(element);
        if (directPrice.HasValue)
        {
            AddPrice(
                prices,
                ExtractString(element, "company", "grader", "grading_company") ?? companyHint,
                ExtractString(element, "grade", "label", "name") ?? gradeHint,
                directPrice);
            return;
        }

        foreach (JsonProperty property in element.EnumerateObject())
        {
            if (IsMetadataProperty(property.Name))
            {
                continue;
            }

            string nextCompany = LooksLikeCompany(property.Name) ? property.Name : companyHint;
            string nextGrade = LooksLikeCompany(property.Name) ? gradeHint : property.Name;

            if (property.Value.ValueKind == JsonValueKind.Number)
            {
                AddPrice(prices, nextCompany, nextGrade, ReadDecimal(property.Value));
                continue;
            }

            ReadElement(property.Value, prices, nextCompany, nextGrade);
        }
    }

    private static void AddPrice(List<PokemonGradedPrice> prices, string company, string grade, decimal? price)
    {
        if (!price.HasValue)
        {
            return;
        }

        string resolvedGrade = string.IsNullOrWhiteSpace(grade) ? company : grade;
        string resolvedCompany = string.IsNullOrWhiteSpace(company) ? InferCompany(resolvedGrade) : company;

        prices.Add(new PokemonGradedPrice
        {
            Company = resolvedCompany,
            Grade = resolvedGrade,
            Price = price
        });
    }

    private static decimal? ExtractPrice(JsonElement element)
    {
        foreach (string propertyName in new[] { "price", "value", "amount", "market_price", "market", "average", "avg", "listing_price", "listed_price" })
        {
            if (element.TryGetProperty(propertyName, out JsonElement value))
            {
                decimal? price = ReadDecimal(value);
                if (price.HasValue)
                {
                    return price;
                }
            }
        }

        return null;
    }

    private static string? ExtractString(JsonElement element, params string[] propertyNames)
    {
        foreach (string propertyName in propertyNames)
        {
            if (element.TryGetProperty(propertyName, out JsonElement value) && value.ValueKind == JsonValueKind.String)
            {
                return value.GetString();
            }
        }

        return null;
    }

    private static decimal? ReadDecimal(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Number when element.TryGetDecimal(out decimal value) => value,
            JsonValueKind.String when decimal.TryParse(element.GetString(), out decimal value) => value,
            _ => null
        };
    }

    private static bool IsMetadataProperty(string propertyName)
    {
        return propertyName.Equals("currency", StringComparison.OrdinalIgnoreCase)
            || propertyName.Equals("last_updated", StringComparison.OrdinalIgnoreCase);
    }

    private static bool LooksLikeCompany(string value)
    {
        return value.Equals("psa", StringComparison.OrdinalIgnoreCase)
            || value.Equals("bgs", StringComparison.OrdinalIgnoreCase)
            || value.Equals("beckett", StringComparison.OrdinalIgnoreCase)
            || value.Equals("cgc", StringComparison.OrdinalIgnoreCase);
    }

    private static string InferCompany(string grade)
    {
        string prefix = new(grade.TakeWhile(char.IsLetter).ToArray());

        return string.IsNullOrWhiteSpace(prefix) ? string.Empty : prefix;
    }
}
