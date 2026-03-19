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
        if(api == null)
        {
            Console.WriteLine("Error: Null Api received. Cannot find endpoint [EndpointMenu]");
            return null; 
        }

        if (api.Endpoints == null || api.Endpoints.Count == 0)
        {
            Console.WriteLine($"{api.Name} has no endpoints [EndpointMenu]");
            return null;
        }

        int chosenEndpoint;

        while (true)
        {
            if (!Console.IsOutputRedirected && !Console.IsErrorRedirected)
            {
                Console.Clear();
            }
            Console.WriteLine($"Select query basis for {api.Name} API");
            Console.WriteLine($"Enter from range of 1-{api.NumOfEndpoints}\n");

            for (int i = 0; i < api.NumOfEndpoints; i++)
            {
                Console.WriteLine($"[ {i + 1} | {api.Endpoints[i].Name} ]");
            }

            Console.Write("\nEnter choice: ");
            string? input = Console.ReadLine();

            if (!int.TryParse(input, out chosenEndpoint))
            {
                Console.WriteLine("That's not a valid choice.\n");
                Console.Write("Press Enter to continue...");
                Console.ReadLine();
                continue;
            }

            if (chosenEndpoint < 1 || chosenEndpoint > api.NumOfEndpoints)
            {
                Console.WriteLine($"Invalid selection. Please select from range 1-{api.NumOfEndpoints}\n");
                Console.Write("Press Enter to continue...");
                Console.ReadLine();
                continue;
            }

            break;
        }

        return api.Endpoints[chosenEndpoint - 1];
    }
}
