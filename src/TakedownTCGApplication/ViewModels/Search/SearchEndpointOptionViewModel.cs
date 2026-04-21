namespace TakedownTCGApplication.ViewModels.Search;

public sealed class SearchEndpointOptionViewModel
{
    public string Value { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public string SupportedApis { get; init; } = string.Empty;
    public string CategoryIds { get; init; } = string.Empty;
}
