/// <summary>
/// Defines the supported JustTCG card condition filters used by the console application.
/// </summary>
public enum Condition
{
    S,
    NM,
    LP,
    MP,
    HP,
    DMG
}

/// <summary>
/// Stores the user-provided parameters for a JustTCG card search request.
/// </summary>
public class JustTCGCardQueryParams
{
    /// <summary>
    /// Gets or sets the free-text card search term.
    /// </summary>
    public string? Q { get; set; }

    /// <summary>
    /// Gets or sets the card number filter.
    /// </summary>
    public string? Number { get; set; }

    /// <summary>
    /// Gets or sets the printing filter.
    /// </summary>
    public string? Printing { get; set; }

    /// <summary>
    /// Gets or sets the condition filter.
    /// </summary>
    public Condition? Condition { get; set; }
}
