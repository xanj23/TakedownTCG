using System;
using TakedownTCG.cli.Models.JustTcg.Response;
using TakedownTCG.cli.Models.UserAccounts;
using TakedownTCG.cli.Services.UserAccounts;
using TakedownTCG.cli.Views.Input;
using TakedownTCG.cli.Views.Menus;
using TakedownTCG.cli.Views.Shared;

namespace TakedownTCG.cli.Controllers
{
    public sealed class UserAccountController
    {
        private readonly AccountService _accountService;
        private readonly FavoriteService _favoriteService;
        private readonly FavoriteController _favoriteController;
        private User? _currentUser;

        public UserAccountController(AccountService accountService, FavoriteService favoriteService)
        {
            _accountService = accountService;
            _favoriteService = favoriteService;
            _favoriteController = new FavoriteController(favoriteService, () => _currentUser);
        }

        public User? CurrentUser => _currentUser;

        public void OfferToFavorite(object responseData)
        {
            try
            {
                Console.WriteLine();
                string? favInput = UserInput.InputString("Add a result to favorites? Enter result number or press Enter to skip");
                if (string.IsNullOrWhiteSpace(favInput))
                {
                    return;
                }

                if (!int.TryParse(favInput.Trim(), out int favIndex))
                {
                    Console.WriteLine("Invalid number.");
                    return;
                }

                if (_currentUser is null)
                {
                    Console.WriteLine("Login to add favorites.");
                    return;
                }

                if (responseData is Response<Card> cardResp)
                {
                    AddCardFavorite(favIndex, cardResp);
                    return;
                }

                if (responseData is Response<Set> setResp)
                {
                    AddSetFavorite(favIndex, setResp);
                    return;
                }

                if (responseData is Response<Game> gameResp)
                {
                    AddGameFavorite(favIndex, gameResp);
                    return;
                }

                Console.WriteLine("Favoriting is not supported for this response type.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to add favorite: {ex.Message}");
            }
        }

        public void Run()
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

        private void AddCardFavorite(int favIndex, Response<Card> cardResp)
        {
            if (!IsIndexInRange(favIndex, cardResp.Data.Count))
            {
                Console.WriteLine("Index out of range.");
                return;
            }

            Card card = cardResp.Data[favIndex - 1];
            bool added = _favoriteService.AddFavorite(_currentUser!.UserName, "Card", card.Id, card.Name);

            Console.WriteLine(added ? "Added to favorites." : "Already favorited or failed.");
        }

        private void AddSetFavorite(int favIndex, Response<Set> setResp)
        {
            if (!IsIndexInRange(favIndex, setResp.Data.Count))
            {
                Console.WriteLine("Index out of range.");
                return;
            }

            Set set = setResp.Data[favIndex - 1];
            bool added = _favoriteService.AddFavorite(_currentUser!.UserName, "Set", set.Id, set.Name);

            Console.WriteLine(added ? "Added to favorites." : "Already favorited or failed.");
        }

        private void AddGameFavorite(int favIndex, Response<Game> gameResp)
        {
            if (!IsIndexInRange(favIndex, gameResp.Data.Count))
            {
                Console.WriteLine("Index out of range.");
                return;
            }

            Game game = gameResp.Data[favIndex - 1];
            bool added = _favoriteService.AddFavorite(_currentUser!.UserName, "Game", game.Id, game.Name);

            Console.WriteLine(added ? "Added to favorites." : "Already favorited or failed.");
        }

        private static bool IsIndexInRange(int index, int count)
        {
            return index >= 1 && index <= count;
        }

        private void RunAction(UserAccountMenu.Action selectedAction)
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
                    _favoriteController.ShowFavoritesMenu();
                    break;
                case UserAccountMenu.Action.DeleteAccount:
                    DeleteAccount();
                    break;
                default:
                    Console.WriteLine("Unknown user account menu option was selected.");
                    break;
            }
        }

        private void Register()
        {
            string userName = UserInput.InputRequiredString("Enter username");
            string email = UserInput.InputRequiredString("Enter email");
            string password = UserInput.InputRequiredString("Enter password");

            bool success = _accountService.CreateAccount(userName, email, password, true);
            Console.WriteLine(success ? "Account created successfully." : "Failed to create account. Username or email may already exist.");
        }

        private void Login()
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

        private void Logout()
        {
            if (_currentUser is null)
            {
                Console.WriteLine("No user is logged in.");
                return;
            }

            Console.WriteLine($"User {_currentUser.UserName} logged out.");
            _currentUser = null;
        }

        private void ViewProfile()
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

        private void ChangeUserName()
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

        private void ChangeEmail()
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

        private void ChangePassword()
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

        private void ToggleNotifications()
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

        private void DeleteAccount()
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
