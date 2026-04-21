using TakedownTCGApplication.Abstractions;
using TakedownTCGApplication.Models.UserAccounts;

namespace TakedownTCG.Tests.Shared;

public sealed class InMemoryAccountStore
{
    private readonly Dictionary<string, User> _usersByName = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _userNamesByEmail = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Favorite> _favorites = new(StringComparer.Ordinal);
    private readonly object _gate = new();

    public Task<bool> InsertUserAsync(User user)
    {
        lock (_gate)
        {
            if (_usersByName.ContainsKey(user.UserName) || _userNamesByEmail.ContainsKey(user.UserEmail))
            {
                return Task.FromResult(false);
            }

            User copy = Clone(user);
            _usersByName[copy.UserName] = copy;
            _userNamesByEmail[copy.UserEmail] = copy.UserName;

            return Task.FromResult(true);
        }
    }

    public Task<User?> GetByUserNameAsync(string userName)
    {
        lock (_gate)
        {
            return Task.FromResult(
                _usersByName.TryGetValue(userName, out User? user) ? Clone(user) : null);
        }
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        lock (_gate)
        {
            if (!_userNamesByEmail.TryGetValue(email, out string? userName))
            {
                return Task.FromResult<User?>(null);
            }

            return Task.FromResult<User?>(Clone(_usersByName[userName]));
        }
    }

    public Task<User?> GetByUserNameOrEmailAsync(string input)
    {
        lock (_gate)
        {
            if (_usersByName.TryGetValue(input, out User? user))
            {
                return Task.FromResult<User?>(Clone(user));
            }

            if (_userNamesByEmail.TryGetValue(input, out string? userName))
            {
                return Task.FromResult<User?>(Clone(_usersByName[userName]));
            }

            return Task.FromResult<User?>(null);
        }
    }

    public Task<bool> UpdateUserNameAsync(string currentUserName, string newUserName)
    {
        lock (_gate)
        {
            if (!_usersByName.TryGetValue(currentUserName, out User? user) || _usersByName.ContainsKey(newUserName))
            {
                return Task.FromResult(false);
            }

            _usersByName.Remove(user.UserName);
            user.UserName = newUserName;
            _usersByName[user.UserName] = user;

            List<KeyValuePair<string, Favorite>> renamedFavorites = _favorites
                .Where(pair => pair.Value.UserName.Equals(currentUserName, StringComparison.OrdinalIgnoreCase))
                .Select(pair => new KeyValuePair<string, Favorite>(pair.Key, Clone(pair.Value)))
                .ToList();

            foreach ((string oldKey, Favorite favorite) in renamedFavorites)
            {
                _favorites.Remove(oldKey);
                favorite.UserName = newUserName;
                _favorites[BuildFavoriteKey(favorite.UserName, favorite.ItemType, favorite.ItemId)] = favorite;
            }

            return Task.FromResult(true);
        }
    }

    public Task<bool> UpdateEmailAsync(string userName, string newEmail)
    {
        lock (_gate)
        {
            if (!_usersByName.TryGetValue(userName, out User? user))
            {
                return Task.FromResult(false);
            }

            if (_userNamesByEmail.TryGetValue(newEmail, out string? existingUserName) &&
                !existingUserName.Equals(user.UserName, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(false);
            }

            _userNamesByEmail.Remove(user.UserEmail);
            user.UserEmail = newEmail;
            _userNamesByEmail[user.UserEmail] = user.UserName;

            return Task.FromResult(true);
        }
    }

    public Task<bool> UpdatePasswordHashAsync(string userName, string newPasswordHash)
    {
        lock (_gate)
        {
            if (!_usersByName.TryGetValue(userName, out User? user))
            {
                return Task.FromResult(false);
            }

            user.PasswordHash = newPasswordHash;
            return Task.FromResult(true);
        }
    }

    public Task<bool> UpdateNotificationsAsync(string userName, bool enabled)
    {
        lock (_gate)
        {
            if (!_usersByName.TryGetValue(userName, out User? user))
            {
                return Task.FromResult(false);
            }

            user.UserNotifications = enabled;
            return Task.FromResult(true);
        }
    }

    public Task<bool> DeleteUserAsync(string userName)
    {
        lock (_gate)
        {
            if (!_usersByName.TryGetValue(userName, out User? user))
            {
                return Task.FromResult(false);
            }

            _usersByName.Remove(user.UserName);
            _userNamesByEmail.Remove(user.UserEmail);

            string[] favoriteKeys = _favorites
                .Where(pair => pair.Value.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase))
                .Select(pair => pair.Key)
                .ToArray();

            foreach (string favoriteKey in favoriteKeys)
            {
                _favorites.Remove(favoriteKey);
            }

            return Task.FromResult(true);
        }
    }

