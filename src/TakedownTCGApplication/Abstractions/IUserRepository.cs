using TakedownTCGApplication.Models.UserAccounts;

namespace TakedownTCGApplication.Abstractions;

public interface IUserRepository
{
    Task<bool> InsertUserAsync(User user);
    Task<User?> GetByUserNameAsync(string userName);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUserNameOrEmailAsync(string input);
    Task<bool> UpdateUserNameAsync(string currentUserName, string newUserName);
    Task<bool> UpdateEmailAsync(string userName, string newEmail);
    Task<bool> UpdatePasswordHashAsync(string userName, string newPasswordHash);
    Task<bool> UpdateNotificationsAsync(string userName, bool enabled);
    Task<bool> DeleteUserAsync(string userName);
}
