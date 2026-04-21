using TakedownTCGApplication.Abstractions;
using TakedownTCGApplication.Models.PokemonTcg.Response;
using TakedownTCGApplication.Models.Search;

namespace TakedownTCGApplication.Services.PokemonTcg;

public sealed class PokemonSearchResultMapper : IPokemonSearchResultMapper
{
    private const string MissingCardImageUrl = "https://tcgplayer-cdn.tcgplayer.com/product/image-missing.svg";
    private const decimal EuroToUsdRate = 1.18m;

    public IReadOnlyList<CardSearchResult> MapCards(IEnumerable<PokemonCard> cards, IReadOnlySet<string> favoriteCardIds)
    {
        return cards
            .Select(card => MapCard(card, favoriteCardIds.Contains(card.Id)))
            .ToList();
    }

    private static CardSearchResult MapCard(PokemonCard card, bool isFavorited)
    {
        GradedPriceSelection gradedPrice = SelectGradedPrice(card);

        return new CardSearchResult
        {
            Id = card.Id,
            Name = card.Name,
            Game = "pokemon",
            SetName = SelectSetName(card),
            SetCode = SelectSetCode(card),
            Number = SelectCardNumber(card),
            Rarity = card.Rarity,
            Details = BuildDetails(card),
            VariantsCount = CountPriceVariants(card),
            FallbackImageUrl = MissingCardImageUrl,
            ImageUrl = SelectImageUrl(card),
            TcgplayerProductUrl = string.IsNullOrWhiteSpace(card.TcgPlayer?.Url) ? null : card.TcgPlayer.Url,
            DisplayPrice = gradedPrice.Price ?? SelectRawDisplayPrice(card),
            DisplayPriceLabel = gradedPrice.Price.HasValue ? $"Graded price ({gradedPrice.Label})" : "Raw price",
            Price90d = null,
            IsFavorited = isFavorited
        };
    }

    private static string SelectImageUrl(PokemonCard card)
    {
        if (!string.IsNullOrWhiteSpace(card.Images?.Large))
        {
            return card.Images.Large;
        }

        if (!string.IsNullOrWhiteSpace(card.Images?.Small))
        {
            return card.Images.Small;
        }

        if (!string.IsNullOrWhiteSpace(card.ImageUrl))
        {
            return card.ImageUrl;
        }

        return string.IsNullOrWhiteSpace(card.Image) ? MissingCardImageUrl : card.Image;
    }

    private static string SelectSetName(PokemonCard card)
    {
        if (!string.IsNullOrWhiteSpace(card.Set?.Name))
        {
            return card.Set.Name;
        }

        if (!string.IsNullOrWhiteSpace(card.Set?.Title))
        {
            return card.Set.Title;
        }

        if (!string.IsNullOrWhiteSpace(card.Episode?.Name))
        {
            return card.Episode.Name;
        }

        return card.Episode?.Title ?? string.Empty;
    }

    private static string SelectSetCode(PokemonCard card)
    {
        if (!string.IsNullOrWhiteSpace(card.Set?.Code))
        {
            return card.Set.Code;
        }

        if (!string.IsNullOrWhiteSpace(card.Set?.Id))
        {
            return card.Set.Id;
        }

        if (!string.IsNullOrWhiteSpace(card.Episode?.Code))
        {
            return card.Episode.Code;
        }

        return card.Episode?.Id ?? string.Empty;
    }

    private static string SelectCardNumber(PokemonCard card)
    {
        if (!string.IsNullOrWhiteSpace(card.CardNumber))
        {
            return card.CardNumber;
        }

        return card.Number;
    }

    private static string BuildDetails(PokemonCard card)
    {
        List<string> details = new();

        if (card.Prices?.TcgPlayer?.MarketPrice.HasValue == true)
        {
            details.Add($"TCGPlayer market: {FormatUsd(card.Prices.TcgPlayer.MarketPrice.Value)}");
        }

        if (card.Prices?.TcgPlayer?.MidPrice.HasValue == true)
        {
            details.Add($"TCGPlayer mid: {FormatUsd(card.Prices.TcgPlayer.MidPrice.Value)}");
        }

        if (card.Prices?.CardMarket?.LowestNearMint.HasValue == true)
        {
            details.Add($"Cardmarket NM: {FormatUsd(ConvertToUsd(card.Prices.CardMarket.LowestNearMint.Value, card.Prices.CardMarket.Currency))}");
        }

        if (card.Types.Count > 0)
        {
            details.Add($"Types: {string.Join(", ", card.Types)}");
        }

        if (!string.IsNullOrWhiteSpace(card.Artist))
        {
            details.Add($"Artist: {card.Artist}");
        }

        return string.Join(" | ", details);
    }

