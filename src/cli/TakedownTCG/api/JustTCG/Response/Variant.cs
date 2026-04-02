namespace TakedownTCG.cli.Api.JustTCG.Response
{
    /// <summary>
    /// Represents a historical price point for a card variant.
    /// </summary>
    public class PricePoint
    {
        /// <summary>
        /// Gets or sets the recorded price.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the Unix timestamp at which the price was recorded.
        /// </summary>
        public long Timestamp { get; set; }
    }

    /// <summary>
    /// Represents a specific purchasable card variant returned by the JustTCG API.
    /// </summary>
    public class Variant
    {
        /// <summary>
        /// Gets or sets the unique variant identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the reported card condition.
        /// </summary>
        public string Condition { get; set; }

        /// <summary>
        /// Gets or sets the printing type.
        /// </summary>
        public string Printing { get; set; }

        /// <summary>
        /// Gets or sets the language of the variant.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the linked TCGplayer SKU identifier.
        /// </summary>
        public string TcgplayerSkuId { get; set; }

        /// <summary>
        /// Gets or sets the current price.
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// Gets or sets the Unix timestamp of the last price update.
        /// </summary>
        public long LastUpdated { get; set; }

        /// <summary>
        /// Gets or sets the twenty-four-hour price change percentage.
        /// </summary>
        public decimal? PriceChange24hr { get; set; }

        /// <summary>
        /// Gets or sets the seven-day price change percentage.
        /// </summary>
        public decimal? PriceChange7d { get; set; }

        /// <summary>
        /// Gets or sets the seven-day average price.
        /// </summary>
        public decimal? AvgPrice { get; set; }

        /// <summary>
        /// Gets or sets the default price history collection.
        /// </summary>
        public List<PricePoint>? PriceHistory { get; set; }

        /// <summary>
        /// Gets or sets the minimum seven-day price.
        /// </summary>
        public decimal? MinPrice7d { get; set; }

        /// <summary>
        /// Gets or sets the maximum seven-day price.
        /// </summary>
        public decimal? MaxPrice7d { get; set; }

        /// <summary>
        /// Gets or sets the seven-day population standard deviation.
        /// </summary>
        public decimal? StddevPopPrice7d { get; set; }

        /// <summary>
        /// Gets or sets the seven-day coefficient of variation.
        /// </summary>
        public decimal? CovPrice7d { get; set; }

        /// <summary>
        /// Gets or sets the seven-day interquartile range.
        /// </summary>
        public decimal? IqrPrice7d { get; set; }

        /// <summary>
        /// Gets or sets the seven-day trend slope.
        /// </summary>
        public decimal? TrendSlope7d { get; set; }

        /// <summary>
        /// Gets or sets the number of distinct seven-day price changes.
        /// </summary>
        public int? PriceChangesCount7d { get; set; }

        /// <summary>
        /// Gets or sets the thirty-day price change percentage.
        /// </summary>
        public decimal? PriceChange30d { get; set; }

        /// <summary>
        /// Gets or sets the thirty-day average price.
        /// </summary>
        public decimal? AvgPrice30d { get; set; }

        /// <summary>
        /// Gets or sets the deprecated thirty-day price history collection.
        /// </summary>
        public List<PricePoint>? PriceHistory30d { get; set; }

        /// <summary>
        /// Gets or sets the minimum thirty-day price.
        /// </summary>
        public decimal? MinPrice30d { get; set; }

        /// <summary>
        /// Gets or sets the maximum thirty-day price.
        /// </summary>
        public decimal? MaxPrice30d { get; set; }

        /// <summary>
        /// Gets or sets the thirty-day population standard deviation.
        /// </summary>
        public decimal? StddevPopPrice30d { get; set; }

        /// <summary>
        /// Gets or sets the thirty-day coefficient of variation.
        /// </summary>
        public decimal? CovPrice30d { get; set; }

        /// <summary>
        /// Gets or sets the thirty-day interquartile range.
        /// </summary>
        public decimal? IqrPrice30d { get; set; }

        /// <summary>
        /// Gets or sets the thirty-day trend slope.
        /// </summary>
        public decimal? TrendSlope30d { get; set; }

        /// <summary>
        /// Gets or sets the number of distinct thirty-day price changes.
        /// </summary>
        public int? PriceChangesCount30d { get; set; }

        /// <summary>
        /// Gets or sets the current price position within the thirty-day range.
        /// </summary>
        public decimal? PriceRelativeTo30dRange { get; set; }

        /// <summary>
        /// Gets or sets the ninety-day price change percentage.
        /// </summary>
        public decimal? PriceChange90d { get; set; }

        /// <summary>
        /// Gets or sets the ninety-day average price.
        /// </summary>
        public decimal? AvgPrice90d { get; set; }

        /// <summary>
        /// Gets or sets the minimum ninety-day price.
        /// </summary>
        public decimal? MinPrice90d { get; set; }

        /// <summary>
        /// Gets or sets the maximum ninety-day price.
        /// </summary>
        public decimal? MaxPrice90d { get; set; }

        /// <summary>
        /// Gets or sets the ninety-day population standard deviation.
        /// </summary>
        public decimal? StddevPopPrice90d { get; set; }

        /// <summary>
        /// Gets or sets the ninety-day coefficient of variation.
        /// </summary>
        public decimal? CovPrice90d { get; set; }

        /// <summary>
        /// Gets or sets the ninety-day interquartile range.
        /// </summary>
        public decimal? IqrPrice90d { get; set; }

        /// <summary>
        /// Gets or sets the ninety-day trend slope.
        /// </summary>
        public decimal? TrendSlope90d { get; set; }

        /// <summary>
        /// Gets or sets the number of distinct ninety-day price changes.
        /// </summary>
        public int? PriceChangesCount90d { get; set; }

        /// <summary>
        /// Gets or sets the current price position within the ninety-day range.
        /// </summary>
        public decimal? PriceRelativeTo90dRange { get; set; }

        /// <summary>
        /// Gets or sets the minimum price observed in the last year.
        /// </summary>
        public decimal? MinPrice1y { get; set; }

        /// <summary>
        /// Gets or sets the maximum price observed in the last year.
        /// </summary>
        public decimal? MaxPrice1y { get; set; }

        /// <summary>
        /// Gets or sets the all-time minimum price.
        /// </summary>
        public decimal? MinPriceAllTime { get; set; }

        /// <summary>
        /// Gets or sets the date of the all-time minimum price.
        /// </summary>
        public string? MinPriceAllTimeDate { get; set; }

        /// <summary>
        /// Gets or sets the all-time maximum price.
        /// </summary>
        public decimal? MaxPriceAllTime { get; set; }

        /// <summary>
        /// Gets or sets the date of the all-time maximum price.
        /// </summary>
        public string? MaxPriceAllTimeDate { get; set; }
    }
}
