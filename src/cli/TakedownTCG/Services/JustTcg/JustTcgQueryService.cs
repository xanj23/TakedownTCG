using System;
using System.Collections.Generic;
using TakedownTCG.cli.Models.JustTcg.Query;
using TakedownTCG.cli.Views.Input;

namespace TakedownTCG.cli.Services.JustTcg
{
    /// <summary>
    /// Handles JustTCG endpoint query input and URL building.
    /// </summary>
    public sealed class JustTcgQueryService
    {
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

            JustTcgInputView.ShowInputSearchParametersHeader();

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

        public string BuildUrl(JustTCGClient.Action endpoint, IQueryParams query, string baseUrl)
        {
            string path = endpoint switch
            {
                JustTCGClient.Action.Cards => "/cards",
                JustTCGClient.Action.Sets => "/sets",
                JustTCGClient.Action.Games => "/games",
                _ => throw new NotSupportedException($"Unsupported endpoint: {endpoint}")
            };

            string queryString = BuildQuery(query);
            return $"{baseUrl}{path}{queryString}";
        }

        private static string BuildQuery(IQueryParams query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            Dictionary<string, string> rawQuery = BuildRawQuery(query);
            return FormatQueryString(rawQuery);
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
    }
}
