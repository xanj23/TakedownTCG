namespace TakedownTCGApplication.Abstractions;

public interface IPokemonTcgHttpGateway
{
    Task<string> FetchResponseAsync(
        string url,
        string apiHostHeaderName,
        string apiHost,
        string apiKeyHeaderName,
        string apiKey,
        CancellationToken cancellationToken = default);
}
