namespace JustTCG
{
    /// <summary>
    /// Defines the supported sort fields for a JustTCG set search request.
    /// </summary>
    public enum OrderBy
    {
        name,
        release_date
    }

    /// <summary>
    /// Defines the supported sort directions for a JustTCG set search request.
    /// </summary>
    public enum Order
    {
        asc,
        desc
    }

    /// <summary>
    /// Stores the user-provided parameters for a JustTCG set search request.
    /// </summary>
    public class SetQueryParams
    {
        /// <summary>
        /// Gets or sets the optional free-text set search term.
        /// </summary>
        public string? Q { get; set; }

        /// <summary>
        /// Gets or sets the required game filter.
        /// </summary>
        public string? Game { get; set; }

        /// <summary>
        /// Gets or sets the field used to order the returned sets.
        /// </summary>
        public OrderBy? OrderBy { get; set; }

        /// <summary>
        /// Gets or sets the direction used to order the returned sets.
        /// </summary>
        public Order? Order { get; set; }
    }

}