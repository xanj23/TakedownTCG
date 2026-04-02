using System.Collections.Generic;

namespace TakedownTCG.cli.Api.JustTCG.Query
{
    /// <summary>
    /// Generic class representing a query parameter.
    /// </summary>
    /// <typeparam name="T">The type of the parameter value.</typeparam>
    public class QueryParam<T>
    {
        public T? Value { get; set; }
        public string Label { get; set; }
        public bool IsRequired { get; set; }
        public List<T>? Options { get; set; }

        public QueryParam(string label, bool isRequired, List<T>? options = null)
        {
            Label = label;
            IsRequired = isRequired;
            Options = options;
        }
    }
}
