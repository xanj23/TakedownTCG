using TCGAPP;

namespace JustTCG {
    public class SetClientHeader
    {
        public static void Run(IApi api, HttpClient client)
        {
        Console.WriteLine($"Setting Client Header | Api:{api.Name} | KeyHead:x-api-key, | KeyValue:{api.ApiKey}");
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("x-api-key", api.ApiKey);
        }
    }
}