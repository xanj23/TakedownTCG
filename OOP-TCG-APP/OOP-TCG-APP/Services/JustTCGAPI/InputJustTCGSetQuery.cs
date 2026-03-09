/// <summary>
/// Collects JustTCG set search filters from the console and maps them into a set query model.
/// </summary>
public class InputJustTCGSetQuery
{
    /// <summary>
    /// Prompts the user for JustTCG set search inputs and returns the resulting query parameters.
    /// </summary>
    /// <returns>A populated <see cref="JustTCGSetQueryParams"/> instance.</returns>
    public static JustTCGSetQueryParams Run()
    {
        JustTCGSetQueryParams param = new JustTCGSetQueryParams();
        bool loop = true;

        while (loop)
        {
            Console.WriteLine("Set Search Query:");
            Console.WriteLine("Must fill in required areas.\nIf you would like to not use a parameter then just press the 'enter' key\n");
            Console.Write("Game (required, e.g. mtg, pokemon): ");

            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.Clear();
                Console.WriteLine("Invalid Input. Can not be null or blank(' ').");
            }
            else
            {
                param.Game = input;
                loop = false;
            }
        }

        Console.Write("Search query (optional): ");
        string? query = Console.ReadLine();
        param.Q = string.IsNullOrWhiteSpace(query) ? null : query;

        loop = true;
        while (loop)
        {
            Console.WriteLine("Order by (optional, press enter to skip):");
            Console.WriteLine("[1 | name]");
            Console.WriteLine("[2 | release date]");
            Console.Write("Enter choice: ");

            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                param.OrderBy = null;
                loop = false;
            }
            else if (input == "1")
            {
                param.OrderBy = OrderBy.name;
                loop = false;
            }
            else if (input == "2")
            {
                param.OrderBy = OrderBy.release_date;
                loop = false;
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Invalid input. Please enter 1 or 2.");
            }
        }

        loop = true;
        while (loop)
        {
            Console.WriteLine("Sort order (optional, press enter to skip):");
            Console.WriteLine("[1] asc");
            Console.WriteLine("[2] desc");
            Console.Write("Enter choice: ");

            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                param.Order = null;
                loop = false;
            }
            else if (input == "1")
            {
                param.Order = Order.asc;
                loop = false;
            }
            else if (input == "2")
            {
                param.Order = Order.desc;
                loop = false;
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Invalid input. Please enter 1 or 2.");
            }
        }

        return param;
    }
}
