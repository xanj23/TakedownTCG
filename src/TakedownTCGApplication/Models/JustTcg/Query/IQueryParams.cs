using System.Collections.Generic;

namespace TakedownTCGApplication.Models.JustTcg.Query
{
    public interface IQueryParams
    {
        Dictionary<string, QueryParameter> Parameters { get; }
    }
}

