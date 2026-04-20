using System.Collections.Generic;

namespace TakedownTCG.cli.Models.JustTcg.Query
{
    public interface IQueryParams
    {
        Dictionary<string, QueryParameter> Parameters { get; }
    }
}

