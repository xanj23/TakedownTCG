using System.Collections.Generic;

namespace TakedownTCG.cli.Menu
{
    /// <summary>
    /// Defines the top-level CLI menu.
    /// </summary>
    public static class MainMenu
    {
        /// <summary>
        /// Gets the menu definition used by the main controller.
        /// </summary>
        public static MenuDefinition<Action> Definition { get; }

        static MainMenu()
        {
            Definition = new MenuDefinition<Action>(
                "TakedownTCG",
                new List<MenuOption<Action>>
                {
                    new MenuOption<Action>("Search APIs", Action.SearchAPIs),
                    new MenuOption<Action>("User Account", Action.UserAccount),
                    new MenuOption<Action>("Quit", Action.Quit)
                },
                null,
                Action.Quit);
        }

        /// <summary>
        /// Represents the actions available from the main menu.
        /// </summary>
        public enum Action
        {
            SearchAPIs = 0,
            UserAccount = 1,
            Quit = 2
        }
    }
}
