
// <summary>
// This model is for information about the API key owners plan.
// This keeps track of request information when a api response comes in.
// </summary>
public class Metadata
{
    public string ApiPlan { get; set; }                     // Name of your current API subscription plan
    public int ApiRequestLimit { get; set; }                // Total number of API requests allowed for your plan
    public int ApiRequestsUsed { get; set; }               // Number of API requests used so far
    public int ApiRequestsRemaining { get; set; }          // Number of requests remaining before reaching total limit (from API)

    public int ApiDailyLimit { get; set; }                 // Maximum number of API requests allowed per day
    public int ApiDailyRequestsUsed { get; set; }         // Number of API requests used today
    public int ApiDailyRequestsRemaining { get; set; }    // Number of API requests remaining for today (from API)

    public int ApiRateLimit { get; set; }                  // Maximum number of requests allowed per minute
}

