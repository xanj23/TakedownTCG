using System.Text.Json;
using TakedownTCGApplication.Abstractions;
using TakedownTCGApplication.Infrastructure.Config;
using TakedownTCGApplication.Models.Ebay.Response;

namespace TakedownTCGApplication.Services.Ebay;

public sealed class EbayOAuthTokenService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly EbayApiOptions _apiOptions;
    private readonly IEbayHttpGateway _httpGateway;
    private string _accessToken = string.Empty;
    private DateTimeOffset _expiresAt = DateTimeOffset.MinValue;

    public EbayOAuthTokenService(EbayApiOptions apiOptions, IEbayHttpGateway httpGateway)
    {
        _apiOptions = apiOptions;
        _httpGateway = httpGateway;
    }

    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(_accessToken) && _expiresAt > DateTimeOffset.UtcNow.AddMinutes(5))
        {
            return _accessToken;
        }

        string responseContent = await _httpGateway.FetchTokenAsync(
            _apiOptions.TokenUrl,
            _apiOptions.ClientId,
            _apiOptions.ClientSecret,
            _apiOptions.Scope,
            cancellationToken);

        EbayAccessTokenResponse? response = JsonSerializer.Deserialize<EbayAccessTokenResponse>(responseContent, JsonOptions);
        if (response is null || string.IsNullOrWhiteSpace(response.AccessToken))
        {
            throw new InvalidOperationException("Failed to deserialize eBay access token response.");
        }

        _accessToken = response.AccessToken;
        _expiresAt = DateTimeOffset.UtcNow.AddSeconds(Math.Max(60, response.ExpiresIn));
        return _accessToken;
    }
}
