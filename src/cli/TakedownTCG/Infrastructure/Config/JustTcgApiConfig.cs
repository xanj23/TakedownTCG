namespace TakedownTCG.cli.Infrastructure.Config
{
    /// <summary>
    /// Stores JustTCG API configuration values used by the CLI.
    /// </summary>
    public sealed class JustTcgApiConfig
    {
        public string BaseUrl { get; } = "https://api.justtcg.com/v1";
        public string ApiKey { get; } = "tcg_5b35dc7894bf4ea6bfd7234e094ae2e1";
        public string ApiKeyHeaderName { get; } = "x-api-key";
    }
}
