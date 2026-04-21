namespace TakedownTCGApplication.Models.Search;

public sealed class SetSearchResult
{
    public string Name { get; set; } = string.Empty;
    public string Game { get; set; } = string.Empty;
    public string ReleaseDate { get; set; } = string.Empty;
    public int CardsCount { get; set; }
    public int VariantsCount { get; set; }
    public int SealedCount { get; set; }
    public float SetValueUsd { get; set; }
    public string? TcgplayerSetUrl { get; set; }
}
