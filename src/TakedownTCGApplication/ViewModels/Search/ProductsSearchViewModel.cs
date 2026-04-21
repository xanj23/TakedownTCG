using TakedownTCGApplication.Models.Search;

namespace TakedownTCGApplication.ViewModels.Search;

public sealed class ProductsSearchViewModel
{
    public string Api { get; set; } = "JustTCG";
    public string Endpoint { get; set; } = string.Empty;

    public string? CardQuery { get; set; }
    public string? CardNumber { get; set; }
    public string? CardPrinting { get; set; }
    public string? CardCondition { get; set; }
    public string? CardOrderBy { get; set; }
    public string? CardOrder { get; set; }

    public string? PokemonSearch { get; set; }
    public string? PokemonTcgId { get; set; }
    public string? PokemonSort { get; set; }

    public string? EbaySearch { get; set; }
    public string? EbayCategoryIds { get; set; }
    public string? EbayBuyingOptions { get; set; }
    public string? EbaySort { get; set; }

    public string? SetGame { get; set; }
    public string? SetQuery { get; set; }
    public string? SetOrderBy { get; set; }
    public string? SetOrder { get; set; }

    public int Offset { get; set; }
    public int Limit { get; set; } = 20;
    public int NextOffset { get; set; }
    public int PreviousOffset { get; set; }
    public int Total { get; set; }
    public bool HasMore { get; set; }
    public bool HasPrevious { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public bool HasSearched { get; set; }

    public IReadOnlyList<CardSearchResult> CardDisplayResults { get; set; } = Array.Empty<CardSearchResult>();
    public IReadOnlyList<SetSearchResult> SetDisplayResults { get; set; } = Array.Empty<SetSearchResult>();
    public IReadOnlyList<GameSearchResult> GameDisplayResults { get; set; } = Array.Empty<GameSearchResult>();
    public string ErrorMessage { get; set; } = string.Empty;

    public IReadOnlyList<SearchApiOptionViewModel> ApiOptions { get; set; } = Array.Empty<SearchApiOptionViewModel>();
    public IReadOnlyList<SearchEndpointOptionViewModel> EndpointOptions { get; set; } = Array.Empty<SearchEndpointOptionViewModel>();
}
