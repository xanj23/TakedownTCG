using System.Net.Http;

namespace TakedownTCG.cli.Infrastructure.Http
{
    /// <summary>
    /// Handles HTTP request creation and execution for JustTCG.
    /// </summary>
    public sealed class JustTcgHttpGateway
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        public string FetchResponse(string url, string apiKeyHeaderName, string apiKey)
        {
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                request.Headers.Add(apiKeyHeaderName, apiKey);
            }

            using HttpResponseMessage response = HttpClient.Send(request);
            response.EnsureSuccessStatusCode();
            return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        }
    }
}
