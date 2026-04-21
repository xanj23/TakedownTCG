using Microsoft.AspNetCore.Mvc;
using TakedownTCGApplication.Abstractions;
using TakedownTCGApplication.Models.Search;
using TakedownTCGApplication.ViewModels.Search;

namespace TakedownTCGApplication.Controllers;

public sealed class SearchController : Controller
{
    private readonly IProductsSearchWorkflow _productsSearchWorkflow;
    private const string JustTcgApi = "JustTCG";
    private const string PokemonTcgApi = "PokemonTCG";
    private const string EbayApi = "eBay";
    private const string EndpointCards = "cards";
    private const string EndpointSets = "sets";
    private const string EndpointGames = "games";
    private const string EbayTradingCardCategoryIds = "183454";

    public SearchController(IProductsSearchWorkflow productsSearchWorkflow)
    {
        _productsSearchWorkflow = productsSearchWorkflow;
    }

    [HttpGet]
    public IActionResult Index(string? endpoint = null)
    {
        return View(ToViewModel(_productsSearchWorkflow.CreateSearchRequest(endpoint)));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ProductsSearchViewModel model)
    {
        ProductsSearchWorkflowResult result = await _productsSearchWorkflow.SearchAsync(ToRequest(model), User.Identity?.Name);
        foreach (SearchValidationError error in result.ValidationErrors)
        {
            ModelState.AddModelError(error.FieldName, error.Message);
        }

        return View(ToViewModel(result));
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

    private static ProductsSearchRequest ToRequest(ProductsSearchViewModel model)
    {
        return new ProductsSearchRequest
        {
            Api = model.Api,
            Endpoint = model.Endpoint,
            CardQuery = model.CardQuery,
            CardNumber = model.CardNumber,
            CardPrinting = model.CardPrinting,
            CardCondition = model.CardCondition,
            CardOrderBy = model.CardOrderBy,
            CardOrder = model.CardOrder,
            PokemonSearch = model.PokemonSearch,
            PokemonTcgId = model.PokemonTcgId,
            PokemonSort = model.PokemonSort,
            EbaySearch = model.EbaySearch,
            EbayCategoryIds = model.EbayCategoryIds,
            EbayBuyingOptions = model.EbayBuyingOptions,
            EbaySort = model.EbaySort,
            SetGame = model.SetGame,
            SetQuery = model.SetQuery,
            SetOrderBy = model.SetOrderBy,
            SetOrder = model.SetOrder,
            Offset = model.Offset,
            Limit = model.Limit
        };
    }

    private static ProductsSearchViewModel ToViewModel(ProductsSearchRequest request)
    {
        return ToViewModel(new ProductsSearchWorkflowResult { Request = request }, hasSearched: false);
    }

    private static ProductsSearchViewModel ToViewModel(ProductsSearchWorkflowResult result)
    {
        return ToViewModel(result, hasSearched: true);
    }

    private static ProductsSearchViewModel ToViewModel(ProductsSearchWorkflowResult result, bool hasSearched)
    {
        ProductsSearchRequest request = result.Request;
        ProductsSearchOperationResult operationResult = result.OperationResult;

        return new ProductsSearchViewModel
        {
            Api = request.Api,
            Endpoint = request.Endpoint,
            CardQuery = request.CardQuery,
            CardNumber = request.CardNumber,
            CardPrinting = request.CardPrinting,
            CardCondition = request.CardCondition,
            CardOrderBy = request.CardOrderBy,
            CardOrder = request.CardOrder,
            PokemonSearch = request.PokemonSearch,
            PokemonTcgId = request.PokemonTcgId,
            PokemonSort = request.PokemonSort,
            EbaySearch = request.EbaySearch,
            EbayCategoryIds = request.EbayCategoryIds,
            EbayBuyingOptions = request.EbayBuyingOptions,
            EbaySort = request.EbaySort,
            SetGame = request.SetGame,
            SetQuery = request.SetQuery,
            SetOrderBy = request.SetOrderBy,
            SetOrder = request.SetOrder,
            Offset = operationResult.Offset,
            Limit = operationResult.Limit,
            NextOffset = operationResult.NextOffset,
            PreviousOffset = operationResult.PreviousOffset,
            Total = operationResult.Total,
            HasMore = operationResult.HasMore,
            HasPrevious = operationResult.HasPrevious,
            CurrentPage = operationResult.CurrentPage,
            TotalPages = operationResult.TotalPages,
            HasSearched = hasSearched,
            CardDisplayResults = operationResult.CardDisplayResults,
            SetDisplayResults = operationResult.SetDisplayResults,
            GameDisplayResults = operationResult.GameDisplayResults,
            ErrorMessage = operationResult.ErrorMessage,
            ApiOptions = CreateApiOptions(),
            EndpointOptions = CreateEndpointOptions()
        };
    }

    private static IReadOnlyList<SearchApiOptionViewModel> CreateApiOptions()
    {
        return
        [
            new SearchApiOptionViewModel
            {
                Value = JustTcgApi,
                Label = "JustTCG",
                EndpointLabel = "Endpoint",
                RequiresEndpointSelection = true
            },
            new SearchApiOptionViewModel
            {
                Value = PokemonTcgApi,
                Label = "Pokemon API",
                DefaultEndpoint = EndpointCards,
                RequiresEndpointSelection = false
            },
            new SearchApiOptionViewModel
            {
                Value = EbayApi,
                Label = "eBay",
                EndpointLabel = "Category",
                DefaultEndpoint = EndpointCards,
                RequiresEndpointSelection = true
            }
        ];
    }

    private static IReadOnlyList<SearchEndpointOptionViewModel> CreateEndpointOptions()
    {
        return
        [
            new SearchEndpointOptionViewModel
            {
                Label = "Select endpoint",
                SupportedApis = JustTcgApi
            },
            new SearchEndpointOptionViewModel
            {
                Label = "All",
                SupportedApis = EbayApi
            },
            new SearchEndpointOptionViewModel
            {
                Value = EndpointCards,
                Label = "Cards",
                SupportedApis = string.Join('|', [JustTcgApi, PokemonTcgApi, EbayApi]),
                CategoryIds = EbayTradingCardCategoryIds
            },
            new SearchEndpointOptionViewModel
            {
                Value = EndpointSets,
                Label = "Sets",
                SupportedApis = JustTcgApi
            },
            new SearchEndpointOptionViewModel
            {
                Value = EndpointGames,
                Label = "Games",
                SupportedApis = JustTcgApi
            }
        ];
    }
}
