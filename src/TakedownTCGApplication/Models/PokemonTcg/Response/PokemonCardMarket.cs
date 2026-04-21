using System.Text.Json.Serialization;

namespace TakedownTCGApplication.Models.PokemonTcg.Response;

public sealed class PokemonCardMarket
{
    [JsonPropertyName("prices")]
    public PokemonCardMarketPrices? Prices { get; set; }
}
