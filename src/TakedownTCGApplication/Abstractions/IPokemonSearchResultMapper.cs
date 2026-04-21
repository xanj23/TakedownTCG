using TakedownTCGApplication.Models.PokemonTcg.Response;
using TakedownTCGApplication.Models.Search;

namespace TakedownTCGApplication.Abstractions;

public interface IPokemonSearchResultMapper
{
    IReadOnlyList<CardSearchResult> MapCards(IEnumerable<PokemonCard> cards, IReadOnlySet<string> favoriteCardIds);
}
