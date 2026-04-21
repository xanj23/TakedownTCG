namespace TakedownTCGApplication.ViewModels.Search;

public sealed class SearchApiOptionViewModel
{
    public string Value { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public string EndpointLabel { get; init; } = "Endpoint";
    public string DefaultEndpoint { get; init; } = string.Empty;
    public bool RequiresEndpointSelection { get; init; } = true;
}
