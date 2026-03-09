/// <summary>
/// Represents a collectible card game set, including metadata such as the game it belongs to,
/// the number of cards, variants, sealed products, release date, and market value changes.
/// This class is designed to model the API response for a set.
/// </summary>
public class Set
{
    /// <summary>
    /// Unique identifier for the set.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Name of the set.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// ID of the game this set belongs to.
    /// </summary>
    public string GameId { get; set; }

    /// <summary>
    /// Name of the game this set belongs to.
    /// </summary>
    public string Game { get; set; }

    /// <summary>
    /// Number of cards in the set.
    /// </summary>
    public int CardsCount { get; set; }

    /// <summary>
    /// Number of variants in the set.
    /// </summary>
    public int VariantsCount { get; set; }

    /// <summary>
    /// Number of sealed products in the set.
    /// </summary>
    public int SealedCount { get; set; }

    /// <summary>
    /// Release date of the set in ISO 8601 format.
    /// </summary>
    public string ReleaseDate { get; set; }

    /// <summary>
    /// Total value of all cards in the set in USD.
    /// Uses the highest price of any variant for each card.
    /// </summary>
    public float SetValueUsd { get; set; }

    /// <summary>
    /// Percentage change in set value over the last 7 days.
    /// </summary>
    public float SetValueChange7dPct { get; set; }

    /// <summary>
    /// Percentage change in set value over the last 30 days.
    /// </summary>
    public float SetValueChange30dPct { get; set; }

    /// <summary>
    /// Percentage change in set value over the last 90 days.
    /// </summary>
    public float SetValueChange90dPct { get; set; }
}