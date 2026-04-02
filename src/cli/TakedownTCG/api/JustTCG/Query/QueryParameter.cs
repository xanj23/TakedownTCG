using System.Collections.Generic;

namespace TakedownTCG.cli.Api.JustTCG.Query
{
    /// <summary>
    /// Represents a single query field, including prompt metadata and the entered value.
    /// </summary>
    public sealed class QueryParameter
    {
        public object? Value { get; set; }
        public string Label { get; }
        public bool IsRequired { get; }
        public IReadOnlyList<object>? Options { get; }

        public QueryParameter(string label, bool isRequired, IReadOnlyList<object>? options = null)
        {
            Label = label;
            IsRequired = isRequired;
            Options = options;
        }
    }
}
