/// <summary>
/// 
/// </summary>
public class QueryMenu
{
    private enum Endpoint
    {
        Card = 1,
        Game = 2,
        Set = 3
    }

    private static readonly int numOfEndpoints = Enum.GetValues(typeof(Endpoint)).Length;
    private static readonly Endpoint[] endpoints = Enum.GetValues<Endpoint>();

    public static async Task Run()
    {
        bool menu = true;
        int ep = 0;
        string? query = null;

        while (menu)
        {
            Console.WriteLine("What would you like to search?");
            for (int i = 0; i < numOfEndpoints; i++)
            {
                string epName = endpoints[i].ToString();
                int epValue = (int)endpoints[i];
                Console.WriteLine($"[{epName} | {epValue}]");
            }

            Console.Write("Enter choice: ");
            string? input = Console.ReadLine();

            if (!int.TryParse(input, out int chosenEndpoint))
            {
                Console.Clear();
                Console.WriteLine("That's not a valid choice.\n");
                continue;
            }

            if (chosenEndpoint > numOfEndpoints || chosenEndpoint < 1)
            {
                Console.Clear();
                Console.WriteLine("Invalid selection. Please select from range 1-" + numOfEndpoints + '\n');
                continue;
            }

            Console.Clear();
            switch (chosenEndpoint)
            {
                case (int)Endpoint.Card:
                    ep = chosenEndpoint;
                    query = "/cards" + BuildQuery.Run(ObjToDict.Run(CardInputQuery.Run()));
                    menu = false;
                    break;
                case (int)Endpoint.Game:
                    ep = chosenEndpoint;
                    query = "/games";
                    menu = false;
                    break;
                case (int)Endpoint.Set:
                    ep = chosenEndpoint;
                    query = "/sets" + BuildQuery.Run(ObjToDict.Run(SetInputQuery.Run()));
                    menu = false;
                    break;
                default:
                    Console.WriteLine("Invalid selection. Please select from range 1-" + numOfEndpoints + '\n');
                    break;
            }
        }

        string? rawResponse = null;
        try
        {
            rawResponse = await FetchApi.Run(query);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        }

        if (rawResponse == null)
        {
            Console.WriteLine("Error: Fetch return error or null. Check for previous error codes.");
            return;
        }

        switch (ep)
        {
            case (int)Endpoint.Card:
                Console.WriteLine(DeserializeApi.Run<Card>(rawResponse));
                break;
            case (int)Endpoint.Set:
                Console.WriteLine(DeserializeApi.Run<Set>(rawResponse));
                break;
            case (int)Endpoint.Game:
                Console.WriteLine(DeserializeApi.Run<Game>(rawResponse));
                break;
            default:
                Console.WriteLine("Error: Endpoint not found cannot send deserializer");
                break;
        }
    }
}
