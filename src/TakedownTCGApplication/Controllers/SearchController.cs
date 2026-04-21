using Microsoft.AspNetCore.Mvc;
using TakedownTCG.Core.Abstractions;
using TakedownTCG.Core.Models.JustTcg.Query;
using TakedownTCG.Core.Models.JustTcg.Response;
using TakedownTCG.Core.Models.UserAccounts;
using TakedownTCG.Core.Services.JustTcg;
using TakedownTCGApplication.ViewModels.Search;

namespace TakedownTCGApplication.Controllers;

public sealed class SearchController : Controller
{
    private readonly IJustTcgSearchService _searchService;
    private readonly IFavoriteService _favoriteService;
    private const string JustTcgApi = "JustTCG";
    private const string EndpointCards = "cards";
    private const string EndpointSets = "sets";
    private const string EndpointGames = "games";

    public SearchController(IJustTcgSearchService searchService, IFavoriteService favoriteService)
    {
        _searchService = searchService;
        _favoriteService = favoriteService;
    }

    [HttpGet]
    public IActionResult Index(string? endpoint = null)
    {
        ProductsSearchViewModel model = new();
        if (!string.IsNullOrWhiteSpace(endpoint))
        {
            model.Endpoint = NormalizeEndpoint(endpoint);
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ProductsSearchViewModel model)
    {
        model.Api = NormalizeInput(model.Api);
        model.Endpoint = NormalizeEndpoint(model.Endpoint);
        model.CardQuery = NormalizeInput(model.CardQuery);
        model.CardNumber = NormalizeInput(model.CardNumber);
        model.CardPrinting = NormalizeInput(model.CardPrinting);
        model.CardCondition = NormalizeInput(model.CardCondition);
        model.CardOrderBy = NormalizeInput(model.CardOrderBy);
        model.CardOrder = NormalizeInput(model.CardOrder);
        model.SetGame = NormalizeInput(model.SetGame);
        model.SetQuery = NormalizeInput(model.SetQuery);
        model.SetOrderBy = NormalizeInput(model.SetOrderBy);
        model.SetOrder = NormalizeInput(model.SetOrder);
        model.Offset = Math.Max(0, model.Offset);
        model.Limit = model.Limit <= 0 ? 20 : Math.Min(model.Limit, 100);
        model.HasSearched = true;

        if (!model.Api.Equals(JustTcgApi, StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(model.Api), "Please select a valid API.");
        }

        if (string.IsNullOrWhiteSpace(model.Endpoint))
        {
            ModelState.AddModelError(nameof(model.Endpoint), "Please select an endpoint.");
        }

        if (model.Endpoint == EndpointCards && string.IsNullOrWhiteSpace(model.CardQuery))
        {
            ModelState.AddModelError(nameof(model.CardQuery), "Name is required for cards.");
        }

        if (model.Endpoint == EndpointSets && string.IsNullOrWhiteSpace(model.SetGame))
        {
            ModelState.AddModelError(nameof(model.SetGame), "Game is required for sets.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            switch (model.Endpoint)
            {
                case EndpointCards:
                    await SearchCardsAsync(model);
                    break;
                case EndpointSets:
                    await SearchSetsAsync(model);
                    break;
                case EndpointGames:
                    await SearchGamesAsync(model);
                    break;
                default:
                    ModelState.AddModelError(nameof(model.Endpoint), "Unsupported endpoint selected.");
                    break;
            }
        }
        catch (Exception ex)
        {
            model.ErrorMessage = $"Search failed: {ex.Message}";
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Cards()
    {
        return RedirectToAction(nameof(Index), new { endpoint = EndpointCards });
    }

    [HttpGet]
    public IActionResult Sets()
    {
        return RedirectToAction(nameof(Index), new { endpoint = EndpointSets });
    }

    [HttpGet]
    public IActionResult Games()
    {
        return RedirectToAction(nameof(Index), new { endpoint = EndpointGames });
    }

    private static string NormalizeInput(string? value)
    {
        return value?.Trim() ?? string.Empty;
    }

    private static string NormalizeEndpoint(string? endpoint)
    {
        return (endpoint ?? string.Empty).Trim().ToLowerInvariant();
    }

    private async Task SearchCardsAsync(ProductsSearchViewModel model)
    {
        CardQueryParams query = new();
        query.Parameters["q"].Value = model.CardQuery;
        query.Parameters["number"].Value = model.CardNumber;
        query.Parameters["printing"].Value = model.CardPrinting;
        query.Parameters["condition"].Value = model.CardCondition;
        query.Parameters["orderBy"].Value = model.CardOrderBy;
        query.Parameters["order"].Value = model.CardOrder;
        query.Parameters["limit"].Value = model.Limit;
        query.Parameters["offset"].Value = model.Offset;

        Response<Card> response = (Response<Card>)await _searchService.SearchAsync(JustTcgEndpoint.Cards, query);
        model.CardResults = response.Data;
        model.ErrorMessage = response.Error ?? string.Empty;
        ApplyPaging(model, response.Meta);

        string? userName = User.Identity?.Name;
        if (!string.IsNullOrWhiteSpace(userName))
        {
            IReadOnlyList<Favorite> favorites = await _favoriteService.GetFavoritesAsync(userName);
            model.FavoriteCardIds = favorites
                .Where(f => string.Equals(f.ItemType, "card", StringComparison.OrdinalIgnoreCase))
                .Select(f => f.ItemId)
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }
    }

    private async Task SearchSetsAsync(ProductsSearchViewModel model)
    {
        SetQueryParams query = new();
        query.Parameters["game"].Value = model.SetGame;
        query.Parameters["q"].Value = model.SetQuery;
        query.Parameters["orderBy"].Value = model.SetOrderBy;
        query.Parameters["order"].Value = model.SetOrder;
        query.Parameters["limit"].Value = model.Limit;
        query.Parameters["offset"].Value = model.Offset;

        Response<Set> response = (Response<Set>)await _searchService.SearchAsync(JustTcgEndpoint.Sets, query);
        model.SetResults = response.Data;
        model.ErrorMessage = response.Error ?? string.Empty;
        ApplyPaging(model, response.Meta);
    }

    private async Task SearchGamesAsync(ProductsSearchViewModel model)
    {
        Response<Game> response = (Response<Game>)await _searchService.SearchAsync(JustTcgEndpoint.Games, new GameQueryParams());
        model.GameResults = response.Data;
        model.ErrorMessage = response.Error ?? string.Empty;
        ApplyPaging(model, response.Meta);
    }

    private static void ApplyPaging(ProductsSearchViewModel model, Meta meta)
    {
        model.Total = meta.Total;
        model.Offset = meta.Offset;
        model.Limit = meta.Limit > 0 ? meta.Limit : model.Limit;
        model.HasMore = meta.HasMore;
        model.NextOffset = model.Offset + model.Limit;
        model.HasPrevious = model.Offset > 0;
        model.PreviousOffset = Math.Max(0, model.Offset - model.Limit);
        model.CurrentPage = (model.Offset / model.Limit) + 1;
        model.TotalPages = Math.Max(1, (int)Math.Ceiling((double)model.Total / model.Limit));
    }
}
