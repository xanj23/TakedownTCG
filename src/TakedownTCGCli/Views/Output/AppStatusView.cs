namespace TakedownTCG.cli.Views.Output
{
    /// <summary>
    /// Renders shared application status messages to the console.
    /// </summary>
    public static class AppStatusView
    {
        public static void ShowUserAccountNotImplemented()
        {
            Console.WriteLine("User Account is not implemented yet.");
        }

        public static void ShowUnknownMainMenuOption()
        {
            Console.WriteLine("Unknown menu option was selected.");
        }

        public static void ShowUnknownApiMenuOption()
        {
            Console.WriteLine("Unknown API menu option was selected.");
        }
    }
}
