using System;
using System.Net.Http;
using TakedownTCG.cli.Api.JustTCG.Query;
using TakedownTCG.cli.Api.JustTCG.Response;
using TakedownTCG.cli.Api;
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

                // Offer to add a displayed result to favorites for logged-in users
                try
                {
                    Console.WriteLine();
                    string favInput = TakedownTCG.cli.Util.UserInput.InputString("Add a result to favorites? Enter result number or press Enter to skip");
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

                            var card = cardResp.Data[favIndex - 1];
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

                            var set = setResp.Data[favIndex - 1];
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

                            var game = gameResp.Data[favIndex - 1];
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
