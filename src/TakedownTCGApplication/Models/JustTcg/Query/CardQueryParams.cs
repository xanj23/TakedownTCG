using System.Collections.Generic;

namespace TakedownTCG.Core.Models.JustTcg.Query
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
        // Key = API parameter name, Value = prompt/value metadata for that field.
        public Dictionary<string, QueryParameter> Parameters { get; } = new Dictionary<string, QueryParameter>();

        public CardQueryParams()
        {
            Parameters.Add("q", new QueryParameter("Name", true));
            Parameters.Add("number", new QueryParameter("Card number (e.g., 15)", false));
            Parameters.Add("printing", new QueryParameter("Printing", false));
            Parameters.Add("condition", new QueryParameter("Condition", false));
            Parameters.Add("limit", new QueryParameter("Page size", false));
            Parameters.Add("offset", new QueryParameter("Page offset", false));
        }
    }
}

