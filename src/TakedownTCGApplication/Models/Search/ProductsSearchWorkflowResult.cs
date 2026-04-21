namespace TakedownTCGApplication.Models.Search;

public sealed class ProductsSearchWorkflowResult
{
    public ProductsSearchRequest Request { get; init; } = new();
    public ProductsSearchOperationResult OperationResult { get; init; } = new();
    public IReadOnlyList<SearchValidationError> ValidationErrors { get; init; } = Array.Empty<SearchValidationError>();
}
