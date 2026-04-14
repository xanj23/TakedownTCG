namespace TakedownTCG.cli
{
    public static class Program
    {
        public static void Main()
        {
            // Initialize user account related DB objects (favorites table, user table)
            var _ = TakedownTCG.cli.Api.UserAccountController.CurrentUser;

            Controller.RunCli();
        }
    }
}
