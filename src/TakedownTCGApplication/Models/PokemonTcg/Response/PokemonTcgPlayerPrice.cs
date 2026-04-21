using System.Text.Json.Serialization;

namespace TakedownTCGApplication.Models.PokemonTcg.Response;

public sealed class PokemonTcgPlayerPrice
{
    [JsonPropertyName("market")]
    public decimal? Market { get; set; }

    [JsonPropertyName("mid")]
    public decimal? Mid { get; set; }

    [JsonPropertyName("low")]
    public decimal? Low { get; set; }
}
