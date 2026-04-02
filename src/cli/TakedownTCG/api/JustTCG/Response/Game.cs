namespace TakedownTCG.cli.Api.JustTCG.Response
{
/// <summary>
/// Represents a trading card game summary returned by the JustTCG games endpoint.
/// </summary>
public class Game
{
    /// <summary>
    /// Gets or sets the unique game identifier.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name of the game.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of cards tracked for the game.
    /// </summary>
    public int CardsCount { get; set; }

    /// <summary>
    /// Gets or sets the number of card variants tracked for the game.
    /// </summary>
    public int VariantsCount { get; set; }

    /// <summary>
    /// Gets or sets the number of sealed products tracked for the game.
    /// </summary>
    public int SealedCount { get; set; }

    /// <summary>
    /// Gets or sets the number of sets tracked for the game.
    /// </summary>
    public int SetsCount { get; set; }

    /// <summary>
    /// Gets or sets the Unix timestamp of the last update.
    /// </summary>
    public long LastUpdated { get; set; }

    /// <summary>
    /// Gets or sets the aggregate game value index in cents.
    /// </summary>
    public int GameValueIndexCents { get; set; }

    /// <summary>
    /// Gets or sets the seven-day percentage change in game value.
    /// </summary>
    public float GameValueChange7dPct { get; set; }

    /// <summary>
    /// Gets or sets the thirty-day percentage change in game value.
    /// </summary>
    public float GameValueChange30dPct { get; set; }

    /// <summary>
    /// Gets or sets the ninety-day percentage change in game value.
    /// </summary>
    public float GameValueChange90dPct { get; set; }

    /// <summary>
    /// Gets or sets the count of cards with positive seven-day price movement.
    /// </summary>
    public int CardsPos7dCount { get; set; }

    /// <summary>
    /// Gets or sets the count of cards with negative seven-day price movement.
    /// </summary>
    public int CardsNeg7dCount { get; set; }

    /// <summary>
    /// Gets or sets the count of sealed products with positive seven-day price movement.
    /// </summary>
    public int SealedCardsPos7dCount { get; set; }

    /// <summary>
    /// Gets or sets the count of sealed products with negative seven-day price movement.
    /// </summary>
    public int SealedCardsNeg7dCount { get; set; }

    /// <summary>
    /// Gets or sets the count of cards with positive thirty-day price movement.
    /// </summary>
    public int CardsPos30dCount { get; set; }

    /// <summary>
    /// Gets or sets the count of cards with negative thirty-day price movement.
    /// </summary>
    public int CardsNeg30dCount { get; set; }

    /// <summary>
    /// Gets or sets the count of sealed products with positive thirty-day price movement.
    /// </summary>
    public int SealedCardsPos30dCount { get; set; }

    /// <summary>
    /// Gets or sets the count of sealed products with negative thirty-day price movement.
    /// </summary>
    public int SealedCardsNeg30dCount { get; set; }

    /// <summary>
    /// Gets or sets the count of cards with positive ninety-day price movement.
    /// </summary>
    public int CardsPos90dCount { get; set; }

    /// <summary>
    /// Gets or sets the count of cards with negative ninety-day price movement.
    /// </summary>
    public int CardsNeg90dCount { get; set; }

    /// <summary>
    /// Gets or sets the count of sealed products with positive ninety-day price movement.
    /// </summary>
    public int SealedCardsPos90dCount { get; set; }

    /// <summary>
    /// Gets or sets the count of sealed products with negative ninety-day price movement.
    /// </summary>
    public int SealedCardsNeg90dCount { get; set; }
}
}
