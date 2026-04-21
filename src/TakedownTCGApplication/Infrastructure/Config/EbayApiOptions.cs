namespace TakedownTCGApplication.Infrastructure.Config;

public sealed class EbayApiOptions
{
    public string BaseUrl { get; set; } = "https://api.ebay.com";
    public string TokenUrl { get; set; } = "https://api.ebay.com/identity/v1/oauth2/token";
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string Scope { get; set; } = "https://api.ebay.com/oauth/api_scope";
    public string MarketplaceId { get; set; } = "EBAY_US";
}
