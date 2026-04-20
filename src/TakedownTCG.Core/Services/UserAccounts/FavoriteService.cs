using TakedownTCG.Core.Abstractions;
using TakedownTCG.Core.Models.UserAccounts;

namespace TakedownTCG.Core.Services.UserAccounts;

public sealed class FavoriteService : IFavoriteService
{
    private readonly IFavoriteRepository _favoriteRepository;
    private readonly IUserRepository _userRepository;

    public FavoriteService(IFavoriteRepository favoriteRepository, IUserRepository userRepository)
    {
        _favoriteRepository = favoriteRepository;
        _userRepository = userRepository;
    }

    public async Task<bool> AddFavoriteAsync(string userName, string itemType, string itemId, string itemName)
    {
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(itemType) || string.IsNullOrWhiteSpace(itemId))
        {
            return false;
        }

        User? user = await _userRepository.GetByUserNameAsync(userName.Trim());
        if (user is null)
        {
            return false;
        }

        if (await _favoriteRepository.FavoriteExistsAsync(userName.Trim(), itemType.Trim(), itemId.Trim()))
        {
            return false;
        }

        Favorite favorite = new()
        {
            UserName = userName.Trim(),
            ItemType = itemType.Trim(),
            ItemId = itemId.Trim(),
            ItemName = itemName?.Trim() ?? string.Empty,
            CreatedAt = DateTime.UtcNow
        };

        return await _favoriteRepository.AddFavoriteAsync(favorite);
    }

    public async Task<IReadOnlyList<Favorite>> GetFavoritesAsync(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            return Array.Empty<Favorite>();
        }

        return await _favoriteRepository.GetFavoritesByUserAsync(userName.Trim());
    }

    public async Task<bool> RemoveFavoriteAsync(string userName, string itemType, string itemId)
    {
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(itemType) || string.IsNullOrWhiteSpace(itemId))
        {
            return false;
        }

        return await _favoriteRepository.RemoveFavoriteAsync(userName.Trim(), itemType.Trim(), itemId.Trim());
    }
}
