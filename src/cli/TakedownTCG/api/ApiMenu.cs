using System.Collections.Generic;
using TakedownTCG.cli.Menu;

namespace TakedownTCG.cli.Api
{
    /// <summary>
    /// Defines the API-selection submenu.
    /// </summary>
    public static class ApiMenu
    {
        /// <summary>
        /// Gets the menu definition used by the API controller.
        /// </summary>
        public static MenuDefinition<Action> Definition { get; }

        static ApiMenu()
        {
            Definition = new MenuDefinition<Action>(
                "Api Menu",
                new List<MenuOption<Action>>
                {
                    new MenuOption<Action>("JustTCG", Action.JustTCG),
                    new MenuOption<Action>("Back", Action.Back),
                    new MenuOption<Action>("Quit", Action.Quit)
                },
                Action.Back,
                Action.Quit);
        }

        /// <summary>
        /// Represents the actions available from the API menu.
        /// </summary>
        public enum Action
        {
            JustTCG = 0,
            Back = 1,
            Quit = 2
        }
    }
}
