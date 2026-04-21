using System.Text.Json;
using TakedownTCGApplication.Models.Ebay.Response;

namespace TakedownTCGApplication.Services.Ebay;

public sealed class EbayResponseService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public EbaySearchResponse DeserializeSearch(string responseContent)
    {
        EbaySearchResponse? response = JsonSerializer.Deserialize<EbaySearchResponse>(responseContent, JsonOptions);
        if (response is not null)
        {
            return response;
        }

        throw new InvalidOperationException("Failed to deserialize eBay search response.");
    }
}
