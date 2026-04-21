namespace TakedownTCGApplication.Models.PokemonTcg.Query;

public sealed class PokemonCardQueryParams
{
    public string? Search { get; set; }
    public string? TcgId { get; set; }
    public string? Sort { get; set; } = "relevance";
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 20;
}
