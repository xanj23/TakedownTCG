using System.Text.Json.Serialization;

namespace TakedownTCG.Core.Models.JustTcg.Response
{
    /// <summary>
    /// Represents a historical price point for a card variant.
    /// </summary>
    public class PricePoint
    {
        /// <summary>
        /// Gets or sets the recorded price.
        /// </summary>
        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the Unix timestamp at which the price was recorded.
        /// </summary>
        [JsonPropertyName("timestamp")]
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
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the reported card condition.
        /// </summary>
        [JsonPropertyName("condition")]
        public string Condition { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the printing type.
        /// </summary>
        [JsonPropertyName("printing")]
        public string Printing { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the language of the variant.
        /// </summary>
        [JsonPropertyName("language")]
        public string Language { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the linked TCGplayer SKU identifier.
        /// </summary>
        [JsonPropertyName("tcgplayerSkuId")]
        public string TcgplayerSkuId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the current price.
        /// </summary>
        [JsonPropertyName("price")]
        public decimal? Price { get; set; }

        /// <summary>
        /// Gets or sets the Unix timestamp of the last price update.
        /// </summary>
        [JsonPropertyName("last_updated")]
        public string LastUpdated { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the twenty-four-hour price change percentage.
        /// </summary>
        [JsonPropertyName("price_change_24hr")]
        public decimal? PriceChange24hr { get; set; }

        /// <summary>
        /// Gets or sets the seven-day price change percentage.
        /// </summary>
        [JsonPropertyName("price_change_7d")]
        public decimal? PriceChange7d { get; set; }

        /// <summary>
        /// Gets or sets the seven-day average price.
        /// </summary>
        [JsonPropertyName("avg_price")]
        public decimal? AvgPrice { get; set; }

        /// <summary>
        /// Gets or sets the default price history collection.
        /// </summary>
        [JsonPropertyName("price_history")]
        public List<PricePoint>? PriceHistory { get; set; }

        /// <summary>
        /// Gets or sets the minimum seven-day price.
        /// </summary>
        [JsonPropertyName("min_price_7d")]
        public decimal? MinPrice7d { get; set; }

        /// <summary>
        /// Gets or sets the maximum seven-day price.
        /// </summary>
        [JsonPropertyName("max_price_7d")]
        public decimal? MaxPrice7d { get; set; }

        /// <summary>
        /// Gets or sets the seven-day population standard deviation.
        /// </summary>
        [JsonPropertyName("stddev_pop_price_7d")]
        public decimal? StddevPopPrice7d { get; set; }

        /// <summary>
        /// Gets or sets the seven-day coefficient of variation.
        /// </summary>
        [JsonPropertyName("cov_price_7d")]
        public decimal? CovPrice7d { get; set; }

        /// <summary>
        /// Gets or sets the seven-day interquartile range.
        /// </summary>
        [JsonPropertyName("iqr_price_7d")]
        public decimal? IqrPrice7d { get; set; }

        /// <summary>
        /// Gets or sets the seven-day trend slope.
        /// </summary>
        [JsonPropertyName("trend_slope_7d")]
        public decimal? TrendSlope7d { get; set; }

        /// <summary>
        /// Gets or sets the number of distinct seven-day price changes.
        /// </summary>
        [JsonPropertyName("price_changes_count_7d")]
        public int? PriceChangesCount7d { get; set; }

        /// <summary>
        /// Gets or sets the thirty-day price change percentage.
        /// </summary>
        [JsonPropertyName("price_change_30d")]
        public decimal? PriceChange30d { get; set; }

        /// <summary>
        /// Gets or sets the thirty-day average price.
        /// </summary>
        [JsonPropertyName("avg_price_30d")]
        public decimal? AvgPrice30d { get; set; }

        /// <summary>
        /// Gets or sets the deprecated thirty-day price history collection.
        /// </summary>
        [JsonPropertyName("price_history_30d")]
        public List<PricePoint>? PriceHistory30d { get; set; }

        /// <summary>
        /// Gets or sets the minimum thirty-day price.
        /// </summary>
        [JsonPropertyName("min_price_30d")]
        public decimal? MinPrice30d { get; set; }

        /// <summary>
        /// Gets or sets the maximum thirty-day price.
        /// </summary>
        [JsonPropertyName("max_price_30d")]
        public decimal? MaxPrice30d { get; set; }

        /// <summary>
        /// Gets or sets the thirty-day population standard deviation.
        /// </summary>
        [JsonPropertyName("stddev_pop_price_30d")]
        public decimal? StddevPopPrice30d { get; set; }

        /// <summary>
        /// Gets or sets the thirty-day coefficient of variation.
        /// </summary>
        [JsonPropertyName("cov_price_30d")]
        public decimal? CovPrice30d { get; set; }

        /// <summary>
        /// Gets or sets the thirty-day interquartile range.
        /// </summary>
        [JsonPropertyName("iqr_price_30d")]
        public decimal? IqrPrice30d { get; set; }

        /// <summary>
        /// Gets or sets the thirty-day trend slope.
        /// </summary>
        [JsonPropertyName("trend_slope_30d")]
        public decimal? TrendSlope30d { get; set; }

        /// <summary>
        /// Gets or sets the number of distinct thirty-day price changes.
        /// </summary>
        [JsonPropertyName("price_changes_count_30d")]
        public int? PriceChangesCount30d { get; set; }

        /// <summary>
        /// Gets or sets the current price position within the thirty-day range.
        /// </summary>
        [JsonPropertyName("price_relative_to_30d_range")]
        public decimal? PriceRelativeTo30dRange { get; set; }

        /// <summary>
        /// Gets or sets the ninety-day price change percentage.
        /// </summary>
        [JsonPropertyName("price_change_90d")]
        public decimal? PriceChange90d { get; set; }

        /// <summary>
        /// Gets or sets the ninety-day average price.
        /// </summary>
        [JsonPropertyName("avg_price_90d")]
        public decimal? AvgPrice90d { get; set; }

        /// <summary>
        /// Gets or sets the minimum ninety-day price.
        /// </summary>
        [JsonPropertyName("min_price_90d")]
        public decimal? MinPrice90d { get; set; }

        /// <summary>
        /// Gets or sets the maximum ninety-day price.
        /// </summary>
        [JsonPropertyName("maxPrice90d")]
        public decimal? MaxPrice90d { get; set; }

        [JsonPropertyName("max_price_90d")]
        public decimal? MaxPrice90dLegacy
        {
            set => MaxPrice90d = value;
        }

        /// <summary>
        /// Gets or sets the ninety-day population standard deviation.
        /// </summary>
        [JsonPropertyName("stddev_pop_price_90d")]
        public decimal? StddevPopPrice90d { get; set; }

        /// <summary>
        /// Gets or sets the ninety-day coefficient of variation.
        /// </summary>
        [JsonPropertyName("cov_price_90d")]
        public decimal? CovPrice90d { get; set; }

        /// <summary>
        /// Gets or sets the ninety-day interquartile range.
        /// </summary>
        [JsonPropertyName("iqr_price_90d")]
        public decimal? IqrPrice90d { get; set; }

        /// <summary>
        /// Gets or sets the ninety-day trend slope.
        /// </summary>
        [JsonPropertyName("trend_slope_90d")]
        public decimal? TrendSlope90d { get; set; }

        /// <summary>
        /// Gets or sets the number of distinct ninety-day price changes.
        /// </summary>
        [JsonPropertyName("price_changes_count_90d")]
        public int? PriceChangesCount90d { get; set; }

        /// <summary>
        /// Gets or sets the current price position within the ninety-day range.
        /// </summary>
        [JsonPropertyName("price_relative_to_90d_range")]
        public decimal? PriceRelativeTo90dRange { get; set; }

        /// <summary>
        /// Gets or sets the minimum price observed in the last year.
        /// </summary>
        [JsonPropertyName("min_price_1y")]
        public decimal? MinPrice1y { get; set; }

        /// <summary>
        /// Gets or sets the maximum price observed in the last year.
        /// </summary>
        [JsonPropertyName("max_price_1y")]
        public decimal? MaxPrice1y { get; set; }

        /// <summary>
        /// Gets or sets the all-time minimum price.
        /// </summary>
        [JsonPropertyName("min_price_all_time")]
        public decimal? MinPriceAllTime { get; set; }

        /// <summary>
        /// Gets or sets the date of the all-time minimum price.
        /// </summary>
        [JsonPropertyName("min_price_all_time_date")]
        public string? MinPriceAllTimeDate { get; set; }

        /// <summary>
        /// Gets or sets the all-time maximum price.
        /// </summary>
        [JsonPropertyName("max_price_all_time")]
        public decimal? MaxPriceAllTime { get; set; }

        /// <summary>
        /// Gets or sets the date of the all-time maximum price.
        /// </summary>
        [JsonPropertyName("max_price_all_time_date")]
        public string? MaxPriceAllTimeDate { get; set; }
    }
}

