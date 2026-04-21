using System.Text.Json.Serialization;

namespace TakedownTCGApplication.Models.JustTcg.Response
{
    /// <summary>
    /// Represents a set record returned by the JustTCG sets endpoint.
    /// </summary>
    public class Set
    {
        /// <summary>
        /// Gets or sets the unique set identifier.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the set name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the owning game identifier.
        /// </summary>
        [JsonPropertyName("game_id")]
        public string GameId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the owning game name.
        /// </summary>
        [JsonPropertyName("game")]
        public string Game { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the number of cards in the set.
        /// </summary>
        [JsonPropertyName("cards_count")]
        public int CardsCount { get; set; }

        /// <summary>
        /// Gets or sets the number of variants in the set.
        /// </summary>
        [JsonPropertyName("variants_count")]
        public int VariantsCount { get; set; }

        /// <summary>
        /// Gets or sets the number of sealed products in the set.
        /// </summary>
        [JsonPropertyName("sealed_count")]
        public int SealedCount { get; set; }

        /// <summary>
        /// Gets or sets the release date string returned by the API.
        /// </summary>
        [JsonPropertyName("release_date")]
        public string ReleaseDate { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the aggregate set value in USD.
        /// </summary>
        [JsonPropertyName("set_value_usd")]
        public float SetValueUsd { get; set; }

        /// <summary>
        /// Gets or sets the seven-day percentage change in set value.
        /// </summary>
        [JsonPropertyName("set_value_change_7d_pct")]
        public float SetValueChange7dPct { get; set; }

        /// <summary>
        /// Gets or sets the thirty-day percentage change in set value.
        /// </summary>
        [JsonPropertyName("set_value_change_30d_pct")]
        public float SetValueChange30dPct { get; set; }

        /// <summary>
        /// Gets or sets the ninety-day percentage change in set value.
        /// </summary>
        [JsonPropertyName("set_value_change_90d_pct")]
        public float SetValueChange90dPct { get; set; }
    }
}

