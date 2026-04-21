using TakedownTCGApplication.Abstractions;
using TakedownTCGApplication.Models.SerpApi.Response;
using TakedownTCGApplication.ViewModels.Home;

namespace TakedownTCGApplication.Services.SerpApi;

public sealed class CompletedTcgSaleMapper : ICompletedTcgSaleMapper
{
    private const string MissingImageUrl = "https://tcgplayer-cdn.tcgplayer.com/product/image-missing.svg";

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
            ImageUrl = string.IsNullOrWhiteSpace(result.Thumbnail) ? MissingImageUrl : result.Thumbnail,
            FallbackImageUrl = MissingImageUrl,
            Price = price?.Extracted,
            PriceText = SelectPriceText(result.Price),
            Condition = result.Condition,
            Seller = result.Seller?.Username ?? string.Empty,
            Shipping = result.Shipping
        };
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
