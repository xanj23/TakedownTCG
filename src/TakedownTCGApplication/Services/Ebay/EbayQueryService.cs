using TakedownTCGApplication.Models.Ebay.Query;

namespace TakedownTCGApplication.Services.Ebay;

public sealed class EbayQueryService
{
    public string BuildItemSearchUrl(EbayItemSearchQueryParams query, string baseUrl)
    {
        Dictionary<string, string> parameters = new()
        {
            ["q"] = query.Search?.Trim() ?? string.Empty,
            ["limit"] = Math.Clamp(query.Limit, 1, 200).ToString(),
            ["offset"] = Math.Max(0, query.Offset).ToString()
        };

        if (!string.IsNullOrWhiteSpace(query.CategoryIds))
        {
            parameters["category_ids"] = query.CategoryIds.Trim();
        }

        string filter = BuildFilter(query);
        if (!string.IsNullOrWhiteSpace(filter))
        {
            parameters["filter"] = filter;
        }

        if (!string.IsNullOrWhiteSpace(query.Sort) && !query.Sort.Equals("relevance", StringComparison.OrdinalIgnoreCase))
        {
            parameters["sort"] = query.Sort.Trim();
        }

        return $"{baseUrl.TrimEnd('/')}/buy/browse/v1/item_summary/search?{BuildQueryString(parameters)}";
    }

    private static string BuildFilter(EbayItemSearchQueryParams query)
    {
        List<string> filters = new();
        if (!string.IsNullOrWhiteSpace(query.BuyingOptions))
        {
            filters.Add($"buyingOptions:{{{query.BuyingOptions.Trim()}}}");
        }

        return string.Join(',', filters);
    }

    private static string BuildQueryString(Dictionary<string, string> parameters)
    {
        return string.Join('&', parameters
            .Where(parameter => !string.IsNullOrWhiteSpace(parameter.Value))
            .Select(parameter => $"{Uri.EscapeDataString(parameter.Key)}={Uri.EscapeDataString(parameter.Value)}"));
    }
}
