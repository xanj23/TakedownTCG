using TakedownTCGApplication.Models.Ebay.Query;
using TakedownTCGApplication.Models.Search;

namespace TakedownTCGApplication.Abstractions;

public interface IEbayProductsSearchService
{
    Task<ProductsSearchOperationResult> SearchCardsAsync(
        EbayItemSearchQueryParams query,
        string? userName,
        CancellationToken cancellationToken = default);
}
