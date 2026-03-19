using System;
using TCGAPP;
using JustTCG;
using System.Net;

/// <summary>
/// 
/// </summary>
public class EndpointMenu
{
    ///<param name="api">
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static Endpoint Run(IApi api)
    {
        Console.Clear();

        if(api == null)
        {
            Console.WriteLine($"Error: Null Api received. Cannot find endpoint");
            return null; 
        }

        if (api.Endpoints == null || api.Endpoints.Count == 0)
        {
            Console.WriteLine($"{api.Name} has no endpoints");
            return null;
        }

        Console.WriteLine($"Select query basis for {api.Name} API");
        Console.WriteLine($"Enter from range of 1-{api.NumOfEndpoints}\n");

        int chosenEndpoint;

        while (true)
        {
            for (int i = 0; i < api.NumOfEndpoints; i++)
            {
                Console.WriteLine($"[ {i + 1} | {api.Endpoints[i].Name} ]");
            }

            Console.Write("\nEnter choice: ");
            string? input = Console.ReadLine();

            if (!int.TryParse(input, out chosenEndpoint))
            {
                Console.Clear();
                Console.WriteLine("That's not a valid choice.\n");
                continue;
            }

            if (chosenEndpoint < 1 || chosenEndpoint > api.NumOfEndpoints)
            {
                Console.Clear();
                Console.WriteLine($"Invalid selection. Please select from range 1-{api.NumOfEndpoints}\n");
                continue;
            }

            break;
        }

        return api.Endpoints[chosenEndpoint - 1];
    }
}
