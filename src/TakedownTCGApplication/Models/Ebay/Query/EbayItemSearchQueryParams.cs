namespace TakedownTCGApplication.Models.Ebay.Query;

public sealed class EbayItemSearchQueryParams
{
    public string? Search { get; set; }
    public string? CategoryIds { get; set; }
    public string? BuyingOptions { get; set; }
    public string? Sort { get; set; }
    public int Limit { get; set; } = 20;
    public int Offset { get; set; }
}
