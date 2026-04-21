using TakedownTCGApplication.Abstractions;
using TakedownTCGApplication.Models.Ebay.Query;
using TakedownTCGApplication.Models.JustTcg.Query;
using TakedownTCGApplication.Models.PokemonTcg.Query;
using TakedownTCGApplication.Models.Search;
using TakedownTCGApplication.ViewModels.Search;

namespace TakedownTCGApplication.Services.JustTcg;

public sealed class ProductsSearchWorkflow : IProductsSearchWorkflow
{
    private readonly IProductsSearchService _productsSearchService;
    private readonly IPokemonProductsSearchService _pokemonProductsSearchService;
    private readonly IEbayProductsSearchService _ebayProductsSearchService;
    private const string JustTcgApi = "JustTCG";
    private const string PokemonTcgApi = "PokemonTCG";
    private const string EbayApi = "eBay";
    private const string EndpointCards = "cards";
    private const string EndpointSets = "sets";
    private const string EndpointGames = "games";

    public ProductsSearchWorkflow(
        IProductsSearchService productsSearchService,
        IPokemonProductsSearchService pokemonProductsSearchService,
        IEbayProductsSearchService ebayProductsSearchService)
    {
        _productsSearchService = productsSearchService;
        _pokemonProductsSearchService = pokemonProductsSearchService;
        _ebayProductsSearchService = ebayProductsSearchService;
    }

    public ProductsSearchViewModel CreateSearchModel(string? endpoint = null)
    {
        ProductsSearchViewModel model = new();
        if (!string.IsNullOrWhiteSpace(endpoint))
        {
            model.Endpoint = NormalizeEndpoint(endpoint);
        }

        return model;
    }

    public async Task<ProductsSearchWorkflowResult> SearchAsync(
        ProductsSearchViewModel model,
        string? userName,
        CancellationToken cancellationToken = default)
    {
        Normalize(model);
        IReadOnlyList<SearchValidationError> validationErrors = Validate(model);
        if (validationErrors.Count > 0)
        {
            return new ProductsSearchWorkflowResult
            {
                Model = model,
                ValidationErrors = validationErrors
            };
        }

        try
        {
            ProductsSearchOperationResult result = model.Api.ToLowerInvariant() switch
            {
                "pokemontcg" => await SearchPokemonAsync(model, userName, cancellationToken),
                "ebay" => await SearchEbayAsync(model, userName, cancellationToken),
                _ => await SearchJustTcgAsync(model, userName, cancellationToken)
            };

            ApplyOperationResult(model, result);
        }
        catch (Exception ex)
        {
            model.ErrorMessage = $"Search failed: {ex.Message}";
        }

        return new ProductsSearchWorkflowResult { Model = model };
    }

    private async Task<ProductsSearchOperationResult> SearchJustTcgAsync(
        ProductsSearchViewModel model,
        string? userName,
        CancellationToken cancellationToken)
    {
        return model.Endpoint switch
        {
            EndpointCards => await _productsSearchService.SearchCardsAsync(
                BuildCardQuery(model),
                userName,
                model.Limit,
                cancellationToken),
            EndpointSets => await _productsSearchService.SearchSetsAsync(
                BuildSetQuery(model),
                model.Limit,
                cancellationToken),
            EndpointGames => await _productsSearchService.SearchGamesAsync(
                new GameQueryParams(),
                model.Limit,
                cancellationToken),
            _ => new ProductsSearchOperationResult()
        };
    }

    private async Task<ProductsSearchOperationResult> SearchPokemonAsync(
        ProductsSearchViewModel model,
        string? userName,
        CancellationToken cancellationToken)
    {
        return await _pokemonProductsSearchService.SearchCardsAsync(
            BuildPokemonCardQuery(model),
            userName,
            cancellationToken);
    }

    private async Task<ProductsSearchOperationResult> SearchEbayAsync(
        ProductsSearchViewModel model,
        string? userName,
        CancellationToken cancellationToken)
    {
        return model.Endpoint switch
        {
            EndpointCards => await _ebayProductsSearchService.SearchCardsAsync(
                BuildEbayItemSearchQuery(model),
                userName,
                cancellationToken),
            _ => new ProductsSearchOperationResult()
        };
    }

