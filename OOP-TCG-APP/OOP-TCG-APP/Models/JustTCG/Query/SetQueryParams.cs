using TCGAPP;

namespace JustTCG
{
    /// <summary>
    /// Defines the supported sort fields for a JustTCG set search request.
    /// </summary>
    public enum OrderBy
    {
        name,
        release_date
    }

    /// <summary>
    /// Defines the supported sort directions for a JustTCG set search request.
    /// </summary>
    public enum Order
    {
        asc,
        desc
    }

    /// <summary>
    /// Stores parameters for a JustTCG set search dynamically in a dictionary.
    /// </summary>
    public class SetQueryParams : IQueryParams
    {
        // Key = param name, Value = QueryParam (string or enum)
        public Dictionary<string, QueryParam<object>> Parameters { get; } = new Dictionary<string, QueryParam<object>>();

        public SetQueryParams()
        {
            Parameters.Add("game", new QueryParam<object>("Game (e.g., mtg, pokemon)", true));
            Parameters.Add("q", new QueryParam<object>("Search query", false));
            Parameters.Add("orderBy", new QueryParam<object>("Order by (name, release date)", false));
            Parameters.Add("order", new QueryParam<object>("Sort order (asc, desc)", false));
        }
    }
}
