using TakedownTCGApplication.Models.Home;
using TakedownTCGApplication.Models.SerpApi.Response;

namespace TakedownTCGApplication.Abstractions;

public interface ICompletedTcgSaleMapper
{
    IReadOnlyList<CompletedTcgSale> MapSales(IEnumerable<SerpApiEbayOrganicResult> results, int limit);
}
