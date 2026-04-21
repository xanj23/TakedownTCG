using TakedownTCGApplication.Abstractions;
using TakedownTCGApplication.Infrastructure.Config;
using TakedownTCGApplication.Models.PokemonTcg.Query;

namespace TakedownTCGApplication.Services.PokemonTcg;

public sealed class PokemonTcgSearchService : IPokemonTcgSearchService
{
    private readonly PokemonTcgApiOptions _apiOptions;
    private readonly PokemonTcgQueryService _queryService;
    private readonly PokemonTcgResponseService _responseService;
    private readonly IPokemonTcgHttpGateway _httpGateway;

    public PokemonTcgSearchService(
        PokemonTcgApiOptions apiOptions,
        PokemonTcgQueryService queryService,
        PokemonTcgResponseService responseService,
        IPokemonTcgHttpGateway httpGateway)
    {
        _apiOptions = apiOptions;
        _queryService = queryService;
        _responseService = responseService;
        _httpGateway = httpGateway;
    }

    public async Task<object> SearchCardsAsync(PokemonCardQueryParams query, CancellationToken cancellationToken = default)
    {
        string url = _queryService.BuildCardsUrl(query, _apiOptions.BaseUrl);
        string responseContent = await _httpGateway.FetchResponseAsync(
            url,
            _apiOptions.ApiHostHeaderName,
            _apiOptions.ApiHost,
            _apiOptions.ApiKeyHeaderName,
            _apiOptions.ApiKey,
            cancellationToken);

        return _responseService.DeserializeCards(responseContent);
    }
}
