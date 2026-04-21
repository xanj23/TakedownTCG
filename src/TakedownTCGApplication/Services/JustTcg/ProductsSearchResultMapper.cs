using TakedownTCGApplication.Abstractions;
using TakedownTCGApplication.Models.JustTcg.Response;
using TakedownTCGApplication.Models.Search;

namespace TakedownTCGApplication.Services.JustTcg;

public sealed class ProductsSearchResultMapper : IProductsSearchResultMapper
{
    private const string MissingCardImageUrl = "https://tcgplayer-cdn.tcgplayer.com/product/image-missing.svg";

    public IReadOnlyList<CardSearchResult> MapCards(IEnumerable<Card> cards, IReadOnlySet<string> favoriteCardIds)
    {
        return cards
            .Select(card => MapCardResult(card, favoriteCardIds.Contains(card.Id)))
            .ToList();
    }

    public IReadOnlyList<SetSearchResult> MapSets(IEnumerable<Set> sets)
    {
        return sets.Select(MapSetResult).ToList();
    }

    public IReadOnlyList<GameSearchResult> MapGames(IEnumerable<Game> games)
    {
        return games.Select(MapGameResult).ToList();
    }

    private static CardSearchResult MapCardResult(Card card, bool isFavorited)
    {
        string imageUrl = !string.IsNullOrWhiteSpace(card.TcgplayerId)
            ? $"https://tcgplayer-cdn.tcgplayer.com/product/{card.TcgplayerId}_in_1000x1000.jpg"
            : MissingCardImageUrl;

        decimal? displayPrice = card.Variants
            .Where(v => v.Price.HasValue)
            .Select(v => v.Price)
            .FirstOrDefault();

        decimal? price90d = card.Variants
            .Where(v => v.MaxPrice90d.HasValue)
            .Select(v => v.MaxPrice90d)
            .FirstOrDefault();

        return new CardSearchResult
        {
            Id = card.Id,
            Name = card.Name,
            Game = card.Game,
            SetName = card.SetName,
            SetCode = card.Set,
            Number = card.Number,
            Rarity = card.Rarity,
            Details = card.Details,
            VariantsCount = card.Variants.Count,
            FallbackImageUrl = MissingCardImageUrl,
            ImageUrl = imageUrl,
            TcgplayerProductUrl = !string.IsNullOrWhiteSpace(card.TcgplayerId)
                ? $"https://www.tcgplayer.com/product/{card.TcgplayerId}"
                : null,
            DisplayPrice = displayPrice,
            Price90d = price90d,
            IsFavorited = isFavorited
        };
    }

    private static SetSearchResult MapSetResult(Set setItem)
    {
        string setGameSlug = GetTcgplayerGameSlug(setItem.Game);
        string setNameSlug = Slugify(setItem.Name);
        bool hasValidReleaseDate = !string.IsNullOrWhiteSpace(setItem.ReleaseDate)
                                   && !setItem.ReleaseDate.Equals("null", StringComparison.OrdinalIgnoreCase);

        return new SetSearchResult
        {
            Name = setItem.Name,
            Game = setItem.Game,
            ReleaseDate = setItem.ReleaseDate,
            CardsCount = setItem.CardsCount,
            VariantsCount = setItem.VariantsCount,
            SealedCount = setItem.SealedCount,
            SetValueUsd = setItem.SetValueUsd,
            TcgplayerSetUrl = !string.IsNullOrWhiteSpace(setGameSlug) && !string.IsNullOrWhiteSpace(setNameSlug) && hasValidReleaseDate
                ? $"https://www.tcgplayer.com/categories/trading-and-collectible-card-games/{setGameSlug}/{setNameSlug}"
                : null
        };
    }

    private static GameSearchResult MapGameResult(Game game)
    {
        string gameSlug = GetTcgplayerGameSlug(game.Name);

        return new GameSearchResult
        {
            Name = game.Name,
            SetsCount = game.SetsCount,
            CardsCount = game.CardsCount,
            VariantsCount = game.VariantsCount,
            SealedCount = game.SealedCount,
            TcgplayerGameUrl = !string.IsNullOrWhiteSpace(gameSlug)
                ? $"https://www.tcgplayer.com/categories/trading-and-collectible-card-games/{gameSlug}"
                : null
        };
    }

    private static string Slugify(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        char[] chars = value.Trim().ToLowerInvariant().ToCharArray();
        var buffer = new System.Text.StringBuilder(chars.Length);
        bool previousWasDash = false;

        foreach (char ch in chars)
        {
            if (char.IsLetterOrDigit(ch))
            {
                buffer.Append(ch);
                previousWasDash = false;
            }
            else if (!previousWasDash)
            {
                buffer.Append('-');
                previousWasDash = true;
            }
        }

        return buffer.ToString().Trim('-');
    }

    private static string GetTcgplayerGameSlug(string? game)
    {
        string normalized = Slugify(game);
        return normalized switch
        {
            "mtg" => "magic-the-gathering",
            "magic-the-gathering" => "magic-the-gathering",
            "yugioh" => "yu-gi-oh",
            "yu-gi-oh" => "yu-gi-oh",
            _ => normalized
        };
    }
}
