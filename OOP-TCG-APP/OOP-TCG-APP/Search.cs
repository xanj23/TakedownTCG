using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace JustTcgClient
{
    /// <summary>
    /// Represents the available search filters for a card lookup against the JustTCG API.
    /// </summary>
    /// <param name="Q">The free-text card search term.</param>
    /// <param name="Game">The game slug to filter by.</param>
    /// <param name="Set">The set identifier to filter by.</param>
    /// <param name="Limit">The maximum number of results to return.</param>
    /// <param name="Offset">The zero-based offset used for pagination.</param>
    /// <param name="OrderBy">The API field used for sorting.</param>
    /// <param name="Order">The sort direction.</param>
    /// <param name="IncludePriceHistory">Indicates whether price history data should be requested.</param>
    /// <param name="PriceHistoryDuration">The duration window for price history data.</param>
    /// <param name="IncludeStatistics">The statistics ranges that should be included in the response.</param>
    /// <param name="IncludeNullPrices">Indicates whether variants without prices should be included.</param>
    /// <param name="Printing">The printing type to filter by.</param>
    /// <param name="Condition">The card condition filter.</param>
    public sealed record CardSearchQuery(
        string? Q = null,
        string? Game = null,
        string? Set = null,
        int Limit = 20,
        int Offset = 0,
        string? OrderBy = null,
        string? Order = null,
        bool IncludePriceHistory = false,
        string? PriceHistoryDuration = null,
        string? IncludeStatistics = null,
        bool IncludeNullPrices = false,
        string? Printing = null,
        string? Condition = null
    );

    /// <summary>
    /// Represents the simplified card information returned to the caller after a search.
    /// </summary>
    /// <param name="Id">The JustTCG card identifier.</param>
    /// <param name="Name">The card name.</param>
    /// <param name="Game">The game the card belongs to.</param>
    /// <param name="SetId">The JustTCG set identifier.</param>
    /// <param name="SetName">The display name of the set.</param>
    /// <param name="Number">The card number within the set.</param>
    /// <param name="Rarity">The reported card rarity.</param>
    /// <param name="TcgplayerId">The linked TCGplayer product identifier.</param>
    /// <param name="LowestVariantPrice">The lowest available variant price found in the response.</param>
    /// <param name="ImageUrl">The derived TCGplayer image URL when a product identifier is available.</param>
    public sealed record CardSearchResult(
        string Id,
        string Name,
        string Game,
        string SetId,
        string SetName,
        string? Number,
        string? Rarity,
        string? TcgplayerId,
        decimal? LowestVariantPrice,
        string? ImageUrl
    );

    /// <summary>
    /// Represents pagination metadata returned from a JustTCG search response.
    /// </summary>
    /// <param name="Total">The total number of matching items.</param>
    /// <param name="Limit">The page size used by the response.</param>
    /// <param name="Offset">The zero-based starting position of the current page.</param>
    /// <param name="HasMore">Indicates whether another page of results is available.</param>
    public sealed record PageMeta(int Total, int Limit, int Offset, bool HasMore);

    /// <summary>
    /// Represents a page of mapped search results and its related metadata.
    /// </summary>
    /// <typeparam name="T">The item type returned in the page.</typeparam>
    /// <param name="Items">The items contained in the current page.</param>
    /// <param name="Meta">The pagination metadata returned by the API.</param>
    public sealed record PagedResult<T>(IReadOnlyList<T> Items, PageMeta? Meta);

    /// <summary>
    /// Represents an API error returned from the JustTCG service.
    /// </summary>
    public sealed class JustTcgApiException : Exception
    {
        /// <summary>
        /// Gets the HTTP status code returned by the API.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Gets the API-specific error code when one is returned.
        /// </summary>
        public string? ApiCode { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JustTcgApiException"/> class.
        /// </summary>
        /// <param name="statusCode">The HTTP status code returned by the API.</param>
        /// <param name="message">The human-readable error message.</param>
        /// <param name="apiCode">The API-specific error code, if available.</param>
        public JustTcgApiException(HttpStatusCode statusCode, string message, string? apiCode = null)
            : base(message)
        {
            StatusCode = statusCode;
            ApiCode = apiCode;
        }
    }

    /// <summary>
    /// Provides a typed client for issuing card search requests to the JustTCG API.
    /// </summary>
    public sealed class JustTcgCardsClient
    {
        private const string ApiKeyEnvironmentVariable = "JUSTTCG_API_KEY";
        private readonly HttpClient _http;
        private readonly Uri _baseUri = new("https://api.justtcg.com/v1/");

        /// <summary>
        /// Initializes a new instance of the <see cref="JustTcgCardsClient"/> class.
        /// </summary>
        /// <param name="http">The HTTP client used to communicate with the API.</param>
        public JustTcgCardsClient(HttpClient http)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
        }

        /// <summary>
        /// Searches the JustTCG cards endpoint by using the supplied filters.
        /// </summary>
        /// <param name="query">The search filters to apply to the request.</param>
        /// <param name="ct">A token that can cancel the request.</param>
        /// <returns>A page of mapped card search results.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the API key environment variable is not set.</exception>
        /// <exception cref="JustTcgApiException">Thrown when the API returns an unsuccessful response.</exception>
        public async Task<PagedResult<CardSearchResult>> SearchCardsAsync(CardSearchQuery query, CancellationToken ct = default)
        {
            query = Normalize(query);

            using var req = new HttpRequestMessage(HttpMethod.Get, BuildCardsUri(query));
            req.Headers.TryAddWithoutValidation("x-api-key", GetApiKey());

            using var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
            await using var stream = await resp.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);

            if (!resp.IsSuccessStatusCode)
            {
                ApiErrorEnvelope? err = null;
                try
                {
                    err = await JsonSerializer.DeserializeAsync<ApiErrorEnvelope>(stream, JsonOpts, ct).ConfigureAwait(false);
                }
                catch (JsonException)
                {
                }

                string message = err?.Error ?? $"HTTP {(int)resp.StatusCode} {resp.ReasonPhrase}";
                throw new JustTcgApiException(resp.StatusCode, message, err?.Code);
            }

            ApiEnvelope<List<ApiCard>>? api =
                await JsonSerializer.DeserializeAsync<ApiEnvelope<List<ApiCard>>>(stream, JsonOpts, ct).ConfigureAwait(false);
            List<ApiCard> cards = api?.Data ?? new List<ApiCard>();
            List<CardSearchResult> items = cards.Select(MapToResult).ToList();

            PageMeta? meta = null;
            if (api?.Meta is not null)
            {
                meta = new PageMeta(api.Meta.Total, api.Meta.Limit, api.Meta.Offset, api.Meta.HasMore);
            }

            return new PagedResult<CardSearchResult>(items, meta);
        }

        /// <summary>
        /// Builds the complete cards endpoint URI from the supplied query values.
        /// </summary>
        /// <param name="query">The normalized search query.</param>
        /// <returns>The fully qualified cards endpoint URI.</returns>
        private Uri BuildCardsUri(CardSearchQuery query)
        {
            Dictionary<string, string?> queryParams = new()
            {
                ["q"] = query.Q,
                ["game"] = query.Game,
                ["set"] = query.Set,
                ["limit"] = query.Limit.ToString(),
                ["offset"] = query.Offset.ToString(),
                ["orderBy"] = query.OrderBy,
                ["order"] = query.Order,
                ["include_price_history"] = query.IncludePriceHistory ? "true" : "false",
                ["priceHistoryDuration"] = query.PriceHistoryDuration,
                ["include_statistics"] = query.IncludeStatistics,
                ["include_null_prices"] = query.IncludeNullPrices ? "true" : "false",
                ["printing"] = query.Printing,
                ["condition"] = query.Condition
            };

            string queryString = string.Join(
                "&",
                queryParams
                    .Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
                    .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value!)}"));

            UriBuilder builder = new(new Uri(_baseUri, "cards"))
            {
                Query = queryString
            };

            return builder.Uri;
        }

        /// <summary>
        /// Normalizes user-supplied search values before a request is sent.
        /// </summary>
        /// <param name="query">The raw query values supplied by the caller.</param>
        /// <returns>A normalized query with bounded pagination values.</returns>
        private static CardSearchQuery Normalize(CardSearchQuery query)
        {
            static string? Clean(string? value)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }

                string[] parts = value.Trim().Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                return string.Join(' ', parts);
            }

            int limit = query.Limit;
            if (limit < 1)
            {
                limit = 20;
            }

            if (limit > 200)
            {
                limit = 200;
            }

            int offset = query.Offset < 0 ? 0 : query.Offset;

            return query with
            {
                Q = Clean(query.Q),
                Game = Clean(query.Game),
                Set = Clean(query.Set),
                OrderBy = Clean(query.OrderBy),
                Order = Clean(query.Order),
                PriceHistoryDuration = Clean(query.PriceHistoryDuration),
                IncludeStatistics = Clean(query.IncludeStatistics),
                Printing = Clean(query.Printing),
                Condition = Clean(query.Condition),
                Limit = limit,
                Offset = offset
            };
        }

        /// <summary>
        /// Maps a raw API card object into the simplified search result record used by the client.
        /// </summary>
        /// <param name="card">The raw API card object.</param>
        /// <returns>A simplified card search result.</returns>
        private static CardSearchResult MapToResult(ApiCard card)
        {
            decimal? lowest = null;
            if (card.Variants is { Count: > 0 })
            {
                lowest = card.Variants
                    .Where(v => v.Price is not null)
                    .Select(v => v.Price!.Value)
                    .DefaultIfEmpty()
                    .Min();

                if (lowest == 0m)
                {
                    lowest = null;
                }
            }

            string? imageUrl = null;
            if (!string.IsNullOrWhiteSpace(card.TcgplayerId))
            {
                imageUrl = $"https://tcgplayer-cdn.tcgplayer.com/product/{card.TcgplayerId}_in_1000x1000.jpg";
            }

            return new CardSearchResult(
                Id: card.Id ?? string.Empty,
                Name: card.Name ?? string.Empty,
                Game: card.Game ?? string.Empty,
                SetId: card.Set ?? string.Empty,
                SetName: card.SetName ?? string.Empty,
                Number: card.Number,
                Rarity: card.Rarity,
                TcgplayerId: card.TcgplayerId,
                LowestVariantPrice: lowest,
                ImageUrl: imageUrl);
        }

        /// <summary>
        /// Retrieves the JustTCG API key from the process environment.
        /// </summary>
        /// <returns>The configured API key.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the API key environment variable is not set.</exception>
        private static string GetApiKey()
        {
            string? apiKey = Environment.GetEnvironmentVariable(ApiKeyEnvironmentVariable);
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException(
                    $"Missing JustTCG API key. Set the {ApiKeyEnvironmentVariable} environment variable before running the client.");
            }

            return apiKey;
        }

        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true
        };

        /// <summary>
        /// Represents the outer API response envelope returned by JustTCG.
        /// </summary>
        /// <typeparam name="T">The type stored in the response data payload.</typeparam>
        private sealed class ApiEnvelope<T>
        {
            /// <summary>
            /// Gets or sets the response data payload.
            /// </summary>
            [JsonPropertyName("data")]
            public T? Data { get; set; }

            /// <summary>
            /// Gets or sets the pagination metadata.
            /// </summary>
            [JsonPropertyName("meta")]
            public ApiMeta? Meta { get; set; }

            /// <summary>
            /// Gets or sets the error message when a request fails.
            /// </summary>
            [JsonPropertyName("error")]
            public string? Error { get; set; }

            /// <summary>
            /// Gets or sets the API error code when a request fails.
            /// </summary>
            [JsonPropertyName("code")]
            public string? Code { get; set; }
        }

        /// <summary>
        /// Represents pagination metadata returned by the cards endpoint.
        /// </summary>
        private sealed class ApiMeta
        {
            [JsonPropertyName("total")]
            public int Total { get; set; }

            [JsonPropertyName("limit")]
            public int Limit { get; set; }

            [JsonPropertyName("offset")]
            public int Offset { get; set; }

            [JsonPropertyName("hasMore")]
            public bool HasMore { get; set; }
        }

        /// <summary>
        /// Represents the error payload returned by the API for unsuccessful requests.
        /// </summary>
        private sealed class ApiErrorEnvelope
        {
            [JsonPropertyName("error")]
            public string? Error { get; set; }

            [JsonPropertyName("code")]
            public string? Code { get; set; }
        }

        /// <summary>
        /// Represents the raw card object returned by the cards endpoint.
        /// </summary>
        private sealed class ApiCard
        {
            [JsonPropertyName("id")]
            public string? Id { get; set; }

            [JsonPropertyName("name")]
            public string? Name { get; set; }

            [JsonPropertyName("game")]
            public string? Game { get; set; }

            [JsonPropertyName("set")]
            public string? Set { get; set; }

            [JsonPropertyName("set_name")]
            public string? SetName { get; set; }

            [JsonPropertyName("number")]
            public string? Number { get; set; }

            [JsonPropertyName("tcgplayerId")]
            public string? TcgplayerId { get; set; }

            [JsonPropertyName("rarity")]
            public string? Rarity { get; set; }

            [JsonPropertyName("details")]
            public string? Details { get; set; }

            [JsonPropertyName("variants")]
            public List<ApiVariant>? Variants { get; set; }
        }

        /// <summary>
        /// Represents the raw variant object nested under a card response.
        /// </summary>
        private sealed class ApiVariant
        {
            [JsonPropertyName("id")]
            public string? Id { get; set; }

            [JsonPropertyName("condition")]
            public string? Condition { get; set; }

            [JsonPropertyName("printing")]
            public string? Printing { get; set; }

            [JsonPropertyName("language")]
            public string? Language { get; set; }

            [JsonPropertyName("tcgplayerSkuId")]
            public string? TcgplayerSkuId { get; set; }

            [JsonPropertyName("price")]
            public decimal? Price { get; set; }

            [JsonPropertyName("lastUpdated")]
            public long? LastUpdated { get; set; }
        }
    }
}
