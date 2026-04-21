namespace TakedownTCGApplication.ViewModels.Home;

public sealed class CompletedTcgSaleViewModel
{
    public string Title { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string FallbackImageUrl { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public string PriceText { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public string Seller { get; set; } = string.Empty;
    public string Shipping { get; set; } = string.Empty;
}
