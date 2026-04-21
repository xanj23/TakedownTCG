using TakedownTCGApplication.Abstractions;

namespace TakedownTCGApplication.Infrastructure.Http;

public sealed class PokemonTcgHttpGateway : IPokemonTcgHttpGateway
{
    private static readonly HttpClient HttpClient = new HttpClient();

    public async Task<string> FetchResponseAsync(
        string url,
        string apiHostHeaderName,
        string apiHost,
        string apiKeyHeaderName,
        string apiKey,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                "Pokemon API key is missing. Add ApiKey to PokemonTcgApi configuration before starting the app.");
        }

        using HttpRequestMessage request = new(HttpMethod.Get, url);
        request.Headers.Add("Accept", "application/json");

        if (!string.IsNullOrWhiteSpace(apiHost))
        {
            request.Headers.Add(apiHostHeaderName, apiHost);
        }

        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            request.Headers.Add(apiKeyHeaderName, apiKey);
        }

        using HttpResponseMessage response = await HttpClient.SendAsync(request, cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            throw new InvalidOperationException(
                "Pokemon API authorization failed. Check that PokemonTcgApi:ApiKey is valid and subscribed to pokemon-tcg-api on RapidAPI.");
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}
