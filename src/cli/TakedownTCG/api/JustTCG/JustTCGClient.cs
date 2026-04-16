using System;
using TakedownTCG.cli.Models.JustTcg.Query;
using TakedownTCG.cli.Infrastructure.Config;
using TakedownTCG.cli.Infrastructure.Http;
using TakedownTCG.cli.Services.JustTcg;
using TakedownTCG.cli.Views.Menus;
using TakedownTCG.cli.Views.Output;
using TakedownTCG.cli.Views.Shared;

namespace TakedownTCG.cli.Api.JustTCG
{
    /// <summary>
    /// Coordinates the JustTCG client flow from menu selection through response output.
    /// </summary>
    public class JustTCGClient : IApiClient
    {
        private readonly JustTcgApiConfig _config;
        private readonly JustTcgQueryService _queryService;
        private readonly JustTcgResponseService _responseService;
        private readonly JustTcgHttpGateway _httpGateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="JustTCGClient"/> class.
        /// </summary>
        public JustTCGClient()
        {
            _config = new JustTcgApiConfig();
            _queryService = new JustTcgQueryService();
            _responseService = new JustTcgResponseService();
            _httpGateway = new JustTcgHttpGateway();
        }

        /// <summary>
        /// Gets the display name of the API client.
        /// </summary>
        public string Name => "JustTCG";

        /// <summary>
        /// Gets the JustTCG API base URL.
        /// </summary>
        public string BaseUrl => _config.BaseUrl;

        /// <summary>
        /// Gets the JustTCG API key used for authenticated requests.
        /// </summary>
        public string ApiKey => _config.ApiKey;

        /// <summary>
        /// Represents the endpoint actions available in the JustTCG menu.
        /// </summary>
        public enum Action
        {
            Cards = 0,
            Sets = 1,
            Games = 2,
            Back = 3,
            Quit = 4
        }

        /// <summary>
        /// Runs the JustTCG endpoint-selection and query flow.
        /// </summary>
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

                IQueryParams query = _queryService.InputQuery(selectedAction);
                string url = _queryService.BuildUrl(selectedAction, query, BaseUrl);
                string responseContent = _httpGateway.FetchResponse(url, _config.ApiKeyHeaderName, ApiKey);
                object responseData = _responseService.Deserialize(selectedAction, responseContent);
                string mappedData = _responseService.Map(responseData);
                JustTcgOutputView.DisplayMappedData(mappedData);
            }
        }
    }
}

