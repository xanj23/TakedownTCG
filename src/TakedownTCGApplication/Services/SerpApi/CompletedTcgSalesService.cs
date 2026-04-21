using TakedownTCGApplication.Abstractions;
using TakedownTCGApplication.Infrastructure.Config;
using TakedownTCGApplication.Models.SerpApi.Response;
using TakedownTCGApplication.ViewModels.Home;

namespace TakedownTCGApplication.Services.SerpApi;

public sealed class CompletedTcgSalesService : ICompletedTcgSalesService
{
    private readonly SerpApiOptions _options;
    private readonly SerpApiQueryService _queryService;
    private readonly SerpApiResponseService _responseService;
    private readonly ISerpApiHttpGateway _httpGateway;
    private readonly ICompletedTcgSaleMapper _mapper;

    public CompletedTcgSalesService(
        SerpApiOptions options,
        SerpApiQueryService queryService,
        SerpApiResponseService responseService,
        ISerpApiHttpGateway httpGateway,
        ICompletedTcgSaleMapper mapper)
    {
        _options = options;
        _queryService = queryService;
        _responseService = responseService;
        _httpGateway = httpGateway;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<CompletedTcgSaleViewModel>> GetRecentCompletedSalesAsync(CancellationToken cancellationToken = default)
    {
        string url = _queryService.BuildCompletedSalesUrl(_options);
        string responseContent = await _httpGateway.FetchResponseAsync(url, _options.ApiKey, cancellationToken);
        SerpApiEbaySearchResponse response = _responseService.DeserializeEbaySearch(responseContent);
        return _mapper.MapSales(response.OrganicResults, _options.CarouselLimit);
    }
}
