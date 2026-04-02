using System.Collections.Generic;

namespace TakedownTCG.cli.Api.JustTCG.Query
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
        // Key = API parameter name, Value = prompt/value metadata for that field.
        public Dictionary<string, QueryParameter> Parameters { get; } = new Dictionary<string, QueryParameter>();

        public SetQueryParams()
        {
            Parameters.Add("game", new QueryParameter("Game (e.g., mtg, pokemon)", true));
            Parameters.Add("q", new QueryParameter("Search query", false));
            Parameters.Add("orderBy", new QueryParameter("Order by (name, release date)", false));
            Parameters.Add("order", new QueryParameter("Sort order (asc, desc)", false));
        }
    }
}
