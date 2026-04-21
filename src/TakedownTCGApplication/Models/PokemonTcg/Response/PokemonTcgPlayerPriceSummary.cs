using System.Text.Json.Serialization;

namespace TakedownTCGApplication.Models.PokemonTcg.Response;

public sealed class PokemonTcgPlayerPriceSummary
{
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;

    [JsonPropertyName("market_price")]
    public decimal? MarketPrice { get; set; }

    [JsonPropertyName("mid_price")]
    public decimal? MidPrice { get; set; }
}
