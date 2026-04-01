using TCGAPP;

namespace JustTCG
{
    /// <summary>
    /// Defines the supported JustTCG card condition filters used by the console application.
    /// </summary>
    public enum Condition
    {
        S,
        NM,
        LP,
        MP,
        HP,
        DMG
    }

    /// <summary>
    /// Stores parameters for a JustTCG card search dynamically in a dictionary.
    /// </summary>
    public class CardQueryParams : IQueryParams
    {
        // Key = param name, Value = QueryParam (string or enum)
        public Dictionary<string, QueryParam<object>> Parameters { get; } = new Dictionary<string, QueryParam<object>>();

        public CardQueryParams()
        {
            // Add parameters dynamically
            Parameters.Add("q", new QueryParam<object>("Name", true));
            Parameters.Add("number", new QueryParam<object>("Card number (e.g., 15)", false));
            Parameters.Add("printing", new QueryParam<object>("Printing", false));
            Parameters.Add("condition", new QueryParam<object>("Condition", false));
        }
    }
}
