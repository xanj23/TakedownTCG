using System.Runtime.InteropServices;
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
  public Response<T> Handler<T>(Endpoint chosenEndpoint)
    {
        var _rawQuery;
        string _queryURL;
        string _rawResponse;
        object _deseralResponse;
        Dictionary<> _refinedQuery = new Directory<>;
        switch (chosenEndpoint.Name)
        {
            case "Card":
                _rawQuery = InputCardQuery.Run();
                _refinedQuery = ObjToDict.Run(_rawQuery);
                _queryURL = BuildQuery.Run(_refinedQuery);
                string apiURL = BuildApiURL.Run(ApiManager.ApiRepositories[0], chosenEndpoint, _queryURL);
                _rawResponse = FetchApi.Run(apiURL);
                _deseralResponse = DeserializeResponse.Run<Card>(_rawResponse);
                break;
            case "Set":
                _rawQuery = InputSetQuery.Run();
                _refinedQuery = ObjToDict.Run(_rawQuery);
                _queryURL = BuildQuery.Run(_refinedQuery);
                break;
            case "Game":
                break;
            default:
                Console.WriteLine("Error: No endpoint found in JustApi");
                return
                break;
            
        }
        return 
    }
}
