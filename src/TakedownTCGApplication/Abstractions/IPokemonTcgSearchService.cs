using TakedownTCGApplication.Models.PokemonTcg.Query;

namespace TakedownTCGApplication.Abstractions;

public interface IPokemonTcgSearchService
{
    Task<object> SearchCardsAsync(PokemonCardQueryParams query, CancellationToken cancellationToken = default);
}
