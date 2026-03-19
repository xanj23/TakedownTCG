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
            Parameters.Add("Q", new QueryParam<object>("Name", true));
            Parameters.Add("Number", new QueryParam<object>("Card number (e.g., 15)", false));
            Parameters.Add("Printing", new QueryParam<object>("Printing", false));
            Parameters.Add("Condition", new QueryParam<object>("Condition", false));
        }
    }
}