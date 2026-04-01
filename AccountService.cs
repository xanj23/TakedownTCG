using TakedownTCG.UserAccounts.Models;
using TakedownTCG.UserAccounts.Repositories;

namespace TakedownTCG.UserAccounts.Services
{
    public class AccountService
    {
        private readonly UserRepository _userRepository;

        public AccountService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public bool CreateAccount(string userName, string email, string password, bool userNotifications = true)
        {
            if (string.IsNullOrWhiteSpace(userName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            if (_userRepository.GetByUserName(userName) is not null)
            {
                return false;
            }

            if (_userRepository.GetByEmail(email) is not null)
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

            return _userRepository.InsertUser(user);
        }

        public User? Login(string userNameOrEmail, string password)
        {
            if (string.IsNullOrWhiteSpace(userNameOrEmail) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            User? user = _userRepository.GetByUserNameOrEmail(userNameOrEmail.Trim());
            if (user is null)
            {
                return null;
            }

            return PasswordHasher.VerifyPassword(password, user.PasswordHash) ? user : null;
        }

        public bool ChangeUserName(string currentUserName, string newUserName)
        {
            if (string.IsNullOrWhiteSpace(currentUserName) || string.IsNullOrWhiteSpace(newUserName))
            {
                return false;
            }

            if (_userRepository.GetByUserName(newUserName.Trim()) is not null)
            {
                return false;
            }

            return _userRepository.UpdateUserName(currentUserName.Trim(), newUserName.Trim());
        }

        public bool ChangeEmail(string userName, string newEmail)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(newEmail))
            {
                return false;
            }

            if (_userRepository.GetByEmail(newEmail.Trim()) is not null)
            {
                return false;
            }

            return _userRepository.UpdateEmail(userName.Trim(), newEmail.Trim());
        }

        public bool ChangePassword(string userName, string currentPassword, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(userName) ||
                string.IsNullOrWhiteSpace(currentPassword) ||
                string.IsNullOrWhiteSpace(newPassword))
            {
                return false;
            }

            User? user = _userRepository.GetByUserName(userName.Trim());
            if (user is null)
            {
                return false;
            }

            if (!PasswordHasher.VerifyPassword(currentPassword, user.PasswordHash))
            {
                return false;
            }

            string newPasswordHash = PasswordHasher.HashPassword(newPassword);
            return _userRepository.UpdatePasswordHash(userName.Trim(), newPasswordHash);
        }

        public bool UpdateUserNotifications(string userName, bool enabled)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return false;
            }

            return _userRepository.UpdateNotifications(userName.Trim(), enabled);
        }

        public bool DeleteAccount(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return false;
            }

            return _userRepository.DeleteUser(userName.Trim());
        }
    }
}