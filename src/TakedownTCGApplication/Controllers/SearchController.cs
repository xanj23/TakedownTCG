using Microsoft.AspNetCore.Mvc;
using TakedownTCG.Core.Abstractions;
using TakedownTCG.Core.Models.JustTcg.Query;
using TakedownTCG.Core.Models.JustTcg.Response;
using TakedownTCG.Core.Services.JustTcg;
using TakedownTCGApplication.ViewModels.Search;

namespace TakedownTCGApplication.Controllers;

public sealed class SearchController : Controller
{
    private readonly IJustTcgSearchService _searchService;

    public SearchController(IJustTcgSearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return RedirectToAction(nameof(Cards));
    }

    [HttpGet]
    public IActionResult Cards()
    {
        return View(new CardsSearchViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cards(CardsSearchViewModel model)
    {
        model.Query = NormalizeInput(model.Query);
        model.Number = NormalizeInput(model.Number);
        model.Printing = NormalizeInput(model.Printing);
        model.Condition = NormalizeInput(model.Condition);
        model.Offset = Math.Max(0, model.Offset);
        model.Limit = model.Limit <= 0 ? 20 : Math.Min(model.Limit, 100);

        if (string.IsNullOrWhiteSpace(model.Query))
        {
            ModelState.AddModelError(nameof(model.Query), "Name is required.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        CardQueryParams query = new();
        query.Parameters["q"].Value = model.Query;
        query.Parameters["number"].Value = model.Number;
        query.Parameters["printing"].Value = model.Printing;
        query.Parameters["condition"].Value = model.Condition;
        query.Parameters["limit"].Value = model.Limit;
        query.Parameters["offset"].Value = model.Offset;

        try
        {
            Response<Card> response = (Response<Card>)await _searchService.SearchAsync(JustTcgEndpoint.Cards, query);
            model.Results = response.Data;
            model.ErrorMessage = response.Error ?? string.Empty;
            model.Total = response.Meta.Total;
            model.Offset = response.Meta.Offset;
            model.Limit = response.Meta.Limit > 0 ? response.Meta.Limit : model.Limit;
            model.HasMore = response.Meta.HasMore;
            model.NextOffset = model.Offset + model.Limit;
            model.HasPrevious = model.Offset > 0;
            model.PreviousOffset = Math.Max(0, model.Offset - model.Limit);
            model.CurrentPage = (model.Offset / model.Limit) + 1;
            model.TotalPages = Math.Max(1, (int)Math.Ceiling((double)model.Total / model.Limit));
        }
        catch (Exception ex)
        {
            model.ErrorMessage = $"Search failed: {ex.Message}";
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Sets()
    {
        return View(new SetsSearchViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sets(SetsSearchViewModel model)
    {
        model.Game = NormalizeInput(model.Game);
        model.Query = NormalizeInput(model.Query);
        model.OrderBy = NormalizeInput(model.OrderBy);
        model.Order = NormalizeInput(model.Order);

        if (string.IsNullOrWhiteSpace(model.Game))
        {
            ModelState.AddModelError(nameof(model.Game), "Game is required.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        SetQueryParams query = new();
        query.Parameters["game"].Value = model.Game;
        query.Parameters["q"].Value = model.Query;
        query.Parameters["orderBy"].Value = model.OrderBy;
        query.Parameters["order"].Value = model.Order;

        try
        {
            Response<Set> response = (Response<Set>)await _searchService.SearchAsync(JustTcgEndpoint.Sets, query);
            model.Results = response.Data;
            model.ErrorMessage = response.Error ?? string.Empty;
        }
        catch (Exception ex)
        {
            model.ErrorMessage = $"Search failed: {ex.Message}";
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Games()
    {
        return View(new GamesSearchViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Games(GamesSearchViewModel model)
    {
        try
        {
            Response<Game> response = (Response<Game>)await _searchService.SearchAsync(JustTcgEndpoint.Games, new GameQueryParams());
            model.Results = response.Data;
            model.ErrorMessage = response.Error ?? string.Empty;
        }
        catch (Exception ex)
        {
            model.ErrorMessage = $"Search failed: {ex.Message}";
        }

        return View(model);
    }

    private static string NormalizeInput(string? value)
    {
        return value?.Trim() ?? string.Empty;
    }
}
