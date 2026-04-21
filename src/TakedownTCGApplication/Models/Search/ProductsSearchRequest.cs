namespace TakedownTCGApplication.Models.Search;

public sealed class ProductsSearchRequest
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
}
