namespace TakedownTCGApplication.Models.Search;

public sealed class CardSearchResult
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Game { get; set; } = string.Empty;
    public string SetName { get; set; } = string.Empty;
    public string SetCode { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string Rarity { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public int VariantsCount { get; set; }

    public string FallbackImageUrl { get; set; } = "https://tcgplayer-cdn.tcgplayer.com/product/image-missing.svg";
    public string ImageUrl { get; set; } = string.Empty;
    public string? TcgplayerProductUrl { get; set; }

    public decimal? DisplayPrice { get; set; }
    public string DisplayPriceLabel { get; set; } = "Price";
    public decimal? Price90d { get; set; }
    public bool IsFavorited { get; set; }
}
