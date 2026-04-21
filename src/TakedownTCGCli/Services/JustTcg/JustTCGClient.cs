using TakedownTCG.cli.Infrastructure.Config;
using TakedownTCG.cli.Infrastructure.Http;
using TakedownTCG.cli.Services.Api;
using TakedownTCG.cli.Views.Menus;
using TakedownTCG.cli.Views.Output;
using TakedownTCG.cli.Views.Shared;

namespace TakedownTCG.cli.Services.JustTcg;

/// <summary>
/// Coordinates the JustTCG client flow from menu selection through response output.
/// </summary>
public sealed class JustTCGClient : IApiClient
{
    private readonly JustTcgApiConfig _config;
    private readonly JustTcgHttpGateway _httpGateway;
    private readonly JustTcgQueryService _queryService;
    private readonly JustTcgResponseService _responseService;
    private readonly Action<object>? _afterResponse;

    public JustTCGClient(
        JustTcgApiConfig config,
        JustTcgHttpGateway httpGateway,
        JustTcgQueryService queryService,
        JustTcgResponseService responseService,
        Action<object>? afterResponse = null)
    {
        _config = config;
        _httpGateway = httpGateway;
        _queryService = queryService;
        _responseService = responseService;
        _afterResponse = afterResponse;
    }

    public string Name => "JustTCG";
    public string BaseUrl => _config.BaseUrl;
    public string ApiKey => _config.ApiKey;

    public enum Action
    {
        Cards = 0,
        Sets = 1,
        Games = 2,
        Back = 3,
        Quit = 4
    }

    public void Run()
    {
        while (true)
        {
            Action selectedAction = MenuRunner.Select(JustTCGMenu.Definition);

            if (JustTCGMenu.Definition.BackAction.HasValue && selectedAction == JustTCGMenu.Definition.BackAction.Value)
            {
                return;
            }

            if (selectedAction == JustTCGMenu.Definition.QuitAction)
            {
                Environment.Exit(0);
            }

            object responseData = Search(selectedAction);
            string mappedData = _responseService.Map(responseData);
            JustTcgOutputView.DisplayMappedData(mappedData);

            _afterResponse?.Invoke(responseData);
        }
    }

    private object Search(Action action)
    {
        var query = _queryService.InputQuery(action);
        string url = _queryService.BuildUrl(action, query, _config.BaseUrl);
        string responseContent = _httpGateway.FetchResponse(url, _config.ApiKeyHeaderName, _config.ApiKey);
        return _responseService.Deserialize(action, responseContent);
    }

}
