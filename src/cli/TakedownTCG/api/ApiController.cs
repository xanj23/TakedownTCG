using TakedownTCG.cli.Menu;

namespace TakedownTCG.cli.Api
{
    /// <summary>
    /// Coordinates the API-selection submenu flow.
    /// </summary>
    public static class ApiController
    {
        /// <summary>
        /// Runs the API submenu loop.
        /// </summary>
        public static void Run()
        {
            while (true)
            {
                ApiMenu.Action selectedAction = MenuRunner.Select(ApiMenu.Definition);

                if (ApiMenu.Definition.BackAction.HasValue && selectedAction == ApiMenu.Definition.BackAction.Value)
                {
                    return;
                }

                RunAction(selectedAction);
            }
        }

        /// <summary>
        /// Executes the selected API-menu action.
        /// </summary>
        /// <param name="selectedAction">The selected API action.</param>
        private static void RunAction(ApiMenu.Action selectedAction)
        {
            if (selectedAction == ApiMenu.Definition.QuitAction)
            {
                Environment.Exit(0);
            }

            switch (selectedAction)
            {
                case ApiMenu.Action.JustTCG:
                    IApiClient client = ApiRegistry.Resolve(selectedAction);
                    client.Run();
                    break;
                default:
                    Console.WriteLine("Unknown API menu option was selected.");
                    break;
            }
        }
    }
}
