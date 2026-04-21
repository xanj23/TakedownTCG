using System.Text.Json.Serialization;

namespace TakedownTCGApplication.Models.PokemonTcg.Response;

public sealed class PokemonCardMarketPrices
{
    [JsonPropertyName("averageSellPrice")]
    public decimal? AverageSellPrice { get; set; }
}
