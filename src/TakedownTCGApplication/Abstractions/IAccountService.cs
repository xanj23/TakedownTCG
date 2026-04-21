using TakedownTCG.Core.Models.UserAccounts;

namespace TakedownTCG.Core.Abstractions;

public interface IAccountService
{
    Task<bool> CreateAccountAsync(string userName, string email, string password, bool userNotifications = true);
    Task<User?> LoginAsync(string userNameOrEmail, string password);
    Task<User?> GetByUserNameAsync(string userName);
    Task<bool> ChangeUserNameAsync(string currentUserName, string newUserName);
    Task<bool> ChangeEmailAsync(string userName, string newEmail);
    Task<bool> ChangePasswordAsync(string userName, string currentPassword, string newPassword);
    Task<bool> UpdateUserNotificationsAsync(string userName, bool enabled);
    Task<bool> DeleteAccountAsync(string userName);
}
