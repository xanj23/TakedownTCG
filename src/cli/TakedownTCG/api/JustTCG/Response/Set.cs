/// <summary>
/// Represents a set record returned by the JustTCG sets endpoint.
/// </summary>
public class Set
{
    /// <summary>
    /// Gets or sets the unique set identifier.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the set name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the owning game identifier.
    /// </summary>
    public string GameId { get; set; }

    /// <summary>
    /// Gets or sets the owning game name.
    /// </summary>
    public string Game { get; set; }

    /// <summary>
    /// Gets or sets the number of cards in the set.
    /// </summary>
    public int CardsCount { get; set; }

    /// <summary>
    /// Gets or sets the number of variants in the set.
    /// </summary>
    public int VariantsCount { get; set; }

    /// <summary>
    /// Gets or sets the number of sealed products in the set.
    /// </summary>
    public int SealedCount { get; set; }

    /// <summary>
    /// Gets or sets the release date string returned by the API.
    /// </summary>
    public string ReleaseDate { get; set; }

    /// <summary>
    /// Gets or sets the aggregate set value in USD.
    /// </summary>
    public float SetValueUsd { get; set; }

    /// <summary>
    /// Gets or sets the seven-day percentage change in set value.
    /// </summary>
    public float SetValueChange7dPct { get; set; }

    /// <summary>
    /// Gets or sets the thirty-day percentage change in set value.
    /// </summary>
    public float SetValueChange30dPct { get; set; }

    /// <summary>
    /// Gets or sets the ninety-day percentage change in set value.
    /// </summary>
    public float SetValueChange90dPct { get; set; }
}
