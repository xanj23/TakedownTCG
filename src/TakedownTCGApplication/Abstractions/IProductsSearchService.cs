using TakedownTCGApplication.Models.JustTcg.Query;
using TakedownTCGApplication.Models.Search;

namespace TakedownTCGApplication.Abstractions;

public interface IProductsSearchService
{
    Task<ProductsSearchOperationResult> SearchCardsAsync(CardQueryParams query, string? userName, int fallbackLimit, CancellationToken cancellationToken = default);
    Task<ProductsSearchOperationResult> SearchSetsAsync(SetQueryParams query, int fallbackLimit, CancellationToken cancellationToken = default);
    Task<ProductsSearchOperationResult> SearchGamesAsync(GameQueryParams query, int fallbackLimit, CancellationToken cancellationToken = default);
}
