using System;
using TCGAPP;

public class ApiHandler
{
    public static async Task Run()
    {
        IApi chosenApi = ApiMenu.Run();
        if (chosenApi == null)
        {
            Console.WriteLine("Error: No API selected. [ApiHandler]");
            return;
        }

        Endpoint chosenEndpoint = EndpointMenu.Run(chosenApi);
        if (chosenEndpoint == null)
        {
            Console.WriteLine("Error: No endpoint selected. [ApiHandler]");
            return;
        }

        Dictionary<string, string> rawQuery = InputQuery.Run<object>(chosenEndpoint);

        object? rawResponse = await chosenApi.Handler(chosenEndpoint, rawQuery);
        if (rawResponse == null)
        {
            Console.WriteLine("Error: No response received. [ApiHandler]");
            return;
        }

        UniversalResponse response = MaptoUniversalResponse.Run(chosenApi, rawResponse);
        DisplayResponse.Run(response);
    }
}
