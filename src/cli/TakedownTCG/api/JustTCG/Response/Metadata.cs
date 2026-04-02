namespace TakedownTCG.cli.Api.JustTCG.Response
{
/// <summary>
/// Represents API plan and rate-limit metadata returned by JustTCG.
/// </summary>
public class Metadata
{
    /// <summary>
    /// Gets or sets the name of the active API plan.
    /// </summary>
    public string ApiPlan { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the total request limit for the active plan.
    /// </summary>
    public int ApiRequestLimit { get; set; }

    /// <summary>
    /// Gets or sets the number of requests used against the total plan limit.
    /// </summary>
    public int ApiRequestsUsed { get; set; }

    /// <summary>
    /// Gets or sets the remaining requests under the total plan limit.
    /// </summary>
    public int ApiRequestsRemaining { get; set; }

    /// <summary>
    /// Gets or sets the daily request limit for the active plan.
    /// </summary>
    public int ApiDailyLimit { get; set; }

    /// <summary>
    /// Gets or sets the number of API requests used today.
    /// </summary>
    public int ApiDailyRequestsUsed { get; set; }

    /// <summary>
    /// Gets or sets the number of API requests remaining today.
    /// </summary>
    public int ApiDailyRequestsRemaining { get; set; }

    /// <summary>
    /// Gets or sets the per-minute rate limit returned by the API.
    /// </summary>
    public int ApiRateLimit { get; set; }
}
}
