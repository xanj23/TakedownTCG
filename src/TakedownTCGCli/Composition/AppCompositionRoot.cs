using TakedownTCG.cli.Controllers;
using TakedownTCG.cli.Services.Api;
using TakedownTCG.cli.Views.Menus;

namespace TakedownTCG.cli.Composition
{
    /// <summary>
    /// Composes application dependencies and starts the CLI workflow.
    /// </summary>
    public static class AppCompositionRoot
    {
        private static bool _initialized;
        private static Func<IApiClient>? _justTcgFactory;

        public static void Configure(Func<IApiClient> justTcgFactory)
        {
            _justTcgFactory = justTcgFactory;
        }

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

            if (_justTcgFactory is null)
            {
                throw new InvalidOperationException("AppCompositionRoot.Configure must be called before Run.");
            }

            ApiRegistry.Register(ApiMenu.Action.JustTCG, _justTcgFactory);
            _initialized = true;
        }
    }
}
