using System.Reflection;

/// <summary>
/// Converts a query parameter object into a dictionary of non-null property values.
/// </summary>
public class ObjToDict
{
    /// <summary>
    /// Reads the public properties of the supplied object and maps non-null values by property name.
    /// </summary>
    /// <param name="rawQuery">The object that contains query parameter values.</param>
    /// <returns>A dictionary of lowercase property names and their string values.</returns>
    public static Dictionary<string, string> Run(object rawQuery)
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();
        PropertyInfo[] properties = rawQuery.GetType().GetProperties();

        foreach (PropertyInfo property in properties)
        {
            string key = property.Name.ToLower();
            object? value = property.GetValue(rawQuery);

            if (value != null)
            {
                dict.Add(key, value.ToString() ?? string.Empty);
            }
        }

        return dict;
    }
}
