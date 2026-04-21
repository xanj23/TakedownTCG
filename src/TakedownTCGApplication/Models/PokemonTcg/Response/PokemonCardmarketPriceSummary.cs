using System.Text.Json.Serialization;

namespace TakedownTCGApplication.Models.PokemonTcg.Response;

public sealed class PokemonCardmarketPriceSummary
{
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;

    [JsonPropertyName("lowest_near_mint")]
    public decimal? LowestNearMint { get; set; }

    [JsonPropertyName("30d_average")]
    public decimal? Average30d { get; set; }

    [JsonPropertyName("7d_average")]
    public decimal? Average7d { get; set; }

    [JsonPropertyName("graded")]
    [JsonConverter(typeof(PokemonGradedPriceListConverter))]
    public List<PokemonGradedPrice> Graded { get; set; } = new();
}
