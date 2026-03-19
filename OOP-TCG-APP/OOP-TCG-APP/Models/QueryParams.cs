namespace TCGAPP
{
    /// <summary>
    /// Generic class representing a query parameter.
    /// </summary>
    /// <typeparam name="T">The type of the parameter value.</typeparam>
    public class QueryParam<T>
    {
        public T? Value { get; set; }          // The user-provided value
        public string Label { get; set; }      // Friendly label for console prompts
        public bool IsRequired { get; set; }   // Whether the field is required
        public List<T>? Options { get; set; }

        public QueryParam(string label, bool isRequired, List<T>? options = null)
        {
            Label = label;
            IsRequired = isRequired;
            Options = options;
        }
    }
}