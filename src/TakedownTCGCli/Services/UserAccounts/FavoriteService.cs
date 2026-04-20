using System;
using System.Collections.Generic;
using TakedownTCG.cli.Infrastructure.Persistence.UserAccounts;
using TakedownTCG.cli.Models.UserAccounts;

namespace TakedownTCG.cli.Services.UserAccounts
{
    public class FavoriteService
    {
        private readonly FavoriteRepository _favoriteRepository;
        private readonly UserRepository _userRepository;

        public FavoriteService(FavoriteRepository favoriteRepository, UserRepository userRepository)
        {
            _favoriteRepository = favoriteRepository;
            _userRepository = userRepository;
        }

        public bool AddFavorite(string userName, string itemType, string itemId, string itemName)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(itemType) || string.IsNullOrWhiteSpace(itemId))
            {
                return false;
            }

            var user = _userRepository.GetByUserName(userName);
            if (user is null)
            {
                return false;
            }

            if (_favoriteRepository.FavoriteExists(userName, itemType, itemId))
            {
                return false;
            }

            var fav = new Favorite
            {
                UserName = userName,
                ItemType = itemType,
                ItemId = itemId,
                ItemName = itemName ?? string.Empty,
                CreatedAt = DateTime.UtcNow
            };

            return _favoriteRepository.AddFavorite(fav);
        }

        public List<Favorite> GetFavorites(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return new List<Favorite>();
            }

            return _favoriteRepository.GetFavoritesByUser(userName);
        }

        public bool RemoveFavorite(string userName, string itemType, string itemId)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(itemType) || string.IsNullOrWhiteSpace(itemId))
            {
                return false;
            }

            return _favoriteRepository.RemoveFavorite(userName, itemType, itemId);
        }
    }
}
