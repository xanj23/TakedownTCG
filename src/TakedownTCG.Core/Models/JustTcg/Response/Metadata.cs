using System.Text.Json.Serialization;

namespace TakedownTCG.Core.Models.JustTcg.Response
{
    /// <summary>
    /// Represents API plan and rate-limit metadata returned by JustTCG.
    /// </summary>
    public class Metadata
    {
        /// <summary>
        /// Gets or sets the name of the active API plan.
        /// </summary>
        [JsonPropertyName("api_plan")]
        public string ApiPlan { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the total request limit for the active plan.
        /// </summary>
        [JsonPropertyName("api_request_limit")]
        public int ApiRequestLimit { get; set; }

        /// <summary>
        /// Gets or sets the number of requests used against the total plan limit.
        /// </summary>
        [JsonPropertyName("api_requests_used")]
        public int ApiRequestsUsed { get; set; }

        /// <summary>
        /// Gets or sets the remaining requests under the total plan limit.
        /// </summary>
        [JsonPropertyName("api_requests_remaining")]
        public int ApiRequestsRemaining { get; set; }

        /// <summary>
        /// Gets or sets the daily request limit for the active plan.
        /// </summary>
        [JsonPropertyName("api_daily_limit")]
        public int ApiDailyLimit { get; set; }

        /// <summary>
        /// Gets or sets the number of API requests used today.
        /// </summary>
        [JsonPropertyName("api_daily_requests_used")]
        public int ApiDailyRequestsUsed { get; set; }

        /// <summary>
        /// Gets or sets the number of API requests remaining today.
        /// </summary>
        [JsonPropertyName("api_daily_requests_remaining")]
        public int ApiDailyRequestsRemaining { get; set; }

        /// <summary>
        /// Gets or sets the per-minute rate limit returned by the API.
        /// </summary>
        [JsonPropertyName("api_rate_limit")]
        public int ApiRateLimit { get; set; }
    }
}

