using TakedownTCG.Core.Abstractions;

namespace TakedownTCG.Core.Infrastructure.Http;

public sealed class JustTcgHttpGateway : IJustTcgHttpGateway
{
    private static readonly HttpClient HttpClient = new HttpClient();

    public async Task<string> FetchResponseAsync(
        string url,
        string apiKeyHeaderName,
        string apiKey,
        CancellationToken cancellationToken = default)
    {
        using HttpRequestMessage request = new(HttpMethod.Get, url);

        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            request.Headers.Add(apiKeyHeaderName, apiKey);
        }

        using HttpResponseMessage response = await HttpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}
