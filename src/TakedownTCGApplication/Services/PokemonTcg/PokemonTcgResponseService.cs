using System.Text.Json;
using TakedownTCGApplication.Models.PokemonTcg.Response;

namespace TakedownTCGApplication.Services.PokemonTcg;

public sealed class PokemonTcgResponseService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    static PokemonTcgResponseService()
    {
        JsonOptions.Converters.Add(new FlexibleStringConverter());
    }

    public PokemonCardsResponse DeserializeCards(string responseContent)
    {
        PokemonCardsResponse? response = JsonSerializer.Deserialize<PokemonCardsResponse>(responseContent, JsonOptions);
        if (response is not null)
        {
            return response;
        }

        List<PokemonCard>? cards = JsonSerializer.Deserialize<List<PokemonCard>>(responseContent, JsonOptions);
        if (cards is not null)
        {
            return new PokemonCardsResponse
            {
                Data = cards,
                Count = cards.Count,
                TotalCount = cards.Count,
                PageSize = cards.Count
            };
        }

        throw new InvalidOperationException("Failed to deserialize Pokemon TCG response.");
    }
}
