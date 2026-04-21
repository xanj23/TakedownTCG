using TakedownTCGApplication.Models.UserAccounts;

namespace TakedownTCGApplication.Infrastructure.Persistence.UserAccounts;

public sealed class InMemoryAccountStore
{
    private readonly Dictionary<string, User> _usersByName = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _userNamesByEmail = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Favorite> _favorites = new(StringComparer.OrdinalIgnoreCase);
    private readonly object _gate = new();

    public bool InsertUser(User user)
    {
        lock (_gate)
        {
            if (_usersByName.ContainsKey(user.UserName) || _userNamesByEmail.ContainsKey(user.UserEmail))
            {
                return false;
            }

            User copy = Clone(user);
            _usersByName[copy.UserName] = copy;
            _userNamesByEmail[copy.UserEmail] = copy.UserName;
            return true;
        }
    }

    public User? GetByUserName(string userName)
    {
        lock (_gate)
        {
            return _usersByName.TryGetValue(userName, out User? user) ? Clone(user) : null;
        }
    }

    public User? GetByEmail(string email)
    {
        lock (_gate)
        {
            return _userNamesByEmail.TryGetValue(email, out string? userName)
                ? Clone(_usersByName[userName])
                : null;
        }
    }

    public User? GetByUserNameOrEmail(string input)
    {
        lock (_gate)
        {
            if (_usersByName.TryGetValue(input, out User? user))
            {
                return Clone(user);
            }

            return _userNamesByEmail.TryGetValue(input, out string? userName)
                ? Clone(_usersByName[userName])
                : null;
        }
    }

    public bool UpdateUserName(string currentUserName, string newUserName)
    {
        lock (_gate)
        {
            if (!_usersByName.TryGetValue(currentUserName, out User? user) || _usersByName.ContainsKey(newUserName))
            {
                return false;
            }

            User renamedUser = new()
            {
                UserName = newUserName,
                UserEmail = user.UserEmail,
                PasswordHash = user.PasswordHash,
                UserNotifications = user.UserNotifications
            };

            _usersByName.Remove(user.UserName);
            _usersByName[renamedUser.UserName] = renamedUser;
            _userNamesByEmail[renamedUser.UserEmail] = renamedUser.UserName;

            List<KeyValuePair<string, Favorite>> renamedFavorites = _favorites
                .Where(pair => pair.Value.UserName.Equals(currentUserName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach ((string oldKey, Favorite favorite) in renamedFavorites)
            {
                _favorites.Remove(oldKey);
                Favorite renamedFavorite = new()
                {
                    UserName = newUserName,
                    ItemType = favorite.ItemType,
                    ItemId = favorite.ItemId,
                    ItemName = favorite.ItemName,
                    CreatedAt = favorite.CreatedAt
                };
                _favorites[BuildFavoriteKey(renamedFavorite.UserName, renamedFavorite.ItemType, renamedFavorite.ItemId)] = renamedFavorite;
            }

            return true;
        }
    }

    public bool UpdateEmail(string userName, string newEmail)
    {
        lock (_gate)
        {
            if (!_usersByName.TryGetValue(userName, out User? user))
            {
                return false;
            }

            if (_userNamesByEmail.TryGetValue(newEmail, out string? existingUserName) &&
                !existingUserName.Equals(user.UserName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            User updatedUser = new()
            {
                UserName = user.UserName,
                UserEmail = newEmail,
                PasswordHash = user.PasswordHash,
                UserNotifications = user.UserNotifications
            };

            _userNamesByEmail.Remove(user.UserEmail);
            _usersByName[user.UserName] = updatedUser;
            _userNamesByEmail[updatedUser.UserEmail] = updatedUser.UserName;
            return true;
        }
    }

    public bool UpdatePasswordHash(string userName, string newPasswordHash)
    {
        lock (_gate)
        {
            if (!_usersByName.TryGetValue(userName, out User? user))
            {
                return false;
            }

            _usersByName[user.UserName] = new User
            {
                UserName = user.UserName,
                UserEmail = user.UserEmail,
                PasswordHash = newPasswordHash,
                UserNotifications = user.UserNotifications
            };
            return true;
        }
    }

    public bool UpdateNotifications(string userName, bool enabled)
    {
        lock (_gate)
        {
            if (!_usersByName.TryGetValue(userName, out User? user))
            {
                return false;
            }

            _usersByName[user.UserName] = new User
            {
                UserName = user.UserName,
                UserEmail = user.UserEmail,
                PasswordHash = user.PasswordHash,
                UserNotifications = enabled
            };
            return true;
        }
    }

    public bool DeleteUser(string userName)
    {
        lock (_gate)
        {
            if (!_usersByName.TryGetValue(userName, out User? user))
            {
                return false;
            }

            _usersByName.Remove(user.UserName);
            _userNamesByEmail.Remove(user.UserEmail);

            foreach (string favoriteKey in _favorites
                         .Where(pair => pair.Value.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase))
                         .Select(pair => pair.Key)
                         .ToArray())
            {
                _favorites.Remove(favoriteKey);
            }

            return true;
        }
    }

    public bool AddFavorite(Favorite favorite)
    {
        lock (_gate)
        {
            if (!_usersByName.ContainsKey(favorite.UserName))
            {
                return false;
            }

            string key = BuildFavoriteKey(favorite.UserName, favorite.ItemType, favorite.ItemId);
            if (_favorites.ContainsKey(key))
            {
                return false;
            }

            _favorites[key] = Clone(favorite);
            return true;
        }
    }

    public bool RemoveFavorite(string userName, string itemType, string itemId)
    {
        lock (_gate)
        {
            return _favorites.Remove(BuildFavoriteKey(userName, itemType, itemId));
        }
    }

    public bool FavoriteExists(string userName, string itemType, string itemId)
    {
        lock (_gate)
        {
            return _favorites.ContainsKey(BuildFavoriteKey(userName, itemType, itemId));
        }
    }

    public IReadOnlyList<Favorite> GetFavoritesByUser(string userName)
    {
        lock (_gate)
        {
            return _favorites.Values
                .Where(favorite => favorite.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(favorite => favorite.CreatedAt)
                .Select(Clone)
                .ToList();
        }
    }

    private static string BuildFavoriteKey(string userName, string itemType, string itemId)
    {
        return $"{userName}::{itemType}::{itemId}";
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
