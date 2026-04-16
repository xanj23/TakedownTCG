using TakedownTCG.cli.Controllers;
using TakedownTCG.cli.Services.Api;
using TakedownTCG.cli.Services.JustTcg;
using TakedownTCG.cli.Views.Menus;

namespace TakedownTCG.cli.Composition
{
    /// <summary>
    /// Composes application dependencies and starts the CLI workflow.
    /// </summary>
    public static class AppCompositionRoot
    {
        private static bool _initialized;

        public static void Run()
        {
            Initialize();
            AppController.RunCli();
        }

        private static void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            ApiRegistry.Register<JustTCGClient>(ApiMenu.Action.JustTCG);
            _initialized = true;
        }
    }
}