    private static GradedPriceSelection SelectGradedPrice(PokemonCard card)
    {
        IEnumerable<GradedPriceSelection> prices = card.Prices?.CardMarket?.Graded
            .Select(price => new GradedPriceSelection(
                price.Price.HasValue
                    ? ConvertToUsd(price.Price.Value, card.Prices.CardMarket.Currency)
                    : null,
                FormatGradedLabel(price.Company, price.Grade),
                price.Company,
                price.Grade))
            .Where(price => price.Price.HasValue)
            ?? Enumerable.Empty<GradedPriceSelection>();

        return prices
            .OrderByDescending(price => price.Price)
            .ThenBy(price => CompanySortOrder(price.Company))
            .ThenBy(price => price.Grade)
            .FirstOrDefault();
    }

    private static decimal? SelectRawDisplayPrice(PokemonCard card)
    {
        decimal? tcgPlayerPrice = card.TcgPlayer?.Prices.Values
            .Select(price => price.Market ?? price.Mid ?? price.Low)
            .FirstOrDefault(price => price.HasValue);

        return tcgPlayerPrice
            ?? card.Prices?.TcgPlayer?.MarketPrice
            ?? card.Prices?.TcgPlayer?.MidPrice
            ?? ConvertNullableToUsd(card.Prices?.CardMarket?.LowestNearMint, card.Prices?.CardMarket?.Currency)
            ?? ConvertNullableToUsd(card.Prices?.CardMarket?.Average30d, card.Prices?.CardMarket?.Currency)
            ?? ConvertNullableToUsd(card.Prices?.CardMarket?.Average7d, card.Prices?.CardMarket?.Currency)
            ?? card.CardMarket?.Prices?.AverageSellPrice
            ?? card.MarketPrice
            ?? card.Price;
    }

    private static int CountPriceVariants(PokemonCard card)
    {
        int gradedCount = card.Prices?.CardMarket?.Graded.Count ?? 0;

        return gradedCount > 0 ? gradedCount : card.TcgPlayer?.Prices.Count ?? 0;
    }

    private static string FormatGradedLabel(string company, string grade)
    {
        return $"{FormatCompany(company)} {FormatGrade(grade)}";
    }

    private static string FormatCompany(string company)
    {
        return company.ToLowerInvariant() switch
        {
            "psa" => "PSA",
            "bgs" => "BGS",
            "beckett" => "BGS",
            "cgc" => "CGC",
            _ => company.ToUpperInvariant()
        };
    }

    private static string FormatGrade(string grade)
    {
        string companyPrefix = new(grade.TakeWhile(char.IsLetter).ToArray());
        string numericGrade = new(grade.SkipWhile(char.IsLetter).ToArray());

        return string.IsNullOrWhiteSpace(numericGrade) ? grade.ToUpperInvariant() : numericGrade;
    }

    private static int CompanySortOrder(string company)
    {
        return company.ToLowerInvariant() switch
        {
            "psa" => 0,
            "bgs" => 1,
            "beckett" => 1,
            "cgc" => 2,
            _ => 3
        };
    }

    private static string FormatUsd(decimal value)
    {
        return $"${value:0.00}";
    }

    private static decimal? ConvertNullableToUsd(decimal? value, string? currency)
    {
        return value.HasValue ? ConvertToUsd(value.Value, currency) : null;
    }

    private static decimal ConvertToUsd(decimal value, string? currency)
    {
        return currency?.Equals("EUR", StringComparison.OrdinalIgnoreCase) == true
            ? decimal.Round(value * EuroToUsdRate, 2)
            : value;
    }

    private readonly record struct GradedPriceSelection(decimal? Price, string Label, string Company, string Grade);
}
