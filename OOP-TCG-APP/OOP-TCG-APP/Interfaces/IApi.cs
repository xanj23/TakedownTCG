namespace TCGAPP
{
    public interface IApi
    {
        string Name { get; }                  /// API display name
        string BaseUrl {get; }                /// Base url used for calling API
        string ApiKey {get; }                 /// Api Key used for calling API
        List<Endpoint> Endpoints { get; }     /// Endpoints this API supports
        int NumOfEndpoints { get; }           /// Convenience property
        Task<object?> Handler(Endpoint chosenEndpoint, Dictionary<string, string> rawQuery);

    }
}
