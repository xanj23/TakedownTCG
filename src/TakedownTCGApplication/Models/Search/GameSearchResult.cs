namespace TakedownTCGApplication.Models.Search;

public sealed class GameSearchResult
{
    public string Name { get; set; } = string.Empty;
    public int SetsCount { get; set; }
    public int CardsCount { get; set; }
    public int VariantsCount { get; set; }
    public int SealedCount { get; set; }
    public string? TcgplayerGameUrl { get; set; }
}
