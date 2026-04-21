using TakedownTCGApplication.Models.Search;
using TakedownTCGApplication.ViewModels.Search;

namespace TakedownTCGApplication.Abstractions;

public interface IProductsSearchWorkflow
{
    ProductsSearchViewModel CreateSearchModel(string? endpoint = null);

    Task<ProductsSearchWorkflowResult> SearchAsync(
        ProductsSearchViewModel model,
        string? userName,
        CancellationToken cancellationToken = default);
}
