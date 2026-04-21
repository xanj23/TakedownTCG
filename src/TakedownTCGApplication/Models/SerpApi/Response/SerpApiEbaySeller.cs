using System.Text.Json.Serialization;

namespace TakedownTCGApplication.Models.SerpApi.Response;

public sealed class SerpApiEbaySeller
{
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;
}
