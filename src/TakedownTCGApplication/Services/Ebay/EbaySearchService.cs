using TakedownTCGApplication.Abstractions;
using TakedownTCGApplication.Infrastructure.Config;
using TakedownTCGApplication.Models.Ebay.Query;

namespace TakedownTCGApplication.Services.Ebay;

public sealed class EbaySearchService : IEbaySearchService
{
    private readonly EbayApiOptions _apiOptions;
    private readonly EbayQueryService _queryService;
    private readonly EbayResponseService _responseService;
    private readonly EbayOAuthTokenService _tokenService;
    private readonly IEbayHttpGateway _httpGateway;

    public EbaySearchService(
        EbayApiOptions apiOptions,
        EbayQueryService queryService,
        EbayResponseService responseService,
        EbayOAuthTokenService tokenService,
        IEbayHttpGateway httpGateway)
    {
        _apiOptions = apiOptions;
        _queryService = queryService;
        _responseService = responseService;
        _tokenService = tokenService;
        _httpGateway = httpGateway;
    }

    public async Task<object> SearchItemsAsync(EbayItemSearchQueryParams query, CancellationToken cancellationToken = default)
    {
        string accessToken = await _tokenService.GetAccessTokenAsync(cancellationToken);
        string url = _queryService.BuildItemSearchUrl(query, _apiOptions.BaseUrl);
        string responseContent = await _httpGateway.FetchSearchResponseAsync(
            url,
            accessToken,
            _apiOptions.MarketplaceId,
            cancellationToken);

        return _responseService.DeserializeSearch(responseContent);
    }
}
