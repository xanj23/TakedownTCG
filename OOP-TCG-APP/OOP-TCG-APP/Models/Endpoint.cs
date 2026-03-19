namespace TCGAPP
{
    // All APIs must implement this
    // Represents a single endpoint for an API
    public class Endpoint
    {
        public string Name { get; set; }  // Display name
        public string URL { get; set; }   // URL path
        public IQueryParams? Parameters { get; set; } // Where to get parameters
    }
}