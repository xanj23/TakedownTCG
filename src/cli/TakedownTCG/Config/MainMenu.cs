using System.Collections.Generic;

namespace TakedownTCG.Config
{
    public static class MainMenu
    {
        public static string Name { get; }
        public static List<string> Options { get; }
        public static List<Action> Actions { get; }

        static MainMenu()
        {
            Name = "Main Menu";
            Actions = new List<Action>
            {
                Action.SearchAPIs,
                Action.UserAccount,
                Action.Quit
            };

            Options = new List<string>
            {
                "Search APIs",
                "User Account",
                "Quit"
            };
        }

        public enum Action
        {
            SearchAPIs = 0,
            UserAccount = 1,
            Quit = 2
        }
    }
}
