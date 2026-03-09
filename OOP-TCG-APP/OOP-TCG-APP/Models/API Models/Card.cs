using System.Collections.Generic;

/// <summary>
/// Represents a collectible card, including metadata such as the game it belongs to,
/// the set it is part of, card number, rarity, and available variants.
/// This class models the general card information, not individual cards for purchase.
/// </summary>
public class Card
{
    /// <summary>
    /// Unique identifier for the card.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Name of the card.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Name of the game this card belongs to.
    /// </summary>
    public string Game { get; set; }

    /// <summary>
    /// Unique identifier of the set this card comes from.
    /// </summary>
    public string Set { get; set; }

    /// <summary>
    /// Name of the set this card comes from.
    /// </summary>
    public string SetName { get; set; }

    /// <summary>
    /// Card number within the set.
    /// </summary>
    public string Number { get; set; }

    /// <summary>
    /// TCGplayer product ID for this card.
    /// </summary>
    public string TcgplayerId { get; set; }

    /// <summary>
    /// MTGJSON UUID for the card.
    /// </summary>
    public string MtgjsonId { get; set; }

    /// <summary>
    /// Scryfall UUID for the card.
    /// </summary>
    public string ScryfallId { get; set; }

    /// <summary>
    /// Rarity of the card (e.g., Common, Rare, etc.).
    /// </summary>
    public string Rarity { get; set; }

    /// <summary>
    /// Additional card-specific details, if available.
    /// </summary>
    public string Details { get; set; }

    /// <summary>
    /// List of all variants found for this card.
    /// </summary>
    public List<Variant> Variants { get; set; } = new();
}