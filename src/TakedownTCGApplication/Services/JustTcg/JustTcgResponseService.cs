using System.Text;
using System.Text.Json;
using TakedownTCGApplication.Abstractions;
using TakedownTCGApplication.Models.JustTcg.Response;

namespace TakedownTCGApplication.Services.JustTcg;

public sealed class JustTcgResponseService : IJustTcgResponseMapper
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public object Deserialize(JustTcgEndpoint endpoint, string responseContent)
    {
        return endpoint switch
        {
            JustTcgEndpoint.Cards => DeserializeResponse<Card>(responseContent),
            JustTcgEndpoint.Sets => DeserializeResponse<Set>(responseContent),
            JustTcgEndpoint.Games => DeserializeResponse<Game>(responseContent),
            _ => throw new NotSupportedException($"Unsupported endpoint: {endpoint}")
        };
    }

    public string Map(object responseData)
    {
        return responseData switch
        {
            Response<Card> cardResponse => MapCards(cardResponse),
            Response<Set> setResponse => MapSets(setResponse),
            Response<Game> gameResponse => MapGames(gameResponse),
            _ => throw new NotSupportedException("Unsupported response type.")
        };
    }

    private static Response<T> DeserializeResponse<T>(string responseContent)
    {
        Response<T>? response = JsonSerializer.Deserialize<Response<T>>(responseContent, JsonOptions);

        if (response == null)
        {
            throw new InvalidOperationException("Failed to deserialize JustTCG response.");
        }

        return response;
    }

    private static string MapCards(Response<Card> response)
    {
        StringBuilder builder = new();
        AppendResponseHeader(builder, response.Data.Count, response.Meta.HasMore, response.Error, response.Code);

        for (int i = 0; i < response.Data.Count; i++)
        {
            Card card = response.Data[i];
            builder.AppendLine($"{i + 1}. {card.Name}");
            builder.AppendLine($"   Id: {card.Id}");
            builder.AppendLine($"   Game: {card.Game}");
            builder.AppendLine($"   Set: {card.SetName} ({card.Set})");
            builder.AppendLine($"   Number: {card.Number}");
            builder.AppendLine($"   Rarity: {card.Rarity}");

            if (!string.IsNullOrWhiteSpace(card.Details))
            {
                builder.AppendLine($"   Details: {card.Details}");
            }

            builder.AppendLine($"   Variants: {card.Variants.Count}");
            builder.AppendLine();
        }

        AppendNoResults(builder, response.Data.Count);
        return builder.ToString().TrimEnd();
    }

    private static string MapSets(Response<Set> response)
    {
        StringBuilder builder = new();
        AppendResponseHeader(builder, response.Data.Count, response.Meta.HasMore, response.Error, response.Code);

        for (int i = 0; i < response.Data.Count; i++)
        {
            Set set = response.Data[i];
            builder.AppendLine($"{i + 1}. {set.Name}");
            builder.AppendLine($"   Id: {set.Id}");
            builder.AppendLine($"   Game: {set.Game}");
            builder.AppendLine($"   Release Date: {set.ReleaseDate}");
            builder.AppendLine($"   Cards: {set.CardsCount}");
            builder.AppendLine($"   Variants: {set.VariantsCount}");
            builder.AppendLine($"   Sealed: {set.SealedCount}");
            builder.AppendLine($"   Set Value USD: {set.SetValueUsd}");
            builder.AppendLine();
        }

        AppendNoResults(builder, response.Data.Count);
        return builder.ToString().TrimEnd();
    }

    private static string MapGames(Response<Game> response)
    {
        StringBuilder builder = new();
        AppendResponseHeader(builder, response.Data.Count, response.Meta.HasMore, response.Error, response.Code);

        for (int i = 0; i < response.Data.Count; i++)
        {
            Game game = response.Data[i];
            builder.AppendLine($"{i + 1}. {game.Name}");
            builder.AppendLine($"   Id: {game.Id}");
            builder.AppendLine($"   Sets: {game.SetsCount}");
            builder.AppendLine($"   Cards: {game.CardsCount}");
            builder.AppendLine($"   Variants: {game.VariantsCount}");
            builder.AppendLine($"   Sealed: {game.SealedCount}");
            builder.AppendLine($"   Game Value Index Cents: {game.GameValueIndexCents}");
            builder.AppendLine($"   Last Updated: {game.LastUpdated}");
            builder.AppendLine();
        }

        AppendNoResults(builder, response.Data.Count);
        return builder.ToString().TrimEnd();
    }

    private static void AppendResponseHeader(StringBuilder builder, int count, bool hasMore, string? error, string? code)
    {
        if (!string.IsNullOrWhiteSpace(error))
        {
            builder.AppendLine($"Error: {error}");

            if (!string.IsNullOrWhiteSpace(code))
            {
                builder.AppendLine($"Code: {code}");
            }

            builder.AppendLine();
        }

        builder.AppendLine($"Results: {count}");
        builder.AppendLine($"Has More: {hasMore}");
        builder.AppendLine();
    }

    private static void AppendNoResults(StringBuilder builder, int count)
    {
        if (count == 0)
        {
            builder.AppendLine("No results found.");
        }
    }
}
