using System;
using System.Collections.Generic; // For list

// <summary>
// Small class for managing the price of the card at certain points of times.
// </summary>
public class PricePoint
{
    public decimal Price { get; set; }   // Price at this timestamp
    public long Timestamp { get; set; }  // Unix timestamp (seconds)
}

// <summary>
// This is for card data on cards that are for sale. This shows price and different conditions
// of them being sold.
// </summary>
public class Variant
{
    public string Id { get; set; }                 // Unique identifier for this specific variant (variantId)
    public string Condition { get; set; }         // Condition of the card (e.g., "Near Mint", "Lightly Played")
    public string Printing { get; set; }          // Printing type (e.g., "Normal", "Foil")
    public string Language { get; set; }          // Language of the card (e.g., "English", "Japanese")
    public string TcgplayerSkuId { get; set; }    // TCGPlayer SKU ID for this variant
    public decimal? Price { get; set; }           // Current price in USD
    public long LastUpdated { get; set; }         // Unix timestamp of when the price was last updated

    // Price changes and averages
    public decimal? PriceChange24hr { get; set; }       // Percentage price change over last 24 hours
    public decimal? PriceChange7d { get; set; }         // Percentage price change over last 7 days
    public decimal? AvgPrice { get; set; }             // Average price over last 7 days
    public List<PricePoint>? PriceHistory { get; set; } // Array of historical price points over 7 days by default
    public decimal? MinPrice7d { get; set; }           // Minimum price in last 7 days
    public decimal? MaxPrice7d { get; set; }           // Maximum price in last 7 days
    public decimal? StddevPopPrice7d { get; set; }     // Population standard deviation of prices over last 7 days
    public decimal? CovPrice7d { get; set; }           // Coefficient of variation (StdDev / Mean)
    public decimal? IqrPrice7d { get; set; }           // Interquartile range (75th - 25th percentile)
    public decimal? TrendSlope7d { get; set; }         // Slope of linear regression trend line for last 7 days
    public int? PriceChangesCount7d { get; set; }      // Count of distinct price changes in last 7 days

    // 30-day statistics
    public decimal? PriceChange30d { get; set; }       // Percentage price change over last 30 days
    public decimal? AvgPrice30d { get; set; }          // Average price over last 30 days
    public List<PricePoint>? PriceHistory30d { get; set; } // [DEPRECATED] historical price points over 30 days
    public decimal? MinPrice30d { get; set; }          // Minimum price in last 30 days
    public decimal? MaxPrice30d { get; set; }          // Maximum price in last 30 days
    public decimal? StddevPopPrice30d { get; set; }    // Population std dev of prices over last 30 days
    public decimal? CovPrice30d { get; set; }          // Coefficient of variation for last 30 days
    public decimal? IqrPrice30d { get; set; }          // Interquartile range for last 30 days
    public decimal? TrendSlope30d { get; set; }        // Trend slope over last 30 days
    public int? PriceChangesCount30d { get; set; }     // Count of distinct price changes last 30 days
    public decimal? PriceRelativeTo30dRange { get; set; } // Position within 30-day min/max range (0-1)

    // 90-day statistics
    public decimal? PriceChange90d { get; set; }       // Percentage price change over last 90 days
    public decimal? AvgPrice90d { get; set; }          // Average price over last 90 days
    public decimal? MinPrice90d { get; set; }          // Minimum price in last 90 days
    public decimal? MaxPrice90d { get; set; }          // Maximum price in last 90 days
    public decimal? StddevPopPrice90d { get; set; }    // Population std dev of prices over last 90 days
    public decimal? CovPrice90d { get; set; }          // Coefficient of variation over last 90 days
    public decimal? IqrPrice90d { get; set; }          // Interquartile range over last 90 days
    public decimal? TrendSlope90d { get; set; }        // Trend slope over last 90 days
    public int? PriceChangesCount90d { get; set; }     // Count of distinct price changes last 90 days
    public decimal? PriceRelativeTo90dRange { get; set; } // Position within 90-day min/max range (0-1)

    // Yearly / all-time statistics
    public decimal? MinPrice1y { get; set; }           // Minimum price in last year
    public decimal? MaxPrice1y { get; set; }           // Maximum price in last year
    public decimal? MinPriceAllTime { get; set; }      // Lowest price ever recorded
    public string? MinPriceAllTimeDate { get; set; }   // ISO 8601 date of all-time minimum
    public decimal? MaxPriceAllTime { get; set; }      // Highest price ever recorded
    public string? MaxPriceAllTimeDate { get; set; }   // ISO 8601 date of all-time maximum
}
