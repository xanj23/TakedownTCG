using System;
using TakedownTCG.cli.Models.UserAccounts;
using TakedownTCG.cli.Services.UserAccounts;
using TakedownTCG.cli.Views.Input;
using TakedownTCG.cli.Views.Menus;
using TakedownTCG.cli.Views.Shared;

namespace TakedownTCG.cli.Controllers
{
    public static class UserAccountController
    {
        private static AccountService? _accountService;
        private static FavoriteService? _favoriteService;
        private static User? _currentUser;

        public static User? CurrentUser => _currentUser;
        public static FavoriteService FavoriteService =>
            _favoriteService ?? throw new InvalidOperationException("Favorite service is not initialized.");

        public static void Configure(AccountService accountService, FavoriteService favoriteService)
        {
            _accountService = accountService;
            _favoriteService = favoriteService;
            _currentUser = null;
        }

        public static void Run()
        {
            EnsureInitialized();

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("User Account Menu");
                Console.WriteLine(_currentUser is null ? "Not logged in" : $"Logged in as {_currentUser.UserName} ({_currentUser.UserEmail})");

                UserAccountMenu.Action selectedAction = MenuRunner.Select(UserAccountMenu.Definition);

                if (UserAccountMenu.Definition.BackAction.HasValue && selectedAction == UserAccountMenu.Definition.BackAction.Value)
                {
                    return;
                }

                RunAction(selectedAction);
            }
        }

        private static AccountService AccountService =>
            _accountService ?? throw new InvalidOperationException("Account service is not initialized.");

        private static void EnsureInitialized()
        {
            if (_accountService is null || _favoriteService is null)
            {
                throw new InvalidOperationException("UserAccountController.Configure must be called before use.");
            }
        }

        private static void RunAction(UserAccountMenu.Action selectedAction)
        {
            if (selectedAction == UserAccountMenu.Definition.QuitAction)
            {
                Environment.Exit(0);
            }

            switch (selectedAction)
            {
                case UserAccountMenu.Action.Register:
                    Register();
                    break;
                case UserAccountMenu.Action.Login:
                    Login();
                    break;
                case UserAccountMenu.Action.Logout:
                    Logout();
                    break;
                case UserAccountMenu.Action.ViewProfile:
                    ViewProfile();
                    break;
                case UserAccountMenu.Action.ChangeUserName:
                    ChangeUserName();
                    break;
                case UserAccountMenu.Action.ChangeEmail:
                    ChangeEmail();
                    break;
                case UserAccountMenu.Action.ChangePassword:
                    ChangePassword();
                    break;
                case UserAccountMenu.Action.ToggleNotifications:
                    ToggleNotifications();
                    break;
                case UserAccountMenu.Action.Favorites:
                    FavoriteController.ShowFavoritesMenu();
                    break;
                case UserAccountMenu.Action.DeleteAccount:
                    DeleteAccount();
                    break;
                default:
                    Console.WriteLine("Unknown user account menu option was selected.");
                    break;
            }
        }

        private static void Register()
        {
            string userName = UserInput.InputRequiredString("Enter username");
            string email = UserInput.InputRequiredString("Enter email");
            string password = UserInput.InputRequiredString("Enter password");

            bool success = AccountService.CreateAccount(userName, email, password, true);
            Console.WriteLine(success ? "Account created successfully." : "Failed to create account. Username or email may already exist.");
        }

        private static void Login()
        {
            if (_currentUser != null)
            {
                Console.WriteLine("Already logged in. Logout before switching user.");
                return;
            }

            string userNameOrEmail = UserInput.InputRequiredString("Enter username or email");
            string password = UserInput.InputRequiredString("Enter password");

            User? user = AccountService.Login(userNameOrEmail, password);
            if (user is null)
            {
                Console.WriteLine("Login failed. Please check credentials.");
                return;
            }

            _currentUser = user;
            Console.WriteLine($"Logged in as {_currentUser.UserName}.");
        }

        private static void Logout()
        {
            if (_currentUser is null)
            {
                Console.WriteLine("No user is logged in.");
                return;
            }

            Console.WriteLine($"User {_currentUser.UserName} logged out.");
            _currentUser = null;
        }

        private static void ViewProfile()
        {
            if (_currentUser is null)
            {
                Console.WriteLine("Login to view profile.");
                return;
            }

            Console.WriteLine($"User Name: {_currentUser.UserName}");
            Console.WriteLine($"Email: {_currentUser.UserEmail}");
            Console.WriteLine($"Notifications: {(_currentUser.UserNotifications ? "Enabled" : "Disabled")}");
        }

        private static void ChangeUserName()
        {
            if (_currentUser is null)
            {
                Console.WriteLine("Login to change username.");
                return;
            }

            string newUserName = UserInput.InputRequiredString("New username");
            bool success = AccountService.ChangeUserName(_currentUser.UserName, newUserName);
            if (success)
            {
                _currentUser.UserName = newUserName;
                Console.WriteLine("Username updated successfully.");
            }
            else
            {
                Console.WriteLine("Failed to update username. New name may already be taken.");
            }
        }

        private static void ChangeEmail()
        {
            if (_currentUser is null)
            {
                Console.WriteLine("Login to change email.");
                return;
            }

            string newEmail = UserInput.InputRequiredString("New email");
            bool success = AccountService.ChangeEmail(_currentUser.UserName, newEmail);
            if (success)
            {
                _currentUser.UserEmail = newEmail;
                Console.WriteLine("Email updated successfully.");
            }
            else
            {
                Console.WriteLine("Failed to update email. New email may already be taken.");
            }
        }

        private static void ChangePassword()
        {
            if (_currentUser is null)
            {
                Console.WriteLine("Login to change password.");
                return;
            }

            string currentPassword = UserInput.InputRequiredString("Current password");
            string newPassword = UserInput.InputRequiredString("New password");

            bool success = AccountService.ChangePassword(_currentUser.UserName, currentPassword, newPassword);
            Console.WriteLine(success ? "Password changed successfully." : "Failed to change password. Current password may be incorrect.");
        }

        private static void ToggleNotifications()
        {
            if (_currentUser is null)
            {
                Console.WriteLine("Login to update notification preferences.");
                return;
            }

            bool currentSetting = _currentUser.UserNotifications;
            bool newSetting = !currentSetting;
            bool success = AccountService.UpdateUserNotifications(_currentUser.UserName, newSetting);
            if (success)
            {
                _currentUser.UserNotifications = newSetting;
                Console.WriteLine(newSetting ? "Notifications enabled." : "Notifications disabled.");
            }
            else
            {
                Console.WriteLine("Failed to update notification settings.");
            }
        }

        private static void DeleteAccount()
        {
            if (_currentUser is null)
            {
                Console.WriteLine("Login to delete account.");
                return;
            }

            string confirmation = UserInput.InputRequiredString("Type DELETE to confirm account deletion");
            if (!confirmation.Equals("DELETE", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Account deletion cancelled.");
                return;
            }

            bool success = AccountService.DeleteAccount(_currentUser.UserName);
            if (success)
            {
                Console.WriteLine("Account deleted successfully.");
                _currentUser = null;
            }
            else
            {
                Console.WriteLine("Failed to delete account.");
            }
        }
    }
}
