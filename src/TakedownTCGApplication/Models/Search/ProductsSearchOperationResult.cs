using TakedownTCGApplication.Models.JustTcg.Response;

namespace TakedownTCGApplication.Models.Search;

public sealed class ProductsSearchOperationResult
{
    public IReadOnlyList<Card> CardResults { get; set; } = Array.Empty<Card>();
    public IReadOnlyList<Set> SetResults { get; set; } = Array.Empty<Set>();
    public IReadOnlyList<Game> GameResults { get; set; } = Array.Empty<Game>();

    public IReadOnlyList<CardSearchResult> CardDisplayResults { get; set; } = Array.Empty<CardSearchResult>();
    public IReadOnlyList<SetSearchResult> SetDisplayResults { get; set; } = Array.Empty<SetSearchResult>();
    public IReadOnlyList<GameSearchResult> GameDisplayResults { get; set; } = Array.Empty<GameSearchResult>();
    public IReadOnlySet<string> FavoriteCardIds { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public string ErrorMessage { get; set; } = string.Empty;
    public int Total { get; set; }
    public int Offset { get; set; }
    public int Limit { get; set; } = 20;
    public bool HasMore { get; set; }
    public bool HasPrevious { get; set; }
    public int PreviousOffset { get; set; }
    public int NextOffset { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
}
