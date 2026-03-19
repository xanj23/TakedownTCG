using System.Net;
using TCGAPP;

public class BuildApiURL
{
    public static string Run(IApi api, Endpoint endpoint, string builtQuery)
    {
        return api.BaseUrl + endpoint.URL + builtQuery;
    }
}