using System.Net;

public static class BuildQuery
{
    /// <summary>
    /// 
    /// </summary>
    public static string Run(Dictionary<string, string> rawQuery)
    {
        if (rawQuery == null || rawQuery.Count == 0)
        {
            return "";
        }

        List<string> parameters = new List<string>();
        foreach (KeyValuePair<string, string> pair in rawQuery)
        {
            string key = pair.Key;
            string value = WebUtility.UrlEncode(pair.Value);
            parameters.Add(key + '=' + value);
        }

        return '?' + string.Join('&', parameters);
    }
}
