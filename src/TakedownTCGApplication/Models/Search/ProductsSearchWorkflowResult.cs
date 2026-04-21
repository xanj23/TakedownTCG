using TakedownTCGApplication.ViewModels.Search;

namespace TakedownTCGApplication.Models.Search;

public sealed class ProductsSearchWorkflowResult
{
    public ProductsSearchViewModel Model { get; init; } = new();
    public IReadOnlyList<SearchValidationError> ValidationErrors { get; init; } = Array.Empty<SearchValidationError>();
}
