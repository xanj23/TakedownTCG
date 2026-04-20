namespace TakedownTCG.cli.Infrastructure.Config
{
    /// <summary>
    /// Stores JustTCG API configuration values used by the CLI.
    /// </summary>
    public sealed class JustTcgApiConfig
    {
        public string BaseUrl { get; } = "https://api.justtcg.com/v1";
        public string ApiKey { get; } = Environment.GetEnvironmentVariable("JUSTTCG_API_KEY") ?? string.Empty;
        public string ApiKeyHeaderName { get; } = "x-api-key";
    }
}
