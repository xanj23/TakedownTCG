using System.Collections.Generic;

namespace TakedownTCG.cli.Menu
{
    /// <summary>
    /// Defines the user-account submenu.
    /// </summary>
    public static class UserAccountMenu
    {
        public static MenuDefinition<Action> Definition { get; }

        static UserAccountMenu()
        {
            Definition = new MenuDefinition<Action>(
                "User Account",
                new List<MenuOption<Action>>
                {
                    new MenuOption<Action>("Register", Action.Register),
                    new MenuOption<Action>("Login", Action.Login),
                    new MenuOption<Action>("Logout", Action.Logout),
                    new MenuOption<Action>("View Profile", Action.ViewProfile),
                    new MenuOption<Action>("Change Username", Action.ChangeUserName),
                    new MenuOption<Action>("Change Email", Action.ChangeEmail),
                    new MenuOption<Action>("Change Password", Action.ChangePassword),
                    new MenuOption<Action>("Toggle Notifications", Action.ToggleNotifications),
                    new MenuOption<Action>("Delete Account", Action.DeleteAccount),
                    new MenuOption<Action>("Back", Action.Back),
                    new MenuOption<Action>("Quit", Action.Quit)
                },
                Action.Back,
                Action.Quit);
        }

        public enum Action
        {
            Register = 0,
            Login = 1,
            Logout = 2,
            ViewProfile = 3,
            ChangeUserName = 4,
            ChangeEmail = 5,
            ChangePassword = 6,
            ToggleNotifications = 7,
            DeleteAccount = 8,
            Back = 9,
            Quit = 10
        }
    }
}
