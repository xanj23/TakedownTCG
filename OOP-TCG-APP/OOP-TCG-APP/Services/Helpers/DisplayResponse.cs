using System;

namespace TCGAPP
{
    /// <summary>
    /// Displays a <see cref="UniversalResponse"/> in a readable console format.
    /// </summary>
    public class DisplayResponse
    {
        /// <summary>
        /// Writes the response source and each record to the console.
        /// </summary>
        /// <param name="response">The response to display.</param>
        public static void Run(UniversalResponse response)
        {
            if (response == null)
            {
                Console.WriteLine("Error: response was null. [DisplayResponse]");
                return;
            }

            string source = string.IsNullOrWhiteSpace(response.SourceType) ? "Unknown" : response.SourceType;
            Console.WriteLine($"Results from: {source}\n");

            if (response.Records.Count == 0)
            {
                Console.WriteLine("No records found.");
                return;
            }

            for (int i = 0; i < response.Records.Count; i++)
            {
                var record = response.Records[i];
                Console.WriteLine($"Record {i + 1}:");

                if (record.Fields.Count == 0)
                {
                    Console.WriteLine("  (no fields)");
                    Console.WriteLine();
                    continue;
                }

                foreach (var field in record.Fields)
                {
                    Console.WriteLine($"  {field.Key}: {field.Value}");
                }

                Console.WriteLine();
            }
        }
    }
}
