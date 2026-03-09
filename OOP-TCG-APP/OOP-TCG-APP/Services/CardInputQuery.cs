using System;

/// <summary>
/// 
/// </summary>
/// 
public class CardInputQuery
{
    public static CardQueryParameters Run()
    {
        CardQueryParameters param = new CardQueryParameters();
        string input;
        bool loop = true;

        // Q (required)
        while (loop)
        {
            Console.WriteLine("Card Search Query:");
            Console.WriteLine("Must fill in required areas.\nIf you would like to not use a parameter then just press the 'enter' key\n");
            Console.Write("Search query (required): ");
            input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.Clear();
                Console.WriteLine("Invalid Input. Can not be null or blank(' ').");
            }
            else
            {
                param.Q = input;
                loop = false;
            }
        }

        // Number (optional)
        Console.Write("Card number (optional, e.g. 15): ");
        input = Console.ReadLine();
        param.Number = string.IsNullOrWhiteSpace(input) ? null : input;

        // Printing (optional)
        Console.Write("Printing (optional, e.g. Normal, Foil): ");
        input = Console.ReadLine();
        param.Printing = string.IsNullOrWhiteSpace(input) ? null : input;

        // Condition (optional)
        loop = true;
        while (loop)
        {
            Console.Write("Condition (optional): S, NM, LP, MP, HP, DMG: ");
            input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                param.Condition = null;
                loop = false;
            }
            else if (Enum.TryParse(input, true, out Condition condition))
            {
                param.Condition = condition;
                loop = false;
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Invalid condition. Please enter one of: S, NM, LP, MP, HP, DMG");
            }
        }

        return param;
    }
}