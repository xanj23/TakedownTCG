/// <summary>
/// Represents information about the API key owner's subscription plan and usage.
/// Tracks total request limits, daily usage, and rate limits returned by the API.
/// </summary>
public class Metadata
{
    /// <summary>
    /// Name of the current API subscription plan.
    /// </summary>
    public string ApiPlan { get; set; }

    /// <summary>
    /// Total number of API requests allowed for the plan.
    /// </summary>
    public int ApiRequestLimit { get; set; }

    /// <summary>
    /// Number of API requests used so far.
    /// </summary>
    public int ApiRequestsUsed { get; set; }

    /// <summary>
    /// Number of API requests remaining before reaching the total limit (from API).
    /// </summary>
    public int ApiRequestsRemaining { get; set; }

    /// <summary>
    /// Maximum number of API requests allowed per day.
    /// </summary>
    public int ApiDailyLimit { get; set; }

    /// <summary>
    /// Number of API requests used today.
    /// </summary>
    public int ApiDailyRequestsUsed { get; set; }

    /// <summary>
    /// Number of API requests remaining for today (from API).
    /// </summary>
    public int ApiDailyRequestsRemaining { get; set; }

    /// <summary>
    /// Maximum number of requests allowed per minute (rate limit).
    /// </summary>
    public int ApiRateLimit { get; set; }
}