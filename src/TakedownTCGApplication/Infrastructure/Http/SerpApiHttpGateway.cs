using TakedownTCGApplication.Abstractions;

namespace TakedownTCGApplication.Infrastructure.Http;

public sealed class SerpApiHttpGateway : ISerpApiHttpGateway
{
    private static readonly HttpClient HttpClient = new();

    public async Task<string> FetchResponseAsync(string url, string apiKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("SerpApi key is missing. Add ApiKey to SerpApi configuration.");
        }

        string separator = url.Contains('?') ? "&" : "?";
        string requestUrl = $"{url}{separator}api_key={Uri.EscapeDataString(apiKey)}";
        using HttpRequestMessage request = new(HttpMethod.Get, requestUrl);
        request.Headers.Add("Accept", "application/json");

        using HttpResponseMessage response = await HttpClient.SendAsync(request, cancellationToken);
        string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"SerpApi request failed: {(int)response.StatusCode} {response.ReasonPhrase}. {responseContent}");
        }

        return responseContent;
    }
}
