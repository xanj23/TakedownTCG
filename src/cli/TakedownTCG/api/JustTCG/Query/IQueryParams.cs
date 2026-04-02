using System.Collections.Generic;

namespace TakedownTCG.cli.Api.JustTCG.Query
{
    public interface IQueryParams
    {
        Dictionary<string, QueryParameter> Parameters { get; }
    }
}
