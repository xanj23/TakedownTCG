using System.Text.Json.Serialization;

namespace TakedownTCGApplication.Models.Ebay.Response;

public sealed class EbayImage
{
    [JsonPropertyName("imageUrl")]
    public string ImageUrl { get; set; } = string.Empty;
}
