using System;
using TCGAPP;

public class ApiHandler
{
    public static async Task Run()
    {
        IApi chosenApi = ApiMenu.Run();
        if (chosenApi == null)
        {
            Console.WriteLine("Error: No API selected.");
            return;
        }

        Endpoint chosenEndpoint = EndpointMenu.Run(chosenApi);
        if (chosenEndpoint == null)
        {
            Console.WriteLine("Error: No endpoint selected.");
            return;
        }

        Dictionary<string, string> rawQuery = InputQuery.Run<object>(chosenEndpoint);
        
        object? rawResponse = await chosenApi.Handler(chosenEndpoint, rawQuery);
        if (rawResponse == null)
        {
            Console.WriteLine("Error: No response received.");
            return;
        }

        Console.WriteLine(rawResponse);
    }
}
