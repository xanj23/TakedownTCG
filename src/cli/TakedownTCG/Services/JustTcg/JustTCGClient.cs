using System;
using TakedownTCG.cli.Controllers;
using TakedownTCG.cli.Infrastructure.Config;
using TakedownTCG.cli.Infrastructure.Http;
using TakedownTCG.cli.Models.JustTcg.Query;
using TakedownTCG.cli.Models.JustTcg.Response;
using TakedownTCG.cli.Services.Api;
using TakedownTCG.cli.Services.JustTcg;
using TakedownTCG.cli.Views.Input;
using TakedownTCG.cli.Views.Menus;
using TakedownTCG.cli.Views.Output;
using TakedownTCG.cli.Views.Shared;

namespace TakedownTCG.cli.Services.JustTcg
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

                // Offer to add a displayed result to favorites for logged-in users.
                try
                {
                    Console.WriteLine();
                    string? favInput = UserInput.InputString("Add a result to favorites? Enter result number or press Enter to skip");
                    if (!string.IsNullOrWhiteSpace(favInput))
                    {
                        if (!int.TryParse(favInput.Trim(), out int favIndex))
                        {
                            Console.WriteLine("Invalid number.");
                            continue;
                        }

                        var current = UserAccountController.CurrentUser;
                        if (current is null)
                        {
                            Console.WriteLine("Login to add favorites.");
                            continue;
                        }

                        if (responseData is Response<Card> cardResp)
                        {
                            if (favIndex < 1 || favIndex > cardResp.Data.Count)
                            {
                                Console.WriteLine("Index out of range.");
                                continue;
                            }

                            Card card = cardResp.Data[favIndex - 1];
                            bool added = UserAccountController.FavoriteService.AddFavorite(current.UserName, "Card", card.Id, card.Name);
                            Console.WriteLine(added ? "Added to favorites." : "Already favorited or failed.");
                        }
                        else if (responseData is Response<Set> setResp)
                        {
                            if (favIndex < 1 || favIndex > setResp.Data.Count)
                            {
                                Console.WriteLine("Index out of range.");
                                continue;
                            }

                            Set set = setResp.Data[favIndex - 1];
                            bool added = UserAccountController.FavoriteService.AddFavorite(current.UserName, "Set", set.Id, set.Name);
                            Console.WriteLine(added ? "Added to favorites." : "Already favorited or failed.");
                        }
                        else if (responseData is Response<Game> gameResp)
                        {
                            if (favIndex < 1 || favIndex > gameResp.Data.Count)
                            {
                                Console.WriteLine("Index out of range.");
                                continue;
                            }

                            Game game = gameResp.Data[favIndex - 1];
                            bool added = UserAccountController.FavoriteService.AddFavorite(current.UserName, "Game", game.Id, game.Name);
                            Console.WriteLine(added ? "Added to favorites." : "Already favorited or failed.");
                        }
                        else
                        {
                            Console.WriteLine("Favoriting is not supported for this response type.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to add favorite: {ex.Message}");
                }
            }
        }
    }
}
