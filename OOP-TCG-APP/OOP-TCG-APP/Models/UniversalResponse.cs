using System.Collections.Generic;

namespace TCGAPP
{
    /// <summary>
    /// Canonical response model used as the basis for displaying API results.
    /// </summary>
    public class UniversalResponse
    {
        /// <summary>
        /// Captures the source response type name for diagnostics and display context.
        /// </summary>
        public string? SourceType { get; set; }

        /// <summary>
        /// Stores the raw response content when a response is not structured.
        /// </summary>
        public string? Raw { get; set; }

        /// <summary>
        /// Collects any errors encountered during mapping or returned by the API.
        /// </summary>
        public List<string> Errors { get; } = new List<string>();

        /// <summary>
        /// Stores metadata such as pagination or rate-limit details for display.
        /// </summary>
        public Dictionary<string, string?> Meta { get; } = new Dictionary<string, string?>();

        /// <summary>
        /// Contains the normalized records that can be rendered uniformly by the UI.
        /// </summary>
        public List<UniversalRecord> Records { get; } = new List<UniversalRecord>();
    }

    /// <summary>
    /// A single record of displayable fields.
    /// </summary>
    public class UniversalRecord
    {
        /// <summary>
        /// Holds field/value pairs for a single response item.
        /// </summary>
        public Dictionary<string, string?> Fields { get; } = new Dictionary<string, string?>();
    }
}
