using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using TCGAPP;

public class MaptoUniversalResponse
{
    /// <summary>
    /// Maps any API response object to a <see cref="UniversalResponse"/> for display.
    /// </summary>
    /// <param name="api">The API that produced the response.</param>
    /// <param name="response">The raw response object to convert.</param>
    /// <returns>A populated <see cref="UniversalResponse"/> instance.</returns>
    public static UniversalResponse Run(TCGAPP.IApi api, object? response)
    {
        var result = new UniversalResponse
        {
            SourceType = api?.Name
        };

        if (response == null)
        {
            result.Errors.Add("Response was null. [MaptoUniversalResponse]");
            return result;
        }

        if (response is string rawString)
        {
            if (!TryMapJsonEnvelope(rawString, result))
            {
                result.Raw = rawString;
            }
            return result;
        }

        if (TryExtractEnvelope(response, out object? data, out object? meta, out object? metadata))
        {
            AddMeta(meta, result.Meta);
            AddMeta(metadata, result.Meta);
            AddDataRecords(data, result.Records);
            return result;
        }

        if (response is IEnumerable enumerable && response is not string)
        {
            foreach (object? item in enumerable)
            {
                result.Records.Add(ToRecord(item));
            }

            return result;
        }

        result.Records.Add(ToRecord(response));
        return result;
    }

    private static bool TryExtractEnvelope(object response, out object? data, out object? meta, out object? metadata)
    {
        data = null;
        meta = null;
        metadata = null;

        Type type = response.GetType();
        PropertyInfo? dataProp = type.GetProperty("Data");
        if (dataProp == null)
        {
            return false;
        }

        data = dataProp.GetValue(response);
        meta = type.GetProperty("Meta")?.GetValue(response);
        metadata = type.GetProperty("_metadata")?.GetValue(response) ?? type.GetProperty("Metadata")?.GetValue(response);
        return true;
    }

    /// <summary>
    /// Appends normalized records from a response payload into the target list.
    /// </summary>
    /// <param name="data">The data payload to process.</param>
    /// <param name="records">The destination record list.</param>
    private static void AddDataRecords(object? data, List<UniversalRecord> records)
    {
        if (data == null)
        {
            return;
        }

        if (data is IEnumerable enumerable && data is not string)
        {
            foreach (object? item in enumerable)
            {
                records.Add(ToRecord(item));
            }

            return;
        }

        records.Add(ToRecord(data));
    }

    /// <summary>
    /// Adds metadata fields into the universal metadata bucket.
    /// </summary>
    /// <param name="meta">The metadata object to flatten.</param>
    /// <param name="metaBucket">The destination metadata dictionary.</param>
    private static void AddMeta(object? meta, Dictionary<string, string?> metaBucket)
    {
        if (meta == null)
        {
            return;
        }

        foreach (var pair in ToFieldDictionary(meta))
        {
            if (!metaBucket.ContainsKey(pair.Key))
            {
                metaBucket.Add(pair.Key, pair.Value);
            }
        }
    }

    /// <summary>
    /// Converts an object into a <see cref="UniversalRecord"/> of displayable fields.
    /// </summary>
    /// <param name="item">The object to convert.</param>
    /// <returns>A record containing the object's public properties.</returns>
    private static UniversalRecord ToRecord(object? item)
    {
        var record = new UniversalRecord();
        if (item == null)
        {
            return record;
        }

        if (item is JsonElement jsonElement)
        {
            foreach (var pair in JsonElementToFields(jsonElement))
            {
                record.Fields[pair.Key] = pair.Value;
            }
            return record;
        }

        foreach (var pair in ToFieldDictionary(item))
        {
            record.Fields[pair.Key] = pair.Value;
        }

        return record;
    }

    /// <summary>
    /// Flattens public instance properties into string key/value pairs.
    /// </summary>
    /// <param name="obj">The object to inspect.</param>
    /// <returns>A dictionary of property names and stringified values.</returns>
    private static Dictionary<string, string?> ToFieldDictionary(object obj)
    {
        var fields = new Dictionary<string, string?>();
        Type type = obj.GetType();
        PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (PropertyInfo prop in props)
        {
            if (prop.GetIndexParameters().Length > 0)
            {
                continue;
            }

            object? value = prop.GetValue(obj);
            fields[prop.Name] = FormatValue(value);
        }

        return fields;
    }

    /// <summary>
    /// Formats a value for display, with special handling for collections.
    /// </summary>
    /// <param name="value">The value to format.</param>
    /// <returns>A display-friendly string or null.</returns>
    private static string? FormatValue(object? value)
    {
        if (value == null)
        {
            return null;
        }

        if (value is JsonElement jsonElement)
        {
            return JsonElementToString(jsonElement);
        }

        if (value is string str)
        {
            return str;
        }

        if (value is IEnumerable enumerable)
        {
            return FormatEnumerable(enumerable);
        }

        return value.ToString();
    }

    /// <summary>
    /// Summarizes an enumerable by returning a count-based descriptor.
    /// </summary>
    /// <param name="enumerable">The collection to summarize.</param>
    /// <returns>A string describing the item count.</returns>
    private static string FormatEnumerable(IEnumerable enumerable)
    {
        int count = 0;
        foreach (object? _ in enumerable)
        {
            count++;
            if (count > 100)
            {
                return "[100+ items]";
            }
        }

        return $"[{count} items]";
    }

    private static bool TryMapJsonEnvelope(string rawJson, UniversalResponse result)
    {
        try
        {
            using JsonDocument doc = JsonDocument.Parse(rawJson);
            JsonElement root = doc.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
            {
                return false;
            }

            if (!root.TryGetProperty("data", out JsonElement dataElement))
            {
                return false;
            }

            if (root.TryGetProperty("meta", out JsonElement metaElement))
            {
                AddJsonMeta(metaElement, result.Meta);
            }

            if (root.TryGetProperty("_metadata", out JsonElement metadataElement))
            {
                AddJsonMeta(metadataElement, result.Meta);
            }

            AddJsonRecords(dataElement, result.Records);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static void AddJsonRecords(JsonElement dataElement, List<UniversalRecord> records)
    {
        if (dataElement.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement item in dataElement.EnumerateArray())
            {
                records.Add(ToRecord(item));
            }
            return;
        }

        if (dataElement.ValueKind == JsonValueKind.Object)
        {
            records.Add(ToRecord(dataElement));
        }
    }

    private static void AddJsonMeta(JsonElement metaElement, Dictionary<string, string?> metaBucket)
    {
        if (metaElement.ValueKind != JsonValueKind.Object)
        {
            return;
        }

        foreach (JsonProperty prop in metaElement.EnumerateObject())
        {
            if (!metaBucket.ContainsKey(prop.Name))
            {
                metaBucket.Add(prop.Name, JsonElementToString(prop.Value));
            }
        }
    }

    private static Dictionary<string, string?> JsonElementToFields(JsonElement element)
    {
        var fields = new Dictionary<string, string?>();
        if (element.ValueKind != JsonValueKind.Object)
        {
            return fields;
        }

        foreach (JsonProperty prop in element.EnumerateObject())
        {
            fields[prop.Name] = JsonElementToString(prop.Value);
        }

        return fields;
    }

    private static string? JsonElementToString(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => element.ToString(),
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            JsonValueKind.Array => $"[{element.GetArrayLength()} items]",
            JsonValueKind.Object => "{...}",
            JsonValueKind.Null => null,
            _ => element.ToString()
        };
    }
}
