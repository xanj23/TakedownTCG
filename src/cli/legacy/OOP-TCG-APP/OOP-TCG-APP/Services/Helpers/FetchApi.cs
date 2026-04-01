using System.Net.Http;

namespace TCGAPP
{
    /// <summary>
    /// Sends authenticated HTTP requests to the JustTCG API.
    /// </summary>
    public static class FetchApi
    {
        /// <summary>
        /// Executes a GET request against the JustTCG API.
        /// </summary>
        /// <param name="query">The relative endpoint path and optional query string.</param>
        /// <returns>The raw response body, or <see langword="null"/> when the request cannot be completed successfully.</returns>
        public static async Task<string?> Run(string? apiURL, HttpClient client)
        {
            if (string.IsNullOrWhiteSpace(apiURL))
            {
                Console.WriteLine($"Error: No query to fetch. query={apiURL} [FetchApi]");
                return null;
            }

            Console.WriteLine($"Fetching: {apiURL}");
            HttpResponseMessage response = await client.GetAsync(apiURL);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error: [{(int)response.StatusCode}] {response.StatusCode} [FetchApi]");
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }
    }
}
