/// <summary>
/// Collects JustTCG card search filters from the console and maps them into a card query model.
/// </summary>
public class InputJustTCGCardQuery
{
    /// <summary>
    /// Prompts the user for JustTCG card search inputs and returns the resulting query parameters.
    /// </summary>
    /// <returns>A populated <see cref="JustTCGCardQueryParams"/> instance.</returns>
    public static JustTCGCardQueryParams Run()
    {
        JustTCGCardQueryParams param = new JustTCGCardQueryParams();
        bool loop = true;

        while (loop)
        {
            Console.WriteLine("Card Search Query:");
            Console.WriteLine("Must fill in required areas.\nIf you would like to not use a parameter then just press the 'enter' key\n");
            Console.Write("Search query (required): ");

            string? input = Console.ReadLine();
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

        Console.Write("Card number (optional, e.g. 15): ");
        string? number = Console.ReadLine();
        param.Number = string.IsNullOrWhiteSpace(number) ? null : number;

        Console.Write("Printing (optional, e.g. Normal, Foil): ");
        string? printing = Console.ReadLine();
        param.Printing = string.IsNullOrWhiteSpace(printing) ? null : printing;

        loop = true;
        while (loop)
        {
            Console.Write("Condition (optional): S, NM, LP, MP, HP, DMG: ");
            string? input = Console.ReadLine();
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
