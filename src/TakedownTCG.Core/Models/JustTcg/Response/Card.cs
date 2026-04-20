using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TakedownTCG.Core.Models.JustTcg.Response
{
    /// <summary>
    /// Represents a card record returned by the JustTCG cards endpoint.
    /// </summary>
    public class Card
    {
        /// <summary>
        /// Gets or sets the unique card identifier.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the card name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the game the card belongs to.
        /// </summary>
        [JsonPropertyName("game")]
        public string Game { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the set identifier.
        /// </summary>
        [JsonPropertyName("set")]
        public string Set { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the set name.
        /// </summary>
        [JsonPropertyName("set_name")]
        public string SetName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the collector number within the set.
        /// </summary>
        [JsonPropertyName("number")]
        public string Number { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the linked TCGplayer product identifier.
        /// </summary>
        [JsonPropertyName("tcgplayerId")]
        public string TcgplayerId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the linked MTGJSON identifier.
        /// </summary>
        [JsonPropertyName("mtgjsonId")]
        public string MtgjsonId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the linked Scryfall identifier.
        /// </summary>
        [JsonPropertyName("scryfallId")]
        public string ScryfallId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the card rarity.
        /// </summary>
        [JsonPropertyName("rarity")]
        public string Rarity { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets additional descriptive details returned by the API.
        /// </summary>
        [JsonPropertyName("details")]
        public string Details { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the available card variants.
        /// </summary>
        [JsonPropertyName("variants")]
        public List<Variant> Variants { get; set; } = new();
    }
}

