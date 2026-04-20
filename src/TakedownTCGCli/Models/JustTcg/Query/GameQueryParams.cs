using System.Collections.Generic;

namespace TakedownTCG.cli.Models.JustTcg.Query
{
    /// <summary>
    /// Marker query for the JustTCG games endpoint (no query parameters).
    /// </summary>
    public sealed class GameQueryParams : IQueryParams
    {
        public Dictionary<string, QueryParameter> Parameters { get; } = new Dictionary<string, QueryParameter>();
    }
}

