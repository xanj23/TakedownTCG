using TakedownTCG.Core.Abstractions;
using TakedownTCG.Core.Infrastructure.Config;
using TakedownTCG.Core.Models.JustTcg.Query;

namespace TakedownTCG.Core.Services.JustTcg;

public sealed class JustTcgSearchService : IJustTcgSearchService
{
    private readonly JustTcgApiOptions _apiOptions;
    private readonly JustTcgQueryService _queryService;
    private readonly JustTcgResponseService _responseService;
    private readonly IJustTcgHttpGateway _httpGateway;

    public JustTcgSearchService(
        JustTcgApiOptions apiOptions,
        JustTcgQueryService queryService,
        JustTcgResponseService responseService,
        IJustTcgHttpGateway httpGateway)
    {
        _apiOptions = apiOptions;
        _queryService = queryService;
        _responseService = responseService;
        _httpGateway = httpGateway;
    }

    public async Task<object> SearchAsync(
        JustTcgEndpoint endpoint,
        IQueryParams query,
        CancellationToken cancellationToken = default)
    {
        string url = _queryService.BuildUrl(endpoint, query, _apiOptions.BaseUrl);
        string responseContent = await _httpGateway.FetchResponseAsync(
            url,
            _apiOptions.ApiKeyHeaderName,
            _apiOptions.ApiKey,
            cancellationToken);

        return _responseService.Deserialize(endpoint, responseContent);
    }
}
