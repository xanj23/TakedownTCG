using TakedownTCGApplication.Models.UserAccounts;

namespace TakedownTCGApplication.Abstractions;

public interface IFavoriteRepository
{
    Task<bool> AddFavoriteAsync(Favorite favorite);
    Task<bool> RemoveFavoriteAsync(string userName, string itemType, string itemId);
    Task<bool> FavoriteExistsAsync(string userName, string itemType, string itemId);
    Task<IReadOnlyList<Favorite>> GetFavoritesByUserAsync(string userName);
}
