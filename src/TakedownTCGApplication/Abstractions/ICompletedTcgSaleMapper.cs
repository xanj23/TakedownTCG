using TakedownTCGApplication.Models.SerpApi.Response;
using TakedownTCGApplication.ViewModels.Home;

namespace TakedownTCGApplication.Abstractions;

public interface ICompletedTcgSaleMapper
{
    IReadOnlyList<CompletedTcgSaleViewModel> MapSales(IEnumerable<SerpApiEbayOrganicResult> results, int limit);
}
