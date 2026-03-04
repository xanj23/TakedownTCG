using System.Collections.Generic; // Lets us use list

// <summary>
// This Model class is the bases for the API calls from JustTCG. When a call is made it comes in as a JSON. 
//This class means to be a wrapper aroud the model/data classes
//
// There's the "data" which hows the information on the cards from the search or filtering.
// Theres "meta" which is based on the page of data. Due to the large amount of cards. Pages to display is needed.
// Then There's "_metadata" which is based on the API key owner's plan. Gives information on a how many request are left.
// Finally, there's "error" and "code. If there is an error present then it will display a code and message.
//
// 400: Bad Request - The request was malformed or missing required parameters.
// 401: Unauthorized - Missing or invalid API key.
// 403: Forbidden - Valid API key, but insufficient permissions for the requested resource.
// 404: Not Found - The requested resource could not be found.
// 429: Too Many Requests - Rate limit exceeded. Slow down your requests.
// 500: Server Error - An unexpected error occurred on the server.
// </summary>
public class ApiResponse<T>
{
    public List<T> Data { get; set; } = new List<T>();  // the data coming in as a list of cards
    public Meta Meta { get; set; } = new Meta(); // Info about pages
    public Metadata _metadata { get; set; } = new Metadata(); // API usage info
    public string? Error { get; set; } // Error message
    public string? Code { get; set; } // Error code
}
