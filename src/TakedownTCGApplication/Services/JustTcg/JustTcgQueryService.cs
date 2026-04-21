using TakedownTCG.Core.Models.JustTcg.Query;

namespace TakedownTCG.Core.Services.JustTcg;

public sealed class JustTcgQueryService
{
    public string BuildUrl(JustTcgEndpoint endpoint, IQueryParams query, string baseUrl)
    {
        string path = endpoint switch
        {
            JustTcgEndpoint.Cards => "/cards",
            JustTcgEndpoint.Sets => "/sets",
            JustTcgEndpoint.Games => "/games",
            _ => throw new NotSupportedException($"Unsupported endpoint: {endpoint}")
        };

        string queryString = BuildQuery(query);
        return $"{baseUrl}{path}{queryString}";
    }

    private static string BuildQuery(IQueryParams query)
    {
        ArgumentNullException.ThrowIfNull(query);
        Dictionary<string, string> rawQuery = BuildRawQuery(query);
        return FormatQueryString(rawQuery);
    }

    private static Dictionary<string, string> BuildRawQuery(IQueryParams query)
    {
        if (query.Parameters.Count == 0)
        {
            return new Dictionary<string, string>();
        }

        Dictionary<string, string> rawQuery = new();
        foreach (KeyValuePair<string, QueryParameter> kvp in query.Parameters)
        {
            string key = kvp.Key;
            QueryParameter param = kvp.Value;
            object? value = param.Value;

            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                continue;
            }

            rawQuery.Add(Uri.EscapeDataString(key), Uri.EscapeDataString(value.ToString() ?? string.Empty));
        }

        return rawQuery;
    }

    private static string FormatQueryString(Dictionary<string, string> rawQuery)
    {
        if (rawQuery.Count == 0)
        {
            return string.Empty;
        }

        List<string> parameters = new();
        foreach (KeyValuePair<string, string> pair in rawQuery)
        {
            parameters.Add(pair.Key + '=' + pair.Value);
        }

        return '?' + string.Join('&', parameters);
    }
}
