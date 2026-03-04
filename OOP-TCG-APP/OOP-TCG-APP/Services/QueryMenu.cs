/// <summary>
/// 
/// </summary>
public class QueryMenu
{
    public static void Run()
    {
        while (true)
        {
            Console.WriteLine("What would you like to search?");
            Console.WriteLine("[Card | 1]");
            Console.WriteLine("[Game | 2]");
            Console.WriteLine("[Set  | 3]");
            Console.Write("Enter choice: ");

            string input = Console.ReadLine();
            if (!int.TryParse(input, out int number) || !Enum.IsDefined(typeof(EndpointType), number))
            {
                Console.Clear();
                Console.WriteLine("That's not a valid choice.\n");
            }
            else if (number > 3 || number < 0) {
                Console.Clear();
                Console.WriteLine("Invalid selection. Please select from range 1-3.\n");
            }
        }

    }
}
