using System.Text.Json.Serialization;

namespace TakedownTCGApplication.Models.PokemonTcg.Response;

public sealed class PokemonCardImages
{
    [JsonPropertyName("small")]
    public string Small { get; set; } = string.Empty;

    [JsonPropertyName("large")]
    public string Large { get; set; } = string.Empty;
}
