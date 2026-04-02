using TakedownTCG.cli.Util;
using TakedownTCG.cli.Config;
using TakedownTCG.cli.Api;

namespace TakedownTCG.cli
{
    public static class Controller
    {
        /// <summary>
        /// Runs the main CLI loop and dispatches the selected menu action.
        /// </summary>
        public static void RunCli() {
            while (true)
            {
                int selectedIndex = UserInput.GetIndex($"Takedown TCG" + '\n' + MainMenu.Name, MainMenu.Options);
                int selectedAction = MainMenu.Actions[selectedIndex];
                if ( selectedAction == MainMenu.Action.Quit)
                {
                    Environment.Exit(0);
                }
                else
                {
                    RunAction(selectedAction);
                }
            }
        }

        private void RunAction(int selectedAction)
        {
            switch (selectedAction)
            {
                case MainMenu.Action.SearchAPi:
                    ApiController.Run();
                    break;
                case MainMenu.Action.SearchAPi:
                    UserAccountController.Run();
                    break;
                default:
                    Console.WriteLine("Unknown menu option was selected.");
                    break;
            }
        }
    }
}
