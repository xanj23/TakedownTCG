using System;
using System.Net.Http;
using TakedownTCG.cli.Api.JustTCG.Query;
using TakedownTCG.cli.Menu;

namespace TakedownTCG.cli.Api.JustTCG
{
    /// <summary>
    /// Coordinates the JustTCG client flow from menu selection through response output.
    /// </summary>
    public class JustTCGClient : IApiClient
    {
        internal const string ApiKeyHeaderName = "x-api-key";
        internal static readonly HttpClient HttpClient = new HttpClient();
        private readonly JustTCGCommands _commands;

        /// <summary>
        /// Initializes a new instance of the <see cref="JustTCGClient"/> class.
        /// </summary>
        public JustTCGClient()
        {
            _commands = new JustTCGCommands(this);
        }

        /// <summary>
        /// Gets the display name of the API client.
        /// </summary>
        public string Name => "JustTCG";

        /// <summary>
        /// Gets the JustTCG API base URL.
        /// </summary>
        public string BaseUrl => "https://api.justtcg.com/v1";

        /// <summary>
        /// Gets the JustTCG API key used for authenticated requests.
        /// </summary>
        public string ApiKey => "tcg_5b35dc7894bf4ea6bfd7234e094ae2e1";

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

                IQueryParams query = _commands.InputQuery(selectedAction);
                HttpRequestMessage request = _commands.BuildRequest(selectedAction, query);
                string responseContent = _commands.FetchResponse(request);
                object responseData = _commands.Deserialize(selectedAction, responseContent);
                string mappedData = _commands.Map(responseData);
                _commands.Display(mappedData);
            }
        }
    }
}
