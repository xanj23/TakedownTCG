using System.Collections.Generic;

namespace TakedownTCG.cli.Api.JustTCG.Response
{
    /// <summary>
    /// Represents a card record returned by the JustTCG cards endpoint.
    /// </summary>
    public class Card
    {
        /// <summary>
        /// Gets or sets the unique card identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the card name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the game the card belongs to.
        /// </summary>
        public string Game { get; set; }

        /// <summary>
        /// Gets or sets the set identifier.
        /// </summary>
        public string Set { get; set; }

        /// <summary>
        /// Gets or sets the set name.
        /// </summary>
        public string SetName { get; set; }

        /// <summary>
        /// Gets or sets the collector number within the set.
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Gets or sets the linked TCGplayer product identifier.
        /// </summary>
        public string TcgplayerId { get; set; }

        /// <summary>
        /// Gets or sets the linked MTGJSON identifier.
        /// </summary>
        public string MtgjsonId { get; set; }

        /// <summary>
        /// Gets or sets the linked Scryfall identifier.
        /// </summary>
        public string ScryfallId { get; set; }

        /// <summary>
        /// Gets or sets the card rarity.
        /// </summary>
        public string Rarity { get; set; }

        /// <summary>
        /// Gets or sets additional descriptive details returned by the API.
        /// </summary>
        public string Details { get; set; }

        /// <summary>
        /// Gets or sets the available card variants.
        /// </summary>
        public List<Variant> Variants { get; set; } = new();
    }
}
