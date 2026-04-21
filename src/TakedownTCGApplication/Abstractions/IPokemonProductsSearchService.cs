using TakedownTCGApplication.Models.PokemonTcg.Query;
using TakedownTCGApplication.Models.Search;

namespace TakedownTCGApplication.Abstractions;

public interface IPokemonProductsSearchService
{
    Task<ProductsSearchOperationResult> SearchCardsAsync(
        PokemonCardQueryParams query,
        string? userName,
        CancellationToken cancellationToken = default);
}
