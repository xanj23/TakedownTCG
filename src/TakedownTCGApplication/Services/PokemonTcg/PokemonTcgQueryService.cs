using TakedownTCGApplication.Models.PokemonTcg.Query;

namespace TakedownTCGApplication.Services.PokemonTcg;

public sealed class PokemonTcgQueryService
{
    public string BuildCardsUrl(PokemonCardQueryParams query, string baseUrl)
    {
        Dictionary<string, string> parameters = new()
        {
            ["per_page"] = Math.Max(1, query.PerPage).ToString(),
            ["page"] = Math.Max(1, query.Page).ToString(),
            ["sort"] = string.IsNullOrWhiteSpace(query.Sort) ? "relevance" : query.Sort.Trim()
        };

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            parameters["search"] = query.Search.Trim();
        }

        if (!string.IsNullOrWhiteSpace(query.TcgId))
        {
            parameters["tcgid"] = query.TcgId.Trim();
        }

        return $"{baseUrl.TrimEnd('/')}/cards?{BuildQueryString(parameters)}";
    }

    private static string BuildQueryString(Dictionary<string, string> parameters)
    {
        return string.Join('&', parameters.Select(parameter =>
            $"{Uri.EscapeDataString(parameter.Key)}={Uri.EscapeDataString(parameter.Value)}"));
    }
}
