using TakedownTCGApplication.Abstractions;
using TakedownTCGApplication.Models.PokemonTcg.Query;
using TakedownTCGApplication.Models.PokemonTcg.Response;
using TakedownTCGApplication.Models.Search;
using TakedownTCGApplication.Models.UserAccounts;

namespace TakedownTCGApplication.Services.PokemonTcg;

public sealed class PokemonProductsSearchService : IPokemonProductsSearchService
{
    private readonly IPokemonTcgSearchService _searchService;
    private readonly IFavoriteService _favoriteService;
    private readonly IPokemonSearchResultMapper _resultMapper;

    public PokemonProductsSearchService(
        IPokemonTcgSearchService searchService,
        IFavoriteService favoriteService,
        IPokemonSearchResultMapper resultMapper)
    {
        _searchService = searchService;
        _favoriteService = favoriteService;
        _resultMapper = resultMapper;
    }

    public async Task<ProductsSearchOperationResult> SearchCardsAsync(
        PokemonCardQueryParams query,
        string? userName,
        CancellationToken cancellationToken = default)
    {
        PokemonCardsResponse response = (PokemonCardsResponse)await _searchService.SearchCardsAsync(query, cancellationToken);
        IReadOnlySet<string> favoriteCardIds = await LoadFavoriteCardIdsAsync(userName);

        int limit = response.PageSize > 0 ? response.PageSize : Math.Max(1, query.PerPage);
        int total = response.TotalCount > 0 ? response.TotalCount : response.Count;
        int currentPage = Math.Max(1, response.Page);
        int offset = (currentPage - 1) * limit;
        int totalPages = Math.Max(1, (int)Math.Ceiling((double)Math.Max(total, response.Data.Count) / limit));

        return new ProductsSearchOperationResult
        {
            CardDisplayResults = _resultMapper.MapCards(response.Data, favoriteCardIds),
            Total = total,
            Offset = offset,
            Limit = limit,
            HasMore = currentPage < totalPages,
            HasPrevious = currentPage > 1,
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
