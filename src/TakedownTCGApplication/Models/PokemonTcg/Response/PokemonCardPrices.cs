using System.Text.Json.Serialization;

namespace TakedownTCGApplication.Models.PokemonTcg.Response;

public sealed class PokemonCardPrices
{
    [JsonPropertyName("cardmarket")]
    public PokemonCardmarketPriceSummary? CardMarket { get; set; }

    [JsonPropertyName("tcg_player")]
    public PokemonTcgPlayerPriceSummary? TcgPlayer { get; set; }
}
