using TakedownTCGApplication.Abstractions;
using TakedownTCGApplication.Models.Ebay.Query;
using TakedownTCGApplication.Models.Ebay.Response;
using TakedownTCGApplication.Models.Search;
using TakedownTCGApplication.Models.UserAccounts;

namespace TakedownTCGApplication.Services.Ebay;

public sealed class EbayProductsSearchService : IEbayProductsSearchService
{
    private readonly IEbaySearchService _searchService;
    private readonly IFavoriteService _favoriteService;
    private readonly IEbaySearchResultMapper _resultMapper;

    public EbayProductsSearchService(
        IEbaySearchService searchService,
        IFavoriteService favoriteService,
        IEbaySearchResultMapper resultMapper)
    {
        _searchService = searchService;
        _favoriteService = favoriteService;
        _resultMapper = resultMapper;
    }

    public async Task<ProductsSearchOperationResult> SearchCardsAsync(
        EbayItemSearchQueryParams query,
        string? userName,
        CancellationToken cancellationToken = default)
    {
        EbaySearchResponse response = (EbaySearchResponse)await _searchService.SearchItemsAsync(query, cancellationToken);
        IReadOnlySet<string> favoriteCardIds = await LoadFavoriteCardIdsAsync(userName);

        int limit = response.Limit > 0 ? response.Limit : Math.Max(1, query.Limit);
        int total = Math.Max(response.Total, response.ItemSummaries.Count);
        int offset = Math.Max(0, response.Offset);
        int totalPages = Math.Max(1, (int)Math.Ceiling((double)Math.Max(total, response.ItemSummaries.Count) / limit));
        int currentPage = (offset / limit) + 1;

        return new ProductsSearchOperationResult
        {
            CardDisplayResults = _resultMapper.MapItems(response.ItemSummaries, favoriteCardIds),
            Total = total,
            Offset = offset,
            Limit = limit,
            HasMore = offset + limit < total,
            HasPrevious = offset > 0,
            PreviousOffset = Math.Max(0, offset - limit),
            NextOffset = offset + limit,
            CurrentPage = currentPage,
            TotalPages = totalPages
        };
    }

    private async Task<IReadOnlySet<string>> LoadFavoriteCardIdsAsync(string? userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        IReadOnlyList<Favorite> favorites = await _favoriteService.GetFavoritesAsync(userName);
        return favorites
            .Where(f => string.Equals(f.ItemType, "card", StringComparison.OrdinalIgnoreCase))
            .Select(f => f.ItemId)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }
}
