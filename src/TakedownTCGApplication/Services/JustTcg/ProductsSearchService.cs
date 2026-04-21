using TakedownTCGApplication.Abstractions;
using TakedownTCGApplication.Models.JustTcg.Query;
using TakedownTCGApplication.Models.JustTcg.Response;
using TakedownTCGApplication.Models.Search;
using TakedownTCGApplication.Models.UserAccounts;

namespace TakedownTCGApplication.Services.JustTcg;

public sealed class ProductsSearchService : IProductsSearchService
{
    private readonly IJustTcgSearchService _searchService;
    private readonly IFavoriteService _favoriteService;
    private readonly IProductsSearchResultMapper _resultMapper;

    public ProductsSearchService(
        IJustTcgSearchService searchService,
        IFavoriteService favoriteService,
        IProductsSearchResultMapper resultMapper)
    {
        _searchService = searchService;
        _favoriteService = favoriteService;
        _resultMapper = resultMapper;
    }

    public async Task<ProductsSearchOperationResult> SearchCardsAsync(
        CardQueryParams query,
        string? userName,
        int fallbackLimit,
        CancellationToken cancellationToken = default)
    {
        Response<Card> response = (Response<Card>)await _searchService.SearchAsync(JustTcgEndpoint.Cards, query, cancellationToken);

        IReadOnlySet<string> favoriteCardIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (!string.IsNullOrWhiteSpace(userName))
        {
            IReadOnlyList<Favorite> favorites = await _favoriteService.GetFavoritesAsync(userName);
            favoriteCardIds = favorites
                .Where(f => string.Equals(f.ItemType, "card", StringComparison.OrdinalIgnoreCase))
                .Select(f => f.ItemId)
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        ProductsSearchOperationResult result = CreateOperationResult(response.Meta, fallbackLimit);
        result.ErrorMessage = response.Error ?? string.Empty;
        result.CardDisplayResults = _resultMapper.MapCards(response.Data, favoriteCardIds);

        return result;
    }

    public async Task<ProductsSearchOperationResult> SearchSetsAsync(
        SetQueryParams query,
        int fallbackLimit,
        CancellationToken cancellationToken = default)
    {
        Response<Set> response = (Response<Set>)await _searchService.SearchAsync(JustTcgEndpoint.Sets, query, cancellationToken);
        ProductsSearchOperationResult result = CreateOperationResult(response.Meta, fallbackLimit);
        result.ErrorMessage = response.Error ?? string.Empty;
        result.SetDisplayResults = _resultMapper.MapSets(response.Data);
        return result;
    }

    public async Task<ProductsSearchOperationResult> SearchGamesAsync(
        GameQueryParams query,
        int fallbackLimit,
        CancellationToken cancellationToken = default)
    {
        Response<Game> response = (Response<Game>)await _searchService.SearchAsync(JustTcgEndpoint.Games, query, cancellationToken);
        ProductsSearchOperationResult result = CreateOperationResult(response.Meta, fallbackLimit);
        result.ErrorMessage = response.Error ?? string.Empty;
        result.GameDisplayResults = _resultMapper.MapGames(response.Data);
        return result;
    }

    private static ProductsSearchOperationResult CreateOperationResult(Meta meta, int fallbackLimit)
    {
        int limit = meta.Limit > 0 ? meta.Limit : Math.Max(1, fallbackLimit);
        int offset = Math.Max(0, meta.Offset);
        int total = Math.Max(0, meta.Total);

        return new ProductsSearchOperationResult
        {
            Total = total,
            Offset = offset,
            Limit = limit,
            HasMore = meta.HasMore,
            HasPrevious = offset > 0,
            PreviousOffset = Math.Max(0, offset - limit),
            NextOffset = offset + limit,
            CurrentPage = (offset / limit) + 1,
            TotalPages = Math.Max(1, (int)Math.Ceiling((double)total / limit))
        };
    }

}
