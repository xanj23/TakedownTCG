
// <summary>
//This model handles the pages of data. The api response will hit a limit so it gives a page of data.
// This data keeps track of what page we are on.
// </summary>
public class Meta
{
    public int Total { get; set; } // Total number of cards matching the search prompt
    public int Limit { get; set; } // Number of results on the page
    public int Offset { get; set; } // current page on
    public bool HasMore { get; set; } // Whether there are more results available
}
