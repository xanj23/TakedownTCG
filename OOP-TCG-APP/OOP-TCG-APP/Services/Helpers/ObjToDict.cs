using System;
using System.Collections.Generic;
using System.Reflection;

public class ObjToDict
{
    public static Dictionary<string, string> Run(object rawQuery)
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();
        PropertyInfo[] properties = rawQuery.GetType().GetProperties();

        foreach (PropertyInfo property in properties)
        {
            string key = property.Name.ToLower();
            object value = property.GetValue(rawQuery);

            if (value != null)
            {
                dict.Add(key, value.ToString());
            }
        }

        return dict;
    }
}