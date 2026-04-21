using TakedownTCGApplication.Abstractions;
using TakedownTCGApplication.Models.JustTcg.Query;
using TakedownTCGApplication.Models.JustTcg.Response;
using TakedownTCGApplication.Models.Search;
using TakedownTCGApplication.Models.UserAccounts;

namespace TakedownTCGApplication.Services.JustTcg;

public sealed class ProductsSearchService : IProductsSearchService
{
    private readonly IJustTcgSearchService _searchService;
    private readonly IFavoriteService _favoriteService;
    private const string MissingCardImageUrl = "https://tcgplayer-cdn.tcgplayer.com/product/image-missing.svg";

    public ProductsSearchService(IJustTcgSearchService searchService, IFavoriteService favoriteService)
    {
        _searchService = searchService;
        _favoriteService = favoriteService;
    }

    public async Task<ProductsSearchOperationResult> SearchCardsAsync(
        CardQueryParams query,
        string? userName,
        int fallbackLimit,
        CancellationToken cancellationToken = default)
    {
        Response<Card> response = (Response<Card>)await _searchService.SearchAsync(JustTcgEndpoint.Cards, query, cancellationToken);

        IReadOnlySet<string> favoriteCardIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (!string.IsNullOrWhiteSpace(userName))
        {
            IReadOnlyList<Favorite> favorites = await _favoriteService.GetFavoritesAsync(userName);
            favoriteCardIds = favorites
                .Where(f => string.Equals(f.ItemType, "card", StringComparison.OrdinalIgnoreCase))
                .Select(f => f.ItemId)
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        ProductsSearchOperationResult result = CreateOperationResult(response.Meta, fallbackLimit);
        result.CardResults = response.Data;
        result.ErrorMessage = response.Error ?? string.Empty;
        result.FavoriteCardIds = favoriteCardIds;
        result.CardDisplayResults = response.Data
            .Select(card => MapCardResult(card, favoriteCardIds.Contains(card.Id)))
            .ToList();

        return result;
    }

    public async Task<ProductsSearchOperationResult> SearchSetsAsync(
        SetQueryParams query,
        int fallbackLimit,
        CancellationToken cancellationToken = default)
    {
        Response<Set> response = (Response<Set>)await _searchService.SearchAsync(JustTcgEndpoint.Sets, query, cancellationToken);
        ProductsSearchOperationResult result = CreateOperationResult(response.Meta, fallbackLimit);
        result.SetResults = response.Data;
        result.ErrorMessage = response.Error ?? string.Empty;
        result.SetDisplayResults = response.Data.Select(MapSetResult).ToList();
        return result;
    }

    public async Task<ProductsSearchOperationResult> SearchGamesAsync(
        GameQueryParams query,
        int fallbackLimit,
        CancellationToken cancellationToken = default)
    {
        Response<Game> response = (Response<Game>)await _searchService.SearchAsync(JustTcgEndpoint.Games, query, cancellationToken);
        ProductsSearchOperationResult result = CreateOperationResult(response.Meta, fallbackLimit);
        result.GameResults = response.Data;
        result.ErrorMessage = response.Error ?? string.Empty;
        result.GameDisplayResults = response.Data.Select(MapGameResult).ToList();
        return result;
    }

    private static ProductsSearchOperationResult CreateOperationResult(Meta meta, int fallbackLimit)
    {
        int limit = meta.Limit > 0 ? meta.Limit : Math.Max(1, fallbackLimit);
        int offset = Math.Max(0, meta.Offset);
        int total = Math.Max(0, meta.Total);

        return new ProductsSearchOperationResult
        {
            Total = total,
            Offset = offset,
            Limit = limit,
            HasMore = meta.HasMore,
            HasPrevious = offset > 0,
            PreviousOffset = Math.Max(0, offset - limit),
            NextOffset = offset + limit,
            CurrentPage = (offset / limit) + 1,
            TotalPages = Math.Max(1, (int)Math.Ceiling((double)total / limit))
        };
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
