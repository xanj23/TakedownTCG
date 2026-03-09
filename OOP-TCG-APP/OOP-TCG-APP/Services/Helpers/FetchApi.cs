using System.Net.Http;

public static class FetchApi
{
    private static readonly HttpClient client = new HttpClient();
    private const string BaseUrl = "https://api.justtcg.com/v1";
    private const string ApiKeyEnvironmentVariable = "JUSTTCG_API_KEY";

    public static void SetApiHeader()
    {
        string? apiKey = Environment.GetEnvironmentVariable(ApiKeyEnvironmentVariable);
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                $"Missing JustTCG API key. Set the {ApiKeyEnvironmentVariable} environment variable before running the app.");
        }

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("x-api-key", apiKey);
    }

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
