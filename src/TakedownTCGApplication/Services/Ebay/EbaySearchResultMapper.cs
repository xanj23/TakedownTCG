using TakedownTCGApplication.Abstractions;
using TakedownTCGApplication.Models.Ebay.Response;
using TakedownTCGApplication.Models.Search;

namespace TakedownTCGApplication.Services.Ebay;

public sealed class EbaySearchResultMapper : IEbaySearchResultMapper
{
    private const string MissingCardImageUrl = "https://tcgplayer-cdn.tcgplayer.com/product/image-missing.svg";

    public IReadOnlyList<CardSearchResult> MapItems(IEnumerable<EbayItemSummary> items, IReadOnlySet<string> favoriteCardIds)
    {
        return items
            .Select(item => MapItem(item, favoriteCardIds.Contains(item.ItemId)))
            .ToList();
    }

    private static CardSearchResult MapItem(EbayItemSummary item, bool isFavorited)
    {
        return new CardSearchResult
        {
            Id = item.ItemId,
            Name = item.Title,
            Game = "eBay",
            SetName = "Active listing",
            SetCode = item.Price?.Currency ?? string.Empty,
            Number = item.ItemId,
            Rarity = string.IsNullOrWhiteSpace(item.Condition) ? "Listing" : item.Condition,
            Details = BuildDetails(item),
            VariantsCount = item.BuyingOptions.Count,
            FallbackImageUrl = MissingCardImageUrl,
            ImageUrl = SelectImageUrl(item),
            TcgplayerProductUrl = string.IsNullOrWhiteSpace(item.ItemWebUrl) ? null : item.ItemWebUrl,
            DisplayPrice = item.Price?.ExtractedValue,
            DisplayPriceLabel = "eBay listing price",
            Price90d = null,
            IsFavorited = isFavorited
        };
    }

    private static string SelectImageUrl(EbayItemSummary item)
    {
        if (!string.IsNullOrWhiteSpace(item.Image?.ImageUrl))
        {
            return item.Image.ImageUrl;
        }

        string? thumbnail = item.ThumbnailImages
            .Select(image => image.ImageUrl)
            .FirstOrDefault(url => !string.IsNullOrWhiteSpace(url));

        return string.IsNullOrWhiteSpace(thumbnail) ? MissingCardImageUrl : thumbnail;
    }

    private static string BuildDetails(EbayItemSummary item)
    {
        List<string> details = new();

        if (item.BuyingOptions.Count > 0)
        {
            details.Add($"Buying options: {string.Join(", ", item.BuyingOptions)}");
        }

        if (!string.IsNullOrWhiteSpace(item.Seller?.Username))
        {
            details.Add($"Seller: {item.Seller.Username}");
        }

        if (!string.IsNullOrWhiteSpace(item.ItemCreationDate))
        {
            details.Add($"Listed: {FormatDate(item.ItemCreationDate)}");
        }

        if (!string.IsNullOrWhiteSpace(item.ItemEndDate))
        {
            details.Add($"Ends: {FormatDate(item.ItemEndDate)}");
        }

        return string.Join(" | ", details);
    }

    private static string FormatDate(string value)
    {
        return DateTimeOffset.TryParse(value, out DateTimeOffset parsed)
            ? parsed.ToLocalTime().ToString("MMM d, yyyy")
            : value;
    }
}
