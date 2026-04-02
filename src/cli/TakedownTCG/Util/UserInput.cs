using System.Globalization;
using TakedownTCG.cli;

namespace TakedownTCG.cli.Util
{
    public static class UserInput
    {
        /// <summary>
        /// Prompts the user to choose an option and returns the zero-based index.
        /// </summary>
        /// <param name="prompt">Header text shown above the option list.</param>
        /// <param name="options">Options to display; must not be null or empty.</param>
        /// <returns>The zero-based index of the selected option.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static int GetIndex<T>(string prompt, IReadOnlyList<T> options)
        {
            /// Checking to see if options are null or empty and throwing an exception
            if (options == null){throw new ArgumentNullException(nameof(options)); }
            if (options.Count == 0){ throw new ArgumentException("Options list cannot be empty.", nameof(options)); }

            /// Loop to get valid user input
            string currentList = Formatting.BuildOptionsList(options);
            while (true)
            {
                /// Using prompt and formatted options list to show what options
                Console.WriteLine('\n' + prompt + '\n' + "Select an option by number:" + '\n' + currentList);

                /// Getting a raw input and if it comes back null instead it creates an empty string. 
                /// It trims the string of any white space too.
                string rawInput = (Console.ReadLine() ?? string.Empty).Trim();

                /// Checks to see if rawInput is empty 
                /// Then checks if integer
                /// Then checks to see if valid input
                if (rawInput.Length == 0)
                {
                    Console.WriteLine($"User Input was empty. Please enter a number from 1 to {options.Count}.");
                }
                else
                {
                    /// Parse digits-only input (no signs/decimals)
                    bool isInt = int.TryParse(rawInput, NumberStyles.None, CultureInfo.InvariantCulture, out int rawIndex);
                    if(isInt == false)
                    {
                        Console.WriteLine($"User Input was invalid. Please enter a number from 1 to {options.Count}.");
                    }
                    else
                    {
                        int choice = rawIndex - 1;
                        if (choice < 0 || choice >= options.Count)
                        {
                           Console.WriteLine($"User Input was not in range. Please enter a number from 1 to {options.Count}."); 
                        }
                        else
                        {
                            /// Returns valid index chosen by user
                            return choice;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Prompts for a required string value; input must not be null or blank.
        /// </summary>
        /// <param name="label">Prompt label shown to the user.</param>
        /// <returns>Non-empty trimmed string.</returns>
        public static string InputRequiredString(string label)
        {
            while (true)
            {
                Console.Write($"{label}: ");
                string value = (Console.ReadLine() ?? string.Empty).Trim();
                if (value.Length == 0)
                {
                    Console.WriteLine("Input is required and cannot be blank.");
                    continue;
                }

                return value;
            }
        }
    }
}
