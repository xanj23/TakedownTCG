namespace JustTCG {
/// <summary>
/// Builds a URL query string from a dictionary of request parameters.
/// </summary>
public static class BuildQuery
{
    /// <summary>
    /// Converts the supplied parameter dictionary into a URL-encoded query string.
    /// </summary>
    /// <param name="rawQuery">The parameter names and values to append to the request.</param>
    /// <returns>A URL query string beginning with <c>?</c>, or an empty string when no parameters are provided.</returns>
    public static string Run(Dictionary<string, string> rawQuery)
    {
        if (rawQuery == null || rawQuery.Count == 0)
        {
            return string.Empty;
        }

        List<string> parameters = new List<string>();
        foreach (KeyValuePair<string, string> pair in rawQuery)
        {
            string key = pair.Key;
            string value = pair.Value;
            parameters.Add(key + '=' + value);
        }

        return '?' + string.Join('&', parameters);
    }
}
}