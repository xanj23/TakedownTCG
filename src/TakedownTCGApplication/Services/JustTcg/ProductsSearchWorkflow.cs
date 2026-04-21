using TakedownTCGApplication.Abstractions;
using TakedownTCGApplication.Models.Ebay.Query;
using TakedownTCGApplication.Models.JustTcg.Query;
using TakedownTCGApplication.Models.PokemonTcg.Query;
using TakedownTCGApplication.Models.Search;

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

    public ProductsSearchRequest CreateSearchRequest(string? endpoint = null)
    {
        ProductsSearchRequest request = new();
        if (!string.IsNullOrWhiteSpace(endpoint))
        {
            request.Endpoint = NormalizeEndpoint(endpoint);
        }

        return request;
    }

    public async Task<ProductsSearchWorkflowResult> SearchAsync(
        ProductsSearchRequest request,
        string? userName,
        CancellationToken cancellationToken = default)
    {
        Normalize(request);
        IReadOnlyList<SearchValidationError> validationErrors = Validate(request);
        if (validationErrors.Count > 0)
        {
            return new ProductsSearchWorkflowResult
            {
                Request = request,
                ValidationErrors = validationErrors
            };
        }

        ProductsSearchOperationResult result = new();
        try
        {
            result = request.Api.ToLowerInvariant() switch
            {
                "pokemontcg" => await SearchPokemonAsync(request, userName, cancellationToken),
                "ebay" => await SearchEbayAsync(request, userName, cancellationToken),
                _ => await SearchJustTcgAsync(request, userName, cancellationToken)
            };
        }
        catch (Exception ex)
        {
            result.ErrorMessage = $"Search failed: {ex.Message}";
        }

        return new ProductsSearchWorkflowResult
        {
            Request = request,
            OperationResult = result
        };
    }

    private async Task<ProductsSearchOperationResult> SearchJustTcgAsync(
        ProductsSearchRequest request,
        string? userName,
        CancellationToken cancellationToken)
    {
        return request.Endpoint switch
        {
            EndpointCards => await _productsSearchService.SearchCardsAsync(
                BuildCardQuery(request),
                userName,
                request.Limit,
                cancellationToken),
            EndpointSets => await _productsSearchService.SearchSetsAsync(
                BuildSetQuery(request),
                request.Limit,
                cancellationToken),
            EndpointGames => await _productsSearchService.SearchGamesAsync(
                new GameQueryParams(),
                request.Limit,
                cancellationToken),
            _ => new ProductsSearchOperationResult()
        };
    }

    private async Task<ProductsSearchOperationResult> SearchPokemonAsync(
        ProductsSearchRequest request,
        string? userName,
        CancellationToken cancellationToken)
    {
        return await _pokemonProductsSearchService.SearchCardsAsync(
            BuildPokemonCardQuery(request),
            userName,
            cancellationToken);
    }

    private async Task<ProductsSearchOperationResult> SearchEbayAsync(
        ProductsSearchRequest request,
        string? userName,
        CancellationToken cancellationToken)
    {
        return request.Endpoint switch
        {
            EndpointCards => await _ebayProductsSearchService.SearchCardsAsync(
                BuildEbayItemSearchQuery(request),
                userName,
                cancellationToken),
            _ => new ProductsSearchOperationResult()
        };
    }

    private static void Normalize(ProductsSearchRequest request)
    {
        request.Api = NormalizeInput(request.Api);
        request.Endpoint = NormalizeEndpoint(request.Endpoint);
        request.CardQuery = NormalizeInput(request.CardQuery);
        request.CardNumber = NormalizeInput(request.CardNumber);
        request.CardPrinting = NormalizeInput(request.CardPrinting);
        request.CardCondition = NormalizeInput(request.CardCondition);
        request.CardOrderBy = NormalizeInput(request.CardOrderBy);
        request.CardOrder = NormalizeInput(request.CardOrder);
        request.PokemonSearch = NormalizeInput(request.PokemonSearch);
        request.PokemonTcgId = NormalizeInput(request.PokemonTcgId);
        request.PokemonSort = NormalizeInput(request.PokemonSort);
        request.EbaySearch = NormalizeInput(request.EbaySearch);
        request.EbayCategoryIds = NormalizeInput(request.EbayCategoryIds);
        request.EbayBuyingOptions = NormalizeInput(request.EbayBuyingOptions);
        request.EbaySort = NormalizeInput(request.EbaySort);
        request.SetGame = NormalizeInput(request.SetGame);
        request.SetQuery = NormalizeInput(request.SetQuery);
        request.SetOrderBy = NormalizeInput(request.SetOrderBy);
        request.SetOrder = NormalizeInput(request.SetOrder);
        request.Offset = Math.Max(0, request.Offset);
        request.Limit = request.Limit <= 0 ? 20 : Math.Min(request.Limit, 100);
        if (request.Api.Equals(PokemonTcgApi, StringComparison.OrdinalIgnoreCase))
        {
            request.Endpoint = EndpointCards;
        }
        else if (request.Api.Equals(EbayApi, StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(request.Endpoint))
        {
            request.Endpoint = EndpointCards;
        }
    }

    private static IReadOnlyList<SearchValidationError> Validate(ProductsSearchRequest request)
    {
        List<SearchValidationError> errors = new();

        if (!IsSupportedApi(request.Api))
        {
            errors.Add(new SearchValidationError(nameof(request.Api), "Please select a valid API."));
        }

        if (string.IsNullOrWhiteSpace(request.Endpoint))
        {
            errors.Add(new SearchValidationError(nameof(request.Endpoint), "Please select an endpoint."));
        }
        else if (!IsSupportedEndpointForApi(request.Api, request.Endpoint))
        {
            errors.Add(new SearchValidationError(nameof(request.Endpoint), GetUnsupportedEndpointMessage(request.Api)));
        }

        if (request.Api.Equals(PokemonTcgApi, StringComparison.OrdinalIgnoreCase)
            && request.Endpoint == EndpointCards
            && string.IsNullOrWhiteSpace(request.PokemonSearch)
            && string.IsNullOrWhiteSpace(request.PokemonTcgId))
        {
            errors.Add(new SearchValidationError(nameof(request.PokemonSearch), "Search text or Pokemon TCG ID is required for Pokemon cards."));
        }

        if (request.Api.Equals(JustTcgApi, StringComparison.OrdinalIgnoreCase)
            && request.Endpoint == EndpointCards
            && string.IsNullOrWhiteSpace(request.CardQuery))
        {
            errors.Add(new SearchValidationError(nameof(request.CardQuery), "Name is required for cards."));
        }

        if (request.Api.Equals(EbayApi, StringComparison.OrdinalIgnoreCase)
            && request.Endpoint == EndpointCards
            && string.IsNullOrWhiteSpace(request.EbaySearch))
        {
            errors.Add(new SearchValidationError(nameof(request.EbaySearch), "Search text is required for eBay listings."));
        }

        if (request.Api.Equals(JustTcgApi, StringComparison.OrdinalIgnoreCase)
            && request.Endpoint == EndpointSets
            && string.IsNullOrWhiteSpace(request.SetGame))
        {
            errors.Add(new SearchValidationError(nameof(request.SetGame), "Game is required for sets."));
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

    private static CardQueryParams BuildCardQuery(ProductsSearchRequest request)
    {
        CardQueryParams query = new();
        query.Parameters["q"].Value = request.CardQuery;
        query.Parameters["number"].Value = request.CardNumber;
        query.Parameters["printing"].Value = request.CardPrinting;
        query.Parameters["condition"].Value = request.CardCondition;
        query.Parameters["orderBy"].Value = request.CardOrderBy;
        query.Parameters["order"].Value = request.CardOrder;
        query.Parameters["limit"].Value = request.Limit;
        query.Parameters["offset"].Value = request.Offset;
        return query;
    }

    private static PokemonCardQueryParams BuildPokemonCardQuery(ProductsSearchRequest request)
    {
        return new PokemonCardQueryParams
        {
            Search = request.PokemonSearch,
            TcgId = request.PokemonTcgId,
            Sort = string.IsNullOrWhiteSpace(request.PokemonSort) ? "price_highest" : request.PokemonSort,
            Page = (request.Offset / Math.Max(1, request.Limit)) + 1,
            PerPage = request.Limit
        };
    }

    private static EbayItemSearchQueryParams BuildEbayItemSearchQuery(ProductsSearchRequest request)
    {
        return new EbayItemSearchQueryParams
        {
            Search = request.EbaySearch,
            CategoryIds = request.EbayCategoryIds,
            BuyingOptions = request.EbayBuyingOptions,
            Sort = string.IsNullOrWhiteSpace(request.EbaySort) ? "relevance" : request.EbaySort,
            Limit = request.Limit,
            Offset = request.Offset
        };
    }

    private static SetQueryParams BuildSetQuery(ProductsSearchRequest request)
    {
        SetQueryParams query = new();
        query.Parameters["game"].Value = request.SetGame;
        query.Parameters["q"].Value = request.SetQuery;
        query.Parameters["orderBy"].Value = request.SetOrderBy;
        query.Parameters["order"].Value = request.SetOrder;
        query.Parameters["limit"].Value = request.Limit;
        query.Parameters["offset"].Value = request.Offset;
        return query;
    }
}
