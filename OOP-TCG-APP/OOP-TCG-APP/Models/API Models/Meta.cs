/// <summary>
/// Represents pagination metadata returned with a JustTCG collection response.
/// </summary>
public class Meta
{
    /// <summary>
    /// Gets or sets the total number of matching records.
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// Gets or sets the number of results returned in the current page.
    /// </summary>
    public int Limit { get; set; }

    /// <summary>
    /// Gets or sets the zero-based offset for the current page.
    /// </summary>
    public int Offset { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether another page of results is available.
    /// </summary>
    public bool HasMore { get; set; }
}
