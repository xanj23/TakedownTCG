using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using TakedownTCG.cli.Api.JustTCG.Query;
using TakedownTCG.cli.Api.JustTCG.Response;
using TakedownTCG.cli.Util;

namespace TakedownTCG.cli.Api.JustTCG
{
    /// <summary>
    /// Encapsulates the JustTCG command workflow beneath the client orchestration layer.
    /// This helper object exists so the client can stay focused on high-level flow
    /// while request-building, fetching, deserialization, mapping, and display logic
    /// remain grouped in one place with access to client-specific configuration.
    /// </summary>
    public sealed class JustTCGCommands
    {
        private readonly JustTCGClient _client;
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="JustTCGCommands"/> class.
        /// </summary>
        /// <param name="client">The owning JustTCG client.</param>
        public JustTCGCommands(JustTCGClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        /// <summary>
        /// Builds and populates the query object for the selected endpoint.
        /// </summary>
        /// <param name="endpoint">The selected JustTCG endpoint.</param>
        /// <returns>A populated query object for the endpoint.</returns>
        public IQueryParams InputQuery(JustTCGClient.Action endpoint)
        {
            IQueryParams query;

            switch (endpoint)
            {
                case JustTCGClient.Action.Cards:
                    query = new CardQueryParams();
                    break;
                case JustTCGClient.Action.Sets:
                    query = new SetQueryParams();
                    break;
                case JustTCGClient.Action.Games:
                    query = new GameQueryParams();
                    break;
                default:
                    throw new NotSupportedException($"Unsupported endpoint: {endpoint}");
            }

            Console.WriteLine('\n' + "Input search parameters:");

            foreach (KeyValuePair<string, QueryParameter> kvp in query.Parameters)
            {
                QueryParameter param = kvp.Value;
                if (param.IsRequired)
                {
                    param.Value = UserInput.InputRequiredString(param.Label);
                    continue;
                }

                param.Value = UserInput.InputString($"{param.Label} (optional)");
            }

            return query;
        }

        /// <summary>
        /// Builds the query string for the supplied query parameters.
        /// </summary>
        /// <param name="query">The query parameters to format.</param>
        /// <returns>The formatted query string.</returns>
        public string BuildQuery(IQueryParams query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            Dictionary<string, string> rawQuery = BuildRawQuery(query);
            return FormatQueryString(rawQuery);
        }

        /// <summary>
        /// Builds the full request URL for the selected endpoint.
        /// </summary>
        /// <param name="endpoint">The selected endpoint.</param>
        /// <param name="query">The query parameters for the request.</param>
        /// <returns>The full request URL.</returns>
        public string BuildUrl(JustTCGClient.Action endpoint, IQueryParams query)
        {
            string path = endpoint switch
            {
                JustTCGClient.Action.Cards => "/cards",
                JustTCGClient.Action.Sets => "/sets",
                JustTCGClient.Action.Games => "/games",
                _ => throw new NotSupportedException($"Unsupported endpoint: {endpoint}")
            };

            string queryString = BuildQuery(query);
            return $"{_client.BaseUrl}{path}{queryString}";
        }

        /// <summary>
        /// Builds the HTTP request for the selected endpoint and query.
        /// </summary>
        /// <param name="endpoint">The selected endpoint.</param>
        /// <param name="query">The query parameters for the request.</param>
        /// <returns>A configured HTTP request message.</returns>
        public HttpRequestMessage BuildRequest(JustTCGClient.Action endpoint, IQueryParams query)
        {
            string url = BuildUrl(endpoint, query);
            HttpRequestMessage request = CreateRequest(url);
            SetRequestHeaders(request);
            return request;
        }

        /// <summary>
        /// Executes the HTTP request and returns the raw response content.
        /// </summary>
        /// <param name="request">The request to send.</param>
        /// <returns>The raw response payload.</returns>
        public string FetchResponse(HttpRequestMessage request)
        {
            using HttpResponseMessage response = JustTCGClient.HttpClient.Send(request);
            response.EnsureSuccessStatusCode();
            return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Deserializes the raw response content into the appropriate endpoint response type.
        /// </summary>
        /// <param name="endpoint">The selected endpoint.</param>
        /// <param name="responseContent">The raw response payload.</param>
        /// <returns>The typed deserialized response object.</returns>
        public object Deserialize(JustTCGClient.Action endpoint, string responseContent)
        {
            return endpoint switch
            {
                JustTCGClient.Action.Cards => DeserializeResponse<Card>(responseContent),
                JustTCGClient.Action.Sets => DeserializeResponse<Set>(responseContent),
                JustTCGClient.Action.Games => DeserializeResponse<Game>(responseContent),
                _ => throw new NotSupportedException($"Unsupported endpoint: {endpoint}")
            };
        }

        /// <summary>
        /// Maps a deserialized JustTCG response into display text.
        /// </summary>
        /// <param name="responseData">The typed response object.</param>
        /// <returns>A formatted display string.</returns>
        public string Map(object responseData)
        {
            return responseData switch
            {
                Response<Card> cardResponse => MapCards(cardResponse),
                Response<Set> setResponse => MapSets(setResponse),
                Response<Game> gameResponse => MapGames(gameResponse),
                _ => throw new NotSupportedException("Unsupported response type.")
            };
        }

        /// <summary>
        /// Writes the mapped response text to the console.
        /// </summary>
        /// <param name="mappedData">The formatted response text.</param>
        public void Display(string mappedData)
        {
            Console.WriteLine();
            Console.WriteLine(mappedData);
        }

        private static Dictionary<string, string> BuildRawQuery(IQueryParams query)
        {
            if (query.Parameters.Count == 0)
            {
                return new Dictionary<string, string>();
            }

            var rawQuery = new Dictionary<string, string>();
            foreach (KeyValuePair<string, QueryParameter> kvp in query.Parameters)
            {
                string key = kvp.Key;
                QueryParameter param = kvp.Value;
                object? value = param.Value;

                if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                {
                    continue;
                }

                rawQuery.Add(key, value.ToString() ?? string.Empty);
            }

            return rawQuery;
        }

        private static string FormatQueryString(Dictionary<string, string> rawQuery)
        {
            if (rawQuery.Count == 0)
            {
                return string.Empty;
            }

            var parameters = new List<string>();
            foreach (KeyValuePair<string, string> pair in rawQuery)
            {
                parameters.Add(pair.Key + '=' + pair.Value);
            }

            return '?' + string.Join('&', parameters);
        }

        private static HttpRequestMessage CreateRequest(string url)
        {
            return new HttpRequestMessage(HttpMethod.Get, url);
        }

        private static Response<T> DeserializeResponse<T>(string responseContent)
        {
            Response<T>? response = JsonSerializer.Deserialize<Response<T>>(responseContent, JsonOptions);

            if (response == null)
            {
                throw new InvalidOperationException("Failed to deserialize JustTCG response.");
            }

            return response;
        }

        private static string MapCards(Response<Card> response)
        {
            var builder = new StringBuilder();
            AppendResponseHeader(builder, response.Data.Count, response.Meta.HasMore, response.Error, response.Code);

            for (int i = 0; i < response.Data.Count; i++)
            {
                Card card = response.Data[i];
                builder.AppendLine($"{i + 1}. {card.Name}");
                builder.AppendLine($"   Id: {card.Id}");
                builder.AppendLine($"   Game: {card.Game}");
                builder.AppendLine($"   Set: {card.SetName} ({card.Set})");
                builder.AppendLine($"   Number: {card.Number}");
                builder.AppendLine($"   Rarity: {card.Rarity}");

                if (!string.IsNullOrWhiteSpace(card.Details))
                {
                    builder.AppendLine($"   Details: {card.Details}");
                }

                builder.AppendLine($"   Variants: {card.Variants.Count}");
                builder.AppendLine();
            }

            AppendNoResults(builder, response.Data.Count);
            return builder.ToString().TrimEnd();
        }

        private static string MapSets(Response<Set> response)
        {
            var builder = new StringBuilder();
            AppendResponseHeader(builder, response.Data.Count, response.Meta.HasMore, response.Error, response.Code);

            for (int i = 0; i < response.Data.Count; i++)
            {
                Set set = response.Data[i];
                builder.AppendLine($"{i + 1}. {set.Name}");
                builder.AppendLine($"   Id: {set.Id}");
                builder.AppendLine($"   Game: {set.Game}");
                builder.AppendLine($"   Release Date: {set.ReleaseDate}");
                builder.AppendLine($"   Cards: {set.CardsCount}");
                builder.AppendLine($"   Variants: {set.VariantsCount}");
                builder.AppendLine($"   Sealed: {set.SealedCount}");
                builder.AppendLine($"   Set Value USD: {set.SetValueUsd}");
                builder.AppendLine();
            }

            AppendNoResults(builder, response.Data.Count);
            return builder.ToString().TrimEnd();
        }

        private static string MapGames(Response<Game> response)
        {
            var builder = new StringBuilder();
            AppendResponseHeader(builder, response.Data.Count, response.Meta.HasMore, response.Error, response.Code);

            for (int i = 0; i < response.Data.Count; i++)
            {
                Game game = response.Data[i];
                builder.AppendLine($"{i + 1}. {game.Name}");
                builder.AppendLine($"   Id: {game.Id}");
                builder.AppendLine($"   Sets: {game.SetsCount}");
                builder.AppendLine($"   Cards: {game.CardsCount}");
                builder.AppendLine($"   Variants: {game.VariantsCount}");
                builder.AppendLine($"   Sealed: {game.SealedCount}");
                builder.AppendLine($"   Game Value Index Cents: {game.GameValueIndexCents}");
                builder.AppendLine($"   Last Updated: {game.LastUpdated}");
                builder.AppendLine();
            }

            AppendNoResults(builder, response.Data.Count);
            return builder.ToString().TrimEnd();
        }

        private static void AppendResponseHeader(StringBuilder builder, int count, bool hasMore, string? error, string? code)
        {
            if (!string.IsNullOrWhiteSpace(error))
            {
                builder.AppendLine($"Error: {error}");

                if (!string.IsNullOrWhiteSpace(code))
                {
                    builder.AppendLine($"Code: {code}");
                }

                builder.AppendLine();
            }

            builder.AppendLine($"Results: {count}");
            builder.AppendLine($"Has More: {hasMore}");
            builder.AppendLine();
        }

        private static void AppendNoResults(StringBuilder builder, int count)
        {
            if (count == 0)
            {
                builder.AppendLine("No results found.");
            }
        }

        private void SetRequestHeaders(HttpRequestMessage request)
        {
            if (!string.IsNullOrWhiteSpace(_client.ApiKey))
            {
                request.Headers.Add(JustTCGClient.ApiKeyHeaderName, _client.ApiKey);
            }
        }
    }
}
