using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TakedownTCG.cli.Api.JustTCG.Query;
using TakedownTCG.cli.Util;

namespace TakedownTCG.cli.Api.JustTCG
{
    public class JustTCGClient : IApiClient
    {
        private const string ApiKeyHeaderName = "x-api-key";
        private static readonly HttpClient HttpClient = new HttpClient();

        public string Name => "JustTCG";
        public string BaseUrl => "https://api.justtcg.com/v1";
        public string ApiKey => Environment.GetEnvironmentVariable("JUSTTCG_API_KEY") ?? string.Empty;

        public static string MenuName { get; } = "JustTCG Endpoints";
        public static List<string> Options { get; } = new List<string> { "Cards", "Sets", "Games", "Back", "Quit" };
        public static List<Action> Actions { get; } = new List<Action>
        {
            Action.Cards,
            Action.Sets,
            Action.Games,
            Action.Back,
            Action.Quit
        };

        public enum Action
        {
            Cards = 0,
            Sets = 1,
            Games = 2,
            Back = 3,
            Quit = 4
        }

        public void Run()
        {
            while (true)
            {
                Action selected = EndpointController.SelectEndpoint();
                if (selected == Action.Back)
                {
                    return;
                }

                if (selected == Action.Quit)
                {
                    Environment.Exit(0);
                }

                IQueryParams query = InputQuery(selected);
                HttpRequestMessage request = BuildRequest(selected, query);
                Console.WriteLine($"Built url: {request.RequestUri}");
            }
        }

        public static IQueryParams InputQuery(Action endpoint)
        {
            IQueryParams query = endpoint switch
            {
                Action.Cards => new CardQueryParams(),
                Action.Sets => new SetQueryParams(),
                Action.Games => new GameQueryParams(),
                _ => throw new NotSupportedException($"Unsupported endpoint: {endpoint}")
            };

            foreach (KeyValuePair<string, QueryParam<object>> kvp in query.Parameters)
            {
                QueryParam<object> param = kvp.Value;
                if (param.IsRequired)
                {
                    string value = UserInput.InputRequiredString(param.Label);
                    param.Value = value;
                }
                else
                {
                    Console.Write($"{param.Label} (optional): ");
                    string value = (Console.ReadLine() ?? string.Empty).Trim();
                    param.Value = value.Length == 0 ? null : value;
                }
            }

            return query;
        }

        public static string BuildQuery(IQueryParams query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var parts = new List<string>();
            foreach (KeyValuePair<string, QueryParam<object>> kvp in query.Parameters)
            {
                string key = kvp.Key;
                QueryParam<object> param = kvp.Value;
                object? value = param.Value;

                if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                {
                    if (param.IsRequired)
                    {
                        throw new InvalidOperationException($"Missing required parameter: {param.Label}");
                    }
                    continue;
                }

                string encodedValue = Uri.EscapeDataString(value.ToString() ?? string.Empty);
                parts.Add($"{key}={encodedValue}");
            }

            if (parts.Count == 0)
            {
                return string.Empty;
            }

            return "?" + string.Join("&", parts);
        }

        public string BuildUrl(Action endpoint, IQueryParams query)
        {
            string path = endpoint switch
            {
                Action.Cards => "/cards",
                Action.Sets => "/sets",
                Action.Games => "/games",
                _ => throw new NotSupportedException($"Unsupported endpoint: {endpoint}")
            };

            string queryString = BuildQuery(query);
            return $"{BaseUrl}{path}{queryString}";
        }

        public HttpRequestMessage BuildRequest(Action endpoint, IQueryParams query)
        {
            string url = BuildUrl(endpoint, query);
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            if (!string.IsNullOrWhiteSpace(ApiKey))
            {
                request.Headers.Add(ApiKeyHeaderName, ApiKey);
            }

            return request;
        }

        public async Task<string> FetchAsync(HttpRequestMessage request)
        {
            using HttpResponseMessage response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
