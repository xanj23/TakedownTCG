using TakedownTCGApplication.Models.JustTcg.Response;

namespace TakedownTCGApplication.ViewModels.Search;

public sealed class GamesSearchViewModel
{
    public IReadOnlyList<Game> Results { get; set; } = Array.Empty<Game>();
    public string ErrorMessage { get; set; } = string.Empty;
}
