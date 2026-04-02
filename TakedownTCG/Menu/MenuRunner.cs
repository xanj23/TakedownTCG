using System.Collections.Generic;
using TakedownTCG.cli.Util;

namespace TakedownTCG.cli.Menu
{
    /// <summary>
    /// Provides shared menu-selection behavior for CLI menus.
    /// </summary>
    public static class MenuRunner
    {
        /// <summary>
        /// Prompts the user to select an option from the supplied menu definition.
        /// </summary>
        /// <typeparam name="TAction">The enum type represented by the menu.</typeparam>
        /// <param name="menu">The menu definition to display.</param>
        /// <returns>The action associated with the selected option.</returns>
        public static TAction Select<TAction>(MenuDefinition<TAction> menu)
            where TAction : struct, Enum
        {
            int selectedIndex = UserInput.GetIndex(menu.Name, menu.Options);
            return menu.Options[selectedIndex].Action;
        }
    }
}
