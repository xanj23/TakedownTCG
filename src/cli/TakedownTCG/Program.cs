using TakedownTCG.cli.Composition;
using TakedownTCG.cli.Controllers;

namespace TakedownTCG.cli
{
    public static class Program
    {
        public static void Main()
        {
            // Initialize user account related DB objects (favorites table, user table)
            var _ = UserAccountController.CurrentUser;

            AppCompositionRoot.Run();
        }
    }
}
