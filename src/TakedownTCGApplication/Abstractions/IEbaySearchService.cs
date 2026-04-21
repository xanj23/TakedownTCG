using TakedownTCGApplication.Models.Ebay.Query;

namespace TakedownTCGApplication.Abstractions;

public interface IEbaySearchService
{
    Task<object> SearchItemsAsync(EbayItemSearchQueryParams query, CancellationToken cancellationToken = default);
}
