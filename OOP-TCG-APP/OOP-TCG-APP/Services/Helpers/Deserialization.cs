using System;
using System.Text.Json;

// <summary>
// A helper class responsible for deserializing JSON from the API
// Converts the raw JSON string from the API into a ApiResponse object.
// </summary>
public static class ApiDeserializer
{
    public static ApiResponse DeserializeApiResponse(string json)
    {
        ApiResponse apiResponse;

        // Validate input
        if (string.IsNullOrWhiteSpace(json)) {
            return null;
        }
        try
        {
            // Perform deserialization
            apiResponse = JsonSerializer.Deserialize<ApiResponse>(json);
        }       
        catch (JsonException)
        {
            return null;
        }
        return apiResponse;
    }
}
