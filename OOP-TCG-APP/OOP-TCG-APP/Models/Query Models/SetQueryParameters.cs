/// <summary>
/// 
/// </summary>

public enum OrderBy { name, release_date }
public enum Order { asc, desc }
public class SetQueryParameters
{
        public string? Q { get; set; }
        public string? Game { get; set; }
        public OrderBy? OrderBy { get; set; }
        public Order? Order { get; set; }
}
