/// <summary>
/// 
/// </summary>

public enum Condition { S, NM, LP, MP, HP, DMG }

public class CardQueryParameters {
    public string? Q { get; set; }
    public string? Number { get; set; }
    public string? Printing { get; set; }
    public Condition? Condition { get; set; }
}
