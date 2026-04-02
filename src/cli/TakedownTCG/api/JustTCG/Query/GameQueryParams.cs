using System.Collections.Generic;

namespace TakedownTCG.cli.Api.JustTCG.Query
{
    /// <summary>
    /// Marker query for the JustTCG games endpoint (no query parameters).
    /// </summary>
    public sealed class GameQueryParams : IQueryParams
    {
        public Dictionary<string, QueryParam<object>> Parameters { get; } = new Dictionary<string, QueryParam<object>>();
    }
}
