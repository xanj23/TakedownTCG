using TakedownTCG.cli;

namespace TakedownTCG.cli.Util
{
    public static class Formatting
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options"></param>
        /// <returns name="result"></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static string BuildOptionsList<T>(IReadOnlyList<T> options)
        {
            /// Checking to see if options are null or empty and throwing an exception
            if (options == null){throw new ArgumentNullException(nameof(options)); }
            if (options.Count == 0){ throw new ArgumentException("Options list cannot be empty.", nameof(options)); }

            /// Loops through options and puts a number in front of the option 
            string result = "";
            for (int i = 0; i < options.Count; i++)
            {
            result += $"{i+1}. {options[i]} \n";
            }

            /// Returns formatted list of options
            return result;
        }
    }
}