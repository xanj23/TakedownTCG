namespace TakedownTCGApplication.Infrastructure.Config;

public sealed class JustTcgApiOptions
{
    public string BaseUrl { get; set; } = "https://api.justtcg.com/v1";
    public string ApiKeyHeaderName { get; set; } = "x-api-key";
    public string ApiKey { get; set; } = string.Empty;
}
