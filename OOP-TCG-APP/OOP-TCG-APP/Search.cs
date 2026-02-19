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
    // Query shape matches JustTCG /cards filters:
    // - Search: q
    // - Filter: game, set
    // - Paging: limit, offset
    // - Sorting: orderBy (price, 24h, 7d, 30d), order (asc/desc)
    // Auth header: x-api-key :contentReference[oaicite:0]{index=0}
    public sealed record CardSearchQuery(
        string? Q = null,
        string? Game = null,   // e.g. "magic-the-gathering", "pokemon" :contentReference[oaicite:1]{index=1}
        string? Set = null,    // set id
        int Limit = 20,
        int Offset = 0,
        string? OrderBy = null,
        string? Order = null,
        bool IncludePriceHistory = false,
        string? PriceHistoryDuration = null, // 7d,30d,90d,180d :contentReference[oaicite:2]{index=2}
        string? IncludeStatistics = null,     // e.g. "7d,30d" or "allTime" :contentReference[oaicite:3]{index=3}
        bool IncludeNullPrices = false,
        string? Printing = null,              // e.g. "Normal", "Foil" :contentReference[oaicite:4]{index=4}
        string? Condition = null              // e.g. "NM,LP" or "Near Mint" :contentReference[oaicite:5]{index=5}
    );

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

    public sealed record PageMeta(int Total, int Limit, int Offset, bool HasMore);

    public sealed record PagedResult<T>(IReadOnlyList<T> Items, PageMeta? Meta);

    public sealed class JustTcgApiException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public string? ApiCode { get; }

        public JustTcgApiException(HttpStatusCode statusCode, string message, string? apiCode = null)
            : base(message)
        {
            StatusCode = statusCode;
            ApiCode = apiCode;
        }
    }

    public sealed class JustTcgCardsClient
    {
        private readonly HttpClient _http;
        private readonly string _apiKey = "tcg_1bc013d729d544629d3362cf062c14a1";
        private readonly Uri _baseUri = new("https://api.justtcg.com/v1/"); // Base URL :contentReference[oaicite:6]{index=6}

        public JustTcgCardsClient(HttpClient http)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
        }

        /// <summary>
        /// GET /cards search + filters. Returns card objects (each contains variants with pricing). :contentReference[oaicite:7]{index=7}
        /// </summary>
        public async Task<PagedResult<CardSearchResult>> SearchCardsAsync(CardSearchQuery query, CancellationToken ct = default)
        {
            query = Normalize(query);

            var url = BuildCardsUri(query);

            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.TryAddWithoutValidation("x-api-key", _apiKey); // Manual API key authentication :contentReference[oaicite:8]{index=8}

            using var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
            await using var stream = await resp.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);

            if (!resp.IsSuccessStatusCode)
            {
                ApiErrorEnvelope? err = null;
                try { err = await JsonSerializer.DeserializeAsync<ApiErrorEnvelope>(stream, JsonOpts, ct).ConfigureAwait(false); }
                catch { /* ignore parse errors */ }

                var msg = err?.Error ?? $"HTTP {(int)resp.StatusCode} {resp.ReasonPhrase}";
                throw new JustTcgApiException(resp.StatusCode, msg, err?.Code);
            }

            var api = await JsonSerializer.DeserializeAsync<ApiEnvelope<List<ApiCard>>>(stream, JsonOpts, ct).ConfigureAwait(false);
            var cards = api?.Data ?? new List<ApiCard>();

            var items = cards.Select(MapToResult).ToList();

            PageMeta? meta = null;
            if (api?.Meta is not null)
                meta = new PageMeta(api.Meta.Total, api.Meta.Limit, api.Meta.Offset, api.Meta.HasMore);

            return new PagedResult<CardSearchResult>(items, meta);
        }

        private Uri BuildCardsUri(CardSearchQuery q)
        {
            // /cards endpoint supports direct lookup OR search query; identifiers take precedence. :contentReference[oaicite:9]{index=9}
            // For your "search function" use q/game/set + pagination + sorting.
            var queryParams = new Dictionary<string, string?>()
            {
                ["q"] = q.Q,
                ["game"] = q.Game,
                ["set"] = q.Set,
                ["limit"] = q.Limit.ToString(),
                ["offset"] = q.Offset.ToString(),
                ["orderBy"] = q.OrderBy, // 'price', '24h', '7d', '30d' :contentReference[oaicite:10]{index=10}
                ["order"] = q.Order,     // 'asc'/'desc' :contentReference[oaicite:11]{index=11}
                ["include_price_history"] = q.IncludePriceHistory ? "true" : "false",
                ["priceHistoryDuration"] = q.PriceHistoryDuration, // 7d,30d,90d,180d :contentReference[oaicite:12]{index=12}
                ["include_statistics"] = q.IncludeStatistics,      // 7d,30d,90d,1y,allTime :contentReference[oaicite:13]{index=13}
                ["include_null_prices"] = q.IncludeNullPrices ? "true" : "false",
                ["printing"] = q.Printing,
                ["condition"] = q.Condition
            };

            var qs = string.Join("&",
                queryParams
                    .Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
                    .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value!)}"));

            var builder = new UriBuilder(new Uri(_baseUri, "cards"))
            {
                Query = qs
            };
            return builder.Uri;
        }

        private static CardSearchQuery Normalize(CardSearchQuery q)
        {
            static string? Clean(string? s)
            {
                if (string.IsNullOrWhiteSpace(s)) return null;
                var parts = s.Trim().Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                return string.Join(' ', parts);
            }

            var limit = q.Limit;
            if (limit < 1) limit = 20;
            if (limit > 200) limit = 200; // doc mentions plan-based batch limits; GET limit still best kept sane :contentReference[oaicite:14]{index=14}

            var offset = q.Offset < 0 ? 0 : q.Offset;

            return q with
            {
                Q = Clean(q.Q),
                Game = Clean(q.Game),
                Set = Clean(q.Set),
                OrderBy = Clean(q.OrderBy),
                Order = Clean(q.Order),
                PriceHistoryDuration = Clean(q.PriceHistoryDuration),
                IncludeStatistics = Clean(q.IncludeStatistics),
                Printing = Clean(q.Printing),
                Condition = Clean(q.Condition),
                Limit = limit,
                Offset = offset
            };
        }

        private static CardSearchResult MapToResult(ApiCard c)
        {
            decimal? lowest = null;
            if (c.Variants is { Count: > 0 })
            {
                lowest = c.Variants
                    .Where(v => v.Price is not null)
                    .Select(v => v.Price!.Value)
                    .DefaultIfEmpty()
                    .Min();

                if (lowest == 0m) lowest = null;
            }

            // Docs: card has tcgplayerId (TCGplayer product ID). :contentReference[oaicite:15]{index=15}
            // If you want TCGplayer image URLs, this is the product ID you’d use.
            string? imageUrl = null;
            if (!string.IsNullOrWhiteSpace(c.TcgplayerId))
                imageUrl = $"https://tcgplayer-cdn.tcgplayer.com/product/{c.TcgplayerId}_in_1000x1000.jpg";

            return new CardSearchResult(
                Id: c.Id ?? "",
                Name: c.Name ?? "",
                Game: c.Game ?? "",
                SetId: c.Set ?? "",
                SetName: c.SetName ?? "",
                Number: c.Number,
                Rarity: c.Rarity,
                TcgplayerId: c.TcgplayerId,
                LowestVariantPrice: lowest,
                ImageUrl: imageUrl
            );
        }

        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true
        };

        // --------------------------
        // DTOs matching JustTCG docs
        // --------------------------

        private sealed class ApiEnvelope<T>
        {
            [JsonPropertyName("data")]
            public T? Data { get; set; }

            [JsonPropertyName("meta")]
            public ApiMeta? Meta { get; set; }

            [JsonPropertyName("error")]
            public string? Error { get; set; }

            [JsonPropertyName("code")]
            public string? Code { get; set; }
        }

        private sealed class ApiMeta
        {
            [JsonPropertyName("total")] public int Total { get; set; }
            [JsonPropertyName("limit")] public int Limit { get; set; }
            [JsonPropertyName("offset")] public int Offset { get; set; }
            [JsonPropertyName("hasMore")] public bool HasMore { get; set; }
        }

        private sealed class ApiErrorEnvelope
        {
            [JsonPropertyName("error")] public string? Error { get; set; }
            [JsonPropertyName("code")] public string? Code { get; set; }
        }

        private sealed class ApiCard
        {
            // Card Object fields :contentReference[oaicite:16]{index=16}
            [JsonPropertyName("id")] public string? Id { get; set; }
            [JsonPropertyName("name")] public string? Name { get; set; }
            [JsonPropertyName("game")] public string? Game { get; set; }
            [JsonPropertyName("set")] public string? Set { get; set; }
            [JsonPropertyName("set_name")] public string? SetName { get; set; }
            [JsonPropertyName("number")] public string? Number { get; set; }
            [JsonPropertyName("tcgplayerId")] public string? TcgplayerId { get; set; }
            [JsonPropertyName("rarity")] public string? Rarity { get; set; }
            [JsonPropertyName("details")] public string? Details { get; set; }

            [JsonPropertyName("variants")]
            public List<ApiVariant>? Variants { get; set; }
        }

        private sealed class ApiVariant
        {
            // Variant Object fields :contentReference[oaicite:17]{index=17}
            [JsonPropertyName("id")] public string? Id { get; set; }
            [JsonPropertyName("condition")] public string? Condition { get; set; }
            [JsonPropertyName("printing")] public string? Printing { get; set; }
            [JsonPropertyName("language")] public string? Language { get; set; }
            [JsonPropertyName("tcgplayerSkuId")] public string? TcgplayerSkuId { get; set; }
            [JsonPropertyName("price")] public decimal? Price { get; set; }
            [JsonPropertyName("lastUpdated")] public long? LastUpdated { get; set; }
        }
    }
}
