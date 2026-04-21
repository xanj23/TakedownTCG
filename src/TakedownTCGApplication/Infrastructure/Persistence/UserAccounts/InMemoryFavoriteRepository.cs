using TakedownTCGApplication.Abstractions;
using TakedownTCGApplication.Models.UserAccounts;

namespace TakedownTCGApplication.Infrastructure.Persistence.UserAccounts;

public sealed class InMemoryFavoriteRepository : IFavoriteRepository
{
    private readonly InMemoryAccountStore _store;

    public InMemoryFavoriteRepository(InMemoryAccountStore store)
    {
        _store = store;
    }

    public Task<bool> AddFavoriteAsync(Favorite favorite) => Task.FromResult(_store.AddFavorite(favorite));
    public Task<bool> RemoveFavoriteAsync(string userName, string itemType, string itemId) => Task.FromResult(_store.RemoveFavorite(userName, itemType, itemId));
    public Task<bool> FavoriteExistsAsync(string userName, string itemType, string itemId) => Task.FromResult(_store.FavoriteExists(userName, itemType, itemId));
    public Task<IReadOnlyList<Favorite>> GetFavoritesByUserAsync(string userName) => Task.FromResult(_store.GetFavoritesByUser(userName));
}
