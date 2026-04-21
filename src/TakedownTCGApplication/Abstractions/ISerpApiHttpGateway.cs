namespace TakedownTCGApplication.Abstractions;

public interface ISerpApiHttpGateway
{
    Task<string> FetchResponseAsync(string url, string apiKey, CancellationToken cancellationToken = default);
}
