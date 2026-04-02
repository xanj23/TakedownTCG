using System.Collections.Generic;

namespace TakedownTCG.cli.Api
{
    public static class Api
    {
        public static string Name { get; }
        public static List<string> Apis { get; }
        public static List<string> Options { get; }
        public static List<Action> Actions { get; }

        static Api()
        {
            Name = "Api Menu";

            Apis = new List<string>
            {
                "JustTCG"
            };

            Actions = new List<Action>
            {
                Action.JustTCG,
                Action.Back,
                Action.Quit
            };

            Options = new List<string>
            {
                "JustTCG",
                "Back",
                "Quit"
            };
        }

        public enum Action
        {
            JustTCG = 0,
            Back = 1,
            Quit = 2
        }
    }
}
