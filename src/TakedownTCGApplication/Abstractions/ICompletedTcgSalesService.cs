using TakedownTCGApplication.ViewModels.Home;

namespace TakedownTCGApplication.Abstractions;

public interface ICompletedTcgSalesService
{
    Task<IReadOnlyList<CompletedTcgSaleViewModel>> GetRecentCompletedSalesAsync(CancellationToken cancellationToken = default);
}
