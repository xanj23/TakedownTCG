using System;

namespace TakedownTCG.cli.Models.UserAccounts
{
    public class Favorite
    {
        public string UserName { get; set; } = string.Empty;
        public string ItemType { get; set; } = string.Empty;
        public string ItemId { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
