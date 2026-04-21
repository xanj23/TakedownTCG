namespace TakedownTCGApplication.Abstractions;

public interface IEbayHttpGateway
{
    Task<string> FetchTokenAsync(
        string tokenUrl,
        string clientId,
        string clientSecret,
        string scope,
        CancellationToken cancellationToken = default);

    Task<string> FetchSearchResponseAsync(
        string url,
        string accessToken,
        string marketplaceId,
        CancellationToken cancellationToken = default);
}
