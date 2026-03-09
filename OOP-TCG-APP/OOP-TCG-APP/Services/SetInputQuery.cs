using System;
/// <summary>
/// 
/// </summary>
/// 
public class SetInputQuery
{
    public static SetQueryParameters Run()
    {
        SetQueryParameters param = new SetQueryParameters();
        string input;
        bool loop = true;

        // Game (required)
        while (loop)
        {
            Console.WriteLine("Set Search Query:");
            Console.WriteLine("Must fill in required areas.\nIf you would like to not use a parameter then just press the 'enter' key\n");
            Console.Write("Game (required, e.g. mtg, pokemon): ");
            input = Console.ReadLine();
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

        // Q (optional)
        Console.Write("Search query (optional): ");
        input = Console.ReadLine();
        param.Q = string.IsNullOrWhiteSpace(input) ? null : input;

        // OrderBy (optional)
        loop = true;
        while (loop)
        {
            Console.WriteLine("Order by (optional, press enter to skip):");
            Console.WriteLine("[1 | name]");
            Console.WriteLine("[2 | release date]");
            Console.Write("Enter choice: ");
            input = Console.ReadLine();
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

        // Order (optional)
        loop = true;
        while (loop)
        {
            Console.WriteLine("Sort order (optional, press enter to skip):");
            Console.WriteLine("[1] asc");
            Console.WriteLine("[2] desc");
            Console.Write("Enter choice: ");
            input = Console.ReadLine();
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