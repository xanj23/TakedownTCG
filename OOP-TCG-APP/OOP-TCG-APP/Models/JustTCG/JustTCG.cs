using TCGAPP;
using JustTCG;
public class JustTCGApi : IApi
{
    public string Name => "JustTCG";
    public string BaseUrl => "https://api.justtcg.com/v1";
    public string ApiKey => "tcg_5b35dc7894bf4ea6bfd7234e094ae2e1";

    public List<Endpoint> Endpoints { get; } = new List<Endpoint>
    {
        new Endpoint { Name = "Card", URL = "/card", Parameters = new CardQueryParams()},
        new Endpoint { Name = "Set",  URL = "/set", Parameters = new SetQueryParams()},
        new Endpoint { Name = "Game", URL = "/game",}
    };
    public int NumOfEndpoints => Endpoints.Count;
    static JustTCGApi()
    {
        ApiManager.RegisterApi(new JustTCGApi());
    }
    public async Task<object?> Handler(Endpoint chosenEndpoint, Dictionary<string, string> rawQuery)
    {
        HttpClient client = new HttpClient();

        string refinedQuery = BuildQuery.Run(rawQuery);
        string apiURL = BuildApiURL.Run(this, chosenEndpoint, refinedQuery);
        SetClientHeader.Run(this, client);
        string rawResponse = await FetchApi.Run(apiURL, client);

        switch(chosenEndpoint.Name)
        {
            case "Card":
                return DeserializeResponse.Run<Card>(rawResponse);
            case "Set":
                return DeserializeResponse.Run<Set>(rawResponse);
            case "Game":
            return DeserializeResponse.Run<Game>(rawResponse);
            default:
                Console.WriteLine($"Error: Could not find matching deserializer for {chosenEndpoint.Name}");
                break;
        }
        return null;
    }
}