    private static void Normalize(ProductsSearchViewModel model)
    {
        model.Api = NormalizeInput(model.Api);
        model.Endpoint = NormalizeEndpoint(model.Endpoint);
        model.CardQuery = NormalizeInput(model.CardQuery);
        model.CardNumber = NormalizeInput(model.CardNumber);
        model.CardPrinting = NormalizeInput(model.CardPrinting);
        model.CardCondition = NormalizeInput(model.CardCondition);
        model.CardOrderBy = NormalizeInput(model.CardOrderBy);
        model.CardOrder = NormalizeInput(model.CardOrder);
        model.PokemonSearch = NormalizeInput(model.PokemonSearch);
        model.PokemonTcgId = NormalizeInput(model.PokemonTcgId);
        model.PokemonSort = NormalizeInput(model.PokemonSort);
        model.EbaySearch = NormalizeInput(model.EbaySearch);
        model.EbayCategoryIds = NormalizeInput(model.EbayCategoryIds);
        model.EbayBuyingOptions = NormalizeInput(model.EbayBuyingOptions);
        model.EbaySort = NormalizeInput(model.EbaySort);
        model.SetGame = NormalizeInput(model.SetGame);
        model.SetQuery = NormalizeInput(model.SetQuery);
        model.SetOrderBy = NormalizeInput(model.SetOrderBy);
        model.SetOrder = NormalizeInput(model.SetOrder);
        model.Offset = Math.Max(0, model.Offset);
        model.Limit = model.Limit <= 0 ? 20 : Math.Min(model.Limit, 100);
        if (model.Api.Equals(PokemonTcgApi, StringComparison.OrdinalIgnoreCase))
        {
            model.Endpoint = EndpointCards;
        }
        else if (model.Api.Equals(EbayApi, StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(model.Endpoint))
        {
            model.Endpoint = EndpointCards;
        }

        model.HasSearched = true;
    }

    private static IReadOnlyList<SearchValidationError> Validate(ProductsSearchViewModel model)
    {
        List<SearchValidationError> errors = new();

        if (!IsSupportedApi(model.Api))
        {
            errors.Add(new SearchValidationError(nameof(model.Api), "Please select a valid API."));
        }

        if (string.IsNullOrWhiteSpace(model.Endpoint))
        {
            errors.Add(new SearchValidationError(nameof(model.Endpoint), "Please select an endpoint."));
        }
        else if (!IsSupportedEndpointForApi(model.Api, model.Endpoint))
        {
            errors.Add(new SearchValidationError(nameof(model.Endpoint), GetUnsupportedEndpointMessage(model.Api)));
        }

        if (model.Api.Equals(PokemonTcgApi, StringComparison.OrdinalIgnoreCase)
            && model.Endpoint == EndpointCards
            && string.IsNullOrWhiteSpace(model.PokemonSearch)
            && string.IsNullOrWhiteSpace(model.PokemonTcgId))
        {
            errors.Add(new SearchValidationError(nameof(model.PokemonSearch), "Search text or Pokemon TCG ID is required for Pokemon cards."));
        }

        if (model.Api.Equals(JustTcgApi, StringComparison.OrdinalIgnoreCase)
            && model.Endpoint == EndpointCards
            && string.IsNullOrWhiteSpace(model.CardQuery))
        {
            errors.Add(new SearchValidationError(nameof(model.CardQuery), "Name is required for cards."));
        }

        if (model.Api.Equals(EbayApi, StringComparison.OrdinalIgnoreCase)
            && model.Endpoint == EndpointCards
            && string.IsNullOrWhiteSpace(model.EbaySearch))
        {
            errors.Add(new SearchValidationError(nameof(model.EbaySearch), "Search text is required for eBay listings."));
        }

        if (model.Api.Equals(JustTcgApi, StringComparison.OrdinalIgnoreCase)
            && model.Endpoint == EndpointSets
            && string.IsNullOrWhiteSpace(model.SetGame))
        {
            errors.Add(new SearchValidationError(nameof(model.SetGame), "Game is required for sets."));
        }

        return errors;
    }

    private static bool IsSupportedApi(string api)
    {
        return api.Equals(JustTcgApi, StringComparison.OrdinalIgnoreCase)
               || api.Equals(PokemonTcgApi, StringComparison.OrdinalIgnoreCase)
               || api.Equals(EbayApi, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsSupportedEndpoint(string endpoint)
    {
        return endpoint is EndpointCards or EndpointSets or EndpointGames;
    }

    private static bool IsSupportedEndpointForApi(string api, string endpoint)
    {
        if (api.Equals(PokemonTcgApi, StringComparison.OrdinalIgnoreCase))
        {
            return endpoint == EndpointCards;
        }

        if (api.Equals(EbayApi, StringComparison.OrdinalIgnoreCase))
        {
            return endpoint == EndpointCards;
        }

        return api.Equals(JustTcgApi, StringComparison.OrdinalIgnoreCase) && IsSupportedEndpoint(endpoint);
    }

    private static string GetUnsupportedEndpointMessage(string api)
    {
        if (api.Equals(PokemonTcgApi, StringComparison.OrdinalIgnoreCase))
        {
            return "Pokemon API currently supports card search only.";
        }

        return api.Equals(EbayApi, StringComparison.OrdinalIgnoreCase)
            ? "eBay API currently supports card listing search only."
            : "Unsupported endpoint selected.";
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

    private static PokemonCardQueryParams BuildPokemonCardQuery(ProductsSearchViewModel model)
    {
        return new PokemonCardQueryParams
        {
            Search = model.PokemonSearch,
            TcgId = model.PokemonTcgId,
            Sort = string.IsNullOrWhiteSpace(model.PokemonSort) ? "price_highest" : model.PokemonSort,
            Page = (model.Offset / Math.Max(1, model.Limit)) + 1,
            PerPage = model.Limit
        };
    }

    private static EbayItemSearchQueryParams BuildEbayItemSearchQuery(ProductsSearchViewModel model)
    {
        return new EbayItemSearchQueryParams
        {
            Search = model.EbaySearch,
            CategoryIds = model.EbayCategoryIds,
            BuyingOptions = model.EbayBuyingOptions,
            Sort = string.IsNullOrWhiteSpace(model.EbaySort) ? "relevance" : model.EbaySort,
            Limit = model.Limit,
            Offset = model.Offset
        };
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
        model.CardDisplayResults = result.CardDisplayResults;
        model.SetDisplayResults = result.SetDisplayResults;
        model.GameDisplayResults = result.GameDisplayResults;
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
