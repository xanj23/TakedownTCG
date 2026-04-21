namespace TakedownTCG.cli.Infrastructure.Config
{
    /// <summary>
    /// Stores JustTCG API configuration values used by the CLI.
    /// </summary>
    public sealed class JustTcgApiConfig
    {
        public string BaseUrl { get; set; } = "https://api.justtcg.com/v1";
        public string ApiKeyHeaderName { get; set; } = "x-api-key";
        public string ApiKey { get; set; } = string.Empty;
    }
}
