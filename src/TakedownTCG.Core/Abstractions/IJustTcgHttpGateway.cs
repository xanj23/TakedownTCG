namespace TakedownTCG.Core.Abstractions;

public interface IJustTcgHttpGateway
{
    Task<string> FetchResponseAsync(
        string url,
        string apiKeyHeaderName,
        string apiKey,
        CancellationToken cancellationToken = default);
}
