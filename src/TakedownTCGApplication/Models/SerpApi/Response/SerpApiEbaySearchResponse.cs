using System.Text.Json.Serialization;

namespace TakedownTCGApplication.Models.SerpApi.Response;

public sealed class SerpApiEbaySearchResponse
{
    [JsonPropertyName("organic_results")]
    public List<SerpApiEbayOrganicResult> OrganicResults { get; set; } = new();

    [JsonPropertyName("error")]
    public string Error { get; set; } = string.Empty;
}
