using TakedownTCG.cli.Controllers;
using TakedownTCG.cli.Infrastructure.Config;
using TakedownTCG.cli.Infrastructure.Http;
using TakedownTCG.cli.Models.JustTcg.Response;
using TakedownTCG.cli.Services.Api;
using TakedownTCG.cli.Views.Input;
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

    public JustTCGClient(
        JustTcgApiConfig config,
        JustTcgHttpGateway httpGateway,
        JustTcgQueryService queryService,
        JustTcgResponseService responseService)
    {
        _config = config;
        _httpGateway = httpGateway;
        _queryService = queryService;
        _responseService = responseService;
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

            OfferToFavorite(responseData);
        }
    }

    private object Search(Action action)
    {
        var query = _queryService.InputQuery(action);
        string url = _queryService.BuildUrl(action, query, _config.BaseUrl);
        string responseContent = _httpGateway.FetchResponse(url, _config.ApiKeyHeaderName, _config.ApiKey);
        return _responseService.Deserialize(action, responseContent);
    }

    private static void OfferToFavorite(object responseData)
    {
        try
        {
            Console.WriteLine();
            string? favInput = UserInput.InputString("Add a result to favorites? Enter result number or press Enter to skip");
            if (string.IsNullOrWhiteSpace(favInput))
            {
                return;
            }

            if (!int.TryParse(favInput.Trim(), out int favIndex))
            {
                Console.WriteLine("Invalid number.");
                return;
            }

            var current = UserAccountController.CurrentUser;
            if (current is null)
            {
                Console.WriteLine("Login to add favorites.");
                return;
            }

            if (responseData is Response<Card> cardResp)
            {
                if (!IsIndexInRange(favIndex, cardResp.Data.Count))
                {
                    Console.WriteLine("Index out of range.");
                    return;
                }

                Card card = cardResp.Data[favIndex - 1];
                bool added = UserAccountController.FavoriteService.AddFavorite(current.UserName, "Card", card.Id, card.Name);

                Console.WriteLine(added ? "Added to favorites." : "Already favorited or failed.");
                return;
            }

            if (responseData is Response<Set> setResp)
            {
                if (!IsIndexInRange(favIndex, setResp.Data.Count))
                {
                    Console.WriteLine("Index out of range.");
                    return;
                }

                Set set = setResp.Data[favIndex - 1];
                bool added = UserAccountController.FavoriteService.AddFavorite(current.UserName, "Set", set.Id, set.Name);

                Console.WriteLine(added ? "Added to favorites." : "Already favorited or failed.");
                return;
            }

            if (responseData is Response<Game> gameResp)
            {
                if (!IsIndexInRange(favIndex, gameResp.Data.Count))
                {
                    Console.WriteLine("Index out of range.");
                    return;
                }

                Game game = gameResp.Data[favIndex - 1];
                bool added = UserAccountController.FavoriteService.AddFavorite(current.UserName, "Game", game.Id, game.Name);

                Console.WriteLine(added ? "Added to favorites." : "Already favorited or failed.");
                return;
            }

            Console.WriteLine("Favoriting is not supported for this response type.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to add favorite: {ex.Message}");
        }
    }

    private static bool IsIndexInRange(int index, int count)
    {
        return index >= 1 && index <= count;
    }
}
