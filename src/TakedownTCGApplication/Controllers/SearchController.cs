using Microsoft.AspNetCore.Mvc;
using TakedownTCGApplication.Abstractions;
using TakedownTCGApplication.Models.Search;
using TakedownTCGApplication.ViewModels.Search;

namespace TakedownTCGApplication.Controllers;

public sealed class SearchController : Controller
{
    private readonly IProductsSearchWorkflow _productsSearchWorkflow;
    private const string EndpointCards = "cards";
    private const string EndpointSets = "sets";
    private const string EndpointGames = "games";

    public SearchController(IProductsSearchWorkflow productsSearchWorkflow)
    {
        _productsSearchWorkflow = productsSearchWorkflow;
    }

    [HttpGet]
    public IActionResult Index(string? endpoint = null)
    {
        return View(_productsSearchWorkflow.CreateSearchModel(endpoint));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ProductsSearchViewModel model)
    {
        ProductsSearchWorkflowResult result = await _productsSearchWorkflow.SearchAsync(model, User.Identity?.Name);
        foreach (SearchValidationError error in result.ValidationErrors)
        {
            ModelState.AddModelError(error.FieldName, error.Message);
        }

        return View(result.Model);
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
}
