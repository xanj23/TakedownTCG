
// <summary>
// This model class is for the card data. When the data has a version of the cardit gives the info on that card.
// This is for the general card. Not indivdual cards that you want to buy.
// </summary>
public class Card
{
    public string Id { get; set; } // cardID
    public string Name { get; set; } // Name of the card
    public string Game { get; set; } // what game its from
    public string Set { get; set; } // Unique Id of the set it comes from
    public string SetName { get; set; } // Name of the set
    public string Number { get; set; } // card number
    public string TcgplayerId { get; set; } // TCGplayer product ID
    public string MtgjsonId { get; set; } //MTGJSON UUID for the card
    public string ScryfallId { get; set; } // Scryfall UUID for the card
    public string Rarity { get; set; } // Rarity of the card (e.g., Common, Rare, etc.)
    public string Details { get; set; } // Additional card-specific details, if avaiable
    public List<Variant> Variants { get; set; } = new(); // creates a list all the variants found
}