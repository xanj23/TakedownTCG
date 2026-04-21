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
        private static UserAccountController? _userAccountController;

        public static void Configure(Func<IApiClient> justTcgFactory, UserAccountController userAccountController)
        {
            _justTcgFactory = justTcgFactory;
            _userAccountController = userAccountController;
        }

        public static void Run()
        {
            Initialize();
            new AppController(_userAccountController!).RunCli();
        }

        private static void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            if (_justTcgFactory is null || _userAccountController is null)
            {
                throw new InvalidOperationException("AppCompositionRoot.Configure must be called before Run.");
            }

            ApiRegistry.Register(ApiMenu.Action.JustTCG, _justTcgFactory);
            _initialized = true;
        }
    }
}
