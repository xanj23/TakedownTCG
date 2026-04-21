using System.Text.Json.Serialization;

namespace TakedownTCGApplication.Models.Ebay.Response;

public sealed class EbayItemSummary
{
    [JsonPropertyName("itemId")]
    public string ItemId { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("itemWebUrl")]
    public string ItemWebUrl { get; set; } = string.Empty;

    [JsonPropertyName("image")]
    public EbayImage? Image { get; set; }

    [JsonPropertyName("thumbnailImages")]
    public List<EbayImage> ThumbnailImages { get; set; } = new();

    [JsonPropertyName("price")]
    public EbayMoney? Price { get; set; }

    [JsonPropertyName("condition")]
    public string Condition { get; set; } = string.Empty;

    [JsonPropertyName("buyingOptions")]
    public List<string> BuyingOptions { get; set; } = new();

    [JsonPropertyName("seller")]
    public EbaySeller? Seller { get; set; }

    [JsonPropertyName("itemCreationDate")]
    public string ItemCreationDate { get; set; } = string.Empty;

    [JsonPropertyName("itemEndDate")]
    public string ItemEndDate { get; set; } = string.Empty;
}
