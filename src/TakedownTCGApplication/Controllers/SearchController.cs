using Microsoft.AspNetCore.Mvc;
using TakedownTCGApplication.Abstractions;
using TakedownTCGApplication.Models.JustTcg.Query;
using TakedownTCGApplication.Models.Search;
using TakedownTCGApplication.ViewModels.Search;

namespace TakedownTCGApplication.Controllers;

public sealed class SearchController : Controller
{
    private readonly IProductsSearchService _productsSearchService;
    private const string JustTcgApi = "JustTCG";
    private const string EndpointCards = "cards";
    private const string EndpointSets = "sets";
    private const string EndpointGames = "games";

    public SearchController(IProductsSearchService productsSearchService)
    {
        _productsSearchService = productsSearchService;
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
                    ProductsSearchOperationResult cardsResult = await _productsSearchService.SearchCardsAsync(
                        BuildCardQuery(model),
                        User.Identity?.Name,
                        model.Limit);
                    ApplyOperationResult(model, cardsResult);
                    break;
                case EndpointSets:
                    ProductsSearchOperationResult setsResult = await _productsSearchService.SearchSetsAsync(
                        BuildSetQuery(model),
                        model.Limit);
                    ApplyOperationResult(model, setsResult);
                    break;
                case EndpointGames:
                    ProductsSearchOperationResult gamesResult = await _productsSearchService.SearchGamesAsync(
                        new GameQueryParams(),
                        model.Limit);
                    ApplyOperationResult(model, gamesResult);
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

    private static CardQueryParams BuildCardQuery(ProductsSearchViewModel model)
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
        return query;
    }

    private static SetQueryParams BuildSetQuery(ProductsSearchViewModel model)
    {
        SetQueryParams query = new();
        query.Parameters["game"].Value = model.SetGame;
        query.Parameters["q"].Value = model.SetQuery;
        query.Parameters["orderBy"].Value = model.SetOrderBy;
        query.Parameters["order"].Value = model.SetOrder;
        query.Parameters["limit"].Value = model.Limit;
        query.Parameters["offset"].Value = model.Offset;
        return query;
    }

    private static void ApplyOperationResult(ProductsSearchViewModel model, ProductsSearchOperationResult result)
    {
        model.CardResults = result.CardResults;
        model.SetResults = result.SetResults;
        model.GameResults = result.GameResults;
        model.CardDisplayResults = result.CardDisplayResults;
        model.SetDisplayResults = result.SetDisplayResults;
        model.GameDisplayResults = result.GameDisplayResults;
        model.FavoriteCardIds = result.FavoriteCardIds;
        model.ErrorMessage = result.ErrorMessage;
        model.Total = result.Total;
        model.Offset = result.Offset;
        model.Limit = result.Limit;
        model.HasMore = result.HasMore;
        model.HasPrevious = result.HasPrevious;
        model.PreviousOffset = result.PreviousOffset;
        model.NextOffset = result.NextOffset;
        model.CurrentPage = result.CurrentPage;
        model.TotalPages = result.TotalPages;
    }

}
