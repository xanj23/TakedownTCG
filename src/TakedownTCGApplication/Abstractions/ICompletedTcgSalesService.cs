using TakedownTCGApplication.Models.Home;

namespace TakedownTCGApplication.Abstractions;

public interface ICompletedTcgSalesService
{
    Task<IReadOnlyList<CompletedTcgSale>> GetRecentCompletedSalesAsync(CancellationToken cancellationToken = default);
}
