using TakedownTCG.cli.Api;
using TakedownTCG.cli.Menu;

namespace TakedownTCG.cli
{
    /// <summary>
    /// Coordinates the top-level CLI menu flow.
    /// </summary>
    public static class Controller
    {
        /// <summary>
        /// Runs the main CLI loop and dispatches the selected menu action.
        /// </summary>
        public static void RunCli()
        {
            while (true)
            {
                MainMenu.Action selectedAction = MenuRunner.Select(MainMenu.Definition);

                if (selectedAction == MainMenu.Definition.QuitAction)
                {
                    Environment.Exit(0);
                }

                RunAction(selectedAction);
            }
        }

        /// <summary>
        /// Executes the selected main-menu action.
        /// </summary>
        /// <param name="selectedAction">The selected main-menu action.</param>
        private static void RunAction(MainMenu.Action selectedAction)
        {
            switch (selectedAction)
            {
                case MainMenu.Action.SearchAPIs:
                    ApiController.Run();
                    break;
                case MainMenu.Action.UserAccount:
                    Console.WriteLine("User Account is not implemented yet.");
                    break;
                default:
                    Console.WriteLine("Unknown menu option was selected.");
                    break;
            }
        }
    }
}
