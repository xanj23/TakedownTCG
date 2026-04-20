using TakedownTCG.Core.Models.JustTcg.Query;
using TakedownTCG.Core.Services.JustTcg;

namespace TakedownTCG.Core.Abstractions;

public interface IJustTcgSearchService
{
    Task<object> SearchAsync(JustTcgEndpoint endpoint, IQueryParams query, CancellationToken cancellationToken = default);
}
