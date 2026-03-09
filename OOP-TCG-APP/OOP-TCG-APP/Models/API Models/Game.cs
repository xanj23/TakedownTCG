using System;

/// <summary>
/// Represents a trading card game in the JustTCG database.
/// This class models the API response for the /games endpoint.
/// </summary>
public class Game
{
    /// <summary>
    /// Unique identifier for the game
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Name of the game
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Number of cards in the game
    /// </summary>
    public int CardsCount { get; set; }

    /// <summary>
    /// Number of variants in the game
    /// </summary>
    public int VariantsCount { get; set; }

    /// <summary>
    /// Number of sealed products in the game
    /// </summary>
    public int SealedCount { get; set; }

    /// <summary>
    /// Number of sets in the game
    /// </summary>
    public int SetsCount { get; set; }

    /// <summary>
    /// Timestamp of the last update to the game data (seconds since Unix epoch)
    /// </summary>
    public long LastUpdated { get; set; }

    /// <summary>
    /// Total value of all cards in the game in cents
    /// Uses the highest price of any variant for each card
    /// </summary>
    public int GameValueIndexCents { get; set; }

    /// <summary>
    /// Percentage change in game value index over the last 7 days
    /// </summary>
    public float GameValueChange7dPct { get; set; }

    /// <summary>
    /// Percentage change in game value index over the last 30 days
    /// </summary>
    public float GameValueChange30dPct { get; set; }

    /// <summary>
    /// Percentage change in game value index over the last 90 days
    /// </summary>
    public float GameValueChange90dPct { get; set; }

    /// <summary>
    /// Number of cards with positive price change in the last 7 days
    /// </summary>
    public int CardsPos7dCount { get; set; }

    /// <summary>
    /// Number of cards with negative price change in the last 7 days
    /// </summary>
    public int CardsNeg7dCount { get; set; }

    /// <summary>
    /// Number of sealed products with positive price change in the last 7 days
    /// </summary>
    public int SealedCardsPos7dCount { get; set; }

    /// <summary>
    /// Number of sealed products with negative price change in the last 7 days
    /// </summary>
    public int SealedCardsNeg7dCount { get; set; }

    /// <summary>
    /// Number of cards with positive price change in the last 30 days
    /// </summary>
    public int CardsPos30dCount { get; set; }

    /// <summary>
    /// Number of cards with negative price change in the last 30 days
    /// </summary>
    public int CardsNeg30dCount { get; set; }

    /// <summary>
    /// Number of sealed products with positive price change in the last 30 days
    /// </summary>
    public int SealedCardsPos30dCount { get; set; }

    /// <summary>
    /// Number of sealed products with negative price change in the last 30 days
    /// </summary>
    public int SealedCardsNeg30dCount { get; set; }

    /// <summary>
    /// Number of cards with positive price change in the last 90 days
    /// </summary>
    public int CardsPos90dCount { get; set; }

    /// <summary>
    /// Number of cards with negative price change in the last 90 days
    /// </summary>
    public int CardsNeg90dCount { get; set; }

    /// <summary>
    /// Number of sealed products with positive price change in the last 90 days
    /// </summary>
    public int SealedCardsPos90dCount { get; set; }

    /// <summary>
    /// Number of sealed products with negative price change in the last 90 days
    /// </summary>
    public int SealedCardsNeg90dCount { get; set; }
}