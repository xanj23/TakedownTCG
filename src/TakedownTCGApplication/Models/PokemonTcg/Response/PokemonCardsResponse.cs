using System.Text.Json.Serialization;

namespace TakedownTCGApplication.Models.PokemonTcg.Response;

public sealed class PokemonCardsResponse
{
    [JsonPropertyName("data")]
    public List<PokemonCard> Data { get; set; } = new();

    [JsonPropertyName("cards")]
    public List<PokemonCard> Cards
    {
        set => Data = value;
    }

    [JsonPropertyName("results")]
    public List<PokemonCard> Results
    {
        set => Data = value;
    }

    [JsonPropertyName("page")]
    public int Page { get; set; } = 1;

    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }

    [JsonPropertyName("per_page")]
    public int PerPage
    {
        set => PageSize = value;
    }

    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    [JsonPropertyName("total_count")]
    public int TotalCountLegacy
    {
        set => TotalCount = value;
    }
}
