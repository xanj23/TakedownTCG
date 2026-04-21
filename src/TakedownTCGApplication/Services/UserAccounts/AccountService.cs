using TakedownTCG.Core.Abstractions;
using TakedownTCG.Core.Models.UserAccounts;

namespace TakedownTCG.Core.Services.UserAccounts;

public sealed class AccountService : IAccountService
{
    private readonly IUserRepository _userRepository;

    public AccountService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> CreateAccountAsync(string userName, string email, string password, bool userNotifications = true)
    {
        if (string.IsNullOrWhiteSpace(userName) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            return false;
        }

        if (await _userRepository.GetByUserNameAsync(userName.Trim()) is not null)
        {
            return false;
        }

        if (await _userRepository.GetByEmailAsync(email.Trim()) is not null)
        {
            return false;
        }

        User user = new()
        {
            UserName = userName.Trim(),
            UserEmail = email.Trim(),
            PasswordHash = PasswordHasher.HashPassword(password),
            UserNotifications = userNotifications
        };

        return await _userRepository.InsertUserAsync(user);
    }

    public async Task<User?> LoginAsync(string userNameOrEmail, string password)
    {
        if (string.IsNullOrWhiteSpace(userNameOrEmail) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        User? user = await _userRepository.GetByUserNameOrEmailAsync(userNameOrEmail.Trim());
        if (user is null)
        {
            return null;
        }

        return PasswordHasher.VerifyPassword(password, user.PasswordHash) ? user : null;
    }

    public Task<User?> GetByUserNameAsync(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            return Task.FromResult<User?>(null);
        }

        return _userRepository.GetByUserNameAsync(userName.Trim());
    }

    public async Task<bool> ChangeUserNameAsync(string currentUserName, string newUserName)
    {
        if (string.IsNullOrWhiteSpace(currentUserName) || string.IsNullOrWhiteSpace(newUserName))
        {
            return false;
        }

        if (await _userRepository.GetByUserNameAsync(newUserName.Trim()) is not null)
        {
            return false;
        }

        return await _userRepository.UpdateUserNameAsync(currentUserName.Trim(), newUserName.Trim());
    }

    public async Task<bool> ChangeEmailAsync(string userName, string newEmail)
    {
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(newEmail))
        {
            return false;
        }

        if (await _userRepository.GetByEmailAsync(newEmail.Trim()) is not null)
        {
            return false;
        }

        return await _userRepository.UpdateEmailAsync(userName.Trim(), newEmail.Trim());
    }

    public async Task<bool> ChangePasswordAsync(string userName, string currentPassword, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(userName) ||
            string.IsNullOrWhiteSpace(currentPassword) ||
            string.IsNullOrWhiteSpace(newPassword))
        {
            return false;
        }

        User? user = await _userRepository.GetByUserNameAsync(userName.Trim());
        if (user is null)
        {
            return false;
        }

        if (!PasswordHasher.VerifyPassword(currentPassword, user.PasswordHash))
        {
            return false;
        }

        string newPasswordHash = PasswordHasher.HashPassword(newPassword);
        return await _userRepository.UpdatePasswordHashAsync(userName.Trim(), newPasswordHash);
    }

    public async Task<bool> UpdateUserNotificationsAsync(string userName, bool enabled)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            return false;
        }

        return await _userRepository.UpdateNotificationsAsync(userName.Trim(), enabled);
    }

    public async Task<bool> DeleteAccountAsync(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            return false;
        }

        return await _userRepository.DeleteUserAsync(userName.Trim());
    }
}
