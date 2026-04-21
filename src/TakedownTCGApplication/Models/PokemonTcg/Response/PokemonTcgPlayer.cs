using System.Text.Json.Serialization;

namespace TakedownTCGApplication.Models.PokemonTcg.Response;

public sealed class PokemonTcgPlayer
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("prices")]
    public Dictionary<string, PokemonTcgPlayerPrice> Prices { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}