    public Task<bool> AddFavoriteAsync(Favorite favorite)
    {
        lock (_gate)
        {
            if (!_usersByName.ContainsKey(favorite.UserName))
            {
                return Task.FromResult(false);
            }

            string key = BuildFavoriteKey(favorite.UserName, favorite.ItemType, favorite.ItemId);
            if (_favorites.ContainsKey(key))
            {
                return Task.FromResult(false);
            }

            _favorites[key] = Clone(favorite);
            return Task.FromResult(true);
        }
    }

    public Task<bool> RemoveFavoriteAsync(string userName, string itemType, string itemId)
    {
        lock (_gate)
        {
            return Task.FromResult(_favorites.Remove(BuildFavoriteKey(userName, itemType, itemId)));
        }
    }

    public Task<bool> FavoriteExistsAsync(string userName, string itemType, string itemId)
    {
        lock (_gate)
        {
            return Task.FromResult(_favorites.ContainsKey(BuildFavoriteKey(userName, itemType, itemId)));
        }
    }

    public Task<IReadOnlyList<Favorite>> GetFavoritesByUserAsync(string userName)
    {
        lock (_gate)
        {
            IReadOnlyList<Favorite> results = _favorites.Values
                .Where(favorite => favorite.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(favorite => favorite.CreatedAt)
                .Select(Clone)
                .ToList();

            return Task.FromResult(results);
        }
    }

    private static string BuildFavoriteKey(string userName, string itemType, string itemId)
    {
        return $"{userName.ToUpperInvariant()}::{itemType}::{itemId}";
    }

    private static User Clone(User user)
    {
        return new User
        {
            UserName = user.UserName,
            UserEmail = user.UserEmail,
            PasswordHash = user.PasswordHash,
            UserNotifications = user.UserNotifications
        };
    }

    private static Favorite Clone(Favorite favorite)
    {
        return new Favorite
        {
            UserName = favorite.UserName,
            ItemType = favorite.ItemType,
            ItemId = favorite.ItemId,
            ItemName = favorite.ItemName,
            CreatedAt = favorite.CreatedAt
        };
    }
}

public sealed class InMemoryUserRepository : IUserRepository
{
    private readonly InMemoryAccountStore _store;

    public InMemoryUserRepository(InMemoryAccountStore store)
    {
        _store = store;
    }

    public Task<bool> InsertUserAsync(User user) => _store.InsertUserAsync(user);
    public Task<User?> GetByUserNameAsync(string userName) => _store.GetByUserNameAsync(userName);
    public Task<User?> GetByEmailAsync(string email) => _store.GetByEmailAsync(email);
    public Task<User?> GetByUserNameOrEmailAsync(string input) => _store.GetByUserNameOrEmailAsync(input);
    public Task<bool> UpdateUserNameAsync(string currentUserName, string newUserName) => _store.UpdateUserNameAsync(currentUserName, newUserName);
    public Task<bool> UpdateEmailAsync(string userName, string newEmail) => _store.UpdateEmailAsync(userName, newEmail);
    public Task<bool> UpdatePasswordHashAsync(string userName, string newPasswordHash) => _store.UpdatePasswordHashAsync(userName, newPasswordHash);
    public Task<bool> UpdateNotificationsAsync(string userName, bool enabled) => _store.UpdateNotificationsAsync(userName, enabled);
    public Task<bool> DeleteUserAsync(string userName) => _store.DeleteUserAsync(userName);
}

public sealed class InMemoryFavoriteRepository : IFavoriteRepository
{
    private readonly InMemoryAccountStore _store;

    public InMemoryFavoriteRepository(InMemoryAccountStore store)
    {
        _store = store;
    }

    public Task<bool> AddFavoriteAsync(Favorite favorite) => _store.AddFavoriteAsync(favorite);
    public Task<bool> RemoveFavoriteAsync(string userName, string itemType, string itemId) => _store.RemoveFavoriteAsync(userName, itemType, itemId);
    public Task<bool> FavoriteExistsAsync(string userName, string itemType, string itemId) => _store.FavoriteExistsAsync(userName, itemType, itemId);
    public Task<IReadOnlyList<Favorite>> GetFavoritesByUserAsync(string userName) => _store.GetFavoritesByUserAsync(userName);
}
