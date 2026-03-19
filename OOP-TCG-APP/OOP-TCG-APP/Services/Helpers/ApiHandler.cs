using System;
using System.Net.Http;
using JustTCG;
using TCGAPP;

public class ApiHandler
{
    public static async Task Run()
    {
        IApi chosenApi = ApiMenu.Run();
        Endpoint chosenEndpoint = EndpointMenu.Run(chosenApi);
        Dictionary<T> rawQuery = InputQuery.Run<T>(chosenEndpoint);
        string apiURL = BuildApiURL.Run(rawQuery);
        SetClientHeader.Run(chosenApi);
        string rawResponse = await FetchApi.Run(apiURL);
        object deseralResponse = DeserializeResponse.Run<T>(rawResponse);
        var refinedResponse = MapToUniversalResponse.Run(deseralResponse);
        DisplayResponse.Run(refinedResponse);
    }
}
