using System.Collections.Generic;
using TakedownTCG.cli.Menu;

namespace TakedownTCG.cli.Api.JustTCG
{
    /// <summary>
    /// Defines the JustTCG endpoint-selection menu.
    /// </summary>
    public static class JustTCGMenu
    {
        /// <summary>
        /// Gets the menu definition used by the JustTCG client.
        /// </summary>
        public static MenuDefinition<JustTCGClient.Action> Definition { get; }

        static JustTCGMenu()
        {
            Definition = new MenuDefinition<JustTCGClient.Action>(
                "JustTCG Endpoints",
                new List<MenuOption<JustTCGClient.Action>>
                {
                    new MenuOption<JustTCGClient.Action>("Cards", JustTCGClient.Action.Cards),
                    new MenuOption<JustTCGClient.Action>("Sets", JustTCGClient.Action.Sets),
                    new MenuOption<JustTCGClient.Action>("Games", JustTCGClient.Action.Games),
                    new MenuOption<JustTCGClient.Action>("Back", JustTCGClient.Action.Back),
                    new MenuOption<JustTCGClient.Action>("Quit", JustTCGClient.Action.Quit)
                },
                JustTCGClient.Action.Back,
                JustTCGClient.Action.Quit);
        }
    }
}
