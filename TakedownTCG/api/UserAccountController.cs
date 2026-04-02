using System;
using System.IO;
using TakedownTCG.cli.Menu;
using TakedownTCG.cli.Util;
using TakedownTCG.UserAccounts.Models;
using TakedownTCG.UserAccounts.Repositories;
using TakedownTCG.UserAccounts.Services;

namespace TakedownTCG.cli.Api
{
    public static class UserAccountController
    {
        private static readonly AccountService _accountService;
        private static User? _currentUser;

        static UserAccountController()
        {
            string dbPath = Path.Combine(AppContext.BaseDirectory, "takedowntcg-users.db");
            _accountService = new AccountService(new UserRepository(dbPath));
            _currentUser = null;
        }

        public static void Run()
        {
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

            bool success = _accountService.CreateAccount(userName, email, password, true);
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

            User? user = _accountService.Login(userNameOrEmail, password);
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
            bool success = _accountService.ChangeUserName(_currentUser.UserName, newUserName);
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
            bool success = _accountService.ChangeEmail(_currentUser.UserName, newEmail);
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

            bool success = _accountService.ChangePassword(_currentUser.UserName, currentPassword, newPassword);
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
            bool success = _accountService.UpdateUserNotifications(_currentUser.UserName, newSetting);
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

            bool success = _accountService.DeleteAccount(_currentUser.UserName);
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
