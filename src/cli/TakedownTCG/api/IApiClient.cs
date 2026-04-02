namespace TakedownTCG.cli.Api
{
    /// <summary>
    /// Defines the common contract for API clients exposed through the CLI.
    /// </summary>
    public interface IApiClient
    {
        /// <summary>
        /// Gets the display name of the API client.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the base URL for the API.
        /// </summary>
        string BaseUrl { get; }

        /// <summary>
        /// Gets the API key used for authenticated requests.
        /// </summary>
        string ApiKey { get; }

        /// <summary>
        /// Runs the API client interaction loop.
        /// </summary>
        void Run();
    }
}
