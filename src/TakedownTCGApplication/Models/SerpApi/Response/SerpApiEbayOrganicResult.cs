using System.Text.Json.Serialization;

namespace TakedownTCGApplication.Models.SerpApi.Response;

public sealed class SerpApiEbayOrganicResult
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("link")]
    public string Link { get; set; } = string.Empty;

    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    [JsonPropertyName("condition")]
    public string Condition { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    public SerpApiEbayPrice? Price { get; set; }

    [JsonPropertyName("thumbnail")]
    public string Thumbnail { get; set; } = string.Empty;

    [JsonPropertyName("seller")]
    public SerpApiEbaySeller? Seller { get; set; }

    [JsonPropertyName("shipping")]
    public string Shipping { get; set; } = string.Empty;
}
