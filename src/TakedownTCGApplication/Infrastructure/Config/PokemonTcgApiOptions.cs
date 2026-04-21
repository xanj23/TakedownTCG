namespace TakedownTCGApplication.Infrastructure.Config;

public sealed class PokemonTcgApiOptions
{
    public string BaseUrl { get; set; } = "https://pokemon-tcg-api.p.rapidapi.com";
    public string ApiKeyHeaderName { get; set; } = "x-rapidapi-key";
    public string ApiHostHeaderName { get; set; } = "x-rapidapi-host";
    public string ApiHost { get; set; } = "pokemon-tcg-api.p.rapidapi.com";
    public string ApiKey { get; set; } = string.Empty;
}
