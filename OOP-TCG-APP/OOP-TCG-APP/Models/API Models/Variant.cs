using System;
using System.Collections.Generic;

/// <summary>
/// Represents a single price point for a card at a specific timestamp.
/// </summary>
public class PricePoint
{
    /// <summary>
    /// Price of the card at this timestamp.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Unix timestamp (seconds) when the price was recorded.
    /// </summary>
    public long Timestamp { get; set; }
}

/// <summary>
/// Represents a specific variant of a card that is for sale, including condition, printing, language,
/// current price, and detailed historical statistics (7-day, 30-day, 90-day, yearly, and all-time).
/// </summary>
public class Variant
{
    /// <summary>
    /// Unique identifier for this specific variant.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Condition of the card (e.g., "Near Mint", "Lightly Played").
    /// </summary>
    public string Condition { get; set; }

    /// <summary>
    /// Printing type of the card (e.g., "Normal", "Foil").
    /// </summary>
    public string Printing { get; set; }

    /// <summary>
    /// Language of the card (e.g., "English", "Japanese").
    /// </summary>
    public string Language { get; set; }

    /// <summary>
    /// TCGPlayer SKU ID for this variant.
    /// </summary>
    public string TcgplayerSkuId { get; set; }

    /// <summary>
    /// Current price in USD.
    /// </summary>
    public decimal? Price { get; set; }

    /// <summary>
    /// Unix timestamp of when the price was last updated.
    /// </summary>
    public long LastUpdated { get; set; }

    /// <summary>
    /// Percentage price change over the last 24 hours.
    /// </summary>
    public decimal? PriceChange24hr { get; set; }

    /// <summary>
    /// Percentage price change over the last 7 days.
    /// </summary>
    public decimal? PriceChange7d { get; set; }

    /// <summary>
    /// Average price over the last 7 days.
    /// </summary>
    public decimal? AvgPrice { get; set; }

    /// <summary>
    /// Historical price points over the last 7 days (default).
    /// </summary>
    public List<PricePoint>? PriceHistory { get; set; }

    /// <summary>
    /// Minimum price over the last 7 days.
    /// </summary>
    public decimal? MinPrice7d { get; set; }

    /// <summary>
    /// Maximum price over the last 7 days.
    /// </summary>
    public decimal? MaxPrice7d { get; set; }

    /// <summary>
    /// Population standard deviation of prices over the last 7 days.
    /// </summary>
    public decimal? StddevPopPrice7d { get; set; }

    /// <summary>
    /// Coefficient of variation (StdDev / Mean) over the last 7 days.
    /// </summary>
    public decimal? CovPrice7d { get; set; }

    /// <summary>
    /// Interquartile range (75th - 25th percentile) over the last 7 days.
    /// </summary>
    public decimal? IqrPrice7d { get; set; }

    /// <summary>
    /// Slope of linear regression trend line for the last 7 days.
    /// </summary>
    public decimal? TrendSlope7d { get; set; }

    /// <summary>
    /// Count of distinct price changes in the last 7 days.
    /// </summary>
    public int? PriceChangesCount7d { get; set; }

    /// <summary>
    /// Percentage price change over the last 30 days.
    /// </summary>
    public decimal? PriceChange30d { get; set; }

    /// <summary>
    /// Average price over the last 30 days.
    /// </summary>
    public decimal? AvgPrice30d { get; set; }

    /// <summary>
    /// [DEPRECATED] Historical price points over the last 30 days.
    /// </summary>
    public List<PricePoint>? PriceHistory30d { get; set; }

    /// <summary>
    /// Minimum price over the last 30 days.
    /// </summary>
    public decimal? MinPrice30d { get; set; }

    /// <summary>
    /// Maximum price over the last 30 days.
    /// </summary>
    public decimal? MaxPrice30d { get; set; }

    /// <summary>
    /// Population standard deviation of prices over the last 30 days.
    /// </summary>
    public decimal? StddevPopPrice30d { get; set; }

    /// <summary>
    /// Coefficient of variation over the last 30 days.
    /// </summary>
    public decimal? CovPrice30d { get; set; }

    /// <summary>
    /// Interquartile range over the last 30 days.
    /// </summary>
    public decimal? IqrPrice30d { get; set; }

    /// <summary>
    /// Trend slope over the last 30 days.
    /// </summary>
    public decimal? TrendSlope30d { get; set; }

    /// <summary>
    /// Count of distinct price changes in the last 30 days.
    /// </summary>
    public int? PriceChangesCount30d { get; set; }

    /// <summary>
    /// Position within the 30-day min/max price range (0-1).
    /// </summary>
    public decimal? PriceRelativeTo30dRange { get; set; }

    /// <summary>
    /// Percentage price change over the last 90 days.
    /// </summary>
    public decimal? PriceChange90d { get; set; }

    /// <summary>
    /// Average price over the last 90 days.
    /// </summary>
    public decimal? AvgPrice90d { get; set; }

    /// <summary>
    /// Minimum price over the last 90 days.
    /// </summary>
    public decimal? MinPrice90d { get; set; }

    /// <summary>
    /// Maximum price over the last 90 days.
    /// </summary>
    public decimal? MaxPrice90d { get; set; }

    /// <summary>
    /// Population standard deviation of prices over the last 90 days.
    /// </summary>
    public decimal? StddevPopPrice90d { get; set; }

    /// <summary>
    /// Coefficient of variation over the last 90 days.
    /// </summary>
    public decimal? CovPrice90d { get; set; }

    /// <summary>
    /// Interquartile range over the last 90 days.
    /// </summary>
    public decimal? IqrPrice90d { get; set; }

    /// <summary>
    /// Trend slope over the last 90 days.
    /// </summary>
    public decimal? TrendSlope90d { get; set; }

    /// <summary>
    /// Count of distinct price changes in the last 90 days.
    /// </summary>
    public int? PriceChangesCount90d { get; set; }

    /// <summary>
    /// Position within the 90-day min/max price range (0-1).
    /// </summary>
    public decimal? PriceRelativeTo90dRange { get; set; }

    /// <summary>
    /// Minimum price in the last year.
    /// </summary>
    public decimal? MinPrice1y { get; set; }

    /// <summary>
    /// Maximum price in the last year.
    /// </summary>
    public decimal? MaxPrice1y { get; set; }

    /// <summary>
    /// Lowest price ever recorded for this variant.
    /// </summary>
    public decimal? MinPriceAllTime { get; set; }

    /// <summary>
    /// ISO 8601 date of the all-time minimum price.
    /// </summary>
    public string? MinPriceAllTimeDate { get; set; }

    /// <summary>
    /// Highest price ever recorded for this variant.
    /// </summary>
    public decimal? MaxPriceAllTime { get; set; }

    /// <summary>
    /// ISO 8601 date of the all-time maximum price.
    /// </summary>
    public string? MaxPriceAllTimeDate { get; set; }
}