using System;

namespace TakedownTCGApplication.Models.UserAccounts
{
    public sealed class Favorite
    {
        private string _userName = string.Empty;
        private string _itemType = string.Empty;
        private string _itemId = string.Empty;
        private string _itemName = string.Empty;

        public string UserName
        {
            get => _userName;
            init => _userName = Require(value, nameof(UserName));
        }

        public string ItemType
        {
            get => _itemType;
            init => _itemType = Require(value, nameof(ItemType));
        }

        public string ItemId
        {
            get => _itemId;
            init => _itemId = Require(value, nameof(ItemId));
        }

        public string ItemName
        {
            get => _itemName;
            init => _itemName = value?.Trim() ?? string.Empty;
        }

        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        private static string Require(string? value, string propertyName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{propertyName} is required.", propertyName);
            }

            return value.Trim();
        }
    }
}
