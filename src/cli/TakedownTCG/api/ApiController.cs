using TakedownTCG.cli.Util;

namespace TakedownTCG.cli.Api
{
    public static class ApiController
    {
        public static void Run()
        {
            while (true)
            {
                int selectedIndex = UserInput.GetIndex(Api.Name, Api.Options);
                Api.Action selectedAction = Api.Actions[selectedIndex];

                if (selectedAction == Api.Action.Back)
                {
                    return;
                }

                if (selectedAction == Api.Action.Quit)
                {
                    Environment.Exit(0);
                }

                if (selectedAction == Api.Action.JustTCG)
                {
                    IApiClient client = ApiRegistry.Resolve(selectedAction);
                    client.Run();
                }
            }
        }
    }
}
