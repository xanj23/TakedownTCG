using System.Text.Json.Serialization;

namespace TakedownTCGApplication.Models.SerpApi.Response;

public sealed class SerpApiEbayPrice
{
    [JsonPropertyName("raw")]
    public string Raw { get; set; } = string.Empty;

    [JsonPropertyName("extracted")]
    public decimal? Extracted { get; set; }

    [JsonPropertyName("from")]
    public SerpApiEbayPrice? From { get; set; }

    [JsonPropertyName("to")]
    public SerpApiEbayPrice? To { get; set; }
}
