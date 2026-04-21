using TakedownTCGApplication.Models.UserAccounts;

namespace TakedownTCGApplication.Abstractions;

public interface IFavoriteService
{
    Task<bool> AddFavoriteAsync(string userName, string itemType, string itemId, string itemName);
    Task<IReadOnlyList<Favorite>> GetFavoritesAsync(string userName);
    Task<bool> RemoveFavoriteAsync(string userName, string itemType, string itemId);
}
