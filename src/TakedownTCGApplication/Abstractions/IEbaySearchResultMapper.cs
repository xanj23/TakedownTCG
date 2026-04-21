using TakedownTCGApplication.Models.Ebay.Response;
using TakedownTCGApplication.Models.Search;

namespace TakedownTCGApplication.Abstractions;

public interface IEbaySearchResultMapper
{
    IReadOnlyList<CardSearchResult> MapItems(IEnumerable<EbayItemSummary> items, IReadOnlySet<string> favoriteCardIds);
}
