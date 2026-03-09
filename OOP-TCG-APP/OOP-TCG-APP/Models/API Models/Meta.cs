/// <summary>
/// Represents metadata for paginated API responses.  
/// Tracks the total results, page size, current offset, and whether more pages are available.
/// </summary>
public class Meta
{
    /// <summary>
    /// Total number of cards matching the search prompt.
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// Number of results on the current page.
    /// </summary>
    public int Limit { get; set; }

    /// <summary>
    /// Current offset in the dataset (used for pagination).
    /// </summary>
    public int Offset { get; set; }

    /// <summary>
    /// Indicates whether there are more results available beyond the current page.
    /// </summary>
    public bool HasMore { get; set; }
}