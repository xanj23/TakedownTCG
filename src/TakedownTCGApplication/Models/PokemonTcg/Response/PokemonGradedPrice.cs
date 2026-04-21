namespace TakedownTCGApplication.Models.PokemonTcg.Response;

public sealed class PokemonGradedPrice
{
    public string Company { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public decimal? Price { get; set; }
}
