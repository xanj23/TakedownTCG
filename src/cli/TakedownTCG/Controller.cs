using TakedownTCG.cli.Util;
using TakedownTCG.Config;

namespace TakedownTCG.cli
{
    public static class Controller
    {
        public static void RunCli() {
            while (true)
            {
                int selectedIndex = UserInput.GetIndex($"Takedown TCG" + '\n' + MainMenu.Name, MainMenu.Options);
                Console.WriteLine(selectedIndex + '\n');
                Console.ReadLine();
                if (MainMenu.Actions[selectedIndex] == MainMenu.Action.Quit)
                {
                    Environment.Exit(0);
                }
            }
        }
    }
}
