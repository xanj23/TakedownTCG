using System.Net.Http;

/// <summary>
/// Sends authenticated HTTP requests to the JustTCG API.
/// </summary>
public static class FetchApi
{
    private static readonly HttpClient client = new HttpClient();
    private const string BaseUrl = "https://api.justtcg.com/v1";
    private const string apiKey = "tcg_5b35dc7894bf4ea6bfd7234e094ae2e1";

    /// <summary>
    /// Configures the shared HTTP client with the API key header required by JustTCG.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the API key environment variable is not set.</exception>
    public static void SetApiHeader()
    {
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("x-api-key", apiKey);
    }

    /// <summary>
    /// Executes a GET request against the JustTCG API.
    /// </summary>
    /// <param name="query">The relative endpoint path and optional query string.</param>
    /// <returns>The raw response body, or <see langword="null"/> when the request cannot be completed successfully.</returns>
    public static async Task<string?> Run(string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            Console.WriteLine($"Error: No query to fetch. query={query}");
            return null;
        }

        string url = BaseUrl + query;
        Console.WriteLine($"Fetching: {url}");
        HttpResponseMessage response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Error: [{(int)response.StatusCode}] {response.StatusCode}");
            return null;
        }

        return await response.Content.ReadAsStringAsync();
    }
}
