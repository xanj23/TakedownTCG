using TakedownTCGApplication.Infrastructure.Config;

namespace TakedownTCGApplication.Services.SerpApi;

public sealed class SerpApiQueryService
{
    public string BuildCompletedSalesUrl(SerpApiOptions options)
    {
        Dictionary<string, string> parameters = new()
        {
            ["engine"] = "ebay",
            ["ebay_domain"] = options.EbayDomain,
            ["_nkw"] = options.DefaultQuery,
            ["show_only"] = options.ShowOnly,
            ["_ipg"] = Math.Clamp(options.PageSize, 25, 200).ToString()
        };

        if (!string.IsNullOrWhiteSpace(options.CategoryId))
        {
            parameters["category_id"] = options.CategoryId;
        }

        return $"{options.BaseUrl.TrimEnd('/')}?{BuildQueryString(parameters)}";
    }

    private static string BuildQueryString(Dictionary<string, string> parameters)
    {
        return string.Join('&', parameters
            .Where(parameter => !string.IsNullOrWhiteSpace(parameter.Value))
            .Select(parameter => $"{Uri.EscapeDataString(parameter.Key)}={Uri.EscapeDataString(parameter.Value)}"));
    }
}
