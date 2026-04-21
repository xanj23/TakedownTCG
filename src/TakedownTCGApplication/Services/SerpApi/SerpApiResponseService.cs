using System.Text.Json;
using TakedownTCGApplication.Models.SerpApi.Response;

namespace TakedownTCGApplication.Services.SerpApi;

public sealed class SerpApiResponseService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public SerpApiEbaySearchResponse DeserializeEbaySearch(string responseContent)
    {
        SerpApiEbaySearchResponse? response = JsonSerializer.Deserialize<SerpApiEbaySearchResponse>(responseContent, JsonOptions);
        if (response is null)
        {
            throw new InvalidOperationException("Failed to deserialize SerpApi eBay response.");
        }

        if (!string.IsNullOrWhiteSpace(response.Error))
        {
            throw new InvalidOperationException(response.Error);
        }

        return response;
    }
}
