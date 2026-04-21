using TakedownTCGApplication.Abstractions;
using TakedownTCGApplication.Models.SerpApi.Response;
using TakedownTCGApplication.ViewModels.Home;

namespace TakedownTCGApplication.Services.SerpApi;

public sealed class CompletedTcgSaleMapper : ICompletedTcgSaleMapper
{
    private const string MissingImageUrl = "https://tcgplayer-cdn.tcgplayer.com/product/image-missing.svg";
    private const string EbayImageHost = "i.ebayimg.com";
    private const string PreferredEbayImageSize = "s-l500.jpg";

    public IReadOnlyList<CompletedTcgSaleViewModel> MapSales(IEnumerable<SerpApiEbayOrganicResult> results, int limit)
    {
        return results
            .Where(result => !string.IsNullOrWhiteSpace(result.Title))
            .Take(Math.Max(0, limit))
            .Select(MapSale)
            .ToList();
    }

    private static CompletedTcgSaleViewModel MapSale(SerpApiEbayOrganicResult result)
    {
        SerpApiEbayPrice? price = result.Price?.From ?? result.Price;
        return new CompletedTcgSaleViewModel
        {
            Title = result.Title,
            ProductId = result.ProductId,
            Url = result.Link,
            ImageUrl = SelectImageUrl(result.Thumbnail),
            FallbackImageUrl = MissingImageUrl,
            Price = price?.Extracted,
            PriceText = SelectPriceText(result.Price),
            Condition = result.Condition,
            Seller = result.Seller?.Username ?? string.Empty,
            Shipping = result.Shipping
        };
    }

    private static string SelectImageUrl(string thumbnail)
    {
        if (string.IsNullOrWhiteSpace(thumbnail))
        {
            return MissingImageUrl;
        }

        if (!Uri.TryCreate(thumbnail, UriKind.Absolute, out Uri? uri) ||
            !string.Equals(uri.Host, EbayImageHost, StringComparison.OrdinalIgnoreCase))
        {
            return thumbnail;
        }

        string[] segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        string fileName = segments.Length > 0 ? segments[^1] : string.Empty;
        if (!fileName.StartsWith("s-l", StringComparison.OrdinalIgnoreCase) ||
            !fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
        {
            return thumbnail;
        }

        UriBuilder builder = new(uri);
        int lastSlashIndex = builder.Path.LastIndexOf('/');
        builder.Path = lastSlashIndex < 0
            ? PreferredEbayImageSize
            : string.Concat(builder.Path.AsSpan(0, lastSlashIndex + 1), PreferredEbayImageSize);

        return builder.Uri.ToString();
    }

    private static string SelectPriceText(SerpApiEbayPrice? price)
    {
        if (!string.IsNullOrWhiteSpace(price?.Raw))
        {
            return price.Raw;
        }

        if (!string.IsNullOrWhiteSpace(price?.From?.Raw) && !string.IsNullOrWhiteSpace(price.To?.Raw))
        {
            return $"{price.From.Raw} - {price.To.Raw}";
        }

        return price?.Extracted.HasValue == true ? $"${price.Extracted.Value:0.00}" : "Price unavailable";
    }
}
