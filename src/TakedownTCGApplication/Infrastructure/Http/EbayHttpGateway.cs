using System.Net.Http.Headers;
using System.Text;
using TakedownTCGApplication.Abstractions;

namespace TakedownTCGApplication.Infrastructure.Http;

public sealed class EbayHttpGateway : IEbayHttpGateway
{
    private static readonly HttpClient HttpClient = new();

    public async Task<string> FetchTokenAsync(
        string tokenUrl,
        string clientId,
        string clientSecret,
        string scope,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
        {
            throw new InvalidOperationException("eBay API credentials are missing. Add ClientId and ClientSecret to EbayApi configuration.");
        }

        using HttpRequestMessage request = new(HttpMethod.Post, tokenUrl);
        string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["scope"] = scope
        });

        using HttpResponseMessage response = await HttpClient.SendAsync(request, cancellationToken);
        string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"eBay token request failed: {(int)response.StatusCode} {response.ReasonPhrase}. {responseContent}");
        }

        return responseContent;
    }

    public async Task<string> FetchSearchResponseAsync(
        string url,
        string accessToken,
        string marketplaceId,
        CancellationToken cancellationToken = default)
    {
        using HttpRequestMessage request = new(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Headers.Add("X-EBAY-C-MARKETPLACE-ID", marketplaceId);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using HttpResponseMessage response = await HttpClient.SendAsync(request, cancellationToken);
        string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"eBay search failed: {(int)response.StatusCode} {response.ReasonPhrase}. {responseContent}");
        }

        return responseContent;
    }
}
