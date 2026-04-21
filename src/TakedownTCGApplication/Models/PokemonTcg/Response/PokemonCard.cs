using System.Text.Json.Serialization;

namespace TakedownTCGApplication.Models.PokemonTcg.Response;

public sealed class PokemonCard
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("set")]
    public PokemonSetSummary? Set { get; set; }

    [JsonPropertyName("episode")]
    public PokemonSetSummary? Episode { get; set; }

    [JsonPropertyName("number")]
    public string Number { get; set; } = string.Empty;

    [JsonPropertyName("card_number")]
    public string CardNumber { get; set; } = string.Empty;

    [JsonPropertyName("name_numbered")]
    public string NameNumbered { get; set; } = string.Empty;

    [JsonPropertyName("rarity")]
    public string Rarity { get; set; } = string.Empty;

    [JsonPropertyName("types")]
    public List<string> Types { get; set; } = new();

    [JsonPropertyName("artist")]
    public string Artist { get; set; } = string.Empty;

    [JsonPropertyName("images")]
    public PokemonCardImages? Images { get; set; }

    [JsonPropertyName("image")]
    public string Image { get; set; } = string.Empty;

    [JsonPropertyName("image_url")]
    public string ImageUrl { get; set; } = string.Empty;

    [JsonPropertyName("tcgplayer")]
    public PokemonTcgPlayer? TcgPlayer { get; set; }

    [JsonPropertyName("cardmarket")]
    public PokemonCardMarket? CardMarket { get; set; }

    [JsonPropertyName("prices")]
    public PokemonCardPrices? Prices { get; set; }

    [JsonPropertyName("price")]
    public decimal? Price { get; set; }

    [JsonPropertyName("market_price")]
    public decimal? MarketPrice { get; set; }
}
