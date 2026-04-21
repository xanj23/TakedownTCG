using System.Text.Json.Serialization;
using System.Globalization;

namespace TakedownTCGApplication.Models.Ebay.Response;

public sealed class EbayMoney
{
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;

    public decimal? ExtractedValue => decimal.TryParse(Value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal parsed)
        ? parsed
        : null;
}
