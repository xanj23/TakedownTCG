using System.Net;
using TCGAPP;

public class BuildApiURL
{
    public static string Run(IApi api, Endpoint endpoint, string builtQuery)
    {
        string apiURL = api.BaseUrl + endpoint.URL + builtQuery;
        return WebUtility.UrlEncode(apiURL);
    }
}