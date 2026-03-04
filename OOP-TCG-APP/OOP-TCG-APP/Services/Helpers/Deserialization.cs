using System;
using System.Text.Json;

// <summary>
// A helper class responsible for deserializing JSON from the API
// Converts the raw JSON string from the API into a ApiResponse object.
// </summary>
public static class DeserializeApi
{
    public static ApiResponse<T>? Run<T>(string json)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(json)) {
            return null;
        }
        try
        {
            // Perform deserialization and return
            return JsonSerializer.Deserialize<ApiResponse<T>>(json);
        }       
        catch (JsonException)
        {
            return null;
        }
    }
}
