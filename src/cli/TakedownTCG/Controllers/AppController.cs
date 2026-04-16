using TakedownTCG.cli.Views.Menus;
using TakedownTCG.cli.Views.Output;
using TakedownTCG.cli.Views.Shared;

namespace TakedownTCG.cli.Controllers
{
    /// <summary>
    /// Coordinates the top-level CLI menu flow.
    /// </summary>
    public static class AppController
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
                    UserAccountController.Run();
                    break;
                default:
                    AppStatusView.ShowUnknownMainMenuOption();
                    break;
            }
        }
    }
}
