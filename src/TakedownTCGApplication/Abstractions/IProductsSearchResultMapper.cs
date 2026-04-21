using TakedownTCGApplication.Models.JustTcg.Response;
using TakedownTCGApplication.Models.Search;

namespace TakedownTCGApplication.Abstractions;

public interface IProductsSearchResultMapper
{
    IReadOnlyList<CardSearchResult> MapCards(IEnumerable<Card> cards, IReadOnlySet<string> favoriteCardIds);
    IReadOnlyList<SetSearchResult> MapSets(IEnumerable<Set> sets);
    IReadOnlyList<GameSearchResult> MapGames(IEnumerable<Game> games);
}
