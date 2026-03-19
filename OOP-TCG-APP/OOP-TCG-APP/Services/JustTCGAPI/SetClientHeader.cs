using TCGAPP;

namespace JustTCG {
    public class SetClientHeader
    {
        public static void Run(IApi api, HttpClient client)
        {
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("x-api-key", api.ApiKey);
        }
    }
}