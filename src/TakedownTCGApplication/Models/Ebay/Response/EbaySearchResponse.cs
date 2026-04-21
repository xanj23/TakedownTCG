using System.Text.Json.Serialization;

namespace TakedownTCGApplication.Models.Ebay.Response;

public sealed class EbaySearchResponse
{
    [JsonPropertyName("itemSummaries")]
    public List<EbayItemSummary> ItemSummaries { get; set; } = new();

    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    [JsonPropertyName("offset")]
    public int Offset { get; set; }
}
