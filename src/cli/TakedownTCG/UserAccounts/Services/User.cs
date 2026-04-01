namespace TakedownTCG.UserAccounts.Models
{
    public class User
    {
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool UserNotifications { get; set; }
    }
}