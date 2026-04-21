using TakedownTCGApplication.Models.Search;

namespace TakedownTCGApplication.Abstractions;

public interface IProductsSearchWorkflow
{
    ProductsSearchRequest CreateSearchRequest(string? endpoint = null);

    Task<ProductsSearchWorkflowResult> SearchAsync(
        ProductsSearchRequest request,
        string? userName,
        CancellationToken cancellationToken = default);
}
