namespace TakedownTCGApplication.Infrastructure.Config;

public sealed class SerpApiOptions
{
    public string BaseUrl { get; set; } = "https://serpapi.com/search";
    public string ApiKey { get; set; } = string.Empty;
    public string EbayDomain { get; set; } = "ebay.com";
    public string DefaultQuery { get; set; } = "tcg trading card";
    public string CategoryId { get; set; } = "183454";
    public string ShowOnly { get; set; } = "Sold,Complete";
    public int PageSize { get; set; } = 25;
    public int CarouselLimit { get; set; } = 6;
}
