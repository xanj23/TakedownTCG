using System.Diagnostics;

public static class BuildQuery
{

    /// <summary>
    /// 
    /// </summary>
    /// 
    public static string Run(Dictionary rawQuery)
    {
        string query;
        for (int i = 1; i < rawQuery.Count; i+2)
        {
          query = query + "/" + tempQuery[i]
        }
        return query;
    }

}