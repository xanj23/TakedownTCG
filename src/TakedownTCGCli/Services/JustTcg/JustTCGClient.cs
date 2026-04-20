using TakedownTCG.Core.Abstractions;
using TakedownTCG.Core.Infrastructure.Config;
using TakedownTCG.Core.Models.JustTcg.Query;
using TakedownTCG.Core.Models.JustTcg.Response;
using TakedownTCG.Core.Services.JustTcg;
using TakedownTCG.cli.Controllers;
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
    private readonly JustTcgApiOptions _config;
    private readonly IJustTcgSearchService _searchService;
    private readonly IJustTcgResponseMapper _responseMapper;

    public JustTCGClient(
        JustTcgApiOptions config,
        IJustTcgSearchService searchService,
        IJustTcgResponseMapper responseMapper)
    {
        _config = config;
        _searchService = searchService;
        _responseMapper = responseMapper;
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

            IQueryParams query = InputQuery(ToEndpoint(selectedAction));
            object responseData = _searchService.SearchAsync(ToEndpoint(selectedAction), query).GetAwaiter().GetResult();
            string mappedData = _responseMapper.Map(responseData);
            JustTcgOutputView.DisplayMappedData(mappedData);

            OfferToFavorite(responseData);
        }
    }

    private static JustTcgEndpoint ToEndpoint(Action action)
    {
        return action switch
        {
            Action.Cards => JustTcgEndpoint.Cards,
            Action.Sets => JustTcgEndpoint.Sets,
            Action.Games => JustTcgEndpoint.Games,
            _ => throw new NotSupportedException($"Unsupported action: {action}")
        };
    }

    private static IQueryParams InputQuery(JustTcgEndpoint endpoint)
    {
        IQueryParams query = endpoint switch
        {
            JustTcgEndpoint.Cards => new CardQueryParams(),
            JustTcgEndpoint.Sets => new SetQueryParams(),
            JustTcgEndpoint.Games => new GameQueryParams(),
            _ => throw new NotSupportedException($"Unsupported endpoint: {endpoint}")
        };

        Console.WriteLine();
        Console.WriteLine("Input search parameters:");

        foreach (KeyValuePair<string, QueryParameter> kvp in query.Parameters)
        {
            QueryParameter param = kvp.Value;
            param.Value = param.IsRequired
                ? UserInput.InputRequiredString(param.Label)
                : UserInput.InputString($"{param.Label} (optional)");
        }

        return query;
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
                bool added = UserAccountController.FavoriteService
                    .AddFavoriteAsync(current.UserName, "Card", card.Id, card.Name)
                    .GetAwaiter()
                    .GetResult();

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
                bool added = UserAccountController.FavoriteService
                    .AddFavoriteAsync(current.UserName, "Set", set.Id, set.Name)
                    .GetAwaiter()
                    .GetResult();

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
                bool added = UserAccountController.FavoriteService
                    .AddFavoriteAsync(current.UserName, "Game", game.Id, game.Name)
                    .GetAwaiter()
                    .GetResult();

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
