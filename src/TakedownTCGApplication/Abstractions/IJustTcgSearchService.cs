using TakedownTCGApplication.Models.JustTcg.Query;
using TakedownTCGApplication.Services.JustTcg;

namespace TakedownTCGApplication.Abstractions;

public interface IJustTcgSearchService
{
    Task<object> SearchAsync(JustTcgEndpoint endpoint, IQueryParams query, CancellationToken cancellationToken = default);
}
