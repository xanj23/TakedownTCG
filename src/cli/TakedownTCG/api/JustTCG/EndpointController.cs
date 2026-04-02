using TakedownTCG.cli.Util;

namespace TakedownTCG.cli.Api.JustTCG
{
    public static class EndpointController
    {
        public static JustTCGClient.Action SelectEndpoint()
        {
            int selectedIndex = UserInput.GetIndex(JustTCGClient.MenuName, JustTCGClient.Options);
            return JustTCGClient.Actions[selectedIndex];
        }
    }
}
