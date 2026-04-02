namespace TakedownTCG.cli.Api.JustTCG.Response
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Wraps the standard JustTCG API response envelope for a collection endpoint.
    /// </summary>
    /// <typeparam name="T">The type stored in the response data collection.</typeparam>
    public class Response<T>
    {
        /// <summary>
        /// Gets or sets the main data payload returned by the endpoint.
        /// </summary>
        [JsonPropertyName("data")]
        public List<T> Data { get; set; } = new List<T>();

        /// <summary>
        /// Gets or sets the pagination metadata returned by the API.
        /// </summary>
        [JsonPropertyName("meta")]
        public Meta Meta { get; set; } = new Meta();

        /// <summary>
        /// Gets or sets the API usage metadata associated with the current request.
        /// </summary>
        [JsonPropertyName("_metadata")]
        public Metadata _metadata { get; set; } = new Metadata();

        /// <summary>
        /// Gets or sets the error message returned by the API when a request fails.
        /// </summary>
        [JsonPropertyName("error")]
        public string? Error { get; set; }

        /// <summary>
        /// Gets or sets the API error code returned with a failed request.
        /// </summary>
        [JsonPropertyName("code")]
        public string? Code { get; set; }
    }
}
